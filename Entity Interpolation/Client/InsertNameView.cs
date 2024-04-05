﻿using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Client
{
    public class InsertNameView : GameStateView
    {

        // Step 1: Give the player a chance to name themselves
        // There is only one server, don't need to offer anything related to server selection or room selection
        // Note: This might take a while. Give a default random name to start (given from server). 
        // Should probably save name to the computer? Who knows.
        // The server needs to know about the name of the individual.


        private Texture2D backgroundImage;
        private bool isESCDown = true;
        SoundEffect hover;
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private Rectangle settings = new Rectangle();
        private Rectangle resume = new Rectangle();
        private Rectangle help = new Rectangle();
        private Rectangle menu = new Rectangle();
        private Rectangle Continue = new Rectangle();
        private bool m_waitForKeyRelease = false;
        SoundEffectInstance soundInstance;
        private bool isEnterUp = false;
        private bool canUseMouse = false;

        private Rectangle TextBox = new Rectangle();

        // TODO: Load playerName from memory...
        private string playerName = "Temp Name";
        private Texture2D whiteImage;
        bool typingBool = false;
        bool isKeySelected = false;



        private enum MenuState
        {
            
            Menu,
            Controls,
            Name,
            None,
            
        }
        private MenuState m_currentSelection = MenuState.Menu;
        private MenuState m_prevSelection = MenuState.Menu;


        public override void loadContent(ContentManager contentManager)
        {
            whiteImage = contentManager.Load<Texture2D>("whiteImage");
            backgroundImage = contentManager.Load<Texture2D>("saturnCool");
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-selected");
            hover = contentManager.Load<SoundEffect>("little_robot_sound_factory_multimedia_Click_Electronic_14");
            soundInstance = hover.CreateInstance();

        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                isEnterUp = true;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !isESCDown)
            {

                if (isKeySelected)
                {
                    isKeySelected = false;
                    isESCDown = true;
                }
                else
                {
                    isESCDown = true;
                    isEnterUp = false;
                    return GameStateEnum.GamePlay;

                }


            }
            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                isESCDown = false;
            }
            if (!m_waitForKeyRelease && isEnterUp)
            {


                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {

                    
                        isEnterUp = false;
                        m_waitForKeyRelease = true;
                    
                }

                if (isKeySelected && !typingBool)
                {
                    // Collect input keys


                    Keys[] keys = Keyboard.GetState().GetPressedKeys();
                    if (keys.Length > 0)
                    {
                        typingBool = true;
                        m_waitForKeyRelease = true;

                        if (keys[0] != Keys.Escape && keys[0] != Keys.Enter)
                        {
                            if (keys[0] == Keys.Back)
                            {
                                if (playerName.Length > 0)
                                {
                                    playerName = playerName.Remove(playerName.Length - 1);

                                }
                            }
                            else
                            {
                                if ((keys[0] >= Keys.A && keys[0] <= Keys.Z) || keys[0] == Keys.Space)
                                {
                                    playerName += ((char)keys[0]);
                                }
                                if (keys[0] == Keys.LeftShift || keys[0] == Keys.RightShift) 
                                {
                                    typingBool = false;
                                }

                            }
                        }

                    }

                }

                if (Keyboard.GetState().GetPressedKeyCount() == 0)
                {
                    typingBool = false;

                }

                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    if (m_currentSelection == MenuState.Name)
                    {
                        m_currentSelection = MenuState.Menu;
                    }
                    else
                    {
                        m_currentSelection++;
                    }
                    m_waitForKeyRelease = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    if (m_currentSelection == MenuState.Menu)
                    {
                        m_currentSelection = MenuState.Name;
                    }
                    else
                    {
                        m_currentSelection--;
                    }
                    m_waitForKeyRelease = true;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Menu)
                {
                    isESCDown = true;
                    isEnterUp = false;
                    canUseMouse = false;
                    return GameStateEnum.MainMenu;
                }

               
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Controls)
                {
                    isESCDown = true;
                    isEnterUp = false;
                    canUseMouse = false;
                    return GameStateEnum.Controls;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Name)
                {
                    isEnterUp = false;
                    canUseMouse = false;
                    isKeySelected = true;

                }
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                m_waitForKeyRelease = false;
            }


            if (canUseMouse)
            {
               
               
                if (menu.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;

                        return GameStateEnum.MainMenu;
                    }
                    m_currentSelection = MenuState.Menu;

                }
                else if (Continue.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;

                        return GameStateEnum.Controls;
                    }
                    m_currentSelection = MenuState.Controls;

                }
                else if (TextBox.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        isEnterUp = false;
                        canUseMouse = false;
                        isKeySelected = true;


                    }
                    m_currentSelection = MenuState.Name;

                }
            }



            /*else 
            {
                m_currentSelection = MenuState.None;
            }*/

            if (m_prevSelection != m_currentSelection && m_currentSelection != MenuState.None)
            {
                if (soundInstance.State == SoundState.Playing)
                {
                    soundInstance.Stop();

                }
                soundInstance.Play();

            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                canUseMouse = false;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                canUseMouse = true;
            }
            m_prevSelection = m_currentSelection;
            return GameStateEnum.EnterName;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), Color.Gray);
            float bottom = drawMenuItem(m_fontMenu, "Insert Your Preferred Name", 100, Color.OrangeRed);
           /* bottom = drawMenuItem(m_currentSelection == MenuState.Settings ? m_fontMenuSelect : m_fontMenu, "Settings", bottom, m_currentSelection == MenuState.Settings ? Color.White : Color.LightGray);

            bottom = drawMenuItem(m_currentSelection == MenuState.Help ? m_fontMenuSelect : m_fontMenu, "Help", bottom, m_currentSelection == MenuState.Help ? Color.White : Color.LightGray);
            bottom = drawMenuItem(m_currentSelection == MenuState.Resume ? m_fontMenuSelect : m_fontMenu, "Resume", bottom, m_currentSelection == MenuState.Resume ? Color.White : Color.LightGray);*/
            bottom = drawMenuItem(m_currentSelection == MenuState.Menu ? m_fontMenuSelect : m_fontMenu, "Main Menu", bottom, m_currentSelection == MenuState.Menu ? Color.White : Color.LightGray);
            bottom = drawMenuItem(m_currentSelection == MenuState.Controls ? m_fontMenuSelect : m_fontMenu, "Continue", bottom, m_currentSelection == MenuState.Controls ? Color.White : Color.LightGray);

            drawMenuItem(m_currentSelection == MenuState.Name ? m_fontMenuSelect : m_fontMenu, playerName, bottom, Color.Black);
            
            m_spriteBatch.End();
        }


        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            m_spriteBatch.Draw(whiteImage, TextBox, isKeySelected ? Color.Red : Color.White);

            float scale = m_graphics.PreferredBackBufferWidth / 1920f;
            Vector2 stringSize = font.MeasureString(text) * scale;
            m_spriteBatch.DrawString(
                           font,
                           text,
                           new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                           color,
                           0,
                           Vector2.Zero,
                           scale,
                           SpriteEffects.None,
                           0);

            
            if (text == "Main Menu")
            {
                menu = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);


            }
            else if (text == "Continue")
            {
                Continue = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);


            }
            else if (text == playerName)
            { 
                TextBox = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);
                if (TextBox.Width == 0)
                {
                    TextBox = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, 10, 10);
                }

            }

            return y + stringSize.Y;
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
