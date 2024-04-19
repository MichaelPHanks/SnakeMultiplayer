﻿
using Microsoft.Xna.Framework;
using Shared.Components;
using Shared.Entities;
using Shared.Messages;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Server
{
    public class GameModel
    {
        private HashSet<int> m_clients = new HashSet<int>();
        private Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();
        private Dictionary<int, uint> m_clientToEntityId = new Dictionary<int, uint>();
        private Dictionary<uint, List<Entity>> m_perPlayerEntities = new Dictionary<uint, List<Entity>>();
        Systems.Network m_systemNetwork = new Server.Systems.Network();
        private const int GameWorldWidth = 5000;
        private const int GameWorldHeight = 5000;
        TimeSpan updateClientClock = TimeSpan.FromSeconds(0.5);

        private const int GameWorldViewPortWidth = 1920;
        private const int GameWorldViewPortHeight = 1080;
        //private List<Tuple<string, int>> m_scores = new List<Tuple<string, int>>();

        private Dictionary<string, int> m_scores = new Dictionary<string, int>();


        private List<int> foodCount = new List<int>();
        private Dictionary<uint, Entity> foodEntities = new Dictionary<uint, Entity>(); 



        /// <summary>
        /// This is where the server-side simulation takes place.  Messages
        /// from the network are processed and then any necessary client
        /// updates are sent out.
        /// </summary>
        public void update(TimeSpan elapsedTime)
        {

            
            m_systemNetwork.update(elapsedTime, MessageQueueServer.instance.getMessages());

            outOfBounds();

            collisionDetection();

            foodEat();

            foodUpdate();
           


            snakeSimulator(elapsedTime);


            // Update clients on head and segments and tail

            updateClientClock -= elapsedTime;
            if (updateClientClock.TotalMilliseconds < 0 )
            {
                updateClientClock = TimeSpan.FromSeconds(0.5);

                foreach (List<Entity> entities in m_perPlayerEntities.Values)
                {
                    foreach (Entity entity in entities)
                    {
                        Message updateEntity = new Shared.Messages.UpdateEntity(entity, elapsedTime);
                        MessageQueueServer.instance.broadcastMessage(updateEntity);

                    }
                }
            }




        }

        private void collisionDetection()
        {
            List<Entity> entitiesToRemove = new List<Entity>();

            // Check every single head entity to see if it collides with 
            foreach (Entity entity in m_entities.Values)
            {
                if (entity.contains<Shared.Components.Head>())
                {
                    foreach (Entity entity1 in m_entities.Values)
                    {
                        if (entity1.contains<Shared.Components.Segment>() || entity1.contains<Shared.Components.Tail>())
                        {
                            if (entity1.get<Shared.Components.Segment>().headId != entity.id)
                            {
                                // Check if they collide

                                Rectangle headRectangle = new Rectangle((int)entity.get<Shared.Components.Position>().position.X, (int)entity.get<Shared.Components.Position>().position.Y, (int)entity.get<Shared.Components.Size>().size.X, (int)entity.get<Shared.Components.Size>().size.Y);
                                Rectangle otherRectangle = new Rectangle((int)entity1.get<Shared.Components.Position>().position.X, (int)entity1.get<Shared.Components.Position>().position.Y, (int)entity1.get<Shared.Components.Size>().size.X, (int)entity1.get<Shared.Components.Size>().size.Y);

                                if (otherRectangle.Intersects(headRectangle))
                                {
                                    // Kill the snake

                                    if (!entitiesToRemove.Contains(entity))
                                    {
                                        entitiesToRemove.Add(entity);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (Entity entity in entitiesToRemove)
            {
                handlePlayerDeath((int)entity.id);
            }
        }

        private void snakeSimulator(TimeSpan elapsedTime)
        {
            foreach (Entity entity in m_entities.Values)
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

                                


                                var turnPoint = turnPoints.Dequeue();
                                Vector2 newOrientation = new Vector2((float)Math.Cos(turnPoint.Item2), (float)Math.Sin(turnPoint.Item2));

                                Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;
                                if (difference.Y > 0.001 && turnPoint.Item2 != position.orientation)
                                {
                                    Console.WriteLine();
                                }

                                position.orientation = turnPoint.Item2;

                                position.position = top.Item1 + difference;


                            }
                        }

                        else if (x >= 0 && y <= 0)
                        {
                            if (position.position.X >= top.Item1.X && position.position.Y <= top.Item1.Y)
                            {
                                var turnPoint = turnPoints.Dequeue();
                                Vector2 newOrientation = new Vector2((float)Math.Cos(turnPoint.Item2), (float)Math.Sin(turnPoint.Item2));

                                Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;


                                position.orientation = turnPoint.Item2;

                                position.position = top.Item1 + difference;

                            }
                        }
                        else if (y >= 0 && x <= 0)
                        {
                            if (position.position.X <= top.Item1.X && position.position.Y >= top.Item1.Y)
                            {
                                var turnPoint = turnPoints.Dequeue();
                                Vector2 newOrientation = new Vector2((float)Math.Cos(turnPoint.Item2), (float)Math.Sin(turnPoint.Item2));

                                Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;


                                position.orientation = turnPoint.Item2;

                                position.position = top.Item1 + difference;

                            }
                        }
                        else
                        {
                            if (position.position.X >= top.Item1.X && position.position.Y >= top.Item1.Y)
                            {
                                var turnPoint = turnPoints.Dequeue();
                                Vector2 newOrientation = new Vector2((float)Math.Cos(turnPoint.Item2), (float)Math.Sin(turnPoint.Item2));

                                Vector2 difference = (position.position - top.Item1) * tempVector * newOrientation;


                                position.orientation = turnPoint.Item2;

                                position.position = top.Item1 + difference;

                            }
                        }
                        // Lets say x = -0.5 and y = 0.5

                        // Going from x = 15 to 14.5, y = 15 to 15.5


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

        private void foodEat()
        {
            Dictionary<uint, Entity> foodToRemove = new Dictionary<uint, Entity>();
            Dictionary<uint, Entity> entityToAdd = new Dictionary<uint, Entity>();
            List<uint> clientsNewSegments = new List<uint>();

            foreach (Entity entity in foodEntities.Values)
            {
                var position = entity.get<Shared.Components.Position>().position;
                var size = entity.get<Shared.Components.Size>().size;

                var foodRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
                foreach (Entity playerEntity in m_entities.Values)
                {
                    if (playerEntity.contains<Shared.Components.Head>())
                    {
                        var playerPosition = playerEntity.get<Shared.Components.Position>().position;
                        var playerSize = playerEntity.get<Shared.Components.Size>().size;

                        var playerRectangle = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, (int)playerSize.X, (int)playerSize.Y);

                        if (foodRectangle.Intersects(playerRectangle))
                        {
                            // Send a message indicating a change in score from server to player.
                            m_scores[playerEntity.get<Shared.Components.Name>().name] += 1;

                            Message scoreMessage = new Shared.Messages.ScoresUpdate(m_scores);

                            MessageQueueServer.instance.broadcastMessage(scoreMessage);
                            // Food disappears
                            foodToRemove.Add(entity.id, entity);
                            float offsetDistance = 1.0f; // Adjust this value to control the distance behind the player
                            float offsetX = (float)(offsetDistance * Math.Cos(playerEntity.get<Shared.Components.Position>().orientation));
                            float offsetY = (float)(offsetDistance * Math.Sin(playerEntity.get<Shared.Components.Position>().orientation));

                            // Create the position for the segment
                            Vector2 segmentPosition = new Vector2(playerPosition.X - offsetX, playerPosition.Y - offsetY);
                            // Add new segment, properly place it.


                            Vector2 prevPosition = m_perPlayerEntities[playerEntity.id].Last().get<Shared.Components.Position>().position;

                            float lastOrientation = m_perPlayerEntities[playerEntity.id].Last().get<Shared.Components.Position>().orientation;
                            float x = (float)Math.Cos(lastOrientation);
                            float y = (float)Math.Sin(lastOrientation);
                            Vector2 direction = new Vector2();

                            if (x < 0 && (int)y == 0)
                            {
                                direction.X = 25;
                            }
                            else if (x > 0 && (int)y == 0)
                            {
                                direction.X = -25;

                            }
                            else if (y < 0 && (int)x == 0)
                            {
                                direction.Y = 25;

                            }
                            else
                            {
                                direction.Y = -25;

                            }
                            Queue<Tuple<Vector2, float>> turnPointsTemp = new Queue<Tuple<Vector2, float>>(m_perPlayerEntities[playerEntity.id].Last().get<Shared.Components.TurnPoints>().turnPoints);

                            Entity newSegment = Shared.Entities.Segment.create("PlayerBody", prevPosition + direction, playerSize.Y, 0.3f, 1, turnPointsTemp, lastOrientation, playerEntity.id);
                            entityToAdd.Add(newSegment.id, newSegment);

                            clientsNewSegments.Add(playerEntity.id);
                            m_perPlayerEntities[playerEntity.id].Add(newSegment);

                            // How will we know which entity we are dealing with and what their tail is? Or even their segments?
                            //Entity yeah = Shared.Entities.Segment.create()
                            /*playerSize.X += 1;
                            playerSize.Y += 1;
                            Message message = new Shared.Messages.UpdateEntity();
                            MessageQueueServer.instance.broadcastMessage(message);*/



                        }
                    }
                }



            }

            foreach (Entity entity in foodToRemove.Values)
            {
                Message message = new Shared.Messages.RemoveEntity(entity.id);
                MessageQueueServer.instance.broadcastMessage(message);
            }
            foreach (Entity food in foodToRemove.Values)
            {
                foodEntities.Remove(food.id);
                m_entities.Remove(food.id);
                m_systemNetwork.remove(food.id);
            }

            int clientCounter = 0;
            foreach (Entity entity1 in entityToAdd.Values)
            {
                addEntity(entity1);

                uint id = clientsNewSegments[clientCounter];
                clientCounter += 1;
                // We need player id here...
                var keys = m_clientToEntityId.Where(pair => pair.Value == id)
                             .Select(pair => pair.Key)
                             .ToList();


                Message message = new Shared.Messages.NewEntity(entity1);

                MessageQueueServer.instance.sendMessage(keys[0], message);


                message = new Shared.Messages.NewEntity(entity1);

                entity1.remove<Appearance>();
                entity1.add(new Appearance("EnemyBody"));
                message = new Shared.Messages.NewEntity(entity1);

                foreach (int otherId in m_clients)
                {
                    if (otherId != keys[0])
                    {
                        MessageQueueServer.instance.sendMessage(otherId, message);
                    }
                }

            }
        }

        private void outOfBounds()
        {
            // Check for collision:
            List<Entity> entitiesToRemove = new List<Entity>();
            // Check for out of bounds death
            foreach (var entity in m_entities.Values)
            {
                if (entity.contains<Shared.Components.Head>())
                {


                    var position1 = entity.get<Shared.Components.Position>().position;
                    if (position1.X < 0 || position1.X > GameWorldWidth)
                    {
                        entitiesToRemove.Add(entity);
                    }
                    else if (position1.Y < 0 || position1.Y > GameWorldHeight)
                    {
                        entitiesToRemove.Add(entity);



                    }
                }

            }
            

            // Handle removing the objects.
            foreach (Entity entity in entitiesToRemove)
            {
                handlePlayerDeath((int)entity.id);
            }
        }

        private void foodUpdate()
        {
            // Food Count Checker
            if (foodCount.Count < 100)
            {
                Random rand = new Random();

                for (int i = foodCount.Count; i < 100; i++)
                {
                    int randomPositionX = rand.Next(0, 5001);
                    int randomPositionY = rand.Next(0, 5001);

                    Entity newFood = Shared.Entities.Food.create("cake", new System.Numerics.Vector2(randomPositionX, randomPositionY), 25);
                    addEntity(newFood);
                    Message message = new NewEntity(newFood);
                    foreach (int otherId in m_clients)
                    {

                        MessageQueueServer.instance.sendMessage(otherId, message);

                    }
                    foodCount.Add(1);
                    foodEntities.Add(newFood.id, newFood);
                }
            }
        }

        /// <summary>
        /// Setup notifications for when new clients connect.
        /// </summary>
        public bool initialize()
        {
            m_systemNetwork.registerHandler(Shared.Messages.Type.Join,handleJoin);
            m_systemNetwork.registerDisconnectHandler(handleDisconnect);

            MessageQueueServer.instance.registerConnectHandler(handleConnect);

            return true;
        }

        /// <summary>
        /// Give everything a chance to gracefully shutdown.
        /// </summary>
        public void shutdown()
        {

        }

        /// <summary>
        /// Upon connection of a new client, create a player entity and
        /// send that info back to the client, along with adding it to
        /// the server simulation.
        /// </summary>
        private void handleConnect(int clientId)
        {
            m_clients.Add(clientId);

            MessageQueueServer.instance.sendMessage(clientId, new Shared.Messages.ConnectAck());
        }



        private void handlePlayerDeath(int clientId)
        {

            // Before removing, place food based on where all of the segments and stuff are.

            foreach (Entity entity in m_perPlayerEntities[(uint)clientId]) 
            {
                Entity newFood = Shared.Entities.Food.create("cake", entity.get<Shared.Components.Position>().position, 25);
                addEntity(newFood);
                foodCount.Add(1);
                foodEntities.Add(newFood.id, newFood);
                Message newFoodMessage = new Shared.Messages.NewEntity(newFood);
                MessageQueueServer.instance.broadcastMessage(newFoodMessage);
            }
            m_entities.Remove((uint)clientId);


            foreach (Entity entity in m_perPlayerEntities[(uint)clientId])
            {
                m_entities.Remove(entity.id);

            }
            m_perPlayerEntities.Remove((uint)clientId);
            m_systemNetwork.remove((uint)clientId);

            Message message = new Shared.Messages.PlayerDeath((uint)clientId);
            MessageQueueServer.instance.broadcastMessage(message);

            

        }

       
        /// <summary>
        /// When a client disconnects, need to tell all the other clients
        /// of the disconnect.
        /// </summary>
        /// <param name="clientId"></param>
        private void handleDisconnect(int clientId)
        {
            m_clients.Remove(clientId);






            Message message = new Shared.Messages.RemoveEntity(m_clientToEntityId[clientId]);
            MessageQueueServer.instance.broadcastMessage(message);



            removeEntity(m_clientToEntityId[clientId]);
            // Remove each of the segments as well.


            if (m_perPlayerEntities.ContainsKey(m_clientToEntityId[clientId]))
            {
                List<Entity> entitiesToRemove = m_perPlayerEntities[m_clientToEntityId[clientId]];


                foreach (Entity entityToRemove in entitiesToRemove)
                {
                    m_entities.Remove(entityToRemove.id);
                }
                m_perPlayerEntities.Remove(m_clientToEntityId[clientId]);

            }




            m_clientToEntityId.Remove(clientId);
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

            if (entity.contains<Shared.Components.Input>())
            {
                // Add to the player
                m_perPlayerEntities[entity.id] = new List<Entity> { entity };
            }

            m_entities[entity.id] = entity;
            m_systemNetwork.add(entity);
        }

        /// <summary>
        /// All entity lists for the systems must be given a chance to remove
        /// the entity.
        /// </summary>
        private void removeEntity(uint id)
        {
            //m_perPlayerEntities.Remove(id);
            m_entities.Remove(id);
            m_systemNetwork.remove(id);
        }

        /// <summary>
        /// For the indicated client, sends messages for all other entities
        /// currently in the game simulation.
        /// </summary>
        private void reportAllEntities(int clientId)
        {
            foreach (var item in m_entities)
            {
                MessageQueueServer.instance.sendMessage(clientId, new Shared.Messages.NewEntity(item.Value));
            }
        }

        /// <summary>
        /// Handler for the Join message.  It gets a player entity created,
        /// added to the server game model, and notifies the requesting client
        /// of the player.
        /// </summary>
        private void handleJoin(int clientId, TimeSpan elapsedTime, Shared.Messages.Message messageJoin)
        {
            Shared.Messages.Join messageNew = (Shared.Messages.Join) messageJoin;


            if (m_scores.ContainsKey(messageNew.name))
            {
                int temp = 0;
                while (true)
                {
                    if (!m_scores.ContainsKey(messageNew.name + "(" + temp + ")"))
                    {
                        messageNew.name = messageNew.name + "(" + temp + ")";
                        break;
                    }
                    temp += 1;
                }
            }

            m_scores.Add(messageNew.name, 2);
            Message scoreMessage = new Shared.Messages.ScoresUpdate(m_scores);

            MessageQueueServer.instance.broadcastMessage(scoreMessage);

            // Step 1: Tell the newly connected player about all other entities
            reportAllEntities(clientId);

            // Step 2: Create an entity for the newly joined player and sent it
            //         to the newly joined client
            Entity player = Shared.Entities.Head.create("PlayerHead",messageNew.name, new System.Numerics.Vector2(GameWorldWidth / 2, GameWorldHeight / 2), 50, 0.3f, (float)Math.PI / 1000);
            addEntity(player);
            m_clientToEntityId[clientId] = player.id;
            MessageQueueServer.instance.sendMessage(clientId, new NewEntity(player));

            // New Step: Make a few different snake segments.
            


            // Step 4: Let all other clients know about this new player entity

            // We change the appearance for a player ship entity for all other clients to a different texture
            player.remove<Appearance>();
            player.add(new Appearance("EnemyHead"));

            // Remove components not needed for "other" players
            player.remove<Shared.Components.Input>();

            Message message = new NewEntity(player);
            foreach (int otherId in m_clients)
            {
                if (otherId != clientId)
                {
                    MessageQueueServer.instance.sendMessage(otherId, message);
                }
            }


            Vector2 position = new Vector2(GameWorldWidth / 2 - 25, GameWorldWidth / 2);
            for (int i = 0; i < 2; i++)
            {

                Entity newSegment = Shared.Entities.Segment.create("PlayerBody", position, 50, 0.3f, 1, new Queue<Tuple<Vector2, float>> { }, player.get<Shared.Components.Position>().orientation, player.id);
                addEntity(newSegment);
                m_perPlayerEntities[player.id].Add(newSegment);

                position.X -= 25;
                MessageQueueServer.instance.sendMessage(clientId, new NewEntity(newSegment));






                newSegment.remove<Appearance>();
                newSegment.add(new Appearance("EnemyBody"));
                Message message2 = new NewEntity(newSegment);
                foreach (int otherId in m_clients)
                {
                    if (otherId != clientId)
                    {
                        MessageQueueServer.instance.sendMessage(otherId, message2);
                    }
                }
            }

        }
    }
}
