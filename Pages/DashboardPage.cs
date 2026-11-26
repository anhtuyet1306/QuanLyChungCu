
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Repositories;

namespace QuanLyChungCu_SkyResidence.Pages;

public class DashboardPage : UserControl
{
    private readonly Color navy = Color.FromArgb(18, 53, 94);
    private readonly Color background = Color.FromArgb(244, 248, 255);

    private readonly ReportsRepository reports = new();
    private readonly CanHoRepository canHoRepository = new();
    private readonly CuDanRepository cuDanRepository = new();
    private readonly HoaDonRepository hoaDonRepository = new();

    private readonly Label lblCanHo = new();
    private readonly Label lblCuDan = new();
    private readonly Label lblHoaDon = new();
    private readonly Label lblSuCo = new();

    private readonly Panel chart = new();
    private readonly DataGridView recentGrid = new();

    private readonly System.Collections.Generic.List<(int Month, decimal Value)> revenue = new();

    public DashboardPage()
    {
        Dock = DockStyle.Fill;
        BackColor = background;
        Padding = new Padding(18);

        BuildInterface();

        LoadDashboard();
    }

    private void BuildInterface()
    {
        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = background
        };

        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 52));

        Controls.Add(main);

        // Tiêu đề
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = background
        };

        header.Controls.Add(new Label
        {
            Text = "TỔNG QUAN SKYRESIDENCE",
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            ForeColor = navy,
            Location = new Point(5, 5),
            AutoSize = true
        });

        header.Controls.Add(new Label
        {
            Text = "Theo dõi tình hình hoạt động chung cư",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            Location = new Point(7, 39),
            AutoSize = true
        });

        main.Controls.Add(header, 0, 0);

        // Bốn thẻ thống kê
        var stats = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = background
        };

        for (int i = 0; i < 4; i++)
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        stats.Controls.Add(CreateStat(
            "TỔNG SỐ CĂN HỘ", lblCanHo,
            Color.FromArgb(224, 239, 255),
            ShowApartments), 0, 0);

        stats.Controls.Add(CreateStat(
            "TỔNG SỐ CƯ DÂN", lblCuDan,
            Color.FromArgb(225, 248, 237),
            ShowResidents), 1, 0);

        stats.Controls.Add(CreateStat(
            "HÓA ĐƠN CÒN NỢ", lblHoaDon,
            Color.FromArgb(255, 241, 220),
            ShowUnpaidInvoices), 2, 0);

        stats.Controls.Add(CreateStat(
            "SỰ CỐ ĐANG XỬ LÝ", lblSuCo,
            Color.FromArgb(239, 231, 255),
            ShowIncidents), 3, 0);

        main.Controls.Add(stats, 0, 1);

        // Tiêu đề thống kê
        var sectionTitle = new Label
        {
            Text = "THỐNG KÊ HOẠT ĐỘNG",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = navy,
            TextAlign = ContentAlignment.MiddleLeft
        };

        main.Controls.Add(sectionTitle, 0, 2);

        // Khu vực biểu đồ và thông tin
        var middle = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = background
        };

        middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));

        var chartCard = CreateCard("DOANH THU THEO THÁNG");

        chart.Dock = DockStyle.Fill;
        chart.BackColor = Color.White;
        chart.Paint += DrawChart;

        chartCard.Controls.Add(chart);

        middle.Controls.Add(chartCard, 0, 0);

        var infoCard = CreateCard("THÔNG TIN HỆ THỐNG");

        var info = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(55, 65, 81),
            Padding = new Padding(5, 10, 5, 5),
            Text =
                "• Quản lý căn hộ và cư dân\n\n" +
                "• Theo dõi hóa đơn và thanh toán\n\n" +
                "• Quản lý đăng ký tiện ích\n\n" +
                "• Tiếp nhận và xử lý sự cố\n\n" +
                "• Báo cáo doanh thu và công nợ"
        };

        infoCard.Controls.Add(info);
        middle.Controls.Add(infoCard, 1, 0);

        main.Controls.Add(middle, 0, 3);

        // Danh sách hóa đơn gần đây
        var recentCard = CreateCard("DANH SÁCH HÓA ĐƠN CÒN NỢ");

        recentGrid.Dock = DockStyle.Fill;
        recentGrid.ReadOnly = true;
        recentGrid.AllowUserToAddRows = false;
        recentGrid.AllowUserToDeleteRows = false;
        recentGrid.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;
        recentGrid.BackgroundColor = Color.White;
        recentGrid.BorderStyle = BorderStyle.None;
        recentGrid.RowHeadersVisible = false;
        recentGrid.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        recentGrid.Font = new Font("Segoe UI", 9);
        recentGrid.ColumnHeadersHeight = 32;
        recentGrid.RowTemplate.Height = 28;

        recentGrid.EnableHeadersVisualStyles = false;
        recentGrid.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(222, 235, 252);
        recentGrid.ColumnHeadersDefaultCellStyle.ForeColor = navy;
        recentGrid.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 9, FontStyle.Bold);

        recentGrid.Columns.Add("MaHoaDon", "Mã hóa đơn");
        recentGrid.Columns.Add("MaCH", "Căn hộ");
        recentGrid.Columns.Add("KyThanhToan", "Kỳ thanh toán");
        recentGrid.Columns.Add("ConNo", "Còn nợ");

        recentCard.Controls.Add(recentGrid);

        main.Controls.Add(recentCard, 0, 4);
    }

    private Control CreateStat(
        string title,
        Label value,
        Color color,
        Action action)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = color,
            Margin = new Padding(4),
            Padding = new Padding(10),
            Cursor = Cursors.Hand
        };

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = navy,
            Cursor = Cursors.Hand
        };

        value.Text = "...";
        value.Dock = DockStyle.Fill;
        value.Font = new Font("Segoe UI", 21, FontStyle.Bold);
        value.ForeColor = navy;
        value.Cursor = Cursors.Hand;

        panel.Controls.Add(value);
        panel.Controls.Add(titleLabel);

        panel.Click += (_, _) => action();
        titleLabel.Click += (_, _) => action();
        value.Click += (_, _) => action();

        panel.MouseEnter += (_, _) =>
            panel.BorderStyle = BorderStyle.FixedSingle;

        panel.MouseLeave += (_, _) =>
            panel.BorderStyle = BorderStyle.None;

        return panel;
    }

    private Panel CreateCard(string title)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(5),
            Padding = new Padding(12)
        };

        var heading = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 32,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = navy
        };

        panel.Controls.Add(heading);

        return panel;
    }

    private void LoadDashboard()
    {
        try
        {
            var data = reports.GetDashboard();

            lblCanHo.Text = data.TongCanHo.ToString("N0");
            lblCuDan.Text = data.TongCuDan.ToString("N0");
            lblHoaDon.Text = data.HoaDonChuaDong.ToString("N0");
            lblSuCo.Text = data.SuCoDangXuLy.ToString("N0");

            LoadRevenue();
            LoadRecentInvoices();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không thể tải dữ liệu trang chủ:\n" + ex.Message,
                "SkyResidence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void LoadRevenue()
    {
        int year = DateTime.Now.Year;

        var data = reports.GetRevenueByYear(year);

        revenue.Clear();

        foreach (var item in data)
            revenue.Add((item.Thang, item.TongThu));

        chart.Invalidate();
    }

    private void LoadRecentInvoices()
    {
        var invoices = hoaDonRepository.GetAll()
            .Where(x => x.ConNo > 0)
            .OrderByDescending(x => x.HanThanhToan)
            .Take(10)
            .ToList();

        recentGrid.Rows.Clear();

        foreach (var item in invoices)
        {
            recentGrid.Rows.Add(
                item.MaHoaDon,
                item.MaCH,
                item.KyThanhToan,
                item.ConNo.ToString("N0") + " đ"
            );
        }
    }

    private void DrawChart(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int left = 65;
        int right = chart.Width - 20;
        int top = 25;
        int bottom = chart.Height - 35;

        if (right <= left || bottom <= top)
            return;

        decimal max = revenue.Count == 0
            ? 0
            : revenue.Max(x => x.Value);

        if (max <= 0)
        {
            using var font = new Font("Segoe UI", 10);
            const string message = "Chưa có dữ liệu doanh thu.";
            var size = g.MeasureString(message, font);

            g.DrawString(
                message,
                font,
                Brushes.Gray,
                (chart.Width - size.Width) / 2,
                (chart.Height - size.Height) / 2);

            return;
        }

        using var gridPen = new Pen(Color.FromArgb(225, 230, 238));
        using var barBrush = new SolidBrush(Color.FromArgb(43, 125, 232));
        using var textBrush = new SolidBrush(Color.Gray);
        using var labelFont = new Font("Segoe UI", 8);

        int height = bottom - top;

        for (int i = 0; i <= 4; i++)
        {
            int y = top + height * i / 4;

            g.DrawLine(gridPen, left, y, right, y);

            decimal value = max * (4 - i) / 4;

            g.DrawString(
                (value / 1000000).ToString("N0") + " tr",
                labelFont,
                textBrush,
                3,
                y - 8);
        }

        float slot = (float)(right - left) / 12;
        float barWidth = Math.Min(28, slot * 0.55f);

        for (int month = 1; month <= 12; month++)
        {
            decimal value = revenue
                .Where(x => x.Month == month)
                .Select(x => x.Value)
                .FirstOrDefault();

            float barHeight = (float)(value / max * height);

            float x = left + (month - 1) * slot
                + (slot - barWidth) / 2;

            float y = bottom - barHeight;

            if (value > 0)
                g.FillRectangle(barBrush, x, y, barWidth, barHeight);

            g.DrawString(
                "T" + month,
                labelFont,
                textBrush,
                x,
                bottom + 7);
        }
    }

    private void ShowApartments()
    {
        try
        {
            var items = canHoRepository.GetAll()
                .Select(x => new
                {
                    MaCanHo = x.MaCH,
                    Toa = x.MaToa,
                    Tang = x.Tang,
                    DienTich = x.DienTich,
                    TrangThai = x.TrangThai
                })
                .ToList();

            ShowDetails("DANH SÁCH CĂN HỘ", items);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ShowResidents()
    {
        try
        {
            var items = cuDanRepository.GetAll()
                .Select(x => new
                {
                    MaCuDan = x.MaCD,
                    HoTen = x.HoTen,
                    SoDienThoai = x.SoDienThoai,
                    MaCanHo = x.MaCH,
                    VaiTro = x.VaiTro
                })
                .ToList();

            ShowDetails("DANH SÁCH CƯ DÂN", items);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ShowUnpaidInvoices()
    {
        try
        {
            var items = hoaDonRepository.GetAll()
                .Where(x => x.ConNo > 0)
                .Select(x => new
                {
                    MaHoaDon = x.MaHoaDon,
                    MaCanHo = x.MaCH,
                    KyThanhToan = x.KyThanhToan,
                    TongTien = x.TongTien,
                    DaThu = x.DaThu,
                    ConNo = x.ConNo,
                    TrangThai = x.TrangThai
                })
                .ToList();

            ShowDetails("HÓA ĐƠN CÒN NỢ", items);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ShowIncidents()
    {
        MessageBox.Show(
            "Hãy mở mục Sự cố ở thanh điều hướng để xem và xử lý các phản ánh.",
            "SkyResidence",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ShowDetails<T>(string title, System.Collections.Generic.List<T> items)
    {
        var form = new Form
        {
            Text = title,
            StartPosition = FormStartPosition.CenterParent,
            Size = new Size(950, 550),
            BackColor = Color.White
        };

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            DataSource = items
        };

        form.Controls.Add(grid);
        form.ShowDialog(this);
    }

    private void ShowError(Exception ex)
    {
        MessageBox.Show(
            ex.Message,
            "SkyResidence",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
