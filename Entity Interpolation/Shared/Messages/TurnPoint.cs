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
        public TurnPoint(Tuple<Vector2,float > turnPoint, uint headId) : base(Type.TurnPoint)
        {
            this.turnPoint = turnPoint;
            this.headId = headId;
        }
        public  Tuple<Vector2,float > turnPoint { get; private set; }

        public uint headId { get; private set; }
        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());
            data.AddRange(BitConverter.GetBytes(headId));
            data.AddRange(BitConverter.GetBytes(turnPoint.Item1.X));
            data.AddRange(BitConverter.GetBytes(turnPoint.Item1.Y));
            data.AddRange(BitConverter.GetBytes(turnPoint.Item2));

            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            this.headId = BitConverter.ToUInt32(data, offset);
            offset += sizeof(UInt32);

            float x = BitConverter.ToSingle(data.ToArray(), offset);

            offset += sizeof(float);
            float y = BitConverter.ToSingle(data.ToArray(), offset);
            offset += sizeof(float);
            float floatValue = BitConverter.ToSingle(data.ToArray(), offset);

            turnPoint = new Tuple<Vector2, float>(new Vector2(x, y), floatValue);
            offset += sizeof(float);

            return offset;
        }
    }
}
