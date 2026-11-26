using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Models;

namespace QuanLyChungCu_SkyResidence.Repositories;

public class CuDanRepository
{
    public List<CuDan> GetAll() => Database.Query(
        "SELECT MaCD, HoTen, NgaySinh, SoDienThoai, MaCH, VaiTro, TinhTrangCuTru FROM dbo.CuDan ORDER BY MaCD",
        r => new CuDan
        {
            MaCD = r.GetString(0), HoTen = r.GetString(1),
            NgaySinh = r.IsDBNull(2) ? null : r.GetDateTime(2),
            SoDienThoai = r.IsDBNull(3) ? "" : r.GetString(3),
            MaCH = r.IsDBNull(4) ? null : r.GetString(4),
            VaiTro = r.IsDBNull(5) ? "Thành viên" : r.GetString(5),
            TinhTrangCuTru = r.IsDBNull(6) ? "" : r.GetString(6)
        });

    public List<string> GetApartmentCodes() => Database.Query(
        "SELECT MaCH FROM dbo.CanHo ORDER BY MaCH", r => r.GetString(0));
    public bool Exists(string maCD) => Database.Scalar<int>(
        "SELECT COUNT(1) FROM dbo.CuDan WHERE MaCD=@MaCD", new SqlParameter("@MaCD", maCD)) > 0;
    public int Insert(CuDan cd) => Write(cd, "insert");
    public int Update(CuDan cd) => Write(cd, "update");
    public int Delete(string maCD) => Write(new CuDan { MaCD = maCD }, "delete");

    // Lưu cư dân và trạng thái căn hộ trong cùng giao dịch: cùng thành công hoặc cùng hủy.
    private static int Write(CuDan cd, string operation)
    {
        using var conn = Database.GetConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction(IsolationLevel.Serializable);
        using var cmd = new SqlCommand("""
            DECLARE @OldMaCH varchar(10), @Affected int = 0;
            SELECT @OldMaCH=MaCH FROM dbo.CuDan WITH (UPDLOCK, HOLDLOCK) WHERE MaCD=@MaCD;
            DECLARE @LockedMaCH varchar(10);
            SELECT @LockedMaCH=MaCH FROM dbo.CanHo WITH (UPDLOCK, HOLDLOCK)
              WHERE MaCH IN (@OldMaCH, @MaCH) ORDER BY MaCH;
            IF @Operation <> 'delete'
            BEGIN
                IF @TinhTrangCuTru=N'Đang cư trú' AND @MaCH IS NOT NULL
                   AND EXISTS (SELECT 1 FROM dbo.CanHo WHERE MaCH=@MaCH AND TrangThai=N'Đang bảo trì')
                    THROW 50001, N'Căn hộ đang bảo trì. Hãy chọn căn hộ khác hoặc cập nhật tình trạng căn hộ trước.', 1;
                IF @TinhTrangCuTru=N'Đang cư trú' AND @VaiTro=N'Chủ hộ'
                   AND EXISTS (SELECT 1 FROM dbo.CuDan WITH (UPDLOCK, HOLDLOCK)
                       WHERE MaCH=@MaCH AND MaCD<>@MaCD AND VaiTro=N'Chủ hộ'
                         AND TinhTrangCuTru=N'Đang cư trú')
                    THROW 50002, N'Căn hộ đã có chủ hộ đang cư trú. Hãy cập nhật chủ hộ cũ trước.', 1;
                IF @Operation='insert'
                    INSERT dbo.CuDan(MaCD,HoTen,NgaySinh,SoDienThoai,MaCH,VaiTro,TinhTrangCuTru)
                    VALUES(@MaCD,@HoTen,@NgaySinh,@SoDienThoai,@MaCH,@VaiTro,@TinhTrangCuTru);
                ELSE
                    UPDATE dbo.CuDan SET HoTen=@HoTen, NgaySinh=@NgaySinh,
                        SoDienThoai=@SoDienThoai, MaCH=@MaCH, VaiTro=@VaiTro, TinhTrangCuTru=@TinhTrangCuTru
                    WHERE MaCD=@MaCD;
                SET @Affected=@@ROWCOUNT;
            END
            ELSE
            BEGIN
                IF EXISTS (SELECT 1 FROM dbo.CuDan WHERE MaCD=@MaCD AND TinhTrangCuTru=N'Đã chuyển đi')
                    THROW 50003, N'Giữ lại hồ sơ đã chuyển đi để tra cứu; không xóa hồ sơ này.', 1;
                DELETE dbo.CuDan WHERE MaCD=@MaCD;
                SET @Affected=@@ROWCOUNT;
            END;
            IF @Affected > 0
                UPDATE c SET TrangThai=CASE WHEN EXISTS (
                    SELECT 1 FROM dbo.CuDan d WHERE d.MaCH=c.MaCH AND d.TinhTrangCuTru=N'Đang cư trú'
                ) THEN N'Đang ở' ELSE N'Còn trống' END
                FROM dbo.CanHo c WHERE c.MaCH IN (@OldMaCH, @MaCH) AND c.TrangThai<>N'Đang bảo trì';
            SELECT @Affected;
            """, conn, transaction);
        cmd.Parameters.AddRange(new[]
        {
            new SqlParameter("@Operation", operation),
            new SqlParameter("@MaCD", SqlDbType.VarChar, 20) { Value = cd.MaCD },
            new SqlParameter("@HoTen", SqlDbType.NVarChar, 150) { Value = cd.HoTen },
            new SqlParameter("@NgaySinh", SqlDbType.Date) { Value = (object?)cd.NgaySinh?.Date ?? DBNull.Value },
            new SqlParameter("@SoDienThoai", SqlDbType.VarChar, 15) { Value = string.IsNullOrWhiteSpace(cd.SoDienThoai) ? DBNull.Value : cd.SoDienThoai },
            new SqlParameter("@MaCH", SqlDbType.VarChar, 10) { Value = (object?)cd.MaCH ?? DBNull.Value },
            new SqlParameter("@VaiTro", SqlDbType.NVarChar, 30) { Value = cd.VaiTro },
            new SqlParameter("@TinhTrangCuTru", SqlDbType.NVarChar, 30) { Value = cd.TinhTrangCuTru }
        });
        int affected = Convert.ToInt32(cmd.ExecuteScalar());
        transaction.Commit();
        return affected;
        // Nếu có lỗi SQL, using hủy giao dịch chưa commit.
    }
}
