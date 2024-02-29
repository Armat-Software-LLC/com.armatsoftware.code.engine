using System;
using ArmatSoftware.Code.Engine.Compiler;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
	[TestFixture, TestOf(typeof(VbCompiler<>))]
	public class VbCompilerTests : VbCompilerTestBase<TestSubject>
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
				Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Dim t = 3 + 4" });

				var executor = Compiler.Compile(Configuration);

				executor.Execute(null);

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Execute_Subject_Property_Assignment()
		{
			Assert.That(() =>
			{
				var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
				var action = new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\"", Order = 1};
				
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
				var action = new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\"", Order = 1};
				
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
				var action = new TestSubjectAction<TestSubject> { Name = "ReadSubject", Code = "Dim temp = Subject.Data" };

				Configuration.Actions.Add(action);

				var executor = Compiler.Compile(Configuration);

				executor.Execute(subject);

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Get_And_Set_Runtime_Value()
		{
			var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
			var action = new TestSubjectAction<TestSubject> { Name = "SetValue", Code = "Save(\"key\", \"test value\")" };
			// difference from the c# syntax for the object to string conversion is due to using object type versus dynamic
			var action2 = new TestSubjectAction<TestSubject> { Name = "GetValue", Code = "Subject.Data = System.Convert.ToString(Read(\"key\"))" };

			Configuration.Actions.Add(action);
			Configuration.Actions.Add(action2);

			var executor = Compiler.Compile(Configuration);

			executor.Execute(subject);

			Assert.IsTrue(executor.Subject.Data == "test value");
		}
		
		[Test]
		public void Should_Not_Execute_Unforeseen_Operation()
		{
			Assert.That(() =>
			{
				var testSubjectAction = new TestSubjectAction<TestSubject> { Name = "TestHttpClient", Code = "New HttpClient()" };
				Configuration.Actions.Add(testSubjectAction);
				var executor = Compiler.Compile(Configuration);
				executor.Execute(new TestSubject());
			}, Throws.InvalidOperationException);
		}
	}

	public class VbCompilerTestBase<TSubject> where TSubject: class
	{
		public ICompilerConfiguration<TSubject> Configuration { get; set; }

		public ICompiler<TSubject> Compiler { get; set; }

		public void Build()
		{
			Configuration = new CompilerConfiguration<TSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
			
			Compiler = new VbCompiler<TSubject>();
		}
	}
}