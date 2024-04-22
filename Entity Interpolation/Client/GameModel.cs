using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;
using Shared.Components;
using Shared.Entities;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Client
{
    public class GameModel
    {
        private ContentManager m_contentManager;
        private Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();
        private Dictionary<uint, string> playerNames = new Dictionary<uint, string>();
        private ParticleSystem m_particleSystemEatFood;
        private ParticleSystem m_particleSystemDeath;
        private ParticleSystemRenderer m_renderFoodEaten;
        private ParticleSystemRenderer m_renderDeath;
        private Entity m_playerEntity;
        int playerScore = 2;
        int bestPlayerPosition = int.MaxValue;
        private int killCount = 0;

        private Systems.Network m_systemNetwork = new Systems.Network();
        private Dictionary<uint, List<Entity>> m_perPlayerEntities = new Dictionary<uint, List<Entity>>();

        private Systems.KeyboardInput m_systemKeyboardInput;
       // private Systems.Interpolation m_systemInterpolation = new Systems.Interpolation();
        private Systems.Renderer m_systemRenderer = new Systems.Renderer();
        private bool loading = false;
        private KeyControlsSnake m_loadedState;

        private List<Tuple<string, int>> m_Scores = new List<Tuple<string, int>>();

        private const int GameWorldWidth = 9600;
        private const int GameWorldHeight = 5400;
        public bool isDead = false;

        private const int GameWorldViewPortWidth = 1920;
        private const int GameWorldViewPortHeight = 1080;
        private SoundEffect gameOver;
        private SoundEffect foodEaten;
        private SoundEffectInstance foodInstance;
        private AnimatedSprite bananaRenderer;
        private Texture2D mainPanel;


        public List<Tuple<string, int>> getScores()
        {
            return m_Scores;
        }
        public Dictionary<uint, string> getPlayerNames()
        {
            return playerNames;
        }
        /*public void resetGameModel()
        {
        m_entities = new Dictionary<uint, Entity>();
            m_systemInterpolation = new Systems.Interpolation();
            m_systemRenderer = new Systems.Renderer();

        }*/
    /// <summary>
    /// This is where everything performs its update.
    /// </summary>
    public void update(GameTime elapsedTime)
        {
            m_particleSystemEatFood.update(elapsedTime);
            m_particleSystemDeath.update(elapsedTime);
            m_systemNetwork.update(elapsedTime.ElapsedGameTime, MessageQueueClient.instance.getMessages());
            if (!isDead)
            {
                m_systemKeyboardInput.update(elapsedTime.ElapsedGameTime);

            }

            foreach (Entity entity in m_entities.Values)
            {
                if (entity.contains<Shared.Components.TurnPoints>())
                {
                    var turnPoints = entity.get<Shared.Components.TurnPoints>().turnPoints;
                    var position = entity.get<Shared.Components.Position>();

                    if (turnPoints.Count > 0)
                    {
                        var top = turnPoints.Peek();

                        double x = Math.Cos(position.orientation);
                        double y = Math.Sin(position.orientation);


                        Vector2 tempVector = new Vector2((float)x, (float)y);
                        /* tempVector.X = (int)tempVector.X;
                         tempVector.Y = (int)tempVector.Y;*/

                        // If we are within 3 points of the turnpoint, then turn

                        float distanceToTurnPoint = Vector2.Distance(position.position, top.Item1);
                        float secondDistanceToTurnPoint = Vector2.Distance(position.position + tempVector, top.Item1);
                        if (tempVector.Y < 0 && tempVector.Y > -0.001)
                        {
                            tempVector.Y = 0;
                        }
                        if (tempVector.X < 0 && tempVector.X > -0.001)
                        {
                            tempVector.X = 0;
                        }

                        if (distanceToTurnPoint < 3)

                        {
                            Tuple<Vector2, float> turnPoint = turnPoints.Dequeue();
                            Vector2 newOrientation = new Vector2((float)Math.Cos(turnPoint.Item2), (float)Math.Sin(turnPoint.Item2));
                            if (newOrientation.Y < 0 && newOrientation.Y > -0.001)
                            {
                                newOrientation.Y = 0;
                            }
                            if (newOrientation.X < 0 && newOrientation.X > -0.001)
                            {
                                newOrientation.X = 0;
                            }
                            /*newOrientation.X = (int)newOrientation.X;
                            newOrientation.Y = (int)newOrientation.Y;*/

                            Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;
                            if (tempVector.X == 0 && tempVector.Y > 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * (Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * (Math.Sign(newOrientation.X)));
                            }
                            if (tempVector.X == 0 && tempVector.Y < 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * -(Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * -(Math.Sign(newOrientation.X)));
                            }
                            else if (tempVector.X > 0 && tempVector.Y == 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).X / Math.Sqrt(2) * (Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).X / Math.Sqrt(2) * (Math.Sign(newOrientation.X)));
                            }
                            else if (tempVector.X < 0 && tempVector.Y == 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).X / Math.Sqrt(2) * -(Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).X / Math.Sqrt(2) * -(Math.Sign(newOrientation.X)));
                            }
                            else if (Math.Abs(tempVector.X) > 0 && Math.Abs(tempVector.Y) > 0 && Math.Abs(newOrientation.X) == 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).Y * Math.Sqrt(2));

                            }
                            else if (Math.Abs(tempVector.X) > 0 && Math.Abs(tempVector.Y) > 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) == 0)
                            {
                                difference.X = (float)((position.position - top.Item1).X * Math.Sqrt(2));
                            }

                            // Angle to angle
                            else if (Math.Abs(tempVector.X) > 0 && Math.Abs(tempVector.Y) > 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.X = (position.position - top.Item1).X * (Math.Sign(newOrientation.X));
                                difference.Y = (position.position - top.Item1).Y * (Math.Sign(newOrientation.Y));
                            }


                            else if (difference.X == 0 && difference.Y == 0)
                            {
                                difference = new Vector2((position.position.Y - top.Item1.Y), (position.position.X - top.Item1.X)) * new Vector2(tempVector.Y, tempVector.X) * newOrientation;
                            }

                            position.orientation = turnPoint.Item2;
                            position.position = top.Item1 + difference;





                            // Lets say x = -0.5 and y = 0.5

                            // Going from x = 15 to 14.5, y = 15 to 15.5

                        }

                        // Make sure we did not pass the position

                        else if (secondDistanceToTurnPoint > distanceToTurnPoint)
                        {

                            Tuple<Vector2, float> turnPoint = turnPoints.Dequeue();
                            Vector2 newOrientation = new Vector2((float)Math.Cos(turnPoint.Item2), (float)Math.Sin(turnPoint.Item2));
                            if (newOrientation.Y < 0 && newOrientation.Y > -0.001)
                            {
                                newOrientation.Y = 0;
                            }
                            if (newOrientation.X < 0 && newOrientation.X > -0.001)
                            {
                                newOrientation.X = 0;
                            }
                            /*newOrientation.X = (int)newOrientation.X;
                            newOrientation.Y = (int)newOrientation.Y;*/

                            Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;
                            if (tempVector.X == 0 && tempVector.Y > 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * (Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * (Math.Sign(newOrientation.X)));
                            }
                            if (tempVector.X == 0 && tempVector.Y < 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * -(Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).Y / Math.Sqrt(2) * -(Math.Sign(newOrientation.X)));
                            }
                            else if (tempVector.X > 0 && tempVector.Y == 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).X / Math.Sqrt(2) * (Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).X / Math.Sqrt(2) * (Math.Sign(newOrientation.X)));
                            }
                            else if (tempVector.X < 0 && tempVector.Y == 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).X / Math.Sqrt(2) * -(Math.Sign(newOrientation.Y)));
                                difference.X = (float)((position.position - top.Item1).X / Math.Sqrt(2) * -(Math.Sign(newOrientation.X)));
                            }
                            else if (Math.Abs(tempVector.X) > 0 && Math.Abs(tempVector.Y) > 0 && Math.Abs(newOrientation.X) == 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.Y = (float)((position.position - top.Item1).Y * Math.Sqrt(2));

                            }
                            else if (Math.Abs(tempVector.X) > 0 && Math.Abs(tempVector.Y) > 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) == 0)
                            {
                                difference.X = (float)((position.position - top.Item1).X * Math.Sqrt(2));
                            }

                            // Angle to angle
                            else if (Math.Abs(tempVector.X) > 0 && Math.Abs(tempVector.Y) > 0 && Math.Abs(newOrientation.X) > 0 && Math.Abs(newOrientation.Y) > 0)
                            {
                                difference.X = (position.position - top.Item1).X * (Math.Sign(newOrientation.X));
                                difference.Y = (position.position - top.Item1).Y * (Math.Sign(newOrientation.Y));
                            }


                            else if (difference.X == 0 && difference.Y == 0)
                            {
                                difference = new Vector2((position.position.Y - top.Item1.Y), (position.position.X - top.Item1.X)) * new Vector2(tempVector.Y, tempVector.X) * newOrientation;
                            }

                            position.orientation = turnPoint.Item2;
                            position.position = top.Item1 + difference;

                        }

                    }




                }
            }
            foreach (List<Entity> entities in m_perPlayerEntities.Values)
            {

                foreach (Entity entity in entities)
                {
                    Shared.Entities.Utility.thrust(entity, elapsedTime.ElapsedGameTime);

                }

            }






        }

        public void render(TimeSpan elapsedTime, SpriteBatch spriteBatch, int gameWidth, int gameHeight, Texture2D backgroundImage, Texture2D wallImage, AnimatedSprite animatedRender, SpriteFont font)
        {
            m_systemRenderer.update(elapsedTime, spriteBatch, gameWidth, gameHeight, backgroundImage, wallImage, animatedRender, font, m_perPlayerEntities, m_Scores, playerNames );
            m_renderDeath.draw(spriteBatch, m_particleSystemDeath);
            m_renderFoodEaten.draw(spriteBatch, m_particleSystemEatFood);
            if (isDead)
            {
                // Render the death screen!
                gameOverRender(spriteBatch, gameWidth, gameHeight, font);
            }

        }

        public void gameOverRender(SpriteBatch spriteBatch, int gameWidth, int gameHeight, SpriteFont font)
        {
            // Get the total kills, total score, and highest position acheived!
            //string infoText = $"Total Kills: {totalKills}\nTotal Score: {totalScore}\nHigh Score: {highScore}";
            spriteBatch.Begin();

            string infoText = $"Total Kills: {killCount} \n\nTotal Score: {playerScore} \n\nHighest Position: {bestPlayerPosition} \n\nPress Enter To Exit";

            Vector2 textSize = font.MeasureString(infoText);
            Rectangle boxRect = new Rectangle(gameWidth/4, gameHeight / 4 , gameWidth / 2, gameHeight / 2);

            // Draw the box
            spriteBatch.Draw(mainPanel,boxRect, Color.Black);

            // Draw the text inside the box
            spriteBatch.DrawString(font, infoText, new Vector2(boxRect.X + 50, boxRect.Y + 50), Color.White);

            // Draw some more info, such as how to return to the menu

            spriteBatch.End();

        }

        /// <summary>
        /// This is where all game model initialization occurs.  In the case
        /// of this "game', start by initializing the systems and then
        /// loading the art assets.
        /// </summary>
        public bool initialize(ContentManager contentManager)
        {
            m_contentManager = contentManager;
            gameOver = contentManager.Load<SoundEffect>("game-over-arcade-6435");
            foodEaten = contentManager.Load<SoundEffect>("eating-sound-effect-36186");
            foodInstance = foodEaten.CreateInstance();
            m_systemNetwork.registerNewEntityHandler(handleNewEntity);
            m_systemNetwork.registerRemoveEntityHandler(handleRemoveEntity);
            m_systemNetwork.registerTurnPointMessage(handleTurnPointMessage);
            m_systemNetwork.registerScoresUpdateHandler(handleScoresMessage);
            m_systemNetwork.registerPlayerDeathHandler(handlePlayerDeath);
            m_systemNetwork.registerHandleFoodEatenHandler(handleFoodEaten);
            m_systemNetwork.registerKillCountHandler(handleKillCount);
            mainPanel = contentManager.Load<Texture2D>("panel");
            m_particleSystemEatFood = new ParticleSystem(
                (int)(1000 / 1920f * 20), (int)(1000 / 1920f * 4),
                (1000 / 1920f * 0.12f), (1000 / 1920f * 0.03f),
                650, 100);
            m_renderFoodEaten = new ParticleSystemRenderer("fire");
            m_particleSystemDeath = new ParticleSystem(
                (int)(1000 / 1920f * 20), (int)(1000 / 1920f * 4),
                (1000 / 1920f * 0.12f), (1000 / 1920f * 0.03f),
                650, 100);
            m_renderDeath = new ParticleSystemRenderer("smoke-2");

            m_renderDeath.LoadContent(contentManager);
            m_renderFoodEaten.LoadContent(contentManager);

            loadKeyControls();
            // Modify this to load in controls
            m_systemKeyboardInput = new Systems.KeyboardInput(new List<Tuple<Shared.Components.Input.Type, Keys>>
            {
                Tuple.Create(Shared.Components.Input.Type.RotateUp, m_loadedState.Up),
                Tuple.Create(Shared.Components.Input.Type.RotateLeft, m_loadedState.Left),
                Tuple.Create(Shared.Components.Input.Type.RotateRight, m_loadedState.Right),
                Tuple.Create(Shared.Components.Input.Type.RotateDown, m_loadedState.Down),

            });

            return true;
        }
        public void handleKillCount(KillCount message)
        {
            if (message != null)
            {
                killCount = message.killCount;
            }
        }
            public void handleFoodEaten(FoodEaten message)
        {

            // Draw the particle effect for eating food
            // For now, just do player perspective
            m_particleSystemEatFood.foodEaten(new Vector2(500,500));
            // Play the sound!
            foodEaten.Play();


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
                        if (storage.FileExists("KeyControlsSnake.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("KeyControlsSnake.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(KeyControlsSnake));
                                    m_loadedState = (KeyControlsSnake)mySerializer.ReadObject(fs);
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
        public void shutdown()
        {

        }

        public void signalKeyPressed(Keys key)
        {
            m_systemKeyboardInput.keyPressed(key);
        }

        public void signalKeyReleased(Keys key)
        {
            m_systemKeyboardInput.keyReleased(key);
        }

        /// <summary>
        /// Based upon an Entity received from the server, create the
        /// entity at the client.
        /// </summary>
        private Entity createEntity(Shared.Messages.NewEntity message)
        {
            Entity entity = new Entity(message.id);

            if (message.hasName)
            {
                entity.add(new Shared.Components.Name(message.name));
            }

            if (message.hasAppearance)
            {
                Texture2D texture = m_contentManager.Load<Texture2D>(message.texture);
                entity.add(new Components.Sprite(texture));
            }

            if (message.hasPosition)
            {
                entity.add(new Shared.Components.Position(message.position, message.orientation));
            }

            if (message.hasSize)
            {
                entity.add(new Shared.Components.Size(message.size));
            }

            if (message.hasMovement)
            {
                entity.add(new Shared.Components.Movement(message.moveRate, message.rotateRate));
            }

            if (message.hasInput)
            {
                entity.add(new Shared.Components.Input(message.inputs));
            }
            if (message.isHead)
            {
                entity.add(new Shared.Components.Head());

            }

            if (message.isTail)
            {
                entity.add(new Shared.Components.Tail(message.headId));

            }
            if (message.isSegment)
            {
                entity.add(new Shared.Components.Segment(message.headId));

            }
            if (message.hasTurnPoints)
            {
                entity.add(new Shared.Components.TurnPoints(message.turnPoints));

            }
            return entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void handleScoresMessage(Shared.Messages.ScoresUpdate message)
        {

            m_Scores = new List<Tuple<string, int>>();

            foreach (KeyValuePair<string,int> newScore in message.scoresTable)
            {
                m_Scores.Add(new Tuple<string, int>(newScore.Key, newScore.Value));
            }

            m_Scores = m_Scores.OrderByDescending(tuple => tuple.Item2).ToList();
            int bestPosition = 1;
            foreach (Tuple<string, int> playerScores in m_Scores)
            {
                if (m_playerEntity != null)
                {
                    if (playerNames[m_playerEntity.id] == playerScores.Item1)
                    {

                        playerScore = playerScores.Item2;
                        if (bestPosition < bestPlayerPosition)
                        {
                            bestPlayerPosition = bestPosition;
                        }
                    }
                }
                bestPosition += 1;
            }


        }
        /// <summary>
        /// As entities are added to the game model, they are run by the systems
        /// to see if they are interested in knowing about them during their
        /// updates.
        /// </summary>
        private void addEntity(Entity entity)
        {
            if (entity == null)
            {
                return;
            }
            if (entity.contains<Shared.Components.Head>())
            {
                if (!m_perPlayerEntities.ContainsKey(entity.id))
                {
                    m_perPlayerEntities[entity.id] = new List<Entity> { entity };

                }
                else
                {
                    m_perPlayerEntities[entity.id].Add(entity);
                }
                playerNames[entity.id] = entity.get<Shared.Components.Name>().name;



            }
            else if (entity.contains<Shared.Components.Segment>())
            {
                if (!m_perPlayerEntities.ContainsKey(entity.get<Shared.Components.Segment>().headId))
                {
                    m_perPlayerEntities[entity.get<Shared.Components.Segment>().headId] = new List<Entity>();
                }
                m_perPlayerEntities[entity.get<Shared.Components.Segment>().headId].Add(entity);
                /*m_perPlayerEntities[entity.get<Shared.Components.Segment>().headId].Sort();*/

            }
            else if (entity.contains<Shared.Components.Tail>())
            {
                if (!m_perPlayerEntities.ContainsKey(entity.get<Shared.Components.Tail>().headId))
                {
                    m_perPlayerEntities[entity.get<Shared.Components.Tail>().headId] = new List<Entity>();
                }
                m_perPlayerEntities[entity.get<Shared.Components.Tail>().headId].Add(entity);
            }

            // Obtain our own player entity. This is for processing stuff based on us!
            if (entity.contains<Shared.Components.Input>()) 
            {
                if (m_playerEntity == null)
                {
                    m_playerEntity = entity;
                }
            }



            m_entities[entity.id] = entity;
            m_systemKeyboardInput.add(entity);
            m_systemRenderer.add(entity);
            m_systemNetwork.add(entity);
            //m_systemInterpolation.add(entity);
        }

        /// <summary>
        /// All entity lists for the systems must be given a chance to remove
        /// the entity.
        /// </summary>
        private void removeEntity(uint id)
        {

            // Get the position of the given piece of food, if it is food.

            m_entities.Remove(id);

            m_systemKeyboardInput.remove(id);
            m_systemNetwork.remove(id);
            m_systemRenderer.remove(id);
            // NOTE: We also remove food. This might be an issue later with this, but it is working for me...
            // Keep in mind the server only keeps track of the head that collides, so we can just receive the head and remove the rest of the body
            m_perPlayerEntities.Remove(id);

            
            //m_systemInterpolation.remove(id);
        }

        private void handleNewEntity(Shared.Messages.NewEntity message)
        {
            Entity entity = createEntity(message);
            addEntity(entity);
        }

        /// <summary>
        /// Handler for the RemoveEntity message.  It removes the entity from
        /// the client game model (that's us!).
        /// </summary>
        private void handleRemoveEntity(Shared.Messages.RemoveEntity message)
        {
            removeEntity(message.id);
        }


        private void handleTurnPointMessage(Shared.Messages.TurnPoint message)
        {
            try 
            {
                List<Entity> givenSegments = m_perPlayerEntities[message.headId];
                foreach (Entity entity in givenSegments)
                {
                    if (entity.contains<Shared.Components.TurnPoints>())
                    {
                            entity.get<Shared.Components.TurnPoints>().turnPoints.Enqueue(message.turnPoint);

                        
                    }
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception();
            }
        }
        private void handlePlayerDeath(PlayerDeath message)
        {
            // Get the particle effects ready

            // We need to do the conversion from here to be able to get it in the viewport

            // Render regardless if its in the area!
            float ScreenX = m_playerEntity.get<Shared.Components.Position>().position.X - 500;
            float ScreenY = m_playerEntity.get<Shared.Components.Position>().position.Y - 500;
            Rectangle viewPort = new Rectangle((int)(ScreenX), (int)ScreenY, 1000, 1000);
            //(int)(position.X - ScreenX), (int)(position.Y - ScreenY), (int)size.X, (int)size.Y





            List<Vector2> centers = new List<Vector2>();
            foreach (Entity player in m_perPlayerEntities[message.id])
            {
                var position = player.get<Shared.Components.Position>().position;
                var size = player.get<Shared.Components.Size>().size;
                centers.Add(new Vector2((position.X - ScreenX), (position.Y - ScreenY)));
            }
            m_particleSystemDeath.playerDeath(centers);
            removeEntity(message.id);

            if (message.id == m_playerEntity.id)
            {
                gameOver.Play();

                isDead = true;
            }

           

        }

    }
}
