using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class KillCount : Message
    {
        public int killCount {  get; set; }
       
        public KillCount(int count) : base(Type.KillCount)
        {
            this.killCount = count;
        }
        public KillCount() : base(Type.KillCount) { }


        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());
            data.AddRange(BitConverter.GetBytes(killCount));

            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            this.killCount = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);

            return offset;
        }
    }
}
