
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyChungCu_SkyResidence.Helpers;

public static class Theme
{
    // Màu sắc chủ đạo
    public static readonly Color Navy = Color.FromArgb(18, 53, 94);
    public static readonly Color Blue = Color.FromArgb(43, 125, 232);
    public static readonly Color LightBlue = Color.FromArgb(222, 235, 252);
    public static readonly Color Background = Color.FromArgb(244, 248, 255);
    public static readonly Color White = Color.White;
    public static readonly Color Text = Color.FromArgb(45, 55, 72);
    public static readonly Color Muted = Color.FromArgb(120, 130, 145);
    public static readonly Color Green = Color.FromArgb(32, 160, 100);
    public static readonly Color Orange = Color.FromArgb(235, 158, 45);
    public static readonly Color Red = Color.FromArgb(210, 70, 70);

    // Font chữ
    public static readonly Font HeadingFont =
        new("Segoe UI", 19, FontStyle.Bold);

    public static readonly Font SubHeadingFont =
        new("Segoe UI", 9.5f);

    public static readonly Font NormalFont =
        new("Segoe UI", 9.5f);

    public static readonly Font ButtonFont =
        new("Segoe UI", 9, FontStyle.Bold);

    // Áp dụng giao diện cho Form
    public static void ApplyForm(Form form)
    {
        form.BackColor = Background;
        form.Font = NormalFont;
        form.ForeColor = Text;
    }

    // Áp dụng giao diện cho nút
    public static void ApplyButton(
        Button button,
        Color? color = null)
    {
        button.BackColor = color ?? Blue;
        button.ForeColor = White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = ButtonFont;
        button.Cursor = Cursors.Hand;
    }

    // Áp dụng giao diện cho bảng
    public static void ApplyGrid(DataGridView grid)
    {
        grid.BackgroundColor = White;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = LightBlue;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Navy;
        grid.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 9, FontStyle.Bold);

        grid.DefaultCellStyle.Font = NormalFont;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor =
            Color.FromArgb(210, 230, 255);
        grid.DefaultCellStyle.SelectionForeColor = Navy;

        grid.RowTemplate.Height = 36;
        grid.ColumnHeadersHeight = 42;
    }
}
