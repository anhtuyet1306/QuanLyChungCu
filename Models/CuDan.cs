
namespace QuanLyChungCu_SkyResidence.Models;

public class CuDan
{
    // Các trạng thái cư trú thống nhất với SQL Server
    public const string ThuongTru = "Thường trú";
    public const string TamTru = "Tạm trú";
    public const string TamVang = "Tạm vắng";
    public const string DaChuyenDi = "Đã chuyển đi";

    // Giữ tương thích với code cũ đang sử dụng hằng số này
    public const string DangCuTru = ThuongTru;

    public string TinhTrangCuTru { get; set; } = ThuongTru;

    public string MaCD { get; set; } = "";
    public string HoTen { get; set; } = "";
    public DateTime? NgaySinh { get; set; }
    public string SoDienThoai { get; set; } = "";
    public string? MaCH { get; set; }
    public string VaiTro { get; set; } = "Thành viên";

    public CuDan Clone() => (CuDan)MemberwiseClone();
}
