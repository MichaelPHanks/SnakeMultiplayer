using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    internal class JoinGame : Message
    {
        public JoinGame() : base(Type.JoinGame)
        {

        }

        /// <summary>
        /// In this case, the message type is all we need, so just sending a single
        /// byte of empty data as the message body.
        /// </summary>
        public override byte[] serialize()
        {
            return base.serialize();
        }

        /// <summary>
        /// Don't actually need to parse anything, as the message body is just a
        /// dummy byte.
        /// </summary>
        public override int parse(byte[] data)
        {
            return base.parse(data);
        }
    }
}
