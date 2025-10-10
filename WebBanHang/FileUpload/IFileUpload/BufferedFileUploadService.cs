


using Microsoft.AspNetCore.Http;

namespace WebBanHang.FileUpload.IFileUpload
{
    public class BufferedFileUploadService : IBufferedFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        public BufferedFileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    // đường dẫn thư mục wwwroot/UploadedFiles
                    string uploadPath = Path.Combine(_env.WebRootPath, "ImgAvatarStudent");
                    // tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // đường dẫn file đầy đủ
                    string filePath = Path.Combine(uploadPath, file.FileName);
                    // lưu đường dẫn file để trả về
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // trả về đường dẫn tương đối để hiển thị ảnh
                    return $"/ImgAvatarStudent/{file.FileName}";
                }

                return null!;
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }

        
    }
}
