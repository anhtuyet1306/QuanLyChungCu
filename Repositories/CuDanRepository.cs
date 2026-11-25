
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Models;

namespace QuanLyChungCu_SkyResidence.Repositories;

/// <summary>
/// Truy cập dữ liệu cư dân trong SQL Server.
/// </summary>
public class CuDanRepository
{
    // Lấy danh sách cư dân
    public List<CuDan> GetAll() => Database.Query(
        "SELECT MaCD, HoTen, NgaySinh, SDT, MaCH, VaiTro " +
        "FROM dbo.CuDan ORDER BY MaCD",
        r => new CuDan
        {
            MaCD = r.GetString(0),
            HoTen = r.GetString(1),
            NgaySinh = r.IsDBNull(2) ? null : r.GetDateTime(2),
            SoDienThoai = r.IsDBNull(3) ? "" : r.GetString(3),
            MaCH = r.IsDBNull(4) ? null : r.GetString(4),
            VaiTro = r.IsDBNull(5) ? "Thành viên" : r.GetString(5)
        });

    // Lấy danh sách mã căn hộ
    public List<string> GetApartmentCodes() => Database.Query(
        "SELECT MaCH FROM dbo.CanHo ORDER BY MaCH",
        r => r.GetString(0));

    // Kiểm tra mã cư dân đã tồn tại chưa
    public bool Exists(string maCD) => Database.Scalar<int>(
        "SELECT COUNT(1) FROM dbo.CuDan WHERE MaCD=@MaCD",
        new SqlParameter("@MaCD", maCD)) > 0;

    // Thêm cư dân
    public int Insert(CuDan cd) => Database.Execute(
        "INSERT dbo.CuDan " +
        "(MaCD, HoTen, NgaySinh, SDT, MaCH, VaiTro, TinhTrangCuTru) " +
        "VALUES (@MaCD, @HoTen, @NgaySinh, @SoDienThoai, @MaCH, @VaiTro, N'Đang cư trú')",
        Parameters(cd));

    // Sửa thông tin cư dân
    public int Update(CuDan cd) => Database.Execute(
        "UPDATE dbo.CuDan SET " +
        "HoTen=@HoTen, " +
        "NgaySinh=@NgaySinh, " +
        "SDT=@SoDienThoai, " +
        "MaCH=@MaCH, " +
        "VaiTro=@VaiTro " +
        "WHERE MaCD=@MaCD",
        Parameters(cd));

    // Xóa cư dân đang cư trú
    public int Delete(string maCD) => Database.Execute(
        "DELETE dbo.CuDan " +
        "WHERE MaCD=@MaCD AND TinhTrangCuTru=N'Đang cư trú'",
        new SqlParameter("@MaCD", maCD));

    // Tạo tham số SQL
    private static SqlParameter[] Parameters(CuDan cd) => new[]
    {
        new SqlParameter("@MaCD", cd.MaCD),
        new SqlParameter("@HoTen", cd.HoTen),

        new SqlParameter(
            "@NgaySinh",
            (object?)cd.NgaySinh?.Date ?? DBNull.Value
        ),

        new SqlParameter(
            "@SoDienThoai",
            string.IsNullOrWhiteSpace(cd.SoDienThoai)
                ? DBNull.Value
                : cd.SoDienThoai
        ),

        new SqlParameter(
            "@MaCH",
            string.IsNullOrWhiteSpace(cd.MaCH)
                ? DBNull.Value
                : cd.MaCH
        ),

        new SqlParameter("@VaiTro", cd.VaiTro)
    };
}
