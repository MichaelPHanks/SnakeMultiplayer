using Microsoft.Xna.Framework;
using Shared.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class TurnPoint : Message
    {
        public TurnPoint() : base(Type.TurnPoint)
        {
        }

        public TurnPoint(Tuple<Vector2,float > turnPoint, uint headId) : base(Type.PlayerDeath)
        {
            this.turnPoint = turnPoint;
            this.headId = headId;
        }
        public  Tuple<Vector2,float > turnPoint { get; private set; }

        public uint headId { get; private set; }
        /*public override byte[] serialize()
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
        }*/
    }
}
