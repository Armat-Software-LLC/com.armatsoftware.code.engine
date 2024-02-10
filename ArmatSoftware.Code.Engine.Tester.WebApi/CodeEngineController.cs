using System.Diagnostics;
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
    public async Task<IActionResult> Create([FromBody] ActionUpdatePostModel codeUpdateModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + codeUpdateModel);

        repo.AddAction<StringOnlySubject>(codeUpdateModel.ActionName, codeUpdateModel.Code, codeUpdateModel.Author, codeUpdateModel.Comment);
        
        var result = repo.GetActions<StringOnlySubject>();
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpPost, Route("update")]
    public async Task<IActionResult> Update([FromBody] ActionUpdatePostModel codeUpdateModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + codeUpdateModel);

        repo.UpdateAction<StringOnlySubject>(codeUpdateModel.ActionName, codeUpdateModel.Code, codeUpdateModel.Author, codeUpdateModel.Comment);

        var result = repo.GetActions<StringOnlySubject>();
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpPost, Route("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ActionReorderPostModel actionReorderPostModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + actionReorderPostModel);

        repo.ReorderAction<StringOnlySubject>(actionReorderPostModel.ActionName, actionReorderPostModel.NewOrder);

        var result = repo.GetActions<StringOnlySubject>();
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpPost, Route("activate")]
    public async Task<IActionResult> Activate([FromBody] RevisionActivationPostModel actionActivatePostModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + actionActivatePostModel);

        repo.ActivateRevision<StringOnlySubject>(actionActivatePostModel.ActionName, actionActivatePostModel.RevisionId);

        var result = repo.GetActions<StringOnlySubject>();
        
        return await Task.FromResult(new OkObjectResult(result));
    }
}