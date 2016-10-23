using System;
using System.IO;
using System.Xml.Serialization;

namespace AutoTune.Settings {

    public abstract class SettingsBase<T> where T : SettingsBase<T>, new() {

        internal static T Instance { get; private set; } = new T();
        internal abstract void OnTerminating();
        internal abstract void OnInitialized();

        internal static void Terminate() {
            Instance.OnTerminating();
            var filePath = Path.Combine(GetFolderPath(), typeof(T).Name + ".xml");
            using (Stream file = new FileStream(filePath, FileMode.Create))
                new XmlSerializer(typeof(T)).Serialize(file, Instance);
        }

        internal static void Initialize() {
            var filePath = Path.Combine(GetFolderPath(), typeof(T).Name + ".xml");
            Directory.CreateDirectory(GetFolderPath());
            if (File.Exists(filePath))
                using (Stream file = new FileStream(filePath, FileMode.Open))
                    Instance = (T)new XmlSerializer(typeof(T)).Deserialize(file);
            using (Stream file = new FileStream(filePath, FileMode.Create))
                new XmlSerializer(typeof(T)).Serialize(file, Instance);
            Instance.OnInitialized();
        }

        internal static string GetFolderPath() {
            var appData = Environment.SpecialFolder.LocalApplicationData;
            return Path.Combine(Environment.GetFolderPath(appData), "AutoTune");
        }
    }
}
