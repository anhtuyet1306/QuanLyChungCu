namespace QuanLyChungCu_SkyResidence.Models;

public class CuDan
{
    public string MaCD { get; set; } = "";
    public string HoTen { get; set; } = "";
    public DateTime? NgaySinh { get; set; }
    public string SoDienThoai { get; set; } = "";
    public string? MaCH { get; set; }
    public string VaiTro { get; set; } = "Thành viên";

    public CuDan Clone() => (CuDan)MemberwiseClone();
}
