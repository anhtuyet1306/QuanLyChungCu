
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Models;

namespace QuanLyChungCu_SkyResidence.Repositories;

public class SuCoRepository
{
    // Lấy toàn bộ danh sách phản ánh
    public List<SuCo> GetAll(string keyword = "")
    {
        const string sql = @"
            SELECT
                MaSC,
                MaCD,
                CuDan,
                MaCH,
                ViTri,
                LoaiSC,
                NoiDung,
                NgayBao,
                MucDoUuTien,
                TrangThai,
                NgayHoanThanh,
                DanhGia,
                NhanXet,
                MaNVXuLyCuoi,
                NguoiXuLyCuoi
            FROM dbo.vw_SuCo
            WHERE
                @Tim = N''
                OR MaSC LIKE @TimKiem
                OR CuDan LIKE @TimKiem
                OR NoiDung LIKE @TimKiem
                OR LoaiSC LIKE @TimKiem
                OR TrangThai LIKE @TimKiem
                OR ViTri LIKE @TimKiem
            ORDER BY NgayBao DESC;";

        string timKiem = "%" + keyword.Trim() + "%";

        return Database.Query(
            sql,
            MapSuCo,
            new SqlParameter("@Tim", SqlDbType.NVarChar, 200)
            {
                Value = keyword.Trim()
            },
            new SqlParameter("@TimKiem", SqlDbType.NVarChar, 400)
            {
                Value = timKiem
            }
        );
    }

    // Lấy danh sách cư dân để chọn người gửi phản ánh
    public List<KeyValuePair<string, string>> GetResidents()
    {
        const string sql = @"
            SELECT MaCD, HoTen
            FROM dbo.CuDan
            ORDER BY HoTen;";

        return Database.Query(
            sql,
            reader => new KeyValuePair<string, string>(
                reader["MaCD"].ToString() ?? "",
                reader["HoTen"].ToString() ?? ""
            )
        );
    }

    // Thêm phản ánh
    public void Insert(SuCo item)
    {
        Validate(item);

        const string sql = @"
            INSERT INTO dbo.SuCo
            (
                MaSC,
                MaCD,
                MaCH,
                ViTri,
                LoaiSC,
                NoiDung,
                NgayBao,
                MucDoUuTien,
                TrangThai
            )
            VALUES
            (
                @MaSC,
                @MaCD,
                @MaCH,
                @ViTri,
                @LoaiSC,
                @NoiDung,
                @NgayBao,
                @MucDoUuTien,
                @TrangThai
            );";

        Database.Execute(sql, CreateParameters(item));
    }

    // Cập nhật phản ánh
    public void Update(SuCo item)
    {
        Validate(item);

        const string sql = @"
            UPDATE dbo.SuCo
            SET
                MaCD = @MaCD,
                MaCH = @MaCH,
                ViTri = @ViTri,
                LoaiSC = @LoaiSC,
                NoiDung = @NoiDung,
                MucDoUuTien = @MucDoUuTien,
                TrangThai = @TrangThai
            WHERE MaSC = @MaSC;";

        int affected = Database.Execute(sql, CreateParameters(item));

        if (affected == 0)
            throw new Exception("Không tìm thấy phản ánh cần cập nhật.");
    }

