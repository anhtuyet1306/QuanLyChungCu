using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Models;
using QuanLyChungCu_SkyResidence.Repositories;

namespace QuanLyChungCu_SkyResidence.Services;

/// <summary>
/// Tầng nghiệp vụ (BLL): kiểm tra dữ liệu, áp dụng luật nghiệp vụ, dịch lỗi SQL
/// thành thông báo dễ hiểu. Form chỉ việc gọi Validate/Save/Delete.
/// </summary>
public class CanHoService
{
    private readonly CanHoRepository _repo = new();

    // Mã căn hộ = <Tòa><Tầng><Số phòng 2 chữ số>, VD: A101, B1205.
    // Nếu khảo sát cho thấy chung cư đặt mã kiểu khác thì sửa regex này.
    private static readonly Regex MaRegex = new(@"^([A-Z])(\d{1,2})(\d{2})$", RegexOptions.Compiled);

    public List<CanHo> GetAll() => _repo.GetAll();

    public List<ToaItem> GetToaList() => _repo.GetToaList();

    /// <summary>Trả về danh sách lỗi; danh sách rỗng nghĩa là hợp lệ.</summary>
    public List<string> Validate(CanHo ch, bool isNew)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ch.MaCH))
        {
            errors.Add("Mã căn hộ không được để trống.");
        }
        else
        {
            var m = MaRegex.Match(ch.MaCH);
            if (!m.Success)
            {
                errors.Add("Mã căn hộ phải có dạng <Tòa><Tầng><Số phòng 2 chữ số>, ví dụ A101, B1205.");
            }
            else
            {
                string chuToa = m.Groups[1].Value;
                int tangTrongMa = int.Parse(m.Groups[2].Value);
                if (chuToa != ch.MaToa)
                    errors.Add($"Mã căn hộ bắt đầu bằng '{chuToa}' nhưng đang chọn tòa {ch.MaToa}.");
                if (tangTrongMa != ch.Tang)
                    errors.Add($"Mã căn hộ cho thấy tầng {tangTrongMa} nhưng đang nhập tầng {ch.Tang}.");
            }

            if (isNew && _repo.Exists(ch.MaCH))
                errors.Add($"Mã căn hộ {ch.MaCH} đã tồn tại.");
        }

        var toa = _repo.GetToaList().FirstOrDefault(t => t.MaToa == ch.MaToa);
        if (toa is null)
            errors.Add("Vui lòng chọn tòa.");
        else if (ch.Tang > toa.SoTang)
            errors.Add($"{toa.TenToa} chỉ có {toa.SoTang} tầng.");

        if (ch.Tang < 1)
            errors.Add("Tầng phải từ 1 trở lên.");
        if (ch.DienTich <= 0 || ch.DienTich > 1000)
            errors.Add("Diện tích phải lớn hơn 0 và không quá 1000 m².");
        if (ch.SoXeMay < 0 || ch.SoXeMay > 20)
            errors.Add("Số xe máy phải từ 0 đến 20.");
        if (ch.SoOTo < 0 || ch.SoOTo > 10)
            errors.Add("Số ô tô phải từ 0 đến 10.");
        if (!CanHo.DanhSachTrangThai.Contains(ch.TrangThai))
            errors.Add("Tình trạng căn hộ không hợp lệ.");

        // Luật: căn hộ còn người ở thì không được chuyển sang "Còn trống".
        if (!isNew && ch.TrangThai == CanHo.ConTrong && _repo.CountResidentsLiving(ch.MaCH) > 0)
            errors.Add("Căn hộ vẫn còn cư dân đang cư trú nên không thể chuyển sang 'Còn trống'. " +
                       "Hãy cập nhật tình trạng cư trú của cư dân trước.");

        return errors;
    }

    public OperationResult Save(CanHo ch, bool isNew)
    {
        try
        {
            if (isNew)
                _repo.Insert(ch);
            else if (_repo.Update(ch) == 0)
                return OperationResult.Fail($"Không tìm thấy căn hộ {ch.MaCH} (có thể đã bị người khác xóa).");

            return OperationResult.Ok(isNew ? $"Đã thêm căn hộ {ch.MaCH}." : $"Đã cập nhật căn hộ {ch.MaCH}.");
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            return OperationResult.Fail($"Mã căn hộ {ch.MaCH} đã tồn tại.");
        }
    }

    public OperationResult Delete(string maCH)
    {
        try
        {
            if (_repo.Delete(maCH) == 0)
                return OperationResult.Fail($"Không tìm thấy căn hộ {maCH}.");
            return OperationResult.Ok($"Đã xóa căn hộ {maCH}.");
        }
        catch (SqlException ex) when (ex.Number == 547) // vi phạm khóa ngoại
        {
            return OperationResult.Fail(
                $"Không thể xóa căn hộ {maCH} vì đã có dữ liệu liên quan (cư dân, hợp đồng, hóa đơn, sự cố...).\n\n" +
                "Hãy chuyển tình trạng sang 'Còn trống' hoặc 'Đang bảo trì' thay vì xóa.");
        }
    }
}
