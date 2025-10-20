


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

        public async Task DeleteFileAsync(string relativePath)
        {
            try
            {
                // 1. Kiểm tra xem đường dẫn có hợp lệ không
                if (string.IsNullOrEmpty(relativePath))
                {
                    // Không có gì để xóa, thoát ra
                    return;
                }

                // 2. Chuyển đổi đường dẫn web tương đối thành đường dẫn vật lý đầy đủ
                // Ví dụ: "/ImgAvatarStudent/abc.jpg" -> "C:\project\wwwroot\ImgAvatarStudent\abc.jpg"
                // TrimStart('/') để loại bỏ dấu / ở đầu, tránh lỗi khi Path.Combine
                var physicalPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

                // 3. Kiểm tra xem file có thực sự tồn tại trên server không
                if (File.Exists(physicalPath))
                {
                    // 4. Nếu có, thực hiện xóa file
                    await Task.Run(() => File.Delete(physicalPath));
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi thay vì ném ra ngoài để không làm gián đoạn quá trình xóa user
                // Hoặc bạn có thể ném exception nếu muốn xử lý ở cấp cao hơn
                Console.WriteLine($"Error deleting file: {relativePath}. Exception: {ex.Message}");
            }
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
