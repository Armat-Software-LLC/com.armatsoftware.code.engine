using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
	public class TestSubjectAction<TSubject> : ISubjectAction<TSubject>
		where TSubject : class
	{
		public string Name { get; set; }

		public string Code { get; set; }

		public int Order { get; set; }
	}
}
