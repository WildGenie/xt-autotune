using System;
using System.IO;
using YAXLib;

namespace AutoTune.Settings {

    internal abstract class SettingsBase<T> where T : SettingsBase<T>, new() {

        internal static T Instance { get; private set; } = new T();
        internal abstract void OnTerminating();
        internal abstract void OnInitialized();

        internal static void Terminate() {
            Instance.OnTerminating();
            var filePath = Path.Combine(GetFolderPath(), typeof(T).Name + ".xml");
            File.WriteAllText(filePath, new YAXSerializer(typeof(T)).Serialize(Instance));
        }

        internal static void Initialize() {
            var filePath = Path.Combine(GetFolderPath(), typeof(T).Name + ".xml");
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
