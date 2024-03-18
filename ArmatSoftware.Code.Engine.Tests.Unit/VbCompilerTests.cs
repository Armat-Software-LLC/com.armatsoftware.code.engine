using System;
using ArmatSoftware.Code.Engine.Compiler;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Compiler.Execution;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
	[TestFixture, TestOf(typeof(VbCompiler<>))]
	internal class VbCompilerTests : VbCompilerTestBase<TestSubject>
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

				var executor = BuildAndCompile(new ISubjectAction<TestSubject>[]
				{
					new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Dim t = 3 + 4" }
				});
				
				executor.Execute(null);

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Execute_Subject_Property_Assignment()
		{
			Assert.That(() =>
			{
				var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
				var executor = BuildAndCompile(new ISubjectAction<TestSubject>[]
				{
					new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\"", Order = 1}
				});
				
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
				var executor = BuildAndCompile(new ISubjectAction<TestSubject>[]
				{
					new TestSubjectAction<TestSubject> { Name = "SimpleOperation", Code = "Subject.Data = \"changed data\"", Order = 1}
				});
				
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
				var executor = BuildAndCompile(new ISubjectAction<TestSubject>[]
				{
					new TestSubjectAction<TestSubject> { Name = "ReadSubject", Code = "Dim temp = Subject.Data" }
				});

				executor.Execute(subject);

			}, Throws.Nothing);
		}

		[Test]
		public void Should_Get_And_Set_Runtime_Value()
		{
			var subject = new TestSubject { Id = 1, Data = "data", Date = DateTime.Now };
			var action = new TestSubjectAction<TestSubject> { Name = "SetValue", Code = "Save(\"key\", \"test value\")" };
			var action2 = new TestSubjectAction<TestSubject> { Name = "GetValue", Code = "Subject.Data = System.Convert.ToString(Read(\"key\"))" };
			var executor = BuildAndCompile(new ISubjectAction<TestSubject>[]
			{
				action, action2
			});

			executor.Execute(subject);

			Assert.IsTrue(executor.Subject.Data == "test value");
		}
		
		[Test]
		public void Should_Not_Execute_Unforeseen_Operation()
		{
			Assert.That(() =>
			{
				var testSubjectAction = new TestSubjectAction<TestSubject> { Name = "TestHttpClient", Code = "New HttpClient()" };
				var executor = BuildAndCompile(new ISubjectAction<TestSubject>[]
				{
					new TestSubjectAction<TestSubject> { Name = "TestHttpClient", Code = "New HttpClient()" }
				});
				executor.Execute(new TestSubject());
			}, Throws.InvalidOperationException);
		}
	}

	internal class VbCompilerTestBase<TSubject> : ExecutorBuilderBase<TSubject>
		where TSubject: class, new()
	{
		public void Build()
		{
			Init();
			RegistrationOptions.CompilerType = CompilerTypeEnum.Vb;
		}
	}
}