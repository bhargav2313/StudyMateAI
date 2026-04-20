
namespace Application.Services;

public class PdfService : IPdfService
{
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    public async Task<string> UploadPdfAsync(byte[] fileContent, string fileName)
    {
        if (fileContent == null || fileContent.Length == 0)
            throw new Exception("File is empty");

        if (!fileName.EndsWith(".pdf"))
            throw new Exception("Only PDF files allowed");

        if (fileContent.Length > 10 * 1024 * 1024)
            throw new Exception("File too large");

        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);

        var filePath = Path.Combine(_uploadPath, Guid.NewGuid() + "_" + fileName);

        await File.WriteAllBytesAsync(filePath, fileContent);

        return filePath;
    }
}