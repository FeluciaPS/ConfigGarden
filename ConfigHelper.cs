using ConfigGarden.Interfaces;
using System;
using System.IO;

namespace ConfigGarden
{
    public class ConfigHelper<T> where T : class , new()
    {
        private ILogger Logger { get; set; }
        private IModInfo ModInfo { get; set; }
        private readonly FileHelper helper;

        private readonly bool isWine = WineHelper.IsWine;

        private string GetRoot()
        {
            string winroot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!isWine) return winroot;

            Logger.Warn("WINE detected, attempting to use Linux specific config paths.");
            string xdg = WineHelper.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (xdg != null && !xdg.StartsWith("/"))
            {
                Logger.Warn("$XDG_CONFIG_HOME variable found, but doesn't contain valid path. Using $HOME/.config as fallback,");
                xdg = null;
            }

            string home = WineHelper.GetEnvironmentVariable("HOME");

            if (home == null && xdg == null)
            {
                Logger.Error("$HOME variable not found or empty, using default path as fallback.");
                return winroot;
            }

            string path = xdg ?? Path.Combine(home, ".config");
            return "Z:" + path.Replace('/', '\\');
        }

        public string FilePath { get; }

        private string DefaultDirectory(string root, IModInfo info)
        {
            return Path.Combine(root, "ModConfig", info.GAME_NAME, info.AUTHOR_NAME, info.MOD_NAME);
        }

        public ConfigHelper(
            IModInfo info,
            string fileName = "config",
            ILogger logger = null,
            Func<string, IModInfo, string> directoryResolver = null
        )
        {
            Logger = new LogWrapper(logger);
            ModInfo = info;
            helper = new FileHelper(Logger);

            string root = GetRoot();
            string dir = (directoryResolver ?? DefaultDirectory)(root, info);
            Directory.CreateDirectory(dir);
            string name = Path.GetFileName(fileName);

            if (!name.Contains(".")) name += ".json";
            FilePath = Path.Combine(dir, name);
        }

        private T configData;

        public T Data
        {
            get
            {
                if (configData == null) Load();
                return configData;
            }
        }

        private T Load()
        {
            configData = helper.Load(FilePath, new T());
            return configData;
        }

        public void Save()
        {
            if (configData == null) return;
            helper.Save(FilePath, configData);
        }
    }
}
