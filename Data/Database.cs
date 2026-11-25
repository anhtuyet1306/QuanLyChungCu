using Microsoft.Data.SqlClient;

namespace QuanLyChungCu_SkyResidence.Data;

public static class Database
{
    // Mỗi máy có thể đặt biến môi trường SKYRESIDENCE_CONN để đổi tên SQL Server
    // mà không cần sửa code (VD: máy dùng instance khác .\SQLEXPRESS).
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("SKYRESIDENCE_CONN")
        ?? @"Server=.\SQLEXPRESS;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection GetConnection() => new(ConnectionString);

    public static bool TestConnection()
    {
        try
        {
            using SqlConnection connection = GetConnection();
            connection.Open();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Lỗi kết nối SQL Server:\n" + ex.Message,
                "SkyResidence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>Chạy SELECT, map từng dòng sang đối tượng T. Luôn truyền tham số qua SqlParameter.</summary>
    public static List<T> Query<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        var list = new List<T>();
        while (reader.Read())
            list.Add(map(reader));
        return list;
    }

    /// <summary>Chạy INSERT / UPDATE / DELETE, trả về số dòng bị ảnh hưởng.</summary>
    public static int Execute(string sql, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        conn.Open();
        return cmd.ExecuteNonQuery();
    }

    /// <summary>Chạy câu lệnh trả về 1 giá trị (COUNT, SUM, ...).</summary>
    public static T? Scalar<T>(string sql, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        conn.Open();
        object? result = cmd.ExecuteScalar();
        if (result is null or DBNull)
            return default;
        return (T)Convert.ChangeType(result, typeof(T));
    }
}
