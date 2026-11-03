using ConfigGarden.Interfaces;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ConfigGarden
{
    internal class FileHelper
    {
        private ILogger Logger { get; set; }

        public FileHelper(ILogger logger)
        {
            Logger = logger;
        }

        private bool Write(string path, string data)
        {
            Logger.Info($"Writing config data to {WineHelper.GetDisplayPath(path)}");

            string tmp = path + ".tmp";
            string bak = path + ".bak";

            try
            {
                using (var stream = new FileStream(tmp, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(data);
                    writer.Flush();
                    stream.Flush(true);
                }

                if (File.Exists(path))
                    File.Replace(tmp, path, bak);
                else
                    File.Move(tmp, path);

                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to write save data: {e.Message}");

                try
                {
                    if (File.Exists(tmp)) File.Delete(tmp);
                }
                catch { }

                return false;
            }
        }

        internal void Save<T>(string path, T data)
        {
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            Write(path, jsonData);
        }

        internal T Load<T>(string path, T defaultValue = default)
        {
            Logger.Info($"Loading config data from {WineHelper.GetDisplayPath(path)}");
            if (!File.Exists(path))
            {
                Save(path, defaultValue);
                return defaultValue;
            }

            string data = File.ReadAllText(path);
            Logger.Warn(data);
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
