
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Entities;
using System;

namespace Client.Systems
{
    public class Renderer : Shared.Systems.System
    {
        // Note: For the game map, it is of size 1920 times 5 wide and 1080 times 5 tall.
        public Renderer() :
            base(
                typeof(Client.Components.Sprite),
                typeof(Shared.Components.Position),
                typeof(Shared.Components.Size)
                )
        {

        }

        public override void update(TimeSpan elapsedTime) { }

        public void update(TimeSpan elapsedTime, SpriteBatch spriteBatch, double gameWidth, double gameHeight, Texture2D backgroundImage, Texture2D wallImage)
        {
            // What to do here: create 'entities' (not actual entities) that are the tiles for the game!
            spriteBatch.Begin();

            // Render the player entity: 

            // Render to the middle of the screen

            // Our position in the world, need to use this to render other players (and future entities)

            // NOTE: The player and enemy are slightly off in terms of rendering,
            // so this potentially means that when we use the try catch block for the first time an entity joins the server,
            // We are skipping sending the first update and position or something of the other players...


            double factorX = 1920 / gameWidth;
            double factorY = 1080 / gameHeight;

            // ANOTHER NOTE: if we want the edges to line up nicely, we need to relate to everything as 1920 X 1080
            if (m_entity != null) {


                

                
            var position1 = m_entity.get<Shared.Components.Position>().position;

                
                // Render the background before anything
                Rectangle tempRectangle = new Rectangle((int)(position1.X - gameWidth / 2), (int)(position1.Y - gameHeight / 2), (int)gameWidth, (int)gameHeight);






                int x = -(int)(position1.X % 1920) + 1920 / 2;
                int y = -(int)(position1.Y % 1080) + 1080 / 2;
                int thingsDrawn = 0;

                if (position1.Y <= 0)
                {
                    position1.Y = 1;
                }

                if (position1.X <= 0)
                {
                    position1.X = 1;
                }

                if (position1.X >= (1920 * 5))
                {
                    position1.X = (1920 * 5) - 1;
                }
                if (position1.Y >= (1080 * 5))
                {
                    position1.Y = (1080 * 5) - 1;
                }


                // Check for boundaries of the game map.


                // If we can render the right side of the player: 

                if (position1.X <= (1920 * 5) - 1920)
                {
                    thingsDrawn += 1;
                    // Take the player position, 
                    // Tile to the right of the player
                    /*spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + gameWidth / 2 + 1920, -(int)(position1.Y % 1080) + gameHeight / 2, 1920, 1080), Color.White);
*/
                    /*if (position1.Y >= 1080)
                    {
                        thingsDrawn += 1;

                        // Tile above and to the right of the player
                        spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) + 1920, -(int)(position1.Y % 1080) + (1080 / 2) - 1080, 1920, 1080), Color.White);
                    }

                    

                    if (position1.Y <= (1080 * 5) - 1080)
                    {
                        thingsDrawn += 1;

                        // Tile bottom and to the right of the player
                        spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) + 1920, -(int)(position1.Y % 1080) + (1080 / 2) + 1080, 1920, 1080), Color.White);

                    }*/
                    

                }

                else
                {
                    /*thingsDrawn += 3;

                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) + 1920, -(int)(position1.Y % 1080) + (1080 / 2) + 1080, 1920, 1080), Color.White);
                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) + 1920, -(int)(position1.Y % 1080) + (1080 / 2) - 1080, 1920, 1080), Color.White);
                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + 1920 / 2 + 1920, -(int)(position1.Y % 1080) + 1080 / 2, 1920, 1080), Color.White);
*/
                }


                if (position1.X >= 1920)
                {
                    // Render the left tiles
                    thingsDrawn += 1;

                    // Tile to the left of the player
                   /* spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) - gameWidth / 2, -(int)(position1.Y % 1080) + gameHeight / 2, 1920, 1080), Color.White);
*/

                    /*if (position1.Y >= 1080)
                    {
                        // Tile above and to the left of the player
                        thingsDrawn += 1;

                        spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) - 1920, -(int)(position1.Y % 1080) + (1080 / 2) - 1080, 1920, 1080), Color.White);

                    }



                    if (position1.Y <= (1080 * 5) - 1080)
                    {
                        thingsDrawn += 1;

                        // Tile bottom and to the left of the player
                        spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) - 1920, -(int)(position1.Y % 1080) + (1080 / 2) + 1080, 1920, 1080), Color.White);

                    }

*/

                }

                else
                {
                    /*thingsDrawn += 3;

                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) - 1920, -(int)(position1.Y % 1080) + (1080 / 2) - 1080, 1920, 1080), Color.White);

                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) - 1920, -(int)(position1.Y % 1080) + (1080 / 2) + 1080, 1920, 1080), Color.White);
                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) - 1920 / 2, -(int)(position1.Y % 1080) + 1080 / 2, 1920, 1080), Color.White);
*/
                }


                /*if (position1.Y <= (1080 * 5) - 1080)
                {
                    // Render below the player.
                    thingsDrawn += 1;

                    // Tile below the player
                    spriteBatch.Draw(backgroundImage, new Rectangle(-(int)(position1.X % 1920) + (gameWidth / 2), -(int)(position1.Y % 1080) + (gameHeight / 2) + 1080, 1920, 1080), Color.White);

                }*/

                /*else
                {
                    thingsDrawn += 3;

                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2), -(int)(position1.Y % 1080) + (1080 / 2) + 1080, 1920, 1080), Color.White);
                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) - 1920, -(int)(position1.Y % 1080) + (1080 / 2) + 1080, 1920, 1080), Color.White);
                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) + 1920, -(int)(position1.Y % 1080) + (1080 / 2) + 1080, 1920, 1080), Color.White);

                }*/


                /*if (position1.Y >= 1080)
                {
                    // Tile above the player
                    thingsDrawn += 1;

                    spriteBatch.Draw(
                        backgroundImage,
                        new Rectangle(-(int)(position1.X % 1920) + gameWidth / 2, -(int)(position1.Y % 1080) + gameHeight / 2 - 1080, 1920, 1080),
                        Color.White
                    );

                }*/

                /*else
                {
                    thingsDrawn += 3;

                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + 1920 / 2, -(int)(position1.Y % 1080) - 1080 / 2, 1920, 1080), Color.White);
                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) - 1920, -(int)(position1.Y % 1080) + (1080 / 2) - 1080, 1920, 1080), Color.White);
                    spriteBatch.Draw(wallImage, new Rectangle(-(int)(position1.X % 1920) + (1920 / 2) + 1920, -(int)(position1.Y % 1080) + (1080 / 2) - 1080, 1920, 1080), Color.White);

                }*/




                // Render this if we are not out of bounds at all

                if (position1.X > 0 && position1.X < (5000) && position1.Y > 0 && position1.Y < (5000))
                {

                    int upperLeftX = (int)(position1.X - 500);
                    int upperLeftY = (int)(position1.Y - 500);
                    // Main tile the player is actually on
                    spriteBatch.Draw(backgroundImage, new Rectangle((int)(-(int)(position1.X % 1000) + gameWidth / 2), (int)(-(int)(position1.Y % 1000) + gameHeight / 2), 1000, 1000), Color.White);
                    thingsDrawn += 1;


                }


                /*int upperLeftX = (int)(position1.X - 500);
                int upperLeftY = (int)(position1.Y - 500);
                for (int i = 0; i < 5000; i+=1000)
                {
                    for (int j = 0; j < 5000; j+=1000)
                    {

                        // i = 0, j= 0; i = 0, j = 1000

                        int yeah = ((i - upperLeftX) / (1000)) * (1000);
                        //spriteBatch.Draw(backgroundImage, new Rectangle(((i-upperLeftX) / (1000)) * (1000), ((j - upperLeftY) / (1000)) * (1000), 1000, 1000), Color.White);


                    }
                }
                spriteBatch.Draw(backgroundImage, new Rectangle(((1000 - upperLeftX) / (1000)) * (1000), ((1000 - upperLeftY) / (1000)) * (1000), 1000, 1000), Color.White);

*/









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

                            /*if (tempRectangle.Contains(position.X, position.Y))
                            {*/
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
                           /* }*/



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
