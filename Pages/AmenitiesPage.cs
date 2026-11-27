
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Repositories;
using QuanLyChungCu_SkyResidence.Services;
namespace QuanLyChungCu_SkyResidence.Pages;

public class AmenitiesPage : BasePage
{
    private readonly DangKyTienIchRepository repository = new();

    public AmenitiesPage(AuthUser user)
    : base(
        "ĐĂNG KÝ TIỆN ÍCH",
        "Quản lý đăng ký, cập nhật và xét duyệt tiện ích cư dân.",
        user,
        "TIENICH")
    {
        AddColumn("Mã đăng ký", nameof(DangKyTienIch.MaDK), 75, true);
        AddColumn("Cư dân", nameof(DangKyTienIch.HoTen), 120);
        AddColumn("Tiện ích", nameof(DangKyTienIch.TenTI), 110);
        AddColumn("Ngày sử dụng", nameof(DangKyTienIch.NgaySuDungHienThi), 95, true);
        AddColumn("Khung giờ", nameof(DangKyTienIch.KhungGio), 100, true);
        AddColumn("Số người", nameof(DangKyTienIch.SoNguoi), 65, true);
        AddColumn("Trạng thái", nameof(DangKyTienIch.TrangThai), 90, true);
        AddColumn("Lý do từ chối", nameof(DangKyTienIch.LyDoTuChoi), 130);

        Grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                Grid.Columns[e.ColumnIndex].Name != "Trạng thái")
                return;

            string status = e.Value?.ToString() ?? "";

