using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Forms;
using QuanLyChungCu_SkyResidence.Helpers;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Pages;

/// <summary>Quản lý cư dân: đọc/ghi trực tiếp SQL Server.</summary>
public class ResidentsPage : CrudPage<CuDan>
{
    private static readonly Font BoldFont = new("Segoe UI", 9, FontStyle.Bold);
    private readonly CuDanService _service = new();
    private List<CuDan> _all = new();

    public ResidentsPage(AuthUser user) : base("QUẢN LÝ CƯ DÂN", "Nhân viên lễ tân phụ trách hồ sơ cư dân và tình trạng cư trú.", user, "CUDAN", "cư dân")
    {
        AddColumn("Mã cư dân", nameof(CuDan.MaCD), 85, center: true);
        AddColumn("Họ và tên", nameof(CuDan.HoTen), 150);
        AddColumn("Ngày sinh", nameof(CuDan.NgaySinh), 100, center: true, format: "dd/MM/yyyy");
        AddColumn("Số điện thoại", nameof(CuDan.SoDienThoai), 115, center: true);
        AddColumn("Căn hộ", nameof(CuDan.MaCH), 80, center: true);
        AddColumn("Vai trò", nameof(CuDan.VaiTro), 95, center: true);
        AddColumn("Cư trú", nameof(CuDan.TinhTrangCuTru), 110, center: true);
    }

    protected override void LoadData()
    {
        if (!CanView) return;
        _all = _service.GetAll();
        ApplyFilter();
    }

    protected override void OnSearch(string keyword) => ApplyFilter();

    private void ApplyFilter()
    {
        string keyword = SearchBox.Text.Trim();
        var list = string.IsNullOrWhiteSpace(keyword) ? _all : _all.Where(cd =>
            TextHelper.ContainsIgnoreAccent($"{cd.MaCD} {cd.HoTen} {cd.SoDienThoai} {cd.MaCH} {cd.VaiTro} {cd.TinhTrangCuTru}", keyword)).ToList();
        Bind(list, $"Hiển thị {list.Count}/{_all.Count} cư dân");
    }

    protected override Form CreateEditor(CuDan? item) => new CuDanEditForm(_service, item);
    protected override string GetKey(CuDan item) => item.MaCD;
    protected override OperationResult DeleteItem(CuDan item) => _service.Delete(item.MaCD);
}
