using AI.Library.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.Library.Service.Controllers;

[ApiController]
[Route("api/tts")]
public class TtsController : ControllerBase
{
    private readonly TtsService _tts;

    public TtsController(TtsService tts)
    {
        _tts = tts;
    }

    [HttpPost]
    public IActionResult Speak([FromBody] TtsRequest req)
    {
        var audio = _tts.GenerateVoice(req.Text);
        return File(audio, "audio/wav");
    }
}

public class TtsRequest
{
    public string Text { get; set; }
}
