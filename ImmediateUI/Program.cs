using OpenTK.Windowing.Desktop;
namespace ImmediateUI
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Window Wind = new(GameWindowSettings.Default, NativeWindowSettings.Default);
            Wind.Run();
            Wind.Dispose();
        }
    }
}

