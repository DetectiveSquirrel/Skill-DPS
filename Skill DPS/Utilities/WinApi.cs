using System.Runtime.InteropServices;

namespace Test_Environment.Utilities
{
    public class WinApi
    {
        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool fBlockIt);
    }
}