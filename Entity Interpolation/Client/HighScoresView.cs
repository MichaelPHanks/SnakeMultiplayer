using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;

namespace Client
{
    public class HighScoresView : GameStateView
    {

        private bool loading = false;
        private HighScoresState m_loadedState = null;
        private SpriteFont m_fontMenu;

        private Texture2D backgroundImage;
        private Rectangle backgroundRect;
        private Texture2D whiteBackground;

        public override void loadContent(ContentManager contentManager)
        {
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/voicActivatedFont");
            backgroundImage = contentManager.Load<Texture2D>("Cartoon_green_texture_grass_smaller");
            backgroundRect = new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
            whiteBackground = contentManager.Load<Texture2D>("whiteImage");
            loadHighScores();
        }

        private void loadHighScores()
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
                            using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(HighScoresState));
                                    m_loadedState = (HighScoresState)mySerializer.ReadObject(fs);
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }
            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            // Draw something bro
            m_spriteBatch.Begin();
            //m_spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), Color.Gray);
            m_spriteBatch.Draw(backgroundImage, backgroundRect, Color.White);


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
            float bottom = drawMenuItem(m_fontMenu, "High Scores!", m_graphics.PreferredBackBufferHeight / 1080f * 100f, Color.Black);
            if (m_loadedState != null)
            {
                foreach (var state in m_loadedState.getHighScores())
                {
                    bottom = drawMenuItem(m_fontMenu, state.Item1.ToString() + " --- " + state.Item2.ToString(), bottom + stringSize2.Y, Color.Black);
                }
            }
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

            return y + stringSize.Y;
        }

        public override void update(GameTime gameTime)
        {
            // Likely going to be nothing in here...
        }
    }
}