            e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            if (status == "Đã duyệt")
                e.CellStyle.ForeColor = Color.FromArgb(32, 140, 80);
            else if (status == "Chờ duyệt")
                e.CellStyle.ForeColor = Color.FromArgb(220, 145, 30);
            else if (status == "Từ chối")
                e.CellStyle.ForeColor = Color.FromArgb(210, 65, 65);
        };

    }

    protected override void LoadData()
    {
        try
        {
            Bind(repository.GetAll());
        }
        catch (Exception ex)
        {
            ShowError("Không tải được danh sách đăng ký tiện ích.\n" + ex.Message);
        }
    }

    protected override void OnSearch(string keyword)
    {
        try
        {
            Bind(repository.GetAll(keyword));
        }
        catch (Exception ex)
        {
            ShowError("Lỗi tìm kiếm:\n" + ex.Message);
        }
    }

    protected override void OnAdd()
    {
        try
        {
            var item = new DangKyTienIch
            {
                NgaySuDung = DateTime.Today.AddDays(1),
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(9, 0, 0),
                SoNguoi = 1,
                TrangThai = "Chờ duyệt"
            };

            if (!ShowRegistrationDialog(item, false))
                return;

            repository.Insert(item);

            LoadData();
            ShowInfo("Đã thêm đăng ký tiện ích thành công.");
        }
        catch (Exception ex)
        {
            ShowError("Không thể thêm đăng ký.\n" + ex.Message);
        }
    }

    protected override void OnEdit()
    {
        var selected = SelectedItem<DangKyTienIch>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn đăng ký cần cập nhật.");
            return;
        }

        try
        {
            var item = new DangKyTienIch
            {
                MaDK = selected.MaDK,
                MaTI = selected.MaTI,
                TenTI = selected.TenTI,
                MaCD = selected.MaCD,
                HoTen = selected.HoTen,
                NgaySuDung = selected.NgaySuDung,
                GioBatDau = selected.GioBatDau,
                GioKetThuc = selected.GioKetThuc,
                SoNguoi = selected.SoNguoi,
                TrangThai = selected.TrangThai,
                LyDoTuChoi = selected.LyDoTuChoi
            };

            if (!ShowRegistrationDialog(item, true))
                return;

            repository.Update(item);

            LoadData();
            ShowInfo("Đã cập nhật đăng ký thành công.");
        }
        catch (Exception ex)
        {
            ShowError("Không thể cập nhật đăng ký.\n" + ex.Message);
        }
    }

    protected override void OnDelete()
    {
        var selected = SelectedItem<DangKyTienIch>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn đăng ký cần xóa.");
            return;
        }

        if (!Confirm($"Bạn có chắc muốn xóa đăng ký {selected.MaDK}?"))
            return;

        try
        {
            repository.Delete(selected.MaDK);
            LoadData();
            ShowInfo("Đã xóa đăng ký thành công.");
        }
        catch (Exception ex)
        {
            ShowError(
                "Không thể xóa đăng ký này.\n" +
                "Có thể đăng ký đã được sử dụng trong hóa đơn.\n\n" +
                ex.Message);
        }
    }

    private bool ShowRegistrationDialog(
        DangKyTienIch item,
        bool isEdit)
    {
        var residents = repository.GetResidents();
        var amenities = repository.GetAmenities();

        if (residents.Count == 0)
        {
            ShowWarning("Chưa có cư dân trong hệ thống.");
            return false;
        }

        if (amenities.Count == 0)
        {
            ShowWarning("Không có tiện ích nào đang hoạt động.");
            return false;
        }

        using Form dialog = new()
        {
            Text = isEdit
                ? "CẬP NHẬT / DUYỆT ĐĂNG KÝ"
                : "THÊM ĐĂNG KÝ TIỆN ÍCH",

            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            ClientSize = new Size(520, isEdit ? 590 : 450),
            MaximizeBox = false,
            MinimizeBox = false,
            Font = new Font("Segoe UI", 9),
            BackColor = Color.White
        };

        Panel content = new()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(15),
            BackColor = Color.White
        };

        dialog.Controls.Add(content);

        int y = 15;

        ComboBox cboResident = CreateCombo(residents, item.MaCD);
        ComboBox cboAmenity = CreateCombo(amenities, item.MaTI);

        DateTimePicker date = new()
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Value = item.NgaySuDung,
            Width = 180
        };

        DateTimePicker start = new()
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "HH:mm",
            ShowUpDown = true,
            Value = DateTime.Today.Add(item.GioBatDau),
            Width = 140
        };

        DateTimePicker end = new()
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "HH:mm",
            ShowUpDown = true,
            Value = DateTime.Today.Add(item.GioKetThuc),
            Width = 140
        };

        NumericUpDown number = new()
        {
            Minimum = 1,
            Maximum = 255,
            Value = Math.Max(1, (int)item.SoNguoi),
            Width = 120
        };

        ComboBox cboStatus = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 320
        };

        cboStatus.Items.AddRange(new object[]
        {
            "Chờ duyệt",
            "Đã duyệt",
            "Từ chối"
        });

        cboStatus.SelectedItem = item.TrangThai;

        if (cboStatus.SelectedIndex < 0)
            cboStatus.SelectedIndex = 0;

        TextBox reason = new()
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Text = item.LyDoTuChoi ?? "",
            Width = 320,
            Height = 75
        };

        AddInputRow(content, "Cư dân:", cboResident, ref y);
        AddInputRow(content, "Tiện ích:", cboAmenity, ref y);
        AddInputRow(content, "Ngày sử dụng:", date, ref y);
        AddInputRow(content, "Giờ bắt đầu:", start, ref y);
        AddInputRow(content, "Giờ kết thúc:", end, ref y);
        AddInputRow(content, "Số người:", number, ref y);

        if (isEdit)
        {
            AddInputRow(content, "Trạng thái:", cboStatus, ref y);
            AddInputRow(content, "Lý do từ chối:", reason, ref y, 90);
        }

        Panel bottom = new()
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            BackColor = Color.FromArgb(245, 247, 250)
        };

        dialog.Controls.Add(bottom);
        bottom.BringToFront();

        Button btnSave = new()
        {
            Text = isEdit ? "Lưu cập nhật" : "Thêm đăng ký",
            Size = new Size(125, 36),
            Location = new Point(245, 10),
            BackColor = Color.FromArgb(35, 105, 180),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        Button btnCancel = new()
        {
            Text = "Hủy",
            Size = new Size(85, 36),
            Location = new Point(380, 10),
            DialogResult = DialogResult.Cancel
        };

        btnSave.Click += (_, _) =>
        {
            if (cboResident.SelectedItem is not SelectOption resident ||
                cboAmenity.SelectedItem is not SelectOption amenity)
            {
                MessageBox.Show(
                    "Vui lòng chọn cư dân và tiện ích.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (end.Value.TimeOfDay <= start.Value.TimeOfDay)
            {
                MessageBox.Show(
                    "Giờ kết thúc phải sau giờ bắt đầu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            item.MaCD = resident.Key;
            item.MaTI = amenity.Key;
            item.NgaySuDung = date.Value.Date;
            item.GioBatDau = start.Value.TimeOfDay;
            item.GioKetThuc = end.Value.TimeOfDay;
            item.SoNguoi = (byte)number.Value;

            if (isEdit)
            {
                item.TrangThai = cboStatus.SelectedItem?.ToString()
                    ?? "Chờ duyệt";

                item.LyDoTuChoi = reason.Text.Trim();

                if (item.TrangThai == "Từ chối" &&
                    string.IsNullOrWhiteSpace(item.LyDoTuChoi))
                {
                    MessageBox.Show(
                        "Vui lòng nhập lý do từ chối.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    reason.Focus();
                    return;
                }

                if (item.TrangThai != "Từ chối")
                    item.LyDoTuChoi = null;
            }

            dialog.DialogResult = DialogResult.OK;
            dialog.Close();
        };

        bottom.Controls.Add(btnSave);
        bottom.Controls.Add(btnCancel);

        dialog.AcceptButton = btnSave;
        dialog.CancelButton = btnCancel;

        return dialog.ShowDialog(this) == DialogResult.OK;
    }

    private static ComboBox CreateCombo(
        List<KeyValuePair<string, string>> data,
        string selectedKey)
    {
        ComboBox combo = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 320
        };

        foreach (var item in data)
        {
            combo.Items.Add(new SelectOption(item.Key, item.Value));
        }

        for (int i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is SelectOption option &&
                option.Key == selectedKey)
            {
                combo.SelectedIndex = i;
                break;
            }
        }

        if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
            combo.SelectedIndex = 0;

        return combo;
    }

    private static void AddInputRow(
        Panel parent,
        string label,
        Control input,
        ref int y,
        int rowHeight = 42)
    {
        Label title = new()
        {
            Text = label,
            Location = new Point(5, y + 5),
            Size = new Size(125, 25),
            TextAlign = ContentAlignment.MiddleLeft
        };

        input.Location = new Point(135, y + 3);
        input.Margin = new Padding(0);

        parent.Controls.Add(title);
        parent.Controls.Add(input);

        y += rowHeight;
    }

    private sealed class SelectOption
    {
        public string Key { get; }
        public string Value { get; }

        public SelectOption(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
