using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.InputHandling
{
    public class ControllerInput : IInputDevice
    {
        /// <summary>
        /// Registers a callback-based command
        /// </summary>
        public void registerCommand(Buttons button, bool buttonPressOnly, IInputDevice.CommandDelegate callback)
        {
            //
            // If already registered, remove it!
            if (m_commandEntries.ContainsKey(button))
            {
                m_commandEntries.Remove(button);
            }
            m_commandEntries.Add(button, new CommandEntry(button, buttonPressOnly, callback));
        }

        /// <summary>
        /// Track all registered commands in this dictionary
        /// </summary>
        private Dictionary<Buttons, CommandEntry> m_commandEntries = new Dictionary<Buttons, CommandEntry>();

        /// <summary>
        /// Used to keep track of the details associated with a command
        /// </summary>
        private struct CommandEntry
        {
            public CommandEntry(Buttons button, bool buttonPressOnly, IInputDevice.CommandDelegate callback)
            {
                this.button = button;
                this.buttonPressOnly = buttonPressOnly;
                this.callback = callback;
            }

            public Buttons button;
            public bool buttonPressOnly;
            public IInputDevice.CommandDelegate callback;
        }

        /// <summary>
        /// Goes through all the registered commands and invokes the callbacks if they
        /// are active.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            foreach (CommandEntry entry in this.m_commandEntries.Values)
            {
                if (entry.buttonPressOnly && buttonPressed(entry.button))
                {
                    entry.callback(gameTime);
                }
                else if (!entry.buttonPressOnly && state.IsButtonDown(entry.button))
                {
                    entry.callback(gameTime);
                }
            }

            //
            // Move the current state to the previous state for the next time around
            m_statePrevious = state;
        }

        private GamePadState m_statePrevious;

        /// <summary>
        /// Checks to see if a key was newly pressed
        /// </summary>
        private bool buttonPressed(Buttons button)
        {
            return GamePad.GetState(PlayerIndex.One).IsButtonDown(button) && !m_statePrevious.IsButtonDown(button);
        }
    }
}