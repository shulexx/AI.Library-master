using Microsoft.AspNetCore.Mvc;
using System;
using AI.Library.Service.Services;

namespace AI.Library.Service.Controllers;

[ApiController]
[Route("api/ocr")]
public class OcrController : ControllerBase
{
    private readonly OcrService _ocr;

    public OcrController(OcrService ocr)
    {
        _ocr = ocr;
    }

    [HttpPost]
    public IActionResult Detect([FromBody] OcrRequest req)
    {
        var bytes = Convert.FromBase64String(req.ImageBase64);
        var result = _ocr.ExtractText(bytes);

        return Ok(new { text = result });
    }
}

public class OcrRequest
{
    public string ImageBase64 { get; set; }
}
