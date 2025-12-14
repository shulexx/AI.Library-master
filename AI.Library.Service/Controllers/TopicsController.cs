using AI.Library.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AI.Library.Service.Controllers;

[ApiController]
[Route("api/topics")]
public class TopicsController : ControllerBase
{
    private readonly EmbedService _embed;

    public TopicsController(EmbedService embed)
    {
        _embed = embed;
    }

    [HttpGet]
    public IActionResult GetTopics()
    {
        var topics = _embed.Books
            .Where(b => b.Keywords != null)
            .SelectMany(b => b.Keywords)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        return Ok(topics);
    }
}
