using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Services;
using QuanLyChungCu_SkyResidence.Styles;

namespace QuanLyChungCu_SkyResidence.Forms;

/// <summary>Hộp thoại Thêm / Sửa căn hộ. existing = null nghĩa là thêm mới.</summary>
public class CanHoEditForm : Form
{
    private readonly CanHoService _service;
    private readonly CanHo _model;
    private readonly bool _isNew;

    private readonly TextBox txtMa = new();
    private readonly ComboBox cboToa = new();
    private readonly NumericUpDown numTang = new();
    private readonly NumericUpDown numDienTich = new();
    private readonly NumericUpDown numXeMay = new();
    private readonly NumericUpDown numOTo = new();
    private readonly ComboBox cboTrangThai = new();

    /// <summary>Căn hộ đã lưu thành công (dùng để chọn lại dòng trên lưới).</summary>
    public CanHo? Result { get; private set; }

    public CanHoEditForm(CanHoService service, CanHo? existing)
    {
        _service = service;
        _isNew = existing is null;
        _model = existing?.Clone() ?? new CanHo { MaToa = "A", Tang = 1, DienTich = 70, TrangThai = CanHo.ConTrong };

        InitializeUi();
        ShowModelOnControls();
    }

    private void InitializeUi()
    {
        Text = _isNew ? "Thêm căn hộ" : $"Sửa căn hộ {_model.MaCH}";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Font = AppStyles.NormalFont;
        BackColor = Color.White;
        ClientSize = new Size(480, _isNew ? 430 : 480);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        Controls.Add(root);

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(24, 16, 24, 0)
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.Controls.Add(fields, 0, 0);

        txtMa.PlaceholderText = "VD: A101";
        txtMa.MaxLength = 10;
        txtMa.CharacterCasing = CharacterCasing.Upper;
        txtMa.Enabled = _isNew; // mã là khóa chính, không cho đổi khi sửa

        cboToa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboToa.DisplayMember = nameof(ToaItem.TenToa);
        cboToa.ValueMember = nameof(ToaItem.MaToa);
        cboToa.DataSource = _service.GetToaList();

        numTang.Minimum = 1;
        numTang.Maximum = 99;

        numDienTich.Minimum = 1;
        numDienTich.Maximum = 1000;
        numDienTich.DecimalPlaces = 2;
        numDienTich.Increment = 0.5M;

        numXeMay.Minimum = 0;
        numXeMay.Maximum = 20;

        numOTo.Minimum = 0;
        numOTo.Maximum = 10;

        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Items.AddRange(CanHo.DanhSachTrangThai);

        AddRow(fields, "Mã căn hộ *", txtMa);
        AddRow(fields, "Tòa *", cboToa);
        AddRow(fields, "Tầng *", numTang);
        AddRow(fields, "Diện tích (m²) *", numDienTich);
        AddRow(fields, "Số xe máy", numXeMay);
        AddRow(fields, "Số ô tô", numOTo);
        AddRow(fields, "Tình trạng *", cboTrangThai);

        if (!_isNew)
        {
            var lblChuHo = new Label
            {
                Text = string.IsNullOrWhiteSpace(_model.ChuHo) ? "(chưa có)" : _model.ChuHo,
                ForeColor = Color.DimGray
            };
            AddRow(fields, "Chủ hộ hiện tại", lblChuHo);
        }

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(24, 10, 24, 10)
        };
        root.Controls.Add(buttons, 0, 1);

        var btnCancel = new Button
        {
            Text = "Hủy",
            Size = new Size(110, 40),
            DialogResult = DialogResult.Cancel,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(95, 110, 130),
            ForeColor = Color.White,
            Font = AppStyles.ButtonFont,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;

        var btnSave = new Button
        {
            Text = "Lưu",
            Size = new Size(110, 40)
        };
        AppStyles.StyleLoginButton(btnSave);
        btnSave.Click += BtnSave_Click;

        buttons.Controls.Add(btnCancel);
        buttons.Controls.Add(btnSave);

        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }

    private static void AddRow(TableLayoutPanel table, string label, Control control)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        table.Controls.Add(new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = AppStyles.Navy,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        }, 0, row);

        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        table.Controls.Add(control, 1, row);
    }

    private void ShowModelOnControls()
    {
        txtMa.Text = _model.MaCH;
        cboToa.SelectedValue = _model.MaToa;
        numTang.Value = Math.Clamp(_model.Tang, (int)numTang.Minimum, (int)numTang.Maximum);
        numDienTich.Value = Math.Clamp(_model.DienTich, numDienTich.Minimum, numDienTich.Maximum);
        numXeMay.Value = Math.Clamp(_model.SoXeMay, (int)numXeMay.Minimum, (int)numXeMay.Maximum);
        numOTo.Value = Math.Clamp(_model.SoOTo, (int)numOTo.Minimum, (int)numOTo.Maximum);
        cboTrangThai.SelectedItem = _model.TrangThai;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var ch = new CanHo
        {
            MaCH = txtMa.Text.Trim().ToUpperInvariant(),
            MaToa = cboToa.SelectedValue as string ?? "",
            Tang = (int)numTang.Value,
            DienTich = numDienTich.Value,
            SoXeMay = (int)numXeMay.Value,
            SoOTo = (int)numOTo.Value,
            TrangThai = cboTrangThai.SelectedItem as string ?? "",
            ChuHo = _model.ChuHo
        };

        try
        {
            var errors = _service.Validate(ch, _isNew);
            if (errors.Count > 0)
            {
                MessageBox.Show(
                    "Vui lòng kiểm tra lại:\n\n• " + string.Join("\n• ", errors),
                    "Dữ liệu chưa hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var result = _service.Save(ch, _isNew);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Không lưu được", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Result = ch;
            DialogResult = DialogResult.OK; // tự đóng hộp thoại
        }
        catch (Exception ex)
        {
            MessageBox.Show("Đã xảy ra lỗi:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
