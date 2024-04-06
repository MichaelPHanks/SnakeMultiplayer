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

namespace Client
{
    public class TutorialView : GameStateView
    {
        // Step 3: Show the Tutorial message/diagram "Your snake will follow your mouse" or something else appropriate
        // for the keyboard, such as, "The snake will change direction based on keypress"
        // Note: Need to think more on this. Worst case scenario, we shove all of this into the game view

        private Texture2D backgroundImage;
        private bool isESCDown = true;
        SoundEffect hover;
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private Rectangle settings = new Rectangle();
        private Rectangle resume = new Rectangle();
        private Rectangle help = new Rectangle();
        private Rectangle menu = new Rectangle();
        private Rectangle GamePlay = new Rectangle();
        private bool m_waitForKeyRelease = false;
        SoundEffectInstance soundInstance;
        private bool isEnterUp = false;
        private bool canUseMouse = false;



        private enum MenuState
        {
            Settings,
            Help,
            Resume,
            Menu,
            GamePlay,
            None,
        }
        private MenuState m_currentSelection = MenuState.Settings;
        private MenuState m_prevSelection = MenuState.Settings;


        public override void loadContent(ContentManager contentManager)
        {
            backgroundImage = contentManager.Load<Texture2D>("NotMainBackground");
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
                isESCDown = true;
                isEnterUp = false;

            }
            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                isESCDown = false;
            }
            if (!m_waitForKeyRelease && isEnterUp)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    if (m_currentSelection == MenuState.Menu)
                    {
                        m_currentSelection = MenuState.Settings;
                    }
                    else
                    {
                        m_currentSelection++;
                    }
                    m_waitForKeyRelease = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    if (m_currentSelection == MenuState.Settings)
                    {
                        m_currentSelection = MenuState.Menu;
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

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Help)
                {
                    isESCDown = true;
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.Help;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Settings)
                {
                    isESCDown = true;
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.Settings;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Resume)
                {
                    isESCDown = true;
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.GamePlay;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.GamePlay)
                {
                    isESCDown = true;
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.GamePlay;
                }
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                m_waitForKeyRelease = false;
            }


            if (canUseMouse)
            {
                if (settings.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;

                        return GameStateEnum.Settings;
                    }
                    m_currentSelection = MenuState.Settings;



                }
                else if (help.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;

                        return GameStateEnum.Help;
                    }
                    m_currentSelection = MenuState.Help;

                }
                else if (resume.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;

                        return GameStateEnum.GamePlay;
                    }
                    m_currentSelection = MenuState.Resume;

                }
                else if (menu.Contains(Mouse.GetState().Position))
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
                else if (GamePlay.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;

                        return GameStateEnum.GamePlay;
                    }
                    m_currentSelection = MenuState.GamePlay;

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
            return GameStateEnum.Tutorial;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), Color.Gray);
            float bottom = drawMenuItem(m_fontMenu, "Tutorial", 100, Color.OrangeRed);
            /*bottom = drawMenuItem(m_currentSelection == MenuState.Settings ? m_fontMenuSelect : m_fontMenu, "Settings", bottom, m_currentSelection == MenuState.Settings ? Color.White : Color.LightGray);

            bottom = drawMenuItem(m_currentSelection == MenuState.Help ? m_fontMenuSelect : m_fontMenu, "Help", bottom, m_currentSelection == MenuState.Help ? Color.White : Color.LightGray);
            bottom = drawMenuItem(m_currentSelection == MenuState.Resume ? m_fontMenuSelect : m_fontMenu, "Resume", bottom, m_currentSelection == MenuState.Resume ? Color.White : Color.LightGray);
            bottom = drawMenuItem(m_currentSelection == MenuState.Menu ? m_fontMenuSelect : m_fontMenu, "Main Menu", bottom, m_currentSelection == MenuState.Menu ? Color.White : Color.LightGray);*/
            drawMenuItem(m_currentSelection == MenuState.GamePlay ? m_fontMenuSelect : m_fontMenu, "Start Game!", bottom, m_currentSelection == MenuState.GamePlay ? Color.White : Color.LightGray);
            m_spriteBatch.End();
        }


        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {

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

            if (text == "Settings")
            {
                settings = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);
            }
            if (text == "Resume")
            {
                resume = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);

            }
            if (text == "Help")
            {
                help = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);

            }
            if (text == "Main Menu")
            {
                menu = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);


            }
            if (text == "Start Game!")
            {
                GamePlay = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);


            }

            return y + stringSize.Y;
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
