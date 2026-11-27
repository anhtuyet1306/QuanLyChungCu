using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Services;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Repositories;

namespace QuanLyChungCu_SkyResidence.Pages;

public class ReportsPage : BasePage
{
    private readonly ReportsRepository repository = new();
    private readonly CanHoRepository canHoRepository = new();
    private readonly CuDanRepository cuDanRepository = new();
    private readonly HoaDonRepository hoaDonRepository = new();

    private readonly Panel reportPanel = new();
    private readonly ComboBox cboNam = new();
    private readonly Label lblTrangThai = new();

    private readonly DataGridView revenueGrid = new();
    private readonly DataGridView feeGrid = new();

    private readonly RevenueChart monthlyChart = new();
    private readonly FeeChart feeChart = new();

    private readonly Label lblCanHo = CreateValueLabel();
    private readonly Label lblCuDan = CreateValueLabel();
    private readonly Label lblHoaDon = CreateValueLabel();
    private readonly Label lblDoanhThu = CreateValueLabel();
    private readonly Label lblCongNo = CreateValueLabel();
    private readonly Label lblQuaHan = CreateValueLabel();

    private readonly List<(int Thang, decimal TongThu)> revenueData = new();
    private readonly List<(string TenPhi, decimal TongThu)> feeData = new();

    public ReportsPage(AuthUser user)
    : base(
        "BÁO CÁO - THỐNG KÊ",
        "Tổng hợp doanh thu, công nợ và tình hình cư dân.",
        user,
        "BAOCAO")
{
    Grid.Visible = false;

    reportPanel.Dock = DockStyle.Fill;
    reportPanel.BackColor = Color.FromArgb(244, 248, 255);
    reportPanel.Padding = new Padding(10);

    Controls.Add(reportPanel);
    reportPanel.BringToFront();

    BuildInterface();
}

    private void BuildInterface()
    {
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(244, 248, 255)
        };

