using System.Diagnostics;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Tester.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArmatSoftware.Code.Engine.Tester.WebApi;

[ApiController]
[Route("api/[controller]")]
public class CodeEngineController : ControllerBase
{
    [HttpGet, Route("execute_default")]
    public async Task<IActionResult> Execute_Default(IExecutor<StringOnlySubject> executor, CancellationToken token)
    {
        executor.Execute(new StringOnlySubject());
        
        return await Task.FromResult(new OkObjectResult(executor.Subject.Data));
    }
    
    [HttpGet, Route("execute_with_key")]
    public async Task<IActionResult> Execute_Key(string key, IExecutorCatalog<StringOnlySubject> catalog, CancellationToken token)
    {
        var executor = catalog.ForKey(key);
        
        executor.Execute(new StringOnlySubject());
        
        return await Task.FromResult(new OkObjectResult(executor.Subject.Data));
    }
    
    [HttpGet, Route("list")]
    public async Task<IActionResult> List(IActionRepository repo, CancellationToken token)
    {
        var result = repo.GetActions<StringOnlySubject>();
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpGet, Route("list_with_key")]
    public async Task<IActionResult> List_Key(string key, IActionRepository repo, CancellationToken token)
    {
        var result = repo.GetActions<StringOnlySubject>(key);
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpPost, Route("create")]
    public async Task<IActionResult> Create([FromBody] ActionUpdatePostModel codeUpdateModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + codeUpdateModel);

        repo.AddAction<StringOnlySubject>(codeUpdateModel.ActionName, codeUpdateModel.Code, codeUpdateModel.Author, codeUpdateModel.Comment, codeUpdateModel.Key);
        
        var result = repo.GetActions<StringOnlySubject>(codeUpdateModel.Key);
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpPost, Route("update")]
    public async Task<IActionResult> Update([FromBody] ActionUpdatePostModel codeUpdateModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + codeUpdateModel);

        repo.UpdateAction<StringOnlySubject>(codeUpdateModel.ActionName, codeUpdateModel.Code, codeUpdateModel.Author, codeUpdateModel.Comment, codeUpdateModel.Key);

        var result = repo.GetActions<StringOnlySubject>(codeUpdateModel.Key);
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpPost, Route("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ActionReorderPostModel actionReorderPostModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + actionReorderPostModel);

        repo.ReorderAction<StringOnlySubject>(actionReorderPostModel.ActionName, actionReorderPostModel.NewOrder, actionReorderPostModel.Key);

        var result = repo.GetActions<StringOnlySubject>(actionReorderPostModel.Key);
        
        return await Task.FromResult(new OkObjectResult(result));
    }
    
    [HttpPost, Route("activate")]
    public async Task<IActionResult> Activate([FromBody] RevisionActivationPostModel actionActivatePostModel, IActionRepository repo,  CancellationToken token)
    {
        Debug.WriteLine("Code: " + actionActivatePostModel);

        repo.ActivateRevision<StringOnlySubject>(actionActivatePostModel.ActionName, actionActivatePostModel.RevisionId, actionActivatePostModel.Key);

        var result = repo.GetActions<StringOnlySubject>(actionActivatePostModel.Key);
        
        return await Task.FromResult(new OkObjectResult(result));
    }
}