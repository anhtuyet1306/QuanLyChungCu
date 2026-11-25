
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Pages;

public class SettingsPage : UserControl
{
    private readonly AuthUser currentUser;

    private readonly Label lblKetNoi = new();

    private readonly TextBox txtMatKhauCu = new();
    private readonly TextBox txtMatKhauMoi = new();
    private readonly TextBox txtXacNhan = new();

    private const int Iterations = 100000;

    public SettingsPage(AuthUser user)
    {
        currentUser = user;

        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(244, 248, 255);
        Padding = new Padding(24);

        var title = new Label
        {
            Text = "CÀI ĐẶT HỆ THỐNG",
            Dock = DockStyle.Top,
            Height = 45,
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        var description = new Label
        {
            Text = "Quản lý thông tin, bảo mật và cấu hình phần mềm SkyResidence.",
            Dock = DockStyle.Top,
            Height = 35,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.DimGray
        };

        // THÔNG TIN PHẦN MỀM
        var infoPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 185,
            BackColor = Color.White,
            Padding = new Padding(22)
        };

        var info = new Label
        {
            Text =
                "THÔNG TIN PHẦN MỀM\n\n" +
                "Tên chung cư: SkyResidence\n\n" +
                "Tài khoản đang đăng nhập: " + currentUser.TenDangNhap + "\n\n" +
                "Vai trò: " + currentUser.MaVaiTro + "\n\n" +
                "Phiên bản: 1.0.0",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        infoPanel.Controls.Add(info);

        // ĐỔI MẬT KHẨU
        var passwordPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 270,
            BackColor = Color.White,
            Padding = new Padding(22)
        };

        var passwordTitle = new Label
        {
            Text = "ĐỔI MẬT KHẨU",
            Location = new Point(22, 15),
            Size = new Size(300, 28),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        TaoTruongNhap(
            passwordPanel,
            "Mật khẩu hiện tại:",
            txtMatKhauCu,
            55
        );

        TaoTruongNhap(
            passwordPanel,
            "Mật khẩu mới:",
            txtMatKhauMoi,
            105
        );

        TaoTruongNhap(
            passwordPanel,
            "Xác nhận mật khẩu mới:",
            txtXacNhan,
            155
        );

        txtMatKhauCu.UseSystemPasswordChar = true;
        txtMatKhauMoi.UseSystemPasswordChar = true;
        txtXacNhan.UseSystemPasswordChar = true;

        var btnDoiMatKhau = new Button
        {
            Text = "ĐỔI MẬT KHẨU",
            Location = new Point(220, 210),
            Size = new Size(160, 38),
            BackColor = Color.FromArgb(43, 125, 232),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        btnDoiMatKhau.Click += (s, e) => DoiMatKhau();

        passwordPanel.Controls.Add(passwordTitle);
        passwordPanel.Controls.Add(btnDoiMatKhau);

        // KIỂM TRA KẾT NỐI
        var connectionPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 125,
            BackColor = Color.White,
            Padding = new Padding(22)
        };

        var connectionTitle = new Label
        {
            Text = "KẾT NỐI CƠ SỞ DỮ LIỆU",
            Location = new Point(22, 15),
            Size = new Size(300, 28),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        lblKetNoi.Text = "Trạng thái: Chưa kiểm tra";
        lblKetNoi.Location = new Point(22, 52);
        lblKetNoi.Size = new Size(350, 28);
        lblKetNoi.Font = new Font("Segoe UI", 10);

        var btnKiemTra = new Button
        {
            Text = "KIỂM TRA KẾT NỐI",
            Location = new Point(390, 48),
            Size = new Size(180, 35),
            BackColor = Color.FromArgb(25, 135, 84),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        btnKiemTra.Click += (s, e) => KiemTraKetNoi();

        connectionPanel.Controls.Add(connectionTitle);
        connectionPanel.Controls.Add(lblKetNoi);
        connectionPanel.Controls.Add(btnKiemTra);

        Controls.Add(connectionPanel);
        Controls.Add(passwordPanel);
        Controls.Add(infoPanel);
        Controls.Add(description);
        Controls.Add(title);
    }

    private void TaoTruongNhap(
        Panel panel,
        string ten,
        TextBox textBox,
        int y)
    {
        var label = new Label
        {
            Text = ten,
            Location = new Point(22, y + 5),
            Size = new Size(190, 28),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        textBox.Location = new Point(220, y);
        textBox.Size = new Size(280, 30);
        textBox.Font = new Font("Segoe UI", 10);

        panel.Controls.Add(label);
        panel.Controls.Add(textBox);
    }

    private void DoiMatKhau()
    {
        string matKhauCu = txtMatKhauCu.Text;
        string matKhauMoi = txtMatKhauMoi.Text;
        string xacNhan = txtXacNhan.Text;

        if (string.IsNullOrWhiteSpace(matKhauCu) ||
            string.IsNullOrWhiteSpace(matKhauMoi) ||
            string.IsNullOrWhiteSpace(xacNhan))
        {
            MessageBox.Show(
                "Vui lòng nhập đầy đủ thông tin!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        if (matKhauMoi.Length < 8)
        {
            MessageBox.Show(
                "Mật khẩu mới phải có ít nhất 8 ký tự!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        if (matKhauMoi != xacNhan)
        {
            MessageBox.Show(
                "Mật khẩu xác nhận không khớp!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            txtXacNhan.Clear();
            txtXacNhan.Focus();
            return;
        }

        if (matKhauCu == matKhauMoi)
        {
            MessageBox.Show(
                "Mật khẩu mới phải khác mật khẩu hiện tại!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        try
        {
            const string selectSql = @"
                SELECT Salt, MatKhauHash
                FROM dbo.TaiKhoan
                WHERE MaTK = @MaTK AND HoatDong = 1;";

            var accounts = Database.Query(
                selectSql,
                reader => new
                {
                    Salt = reader["Salt"].ToString() ?? "",
                    Hash = reader["MatKhauHash"].ToString() ?? ""
                },
                new SqlParameter("@MaTK", SqlDbType.Int)
                {
                    Value = currentUser.MaTK
                }
            );

            if (accounts.Count == 0)
            {
                MessageBox.Show(
                    "Tài khoản không tồn tại hoặc đã bị khóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var account = accounts[0];

            if (!KiemTraMatKhau(
                    matKhauCu,
                    account.Salt,
                    account.Hash))
            {
                MessageBox.Show(
                    "Mật khẩu hiện tại không chính xác!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtMatKhauCu.Clear();
                txtMatKhauCu.Focus();
                return;
            }

            // Tạo Salt và Hash mới.
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                matKhauMoi,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                32
            );

            string saltHex = Convert.ToHexString(salt);
            string hashHex = Convert.ToHexString(hash);

            const string updateSql = @"
                UPDATE dbo.TaiKhoan
                SET Salt = @Salt,
                    MatKhauHash = @Hash
                WHERE MaTK = @MaTK AND HoatDong = 1;";

            int rows = Database.Execute(
                updateSql,
                new SqlParameter("@Salt", SqlDbType.VarChar, 50)
                {
                    Value = saltHex
                },
                new SqlParameter("@Hash", SqlDbType.Char, 64)
                {
                    Value = hashHex
                },
                new SqlParameter("@MaTK", SqlDbType.Int)
                {
                    Value = currentUser.MaTK
                }
            );

            if (rows != 1)
            {
                MessageBox.Show(
                    "Không thể cập nhật mật khẩu. Vui lòng thử lại!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            MessageBox.Show(
                "Đổi mật khẩu thành công!",
                "SkyResidence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            txtMatKhauCu.Clear();
            txtMatKhauMoi.Clear();
            txtXacNhan.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không thể đổi mật khẩu:\n" + ex.Message,
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private static bool KiemTraMatKhau(
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
        catch
        {
            return false;
        }
    }

    private static byte[] DecodeStoredBytes(string value)
    {
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<byte>();

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

        return Convert.FromBase64String(value);
    }

    private void KiemTraKetNoi()
    {
        try
        {
            if (Database.TestConnection())
            {
                lblKetNoi.Text =
                    "Trạng thái: Đã kết nối SQL Server thành công";

                lblKetNoi.ForeColor = Color.Green;
            }
            else
            {
                lblKetNoi.Text =
                    "Trạng thái: Không thể kết nối cơ sở dữ liệu";

                lblKetNoi.ForeColor = Color.Red;
            }
        }
        catch (Exception ex)
        {
            lblKetNoi.Text = "Trạng thái: Kết nối thất bại";
            lblKetNoi.ForeColor = Color.Red;

            MessageBox.Show(
                ex.Message,
                "Lỗi kết nối",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
