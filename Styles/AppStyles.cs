using System.Drawing;
using System.Windows.Forms;

namespace QuanLyChungCu_SkyResidence.Styles;

public static class AppStyles
{
    public static Color Navy { get; } = Color.FromArgb(18, 53, 94);
    public static Color Blue { get; } = Color.FromArgb(43, 125, 232);
    public static Color White { get; } = Color.White;
    public static Color Gray { get; } = Color.Gray;

    public static Font NormalFont { get; } = new Font("Segoe UI", 10);
    public static Font TitleFont { get; } = new Font("Segoe UI", 25, FontStyle.Bold);
    public static Font ButtonFont { get; } = new Font("Segoe UI", 11, FontStyle.Bold);

    public static void StyleLoginButton(Button btn)
    {
        btn.BackColor = Blue;
        btn.ForeColor = White;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = ButtonFont;
        btn.Cursor = Cursors.Hand;
    }

    public static void StyleTitle(Label label)
    {
        label.Font = TitleFont;
        label.ForeColor = Navy;
        label.AutoSize = true;
    }

    public static void StyleTextBox(TextBox txt)
    {
        txt.Font = new Font("Segoe UI", 11);
        txt.BackColor = White;
        txt.ForeColor = Navy;
    }
}
