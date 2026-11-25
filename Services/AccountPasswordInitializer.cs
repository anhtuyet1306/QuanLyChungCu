
using System;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;

namespace QuanLyChungCu_SkyResidence.Services;

public static class AccountPasswordInitializer
{
    private const int Iterations = 100000;

    public static void Initialize()
    {
        var accounts = new[]
        {
            new { Username = "admin", Password = "Admin@123" },
            new { Username = "ketoan", Password = "Ketoan@123" },
            new { Username = "kythuat", Password = "Kythuat@123" },
            new { Username = "letan", Password = "Letan@123" }
        };

        using var connection = Database.GetConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var account in accounts)
            {
                byte[] salt = RandomNumberGenerator.GetBytes(16);

                byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                    account.Password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA256,
                    32
                );

                string saltHex = Convert.ToHexString(salt);
                string hashHex = Convert.ToHexString(hash);

                const string sql = @"
                    UPDATE dbo.TaiKhoan
                    SET Salt = @Salt,
                        MatKhauHash = @Hash
                    WHERE TenDangNhap = @Username;";

                using var command = new SqlCommand(
                    sql,
                    connection,
                    transaction
                );

                command.Parameters.Add(
                    "@Salt",
                    System.Data.SqlDbType.VarChar,
                    50
                ).Value = saltHex;

                command.Parameters.Add(
                    "@Hash",
                    System.Data.SqlDbType.Char,
                    64
                ).Value = hashHex;

                command.Parameters.Add(
                    "@Username",
                    System.Data.SqlDbType.VarChar,
                    50
                ).Value = account.Username;

                int rows = command.ExecuteNonQuery();

                if (rows != 1)
                {
                    throw new Exception(
                        $"Không tìm thấy đúng một tài khoản: {account.Username}"
                    );
                }
            }

            transaction.Commit();

            MessageBox.Show(
                "Đã khởi tạo mật khẩu cho 4 tài khoản thành công!",
                "SkyResidence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            MessageBox.Show(
                "Khởi tạo mật khẩu thất bại:\n" + ex.Message,
                "SkyResidence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
