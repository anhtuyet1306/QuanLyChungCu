
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Models;

namespace QuanLyChungCu_SkyResidence.Repositories;

public class LoaiPhiRepository
{
    public List<LoaiPhi> GetAll() => Database.Query(
        @"SELECT MaLoaiPhi, TenPhi, DonViTinh, DonGia,
                 CachTinh, ChuKy, HoatDong
          FROM dbo.LoaiPhi
          ORDER BY MaLoaiPhi",
        r => new LoaiPhi
        {
            MaLoaiPhi = r.GetString(0),
            TenPhi = r.GetString(1),
            DonViTinh = r.IsDBNull(2) ? "" : r.GetString(2),
            DonGia = r.GetDecimal(3),
            CachTinh = r.IsDBNull(4) ? "" : r.GetString(4),
            ChuKy = r.IsDBNull(5) ? "" : r.GetString(5),
            HoatDong = !r.IsDBNull(6) && r.GetBoolean(6)
        });

    public bool Exists(string maLoaiPhi) => Database.Scalar<int>(
        "SELECT COUNT(1) FROM dbo.LoaiPhi WHERE MaLoaiPhi=@Ma",
        new SqlParameter("@Ma", maLoaiPhi)) > 0;

    public int Insert(LoaiPhi phi) => Database.Execute(
        @"INSERT INTO dbo.LoaiPhi
          (MaLoaiPhi, TenPhi, DonViTinh, DonGia,
           CachTinh, ChuKy, HoatDong)
          VALUES
          (@Ma, @Ten, @DonVi, @DonGia,
           @CachTinh, @ChuKy, @HoatDong)",
        Parameters(phi));

    public int Update(LoaiPhi phi) => Database.Execute(
        @"UPDATE dbo.LoaiPhi
          SET TenPhi=@Ten,
              DonViTinh=@DonVi,
              DonGia=@DonGia,
              CachTinh=@CachTinh,
              ChuKy=@ChuKy,
              HoatDong=@HoatDong
          WHERE MaLoaiPhi=@Ma",
        Parameters(phi));

    public int Delete(string maLoaiPhi) => Database.Execute(
        @"UPDATE dbo.LoaiPhi
          SET HoatDong=0
          WHERE MaLoaiPhi=@Ma AND HoatDong=1",
        new SqlParameter("@Ma", maLoaiPhi));

    private static SqlParameter[] Parameters(LoaiPhi phi) =>
    [
        new SqlParameter("@Ma", phi.MaLoaiPhi),
        new SqlParameter("@Ten", phi.TenPhi),
        new SqlParameter("@DonVi", phi.DonViTinh),
        new SqlParameter("@DonGia", phi.DonGia),
        new SqlParameter("@CachTinh", phi.CachTinh),
        new SqlParameter("@ChuKy", phi.ChuKy),
        new SqlParameter("@HoatDong", phi.HoatDong)
    ];
}
