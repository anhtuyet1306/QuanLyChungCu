using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Services;
using QuanLyChungCu_SkyResidence.Styles;

namespace QuanLyChungCu_SkyResidence.Forms;

/// <summary>Biểu mẫu thêm/sửa cư dân.</summary>
public class CuDanEditForm : Form
{
    private readonly CuDanService _service;
    private readonly bool _isNew;
    private readonly CuDan _model;
    private readonly TextBox txtMa = new();
    private readonly TextBox txtHoTen = new();
    private readonly DateTimePicker dtNgaySinh = new();
    private readonly CheckBox chkCoNgaySinh = new();
    private readonly TextBox txtDienThoai = new();
    private readonly ComboBox cboCanHo = new();
    private readonly ComboBox cboVaiTro = new();

    public CuDan? Result { get; private set; }

    public CuDanEditForm(CuDanService service, CuDan? existing)
    {
        _service = service;
        _isNew = existing is null;
        _model = existing?.Clone() ?? new CuDan();
        InitializeUi();
        LoadModel();
    }

    private void InitializeUi()
    {
        Text = _isNew ? "Thêm cư dân" : $"Sửa cư dân {_model.MaCD}";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Font = AppStyles.NormalFont;
        BackColor = Color.White;
        ClientSize = new Size(500, 430);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        Controls.Add(root);

        var fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(24, 16, 24, 0) };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.Controls.Add(fields, 0, 0);

        txtMa.MaxLength = 20;
        txtMa.CharacterCasing = CharacterCasing.Upper;
        txtMa.PlaceholderText = "VD: CD004";
        txtMa.Enabled = _isNew;
        txtHoTen.MaxLength = 150;
        txtHoTen.PlaceholderText = "Nhập họ và tên";
        dtNgaySinh.Format = DateTimePickerFormat.Custom;
        dtNgaySinh.CustomFormat = "dd/MM/yyyy";
        dtNgaySinh.MaxDate = DateTime.Today;
        chkCoNgaySinh.Text = "Có ngày sinh";
        chkCoNgaySinh.Checked = true;
        txtDienThoai.MaxLength = 15;
        txtDienThoai.PlaceholderText = "VD: 0901234567";

        cboCanHo.DropDownStyle = ComboBoxStyle.DropDownList;
        cboCanHo.Items.Add("(Chưa phân căn hộ)");
        foreach (string ma in _service.GetApartmentCodes()) cboCanHo.Items.Add(ma);
        cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
        cboVaiTro.Items.AddRange(new object[] { "Chủ hộ", "Thành viên" });

        var datePanel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight };
        dtNgaySinh.Width = 135;
        datePanel.Controls.Add(dtNgaySinh);
        datePanel.Controls.Add(chkCoNgaySinh);
        chkCoNgaySinh.CheckedChanged += (_, _) => dtNgaySinh.Enabled = chkCoNgaySinh.Checked;

        AddRow(fields, "Mã cư dân *", txtMa);
        AddRow(fields, "Họ và tên *", txtHoTen);
        AddRow(fields, "Ngày sinh", datePanel);
        AddRow(fields, "Số điện thoại", txtDienThoai);
        AddRow(fields, "Căn hộ", cboCanHo);
        AddRow(fields, "Vai trò *", cboVaiTro);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(24, 10, 24, 10) };
        root.Controls.Add(buttons, 0, 1);
        var cancel = new Button { Text = "Hủy", Size = new Size(110, 40), DialogResult = DialogResult.Cancel, BackColor = Color.FromArgb(95, 110, 130), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        cancel.FlatAppearance.BorderSize = 0;
        var save = new Button { Text = "Lưu", Size = new Size(110, 40) };
        AppStyles.StyleLoginButton(save);
        save.Click += Save_Click;
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        AcceptButton = save;
        CancelButton = cancel;
    }

    private static void AddRow(TableLayoutPanel table, string label, Control control)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        table.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = AppStyles.Navy, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, row);
        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        table.Controls.Add(control, 1, row);
    }

    private void LoadModel()
    {
        txtMa.Text = _model.MaCD;
        txtHoTen.Text = _model.HoTen;
        if (_model.NgaySinh.HasValue) dtNgaySinh.Value = _model.NgaySinh.Value;
        else chkCoNgaySinh.Checked = false;
        txtDienThoai.Text = _model.SoDienThoai;
        cboCanHo.SelectedItem = string.IsNullOrWhiteSpace(_model.MaCH) ? "(Chưa phân căn hộ)" : _model.MaCH;
        if (cboCanHo.SelectedIndex < 0) cboCanHo.SelectedIndex = 0;
        cboVaiTro.SelectedItem = _model.VaiTro;
        if (cboVaiTro.SelectedIndex < 0) cboVaiTro.SelectedItem = "Thành viên";
    }

    private void Save_Click(object? sender, EventArgs e)
    {
        var cd = new CuDan
        {
            MaCD = txtMa.Text.Trim().ToUpperInvariant(),
            HoTen = txtHoTen.Text.Trim(),
            NgaySinh = chkCoNgaySinh.Checked ? dtNgaySinh.Value.Date : null,
            SoDienThoai = txtDienThoai.Text.Trim(),
            MaCH = cboCanHo.SelectedItem?.ToString() == "(Chưa phân căn hộ)" ? null : cboCanHo.SelectedItem?.ToString(),
            VaiTro = cboVaiTro.SelectedItem?.ToString() ?? ""
        };

        try
        {
            var errors = _service.Validate(cd, _isNew);
            if (errors.Count > 0)
            {
                MessageBox.Show("Vui lòng kiểm tra lại:\n\n• " + string.Join("\n• ", errors), "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = _service.Save(cd, _isNew);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Không lưu được", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Result = cd;
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Đã xảy ra lỗi:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
