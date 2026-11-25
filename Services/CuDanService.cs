using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Repositories;

namespace QuanLyChungCu_SkyResidence.Services;

/// <summary>Kiểm tra quy tắc nghiệp vụ và điều phối thao tác cư dân.</summary>
public class CuDanService
{
    private readonly CuDanRepository _repo = new();
    private static readonly Regex PhoneRegex = new(@"^(0|\+84)[0-9]{8,10}$", RegexOptions.Compiled);

    public List<CuDan> GetAll() => _repo.GetAll();
    public List<string> GetApartmentCodes() => _repo.GetApartmentCodes();

    public List<string> Validate(CuDan cd, bool isNew)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(cd.MaCD)) errors.Add("Mã cư dân không được để trống.");
        else if (cd.MaCD.Length > 20) errors.Add("Mã cư dân không được dài quá 20 ký tự.");
        else if (isNew && _repo.Exists(cd.MaCD)) errors.Add($"Mã cư dân {cd.MaCD} đã tồn tại.");

        if (string.IsNullOrWhiteSpace(cd.HoTen)) errors.Add("Họ tên không được để trống.");
        else if (cd.HoTen.Length > 150) errors.Add("Họ tên không được dài quá 150 ký tự.");

        if (cd.NgaySinh.HasValue && cd.NgaySinh.Value.Date > DateTime.Today)
            errors.Add("Ngày sinh không được ở tương lai.");

        if (!string.IsNullOrWhiteSpace(cd.SoDienThoai) && !PhoneRegex.IsMatch(cd.SoDienThoai.Trim()))
            errors.Add("Số điện thoại phải bắt đầu bằng 0 hoặc +84 và có 9–11 chữ số.");

        if (!string.IsNullOrWhiteSpace(cd.MaCH) && !_repo.GetApartmentCodes().Contains(cd.MaCH))
            errors.Add("Căn hộ được chọn không tồn tại.");

        if (cd.VaiTro is not ("Chủ hộ" or "Thành viên"))
            errors.Add("Vai trò phải là Chủ hộ hoặc Thành viên.");
        return errors;
    }

    public OperationResult Save(CuDan cd, bool isNew)
    {
        try
        {
            if (isNew) _repo.Insert(cd);
            else if (_repo.Update(cd) == 0) return OperationResult.Fail($"Không tìm thấy cư dân {cd.MaCD}.");
            return OperationResult.Ok(isNew ? $"Đã thêm cư dân {cd.MaCD}." : $"Đã cập nhật cư dân {cd.MaCD}.");
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            return OperationResult.Fail($"Mã cư dân {cd.MaCD} đã tồn tại.");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return OperationResult.Fail("Không thể lưu vì căn hộ được chọn không hợp lệ hoặc đang có dữ liệu liên quan.");
        }
    }

    public OperationResult Delete(string maCD)
    {
        int affected = _repo.Delete(maCD);
        return affected == 0
            ? OperationResult.Fail("Không thể xóa cư dân. Cư dân không tồn tại hoặc đã chuyển đi.")
            : OperationResult.Ok($"Đã xóa cư dân {maCD}.");
    }
}
