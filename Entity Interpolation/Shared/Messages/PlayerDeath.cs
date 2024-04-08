using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class PlayerDeath : Message
    {
        public PlayerDeath() : base(Type.PlayerDeath)
        {
        }
        public PlayerDeath(uint entityId) : base(Type.PlayerDeath)
        {
            this.id = entityId;
        }

        public uint id { get; private set; }

        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());
            data.AddRange(BitConverter.GetBytes(id));

            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            this.id = BitConverter.ToUInt32(data, offset);
            offset += sizeof(UInt32);

            return offset;
        }
    }
}
