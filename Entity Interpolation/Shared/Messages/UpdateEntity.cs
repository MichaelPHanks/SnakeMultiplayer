
using Microsoft.Xna.Framework;
using Shared.Components;
using Shared.Entities;
using System.Collections.Generic;

namespace Shared.Messages
{
    public class UpdateEntity : Message
    {
        public UpdateEntity(Entity entity, TimeSpan updateWindow) : base(Type.UpdateEntity)
        {
            this.id = entity.id;

            if (entity.contains<Position>())
            {
                this.hasPosition = true;
                this.position = entity.get<Position>().position;
                this.orientation = entity.get<Position>().orientation;
            }

            if (entity.contains<Shared.Components.Segment>() || entity.contains<Shared.Components.Tail>())
            {
                this.hasTurnPoints = true;
                Queue < Tuple<Vector2, float> > tempPoints = new Queue<Tuple<Vector2, float>> (entity.get<Shared.Components.TurnPoints>().turnPoints);
                this.turnPoints = tempPoints;
            }
            else
            {
                this.turnPoints = new Queue<Tuple<Vector2, float>>();

            }
            this.updateWindow = updateWindow;
        }

        public UpdateEntity(): base(Type.UpdateEntity)
        {
            this.turnPoints = new Queue<Tuple<Vector2, float>>();

        }

        public uint id { get; private set; }

        // Position
        public bool hasPosition { get; private set; } = false;
        public Vector2 position { get; private set; }
        public float orientation { get; private set; }

        public bool hasTurnPoints { get; private set; } = false;

        public Queue<Tuple<Vector2, float>> turnPoints { get; private set; }    

        // Only the milliseconds are used/serialized
        public TimeSpan updateWindow { get; private set; } = TimeSpan.Zero;

        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());
            data.AddRange(BitConverter.GetBytes(id));

            data.AddRange(BitConverter.GetBytes(hasPosition));
            if (hasPosition)
            {
                data.AddRange(BitConverter.GetBytes(position.X));
                data.AddRange(BitConverter.GetBytes(position.Y));
                data.AddRange(BitConverter.GetBytes(orientation));
            }
            data.AddRange(BitConverter.GetBytes(hasTurnPoints));

            if (hasTurnPoints)
            {
                data.AddRange(BitConverter.GetBytes(turnPoints.Count));

                foreach (var input in turnPoints)
                {
                    // Convert Vector2's X and Y components to bytes (assuming they are floats)
                    byte[] xBytes = BitConverter.GetBytes(input.Item1.X);
                    byte[] yBytes = BitConverter.GetBytes(input.Item1.Y);

                    // Convert float to bytes
                    byte[] floatBytes = BitConverter.GetBytes(input.Item2);

                    // Add bytes to the list
                    data.AddRange(xBytes);
                    data.AddRange(yBytes);
                    data.AddRange(floatBytes);
                }


            }

            data.AddRange(BitConverter.GetBytes(updateWindow.Milliseconds));

            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            this.id = BitConverter.ToUInt32(data, offset);
            offset += sizeof(uint);

            this.hasPosition = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasPosition)
            {
                float positionX = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                float positionY = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                this.position = new Vector2(positionX, positionY);
                this.orientation = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
            }
            this.hasTurnPoints = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);

            if (hasTurnPoints)
            {
                int howMany = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);
                for (int i = 0; i < howMany; i++)
                {
                    float x = BitConverter.ToSingle(data.ToArray(), offset);

                    offset += sizeof(float);
                    float y = BitConverter.ToSingle(data.ToArray(), offset);
                    offset += sizeof(float);

                    // Parse float value from bytes
                    float floatValue = BitConverter.ToSingle(data.ToArray(), offset);

                    // Create Vector2 and float tuple and add it to the queue
                    turnPoints.Enqueue(new Tuple<Vector2, float>(new Vector2(x, y), floatValue));

                    // Move the index to the next tuple
                    offset += sizeof(float);
                }


            }
            this.updateWindow = new TimeSpan(0, 0, 0, 0, BitConverter.ToInt32(data, offset));
            offset += sizeof(Int32);

            return offset;
        }
    }
}
