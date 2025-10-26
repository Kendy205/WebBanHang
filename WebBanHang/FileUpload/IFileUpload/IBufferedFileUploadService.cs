
using Microsoft.AspNetCore.Http;
namespace WebBanHang.FileUpload.IFileUpload
{
    public interface IBufferedFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string relativePath);
    }
}
