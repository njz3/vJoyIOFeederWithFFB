using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace BackForceFeeder.Utils
{
    public static class Files
    {
        /// <summary>
        /// Perform deep clone using binary serializing to memory buffer.
        /// It is a rather slow process but this will handle sublist and
        /// references.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T original) where T : class
        {
            using (MemoryStream memoryStream = new MemoryStream()) {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, original);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
        /// <summary>
        /// Cache for Xml serializers
        /// </summary>
        static Dictionary<Type, XmlSerializer> XMLSerializers = new Dictionary<Type, XmlSerializer>();
        /// <summary>
        /// Find a serializer given a type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Serialize to disk using XML serializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="db"></param>
        public static void Serialize<T>(string filename, T db) where T : new()
        {
            bool useJson = true;
            if (filename.EndsWith(".xml"))
                useJson = false;
            var path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            try {
                if (useJson) {
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(db, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(filename, jsonString);
                } else {
                    var serializer = FindOrCreateXMLSerializer(typeof(T));
                    using (var writer = new FileStream(filename, FileMode.Create)) {
                        serializer.Serialize(writer, db);
                    }
                }
            } catch (Exception ex) {
                Logger.Log("Could not save configuration due to " + ex.Message, LogLevels.IMPORTANT);
            }
        }

        /// <summary>
        /// Deserialize from disk using XML serializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string filename) where T : new()
        {
            T db = new T();
            bool useJson = true;
            if (filename.EndsWith(".xml"))
                useJson = false;
            try {
                if (useJson) {
                    var jsonString = File.ReadAllText(filename);
                    db = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
                } else {
                    var serializer = FindOrCreateXMLSerializer(typeof(T));
                    using (Stream reader = new FileStream(filename, FileMode.Open)) {
                        db = (T)serializer.Deserialize(reader);
                    }
                }
            } catch (Exception ex) {
                Logger.Log("Could not load configuration file " + filename + " due to " + ex.Message, LogLevels.ERROR);
            }
            return db;
        }
    }
}
