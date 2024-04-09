using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class HelpView : GameStateView
    {
        private Texture2D backgroundImage;
        private Rectangle backgroundRect;
        private SpriteFont m_font;
        private Texture2D whiteBackground;

        public GameStateEnum helpPrevState = GameStateEnum.MainMenu;

        public override void loadContent(ContentManager contentManager)
        {

            backgroundImage = contentManager.Load<Texture2D>("Cartoon_green_texture_grass_smaller");
            backgroundRect = new Rectangle(0 ,0 ,m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
            m_font = contentManager.Load<SpriteFont>("Fonts/voicActivatedFont");
            whiteBackground = contentManager.Load<Texture2D>("whiteImage");



        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return helpPrevState;
            }

            return GameStateEnum.Help;
        }

        public override void render(GameTime gameTime)
        {
            // Render some controls and how to play the game...

            // Render background image
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(backgroundImage, backgroundRect, Color.White);




            float scale1 = m_graphics.PreferredBackBufferWidth / 1920f;

            Vector2 stringSize2 = m_font.MeasureString("Press ESC To Return") * scale1;

            m_spriteBatch.Draw(whiteBackground, new Rectangle((int)(m_graphics.PreferredBackBufferWidth / 5 - stringSize2.X / 2),
            (int)(m_graphics.PreferredBackBufferHeight / 10f - stringSize2.Y), (int)stringSize2.X,(int)stringSize2.Y), Color.White);

            m_spriteBatch.DrawString(
                           m_font,
                           "Press ESC To Return",
                           new Vector2(m_graphics.PreferredBackBufferWidth / 5 - stringSize2.X / 2,
            m_graphics.PreferredBackBufferHeight / 10f - stringSize2.Y),
                           Color.Black,
                           0,
                           Vector2.Zero,
                           scale1,
                           SpriteEffects.None,
                           0);
            float scale = m_graphics.PreferredBackBufferWidth / 1920f;

            Vector2 stringSize = m_font.MeasureString("How To Play") * scale;

            // How to play : Top of screen
            float bottom = drawMenuItem(m_font, "How To Play", m_graphics.PreferredBackBufferHeight / 1080f * 100f,  Color.Black);
            bottom = drawMenuItem(m_font, "Safely Land Under These Conditions: ", bottom + stringSize.Y, Color.Black);
            bottom = drawMenuItem(m_font, " -- Angle Between 355 And 5 Degrees -- ", bottom, Color.Black);

            bottom = drawMenuItem(m_font, " -- Velocity: 2 Or Less m/s -- ", bottom, Color.Black);
            bottom = drawMenuItem(m_font, " -- Land On Specified Flat Surface(s) (In Blue!) -- ", bottom, Color.Black);


            // Show control usage

            bottom = drawMenuItem(m_font, "Controls", m_graphics.PreferredBackBufferHeight / 2, Color.Black);
            bottom = drawMenuItem(m_font, "Thrust: Thrust The Rocket In Tilted Direction", bottom, Color.Black);
            bottom = drawMenuItem(m_font, "Left: Tilt Rocket In Left Direction", bottom, Color.Black);
            bottom = drawMenuItem(m_font, "Right: Tilt Rocket In Right Direction", bottom, Color.Black);
            bottom = drawMenuItem(m_font, "See Settings Page For Key Bindings", bottom + stringSize.Y, Color.Black);





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
            // Nothing here...
        }

        
    }
}
