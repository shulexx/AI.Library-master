using AI.Library.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AI.Library.Service.Controllers;

[ApiController]
[Route("api/stt")]
public class SttController : ControllerBase
{
    private readonly SttService _svc;

    public SttController(SttService svc)
    {
        _svc = svc;
    }

    [HttpPost]
    public IActionResult Recognize([FromBody] SttRequest req)
    {
        var bytes = Convert.FromBase64String(req.AudioBase64);
        var text = _svc.Transcribe(bytes);
        return Ok(new { text });
    }
}

public class SttRequest
{
    public string AudioBase64 { get; set; }
}
