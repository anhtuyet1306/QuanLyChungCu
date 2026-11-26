using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Controls;
using QuanLyChungCu_SkyResidence.Forms;
using QuanLyChungCu_SkyResidence.Helpers;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Pages;

/// <summary>
/// MODULE MẪU: các module khác (Cư dân, Phí, Tiện ích, Sự cố...) làm theo đúng khuôn này.
/// </summary>
public class ApartmentsPage : CrudPage<CanHo>
{
    private static readonly Font BoldFont = new("Segoe UI", 9, FontStyle.Bold);

    private readonly CanHoService _service = new();
    private List<CanHo> _all = new();

    public ApartmentsPage(AuthUser user)
        : base("QUẢN LÝ CĂN HỘ",
            "Nhân viên ban quản lý phụ trách danh mục căn hộ.", user, "CANHO", "căn hộ")
    {
        // Khai báo cột: (tiêu đề, tên thuộc tính trong model, độ rộng, căn giữa, định dạng)
        AddColumn("Mã căn hộ", nameof(CanHo.MaCH), 80, center: true);
        AddColumn("Tòa", nameof(CanHo.MaToa), 55, center: true);
        AddColumn("Tầng", nameof(CanHo.Tang), 55, center: true);
        AddColumn("Diện tích", nameof(CanHo.DienTich), 85, center: true, format: "0.## 'm²'");
        AddColumn("Xe máy", nameof(CanHo.SoXeMay), 60, center: true);
        AddColumn("Ô tô", nameof(CanHo.SoOTo), 60, center: true);
        AddColumn("Chủ hộ", nameof(CanHo.ChuHo), 140);
        AddColumn("Tình trạng", nameof(CanHo.TrangThai), 100, center: true);

        Grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                Grid.Columns[e.ColumnIndex].Name != nameof(CanHo.TrangThai))
                return;

            e.CellStyle.Font = BoldFont;
            e.CellStyle.ForeColor = (e.Value?.ToString()) switch
            {
                CanHo.DangO => Color.FromArgb(32, 140, 80),
                CanHo.ConTrong => Color.FromArgb(43, 125, 232),
                CanHo.DangBaoTri => Color.FromArgb(220, 145, 30),
                _ => e.CellStyle.ForeColor
            };
        };
    }

    // ===== 1. Nạp dữ liệu =====
    protected override void LoadData()
    {
        if (!CanView) return;
        _all = _service.GetAll();
        ApplyFilter();
    }

    // ===== 2. Tìm kiếm (lọc trong bộ nhớ, không phân biệt dấu) =====
    protected override void OnSearch(string keyword) => ApplyFilter();

    private void ApplyFilter()
    {
        string keyword = SearchBox.Text;

        var list = string.IsNullOrWhiteSpace(keyword)
            ? _all
            : _all.Where(c => TextHelper.ContainsIgnoreAccent(
                    $"{c.MaCH} {c.MaToa} {c.Tang} {c.ChuHo} {c.TrangThai}", keyword)).ToList();

        Bind(list, $"Hiển thị {list.Count}/{_all.Count} căn hộ");
    }

    protected override Form CreateEditor(CanHo? item) => new CanHoEditForm(_service, item);
    protected override string GetKey(CanHo item) => item.MaCH;
    protected override OperationResult DeleteItem(CanHo item) => _service.Delete(item.MaCH);
}
