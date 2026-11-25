using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Styles;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Forms;

public class LoginForm : Form
{
    private readonly TextBox txtUsername = new();
    private readonly TextBox txtPassword = new();
    private readonly CheckBox chkRemember = new();
    private readonly CheckBox chkShowPassword = new();
    private readonly AuthService authService = new();

    private readonly string rememberFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SkyResidence",
        "login.dat");

    public LoginForm()
    {
        InitializeLogin();
        LoadRememberedLogin();
         chkRemember.CheckedChanged += (_, _) =>
        {
            if (!chkRemember.Checked)
                ClearRememberedLogin();
        };
    }

    private void InitializeLogin()
    {
        Text = "Đăng nhập - SkyResidence";
        Size = new Size(1000, 650);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = AppStyles.White;
        Font = AppStyles.NormalFont;

        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };

        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        Controls.Add(main);

        var leftPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppStyles.Navy
        };
        main.Controls.Add(leftPanel, 0, 0);

        var logo = new Label
        {
            Text = "🏢",
            Font = new Font("Segoe UI Emoji", 55),
            ForeColor = AppStyles.White,
            AutoSize = true,
            Location = new Point(165, 125)
        };
        leftPanel.Controls.Add(logo);

        var brand = new Label
        {
            Text = "SKYRESIDENCE",
            Font = new Font("Segoe UI", 23, FontStyle.Bold),
            ForeColor = AppStyles.White,
            AutoSize = true,
            Location = new Point(65, 240)
        };
        leftPanel.Controls.Add(brand);

        var description = new Label
        {
            Text = "HỆ THỐNG QUẢN LÝ CHUNG CƯ\n\n" +
                   "Quản lý cư dân, căn hộ,\n" +
                   "phí dịch vụ và tiện ích\n" +
                   "một cách dễ dàng, hiệu quả.",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.WhiteSmoke,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(400, 150),
            Location = new Point(20, 300)
        };
        leftPanel.Controls.Add(description);

        var footer = new Label
        {
            Text = "SKYRESIDENCE MANAGEMENT SYSTEM",
            Dock = DockStyle.Bottom,
            Height = 45,
            ForeColor = Color.LightGray,
            TextAlign = ContentAlignment.MiddleCenter
        };
        leftPanel.Controls.Add(footer);

        var rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppStyles.White,
            Padding = new Padding(65, 65, 65, 30)
        };
        main.Controls.Add(rightPanel, 1, 0);

        var title = new Label
        {
            Text = "ĐĂNG NHẬP",
            Location = new Point(65, 85)
        };
        AppStyles.StyleTitle(title);
        rightPanel.Controls.Add(title);

        var subtitle = new Label
        {
            Text = "Chào mừng bạn đến với SkyResidence!",
            Font = AppStyles.NormalFont,
            ForeColor = AppStyles.Gray,
            AutoSize = true,
            Location = new Point(68, 135)
        };
        rightPanel.Controls.Add(subtitle);

        var lblUsername = new Label
        {
            Text = "Tên đăng nhập",
            Location = new Point(68, 210),
            AutoSize = true,
            ForeColor = AppStyles.Navy,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        rightPanel.Controls.Add(lblUsername);

        txtUsername.Location = new Point(68, 240);
        txtUsername.Size = new Size(340, 35);
        AppStyles.StyleTextBox(txtUsername);
        rightPanel.Controls.Add(txtUsername);

        var lblPassword = new Label
        {
            Text = "Mật khẩu",
            Location = new Point(68, 305),
            AutoSize = true,
            ForeColor = AppStyles.Navy,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        rightPanel.Controls.Add(lblPassword);

        txtPassword.Location = new Point(68, 335);
        txtPassword.Size = new Size(340, 35);
        txtPassword.UseSystemPasswordChar = true;
        AppStyles.StyleTextBox(txtPassword);
        rightPanel.Controls.Add(txtPassword);

        chkShowPassword.Text = "Hiển thị mật khẩu";
        chkShowPassword.Location = new Point(68, 385);
        chkShowPassword.AutoSize = true;
        rightPanel.Controls.Add(chkShowPassword);

        chkShowPassword.CheckedChanged += (_, _) =>
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        };

        chkRemember.Text = "Ghi nhớ đăng nhập";
        chkRemember.Location = new Point(68, 420);
        chkRemember.AutoSize = true;
        rightPanel.Controls.Add(chkRemember);

        var btnLogin = new Button
        {
             Text = "ĐĂNG NHẬP",
            Location = new Point(68, 465),
            Size = new Size(340, 48)
        };

        AppStyles.StyleLoginButton(btnLogin);
        rightPanel.Controls.Add(btnLogin);
        btnLogin.Click += BtnLogin_Click;

        var copyright = new Label
        {
            Text = "© 2026 SkyResidence",
            Dock = DockStyle.Bottom,
            Height = 35,
            ForeColor = AppStyles.Gray,
            TextAlign = ContentAlignment.MiddleCenter
        };
        rightPanel.Controls.Add(copyright);

        AcceptButton = btnLogin;
    }
    private void LoadRememberedLogin()
    {
        try
        {
            if (!File.Exists(rememberFile))
                return;

            string[] lines = File.ReadAllLines(rememberFile);
            if (lines.Length < 2)
                return;

            string username = Encoding.UTF8.GetString(
                Convert.FromBase64String(lines[0]));

            byte[] encryptedPassword = Convert.FromBase64String(lines[1]);

            byte[] passwordBytes = ProtectedData.Unprotect(
                encryptedPassword,
                null,
                DataProtectionScope.CurrentUser);
             string password = Encoding.UTF8.GetString(passwordBytes);

            txtUsername.Text = username;
            txtPassword.Text = password;
            chkRemember.Checked = true;
        }
        catch
        {
            ClearRememberedLogin();
            txtUsername.Clear();
            txtPassword.Clear();
            chkRemember.Checked = false;
        }
    }

    private void SaveRememberedLogin(string username, string password)
    {
        try
        {
            string folder = Path.GetDirectoryName(rememberFile)!;
            Directory.CreateDirectory(folder);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] encryptedPassword = ProtectedData.Protect(
                passwordBytes,
                null,
                DataProtectionScope.CurrentUser);

            string[] lines =
            {
                Convert.ToBase64String(Encoding.UTF8.GetBytes(username)),
                Convert.ToBase64String(encryptedPassword)
            };

            File.WriteAllLines(rememberFile, lines);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không thể lưu thông tin đăng nhập:\n" + ex.Message,
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ClearRememberedLogin()
    {
        try
        {
            if (File.Exists(rememberFile))
                File.Delete(rememberFile);
        }
        catch
        {
            // Không làm gián đoạn quá trình đăng nhập.
        }
    }
    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show(
                "Vui lòng nhập đầy đủ tài khoản và mật khẩu!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        try
        {
            AuthUser? user = authService.Login(username, password);

            if (user == null)
            {
                MessageBox.Show(
                    "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa!",
                    "Đăng nhập thất bại",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }
            if (chkRemember.Checked)
                SaveRememberedLogin(username, password);
            else
                ClearRememberedLogin();

            Hide();

                using (var main = new MainForm(user))
                {
                    main.ShowDialog();
                }

                if (!IsDisposed)
                {
                    txtPassword.Clear();
                    Show();
                    txtUsername.Focus();
                }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không thể đăng nhập. Vui lòng kiểm tra kết nối CSDL và cấu trúc bảng tài khoản.\n\n"
                + ex.Message,
                "Lỗi đăng nhập",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}