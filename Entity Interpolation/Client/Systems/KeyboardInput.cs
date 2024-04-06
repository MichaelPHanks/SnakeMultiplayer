
using Microsoft.Xna.Framework.Input;
using Shared.Components;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Markup;

namespace Client.Systems
{
    public class KeyboardInput : Shared.Systems.System
    {
        private class KeyToType
        {
            public Dictionary<Keys, Shared.Components.Input.Type> m_keyToType = new Dictionary<Keys, Shared.Components.Input.Type>();
        }

        private Dictionary<Shared.Components.Input.Type, Keys> m_typeToKey = new Dictionary<Shared.Components.Input.Type, Keys>();
        private Dictionary<uint, KeyToType> m_keyToFunction = new Dictionary<uint, KeyToType>();

        private HashSet<Keys> m_keysPressed = new HashSet<Keys>();
        private bool m_mouseEnabled = true;
        private List<Shared.Components.Input.Type> m_inputEvents = new List<Shared.Components.Input.Type>();
        
        public KeyboardInput(List<Tuple<Shared.Components.Input.Type, Keys>> mapping) : base(typeof(Shared.Components.Input))
        {
            foreach (var input in mapping)
            {
                m_typeToKey[input.Item1] = input.Item2;
            }
        }

        public override void update(TimeSpan elapsedTime)
        {
            // Keep in mind we are never going to stop thrusting forward, so we should always be going forward
            // Mouse is just changing the orientation!
            foreach (var item in m_entities)
            {
                List<Shared.Components.Input.Type> inputs = new List<Shared.Components.Input.Type>();
                


                    foreach (var key in m_keysPressed)
                    {
                        if (m_keyToFunction[item.Key].m_keyToType.ContainsKey(key))
                        {
                            var type = m_keyToFunction[item.Key].m_keyToType[key];
                            inputs.Add(type);

                            // Client-side prediction of the input
                            switch (type)
                            {
                                case Shared.Components.Input.Type.RotateUp:
                                    Shared.Entities.Utility.rotateUp(item.Value, elapsedTime);
                                    break;
                                case Shared.Components.Input.Type.RotateLeft:
                                    Shared.Entities.Utility.rotateLeft(item.Value, elapsedTime);
                                    break;
                                case Shared.Components.Input.Type.RotateRight:
                                    Shared.Entities.Utility.rotateRight(item.Value, elapsedTime);
                                    break;
                                case Shared.Components.Input.Type.RotateDown:
                                    Shared.Entities.Utility.rotateDown(item.Value,elapsedTime);
                                    break;
                            }
                        }
                    }
                
                /*var vectorX = Math.Cos(position.orientation);
                var vectorY = Math.Sin(position.orientation);*/
                /*if (item.Value.get<Position>().orientation)*/
                /*
                                var vectorX = Math.Cos(item.Value.get<Position>().orientation);
                                var vectorY = Math.Sin(item.Value.get<Position>().orientation);

                                // Get the position of the mouse and the ship, create vector between the two

                                Microsoft.Xna.Framework.Vector2 angle = item.Value.get<Position>().position - new Microsoft.Xna.Framework.Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                                var vectorXMouse = Math.Cos(angle.X);
                                var vectorYMouse = Math.Sin(angle.Y);

                                var angle2 = Math.Atan2(vectorXMouse, vectorYMouse);
                                var angle3 = Math.Atan2(vectorX, vectorY);

                                if (angle3 != angle2)
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateLeft);
                                    Shared.Entities.Utility.rotateLeft(item.Value, elapsedTime);

                                }

                                if (Mouse.GetState().X != 1)
                                {
                                    // Do some more stuff here
                                }*/

                /*else
                {
                    previousX = Mouse.GetState().X;
                    previousY = Mouse.GetState().Y;
                    inputs.Add(Shared.Components.Input.Type.RotateMouse);
                    Shared.Entities.Utility.mouseRotation(item.Value, elapsedTime, Mouse.GetState().X, Mouse.GetState().Y);
                }*/
               

                inputs.Add(Shared.Components.Input.Type.Thrust);
                Shared.Entities.Utility.thrust(item.Value, elapsedTime);

                if (inputs.Count > 0)
                {
                    MessageQueueClient.instance.sendMessageWithId(new Shared.Messages.Input(item.Key, inputs, elapsedTime));
                }
            }
        }

        

        public override bool add(Entity entity)
        {
            if (!base.add(entity))
            {
                return false;
            }

            KeyToType map = new KeyToType();
            foreach (var input in entity.get<Shared.Components.Input>().inputs)
            {
                map.m_keyToType[m_typeToKey[input]] = input;
            }
            m_keyToFunction[entity.id] = map;

            return true;
        }

        public override void remove(uint id)
        {
            base.remove(id);

            m_keyToFunction.Remove(id);
        }

        public void keyPressed(Keys key)
        {
            m_keysPressed.Add(key);
        }

        public void keyReleased(Keys key)
        {
            m_keysPressed.Remove(key);
        }
    }
}
