using ConfigGarden.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

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
            Logger.Info($"Writing config data to {path}");

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
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonData = JsonSerializer.Serialize(data, options);
            Write(path, jsonData);
        }

        internal T Load<T>(string path, T defaultValue = default)
        {
            Logger.Info($"Loading config data from {path}");
            if (!File.Exists(path))
            {
                Save(path, defaultValue);
                return defaultValue;
            }

            return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
        }
    }
}
