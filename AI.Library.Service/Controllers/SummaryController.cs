using AI.Library.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AI.Library.Service.Controllers;

[ApiController]
[Route("api/summary")]
public class SummaryController : ControllerBase
{
    private readonly BookSummaryService _svc;

    public SummaryController(BookSummaryService svc)
    {
        _svc = svc;
    }

    [HttpPost]
    public async Task<IActionResult> Summary([FromBody] SummaryRequest req)
    {
        var result = await _svc.Summarize(req.Text);
        return Ok(new { summary = result });
    }
}

public class SummaryRequest
{
    public string Text { get; set; }
}
