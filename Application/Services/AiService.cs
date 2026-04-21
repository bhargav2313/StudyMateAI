using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Application.Services;

public class AiService
{
    private readonly HttpClient _httpClient;

    public AiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // 🔹 PDF process (already undhi)
    public async Task<string> ProcessPdfAsync(string filePath)
    {
        using var form = new MultipartFormDataContent();

        using var stream = File.OpenRead(filePath);
        form.Add(new StreamContent(stream), "file", Path.GetFileName(filePath));

        var response = await _httpClient.PostAsync("http://127.0.0.1:8000/process-pdf", form);

        return await response.Content.ReadAsStringAsync();
    }

    // 🔥 NEW: Ask question
    public async Task<string> AskQuestionAsync(string question)
    {
        var data = new
        {
            question = question
        };

        var json = JsonSerializer.Serialize(data);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://127.0.0.1:8000/ask", content);

        return await response.Content.ReadAsStringAsync();
    }
}