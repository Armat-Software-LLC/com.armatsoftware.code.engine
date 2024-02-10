using System.ComponentModel.DataAnnotations;

namespace ArmatSoftware.Code.Engine.Tester.WebApi;

public class StringOnlySubject
{
    [MinLength(20, ErrorMessage = "String must be at least 20 characters long")]
    public string Data { get; set; }
}