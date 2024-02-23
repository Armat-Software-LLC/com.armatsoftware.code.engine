using System;
using System.ComponentModel.DataAnnotations;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
	public class TestSubject
	{
		[Required]
		[Range(1, int.MaxValue)]
		public int Id { get; set; }

		[Required]
		[MinLength(20)]
		public string Data { get; set; }

		public DateTime Date { get; set; }
	}
}
