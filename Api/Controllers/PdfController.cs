using Microsoft.AspNetCore.Http;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly IPdfService _pdfService;
    private readonly AiService _aiService;

    public PdfController(IPdfService pdfService, AiService aiService)
    {
        _pdfService = pdfService;
        _aiService = aiService;
    }

    // 🔹 Upload only
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var fileBytes = ms.ToArray();

        var path = await _pdfService.UploadPdfAsync(fileBytes, file.FileName);

        return Ok(new { filePath = path });
    }

    //  Upload + AI processing
    [HttpPost("process")]
    public async Task<IActionResult> Process(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var fileBytes = ms.ToArray();

        // 1. Save PDF
        var path = await _pdfService.UploadPdfAsync(fileBytes, file.FileName);

        // 2. Send to Python AI
        var result = await _aiService.ProcessPdfAsync(path);

        return Ok(result);
    }

    // 🔥 Ask question (RAG + LLM)
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest("Question is required");

        var result = await _aiService.AskQuestionAsync(request.Question);

        return Ok(result);
    }
}