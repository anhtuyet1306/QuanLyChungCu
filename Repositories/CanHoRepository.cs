using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Models;

namespace QuanLyChungCu_SkyResidence.Repositories;

/// <summary>Tầng truy cập dữ liệu (DAL): chỉ chứa SQL, KHÔNG chứa luật nghiệp vụ hay MessageBox.</summary>
public class CanHoRepository
{
    public List<CanHo> GetAll()
    {
        return Database.Query(
            "SELECT MaCH, Toa, Tang, DienTich, SoXeMay, SoOTo, ChuHo, TrangThai " +
            "FROM dbo.vw_CanHo ORDER BY Toa, Tang, MaCH",
            r => new CanHo
            {
                MaCH = r.GetString(0),
                MaToa = r.GetString(1),
                Tang = Convert.ToInt32(r.GetValue(2)),
                DienTich = r.GetDecimal(3),
                SoXeMay = Convert.ToInt32(r.GetValue(4)),
                SoOTo = Convert.ToInt32(r.GetValue(5)),
                ChuHo = r.IsDBNull(6) ? null : r.GetString(6),
                TrangThai = r.GetString(7)
            });
    }

    public List<ToaItem> GetToaList()
    {
        return Database.Query(
            "SELECT MaToa, TenToa, SoTang FROM dbo.Toa ORDER BY MaToa",
            r => new ToaItem(r.GetString(0), r.GetString(1), Convert.ToInt32(r.GetValue(2))));
    }

    public bool Exists(string maCH)
    {
        return Database.Scalar<int>(
            "SELECT COUNT(1) FROM dbo.CanHo WHERE MaCH = @MaCH",
            new SqlParameter("@MaCH", maCH)) > 0;
    }

    /// <summary>Số cư dân đang cư trú (chưa chuyển đi) trong căn hộ.</summary>
    public int CountResidentsLiving(string maCH)
    {
        return Database.Scalar<int>(
            "SELECT COUNT(1) FROM dbo.CuDan WHERE MaCH = @MaCH AND TinhTrangCuTru <> N'Đã chuyển đi'",
            new SqlParameter("@MaCH", maCH));
    }

    public int Insert(CanHo ch)
    {
        return Database.Execute(
            "INSERT dbo.CanHo (MaCH, MaToa, Tang, DienTich, SoXeMay, SoOTo, TrangThai) " +
            "VALUES (@MaCH, @MaToa, @Tang, @DienTich, @SoXeMay, @SoOTo, @TrangThai)",
            ToParams(ch));
    }

    public int Update(CanHo ch)
    {
        return Database.Execute(
            "UPDATE dbo.CanHo SET MaToa = @MaToa, Tang = @Tang, DienTich = @DienTich, " +
            "SoXeMay = @SoXeMay, SoOTo = @SoOTo, TrangThai = @TrangThai WHERE MaCH = @MaCH",
            ToParams(ch));
    }

    public int Delete(string maCH)
    {
        return Database.Execute(
            "DELETE FROM dbo.CanHo WHERE MaCH = @MaCH",
            new SqlParameter("@MaCH", maCH));
    }

    private static SqlParameter[] ToParams(CanHo ch) => new[]
    {
        new SqlParameter("@MaCH", ch.MaCH),
        new SqlParameter("@MaToa", ch.MaToa),
        new SqlParameter("@Tang", (byte)ch.Tang),
        new SqlParameter("@DienTich", SqlDbType.Decimal) { Precision = 6, Scale = 2, Value = ch.DienTich },
        new SqlParameter("@SoXeMay", (byte)ch.SoXeMay),
        new SqlParameter("@SoOTo", (byte)ch.SoOTo),
        new SqlParameter("@TrangThai", ch.TrangThai)
    };
}
