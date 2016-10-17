using System;
using System.IO;
using System.Xml.Serialization;

namespace AutoTune.Shared {

    public abstract class SettingsBase<T> where T : SettingsBase<T>, new() {

        public static T Instance { get; private set; } = new T();
        protected abstract void OnTerminating();
        protected abstract void OnInitialized();

        public static void Terminate() {
            Instance.OnTerminating();
            var filePath = Path.Combine(GetFolderPath(), typeof(T).Name + ".xml");
            using (Stream file = new FileStream(filePath, FileMode.Create))
                new XmlSerializer(typeof(T)).Serialize(file, Instance);
        }

        public static void Initialize() {
            var filePath = Path.Combine(GetFolderPath(), typeof(T).Name + ".xml");
            Directory.CreateDirectory(GetFolderPath());
            if (File.Exists(filePath))
                using (Stream file = new FileStream(filePath, FileMode.Open))
                    Instance = (T)new XmlSerializer(typeof(T)).Deserialize(file);
            using (Stream file = new FileStream(filePath, FileMode.Create))
                new XmlSerializer(typeof(T)).Serialize(file, Instance);
            Instance.OnInitialized();
        }

        public static string GetFolderPath() {
            var appData = Environment.SpecialFolder.LocalApplicationData;
            return Path.Combine(Environment.GetFolderPath(appData), "AutoTune");
        }
    }
}
