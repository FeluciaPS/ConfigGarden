using System;
using System.Runtime.InteropServices;


namespace ConfigGarden
{
    public class WineHelper
    {
        [DllImport("ntdll.dll", EntryPoint = "wine_get_version")]
        private static extern IntPtr WineGetVersion();

        private static bool isChecked = false;
        private static bool isWine = false;
        public static bool IsWine
        {
            get
            {
                if (isChecked) return isWine;

                try { WineGetVersion(); isWine = true; }
                catch (EntryPointNotFoundException) { }
                catch (DllNotFoundException) { }

                isChecked = true;
                return isWine;
            }
        }

        public static string GetEnvironmentVariable(string variable)
        {
            string env = Environment.GetEnvironmentVariable($"WINE_HOST_{variable}");
            if (string.IsNullOrEmpty(env)) return null;

            return env;
        }

        public static string GetDisplayPath(string path)
        {
            if (!IsWine) return path;
            string home = GetEnvironmentVariable("HOME");
            
            string homepath = "Z:" + home.Replace('/', '\\');

            if (path.StartsWith(homepath))
            {
                path = path.Replace(homepath, "~");
            }
            else if (path.StartsWith("Z:"))
            {
                path = path.Replace("Z:", "");
            }

            return path;
        }
    }
}
