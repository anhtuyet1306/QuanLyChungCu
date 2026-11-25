using System.Drawing;
using System.Windows.Forms;

namespace QuanLyChungCu_SkyResidence.Pages;

public class DashboardPage : UserControl
{
    private readonly Color navy = Color.FromArgb(18, 53, 94);
    private readonly Color background = Color.FromArgb(244, 248, 255);

    public DashboardPage()
    {
        Dock = DockStyle.Fill;
        BackColor = background;
        Padding = new Padding(22);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            BackColor = background
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        Controls.Add(layout);

        var stats = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = background,
            Margin = new Padding(0)
        };

        for (int i = 0; i < 4; i++)
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        stats.Controls.Add(Stat("120", "TỔNG SỐ CĂN HỘ", Color.FromArgb(224, 239, 255)), 0, 0);
        stats.Controls.Add(Stat("350", "TỔNG SỐ CƯ DÂN", Color.FromArgb(225, 248, 237)), 1, 0);
        stats.Controls.Add(Stat("18", "HÓA ĐƠN CHƯA ĐÓNG", Color.FromArgb(255, 241, 220)), 2, 0);
        stats.Controls.Add(Stat("05", "SỰ CỐ ĐANG XỬ LÝ", Color.FromArgb(239, 231, 255)), 3, 0);

        layout.Controls.Add(stats, 0, 0);
        layout.SetColumnSpan(stats, 2);

        layout.Controls.Add(Card("THỐNG KÊ THU PHÍ",
            "Phí quản lý: 120.000.000 đ\n\n" +
            "Điện, nước: 85.000.000 đ\n\n" +
            "Gửi xe: 32.000.000 đ\n\n" +
            "Tổng thu tháng này: 237.000.000 đ"), 0, 1);

        layout.Controls.Add(Card("THÔNG BÁO MỚI NHẤT",
            "• 18 hóa đơn chưa thanh toán.\n\n" +
            "• 02 phản ánh đang chờ xử lý.\n\n" +
            "• Lịch bảo trì thang máy ngày 15/11."), 1, 1);

        layout.Controls.Add(Card("DANH SÁCH CĂN HỘ",
            "A101     Nguyễn Văn An       Đang ở\n\n" +
            "A102     Trần Thị Mai          Đang ở\n\n" +
            "B201     Lê Hoàng Nam          Đang ở\n\n" +
            "B202     Phạm Thị Hồng         Còn trống"), 0, 2);

        layout.Controls.Add(Card("TRUY CẬP NHANH",
            "Chọn các mục ở thanh điều hướng bên trái để quản lý:\n\n" +
            "• Cư dân và căn hộ\n" +
            "• Phí dịch vụ và thanh toán\n" +
            "• Đăng ký tiện ích\n" +
            "• Sự cố và phản ánh\n" +
            "• Báo cáo và thống kê"), 1, 2);
    }

    private Control Stat(string value, string label, Color color)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = color,
            Margin = new Padding(0, 0, 10, 8),
            Padding = new Padding(12)
        };

        panel.Controls.Add(new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = navy,
            Location = new Point(12, 10),
            AutoSize = true
        });

        panel.Controls.Add(new Label
        {
            Text = label,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = navy,
            Location = new Point(14, 60),
            AutoSize = true
        });

        return panel;
    }

    private Control Card(string title, string content)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(16),
            Margin = new Padding(5)
        };

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 38,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = navy
        };

        var contentLabel = new Label
        {
            Text = content,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(55, 65, 81),
            Padding = new Padding(2, 10, 2, 2),
            AutoEllipsis = true
        };

        panel.Controls.Add(contentLabel);
        panel.Controls.Add(titleLabel);
        return panel;
    }
}
