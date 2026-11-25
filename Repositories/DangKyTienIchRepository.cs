
using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Models;

namespace QuanLyChungCu_SkyResidence.Repositories;

public class DangKyTienIchRepository
{
    public List<DangKyTienIch> GetAll(string keyword = "")
    {
        const string sql = """
            SELECT
                d.MaDK,
                d.MaTI,
                t.TenTI,
                d.MaCD,
                c.HoTen,
                d.NgaySuDung,
                d.GioBatDau,
                d.GioKetThuc,
                d.SoNguoi,
                d.TrangThai,
                d.NgayDangKy,
                d.MaNVDuyet,
                d.LyDoTuChoi
            FROM dbo.DangKyTienIch d
            INNER JOIN dbo.TienIch t ON d.MaTI = t.MaTI
            INNER JOIN dbo.CuDan c ON d.MaCD = c.MaCD
            WHERE
                @TuKhoa = N''
                OR d.MaDK LIKE @Tim
                OR c.HoTen LIKE @Tim
                OR t.TenTI LIKE @Tim
                OR d.TrangThai LIKE @Tim
            ORDER BY d.NgaySuDung DESC, d.GioBatDau
            """;

        string tim = "%" + keyword.Trim() + "%";

        return Database.Query(sql, reader => new DangKyTienIch
        {
            MaDK = reader.GetString(0),
            MaTI = reader.GetString(1),
            TenTI = reader.GetString(2),
            MaCD = reader.GetString(3),
            HoTen = reader.GetString(4),
            NgaySuDung = reader.GetDateTime(5),
            GioBatDau = reader.GetTimeSpan(6),
            GioKetThuc = reader.GetTimeSpan(7),
            SoNguoi = reader.GetByte(8),
            TrangThai = reader.GetString(9),
            NgayDangKy = reader.GetDateTime(10),
            MaNVDuyet = reader.IsDBNull(11) ? null : reader.GetString(11),
            LyDoTuChoi = reader.IsDBNull(12) ? null : reader.GetString(12)
        },
        new SqlParameter("@TuKhoa", SqlDbType.NVarChar, 200)
        {
            Value = keyword.Trim()
        },
        new SqlParameter("@Tim", SqlDbType.NVarChar, 200)
        {
            Value = tim
        });
    }

    public List<KeyValuePair<string, string>> GetResidents()
    {
        const string sql = """
            SELECT MaCD, HoTen
            FROM dbo.CuDan
            ORDER BY HoTen
            """;

        return Database.Query(sql, reader =>
            new KeyValuePair<string, string>(
                reader.GetString(0),
                reader.GetString(1)));
    }

    public List<KeyValuePair<string, string>> GetAmenities()
    {
        const string sql = """
            SELECT MaTI, TenTI
            FROM dbo.TienIch
            WHERE HoatDong = 1
            ORDER BY TenTI
            """;

        return Database.Query(sql, reader =>
            new KeyValuePair<string, string>(
                reader.GetString(0),
                reader.GetString(1)));
    }

    public void Insert(DangKyTienIch item)
    {
        Validate(item);

        const string sql = """
            INSERT INTO dbo.DangKyTienIch
            (
                MaTI, MaCD, NgaySuDung,
                GioBatDau, GioKetThuc, SoNguoi,
                TrangThai, LyDoTuChoi
            )
            VALUES
            (
                @MaTI, @MaCD, @NgaySuDung,
                @GioBatDau, @GioKetThuc, @SoNguoi,
                N'Chờ duyệt', NULL
            )
            """;

        Database.Execute(sql,
            new SqlParameter("@MaTI", SqlDbType.VarChar, 20) { Value = item.MaTI },
            new SqlParameter("@MaCD", SqlDbType.VarChar, 20) { Value = item.MaCD },
            new SqlParameter("@NgaySuDung", SqlDbType.Date) { Value = item.NgaySuDung.Date },
            new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = item.GioBatDau },
            new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = item.GioKetThuc },
            new SqlParameter("@SoNguoi", SqlDbType.TinyInt) { Value = item.SoNguoi });
    }

    public void Update(DangKyTienIch item)
    {
        Validate(item);

        const string sql = """
            UPDATE dbo.DangKyTienIch
            SET
                MaTI = @MaTI,
                MaCD = @MaCD,
                NgaySuDung = @NgaySuDung,
                GioBatDau = @GioBatDau,
                GioKetThuc = @GioKetThuc,
                SoNguoi = @SoNguoi,
                TrangThai = @TrangThai,
                LyDoTuChoi = @LyDoTuChoi,
                MaNVDuyet = NULL
            WHERE MaDK = @MaDK
            """;

        Database.Execute(sql,
            new SqlParameter("@MaDK", SqlDbType.VarChar, 20) { Value = item.MaDK },
            new SqlParameter("@MaTI", SqlDbType.VarChar, 20) { Value = item.MaTI },
            new SqlParameter("@MaCD", SqlDbType.VarChar, 20) { Value = item.MaCD },
            new SqlParameter("@NgaySuDung", SqlDbType.Date) { Value = item.NgaySuDung.Date },
            new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = item.GioBatDau },
            new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = item.GioKetThuc },
            new SqlParameter("@SoNguoi", SqlDbType.TinyInt) { Value = item.SoNguoi },
            new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = item.TrangThai },
            new SqlParameter("@LyDoTuChoi", SqlDbType.NVarChar, -1)
            {
                Value = string.IsNullOrWhiteSpace(item.LyDoTuChoi)
                    ? DBNull.Value
                    : item.LyDoTuChoi.Trim()
            });
    }

    public void Delete(string maDK)
    {
        const string sql = """
            DELETE FROM dbo.DangKyTienIch
            WHERE MaDK = @MaDK
            """;

        Database.Execute(sql,
            new SqlParameter("@MaDK", SqlDbType.VarChar, 20)
            {
                Value = maDK
            });
    }

    private static void Validate(DangKyTienIch item)
    {
        if (string.IsNullOrWhiteSpace(item.MaTI))
            throw new Exception("Vui lòng chọn tiện ích.");

        if (string.IsNullOrWhiteSpace(item.MaCD))
            throw new Exception("Vui lòng chọn cư dân.");

        if (item.GioKetThuc <= item.GioBatDau)
            throw new Exception("Giờ kết thúc phải sau giờ bắt đầu.");

        if (item.SoNguoi < 1)
            throw new Exception("Số người đăng ký phải lớn hơn 0.");

        if (item.TrangThai == "Từ chối" &&
            string.IsNullOrWhiteSpace(item.LyDoTuChoi))
            throw new Exception("Vui lòng nhập lý do từ chối.");
    }
}
