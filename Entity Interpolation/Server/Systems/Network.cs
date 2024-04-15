using Shared.Entities;
using Shared.Messages;
using Microsoft.Xna.Framework;
using Shared.Components;

namespace Server.Systems
{
    public class Network : Shared.Systems.System
    {
        public delegate void Handler(int clientId, TimeSpan elapsedTime, Shared.Messages.Message message);
        public delegate void JoinHandler(int clientId);
        public delegate void DisconnectHandler(int clientId);
        public delegate void InputHandler(Entity entity, Shared.Components.Input.Type type, TimeSpan elapsedTime);
        private Dictionary<Shared.Messages.Type, Handler> m_commandMap = new Dictionary<Shared.Messages.Type, Handler>();
        private JoinHandler m_joinHandler;
        private DisconnectHandler m_disconnectHandler;

        private HashSet<uint> m_reportThese = new HashSet<uint>();

        /// <summary>
        /// Primary activity in the constructor is to setup the command map
        /// that maps from message types to their handlers.
        /// </summary>
        public Network() :
            base(
                typeof(Shared.Components.Movement),
                typeof(Shared.Components.Position)
            )
        {
            // Register our own join handler
            registerHandler(Shared.Messages.Type.Join, (int clientId, TimeSpan elapsedTime, Shared.Messages.Message message) =>
            {
                if (m_joinHandler != null)
                {
                    m_joinHandler(clientId);
                }
            });

            // Register our own disconnect handler
            registerHandler(Shared.Messages.Type.Disconnect, (int clientId, TimeSpan elapsedTime, Shared.Messages.Message message) =>
            {
                if (m_disconnectHandler != null)
                {
                    m_disconnectHandler(clientId);
                }
            });

            // Register our own input handler
            registerHandler(Shared.Messages.Type.Input, (int clientId, TimeSpan elapsedTime, Shared.Messages.Message message) =>
            {
                handleInput((Shared.Messages.Input)message);
            });
        }

        // Have to implement this because it is abstract in the base class
        public override void update(TimeSpan elapsedTime) { }

        /// <summary>
        /// Have our own version of update, because we need a list of messages to work with, and
        /// messages aren't entities.
        /// </summary>
        public void update(TimeSpan elapsedTime, Queue<Tuple<int, Message>> messages)
        {
            

            if (messages != null)
            {
                while (messages.Count > 0)
                {
                    var message = messages.Dequeue();
                    if (m_commandMap.ContainsKey(message.Item2.type))
                    {
                        m_commandMap[message.Item2.type](message.Item1, elapsedTime, message.Item2);
                    }
                }
            }

            

            // Send updated game state updates back out to connected clients
            updateClients(elapsedTime);
        }

        public void registerJoinHandler(JoinHandler handler)
        {
            m_joinHandler = handler;
        }

        public void registerDisconnectHandler(DisconnectHandler handler)
        {
            m_disconnectHandler = handler;
        }

        private void registerHandler(Shared.Messages.Type type, Handler handler)
        {
            m_commandMap[type] = handler;
        }

