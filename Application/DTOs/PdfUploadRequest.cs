namespace Application.DTOs;

public class PdfUploadRequest
{
    public byte[] FileContent { get; set; }
    public string FileName { get; set; }
}