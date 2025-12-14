using AI.Library.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AI.Library.Service.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly RagService _rag;

    public SearchController(RagService rag)
    {
        _rag = rag;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest req)
    {
        return Ok(await _rag.Search(req.Query));
    }
}

public class SearchRequest
{
    public string Query { get; set; }
}
