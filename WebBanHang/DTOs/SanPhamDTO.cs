namespace WebBanHang.DTOs
{
    public class SanPhamDTO
    {
        public string TenSP { get; set; }
        public string HinhAnh { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public DanhMucDTO DanhMuc { get; set; }
    }
}
