using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Forms;
using QuanLyChungCu_SkyResidence.Helpers;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Pages;

/// <summary>Quản lý cư dân: đọc/ghi trực tiếp SQL Server.</summary>
public class ResidentsPage : BasePage
{
    private static readonly Font BoldFont = new("Segoe UI", 9, FontStyle.Bold);
    private readonly CuDanService _service = new();
    private List<CuDan> _all = new();

    public ResidentsPage() : base("QUẢN LÝ CƯ DÂN", "Danh sách cư dân đang sinh sống trong chung cư.")
    {
        AddColumn("Mã cư dân", nameof(CuDan.MaCD), 85, center: true);
        AddColumn("Họ và tên", nameof(CuDan.HoTen), 150);
        AddColumn("Ngày sinh", nameof(CuDan.NgaySinh), 100, center: true, format: "dd/MM/yyyy");
        AddColumn("Số điện thoại", nameof(CuDan.SoDienThoai), 115, center: true);
        AddColumn("Căn hộ", nameof(CuDan.MaCH), 80, center: true);
        AddColumn("Vai trò", nameof(CuDan.VaiTro), 95, center: true);
    }

    protected override void LoadData()
    {
        _all = _service.GetAll();
        ApplyFilter();
    }

    protected override void OnSearch(string keyword) => ApplyFilter();

    private void ApplyFilter()
    {
        string keyword = SearchBox.Text.Trim();
        var list = string.IsNullOrWhiteSpace(keyword) ? _all : _all.Where(cd =>
            TextHelper.ContainsIgnoreAccent($"{cd.MaCD} {cd.HoTen} {cd.SoDienThoai} {cd.MaCH} {cd.VaiTro}", keyword)).ToList();
        Bind(list, $"Hiển thị {list.Count}/{_all.Count} cư dân");
    }

    protected override void OnAdd()
    {
        using var form = new CuDanEditForm(_service, null);
        if (form.ShowDialog(this) != DialogResult.OK || form.Result is null) return;
        string ma = form.Result.MaCD;
        LoadData();
        SelectRow<CuDan>(cd => cd.MaCD == ma);
    }

    protected override void OnEdit()
    {
        var selected = SelectedItem<CuDan>();
        if (selected is null) { ShowWarning("Vui lòng chọn cư dân cần sửa."); return; }
        using var form = new CuDanEditForm(_service, selected);
        if (form.ShowDialog(this) != DialogResult.OK || form.Result is null) return;
        string ma = form.Result.MaCD;
        LoadData();
        SelectRow<CuDan>(cd => cd.MaCD == ma);
    }

    protected override void OnDelete()
    {
        var selected = SelectedItem<CuDan>();
        if (selected is null) { ShowWarning("Vui lòng chọn cư dân cần xóa."); return; }
        if (!Confirm($"Bạn có chắc muốn xóa cư dân {selected.MaCD} - {selected.HoTen}?")) return;
        var result = _service.Delete(selected.MaCD);
        if (result.Success) { ShowInfo(result.Message); LoadData(); }
        else ShowWarning(result.Message);
    }
}
