
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Entities;
using System;

namespace Client.Systems
{
    public class Renderer : Shared.Systems.System
    {

        public Renderer() :
            base(
                typeof(Client.Components.Sprite),
                typeof(Shared.Components.Position),
                typeof(Shared.Components.Size)
                )
        {

        }

        public override void update(TimeSpan elapsedTime) { }

        public void update(TimeSpan elapsedTime, SpriteBatch spriteBatch, int gameWidth, int gameHeight, Texture2D backgroundImage)
        {

            spriteBatch.Begin();
            
            // Render the player entity: 

            // Render to the middle of the screen

            // Our position in the world, need to use this to render other players (and future entities)

            // NOTE: The player and enemy are slightly off in terms of rendering,
            // so this potentially means that when we use the try catch block for the first time an entity joins the server,
            // We are skipping sending the first update and position or something of the other players...


            // ANOTHER NOTE: if we want the edges to line up nicely, we need to relate to everythng as 1920 X 1080
            if (m_entity != null) { 


                
            var position1 = m_entity.get<Shared.Components.Position>().position;
            // Render the background before anything
            Rectangle tempRectangle = new Rectangle((int)(position1.X - gameWidth / 2), (int)(position1.Y - gameHeight / 2), gameWidth, gameHeight);

            // Render the top
            //spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % gameWidth),-(int)(position1.Y % gameHeight), gameWidth, gameHeight), Color.White);

                // Render the rest...



                spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + 1920 / 2, -(int)(position1.Y % 1080) + 1080 / 2, 1920, 1080), Color.White);
                spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) - 1920 / 2, -(int)(position1.Y % 1080) + 1080 / 2, 1920, 1080), Color.White);
                spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + 1920 / 2, -(int)(position1.Y % 1080) - 1080 / 2, 1920, 1080), Color.White);
                spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + 1920 / 2 + 1920, -(int)(position1.Y % 1080) + 1080 / 2, 1920, 1080), Color.White);

                var orientation1 = m_entity.get<Shared.Components.Position>().orientation;
            var size1 = m_entity.get<Shared.Components.Size>().size;
            var texture1 = m_entity.get<Components.Sprite>().texture;
            var texCenter1 = m_entity.get<Components.Sprite>().center;

            // Build a rectangle centered at position, with width/height of size
            Rectangle rectangle1 = new Rectangle(
                (int)(gameWidth / 2),
                (int)(gameHeight / 2),
                (int)size1.X,
                (int)size1.Y);

                if (m_entity.isAlive)
                {
                    spriteBatch.Draw(
                        texture1,
                        rectangle1,
                        null,
                        Color.White,
                        orientation1,
                        texCenter1,
                        SpriteEffects.None,
                        0);
                }


                // Render all others within area of player entity

                foreach (Entity entity in m_entities.Values)
                {
                    if (entity.isAlive)
                    {


                        if (!entity.contains<Shared.Components.Input>())
                        {
                            var position = entity.get<Shared.Components.Position>().position;

                            if (tempRectangle.Contains(position.X, position.Y))
                            {
                                /*double divider = position1.X / (gameWidth / 2);
                                double divider2 = position1.Y / (gameHeight / 2);
                                double playerX = position.X / divider;
                                double playerY = position.Y / divider2;*/


                                // Get the difference and add them together.


                                double differenceX = position1.X - position.X;
                                double differenceY = position1.Y - position.Y;
                                var orientation = entity.get<Shared.Components.Position>().orientation;
                                var size = entity.get<Shared.Components.Size>().size;
                                var texture = entity.get<Components.Sprite>().texture;
                                var texCenter = entity.get<Components.Sprite>().center;

                                // Build a rectangle centered at position, with width/height of size
                                Rectangle rectangle = new Rectangle(
                                    (int)(gameWidth / 2 - differenceX),
                                    (int)(gameHeight / 2 - differenceY),
                                    (int)size.X,
                                    (int)size.Y);

                                spriteBatch.Draw(
                                    texture,
                                    rectangle,
                                    null,
                                    Color.White,
                                    orientation,
                                    texCenter,
                                    SpriteEffects.None,
                                    0);
                            }



                        }
                    }
                    /*else
                    {

                        var position = entity.get<Shared.Components.Position>().position;


                        var orientation = entity.get<Shared.Components.Position>().orientation;
                        var size = entity.get<Shared.Components.Size>().size;
                        var texture = entity.get<Components.Sprite>().texture;
                        var texCenter = entity.get<Components.Sprite>().center;

                        // Build a rectangle centered at position, with width/height of size
                        Rectangle rectangle = new Rectangle(
                            (int)(position.X - size.X / 2),
                            (int)(position.Y - size.Y / 2),
                            (int)size.X,
                            (int)size.Y);

                        spriteBatch.Draw(
                            texture,
                            rectangle,
                            null,
                            Color.White,
                            orientation,
                            texCenter,
                            SpriteEffects.None,
                            0);
                    }*/
                }
            }

            spriteBatch.End();
        }
    }
}
