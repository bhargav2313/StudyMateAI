using System.Net.Http;

namespace Application.Services;

public class AiService
{
    private readonly HttpClient _httpClient;

    public AiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ProcessPdfAsync(string filePath)
    {
        using var form = new MultipartFormDataContent();

        using var stream = File.OpenRead(filePath);
        form.Add(new StreamContent(stream), "file", Path.GetFileName(filePath));

        var response = await _httpClient.PostAsync("http://127.0.0.1:8000/process-pdf", form);

        return await response.Content.ReadAsStringAsync();
    }
}