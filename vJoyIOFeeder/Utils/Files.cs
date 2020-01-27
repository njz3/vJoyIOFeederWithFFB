using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace vJoyIOFeeder.Utils
{
    public static class Files
    {
        static Dictionary<Type, XmlSerializer> Serializers = new Dictionary<Type, XmlSerializer>();
        static XmlSerializer FindOrCreateSerializer(Type t)
        {
            if (Serializers.ContainsKey(t)) {
                return Serializers[t];
            } else {
                var serializer = new XmlSerializer(t);
                Serializers.Add(t, serializer);
                return serializer;
            }
        }


        public static void Serialize<T>(string filename, T db) where T : new()
        {
            var serializer = FindOrCreateSerializer(typeof(T));
            var path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path)){
                Directory.CreateDirectory(path);
            }
            try {
                using (var writer = new FileStream(filename, FileMode.Create)) {
                    serializer.Serialize(writer, db);
                }
            } catch (Exception ex) {
                Console.WriteLine("Could not save configuration due to " + ex.Message);
            }
        }

        public static T Deserialize<T>(string filename) where T : new()
        {
            T db = new T();
            var serializer = FindOrCreateSerializer(typeof(T));
            try {
                using (Stream reader = new FileStream(filename, FileMode.Open)) {
                    db = (T)serializer.Deserialize(reader);
                }
            } catch (Exception ex) {
                Console.WriteLine("Could not load configuration due to " + ex.Message);
            }
            return db;
        }
    }
}
