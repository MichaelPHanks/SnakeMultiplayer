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
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Client
{
    public class GameModel
    {
        private ContentManager m_contentManager;
        private Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();
        private Systems.Network m_systemNetwork = new Systems.Network();
        private Dictionary<uint, List<Entity>> m_perPlayerEntities = new Dictionary<uint, List<Entity>>();

        private Systems.KeyboardInput m_systemKeyboardInput;
        private Systems.Interpolation m_systemInterpolation = new Systems.Interpolation();
        private Systems.Renderer m_systemRenderer = new Systems.Renderer();
        private bool loading = false;
        private KeyControlsSnake m_loadedState;

        private const int GameWorldWidth = 9600;
        private const int GameWorldHeight = 5400;


        private const int GameWorldViewPortWidth = 1920;
        private const int GameWorldViewPortHeight = 1080;



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
            m_systemKeyboardInput.update(elapsedTime);
            
            m_systemInterpolation.update(elapsedTime);


            foreach (Entity entity in m_entities.Values)
            {
                if (entity.contains<Shared.Components.Movement>())
                {
                    Shared.Entities.Utility.thrust(entity, elapsedTime);

                }
            }



            foreach (List<Entity> entities in m_perPlayerEntities.Values)
            {
                foreach (Entity entity in entities)
                {
                    if (entity.contains<Shared.Components.Segment>())
                    {
                        var turnPoints = entity.get<Shared.Components.TurnPoints>().turnPoints;
                        var position = entity.get<Shared.Components.Position>();

                        if (turnPoints.Count > 0)
                        {
                            var top = turnPoints.Peek();

                            float x = (float)Math.Cos(position.orientation);
                            float y = (float)Math.Sin(position.orientation);


                            Vector2 tempVector = new Vector2(x, y);

                            if (x <= 0 && y <= 0)
                            {
                                if (position.position.X <= top.Item1.X && position.position.Y <= top.Item1.Y)
                                {
                                    Vector2 difference = position.position - top.Item1;
                                    var turnPoint = turnPoints.Dequeue();
                                    position.orientation = turnPoint.Item2;

                                    position.position = top.Item1;

                                }
                            }

                            else if (x >= 0 && y <= 0)
                            {
                                if (position.position.X >= top.Item1.X && position.position.Y <= top.Item1.Y)
                                {
                                    Vector2 difference = position.position - top.Item1;
                                    var turnPoint = turnPoints.Dequeue();
                                    position.orientation = turnPoint.Item2;
                                    position.position = top.Item1;
                                }
                            }
                            else if (y >= 0 && x <= 0)
                            {
                                if (position.position.X <= top.Item1.X && position.position.Y >= top.Item1.Y)
                                {
                                    Vector2 difference = position.position - top.Item1;
                                    var turnPoint = turnPoints.Dequeue();
                                    position.orientation = turnPoint.Item2;
                                    position.position = top.Item1;
                                }
                            }
                            else
                            {
                                if (position.position.X >= top.Item1.X && position.position.Y >= top.Item1.Y)
                                {
                                    Vector2 difference = position.position - top.Item1;
                                    var turnPoint = turnPoints.Dequeue();
                                    position.orientation = turnPoint.Item2;
                                    position.position = top.Item1;
                                }
                            }
                            // Lets say x = -0.5 and y = 0.5

                            // Going from x = 15 to 14.5, y = 15 to 15.5


                        }



                    }
                }
            }


        }

        public void render(TimeSpan elapsedTime, SpriteBatch spriteBatch, int gameWidth, int gameHeight, Texture2D backgroundImage, Texture2D wallImage)
        {
            m_systemRenderer.update(elapsedTime, spriteBatch, gameWidth, gameHeight, backgroundImage, wallImage);
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

                m_perPlayerEntities[entity.id] = new List<Entity> { entity };


            }
            else if (entity.contains<Shared.Components.Segment>())
            {
                m_perPlayerEntities[entity.get<Shared.Components.Segment>().headId].Add(entity);

            }



            m_entities[entity.id] = entity;
            m_systemKeyboardInput.add(entity);
            m_systemRenderer.add(entity);
            m_systemNetwork.add(entity);
            m_systemInterpolation.add(entity);
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
            m_systemInterpolation.remove(id);
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
            }
        }

    }
}
