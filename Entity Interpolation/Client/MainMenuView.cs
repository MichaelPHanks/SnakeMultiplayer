using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Client
{
    public class MainMenuView : GameStateView
    {
        private Texture2D mainBackground;
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private SpriteFont m_fontTitle;
        private Rectangle gameplay = new Rectangle();
        private Rectangle about = new Rectangle();
        private Rectangle quit = new Rectangle();
        private Rectangle help = new Rectangle();
        private Rectangle highScores = new Rectangle();
        private Rectangle settings = new Rectangle();
        private bool isEnterUp = false;
        SoundEffect hover;
        SoundEffectInstance soundInstance;
        public bool canUseMouse = false;





        private enum MenuState
        {
            NewGame,
            HighScores,
            Help,
            Settings,
            About,
            Quit,
            None,
        }

        private MenuState m_currentSelection = MenuState.NewGame;
        private MenuState m_prevSelection = MenuState.NewGame;
        private bool m_waitForKeyRelease = false;
        public override void loadContent(ContentManager contentManager)
        {
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-selected");
            hover = contentManager.Load<SoundEffect>("little_robot_sound_factory_multimedia_Click_Electronic_14");
            mainBackground = contentManager.Load<Texture2D>("flbb_3udr_220615");
            m_fontTitle = contentManager.Load<SpriteFont>("Fonts/mainmenuTitle");
            soundInstance = hover.CreateInstance();




        }

        

        public override GameStateEnum processInput(GameTime gameTime)
        {
            
            if (Keyboard.GetState().IsKeyUp(Keys.Enter)) 
            {
                isEnterUp = true;
            }
            
            
            if (!m_waitForKeyRelease && isEnterUp)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {   if (m_currentSelection == MenuState.Quit)
                    {
                        m_currentSelection = MenuState.NewGame;
                    }
                    else 
                    {
                        m_currentSelection++;
                    }
                    m_waitForKeyRelease = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    if (m_currentSelection == MenuState.NewGame)
                    {
                        m_currentSelection = MenuState.Quit;
                    }
                    else
                    {
                        m_currentSelection--;
                    }
                    m_waitForKeyRelease = true;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.NewGame)
                {
                    canUseMouse = false;
                    isEnterUp = false;
                    return GameStateEnum.GamePlay;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.HighScores)
                {
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.HighScores;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Help)
                {
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.Help;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.About)
                {
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.About;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Quit)
                {
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.Exit;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Settings)
                {
                    isEnterUp = false;
                    canUseMouse = false;

                    return GameStateEnum.Settings;
                }
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                m_waitForKeyRelease = false;
            }


            if (canUseMouse)
            {
                if (gameplay.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        canUseMouse = false;
                        isEnterUp = false;

                        return GameStateEnum.GamePlay;
                    }
                    m_currentSelection = MenuState.NewGame;



                }
                else if (help.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        canUseMouse = false;
                        isEnterUp = false;

                        return GameStateEnum.Help;
                    }
                    m_currentSelection = MenuState.Help;

                }
                else if (about.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        canUseMouse = false;
                        isEnterUp = false;

                        return GameStateEnum.About;
                    }
                    m_currentSelection = MenuState.About;

                }
                else if (highScores.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        canUseMouse = false;
                        isEnterUp = false;

                        return GameStateEnum.HighScores;
                    }
                    m_currentSelection = MenuState.HighScores;

                }
                else if (quit.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        canUseMouse = false;
                        isEnterUp = false;

                        return GameStateEnum.Exit;
                    }
                    m_currentSelection = MenuState.Quit;

                }

                else if (settings.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        canUseMouse = false;
                        isEnterUp = false;

                        return GameStateEnum.Settings;
                    }
                    m_currentSelection = MenuState.Settings;

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

            return GameStateEnum.MainMenu;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            // Rend the background

            m_spriteBatch.Draw(mainBackground, new Rectangle(0,0,m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight),Color.White);

            float bottom = drawMenuItem(m_fontTitle, "LUNAR LANDER", m_graphics.PreferredBackBufferHeight / 4, Color.OrangeRed);
             bottom = drawMenuItem(m_currentSelection == MenuState.NewGame ? m_fontMenuSelect: m_fontMenu, "New Game", bottom, m_currentSelection == MenuState.NewGame ? Color.White:Color.LightGray);
            
            bottom = drawMenuItem(m_currentSelection == MenuState.HighScores ? m_fontMenuSelect : m_fontMenu, "High Scores", bottom, m_currentSelection == MenuState.HighScores ? Color.White : Color.LightGray);
            
            bottom = drawMenuItem(m_currentSelection == MenuState.Help ? m_fontMenuSelect : m_fontMenu, "Help", bottom, m_currentSelection == MenuState.Help ? Color.White : Color.LightGray);
            bottom = drawMenuItem(m_currentSelection == MenuState.Settings ? m_fontMenuSelect : m_fontMenu, "Settings", bottom, m_currentSelection == MenuState.Settings ? Color.White : Color.LightGray);

            bottom = drawMenuItem(m_currentSelection == MenuState.About ? m_fontMenuSelect : m_fontMenu, "About", bottom, m_currentSelection == MenuState.About ? Color.White : Color.LightGray);
            drawMenuItem(m_currentSelection == MenuState.Quit ? m_fontMenuSelect : m_fontMenu, "Quit", bottom, m_currentSelection == MenuState.Quit ? Color.White : Color.LightGray);


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

            if (text == "New Game")
            {
                gameplay = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);
            }
            if (text == "High Scores")
            {
                highScores = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);

            }
            if (text == "Help")
            {
                help = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);

            }
            if (text == "About")
            {
                about = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);

            }
            if (text == "Quit")
            {
                quit = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);

            }
            if (text == "Settings")
            { 
                settings = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, (int)stringSize.X, (int)stringSize.Y);
            }
            return y + stringSize.Y;
        }
        

        public override void update(GameTime gameTime)
        {

            

        }
    }
}
