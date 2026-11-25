
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Repositories;

namespace QuanLyChungCu_SkyResidence.Pages;

public class FeesPage : BasePage
{
    private readonly LoaiPhiRepository repository = new();

    public FeesPage()
        : base("QUẢN LÝ PHÍ DỊCH VỤ",
            "Quản lý các khoản phí và đơn giá áp dụng tại chung cư.")
    {
        AddColumn("Mã phí", nameof(LoaiPhi.MaLoaiPhi), 75, true);
        AddColumn("Tên khoản phí", nameof(LoaiPhi.TenPhi), 150);
        AddColumn("Đơn vị tính", nameof(LoaiPhi.DonViTinh), 85, true);
        AddColumn("Đơn giá", nameof(LoaiPhi.DonGiaHienThi), 105, true);
        AddColumn("Cách tính", nameof(LoaiPhi.CachTinh), 110);
        AddColumn("Chu kỳ", nameof(LoaiPhi.ChuKy), 100, true);
        AddColumn("Trạng thái", nameof(LoaiPhi.TrangThai), 110, true);

        GetColumn(nameof(LoaiPhi.DonGiaHienThi))
            .DefaultCellStyle.ForeColor = Color.FromArgb(32, 120, 85);

        GetColumn(nameof(LoaiPhi.DonGiaHienThi))
            .DefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
    }

    // ==============================
    // TẢI DỮ LIỆU
    // ==============================

    protected override void LoadData()
    {
        var danhSach = repository.GetAll();

        Bind(danhSach, $"Hiển thị {danhSach.Count} khoản phí");
    }

    // ==============================
    // TÌM KIẾM
    // ==============================

    protected override void OnSearch(string keyword)
    {
        var danhSach = repository.GetAll();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            danhSach = danhSach.Where(p =>
                p.MaLoaiPhi.Contains(keyword,
                    StringComparison.OrdinalIgnoreCase) ||
                p.TenPhi.Contains(keyword,
                    StringComparison.OrdinalIgnoreCase) ||
                p.DonViTinh.Contains(keyword,
                    StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        Bind(danhSach, $"Tìm thấy {danhSach.Count} khoản phí");
    }

    // ==============================
    // THÊM MỚI
    // ==============================

    protected override void OnAdd()
    {
        var phi = ShowFeeDialog(null);

        if (phi == null)
            return;

        if (repository.Exists(phi.MaLoaiPhi))
        {
            ShowWarning("Mã phí đã tồn tại. Vui lòng nhập mã khác.");
            return;
        }

        repository.Insert(phi);

        LoadData();
        SelectRow<LoaiPhi>(p => p.MaLoaiPhi == phi.MaLoaiPhi);

        ShowInfo("Thêm khoản phí thành công!");
    }

    // ==============================
    // SỬA
    // ==============================

    protected override void OnEdit()
    {
        var selected = SelectedItem<LoaiPhi>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn khoản phí cần sửa.");
            return;
        }

        var phi = ShowFeeDialog(selected);

        if (phi == null)
            return;

        repository.Update(phi);

        LoadData();
        SelectRow<LoaiPhi>(p => p.MaLoaiPhi == phi.MaLoaiPhi);

        ShowInfo("Cập nhật khoản phí thành công!");
    }

    // ==============================
    // XÓA
    // ==============================

    protected override void OnDelete()
    {
        var selected = SelectedItem<LoaiPhi>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn khoản phí cần xóa.");
            return;
        }

        if (!selected.HoatDong)
        {
            ShowWarning("Khoản phí này đã ngừng áp dụng.");
            return;
        }

        if (!Confirm(
            $"Bạn có chắc muốn ngừng áp dụng khoản phí:\n" +
            $"{selected.MaLoaiPhi} - {selected.TenPhi}?\n\n" +
            "Dữ liệu lịch sử vẫn được giữ lại."))
            return;

        repository.Delete(selected.MaLoaiPhi);

        LoadData();

        ShowInfo("Đã ngừng áp dụng khoản phí thành công!");
    }

    // ==============================
    // FORM THÊM / SỬA
    // ==============================

