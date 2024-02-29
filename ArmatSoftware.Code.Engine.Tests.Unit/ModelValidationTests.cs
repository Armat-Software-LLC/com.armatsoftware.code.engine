using System.ComponentModel.DataAnnotations;
using ArmatSoftware.Code.Engine.Compiler;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit;

[TestFixture, TestOf(typeof(CSharpCompiler<>)), TestOf(typeof(VbCompiler<>))]
public class ModelValidationTests : ModelValidationTestBase<TestSubject>
{
   [Test]
   public void Should_Fail_Validate_Invalid_Model_IfEnabled_CSharp()
   {
       Assert.That(() =>
       {
           Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\";" });
           Configuration.ValidateModelsAfterExecution = true;
           
           var executor = BuildCSharpCompiler();
           
           executor.Execute(new TestSubject());
       }, Throws.TypeOf<ValidationException>());
   }
   
   [Test]
   public void Should_Skip_Validate_Invalid_Model_IfDisabled_CSharp()
   {
       Assert.That(() =>
       {
           Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\";" });
           Configuration.ValidateModelsAfterExecution = false;
           
           var executor = BuildCSharpCompiler();
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
   
   [Test]
   public void Should_Validate_Valid_Model_CSharp()
   {
       Assert.That(() =>
       {
           Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"long enough string value\"; \r\n Subject.Id = 1;" });
           Configuration.ValidateModelsAfterExecution = true;
           
           var executor = BuildCSharpCompiler();
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
   
   [Test]
   public void Should_Fail_Validate_Invalid_Model_IfEnabled_Vb()
   {
       Assert.That(() =>
       {
           Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\"" });
           Configuration.ValidateModelsAfterExecution = true;
           
           var executor = BuildVbCompiler();
           
           executor.Execute(new TestSubject());
       }, Throws.TypeOf<ValidationException>());
   }
   
   [Test]
   public void Should_Skip_Validate_Invalid_Model_IfDisabled_Vb()
   {
       Assert.That(() =>
       {
           Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"short string\"" });
           Configuration.ValidateModelsAfterExecution = false;
           
           var executor = BuildVbCompiler();
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
   
   [Test]
   public void Should_Validate_Valid_Model_Vb()
   {
       Assert.That(() =>
       {
           Configuration.Actions.Add(new TestSubjectAction<TestSubject> { Name = "ValidateThisModel", Code = "Subject.Data = \"long enough string value\" \r\n Subject.Id = 1" });
           Configuration.ValidateModelsAfterExecution = true;
           
           var executor = BuildVbCompiler();
           
           executor.Execute(new TestSubject());
       }, Throws.Nothing);
   }
}

public class ModelValidationTestBase<TSubject> where TSubject: class
{
    public ICompilerConfiguration<TSubject> Configuration { get; private set; }

    [SetUp]
    public void Init()
    {
        Configuration = new CompilerConfiguration<TSubject>("ArmatSoftware.Code.Engine.Tests.Unit");
    }

    protected IExecutor<TSubject> BuildCSharpCompiler()
    {
        var compiler = new CSharpCompiler<TSubject>();
        
        return compiler.Compile(Configuration);
    }
    
    protected IExecutor<TSubject> BuildVbCompiler()
    {
        var compiler = new VbCompiler<TSubject>();
        
        return compiler.Compile(Configuration);
    }
}
