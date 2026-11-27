
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Repositories;
using QuanLyChungCu_SkyResidence.Services;
namespace QuanLyChungCu_SkyResidence.Pages;

public class PaymentsPage : BasePage
{
    private readonly HoaDonRepository repository = new();

    private readonly string[] apartments =
    {
        "A101", "A102", "A201", "A202",
        "B101", "B201", "B202", "C301", "C302"
    };

    public PaymentsPage(AuthUser user)
    : base(
        "QUẢN LÝ THANH TOÁN",
        "Theo dõi hóa đơn, số tiền đã thu và công nợ còn lại.",
        user,
        "THANHTOAN")
    {
        AddColumn("Mã hóa đơn", nameof(HoaDon.MaHoaDon), 85, true);
        AddColumn("Căn hộ", nameof(HoaDon.MaCH), 70, true);
        AddColumn("Kỳ thanh toán", nameof(HoaDon.KyThanhToan), 90, true);
        AddColumn("Tổng tiền", nameof(HoaDon.TongTienHienThi), 110, true);
        AddColumn("Đã thu", nameof(HoaDon.DaThuHienThi), 100, true);
        AddColumn("Còn nợ", nameof(HoaDon.ConNoHienThi), 100, true);
        AddColumn("Trạng thái", nameof(HoaDon.TrangThai), 110, true);

        Grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (Grid.Columns[e.ColumnIndex].Name != nameof(HoaDon.TrangThai))
                return;

            string status = e.Value?.ToString() ?? "";

            e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            if (status.Contains("Đã thanh toán",
                StringComparison.OrdinalIgnoreCase))
                e.CellStyle.ForeColor = Color.SeaGreen;
            else if (status.Contains("Quá hạn",
                StringComparison.OrdinalIgnoreCase))
                e.CellStyle.ForeColor = Color.Firebrick;
            else
                e.CellStyle.ForeColor = Color.DarkOrange;
        };
    }

    protected override void LoadData()
    {
        List<HoaDon> invoices = repository.GetAll();
        Bind(invoices, $"Tổng số: {invoices.Count} hóa đơn");
    }

    protected override void OnSearch(string keyword)
    {
        List<HoaDon> invoices = repository.GetAll(keyword);
        Bind(invoices, $"Tìm thấy: {invoices.Count} hóa đơn");
    }

    // THÊM HÓA ĐƠN
    protected override void OnAdd()
    {
        HoaDon? invoice = ShowInvoiceDialog(null);

        if (invoice == null)
            return;

        if (repository.Exists(invoice.MaCH, invoice.Thang, invoice.Nam))
        {
            ShowWarning("Căn hộ này đã có hóa đơn trong tháng/năm đã chọn.");
            return;
        }

        repository.Insert(invoice);

        LoadData();
        ShowInfo("Thêm hóa đơn thành công!");
    }

    // SỬA HÓA ĐƠN
    protected override void OnEdit()
    {
        HoaDon? selected = SelectedItem<HoaDon>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn hóa đơn cần sửa.");
            return;
        }

        HoaDon? updated = ShowInvoiceDialog(selected);

        if (updated == null)
            return;

        if (repository.Exists(
            updated.MaCH,
            updated.Thang,
            updated.Nam,
            updated.MaHoaDon))
        {
            ShowWarning("Căn hộ này đã có hóa đơn trong tháng/năm đã chọn.");
            return;
        }

        repository.Update(updated);

        LoadData();
        ShowInfo("Cập nhật hóa đơn thành công!");
    }

    // XÓA HÓA ĐƠN
    protected override void OnDelete()
    {
        HoaDon? selected = SelectedItem<HoaDon>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn hóa đơn cần xóa.");
            return;
        }

        bool confirm = Confirm(
            $"Bạn có chắc chắn muốn xóa hóa đơn {selected.MaHoaDon}?\n\n" +
            "Các chi tiết hóa đơn liên quan cũng sẽ bị xóa.\n" +
            "Hóa đơn đã có giao dịch thanh toán sẽ không thể xóa.");

        if (!confirm)
            return;

        try
        {
            repository.Delete(selected.MaHoaDon);

            LoadData();
            ShowInfo("Xóa hóa đơn thành công!");
        }
        catch (Exception ex)
        {
            ShowError(
                "Không thể xóa hóa đơn này.\n" +
                "Có thể hóa đơn đã phát sinh giao dịch thanh toán.\n\n" +
                "Chi tiết: " + ex.Message);
        }
    }

    // GIAO DIỆN NHẬP THÔNG TIN HÓA ĐƠN
    private HoaDon? ShowInvoiceDialog(HoaDon? current)
    {
        using Form dialog = new()
        {
            Text = current == null ? "Thêm hóa đơn" : "Sửa hóa đơn",
            Size = new Size(440, 440),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            Font = new Font("Segoe UI", 9)
        };

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = 6
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (int i = 0; i < 6; i++)
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        ComboBox cboApartment = new()
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDown
        };

        cboApartment.Items.AddRange(apartments);

        if (current != null)
            cboApartment.Text = current.MaCH;

        NumericUpDown numMonth = new()
        {
            Minimum = 1,
            Maximum = 12,
            Value = current?.Thang ?? (byte)DateTime.Today.Month,
            Dock = DockStyle.Left,
            Width = 120
        };

        NumericUpDown numYear = new()
        {
            Minimum = 2000,
            Maximum = 2100,
            Value = current?.Nam ?? (short)DateTime.Today.Year,
            Dock = DockStyle.Left,
            Width = 120
        };

        DateTimePicker dateDue = new()
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Dock = DockStyle.Left,
            Width = 160,
            Value = current?.HanThanhToan ?? DateTime.Today.AddDays(15)
        };

        TextBox txtNote = new()
        {
            Dock = DockStyle.Fill,
            Text = current?.GhiChu ?? ""
        };

        AddField(layout, 0, "Căn hộ:", cboApartment);
        AddField(layout, 1, "Tháng:", numMonth);
        AddField(layout, 2, "Năm:", numYear);
        AddField(layout, 3, "Hạn thanh toán:", dateDue);
        AddField(layout, 4, "Ghi chú:", txtNote);

        FlowLayoutPanel buttons = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };

        Button btnSave = new()
        {
            Text = "Lưu",
            Width = 90,
            Height = 32,
            DialogResult = DialogResult.OK
        };

        Button btnCancel = new()
        {
            Text = "Hủy",
            Width = 90,
            Height = 32,
            DialogResult = DialogResult.Cancel
        };

        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);

        layout.Controls.Add(buttons, 1, 5);

        dialog.Controls.Add(layout);
        dialog.AcceptButton = btnSave;
        dialog.CancelButton = btnCancel;

        if (dialog.ShowDialog() != DialogResult.OK)
            return null;

        string maCH = cboApartment.Text.Trim();

        if (string.IsNullOrWhiteSpace(maCH))
        {
            ShowWarning("Vui lòng nhập mã căn hộ.");
            return null;
        }

        if (dateDue.Value.Date < DateTime.Today.Date &&
            current == null)
        {
            ShowWarning("Hạn thanh toán không được trước ngày hiện tại.");
            return null;
        }

        if (dateDue.Value.Date < (current?.NgayLap ?? DateTime.Today).Date)
        {
            ShowWarning("Hạn thanh toán không được trước ngày lập hóa đơn.");
            return null;
        }

        return new HoaDon
        {
            MaHoaDon = current?.MaHoaDon ?? "",
            MaCH = maCH,
            Thang = (byte)numMonth.Value,
            Nam = (short)numYear.Value,
            HanThanhToan = dateDue.Value.Date,
            MaNV = current?.MaNV,
            GhiChu = txtNote.Text.Trim()
        };
    }

    private static void AddField(
        TableLayoutPanel layout,
        int row,
        string labelText,
        Control input)
    {
        Label label = new()
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        input.Margin = new Padding(3, 7, 3, 7);

        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(input, 1, row);
    }
}
