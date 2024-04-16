using Microsoft.Xna.Framework;
using Shared.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class ScoresUpdate : Message
    {
        public ScoresUpdate() : base(Type.ScoresUpdate)
        {
            this.scoresTable = new Dictionary<string, int>();
        }
        public ScoresUpdate(Dictionary<string, int> scoresTable) : base(Type.ScoresUpdate)
        {
            this.scoresTable = scoresTable;
        }
        public Dictionary<string, int> scoresTable { get; private set; }

        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());



            data.AddRange(BitConverter.GetBytes(scoresTable.Count));
            foreach (var input in scoresTable)
            {
                
                data.AddRange(BitConverter.GetBytes(input.Key.Length));

                data.AddRange(Encoding.UTF8.GetBytes(input.Key));

                data.AddRange(BitConverter.GetBytes(input.Value));
                
            }


            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            int count = BitConverter.ToInt32(data, offset);

            offset += sizeof(int);

            for (int i = 0; i < count; i++)
            {
                int keyLength = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);

                string key = Encoding.UTF8.GetString(data, offset, keyLength);
                offset += keyLength;

                int value = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);

                scoresTable.Add(key, value);
            }


            return offset;
        }

    }
}
