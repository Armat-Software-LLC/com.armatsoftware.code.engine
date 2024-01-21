using ArmatSoftware.Code.Engine.Core;
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
}