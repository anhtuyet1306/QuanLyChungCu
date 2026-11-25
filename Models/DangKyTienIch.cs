
namespace QuanLyChungCu_SkyResidence.Models;

public class DangKyTienIch
{
    public string MaDK { get; set; } = "";
    public string MaTI { get; set; } = "";
    public string TenTI { get; set; } = "";
    public string MaCD { get; set; } = "";
    public string HoTen { get; set; } = "";

    public DateTime NgaySuDung { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }

    public byte SoNguoi { get; set; }
    public string TrangThai { get; set; } = "Chờ duyệt";
    public DateTime NgayDangKy { get; set; }

    public string? MaNVDuyet { get; set; }
    public string? LyDoTuChoi { get; set; }

    public string KhungGio =>
        $"{GioBatDau:hh\\:mm} - {GioKetThuc:hh\\:mm}";

    public string NgaySuDungHienThi =>
        NgaySuDung.ToString("dd/MM/yyyy");
}
