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
				Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "var t = 3 + 4;" });

				var executor = Compiler.Compile(Configuration);

				executor.Execute(null);

			}, Throws.Nothing);
		}
		
		[Test]
		public void Should_Not_Set_Subject()
		{
			Assert.That(() =>
			{
				Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Subject = null;" });

				var executor = Compiler.Compile(Configuration);

				executor.Execute(new TestSubject());

			}, Throws.InvalidOperationException);
		}

		[Test]
		public void Should_Execute_Subject_Property_Assignment()
		{
			Assert.That(() =>
			{
				var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
				var action = new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\";" };
				
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
				var action = new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\";" };
				
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
				var action = new TestSubjectAction<TestSubject> { Name = "ReadSubject", Code = "var temp = Subject.Data;" };

				Configuration.Actions.Add(action);

				var executor = Compiler.Compile(Configuration);

				executor.Execute(subject);

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Get_And_Set_Runtime_Value()
		{
			var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
			var action = new TestSubjectAction<TestSubject> { Name = "SetValue", Code = "Save(\"key\", \"test value\");" };
			var action2 = new TestSubjectAction<TestSubject> { Name = "GetValue", Code = "Subject.Data = Read(\"key\");" };

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
				var testSubjectAction = new TestSubjectAction<TestSubject> { Name = "TestHttpClient", Code = "new HttpClient();" };
				Configuration.Actions.Add(testSubjectAction);
				var executor = Compiler.Compile(Configuration);
				executor.Execute(new TestSubject());
			}, Throws.InvalidOperationException);
		}
	}

	public class CSharpCompilerTestBase<TSubject> where TSubject: class
	{
		public ICompilerConfiguration<TSubject> Configuration { get; set; }

		public ICompiler<TSubject> Compiler { get; set; }

		public void Build()
		{
			Configuration = new CompilerConfiguration<TSubject>("ArmatSoftware.Code.Engine.Tests.Executors");

			Compiler = new CSharpCompiler<TSubject>();
		}
	}
}