using Microsoft.Xna.Framework;
using Shared.Components;
using Shared.Entities;
using System;
using System.Text;

namespace Shared.Messages
{
    public class NewEntity : Message
    {
        public NewEntity(Entity entity) : base(Type.NewEntity)
        {
            this.id = entity.id;

            if (entity.contains<Name>())
            {
                this.hasName = true;
                this.name = entity.get<Name>().name;
            }

            if (entity.contains<Appearance>())
            {
                this.hasAppearance = true;
                this.texture = entity.get<Appearance>().texture;
            }
            else
            {
                this.texture = "";
            }

            if (entity.contains<Position>())
            {
                this.hasPosition = true;
                this.position = entity.get<Position>().position;
                this.orientation = entity.get<Position>().orientation;
            }

            if (entity.contains<Size>())
            {
                this.hasSize = true;
                this.size = entity.get<Size>().size;
            }

            if (entity.contains<Movement>())
            {
                this.hasMovement = true;
                this.moveRate = entity.get<Movement>().moveRate;
                this.rotateRate = entity.get<Movement>().rotateRate;
            }
            

            if (entity.contains<Components.Input>())
            {
                this.hasInput = true;
                this.inputs = entity.get<Components.Input>().inputs;
            }
            else
            {
                this.inputs = new List<Components.Input.Type>();
            }


            if (entity.contains<Components.Segment>())
            {
                this.isSegment = true;
                this.headId = entity.get<Components.Segment>().headId;
            }
            if (entity.contains<Components.Head>())
            {
                this.isHead = true;

            }
            if (entity.contains<Components.Tail>())
            {
                this.isTail = true;
                this.headId = entity.get<Components.Tail>().headId;
            }

            if (entity.contains<Components.TurnPoints>())
            {
                this.hasTurnPoints = true;
                this.turnPoints = entity.get<Components.TurnPoints>().turnPoints;
            }

            else 
            {
                this.turnPoints = new Queue<Tuple<Vector2, float>>();

            }
        }
        public NewEntity() : base(Type.NewEntity)
        {
            this.texture = "";
            this.inputs = new List<Components.Input.Type>();
            this.turnPoints = new Queue<Tuple<Vector2, float>>();
        }

        public uint id { get; private set; }
        public bool hasName { get; private set; } = false;
        public string name { get; private set; }
        // Appearance
        public bool hasAppearance { get; private set; } = false;
        public string texture { get; private set; }

        // Position
        public bool hasPosition { get; private set; } = false;
        public Vector2 position { get; private set; }
        public float orientation { get; private set; }
        
        // size
        public bool hasSize { get; private set; } = false;
        public Vector2 size { get; private set; }

        // Movement
        public bool hasMovement { get; private set; } = false;
        public float moveRate { get; private set; }
        public float rotateRate { get; private set; }

        // Input
        public bool hasInput { get; private set; } = false;
        public List<Components.Input.Type> inputs { get; private set; }

        // Segment
        public bool isSegment { get; private set; }
        public uint headId { get; private set; }
        // Head
        public bool isHead { get; private set; }
        // Tail
        public bool isTail { get; private set; }
        // TurnPoints
        public bool hasTurnPoints {  get; private set; }

        public Queue<Tuple<Vector2, float>> turnPoints { get; private set; }
        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());
            data.AddRange(BitConverter.GetBytes(id));

            data.AddRange(BitConverter.GetBytes(hasName));
            if (hasName)
            {
                data.AddRange(BitConverter.GetBytes(name.Length));
                data.AddRange(Encoding.UTF8.GetBytes(name));
            }

            data.AddRange(BitConverter.GetBytes(hasAppearance));
            if (hasAppearance)
            {
                data.AddRange(BitConverter.GetBytes(texture.Length));
                data.AddRange(Encoding.UTF8.GetBytes(texture));
            }

            data.AddRange(BitConverter.GetBytes(hasPosition));
            if (hasPosition)
            {
                data.AddRange(BitConverter.GetBytes(position.X));
                data.AddRange(BitConverter.GetBytes(position.Y));
                data.AddRange(BitConverter.GetBytes(orientation));
            }

            data.AddRange(BitConverter.GetBytes(hasSize));
            if (hasSize)
            {
                data.AddRange(BitConverter.GetBytes(size.X));
                data.AddRange(BitConverter.GetBytes(size.Y));
            }

            data.AddRange(BitConverter.GetBytes(hasMovement));
            if (hasMovement)
            {
                data.AddRange(BitConverter.GetBytes(moveRate));
                data.AddRange(BitConverter.GetBytes(rotateRate));
            }

            data.AddRange(BitConverter.GetBytes(hasInput));
            if (hasInput)
            {
                data.AddRange(BitConverter.GetBytes(inputs.Count));
                foreach (var input in inputs)
                {
                    data.AddRange(BitConverter.GetBytes((UInt16)input));
                }
            }
            data.AddRange(BitConverter.GetBytes(isSegment));
            if (isSegment)
            {
                data.AddRange(BitConverter.GetBytes(headId));

            }
            data.AddRange(BitConverter.GetBytes(isHead));
            
            data.AddRange(BitConverter.GetBytes(isTail));
            if (isTail)
            {
                data.AddRange(BitConverter.GetBytes(headId));

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

            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            this.id = BitConverter.ToUInt32(data, offset);
            offset += sizeof(uint);

            this.hasName = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasName)
            {
                int nameSize = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
                this.name = Encoding.UTF8.GetString(data, offset, nameSize);
                offset += nameSize;
            }

            this.hasAppearance = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasAppearance)
            {
                int textureSize = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
                this.texture = Encoding.UTF8.GetString(data, offset, textureSize);
                offset += textureSize;
            }

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


            this.hasSize = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasSize)
            {
                float sizeX = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                float sizeY = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                this.size = new Vector2(sizeX, sizeY);
            }

            this.hasMovement = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasMovement)
            {
                this.moveRate = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                this.rotateRate = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
            }

            this.hasInput = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasInput)
            {
                int howMany = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);
                for (int i = 0; i < howMany; i++)
                {
                    inputs.Add((Components.Input.Type)BitConverter.ToUInt16(data, offset));
                    offset += sizeof(UInt16);
                }
            }

            this.isSegment = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (isSegment)
            {
                this.headId = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
            }


            this.isHead = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            this.isTail = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (isTail)
            {
                this.headId = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
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
                    float y = BitConverter.ToSingle(data.ToArray(), offset) ;
                    offset += sizeof(float);

                    // Parse float value from bytes
                    float floatValue = BitConverter.ToSingle(data.ToArray(), offset);

                    // Create Vector2 and float tuple and add it to the queue
                    turnPoints.Enqueue(new Tuple<Vector2, float>(new Vector2(x, y), floatValue));

                    // Move the index to the next tuple
                    offset += sizeof(float);
                }


            }

            return offset;
        }
    }
}
