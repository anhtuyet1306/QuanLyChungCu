
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Repositories;

namespace QuanLyChungCu_SkyResidence.Pages;

public class IncidentsPage : BasePage
{
    private readonly SuCoRepository repository = new();

    public IncidentsPage()
        : base(
            "PHẢN ÁNH SỰ CỐ",
            "Tiếp nhận và theo dõi tiến độ xử lý phản ánh."
        )
    {
        AddColumn("Mã sự cố", "MaSC", 75, true);
        AddColumn("Cư dân", "CuDan", 120);
        AddColumn("Loại sự cố", "LoaiSC", 100);
        AddColumn("Nội dung", "NoiDung", 180);
        AddColumn("Vị trí", "ViTri", 90);
        AddColumn("Ngày báo", "NgayBaoHienThi", 110, true);
        AddColumn("Mức độ", "MucDoUuTien", 80, true);
        AddColumn("Trạng thái", "TrangThai", 100, true);

        Grid.CellFormatting += Grid_CellFormatting;
    }

    // Hiển thị danh sách phản ánh
    protected override void LoadData()
    {
        var danhSach = repository.GetAll();
        Bind(danhSach, $"Có {danhSach.Count} phản ánh sự cố.");
    }

    // Tìm kiếm phản ánh
    protected override void OnSearch(string keyword)
    {
        var danhSach = repository.GetAll(keyword);
        Bind(danhSach, $"Tìm thấy {danhSach.Count} phản ánh.");
    }

    // Thêm phản ánh
    protected override void OnAdd()
    {
        try
        {
            var item = new SuCo
            {
                NgayBao = DateTime.Now,
                MucDoUuTien = "Trung bình",
                TrangThai = "Đã tiếp nhận"
            };

            if (!ShowIncidentDialog(item, false))
                return;

            repository.Insert(item);

            LoadData();

            ShowInfo("Đã thêm phản ánh thành công.");
        }
        catch (Exception ex)
        {
            ShowError("Không thể thêm phản ánh:\n\n" + ex);
        }
    }

    // Sửa phản ánh
    protected override void OnEdit()
    {
        var selected = SelectedItem<SuCo>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn phản ánh cần sửa.");
            return;
        }

