using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.InputHandling
{
    /// <summary>
    /// Abstract base class that defines how input is presented to game code.
    /// </summary>
    public interface IInputDevice
    {
        public delegate void CommandDelegate(GameTime gameTime);
        public delegate void CommandDelegatePosition(GameTime GameTime, int x, int y);

        void Update(GameTime gameTime);
    }
}
