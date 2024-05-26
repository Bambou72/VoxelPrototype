using System.Runtime.InteropServices;
namespace VoxelPrototype.client.Utils
{
    public static class ConsoleUtils
    {
        [DllImport("kernel32.dll")]
        public static extern nint GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
    }
}