    // Xóa phản ánh
    public void Delete(string maSC)
    {
        if (string.IsNullOrWhiteSpace(maSC))
            throw new Exception("Mã sự cố không hợp lệ.");

        using var connection = Database.GetConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Xóa dữ liệu xử lý liên quan trước
            using (var command = new SqlCommand(
                "DELETE FROM dbo.XuLySuCo WHERE MaSC = @MaSC;",
                connection,
                transaction))
            {
                command.Parameters.Add("@MaSC", SqlDbType.VarChar, 10).Value = maSC;
                command.ExecuteNonQuery();
            }

            // Sau đó xóa phản ánh
            using (var command = new SqlCommand(
                "DELETE FROM dbo.SuCo WHERE MaSC = @MaSC;",
                connection,
                transaction))
            {
                command.Parameters.Add("@MaSC", SqlDbType.VarChar, 10).Value = maSC;

                int affected = command.ExecuteNonQuery();

                if (affected == 0)
                    throw new Exception("Không tìm thấy phản ánh cần xóa.");
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Tạo các tham số SQL
    private static SqlParameter[] CreateParameters(SuCo item)
    {
        return new[]
        {
            new SqlParameter("@MaSC", SqlDbType.VarChar, 10)
            {
                Value = item.MaSC.Trim()
            },

            new SqlParameter("@MaCD", SqlDbType.VarChar, 10)
            {
                Value = item.MaCD.Trim()
            },

            new SqlParameter("@MaCH", SqlDbType.VarChar, 10)
            {
                Value = (object?)item.MaCH ?? DBNull.Value
            },

            new SqlParameter("@ViTri", SqlDbType.NVarChar, 100)
            {
                Value = (object?)item.ViTri ?? DBNull.Value
            },

            new SqlParameter("@LoaiSC", SqlDbType.NVarChar, 20)
            {
                Value = item.LoaiSC.Trim()
            },

            new SqlParameter("@NoiDung", SqlDbType.NVarChar, 500)
            {
                Value = item.NoiDung.Trim()
            },

            new SqlParameter("@NgayBao", SqlDbType.DateTime2)
            {
                Value = item.NgayBao
            },

            new SqlParameter("@MucDoUuTien", SqlDbType.NVarChar, 15)
            {
                Value = item.MucDoUuTien.Trim()
            },

            new SqlParameter("@TrangThai", SqlDbType.NVarChar, 15)
            {
                Value = item.TrangThai.Trim()
            }
        };
    }

    // Chuyển dữ liệu SQL thành đối tượng SuCo
    private static SuCo MapSuCo(SqlDataReader reader)
    {
        return new SuCo
        {
            MaSC = reader["MaSC"].ToString() ?? "",
            MaCD = reader["MaCD"].ToString() ?? "",
            CuDan = reader["CuDan"].ToString() ?? "",
            MaCH = GetNullableString(reader, "MaCH"),
            ViTri = GetNullableString(reader, "ViTri"),
            LoaiSC = reader["LoaiSC"].ToString() ?? "",
            NoiDung = reader["NoiDung"].ToString() ?? "",
            NgayBao = Convert.ToDateTime(reader["NgayBao"]),
            MucDoUuTien = reader["MucDoUuTien"].ToString() ?? "",
            TrangThai = reader["TrangThai"].ToString() ?? "",
            NgayHoanThanh = GetNullableDateTime(reader, "NgayHoanThanh"),
            DanhGia = reader["DanhGia"] == DBNull.Value
                ? null
                : Convert.ToByte(reader["DanhGia"]),
            NhanXet = GetNullableString(reader, "NhanXet"),
            MaNVXuLyCuoi = GetNullableString(reader, "MaNVXuLyCuoi"),
            NguoiXuLyCuoi = GetNullableString(reader, "NguoiXuLyCuoi")
        };
    }

    private static string? GetNullableString(
        SqlDataReader reader,
        string column)
    {
        return reader[column] == DBNull.Value
            ? null
            : reader[column].ToString();
    }

    private static DateTime? GetNullableDateTime(
        SqlDataReader reader,
        string column)
    {
        return reader[column] == DBNull.Value
            ? null
            : Convert.ToDateTime(reader[column]);
    }

    // Kiểm tra dữ liệu trước khi ghi vào SQL
    private static void Validate(SuCo item)
    {
        if (item == null)
            throw new Exception("Dữ liệu phản ánh không hợp lệ.");

        if (string.IsNullOrWhiteSpace(item.MaSC))
            throw new Exception("Mã sự cố không được để trống.");

        if (item.MaSC.Trim().Length > 10)
            throw new Exception("Mã sự cố tối đa 10 ký tự.");

        if (string.IsNullOrWhiteSpace(item.MaCD))
            throw new Exception("Vui lòng chọn cư dân.");

        if (item.MaCD.Trim().Length > 10)
            throw new Exception("Mã cư dân tối đa 10 ký tự.");

        if (string.IsNullOrWhiteSpace(item.LoaiSC))
            throw new Exception("Loại sự cố không được để trống.");

        if (item.LoaiSC.Trim().Length > 20)
            throw new Exception("Loại sự cố tối đa 20 ký tự.");

        if (string.IsNullOrWhiteSpace(item.NoiDung))
            throw new Exception("Nội dung phản ánh không được để trống.");

        if (item.NoiDung.Trim().Length > 500)
            throw new Exception("Nội dung phản ánh tối đa 500 ký tự.");

        if (!string.IsNullOrWhiteSpace(item.ViTri) &&
            item.ViTri.Trim().Length > 100)
            throw new Exception("Vị trí tối đa 100 ký tự.");

        if (string.IsNullOrWhiteSpace(item.MucDoUuTien) ||
            item.MucDoUuTien.Trim().Length > 15)
            throw new Exception("Mức độ ưu tiên không hợp lệ.");

        if (string.IsNullOrWhiteSpace(item.TrangThai) ||
            item.TrangThai.Trim().Length > 15)
            throw new Exception("Trạng thái không hợp lệ.");
    }
}
