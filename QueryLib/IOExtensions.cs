using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Source_Stalker {
    static public class IOExtensions {
        static public string ReadNullTerminatedString(this BinaryReader r) {
            using(MemoryStream ms=new MemoryStream()) {
                for(; ;) {
                    byte b = r.ReadByte();
                    if(b == 0) break;
                    ms.WriteByte(b);
                }
                return new string(Encoding.UTF8.GetChars(ms.ToArray()));
            }
        }

        static public byte[] ReadRemainingBytes(this BinaryReader r) {
            return r.ReadBytes((int)r.BaseStream.BytesAvailable());
        }

        static public long BytesAvailable(this Stream s) {
            return s.Length - s.Position;
        }
    }
}
