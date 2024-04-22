
using Client.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Entities;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace Client.Systems
{
    public class Renderer : Shared.Systems.System
    {
        // Note: For the game map, it is of size 1000 times 5 wide and 1000 times 5 tall.
        public Renderer() :
            base(
                typeof(Client.Components.Sprite),
                typeof(Shared.Components.Position),
                typeof(Shared.Components.Size)
                )
        {

        }

        public override void update(TimeSpan elapsedTime) { }

        public void update(TimeSpan elapsedTime, SpriteBatch spriteBatch, double gameWidth, double gameHeight, Texture2D backgroundImage, Texture2D wallImage, AnimatedSprite animatedRenderer, SpriteFont font, Dictionary<uint, List<Entity>> perPlayerEntities, List<Tuple<string, int>> gameScores, Dictionary<uint, string> playerNames)
        {

            // Create all of the rectangles:
            List<Rectangle> backgroundTiles = new List<Rectangle>();
            for (int i = 0; i < 5000; i += 1000)
            {
                for (int j = 0; j < 5000; j += 1000)
                {
                    backgroundTiles.Add(new Rectangle(i,j,1000,1000));
                }
            }

            // Make border tiles
            List<Rectangle> borderTiles = new List<Rectangle>();

            borderTiles.Add(new Rectangle(-1000,0, 1000, 1000));
            borderTiles.Add(new Rectangle(-1000,-1000,1000,1000));
            borderTiles.Add(new Rectangle(-1000, 1000, 1000,1000));
            borderTiles.Add(new Rectangle(-1000,2000,1000,1000));
            borderTiles.Add(new Rectangle(-1000, 3000, 1000, 1000));

            borderTiles.Add(new Rectangle(-1000, 4000, 1000, 1000));
            borderTiles.Add(new Rectangle(-1000, 5000, 1000, 1000));
            borderTiles.Add(new Rectangle(0, 5000, 1000, 1000));
            borderTiles.Add(new Rectangle(1000, 5000, 1000, 1000));
            borderTiles.Add(new Rectangle(2000, 5000, 1000, 1000));
            borderTiles.Add(new Rectangle(3000, 5000, 1000, 1000));
            borderTiles.Add(new Rectangle(4000, 5000, 1000, 1000));
            borderTiles.Add(new Rectangle(5000, 5000, 1000, 1000));

            borderTiles.Add(new Rectangle(5000, 4000, 1000, 1000));
            borderTiles.Add(new Rectangle(5000, 3000, 1000, 1000));
            borderTiles.Add(new Rectangle(5000, 2000, 1000, 1000));
            borderTiles.Add(new Rectangle(5000, 1000, 1000, 1000));
            borderTiles.Add(new Rectangle(5000, 0, 1000, 1000));
            borderTiles.Add(new Rectangle(5000, -1000, 1000, 1000));


            borderTiles.Add(new Rectangle(0, -1000, 1000, 1000));
            borderTiles.Add(new Rectangle(1000, -1000, 1000, 1000));
            borderTiles.Add(new Rectangle(2000, -1000, 1000, 1000));
            borderTiles.Add(new Rectangle(3000, -1000, 1000, 1000));
            borderTiles.Add(new Rectangle(4000, -1000, 1000, 1000));

            // What to do here: create 'entities' (not actual entities) that are the tiles for the game!
            spriteBatch.Begin();


            




            



            if (m_entity != null)
            {
                float ScreenX = m_entity.get<Shared.Components.Position>().position.X - 500;
                float ScreenY = m_entity.get<Shared.Components.Position>().position.Y - 500;

                Rectangle viewPort = new Rectangle((int)(ScreenX), (int)ScreenY , 1000, 1000);
                foreach (Rectangle r in backgroundTiles)
                {
                    if (viewPort.Intersects(r))
                    {
                        // Convert from world to screen

                        Rectangle tempRectangle = new Rectangle((int)(r.X - ScreenX), (int)(r.Y - ScreenY), 1000,1000);
                        // Render the tile

                        spriteBatch.Draw(backgroundImage, tempRectangle, Color.White);

                    }
                }

                foreach (Rectangle r in borderTiles)
                {
                    if (viewPort.Intersects(r))
                    {

                        // Convert from world to screen


                        int tempX = r.X - 500;
                        int tempY = r.Y - 500;  

                        Rectangle tempRectangle = new Rectangle((int)(r.X - ScreenX), (int)(r.Y - ScreenY), 1000, 1000);
                        // Render the tile

                        spriteBatch.Draw(wallImage, tempRectangle, Color.White);

                    }
                }



              

                foreach (List<Entity> entities in perPlayerEntities.Values)
                {

                    foreach (Entity entity in entities)
                    {
                        if ((entity.contains<Shared.Components.Head>() || entity.contains<Shared.Components.Segment>() || entity.contains<Shared.Components.Tail>()) && entity.isAlive)
                        {



                            int tempX = (int)entity.get<Shared.Components.Position>().position.X - 500;
                            int tempY = (int)entity.get<Shared.Components.Position>().position.Y - 500;
                            var position = entity.get<Shared.Components.Position>().position;
                            var size = entity.get<Shared.Components.Size>().size;


                            var orientation = entity.get<Shared.Components.Position>().orientation;
                            var texture = entity.get<Components.Sprite>().texture;
                            var texCenter = entity.get<Components.Sprite>().center;
                            Rectangle entityRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
                            Rectangle tempRectangle = new Rectangle();
                            if (viewPort.Intersects(entityRectangle))
                            {

                                // Convert from world to screen




                                tempRectangle = new Rectangle((int)(position.X - ScreenX), (int)(position.Y - ScreenY), (int)size.X, (int)size.Y);
                                // Render the tile



                                // Build a rectangle centered at position, with width/height of size


                                spriteBatch.Draw(
                                    texture,
                                    tempRectangle,
                                    null,
                                    Color.White,
                                    orientation,
                                    texCenter,
                                    SpriteEffects.None,
                                    0);
                            }
                            if (entity.contains<Shared.Components.Head>())
                            {
                                // Render the name of the player.

                               
                                float scale = 1000 / 1920f;

                                spriteBatch.DrawString(
                                                 font,
                                                 entity.get<Shared.Components.Name>().name,
                                                 new Vector2(tempRectangle.X - size.X, tempRectangle.Y - size.Y),
                                                 Color.White,
                                                 0,
                                                 Vector2.Zero,
                                                 scale,
                                                 SpriteEffects.None,
                                                 0);

                            }
                        }
                    }
                }



                foreach (Entity entity in m_entities.Values)
                {
                    if (!entity.contains<Shared.Components.Head>() && !entity.contains<Shared.Components.Segment>() && !entity.contains<Shared.Components.Tail>())
                    {

                        var position = entity.get<Shared.Components.Position>().position;
                        var size = entity.get<Shared.Components.Size>().size;


                        var orientation = entity.get<Shared.Components.Position>().orientation;
                        var texture = entity.get<Components.Sprite>().texture;
                        var texCenter = entity.get<Components.Sprite>().center;
                        Rectangle entityRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
                        // Render the food!
                        if (viewPort.Intersects(entityRectangle))
                        {
                            Rectangle tempRectangle = new Rectangle((int)(position.X - ScreenX), (int)(position.Y - ScreenY), (int)size.X, (int)size.Y);

                            Banana banana = new Banana(size*(float)1.5, new Vector2(tempRectangle.X + size.X / 2, tempRectangle.Y + size.Y / 2), 75 / 1000.0, // Pixels per second
                    (float)(Math.PI / 1000.0));


                            animatedRenderer.draw(spriteBatch, banana);

                        }

                    }


                }


                foreach (Tuple<string, int> playerScore in gameScores)
                {
                    if (playerNames[m_entity.id] == playerScore.Item1)
                    {
                        // Render the players score

                        float scale = 2;
                        Vector2 stringSize1 = font.MeasureString(playerScore.Item2.ToString()) * scale;
                        spriteBatch.DrawString(
                        font,
                        playerScore.Item2.ToString(),
                                  new Vector2((float)gameWidth /2f - stringSize1.X / 2,
                  (float)gameHeight / 4f - stringSize1.Y),
                                  Color.White,
                                  0,
                                  Vector2.Zero,
                                  scale,
                                  SpriteEffects.None,
                                  0);

                    }
                }
               


               





            }
            spriteBatch.End();
        }
    }
}
