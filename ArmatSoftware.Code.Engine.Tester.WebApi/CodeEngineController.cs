using ArmatSoftware.Code.Engine.Core;
using Microsoft.AspNetCore.Mvc;

namespace ArmatSoftware.Code.Engine.Tester.WebApi;

[ApiController]
[Route("api/[controller]")]
public class CodeEngineController : ControllerBase
{
    private IExecutor<TestPayload> _executor;
    
    public CodeEngineController(IExecutor<TestPayload> executor)
    {
        _executor = executor;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken token)
    {
        _executor.Subject = new TestPayload { Data = "bye, world!" };
        _executor.Execute();
        
        return await Task.FromResult(new OkObjectResult(_executor.Subject.Data));
    }
}