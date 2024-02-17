namespace ArmatSoftware.Code.Engine.Compiler
{
	public interface ITemplateGenerator<TSubject> where TSubject: class
	{
		string Generate(ICompilerConfiguration<TSubject> configuration);
	}
}
