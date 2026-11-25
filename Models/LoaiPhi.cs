
namespace QuanLyChungCu_SkyResidence.Models;

public class LoaiPhi
{
    public string MaLoaiPhi { get; set; } = "";
    public string TenPhi { get; set; } = "";
    public string DonViTinh { get; set; } = "";
    public decimal DonGia { get; set; }
    public string CachTinh { get; set; } = "";
    public string ChuKy { get; set; } = "";
    public bool HoatDong { get; set; } = true;

    // Các thuộc tính dùng để hiển thị trên bảng
    public string DonGiaHienThi => DonGia.ToString("#,##0.##") + " đ";

    public string TrangThai => HoatDong ? "Đang áp dụng" : "Ngừng áp dụng";
}
