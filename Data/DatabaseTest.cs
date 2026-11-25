
using System.Windows.Forms;

namespace QuanLyChungCu_SkyResidence.Data;

public static class DatabaseTest
{
    public static void Run()
    {
        if (Database.TestConnection())
        {
            MessageBox.Show(
                "KẾT NỐI SQL SERVER THÀNH CÔNG!",
                "SkyResidence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
