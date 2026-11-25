using System;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Forms;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        Application.Run(new LoginForm());
    }
}
