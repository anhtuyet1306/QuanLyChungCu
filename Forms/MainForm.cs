using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Pages;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Forms;

public class MainForm : Form
{
    private readonly Color navy = Color.FromArgb(18, 53, 94);
    private readonly Color background = Color.FromArgb(244, 248, 255);
    private readonly Panel pageHost = new();
    private readonly AuthUser currentUser;

    public MainForm(AuthUser user)
    {
        currentUser = user ?? throw new ArgumentNullException(nameof(user));

        Text = "Hệ thống Quản lý Chung cư - SkyResidence";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);
        Size = new Size(1450, 850);
        Font = new Font("Segoe UI", 10);
        BackColor = background;
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 235));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(root);

        root.Controls.Add(BuildSidebar(), 0, 0);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = background,
            Margin = new Padding(0)
        };

        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(right, 1, 0);
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(24, 0, 20, 0)
        };

        var title = new Label
        {
            Text = "HỆ THỐNG QUẢN LÝ CHUNG CƯ",
            Dock = DockStyle.Left,
            Width = 650,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            ForeColor = navy
        };

        var userLabel = new Label
        {
            Text = $"{currentUser.TenDangNhap}  |  {GetRoleDisplayName(currentUser.MaVaiTro)}",
            Dock = DockStyle.Right,
            Width = 260,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = navy
        };
        header.Controls.Add(userLabel);
        header.Controls.Add(title);
        right.Controls.Add(header, 0, 0);

        pageHost.Dock = DockStyle.Fill;
        pageHost.BackColor = background;
        pageHost.Padding = new Padding(0);
        right.Controls.Add(pageHost, 0, 1);

        OpenInitialPage();
    }

    private string GetRoleDisplayName(string maVaiTro)
    {
        return maVaiTro.ToUpperInvariant() switch
        {
            "ADMIN" => "Quản trị viên",
            "KETOAN" => "Kế toán",
            "KYTHUAT" => "Kỹ thuật",
            "LETAN" => "Lễ tân",
            _ => maVaiTro
        };
    }
    private Control BuildSidebar()
    {
        var side = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = navy,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 135));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

        var brand = new Label
        {
            Text = "🏢\nSKYRESIDENCE\nQUẢN LÝ CHUNG CƯ",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            BackColor = navy
        };
        side.Controls.Add(brand, 0, 0);
         var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(8, 10, 8, 10),
            BackColor = navy,
            Margin = new Padding(0)
        };
        side.Controls.Add(menu, 0, 1);

        AddMenu(menu, "DASHBOARD", "Trang chủ",
            () => ShowPage(new DashboardPage(currentUser)));

        AddMenu(menu, "CUDAN", "Quản lý cư dân",
            () => ShowPage(new ResidentsPage(currentUser)));

        AddMenu(menu, "CANHO", "Quản lý căn hộ",
            () => ShowPage(new ApartmentsPage(currentUser)));

        AddMenu(menu, "PHI", "Quản lý phí dịch vụ",
            () => ShowPage(new FeesPage(currentUser)));

        AddMenu(menu, "THANHTOAN", "Quản lý thanh toán",
            () => ShowPage(new PaymentsPage(currentUser)));

        AddMenu(menu, "TIENICH", "Đăng ký tiện ích",
            () => ShowPage(new AmenitiesPage(currentUser)));

        AddMenu(menu, "SUCO", "Phản ánh sự cố",
            () => ShowPage(new IncidentsPage(currentUser)));

        AddMenu(menu, "BAOCAO", "Báo cáo - Thống kê",
            () => ShowPage(new ReportsPage(currentUser)));

        AddMenu(menu, "HETHONG", "Cài đặt",
            () => ShowPage(new SettingsPage(currentUser)));
        var logout = new Button
        {
            Text = "↪  Đăng xuất",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(13, 42, 76),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        logout.FlatAppearance.BorderSize = 0;
        logout.Click += (_, _) =>
        {
            var result = MessageBox.Show(
                "Bạn có muốn đăng xuất khỏi tài khoản này?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Close();
            }
        };
        side.Controls.Add(logout, 0, 2);
        return side;
    }

    private void AddMenu(
        FlowLayoutPanel menu,
        string maChucNang,
        string text,
        Action action)
    {
        if (!currentUser.HasPermission(maChucNang, "xem"))
            return;

        var button = new Button
        {
            Text = "   " + text,
            Width = 203,
            Height = 46,
            FlatStyle = FlatStyle.Flat,
            BackColor = navy,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Margin = new Padding(0, 3, 0, 3),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 87, 145);
        button.Click += (_, _) => action();
        menu.Controls.Add(button);
    }

    private void OpenInitialPage()
    {
        if (currentUser.HasPermission("DASHBOARD", "xem"))
        {
            ShowPage(new DashboardPage(currentUser));
            return;
        }

        if (currentUser.HasPermission("CUDAN", "xem"))
            ShowPage(new ResidentsPage(currentUser));
        else if (currentUser.HasPermission("CANHO", "xem"))
            ShowPage(new ApartmentsPage(currentUser));
        else if (currentUser.HasPermission("PHI", "xem"))
            ShowPage(new FeesPage(currentUser));
        else if (currentUser.HasPermission("THANHTOAN", "xem"))
            ShowPage(new PaymentsPage(currentUser));
        else if (currentUser.HasPermission("TIENICH", "xem"))
            ShowPage(new AmenitiesPage(currentUser));
        else if (currentUser.HasPermission("SUCO", "xem"))
            ShowPage(new IncidentsPage(currentUser));
        else if (currentUser.HasPermission("BAOCAO", "xem"))
            ShowPage(new ReportsPage(currentUser));
        else if (currentUser.HasPermission("HETHONG", "xem"))
            ShowPage(new SettingsPage(currentUser));
        else
        {
            MessageBox.Show(
                "Tài khoản này chưa được cấp quyền xem chức năng nào.\nVui lòng liên hệ quản trị viên.",
                "Chưa được cấp quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            Shown += (_, _) => Close();
        }
    }

    private void ShowPage(UserControl page)
    {
        pageHost.SuspendLayout();
        pageHost.Controls.Clear();
        page.Dock = DockStyle.Fill;
        pageHost.Controls.Add(page);
        pageHost.ResumeLayout();
    }
}