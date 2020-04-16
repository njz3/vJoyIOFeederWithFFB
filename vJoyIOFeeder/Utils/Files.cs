using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace vJoyIOFeeder.Utils
{
    public static class Files
    {

        public static T DeepCopy<T>(this T original) where T : class
        {
            using (MemoryStream memoryStream = new MemoryStream()) {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, original);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        static Dictionary<Type, XmlSerializer> XMLSerializers = new Dictionary<Type, XmlSerializer>();
        static XmlSerializer FindOrCreateXMLSerializer(Type t)
        {
            if (XMLSerializers.ContainsKey(t)) {
                return XMLSerializers[t];
            } else {
                var serializer = new XmlSerializer(t);
                XMLSerializers.Add(t, serializer);
                return serializer;
            }
        }


        public static void Serialize<T>(string filename, T db) where T : new()
        {
            var serializer = FindOrCreateXMLSerializer(typeof(T));
            var path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path)){
                Directory.CreateDirectory(path);
            }
            try {
                using (var writer = new FileStream(filename, FileMode.Create)) {
                    serializer.Serialize(writer, db);
                }
            } catch (Exception ex) {
                Logger.Log("Could not save configuration due to " + ex.Message, LogLevels.IMPORTANT);
            }
        }

        public static T Deserialize<T>(string filename) where T : new()
        {
            T db = new T();
            var serializer = FindOrCreateXMLSerializer(typeof(T));
            try {
                using (Stream reader = new FileStream(filename, FileMode.Open)) {
                    db = (T)serializer.Deserialize(reader);
                }
            } catch (Exception ex) {
                Logger.Log("Could not load configuration due to " + ex.Message, LogLevels.IMPORTANT);
            }
            return db;
        }
    }
}
