
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Models;

namespace QuanLyChungCu_SkyResidence.Repositories;

public class HoaDonRepository
{
    // Lấy danh sách hóa đơn, hỗ trợ tìm kiếm
    public List<HoaDon> GetAll(string keyword = "")
    {
        return Database.Query(
            @"SELECT
                MaHoaDon,
                MaCH,
                KyThanhToan,
                Thang,
                Nam,
                NgayLap,
                HanThanhToan,
                TongTien,
                DaThu,
                ConNo,
                NgayThanhToanCuoi,
                TrangThai,
                QuaHan
              FROM dbo.vw_HoaDon
              WHERE
                @TuKhoa = ''
                OR MaHoaDon LIKE @TimKiem
                OR MaCH LIKE @TimKiem
                OR KyThanhToan LIKE @TimKiem
                OR TrangThai LIKE @TimKiem
              ORDER BY Nam DESC, Thang DESC, MaHoaDon DESC",
            r => new HoaDon
            {
                MaHoaDon = r.GetString(0),
                MaCH = r.GetString(1),
                KyThanhToan = r.IsDBNull(2) ? "" : r.GetString(2),
                Thang = r.GetByte(3),
                Nam = r.GetInt16(4),
                NgayLap = r.IsDBNull(5) ? null : r.GetDateTime(5),
                HanThanhToan = r.IsDBNull(6) ? null : r.GetDateTime(6),
                TongTien = r.IsDBNull(7) ? 0 : r.GetDecimal(7),
                DaThu = r.IsDBNull(8) ? 0 : r.GetDecimal(8),
                ConNo = r.IsDBNull(9) ? 0 : r.GetDecimal(9),
                NgayThanhToanCuoi = r.IsDBNull(10)
                    ? null : r.GetDateTime(10),
                TrangThai = r.IsDBNull(11) ? "" : r.GetString(11),
                QuaHan = r.IsDBNull(12) ? 0 : r.GetInt32(12)
            },
            new SqlParameter("@TuKhoa", keyword.Trim()),
            new SqlParameter("@TimKiem", "%" + keyword.Trim() + "%")
        );
    }

    // Thêm hóa đơn
    // MaHoaDon và NgayLap được SQL Server tự tạo theo DEFAULT
    public int Insert(HoaDon hoaDon)
    {
        return Database.Execute(
            @"INSERT INTO dbo.HoaDon
                (MaCH, Thang, Nam, HanThanhToan,
                 TongTien, TrangThai, MaNV, GhiChu)
              VALUES
                (@MaCH, @Thang, @Nam, @HanThanhToan,
                 0, N'Chưa thanh toán', @MaNV, @GhiChu)",
            new SqlParameter("@MaCH", hoaDon.MaCH),
            new SqlParameter("@Thang", hoaDon.Thang),
            new SqlParameter("@Nam", hoaDon.Nam),
            new SqlParameter("@HanThanhToan",
                hoaDon.HanThanhToan ?? DateTime.Today),
            new SqlParameter("@MaNV",
                string.IsNullOrWhiteSpace(hoaDon.MaNV)
                    ? DBNull.Value : hoaDon.MaNV),
            new SqlParameter("@GhiChu",
                string.IsNullOrWhiteSpace(hoaDon.GhiChu)
                    ? DBNull.Value : hoaDon.GhiChu)
        );
    }

    // Sửa thông tin hóa đơn
    public int Update(HoaDon hoaDon)
    {
        return Database.Execute(
            @"UPDATE dbo.HoaDon
              SET MaCH = @MaCH,
                  Thang = @Thang,
                  Nam = @Nam,
                  HanThanhToan = @HanThanhToan,
                  GhiChu = @GhiChu
              WHERE MaHoaDon = @MaHoaDon",
            new SqlParameter("@MaHoaDon", hoaDon.MaHoaDon),
            new SqlParameter("@MaCH", hoaDon.MaCH),
            new SqlParameter("@Thang", hoaDon.Thang),
            new SqlParameter("@Nam", hoaDon.Nam),
            new SqlParameter("@HanThanhToan",
                hoaDon.HanThanhToan ?? DateTime.Today),
            new SqlParameter("@GhiChu",
                string.IsNullOrWhiteSpace(hoaDon.GhiChu)
                    ? DBNull.Value : hoaDon.GhiChu)
        );
    }

    // Xóa hóa đơn
    // Nếu hóa đơn đã có giao dịch thanh toán, SQL Server sẽ chặn xóa
    public int Delete(string maHoaDon)
    {
        return Database.Execute(
            @"DELETE FROM dbo.HoaDon
              WHERE MaHoaDon = @MaHoaDon",
            new SqlParameter("@MaHoaDon", maHoaDon)
        );
    }

    // Kiểm tra một căn hộ đã có hóa đơn trong tháng/năm chưa
    public bool Exists(string maCH, int thang, int nam,
        string? maHoaDonBoQua = null)
    {
        object maBoQua = string.IsNullOrWhiteSpace(maHoaDonBoQua)
            ? DBNull.Value
            : maHoaDonBoQua;

        int count = Database.Scalar<int>(
            @"SELECT COUNT(1)
              FROM dbo.HoaDon
              WHERE MaCH = @MaCH
                AND Thang = @Thang
                AND Nam = @Nam
                AND (@MaBoQua IS NULL OR MaHoaDon <> @MaBoQua)",
            new SqlParameter("@MaCH", maCH),
            new SqlParameter("@Thang", thang),
            new SqlParameter("@Nam", nam),
            new SqlParameter("@MaBoQua", maBoQua)
        );

        return count > 0;
    }
}
