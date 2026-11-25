
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;

namespace QuanLyChungCu_SkyResidence.Services;

public class AuthService
{
    // Số vòng lặp PBKDF2 - phải khớp với lúc tạo MatKhauHash.
    private const int Iterations = 100000;

    /// <summary>
    /// Đăng nhập bằng tài khoản trong SQL Server.
    /// Trả về null nếu tài khoản không tồn tại, bị khóa
    /// hoặc mật khẩu không chính xác.
    /// </summary>
    public AuthUser? Login(string username, string password)
    {
        const string sql = @"
            SELECT
                MaTK,
                TenDangNhap,
                Salt,
                MatKhauHash,
                MaVaiTro,
                MaNV,
                HoatDong
            FROM dbo.TaiKhoan
            WHERE TenDangNhap = @TenDangNhap;";

        var accounts = Database.Query(
            sql,
            reader => new
            {
                MaTK = Convert.ToInt32(reader["MaTK"]),
                TenDangNhap = reader["TenDangNhap"].ToString() ?? "",
                Salt = reader["Salt"].ToString() ?? "",
                MatKhauHash = reader["MatKhauHash"].ToString() ?? "",
                MaVaiTro = reader["MaVaiTro"].ToString() ?? "",
                MaNV = reader["MaNV"] == DBNull.Value
                    ? null
                    : reader["MaNV"].ToString(),
                HoatDong = Convert.ToBoolean(reader["HoatDong"])
            },
            new SqlParameter("@TenDangNhap", SqlDbType.VarChar, 100)
            {
                Value = username.Trim()
            }
        );

        if (accounts.Count == 0)
            return null;

        var account = accounts[0];

        // Không cho phép tài khoản đã bị khóa đăng nhập.
        if (!account.HoatDong)
            return null;

        // Kiểm tra mật khẩu.
        if (!VerifyPassword(
                password,
                account.Salt,
                account.MatKhauHash))
            return null;

        // Cập nhật thời điểm đăng nhập gần nhất.
        const string updateSql = @"
            UPDATE dbo.TaiKhoan
            SET LanDangNhapCuoi = SYSDATETIME()
            WHERE MaTK = @MaTK;";

        Database.Execute(
            updateSql,
            new SqlParameter("@MaTK", SqlDbType.Int)
            {
                Value = account.MaTK
            }
        );

        // Lấy quyền theo vai trò.
        var permissions = GetPermissions(account.MaVaiTro);

        return new AuthUser
        {
            MaTK = account.MaTK,
            TenDangNhap = account.TenDangNhap,
            MaVaiTro = account.MaVaiTro,
            MaNV = account.MaNV,
            Permissions = permissions
        };
    }

    /// <summary>
    /// Kiểm tra mật khẩu bằng PBKDF2-SHA256.
    /// Hỗ trợ Salt và Hash lưu dạng Base64 hoặc Hex.
    /// </summary>
    private static bool VerifyPassword(
        string password,
        string saltText,
        string hashText)
    {
        try
        {
            byte[] salt = DecodeStoredBytes(saltText);
            byte[] expectedHash = DecodeStoredBytes(hashText);

            if (salt.Length == 0 || expectedHash.Length == 0)
                return false;

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length
            );

            return CryptographicOperations.FixedTimeEquals(
                actualHash,
                expectedHash
            );
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    /// Chuyển Salt hoặc Hash dạng Base64/Hex thành byte[].
    /// </summary>
    
private static byte[] DecodeStoredBytes(string value)
{
    value = value.Trim();

    if (string.IsNullOrWhiteSpace(value))
        return Array.Empty<byte>();

    // Ưu tiên kiểm tra Hex trước.
    bool isHex = value.Length % 2 == 0;

    foreach (char c in value)
    {
        if (!Uri.IsHexDigit(c))
        {
            isHex = false;
            break;
        }
    }

    if (isHex)
        return Convert.FromHexString(value);

    // Nếu không phải Hex thì thử Base64.
    try
    {
        return Convert.FromBase64String(value);
    }
    catch (FormatException)
    {
        throw new FormatException(
            "Salt hoặc Hash không đúng định dạng Hex/Base64."
        );
    }
}

    /// <summary>
    /// Lấy danh sách quyền của một vai trò.
    /// </summary>
    public List<AuthPermission> GetPermissions(string maVaiTro)
    {
        const string sql = @"
            SELECT
                MaChucNang,
                CoXem,
                CoThem,
                CoSua,
                CoXoa
            FROM dbo.PhanQuyen
            WHERE MaVaiTro = @MaVaiTro;";

        return Database.Query(
            sql,
            reader => new AuthPermission
            {
                MaChucNang = reader["MaChucNang"].ToString() ?? "",
                CoXem = Convert.ToBoolean(reader["CoXem"]),
                CoThem = Convert.ToBoolean(reader["CoThem"]),
                CoSua = Convert.ToBoolean(reader["CoSua"]),
                CoXoa = Convert.ToBoolean(reader["CoXoa"])
            },
            new SqlParameter("@MaVaiTro", SqlDbType.VarChar, 50)
            {
                Value = maVaiTro
            }
        );
    }
}

/// <summary>
/// Thông tin tài khoản sau khi đăng nhập thành công.
/// </summary>
public class AuthUser
{
    public int MaTK { get; set; }

    public string TenDangNhap { get; set; } = "";

    public string MaVaiTro { get; set; } = "";

    public string? MaNV { get; set; }

    public List<AuthPermission> Permissions { get; set; } = new();

    public bool HasPermission(
        string maChucNang,
        string thaoTac)
    {
        var permission = Permissions.Find(
            p => string.Equals(
                p.MaChucNang,
                maChucNang,
                StringComparison.OrdinalIgnoreCase
            )
        );

        if (permission == null)
            return false;

        return thaoTac.ToLowerInvariant() switch
        {
            "xem" => permission.CoXem,
            "them" => permission.CoThem,
            "sua" => permission.CoSua,
            "xoa" => permission.CoXoa,
            _ => false
        };
    }
}

/// <summary>
/// Quyền của một chức năng.
/// </summary>
public class AuthPermission
{
    public string MaChucNang { get; set; } = "";

    public bool CoXem { get; set; }

    public bool CoThem { get; set; }

    public bool CoSua { get; set; }

    public bool CoXoa { get; set; }
}