        try
        {
            var item = new SuCo
            {
                MaSC = selected.MaSC,
                MaCD = selected.MaCD,
                CuDan = selected.CuDan,
                MaCH = selected.MaCH,
                ViTri = selected.ViTri,
                LoaiSC = selected.LoaiSC,
                NoiDung = selected.NoiDung,
                NgayBao = selected.NgayBao,
                MucDoUuTien = selected.MucDoUuTien,
                TrangThai = selected.TrangThai,
                NgayHoanThanh = selected.NgayHoanThanh,
                DanhGia = selected.DanhGia,
                NhanXet = selected.NhanXet,
                MaNVXuLyCuoi = selected.MaNVXuLyCuoi,
                NguoiXuLyCuoi = selected.NguoiXuLyCuoi
            };

            if (!ShowIncidentDialog(item, true))
                return;

            repository.Update(item);

            LoadData();

            ShowInfo("Đã cập nhật phản ánh thành công.");
        }
        catch (Exception ex)
        {
            ShowError("Không thể cập nhật phản ánh:\n\n" + ex);
        }
    }

    // Xóa phản ánh
    protected override void OnDelete()
    {
        var selected = SelectedItem<SuCo>();

        if (selected == null)
        {
            ShowWarning("Vui lòng chọn phản ánh cần xóa.");
            return;
        }

        if (!Confirm(
            $"Bạn có chắc chắn muốn xóa phản ánh {selected.MaSC} không?"
        ))
            return;

        try
        {
            repository.Delete(selected.MaSC);

            LoadData();

            ShowInfo("Đã xóa phản ánh thành công.");
        }
        catch (Exception ex)
        {
            ShowError("Không thể xóa phản ánh:\n\n" + ex);
        }
    }

    // Hộp thoại thêm và sửa
    private bool ShowIncidentDialog(SuCo item, bool isEdit)
    {
        var residents = repository.GetResidents();

        if (residents.Count == 0)
        {
            ShowWarning("Chưa có dữ liệu cư dân. Vui lòng thêm cư dân trước.");
            return false;
        }

        using var form = new Form
        {
            Text = isEdit ? "CẬP NHẬT PHẢN ÁNH" : "THÊM PHẢN ÁNH SỰ CỐ",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(560, 650),
            BackColor = Color.White
        };

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20)
        };

        form.Controls.Add(content);

        int y = 15;

        Label AddLabel(string text)
        {
            var label = new Label
            {
                Text = text,
                Location = new Point(5, y),
                Size = new Size(490, 24),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            content.Controls.Add(label);
            y += 27;

            return label;
        }

        TextBox AddTextBox(
            string label,
            string value,
            int maxLength,
            bool multiline = false)
        {
            AddLabel(label);

            var box = new TextBox
            {
                Text = value,
                Location = new Point(5, y),
                Size = new Size(490, multiline ? 85 : 32),
                MaxLength = maxLength,
                Multiline = multiline,
                ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None,
                Font = new Font("Segoe UI", 10)
            };

            content.Controls.Add(box);
            y += multiline ? 96 : 43;

            return box;
        }

        ComboBox AddComboBox(string label)
        {
            AddLabel(label);

            var combo = new ComboBox
            {
                Location = new Point(5, y),
                Size = new Size(490, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            content.Controls.Add(combo);
            y += 43;

            return combo;
        }

        var txtMaSC = AddTextBox(
            "Mã sự cố *",
            item.MaSC,
            10
        );

        txtMaSC.Enabled = !isEdit;

        var cboCuDan = AddComboBox("Cư dân phản ánh *");

        cboCuDan.DataSource = residents;
        cboCuDan.DisplayMember = "Value";
        cboCuDan.ValueMember = "Key";

        if (!string.IsNullOrWhiteSpace(item.MaCD))
        {
            cboCuDan.SelectedValue = item.MaCD;
        }

        var txtLoaiSC = AddTextBox(
            "Loại sự cố *",
            item.LoaiSC,
            20
        );

        var txtViTri = AddTextBox(
            "Vị trí xảy ra sự cố",
            item.ViTri ?? "",
            100
        );

        var txtNoiDung = AddTextBox(
            "Nội dung phản ánh *",
            item.NoiDung,
            500,
            true
        );

        var cboMucDo = AddComboBox("Mức độ ưu tiên");

        cboMucDo.Items.AddRange(new object[]
        {
            "Thấp",
            "Trung bình",
            "Cao"
        });

        cboMucDo.SelectedItem = item.MucDoUuTien;

        if (cboMucDo.SelectedIndex < 0)
            cboMucDo.SelectedIndex = 1;

        var cboTrangThai = AddComboBox("Trạng thái");

        cboTrangThai.Items.AddRange(new object[]
        {
            "Đã tiếp nhận",
            "Đang xử lý",
            "Đã hoàn thành"
        });

        cboTrangThai.SelectedItem = item.TrangThai;

        if (cboTrangThai.SelectedIndex < 0)
            cboTrangThai.SelectedIndex = 0;

        // Nút chức năng
        var bottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        form.Controls.Add(bottom);
        bottom.BringToFront();

        var btnSave = new Button
        {
            Text = "LƯU",
            Size = new Size(110, 38),
            Location = new Point(280, 12),
            BackColor = Color.FromArgb(25, 135, 84),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.None
        };

        var btnCancel = new Button
        {
            Text = "HỦY",
            Size = new Size(110, 38),
            Location = new Point(400, 12),
            BackColor = Color.Gray,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.Cancel
        };

        bottom.Controls.Add(btnSave);
        bottom.Controls.Add(btnCancel);

        btnSave.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtMaSC.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sự cố.");
                txtMaSC.Focus();
                return;
            }

            if (cboCuDan.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn cư dân.");
                cboCuDan.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLoaiSC.Text))
            {
                MessageBox.Show("Vui lòng nhập loại sự cố.");
                txtLoaiSC.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung phản ánh.");
                txtNoiDung.Focus();
                return;
            }

            item.MaSC = txtMaSC.Text.Trim();
            item.MaCD = cboCuDan.SelectedValue.ToString() ?? "";
            item.LoaiSC = txtLoaiSC.Text.Trim();
            item.ViTri = string.IsNullOrWhiteSpace(txtViTri.Text)
                ? null
                : txtViTri.Text.Trim();

            item.NoiDung = txtNoiDung.Text.Trim();
            item.MucDoUuTien = cboMucDo.SelectedItem?.ToString() ?? "Trung bình";
            item.TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Đã tiếp nhận";

            if (!isEdit)
            {
                item.NgayBao = DateTime.Now;
                item.NgayHoanThanh = null;
            }
            else if (item.TrangThai == "Đã hoàn thành" &&
                     item.NgayHoanThanh == null)
            {
                item.NgayHoanThanh = DateTime.Now;
            }
            else if (item.TrangThai != "Đã hoàn thành")
            {
                item.NgayHoanThanh = null;
            }

            form.DialogResult = DialogResult.OK;
            form.Close();
        };

        form.CancelButton = btnCancel;

        return form.ShowDialog(FindForm()) == DialogResult.OK;
    }

    // Định dạng màu trạng thái trên bảng
    private void Grid_CellFormatting(
        object? sender,
        DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        string columnName = Grid.Columns[e.ColumnIndex].DataPropertyName;

        if (columnName == "MucDoUuTien")
        {
            string value = e.Value?.ToString() ?? "";

            if (value == "Cao")
                e.CellStyle.ForeColor = Color.Red;
            else if (value == "Trung bình")
                e.CellStyle.ForeColor = Color.DarkOrange;
            else
                e.CellStyle.ForeColor = Color.Green;
        }

        if (columnName == "TrangThai")
        {
            string value = e.Value?.ToString() ?? "";

            if (value == "Đã hoàn thành")
                e.CellStyle.ForeColor = Color.Green;
            else if (value == "Đang xử lý")
                e.CellStyle.ForeColor = Color.DarkOrange;
            else
                e.CellStyle.ForeColor = Color.RoyalBlue;
        }
    }
}
