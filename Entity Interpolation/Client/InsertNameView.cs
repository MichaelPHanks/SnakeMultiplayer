using Microsoft.Xna.Framework.Audio;
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
using Shared;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Json;
using Shared.Components;

namespace Client
{
    public class InsertNameView : GameStateView
    {

        // Step 1: Give the player a chance to name themselves
        // There is only one server, don't need to offer anything related to server selection or room selection
        // Note: This might take a while. Give a default random name to start (given from server). 
        // Should probably save name to the computer? Who knows.
        // The server needs to know about the name of the individual.
        private bool saving = false;
        private bool loading = false;

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

        PlayerNameState m_playerNameState = null;



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
            backgroundImage = contentManager.Load<Texture2D>("NotMainBackground");
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-selected");
            hover = contentManager.Load<SoundEffect>("little_robot_sound_factory_multimedia_Click_Electronic_14");
            soundInstance = hover.CreateInstance();

            loadPlayerName();
            playerName = m_playerNameState.getPlayerName();

        }
        private void savePlayername()
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;

                    // Create something to save
                    PlayerNameState myState = new PlayerNameState(playerName);

                    // Yes, I know the result is not being saved, I dont' need it
                    finalizeSaveAsync(myState);
                }
            }
        }

        private async Task finalizeSaveAsync(PlayerNameState state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("PlayerName.json", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(PlayerNameState));
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

        private void loadPlayerName()
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
                        if (storage.FileExists("PlayerName.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("PlayerName.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(PlayerNameState));
                                    m_playerNameState = (PlayerNameState)mySerializer.ReadObject(fs);
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
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                isEnterUp = true;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !isESCDown)
            {
                savePlayername();

                if (isKeySelected)
                {
                    isKeySelected = false;
                    isESCDown = true;
                }
                else
                {
                    isESCDown = true;
                    isEnterUp = false;

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
                    if (!isKeySelected)
                    {


                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;
                        return GameStateEnum.MainMenu;
                    }
                }

               
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Controls)
                {
                    if (!isKeySelected)
                    {
                        isESCDown = true;
                        isEnterUp = false;
                        canUseMouse = false;
                        return GameStateEnum.Controls;
                    }
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
                        if (!isKeySelected)
                        {
                            isESCDown = true;
                            isEnterUp = false;
                            canUseMouse = false;
                            return GameStateEnum.MainMenu;

                        }
                    }
                    m_currentSelection = MenuState.Menu;

                }
                else if (Continue.Contains(Mouse.GetState().Position))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (!isKeySelected)
                        {
                            isESCDown = true;
                            isEnterUp = false;
                            canUseMouse = false;
                            return GameStateEnum.Controls;

                        }
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
            float bottom = drawMenuItem(m_fontMenu, "Insert Your Preferred Name", 100, Color.Black);
            bottom = drawMenuItem(m_currentSelection == MenuState.Menu ? m_fontMenuSelect : m_fontMenu, "Main Menu", bottom, m_currentSelection == MenuState.Menu ? Color.White : Color.LightGray);
            bottom = drawMenuItem(m_currentSelection == MenuState.Controls ? m_fontMenuSelect : m_fontMenu, "Continue", bottom, m_currentSelection == MenuState.Controls ? Color.White : Color.LightGray);
            m_spriteBatch.Draw(whiteImage, TextBox, isKeySelected ? Color.Red : Color.White);
            bottom = drawMenuItem(m_currentSelection == MenuState.Name ? m_fontMenuSelect : m_fontMenu, playerName, bottom, Color.Black);
            bottom=  drawMenuItem(m_fontMenu, "\n\nSelect the text box above", bottom, Color.Black);
            bottom = drawMenuItem(m_fontMenu, " to insert name\n\n", bottom, Color.Black);
            bottom = drawMenuItem(m_fontMenu, "Unselect text box with ESC", bottom, Color.Black);
            bottom = drawMenuItem(m_fontMenu, "Press Continue when done", bottom, Color.Black);



            m_spriteBatch.End();
        }


        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {

            float scale = m_graphics.PreferredBackBufferHeight / 1080f;
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
                    TextBox = new Rectangle((int)m_graphics.PreferredBackBufferWidth / 2 - (int)stringSize.X / 2, (int)y, 30, 30);
                }

            }

            return y + stringSize.Y;
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
