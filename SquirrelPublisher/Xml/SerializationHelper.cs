using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SquirrelPublisher.Xml {
    public static class SerializationHelper {
        public static T Deserialize<T>(string path) where T : class, new() {
            var ser = new XmlSerializer(typeof(T));
            using(FileStream stream = File.OpenRead(path)) {
                return ser.Deserialize(stream) as T;
            }
        }
        public static void Serialize<T>(string path, T element) {
            var ser = new XmlSerializer(typeof(T));
            using(FileStream stream = File.Create(path)) {
                ser.Serialize(stream, element);
            }
        }
    }
}
