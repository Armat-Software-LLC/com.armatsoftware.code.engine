using System.Diagnostics;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Storage.File;
using Microsoft.AspNetCore.Mvc;

namespace ArmatSoftware.Code.Engine.Tester.WebApi;

[ApiController]
[Route("api/[controller]")]
public class CodeEngineController : ControllerBase
{
    [HttpGet, Route("get1")]
    public async Task<IActionResult> GetTestPayload(IExecutor<StringOnlySubject> executor, CancellationToken token)
    {
        executor.Subject = new StringOnlySubject();
        executor.Execute();
        
        return await Task.FromResult(new OkObjectResult(executor.Subject.Data));
    }
    
    [HttpGet, Route("get2")]
    public async Task<IActionResult> GetTestPayload2(IExecutor<NumericAndStringSubject> executor, CancellationToken token)
    {
        executor.Subject = new NumericAndStringSubject();
        executor.Execute();
        
        return await Task.FromResult(new OkObjectResult(executor.Subject.NumericData + " " + executor.Subject.StringData));
    }
    
    [HttpPost, Route("fail")]
    public async Task<IActionResult> Fail([FromBody] StringOnlySubject subject, IExecutor<NumericAndStringSubject> executor, CancellationToken token)
    {
        if (ModelState.TryAddModelError("Subject", "Subject is required"))
        {
            Debug.WriteLine("Added fail state to model");
        }
        return await Task.FromResult(new OkResult());
    }
    
    [HttpPost, Route("create")]
    public async Task<IActionResult> Fail([FromBody] string code,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + code);

        StoredActions<StringOnlySubject> actions = new();
        
        var action = actions.Add("StringAction");
        action.Update(code, "author", "comment");
        action.Activate(1);
        
        
        
        return await Task.FromResult(new OkResult());
    }
}