    private LoaiPhi? ShowFeeDialog(LoaiPhi? current)
    {
        bool isEdit = current != null;

        var phi = current == null
            ? new LoaiPhi()
            : new LoaiPhi
            {
                MaLoaiPhi = current.MaLoaiPhi,
                TenPhi = current.TenPhi,
                DonViTinh = current.DonViTinh,
                DonGia = current.DonGia,
                CachTinh = current.CachTinh,
                ChuKy = current.ChuKy,
                HoatDong = current.HoatDong
            };

        using var form = new Form
        {
            Text = isEdit ? "Sửa khoản phí" : "Thêm khoản phí",
            Size = new Size(460, 510),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.White,
            Font = new Font("Segoe UI", 9.5f)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 2,
            RowCount = 9,
            BackColor = Color.White
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (int i = 0; i < 8; i++)
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));

        form.Controls.Add(layout);

        TextBox txtMa = new()
        {
            Text = phi.MaLoaiPhi,
            Dock = DockStyle.Fill,
            ReadOnly = isEdit
        };

        TextBox txtTen = new()
        {
            Text = phi.TenPhi,
            Dock = DockStyle.Fill
        };

        TextBox txtDonVi = new()
        {
            Text = phi.DonViTinh,
            Dock = DockStyle.Fill
        };

        NumericUpDown numDonGia = new()
        {
            DecimalPlaces = 2,
            Maximum = 1000000000000M,
            Minimum = 0,
            Value = Math.Max(0, Math.Min(
                phi.DonGia, 1000000000000M)),
            Dock = DockStyle.Left,
            Width = 220,
            ThousandsSeparator = true
        };

        TextBox txtCachTinh = new()
        {
            Text = phi.CachTinh,
            Dock = DockStyle.Fill
        };

        ComboBox cboChuKy = new()
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDown
        };

        cboChuKy.Items.AddRange(new object[]
        {
            "Hàng tháng",
            "Hàng quý",
            "Hàng năm",
            "Theo lượt",
            "Một lần"
        });

        cboChuKy.Text = phi.ChuKy;

        CheckBox chkHoatDong = new()
        {
            Text = "Đang áp dụng",
            Checked = phi.HoatDong,
            AutoSize = true
        };

        AddField(layout, 0, "Mã phí:", txtMa);
        AddField(layout, 1, "Tên khoản phí:", txtTen);
        AddField(layout, 2, "Đơn vị tính:", txtDonVi);
        AddField(layout, 3, "Đơn giá (đ):", numDonGia);
        AddField(layout, 4, "Cách tính:", txtCachTinh);
        AddField(layout, 5, "Chu kỳ:", cboChuKy);
        AddField(layout, 6, "Trạng thái:", chkHoatDong);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };

        var btnSave = new Button
        {
            Text = "Lưu",
            Width = 95,
            Height = 32,
            BackColor = Color.FromArgb(43, 125, 232),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        var btnCancel = new Button
        {
            Text = "Hủy",
            Width = 95,
            Height = 32,
            DialogResult = DialogResult.Cancel
        };

        btnSave.FlatAppearance.BorderSize = 0;

        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Controls.Add(btnSave);

        layout.Controls.Add(buttonPanel, 0, 8);
        layout.SetColumnSpan(buttonPanel, 2);

        btnSave.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(txtMa.Text) ||
                string.IsNullOrWhiteSpace(txtTen.Text) ||
                string.IsNullOrWhiteSpace(txtDonVi.Text) ||
                string.IsNullOrWhiteSpace(cboChuKy.Text))
            {
                ShowWarning("Vui lòng nhập đầy đủ thông tin bắt buộc.");
                return;
            }

            phi.MaLoaiPhi = txtMa.Text.Trim();
            phi.TenPhi = txtTen.Text.Trim();
            phi.DonViTinh = txtDonVi.Text.Trim();
            phi.DonGia = numDonGia.Value;
            phi.CachTinh = txtCachTinh.Text.Trim();
            phi.ChuKy = cboChuKy.Text.Trim();
            phi.HoatDong = chkHoatDong.Checked;

            form.DialogResult = DialogResult.OK;
            form.Close();
        };

        form.AcceptButton = btnSave;
        form.CancelButton = btnCancel;

        return form.ShowDialog(this) == DialogResult.OK
            ? phi
            : null;
    }

    private static void AddField(
        TableLayoutPanel layout,
        int row,
        string title,
        Control control)
    {
        var label = new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(18, 53, 94)
        };

        control.Margin = new Padding(3, 6, 3, 6);

        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }
}
