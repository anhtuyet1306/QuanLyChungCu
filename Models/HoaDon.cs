
using System;

namespace QuanLyChungCu_SkyResidence.Models;

public class HoaDon
{
    public string MaHoaDon { get; set; } = "";
    public string MaCH { get; set; } = "";
    public string KyThanhToan { get; set; } = "";
    public byte Thang { get; set; }
    public short Nam { get; set; }
    public DateTime? NgayLap { get; set; }
    public DateTime? HanThanhToan { get; set; }
    public decimal TongTien { get; set; }
    public decimal DaThu { get; set; }
    public decimal ConNo { get; set; }
    public DateTime? NgayThanhToanCuoi { get; set; }
    public string TrangThai { get; set; } = "";
    public int QuaHan { get; set; }
    public string? MaNV { get; set; }
    public string? GhiChu { get; set; }

    public string TongTienHienThi => TongTien.ToString("N0") + " đ";
    public string DaThuHienThi => DaThu.ToString("N0") + " đ";
    public string ConNoHienThi => ConNo.ToString("N0") + " đ";

    public string NgayThanhToanHienThi =>
        NgayThanhToanCuoi?.ToString("dd/MM/yyyy") ?? "Chưa thanh toán";
}
