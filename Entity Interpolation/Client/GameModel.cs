using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared;
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

        private Entity m_playerEntity;

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
        private bool isDead = false;

        private const int GameWorldViewPortWidth = 1920;
        private const int GameWorldViewPortHeight = 1080;

        private AnimatedSprite bananaRenderer;


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
    public void update(TimeSpan elapsedTime)
        {
            
            m_systemNetwork.update(elapsedTime, MessageQueueClient.instance.getMessages());
            if (!isDead)
            {
                m_systemKeyboardInput.update(elapsedTime);

            }

            //m_systemInterpolation.update(elapsedTime);
           /* if (m_perPlayerEntities.Count > 0)
            {
                for (int i = 2; i < m_perPlayerEntities[m_playerEntity.id].Count; i++)
                {
                    if (m_perPlayerEntities[m_playerEntity.id][i].get<Shared.Components.TurnPoints>().turnPoints.Count < m_perPlayerEntities[m_playerEntity.id][i - 1].get<Shared.Components.TurnPoints>().turnPoints.Count)
                    {
                        Console.WriteLine();
                    }
                }
            }*/


            foreach (Entity entity in m_entities.Values)
            {
                if (entity.contains<Shared.Components.Segment>())
                {
                    var turnPoints = entity.get<Shared.Components.TurnPoints>().turnPoints;
                    var position = entity.get<Shared.Components.Position>();

                    if (turnPoints.Count > 0)
                    {
                        var top = turnPoints.Peek();

                        double x = Math.Cos(position.orientation);
                        double y = Math.Sin(position.orientation);


                        Vector2 tempVector = new Vector2((float)x, (float)y);
                        tempVector.X = (int)tempVector.X;
                        tempVector.Y = (int)tempVector.Y;

                        // If we are within 3 points of the turnpoint, then turn

                        float distanceToTurnPoint = Vector2.Distance(position.position, top.Item1);
                        float secondDistanceToTurnPoint = Vector2.Distance(position.position + tempVector, top.Item1);


                        if (distanceToTurnPoint < 3)

                        {
                            if (distanceToTurnPoint > 4)
                            {
                                Console.WriteLine();
                            }


                            Tuple<Vector2, float> turnPoint = turnPoints.Dequeue();
                            Vector2 newOrientation = new Vector2((float)Math.Cos(turnPoint.Item2), (float)Math.Sin(turnPoint.Item2));

                            newOrientation.X = (int)newOrientation.X;
                            newOrientation.Y = (int)newOrientation.Y;

                            Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;


                            if (difference.X == 0 && difference.Y == 0)
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

                            newOrientation.X = (int)newOrientation.X;
                            newOrientation.Y = (int)newOrientation.Y;

                            Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;


                            if (difference.X == 0 && difference.Y == 0)
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
                    Shared.Entities.Utility.thrust(entity, elapsedTime);

                }

            }


        }

        public void render(TimeSpan elapsedTime, SpriteBatch spriteBatch, int gameWidth, int gameHeight, Texture2D backgroundImage, Texture2D wallImage, AnimatedSprite animatedRender, SpriteFont font)
        {
            m_systemRenderer.update(elapsedTime, spriteBatch, gameWidth, gameHeight, backgroundImage, wallImage, animatedRender, font, m_perPlayerEntities, m_Scores, playerNames );
        }

        /// <summary>
        /// This is where all game model initialization occurs.  In the case
        /// of this "game', start by initializing the systems and then
        /// loading the art assets.
        /// </summary>
        public bool initialize(ContentManager contentManager)
        {
            m_contentManager = contentManager;

            m_systemNetwork.registerNewEntityHandler(handleNewEntity);
            m_systemNetwork.registerRemoveEntityHandler(handleRemoveEntity);
            m_systemNetwork.registerTurnPointMessage(handleTurnPointMessage);
            m_systemNetwork.registerScoresUpdateHandler(handleScoresMessage);
            m_systemNetwork.registerPlayerDeathHandler(handlePlayerDeath);
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
                entity.add(new Shared.Components.Tail());

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

            foreach (KeyValuePair<string,int> yeah in message.scoresTable)
            {
                m_Scores.Add(new Tuple<string, int>(yeah.Key, yeah.Value));
            }

            m_Scores.OrderBy(tuple => tuple.Item2).ToList();

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
            removeEntity(message.id);

            if (message.id == m_playerEntity.id)
            {
                isDead = true;
            }

        }

    }
}
