namespace ArmatSoftware.Code.Engine.Tester.WebApi.Models;

public class ActionReorderPostModel
{
    public string? Key { get; set; }
    
    public string? ActionName { get; set; }
    
    public int NewOrder { get; set; }
}