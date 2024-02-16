using System;
using ArmatSoftware.Code.Engine.Compiler;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
	[TestFixture, TestOf(typeof(CSharpCompiler<>))]
	public class CSharpCompilerTests : CSharpCompilerTestBase<TestSubject>
	{
		[SetUp]
		public void Setup()
		{
			Build();
		}

		[Test]
		public void Should_Execute_Simple_Operation()
		{
			Assert.That(() =>
			{
				Configuration.Actions.Add(new TestSubjectAction { Name = "SimpleOperation", Code = "var t = 3 + 4;" });

				var executor = Compiler.Compile(Configuration);

				executor.Execute();

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Execute_Subject_Property_Assignment()
		{
			Assert.That(() =>
			{
				var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
				var action = new TestSubjectAction { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\";" };
				
				Configuration.Actions.Add(action);
				
				var executor = Compiler.Compile(Configuration);
				
				executor.Execute(subject);

				Assert.IsTrue(subject.Data == "changed data");

			}, Throws.Nothing);
		}
		
		[Test]
		public void Should_Execute_Simplified_Subject_Property_Assignment()
		{
			Assert.That(() =>
			{
				var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
				var action = new TestSubjectAction { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\";" };
				
				Configuration.Actions.Add(action);
				
				var executor = Compiler.Compile(Configuration);
				
				var result = executor.Execute(subject);

				Assert.IsTrue(result.Data == "changed data");
				Assert.IsTrue(subject.Equals(result));

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Execute_Subject_Property_Reading()
		{
			Assert.That(() =>
			{
				var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
				var action = new TestSubjectAction { Name = "ReadSubject", Code = "var temp = Subject.Data;" };

				Configuration.Actions.Add(action);

				var executor = Compiler.Compile(Configuration);

				executor.Execute(subject);

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Get_And_Set_Runtime_Value()
		{
			var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
			var action = new TestSubjectAction { Name = "SetValue", Code = "Save(\"key\", \"test value\");" };
			var action2 = new TestSubjectAction { Name = "GetValue", Code = "Subject.Data = Read(\"key\");" };

			Configuration.Actions.Add(action);
			Configuration.Actions.Add(action2);

			var executor = Compiler.Compile(Configuration);

			executor.Execute(subject);

			Assert.IsTrue(executor.Subject.Data == "test value");
		}
	}

	public class CSharpCompilerTestBase<S> where S : class
	{
		public ICompilerConfiguration<S> Configuration { get; set; }

		public ICompiler<S> Compiler { get; set; }

		public void Build()
		{
			Configuration = new CompilerConfiguration<S>("ArmatSoftware.Code.Engine.Tests.Executors");

			Compiler = new CSharpCompiler<S>();
		}
	}
}