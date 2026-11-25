namespace QuanLyChungCu_SkyResidence.Models;

public class CanHo
{
    public const string DangO = "Đang ở";
    public const string ConTrong = "Còn trống";
    public const string DangBaoTri = "Đang bảo trì";

    public static readonly string[] DanhSachTrangThai = { DangO, ConTrong, DangBaoTri };

    public string MaCH { get; set; } = "";
    public string MaToa { get; set; } = "";
    public int Tang { get; set; } = 1;
    public decimal DienTich { get; set; }
    public int SoXeMay { get; set; }
    public int SoOTo { get; set; }
    public string TrangThai { get; set; } = ConTrong;

    /// <summary>Chỉ để hiển thị (lấy từ vw_CanHo), không lưu ngược vào bảng CanHo.</summary>
    public string? ChuHo { get; set; }

    public CanHo Clone() => (CanHo)MemberwiseClone();
}
