using System.Runtime.InteropServices;
namespace VoxelPrototype.client
{
    public static class Utils
    {
        [DllImport("kernel32.dll")]
        public static extern nint GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
    }
}