        reportPanel.Controls.Add(scroll);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 1,
            RowCount = 6,
            AutoSize = true,
            BackColor = Color.FromArgb(244, 248, 255)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 290));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 290));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        scroll.Controls.Add(layout);

        // Tiêu đề
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(12, 5, 12, 5)
        };

        var title = new Label
        {
            Text = "BÁO CÁO - THỐNG KÊ",
            Dock = DockStyle.Top,
            Height = 29,
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        var description = new Label
        {
            Text = "Theo dõi tình hình hoạt động và tài chính chung cư",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.DimGray
        };

        header.Controls.Add(description);
        header.Controls.Add(title);
        layout.Controls.Add(header, 0, 0);

        // Sáu thẻ thống kê
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 2,
            Padding = new Padding(0, 8, 0, 5),
            BackColor = Color.FromArgb(244, 248, 255)
        };

        for (int i = 0; i < 3; i++)
            cards.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33f)
            );

        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        cards.Controls.Add(CreateCard("TỔNG CĂN HỘ", lblCanHo, ShowApartments), 0, 0);
        cards.Controls.Add(CreateCard("TỔNG CƯ DÂN", lblCuDan, ShowResidents), 1, 0);
        cards.Controls.Add(CreateCard("HÓA ĐƠN CHƯA ĐÓNG", lblHoaDon, ShowUnpaidInvoices), 2, 0);
        cards.Controls.Add(CreateCard("DOANH THU THÁNG NÀY", lblDoanhThu, ShowMonthlyRevenue), 0, 1);
        cards.Controls.Add(CreateCard("TỔNG CÔNG NỢ", lblCongNo, ShowDebtInvoices), 1, 1);
        cards.Controls.Add(CreateCard("HÓA ĐƠN QUÁ HẠN", lblQuaHan, ShowOverdueInvoices), 2, 1);

        layout.Controls.Add(cards, 0, 1);

        // Bộ lọc năm
        var filter = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10, 5, 10, 5)
        };

        int currentYear = DateTime.Now.Year;

        var lblNam = new Label
        {
            Text = "Chọn năm:",
            Location = new Point(10, 10),
            Size = new Size(75, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(18, 53, 94),
            TextAlign = ContentAlignment.MiddleLeft
        };

        cboNam.DropDownStyle = ComboBoxStyle.DropDownList;
        cboNam.Location = new Point(90, 7);
        cboNam.Size = new Size(130, 28);
        cboNam.Font = new Font("Segoe UI", 9);

        for (int year = currentYear - 4; year <= currentYear; year++)
            cboNam.Items.Add(year);

        cboNam.SelectedItem = currentYear;

        var btnThongKe = new Button
        {
            Text = "XEM BÁO CÁO",
            Location = new Point(240, 5),
            Size = new Size(135, 31),
            BackColor = Color.FromArgb(25, 135, 84),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        btnThongKe.FlatAppearance.BorderSize = 0;
        btnThongKe.Click += (_, _) => Safe(LoadRevenue);

        filter.Controls.Add(lblNam);
        filter.Controls.Add(cboNam);
        filter.Controls.Add(btnThongKe);

        layout.Controls.Add(filter, 0, 2);

        // Hai biểu đồ
        var charts = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.FromArgb(244, 248, 255)
        };

        charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));

        var monthlySection = CreateSection("DOANH THU THEO THÁNG");
        var feeSection = CreateSection("DOANH THU THEO LOẠI PHÍ");

        monthlyChart.Dock = DockStyle.Fill;
        monthlyChart.BackColor = Color.White;

        feeChart.Dock = DockStyle.Fill;
        feeChart.BackColor = Color.White;

        monthlySection.Controls.Add(monthlyChart);
        feeSection.Controls.Add(feeChart);

        charts.Controls.Add(monthlySection, 0, 0);
        charts.Controls.Add(feeSection, 1, 0);

        layout.Controls.Add(charts, 0, 3);

        // Hai bảng dữ liệu
        var tables = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.FromArgb(244, 248, 255)
        };

        tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var revenueSection = CreateSection("CHI TIẾT DOANH THU THEO THÁNG");
        var feeTableSection = CreateSection("CHI TIẾT DOANH THU THEO LOẠI PHÍ");

        ConfigureGrid(revenueGrid);
        ConfigureGrid(feeGrid);

        revenueGrid.Columns.Add("Thang", "Tháng");
        revenueGrid.Columns.Add("Nam", "Năm");
        revenueGrid.Columns.Add("SoGiaoDich", "Số giao dịch");
        revenueGrid.Columns.Add("TongThu", "Tổng doanh thu");

        feeGrid.Columns.Add("MaLoaiPhi", "Mã phí");
        feeGrid.Columns.Add("TenPhi", "Tên phí");
        feeGrid.Columns.Add("TongTien", "Doanh thu");

        revenueSection.Controls.Add(revenueGrid);
        feeTableSection.Controls.Add(feeGrid);

        tables.Controls.Add(revenueSection, 0, 0);
        tables.Controls.Add(feeTableSection, 1, 0);

        layout.Controls.Add(tables, 0, 4);

        // Trạng thái
        lblTrangThai.Dock = DockStyle.Fill;
        lblTrangThai.Font = new Font("Segoe UI", 9);
        lblTrangThai.ForeColor = Color.DimGray;
        lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;

        layout.Controls.Add(lblTrangThai, 0, 5);
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.Font = new Font("Segoe UI", 9);
        grid.ColumnHeadersHeight = 35;
        grid.RowTemplate.Height = 30;
        grid.EnableHeadersVisualStyles = false;

        grid.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(222, 235, 252);

        grid.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.FromArgb(18, 53, 94);

        grid.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 9, FontStyle.Bold);
    }

    private static Panel CreateCard(string title, Label value, Action onClick)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(5),
            Padding = new Padding(12),
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Hand
        };

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 25,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.DimGray,
            Cursor = Cursors.Hand
        };

        value.Dock = DockStyle.Fill;
        value.TextAlign = ContentAlignment.MiddleLeft;
        value.Cursor = Cursors.Hand;

        card.Controls.Add(value);
        card.Controls.Add(titleLabel);

        card.Click += (_, _) => onClick();
        titleLabel.Click += (_, _) => onClick();
        value.Click += (_, _) => onClick();

        card.MouseEnter += (_, _) =>
            card.BackColor = Color.FromArgb(230, 240, 255);

        card.MouseLeave += (_, _) =>
            card.BackColor = Color.White;

        titleLabel.MouseEnter += (_, _) =>
            card.BackColor = Color.FromArgb(230, 240, 255);

        value.MouseEnter += (_, _) =>
            card.BackColor = Color.FromArgb(230, 240, 255);

        titleLabel.MouseLeave += (_, _) =>
            card.BackColor = Color.White;

        value.MouseLeave += (_, _) =>
            card.BackColor = Color.White;

        return card;
    }

    private static Label CreateValueLabel()
    {
        return new Label
        {
            Text = "0",
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            ForeColor = Color.FromArgb(32, 120, 85),
            AutoSize = false
        };
    }

    private static Panel CreateSection(string title)
    {
        var section = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10),
            Margin = new Padding(5)
        };

        var heading = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        section.Controls.Add(heading);
        return section;
    }

    protected override void LoadData()
    {
        try
        {
            LoadDashboard();
            LoadRevenue();
        }
        catch (Exception ex)
        {
            ShowError("Không thể tải dữ liệu báo cáo:\n\n" + ex.Message);
        }
    }

    private void LoadDashboard()
    {
        var dashboard = repository.GetDashboard();
        var debt = repository.GetDebtReport();

        lblCanHo.Text = dashboard.TongCanHo.ToString("N0");
        lblCuDan.Text = dashboard.TongCuDan.ToString("N0");
        lblHoaDon.Text = dashboard.HoaDonChuaDong.ToString("N0");
        lblDoanhThu.Text = dashboard.TongThuThangNay.ToString("N0") + " đ";
        lblCongNo.Text = debt.TongCongNo.ToString("N0") + " đ";
        lblQuaHan.Text = debt.SoHoaDonQuaHan.ToString("N0");
    }

    private void LoadRevenue()
    {
        if (cboNam.SelectedItem == null)
            return;

        int year = Convert.ToInt32(cboNam.SelectedItem);

        var revenues = repository.GetRevenueByYear(year);
        var feeRevenues = repository.GetRevenueByFee(year);

        revenueGrid.Rows.Clear();
        feeGrid.Rows.Clear();

        revenueData.Clear();
        feeData.Clear();

        foreach (var item in revenues)
        {
            revenueGrid.Rows.Add(
                "Tháng " + item.Thang,
                item.Nam,
                item.SoGiaoDich.ToString("N0"),
                item.TongThu.ToString("N0") + " đ"
            );

            revenueData.Add((item.Thang, item.TongThu));
        }

        foreach (var item in feeRevenues)
        {
            feeGrid.Rows.Add(
                item.MaLoaiPhi,
                item.TenPhi,
                item.TongTien.ToString("N0") + " đ"
            );

            feeData.Add((item.TenPhi, item.TongTien));
        }

        monthlyChart.SetData(revenueData);
        feeChart.SetData(feeData);

        lblTrangThai.Text =
            $"Báo cáo năm {year}: {revenues.Count} tháng có doanh thu, " +
            $"{feeRevenues.Count} loại phí có doanh thu.";
    }

    private void ShowDetails<T>(string title, List<T> items)
    {
        var form = new Form
        {
            Text = title,
            StartPosition = FormStartPosition.CenterParent,
            Size = new Size(1000, 600),
            MinimumSize = new Size(700, 400),
            BackColor = Color.White,
            Font = new Font("Segoe UI", 9)
        };

        var heading = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(15, 12, 0, 0),
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            DataSource = items
        };

        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(222, 235, 252);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(18, 53, 94);
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        grid.ColumnHeadersHeight = 36;
        grid.RowTemplate.Height = 30;

        form.Controls.Add(grid);
        form.Controls.Add(heading);

        form.ShowDialog(this);
    }

    private void ShowApartments()
    {
        Safe(() =>
        {
            var items = canHoRepository.GetAll()
                .Select(x => new
                {
                    MaCanHo = x.MaCH,
                    Toa = x.MaToa,
                    Tang = x.Tang,
                    DienTich = x.DienTich,
                    SoXeMay = x.SoXeMay,
                    SoOTo = x.SoOTo,
                    ChuHo = x.ChuHo,
                    TrangThai = x.TrangThai
                })
                .ToList();

            ShowDetails("DANH SÁCH CĂN HỘ", items);
        });
    }

    private void ShowResidents()
    {
        Safe(() =>
        {
            var items = cuDanRepository.GetAll()
                .Select(x => new
                {
                    MaCuDan = x.MaCD,
                    HoTen = x.HoTen,
                    NgaySinh = x.NgaySinh,
                    SoDienThoai = x.SoDienThoai,
                    MaCanHo = x.MaCH,
                    VaiTro = x.VaiTro
                })
                .ToList();

            ShowDetails("DANH SÁCH CƯ DÂN", items);
        });
    }

    private void ShowUnpaidInvoices()
    {
        Safe(() =>
        {
            var items = hoaDonRepository.GetAll()
                .Where(x => x.ConNo > 0)
                .Select(x => new
                {
                    MaHoaDon = x.MaHoaDon,
                    MaCanHo = x.MaCH,
                    KyThanhToan = x.KyThanhToan,
                    HanThanhToan = x.HanThanhToan,
                    TongTien = x.TongTien,
                    DaThu = x.DaThu,
                    ConNo = x.ConNo,
                    TrangThai = x.TrangThai,
                    QuaHan = x.QuaHan
                })
                .ToList();

            ShowDetails("DANH SÁCH HÓA ĐƠN CHƯA THANH TOÁN HẾT", items);
        });
    }

    private void ShowMonthlyRevenue()
    {
        Safe(() =>
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            var items = repository.GetRevenueByYear(year)
                .Where(x => x.Thang == month)
                .Select(x => new
                {
                    Thang = x.Thang,
                    Nam = x.Nam,
                    SoGiaoDich = x.SoGiaoDich,
                    TongDoanhThu = x.TongThu
                })
                .ToList();

            ShowDetails("DOANH THU THÁNG NÀY", items);
        });
    }

    private void ShowDebtInvoices()
    {
        Safe(() =>
        {
            var items = hoaDonRepository.GetAll()
                .Where(x => x.ConNo > 0)
                .Select(x => new
                {
                    MaHoaDon = x.MaHoaDon,
                    MaCanHo = x.MaCH,
                    KyThanhToan = x.KyThanhToan,
                    HanThanhToan = x.HanThanhToan,
                    TongTien = x.TongTien,
                    DaThu = x.DaThu,
                    ConNo = x.ConNo,
                    TrangThai = x.TrangThai,
                    QuaHan = x.QuaHan
                })
                .ToList();

            ShowDetails("CHI TIẾT CÔNG NỢ", items);
        });
    }

    private void ShowOverdueInvoices()
    {
        Safe(() =>
        {
            var items = hoaDonRepository.GetAll()
                .Where(x => x.ConNo > 0 && x.QuaHan > 0)
                .Select(x => new
                {
                    MaHoaDon = x.MaHoaDon,
                    MaCanHo = x.MaCH,
                    KyThanhToan = x.KyThanhToan,
                    HanThanhToan = x.HanThanhToan,
                    TongTien = x.TongTien,
                    DaThu = x.DaThu,
                    ConNo = x.ConNo,
                    TrangThai = x.TrangThai,
                    SoNgayQuaHan = x.QuaHan
                })
                .ToList();

            ShowDetails("DANH SÁCH HÓA ĐƠN QUÁ HẠN", items);
        });
    }

    // Biểu đồ doanh thu theo tháng
    private class RevenueChart : Panel
    {
        private readonly List<(int Thang, decimal TongThu)> data = new();

        public void SetData(List<(int Thang, decimal TongThu)> values)
        {
            data.Clear();
            data.AddRange(values);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int left = 65;
            int right = Width - 25;
            int top = 25;
            int bottom = Height - 45;

            if (right <= left || bottom <= top)
                return;

            decimal maxValue = data.Count == 0 ? 0 : data.Max(x => x.TongThu);

            if (maxValue <= 0)
            {
                DrawEmpty(g, "Chưa có dữ liệu doanh thu trong năm này.");
                return;
            }

            using var gridPen = new Pen(Color.FromArgb(225, 230, 238));
            using var barBrush = new SolidBrush(Color.FromArgb(43, 125, 232));
            using var textBrush = new SolidBrush(Color.FromArgb(80, 90, 105));
            using var font = new Font("Segoe UI", 8);

            int chartHeight = bottom - top;

            for (int i = 0; i <= 4; i++)
            {
                int y = top + chartHeight * i / 4;
                g.DrawLine(gridPen, left, y, right, y);

                decimal value = maxValue * (4 - i) / 4;
                string label = (value / 1000000m).ToString("N1") + " tr";

                SizeF size = g.MeasureString(label, font);
                g.DrawString(label, font, textBrush, left - size.Width - 8, y - 8);
            }

            float slotWidth = (float)(right - left) / 12;
            float barWidth = Math.Min(32, slotWidth * 0.55f);

            for (int month = 1; month <= 12; month++)
            {
                decimal value = data
                    .Where(x => x.Thang == month)
                    .Select(x => x.TongThu)
                    .FirstOrDefault();

                float barHeight = (float)(value / maxValue * chartHeight);

                float x = left + (month - 1) * slotWidth + (slotWidth - barWidth) / 2;
                float y = bottom - barHeight;

                if (value > 0)
                    g.FillRectangle(barBrush, x, y, barWidth, barHeight);

                string label = "T" + month;
                SizeF size = g.MeasureString(label, font);

                g.DrawString(label, font, textBrush, x + (barWidth - size.Width) / 2, bottom + 8);
            }
        }
    }

    // Biểu đồ doanh thu theo loại phí
    private class FeeChart : Panel
    {
        private readonly List<(string TenPhi, decimal TongThu)> data = new();

        public void SetData(List<(string TenPhi, decimal TongThu)> values)
        {
            data.Clear();
            data.AddRange(values);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (data.Count == 0 || data.All(x => x.TongThu <= 0))
            {
                DrawEmpty(g, "Chưa có dữ liệu doanh thu theo loại phí.");
                return;
            }

            using var font = new Font("Segoe UI", 8);
            using var textBrush = new SolidBrush(Color.FromArgb(65, 75, 90));
            using var barBrush = new SolidBrush(Color.FromArgb(43, 125, 232));
            using var valueBrush = new SolidBrush(Color.FromArgb(18, 53, 94));

            int left = 12;
            int top = 18;
            int rowHeight = Math.Max(28, (Height - 35) / data.Count);
            int labelWidth = Math.Min(130, Width / 3);
            int barStart = left + labelWidth;
            int barMaxWidth = Width - barStart - 85;

            decimal maxValue = data.Max(x => x.TongThu);

            if (barMaxWidth <= 10)
                return;

            for (int i = 0; i < data.Count; i++)
            {
                int y = top + i * rowHeight;
                var item = data[i];

                string name = item.TenPhi.Length > 19
                    ? item.TenPhi.Substring(0, 17) + "..."
                    : item.TenPhi;

                g.DrawString(name, font, textBrush, left, y + 5);

                float barWidth = (float)(item.TongThu / maxValue * barMaxWidth);

                g.FillRectangle(
                    barBrush,
                    barStart,
                    y + 4,
                    Math.Max(2, barWidth),
                    16
                );

                string amount = (item.TongThu / 1000000m).ToString("N2") + " tr";

                g.DrawString(
                    amount,
                    font,
                    valueBrush,
                    barStart + barWidth + 5,
                    y + 5
                );
            }
        }
    }

    private static void DrawEmpty(Graphics g, string message)
    {
        using var font = new Font("Segoe UI", 10);
        SizeF size = g.MeasureString(message, font);

        g.DrawString(
            message,
            font,
            Brushes.Gray,
            Math.Max(0, (g.VisibleClipBounds.Width - size.Width) / 2),
            Math.Max(0, (g.VisibleClipBounds.Height - size.Height) / 2)
        );
    }
}