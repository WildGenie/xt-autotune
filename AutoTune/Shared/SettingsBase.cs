using System;
using System.IO;
using System.Xml.Serialization;

namespace AutoTune.Shared {

    public class SettingsBase<T> where T : SettingsBase<T>, new() {

        public static T Instance { get; private set; } = new T();

        protected static void Save(string fileName) {
            var filePath = Path.Combine(GetFolderPath(), fileName);
            using (Stream file = new FileStream(filePath, FileMode.Create))
                new XmlSerializer(typeof(T)).Serialize(file, Instance);
        }

        protected static void Initialize(string fileName) {
            var filePath = Path.Combine(GetFolderPath(), fileName);
            Directory.CreateDirectory(GetFolderPath());
            if (File.Exists(filePath))
                using (Stream file = new FileStream(filePath, FileMode.Open))
                    Instance = (T)new XmlSerializer(typeof(T)).Deserialize(file);
            using (Stream file = new FileStream(filePath, FileMode.Create))
                new XmlSerializer(typeof(T)).Serialize(file, Instance);
        }

        public static string GetFolderPath() {
            var appData = Environment.SpecialFolder.LocalApplicationData;
            return Path.Combine(Environment.GetFolderPath(appData), "AutoTune");
        }
    }
}
