using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
	public class TestSubjectAction : ISubjectAction<TestSubject>
	{
		public string Name { get; set; }

		public string Code { get; set; }
		
		public int Order { get; set; }
	}
}
