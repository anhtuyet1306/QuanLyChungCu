
using System;

namespace QuanLyChungCu_SkyResidence.Models;

public class SuCo
{
    // Thông tin phản ánh
    public string MaSC { get; set; } = string.Empty;
    public string MaCD { get; set; } = string.Empty;
    public string CuDan { get; set; } = string.Empty;
    public string? MaCH { get; set; }

    // Nội dung sự cố
    public string? ViTri { get; set; }
    public string LoaiSC { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;

    // Thời gian
    public DateTime NgayBao { get; set; } = DateTime.Now;
    public DateTime? NgayHoanThanh { get; set; }

    // Trạng thái xử lý
    public string MucDoUuTien { get; set; } = "Trung bình";
    public string TrangThai { get; set; } = "Đã tiếp nhận";

    // Đánh giá sau xử lý
    public byte? DanhGia { get; set; }
    public string? NhanXet { get; set; }

    // Nhân viên xử lý
    public string? MaNVXuLyCuoi { get; set; }
    public string? NguoiXuLyCuoi { get; set; }

    // Thuộc tính hiển thị trên bảng
    public string NgayBaoHienThi =>
        NgayBao.ToString("dd/MM/yyyy HH:mm");

    public string NgayHoanThanhHienThi =>
        NgayHoanThanh?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
}
