using System;
using System.IO;
using System.Text.RegularExpressions;
using YAXLib;

namespace AutoTune.Settings {

    abstract class SettingsBase<T> where T : SettingsBase<T>, new() {

        internal static T Instance { get; private set; } = new T();
        internal abstract void OnTerminating();
        internal abstract void OnInitialized();

        static string GetFileName() {
            return Regex.Replace(typeof(T).Name, @"([a-z])([A-Z])", "$1-$2").ToLower() + ".xml";
        }

        internal static void Terminate() {
            Instance.OnTerminating();
            var filePath = Path.Combine(GetFolderPath(), GetFileName());
            File.WriteAllText(filePath, new YAXSerializer(typeof(T)).Serialize(Instance));
        }

        internal static void Initialize() {
            var filePath = Path.Combine(GetFolderPath(), GetFileName());
            Directory.CreateDirectory(GetFolderPath());
            if (File.Exists(filePath))
                Instance = (T)new YAXSerializer(typeof(T)).Deserialize(File.ReadAllText(filePath));
            File.WriteAllText(filePath, new YAXSerializer(typeof(T)).Serialize(Instance));
            Instance.OnInitialized();
        }

        internal static string GetFolderPath() {
            var appData = Environment.SpecialFolder.LocalApplicationData;
            return Path.Combine(Environment.GetFolderPath(appData), "AutoTune");
        }
    }
}