        /// <summary>
        /// Handler for the Input message.  This simply passes the responsibility
        /// to the registered input handler.
        /// </summary>
        /// <param name="message"></param>
        private void handleInput(Shared.Messages.Input message)
        {
            var entity = m_entities[message.entityId];



            foreach (var input in message.inputs)
            {
                switch (input)
                {
                    case Shared.Components.Input.Type.RotateUp:




                        if (Shared.Entities.Utility.rotateUp(entity, message.elapsedTime))
                        {
                            bool canSend = false;
                            Vector2 position1 = new Vector2();
                            foreach (Entity entity1 in m_entities.Values)
                            {
                                if (entity1.contains<Shared.Components.Segment>())
                                {
                                    var headId = entity1.get<Shared.Components.Segment>().headId;
                                    position1 = entity.get<Shared.Components.Position>().position;

                                    if (headId == message.entityId)
                                    {

                                        // Add a turn point to the segment.
                                        canSend = true;
                                        var turnPoints = entity1.get<Shared.Components.TurnPoints>().turnPoints;
                                        turnPoints.Enqueue(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation));
                                        m_reportThese.Add(entity1.id);

                                        // m_reportThese.Add(entity1.id);

                                    }

                                }
                            }
                            if (canSend)
                            {
                                Message newMessage = new Shared.Messages.TurnPoint(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation), entity.id);
                                MessageQueueServer.instance.broadcastMessage(newMessage);

                            }
                        }




                        m_reportThese.Add(message.entityId);
                        break;
                    case Shared.Components.Input.Type.RotateLeft:



                        if (Shared.Entities.Utility.rotateLeft(entity, message.elapsedTime))
                        {
                            bool canSend = false;
                            Vector2 position1 = new Vector2();
                            foreach (Entity entity1 in m_entities.Values)
                            {
                                if (entity1.contains<Shared.Components.Segment>())
                                {
                                    var headId = entity1.get<Shared.Components.Segment>().headId;
                                    position1 = entity.get<Shared.Components.Position>().position;

                                    if (headId == message.entityId)
                                    {

                                        // Add a turn point to the segment.
                                        canSend = true;
                                        var turnPoints = entity1.get<Shared.Components.TurnPoints>().turnPoints;
                                        turnPoints.Enqueue(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation));
                                        m_reportThese.Add(entity1.id);

                                        // m_reportThese.Add(entity1.id);

                                    }

                                }
                            }
                            if (canSend)
                            {
                                Message newMessage = new Shared.Messages.TurnPoint(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation), entity.id);
                                MessageQueueServer.instance.broadcastMessage(newMessage);

                            }
                        }

                        m_reportThese.Add(message.entityId);
                        break;
                    case Shared.Components.Input.Type.RotateRight:
                        if (Shared.Entities.Utility.rotateRight(entity, message.elapsedTime))
                        {
                            bool canSend = false;
                            Vector2 position1 = new Vector2();
                            foreach (Entity entity1 in m_entities.Values)
                            {
                                if (entity1.contains<Shared.Components.Segment>())
                                {
                                    var headId = entity1.get<Shared.Components.Segment>().headId;
                                    position1 = entity.get<Shared.Components.Position>().position;

                                    if (headId == message.entityId)
                                    {

                                        // Add a turn point to the segment.
                                        canSend = true;
                                        var turnPoints = entity1.get<Shared.Components.TurnPoints>().turnPoints;
                                        turnPoints.Enqueue(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation));
                                        m_reportThese.Add(entity1.id);

                                        // m_reportThese.Add(entity1.id);

                                    }

                                }
                            }
                            if (canSend)
                            {
                                Message newMessage = new Shared.Messages.TurnPoint(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation), entity.id);
                                MessageQueueServer.instance.broadcastMessage(newMessage);

                            }
                        }
                        m_reportThese.Add(message.entityId);
                        break;
                    case Shared.Components.Input.Type.RotateDown:
                        if (Shared.Entities.Utility.rotateDown(entity, message.elapsedTime))
                        {
                            bool canSend = false;
                            Vector2 position1 = new Vector2();
                            foreach (Entity entity1 in m_entities.Values)
                            {
                                if (entity1.contains<Shared.Components.Segment>())
                                {
                                    var headId = entity1.get<Shared.Components.Segment>().headId;
                                    position1 = entity.get<Shared.Components.Position>().position;

                                    if (headId == message.entityId)
                                    {

                                        // Add a turn point to the segment.
                                        canSend = true;
                                        var turnPoints = entity1.get<Shared.Components.TurnPoints>().turnPoints;
                                        turnPoints.Enqueue(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation));
                                        m_reportThese.Add(entity1.id);

                                        // m_reportThese.Add(entity1.id);

                                    }

                                }
                            }
                            if (canSend)
                            {
                                Message newMessage = new Shared.Messages.TurnPoint(new Tuple<Vector2, float>(position1, entity.get<Shared.Components.Position>().orientation), entity.id);
                                MessageQueueServer.instance.broadcastMessage(newMessage);

                            }
                        }

                        m_reportThese.Add(message.entityId);
                        break;
                    


                }
            }
        }

        /// <summary>
        /// For the entities that have updates, send those updates to all
        /// connected clients.
        /// </summary>
        private void updateClients(TimeSpan elapsedTime)
        {
            foreach (var entityId in m_reportThese)
            {
                var entity = m_entities[entityId];
                var message = new Shared.Messages.UpdateEntity(entity, elapsedTime);
                MessageQueueServer.instance.broadcastMessageWithLastId(message);
            }

            m_reportThese.Clear();
        }
    }
}
