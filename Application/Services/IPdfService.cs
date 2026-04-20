

namespace Application.Services;

public interface IPdfService
{
    Task<string> UploadPdfAsync(byte[] fileContent, string fileName);
}