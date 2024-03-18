using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using ArmatSoftware.Code.Engine.Compiler.Utils;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.CSharp
{
	/// <summary>
	/// Default implementation for the C# compiler
	/// </summary>
	/// <typeparam name="TSubject">Subject type</typeparam>
	public class CSharpCompiler<TSubject> : ICompiler<TSubject> where TSubject: class
	{
		/// <summary>
		/// Compile the actions using C# compiler.
		/// Important: Essential references for subject and executor types are added to the configuration automagically
		/// </summary>
		/// <param name="configuration">Compiler configuration</param>
		/// <returns>Executor object</returns>
		public IFactoryExecutor<TSubject> Compile(ICompilerConfiguration<TSubject> configuration)
		{
			ValidateConfiguration(configuration);

			AddTemplateReferences(configuration);

			// generate code
			var codeGenerator = new CSharpExecutorTemplate(configuration);
			var code = codeGenerator.TransformText();
			
			var typeName = Names.GenerateFullTypeName(configuration);

			SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(text: code);

			Compilation compilation = CSharpCompilation.Create(
				assemblyName: Names.GenerateUniqueAssemblyName(),
				syntaxTrees: new[] { syntaxTree },
				references: GenerateRequiredReferences(configuration),
				options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
			);

			// Emit the image of this assembly 
			byte[] image = null;
			using (var ms = new MemoryStream())
			{
				var emitResult = compilation.Emit(ms);
				ValidateCompilationResults(emitResult);
				image = ms.ToArray();
			}

			using var stream = new MemoryStream(image);

			var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);

			var executorInstanceHandle = Activator.CreateInstance(assembly.FullName, typeName) ?? 
			                             throw new ApplicationException($"Could not instantiate {typeName}");

			return (IFactoryExecutor<TSubject>)executorInstanceHandle.Unwrap();
		}

		/// <summary>
		/// Generate a full list of references necessary for compilation
		/// </summary>
		/// <param name="configuration"></param>
		/// <returns></returns>
		private static IEnumerable<MetadataReference> GenerateRequiredReferences(ICompilerConfiguration<TSubject> configuration)
		{
			var references = new List<MetadataReference>();

			// adding references supplied by the user fo executor
			references.AddRange(configuration.References.Select(refType => MetadataReference.CreateFromFile(refType.Assembly.Location)));

			// adding required references to compile successfully
			references.AddRange(new List<MetadataReference>
			{
				MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp")).Location),
				MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
				MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
			});
			return references;
		}

		private static void ValidateCompilationResults(EmitResult emitResult)
		{
			if (!emitResult.Success)
			{
				var errorsBuffer = new StringBuilder().AppendLine();
				errorsBuffer.AppendJoin(
					Environment.NewLine,
					emitResult.Diagnostics
					.OrderByDescending(entry => entry.Severity)
					.Select(entry => $"{entry.Severity} {entry.Id}: {entry.GetMessage()} at {entry.Location.GetMappedLineSpan()}")); // in {entry.Location.SourceTree}"));
				throw new InvalidOperationException(errorsBuffer.ToString());
			}
		}

		/// <summary>
		/// Add template specific type references
		/// </summary>
		/// <param name="configuration"></param>
		private static void AddTemplateReferences(ICompilerConfiguration<TSubject> configuration)
		{
			configuration.References.Add(typeof(Dictionary<,>));
			configuration.References.Add(typeof(TSubject));
			configuration.References.Add(typeof(IExecutor<>));
			configuration.References.Add(typeof(ILogger));
			configuration.References.Add(typeof(DynamicAttribute));
			configuration.References.Add(typeof(LogContext));
			
			if (configuration.EnableModelValidation())
			{
				configuration.References.Add(typeof(ObjectValidator));
			}
		}

		private static void ValidateConfiguration(ICompilerConfiguration<TSubject> configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			if (configuration.Actions == null)
			{
				throw new ArgumentException("Actions are null", nameof(configuration.Actions));
			}

			if (configuration.References == null)
			{
				throw new ArgumentNullException(nameof(configuration.References));
			}
		}

	}
}
