
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
        private List<Keys> m_keyList = new List<Keys>();
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
           
            foreach (var item in m_entities)
            {
                List<Shared.Components.Input.Type> inputs = new List<Shared.Components.Input.Type>();

                if (item.Value.isAlive)
                {



                    if (m_keyList.Count > 1)
                    {
                        // Collect the top 2 and if one of them are not an existing key, do not do anything.

                        if (m_keyToFunction[item.Key].m_keyToType.ContainsKey(m_keyList[0]) && m_keyToFunction[item.Key].m_keyToType.ContainsKey(m_keyList[1]))
                        {


                            var firstType = m_keyToFunction[item.Key].m_keyToType[m_keyList[0]];
                            var secondType = m_keyToFunction[item.Key].m_keyToType[m_keyList[1]];

                            // NOTE: If there is some lag with inputs, modify this to handle if opposite directions are pressed. Should be an issue though.
                            if (firstType == Shared.Components.Input.Type.RotateUp && secondType == Shared.Components.Input.Type.RotateRight)
                            {
                                if (Shared.Entities.Utility.rotateTopRight(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateTopRight);
                                }
                                break;
                            }
                            if (firstType == Shared.Components.Input.Type.RotateUp && secondType == Shared.Components.Input.Type.RotateLeft)
                            {
                                if (Shared.Entities.Utility.rotateTopLeft(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateTopLeft);
                                }
                                break;
                            }
                            if (firstType == Shared.Components.Input.Type.RotateDown && secondType == Shared.Components.Input.Type.RotateRight)
                            {
                                if (Shared.Entities.Utility.rotateBottomRight(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateBottomRight);
                                }
                                break;
                            }
                            if (firstType == Shared.Components.Input.Type.RotateDown && secondType == Shared.Components.Input.Type.RotateLeft)
                            {
                                if (Shared.Entities.Utility.rotateBottomLeft(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateBottomLeft);
                                }
                                break;
                            }




                            if (firstType == Shared.Components.Input.Type.RotateRight && secondType == Shared.Components.Input.Type.RotateUp)
                            {
                                if (Shared.Entities.Utility.rotateTopRight(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateTopRight);
                                }
                                break;
                            }
                            if (firstType == Shared.Components.Input.Type.RotateLeft && secondType == Shared.Components.Input.Type.RotateUp)
                            {
                                if (Shared.Entities.Utility.rotateTopLeft(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateTopLeft);
                                }
                                break;
                            }
                            if (firstType == Shared.Components.Input.Type.RotateRight && secondType == Shared.Components.Input.Type.RotateDown)
                            {
                                if (Shared.Entities.Utility.rotateBottomRight(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateBottomRight);
                                }
                                break;
                            }
                            if (firstType == Shared.Components.Input.Type.RotateLeft && secondType == Shared.Components.Input.Type.RotateDown)
                            {
                                if (Shared.Entities.Utility.rotateBottomLeft(item.Value, elapsedTime))
                                {
                                    inputs.Add(Shared.Components.Input.Type.RotateBottomLeft);
                                }
                                break;
                            }


                        }
                    }
                    else if (m_keyList.Count == 1)
                    {

                        if (m_keyToFunction[item.Key].m_keyToType.ContainsKey(m_keyList[0]))
                        {
                            var type = m_keyToFunction[item.Key].m_keyToType[m_keyList[0]];

                            // Client-side prediction of the input
                            switch (type)
                            {
                                case Shared.Components.Input.Type.RotateUp:
                                    if (Shared.Entities.Utility.rotateUp(item.Value, elapsedTime))
                                    {
                                        inputs.Add(type);
                                    }
                                    break;
                                case Shared.Components.Input.Type.RotateLeft:
                                    if (Shared.Entities.Utility.rotateLeft(item.Value, elapsedTime))
                                    {
                                        inputs.Add(type);
                                    }
                                    break;
                                case Shared.Components.Input.Type.RotateRight:
                                    if (Shared.Entities.Utility.rotateRight(item.Value, elapsedTime))
                                    {
                                        inputs.Add(type);
                                    }

                                    break;
                                case Shared.Components.Input.Type.RotateDown:
                                    if (Shared.Entities.Utility.rotateDown(item.Value, elapsedTime))
                                    {
                                        inputs.Add(type);
                                    }
                                    break;
                            }
                        }


                    }
                    m_keysPressed.Clear();


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


                    /*inputs.Add(Shared.Components.Input.Type.Thrust);
                    Shared.Entities.Utility.thrust(item.Value, elapsedTime);*/
                }

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
            m_keyList.Add(key);

        }

        public void keyReleased(Keys key)
        {
            m_keysPressed.Remove(key);
            m_keyList.Remove(key);
        }
    }
}
