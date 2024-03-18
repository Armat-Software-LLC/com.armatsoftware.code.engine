using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit;

[TestFixture, TestOf(typeof(CSharpCompiler<>)), TestOf(typeof(VbCompiler<>))]
internal class ModelValidationTests : ModelValidationTestBase<TestSubject>
{
   [Test]
   public void Should_Fail_Validate_Invalid_Model_IfEnabled_CSharp()
   {
       Assert.That(() =>
       {
           RegistrationOptions.ValidateModelsAfterExecution = true;
           RegistrationOptions.CompilerType = CompilerTypeEnum.CSharp;
           var executor =  BuildAndCompile(new List<ISubjectAction<TestSubject>>()
           {
               new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\";" }
           });
           
           executor.Execute(new TestSubject());
       }, Throws.TypeOf<ValidationException>());
   }
   
   [Test]
   public void Should_Skip_Validate_Invalid_Model_IfDisabled_CSharp()
   {
       Assert.That(() =>
       {
           RegistrationOptions.ValidateModelsAfterExecution = false;
           RegistrationOptions.CompilerType = CompilerTypeEnum.CSharp;
           var executor =  BuildAndCompile(new List<ISubjectAction<TestSubject>>()
           {
               new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\";" }
           });
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
   
   [Test]
   public void Should_Validate_Valid_Model_CSharp()
   {
       Assert.That(() =>
       {
           RegistrationOptions.ValidateModelsAfterExecution = true;
           RegistrationOptions.CompilerType = CompilerTypeEnum.CSharp;
           var executor =  BuildAndCompile(new List<ISubjectAction<TestSubject>>()
           {
               new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"long enough string value\"; \r\n Subject.Id = 1;" }
           });
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
   
   [Test]
   public void Should_Fail_Validate_Invalid_Model_IfEnabled_Vb()
   {
       Assert.That(() =>
       {
           RegistrationOptions.ValidateModelsAfterExecution = true;
           RegistrationOptions.CompilerType = CompilerTypeEnum.Vb;
           var executor =  BuildAndCompile(new List<ISubjectAction<TestSubject>>()
           {
               new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\"" }
           });
           
           executor.Execute(new TestSubject());
       }, Throws.TypeOf<ValidationException>());
   }
   
   [Test]
   public void Should_Skip_Validate_Invalid_Model_IfDisabled_Vb()
   {
       Assert.That(() =>
       {
           RegistrationOptions.ValidateModelsAfterExecution = false;
           RegistrationOptions.CompilerType = CompilerTypeEnum.Vb;
           var executor =  BuildAndCompile(new List<ISubjectAction<TestSubject>>()
           {
               new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\"" }
           });
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
   
   [Test]
   public void Should_Validate_Valid_Model_Vb()
   {
       Assert.That(() =>
       {
           RegistrationOptions.ValidateModelsAfterExecution = true;
           RegistrationOptions.CompilerType = CompilerTypeEnum.Vb;
           var executor =  BuildAndCompile(new List<ISubjectAction<TestSubject>>()
           {
               new TestSubjectAction<TestSubject>
               {
                   Name = "ValidateThisModel", Code = "Subject.Data = \"long enough string value\" \r\n Subject.Id = 1"
               }
           });
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
}

internal class ModelValidationTestBase<TSubject> : ExecutorBuilderBase<TSubject>
    where TSubject: class, new()
{
    [SetUp]
    public void Init()
    {
        base.Init();
    }

    protected IExecutor<TSubject> BuildCSharpCompiler2()
    {
        RegistrationOptions.CompilerType = CompilerTypeEnum.CSharp;
        return BuildAndCompile(new List<ISubjectAction<TSubject>>());
    }
    
    protected IExecutor<TSubject> BuildVbCompiler2()
    {
        RegistrationOptions.CompilerType = CompilerTypeEnum.Vb;
        return BuildAndCompile(new List<ISubjectAction<TSubject>>());
    }
}
