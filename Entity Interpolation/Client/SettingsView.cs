using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class SettingsView : GameStateView
    {
        public Keys up = Keys.W;
        public Keys left = Keys.A;
        public Keys right = Keys.D;
        public GameStateEnum prevState = GameStateEnum.MainMenu;
        private Texture2D backgroundImage;
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private bool isKeySelected = false;
        private bool m_waitForKeyRelease = false;
        private Rectangle Up = new Rectangle();
        private Rectangle Left = new Rectangle();
        private Rectangle Right = new Rectangle();
        private bool saving = false;
        private bool loading = false;
        //private KeyControls m_loadedState = null;
        private Texture2D whiteBackground;
        private bool isEnterUp = false;
        //KeyboardInput keyboard;
        private enum KeySelection
        {
            Up,
            Left,
            Right,
            None,
        }
        private KeySelection m_currentSelection = KeySelection.Up;


        public override void loadContent(ContentManager contentManager)
        {
            backgroundImage = contentManager.Load<Texture2D>("saturnCool");
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/voicActivatedFont");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/selectedVoiceActivatedFont");
            whiteBackground = contentManager.Load<Texture2D>("whiteImage");
            /*keyboard = new KeyboardInput();

            keyboard.registerCommand(Keys.Enter, true, new IInputDevice.CommandDelegate(enterHit));*/
            //loadKeyControls();
/*
            up = m_loadedState.Up;
            left = m_loadedState.Left;
            right = m_loadedState.Right;*/
        }
        public void enterHit(GameTime gameTime)
        {
            if (isEnterUp)
            {
                isKeySelected = true;
            }
            isEnterUp = false;
        }
        public override GameStateEnum processInput(GameTime gameTime)
        {
            //keyboard.Update(gameTime);
           
            if (Keyboard.GetState().IsKeyUp(Keys.Enter)) 
            {
                isEnterUp = true;
            }

            if (!m_waitForKeyRelease)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {

                    if (isKeySelected)
                    {
                        isKeySelected = false;
                        m_waitForKeyRelease = true;

                    }
                    else
                    {
                        isEnterUp = false;
                        m_waitForKeyRelease = true;
                        return prevState;
                    }
                }

                if (isKeySelected)
                {
                    Keys[] keys = Keyboard.GetState().GetPressedKeys();
                    if (keys.Length > 0)
                    {
                        m_waitForKeyRelease = true;

                        if (keys[0] != Keys.Escape && keys[0] != Keys.Enter)
                        {
                            if (m_currentSelection == KeySelection.Left)
                            {
                                isKeySelected = false;

                                left = keys[0];
                                //saveKeyControls();
                            }
                            else if (m_currentSelection == KeySelection.Up)
                            {
                                isKeySelected = false;

                                up = keys[0];
                                //saveKeyControls();

                            }
                            else if (m_currentSelection == KeySelection.Right)
                            {
                                isKeySelected = false;

                                right = keys[0];
                                //saveKeyControls();

                            }
                        }

                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        if (m_currentSelection == KeySelection.Right)
                        {
                            m_currentSelection = KeySelection.Up;
                        }
                        else
                        {
                            m_currentSelection++;
                        }
                        m_waitForKeyRelease = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        if (m_currentSelection == KeySelection.Up)
                        {
                            m_currentSelection = KeySelection.Right;
                        }
                        else
                        {
                            m_currentSelection--;
                        }
                        m_waitForKeyRelease = true;
                    }
                    
                    if (Up.Contains(Mouse.GetState().Position))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            isKeySelected = true;

                        }
                        m_currentSelection = KeySelection.Up;

                    }
                    else if (Left.Contains(Mouse.GetState().Position))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            isKeySelected = true;

                        }
                        m_currentSelection = KeySelection.Left;

                    }
                    else if (Right.Contains(Mouse.GetState().Position))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            isKeySelected = true;

                        }
                        m_currentSelection = KeySelection.Right;

                    }

                }

                
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                m_waitForKeyRelease = false;
            }
            



             




            /*else 
            {
                m_currentSelection = MenuState.None;
            }*/



            return GameStateEnum.Settings;
            
        }

        public override void render(GameTime gameTime)
        {
            // Render background image...
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), Color.Gray);

            float scale1 = m_graphics.PreferredBackBufferWidth / 1920f;

            Vector2 stringSize2 = m_fontMenu.MeasureString("Press ESC To Return") * scale1;

            m_spriteBatch.Draw(whiteBackground, new Rectangle((int)(m_graphics.PreferredBackBufferWidth / 5 - stringSize2.X / 2),
            (int)(m_graphics.PreferredBackBufferHeight / 10f - stringSize2.Y), (int)stringSize2.X, (int)stringSize2.Y), Color.White);

            m_spriteBatch.DrawString(
                           m_fontMenu,
                           "Press ESC To Return",
                           new Vector2(m_graphics.PreferredBackBufferWidth / 5 - stringSize2.X / 2,
            m_graphics.PreferredBackBufferHeight / 10f - stringSize2.Y),
                           Color.Black,
                           0,
                           Vector2.Zero,
                           scale1,
                           SpriteEffects.None,
                           0);
            // Display the current Keys and their buttons...
            float bottom = drawMenuItem(m_fontMenu, "Settings", m_graphics.PreferredBackBufferHeight / 1080f * 100f, Color.Red);
            bottom = drawMenuItem(m_currentSelection == KeySelection.Up ? m_fontMenuSelect: m_fontMenu, "Thrust     : " + up.ToString(), bottom, m_currentSelection == KeySelection.Up && isKeySelected ? Color.Blue : Color.White);

            bottom = drawMenuItem(m_currentSelection == KeySelection.Left ? m_fontMenuSelect : m_fontMenu, "Rotate Left: " + left.ToString(), bottom, m_currentSelection == KeySelection.Left && isKeySelected ? Color.Blue : Color.White);
            bottom = drawMenuItem(m_currentSelection == KeySelection.Right ? m_fontMenuSelect : m_fontMenu, "Rotate Right: " + right.ToString(), bottom, m_currentSelection == KeySelection.Right && isKeySelected ? Color.Blue : Color.White);

            bottom = drawMenuItem(m_fontMenu, "Press Enter To Select a Key Binding To Change", bottom + stringSize2.Y, Color.LightGray);
            bottom = drawMenuItem(m_fontMenu, "Once Blue, Select The Preferred Key For That Control", bottom + stringSize2.Y, Color.LightGray);
            bottom = drawMenuItem(m_fontMenu, "Press Escape If You Change Your Mind (If Blue)", bottom + stringSize2.Y, Color.LightGray);


            m_spriteBatch.End();

        }
        


        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {

            float scale = m_graphics.PreferredBackBufferWidth / 1980f;
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

            if (text == "Thrust     : " + up.ToString())
            {
                Up = new Rectangle(((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2) - 10, (int)y, (int)stringSize.X + 20, (int)stringSize.Y);
                    
            }
            if (text == "Rotate Left: " + left.ToString())
            {
                Left = new Rectangle(((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2) - 10, (int)y, (int)stringSize.X + 20, (int)stringSize.Y);
                    
            }
            if (text == "Rotate Right: " + right.ToString())
            {
                Right = new Rectangle(((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2) - 10, (int)y , (int)stringSize.X + 20, (int)stringSize.Y);
                
            }
            

            return y + stringSize.Y;
        }



        /// <summary>
        /// Demonstrates how serialize an object to storage
        /// </summary>
        /*private void saveKeyControls()
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;

                    // Create something to save
                    KeyControls myState = new KeyControls(this.left, this.right, this.up);

                    // Yes, I know the result is not being saved, I dont' need it
                    finalizeSaveAsync(myState);
                }
            }
        }

        private async Task finalizeSaveAsync(KeyControls state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("KeyControls.json", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(KeyControls));
                                mySerializer.WriteObject(fs, state);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                    }
                }

                this.saving = false;
            });
        }

        /// <summary>
        /// Demonstrates how to deserialize an object from storage device
        /// </summary>
        private void loadKeyControls()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    // Yes, I know the result is not being saved, I dont' need it
                    var result = finalizeLoadAsync();
                    result.Wait();

                }
            }
        }

        private async Task finalizeLoadAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("KeyControls.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("KeyControls.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(KeyControls));
                                    m_loadedState = (KeyControls)mySerializer.ReadObject(fs);
                                }

                                
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                    }
                }

                this.loading = false;
            });
        }*/

        public override void update(GameTime gameTime)
        {
        }
    }
}
