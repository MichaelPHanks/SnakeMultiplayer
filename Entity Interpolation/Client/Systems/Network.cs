
using Microsoft.Xna.Framework;
using Shared;
using Shared.Components;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Client.Systems
{
    public class Network : Shared.Systems.System
    {
        public delegate void Handler(TimeSpan elapsedTime, Shared.Messages.Message message);
        public delegate void TurnPointHandler(Shared.Messages.TurnPoint turnPoint);
        public delegate void RemoveEntityHandler(RemoveEntity message);
        public delegate void NewEntityHandler(NewEntity message);
        public delegate void ScoresUpdateHandler (ScoresUpdate message);
        public delegate void PlayerDeathHandler(PlayerDeath message);


        private Dictionary<Shared.Messages.Type, Handler> m_commandMap = new Dictionary<Shared.Messages.Type, Handler>();
        private RemoveEntityHandler m_removeEntityHandler;
        private TurnPointHandler m_turnPointHandler;
        private NewEntityHandler m_newEntityHandler;
        private ScoresUpdateHandler m_scoresUpdateHandler;
        private PlayerDeathHandler m_playerDeathHandler;
        private uint m_lastMessageId = 0;
        private HashSet<uint> m_updatedEntities = new HashSet<uint>();
        private bool loading = false;
        private PlayerNameState m_playerNameState = null;

        /// <summary>
        /// Primary activity in the constructor is to setup the command map
        // that maps from message types to their handlers.
        /// </summary>
        public Network() :
            base(typeof(Shared.Components.Position))
        {

            registerHandler(Shared.Messages.Type.ConnectAck, (TimeSpan elapsedTime, Message message) =>
            {
                handleConnectAck(elapsedTime, (ConnectAck)message);
            });

            registerHandler(Shared.Messages.Type.NewEntity, (TimeSpan elapsedTime, Message message) =>
            {
                m_newEntityHandler((NewEntity)message);
            });

            registerHandler(Shared.Messages.Type.UpdateEntity, (TimeSpan elapsedTime, Message message) =>
            {
                handleUpdateEntity(elapsedTime, (UpdateEntity)message);
            });

            registerHandler(Shared.Messages.Type.RemoveEntity, (TimeSpan elapsedTime, Message message) =>
            {
                m_removeEntityHandler((RemoveEntity)message);
            });
           /* registerHandler(Shared.Messages.Type.PlayerDeath, (TimeSpan elapsedTime, Message message) =>
            {
                handlePlayerDeath((PlayerDeath)message);
            });*/
            registerHandler(Shared.Messages.Type.TurnPoint, (TimeSpan elapsedTime, Message message) =>
            {
                m_turnPointHandler((TurnPoint)message);
            });
            registerHandler(Shared.Messages.Type.ScoresUpdate, (TimeSpan elapsedTime, Message message) =>
            {
                m_scoresUpdateHandler((ScoresUpdate)message);
            });
            registerHandler(Shared.Messages.Type.PlayerDeath, (TimeSpan elapsedTime, Message message) =>
            {
                m_playerDeathHandler((PlayerDeath)message);
            });

        }



        // Have to implement this because it is abstract in the base class
        public override void update(TimeSpan elapsedTime) { }

        /// <summary>
        /// Have our own version of update, because we need a list of messages to work with, and
        /// messages aren't entities.
        /// </summary>
        public void update(TimeSpan elapsedTime, Queue<Message> messages)
        {
            m_updatedEntities.Clear();

            if (messages != null)
            {
                while (messages.Count > 0)
                {
                    var message = messages.Dequeue();
                    if (m_commandMap.ContainsKey(message.type))
                    {
                        m_commandMap[message.type](elapsedTime, message);
                    }

                    if (message.messageId.HasValue)
                    {
                        m_lastMessageId = message.messageId.Value;
                    }
                }
            }

            // After processing all the messages, perform server reconciliation by
            // resimulating the inputs from any sent messages not yet acknowledged by the server.
            /*var sent = MessageQueueClient.instance.getSendMessageHistory(m_lastMessageId);
            while (sent.Count > 0)
            {
                var message = (Shared.Messages.Input)sent.Dequeue();
                if (message.type == Shared.Messages.Type.Input)
                {
                    var entity = m_entities[message.entityId];
                    if (m_updatedEntities.Contains(entity.id))
                    {
                        foreach (var input in message.inputs)
                        {
                            switch (input)
                            {
                                case Shared.Components.Input.Type.RotateUp:
                                    Shared.Entities.Utility.rotateUp(entity, message.elapsedTime);
                                    break;
                                case Shared.Components.Input.Type.RotateLeft:
                                    Shared.Entities.Utility.rotateLeft(entity, message.elapsedTime);
                                    break;
                                case Shared.Components.Input.Type.RotateRight:
                                    Shared.Entities.Utility.rotateRight(entity, message.elapsedTime);
                                    break;
                                case Shared.Components.Input.Type.RotateDown:
                                    Shared.Entities.Utility.rotateDown(entity, message.elapsedTime);
                                    break;

                            }
                        }
                    }
                }
            }*/
        }


        
        private void registerHandler(Shared.Messages.Type type, Handler handler)
        {
            m_commandMap[type] = handler;
        }

        public void registerNewEntityHandler(NewEntityHandler handler)
        {
            m_newEntityHandler = handler;
        }

        public void registerRemoveEntityHandler(RemoveEntityHandler handler)
        {
            m_removeEntityHandler = handler;
        }

        public void registerTurnPointMessage(TurnPointHandler handler) 
        { 
            m_turnPointHandler = handler;
        }
        public void registerScoresUpdateHandler(ScoresUpdateHandler handler)
        {
           m_scoresUpdateHandler = handler;
        }
        public void registerPlayerDeathHandler(PlayerDeathHandler handler)
        {
            m_playerDeathHandler = handler;
        }

        /// <summary>
        /// Handler for the ConnectAck message.  This records the clientId
        /// assigned to it by the server, it also sends a request to the server
        /// to join the game.
        /// </summary>
        private void handleConnectAck(TimeSpan elapsedTime, ConnectAck message) 
        {
            loadPlayerName();
            MessageQueueClient.instance.sendMessage(new Join(m_playerNameState.getPlayerName()));
        }
        private void loadPlayerName()
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
                        if (storage.FileExists("PlayerName.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("PlayerName.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(PlayerNameState));
                                    m_playerNameState = (PlayerNameState)mySerializer.ReadObject(fs);
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
        /// <summary>
        /// Handler for the UpdateEntity message.  It checks to see if the client
        /// actually has the entity, and if it does, updates the components
        /// that are in common between the message and the entity.
        /// </summary>
        private void handleUpdateEntity(TimeSpan elapsedTime, UpdateEntity message) 
        { 
            if (m_entities.ContainsKey(message.id))
            {
                var entity = m_entities[message.id];
                if (entity.contains<Components.Goal>() && message.hasPosition)
                {
                    var position = entity.get<Position>();
                    var goal = entity.get<Components.Goal>();

                    goal.updateWindow = message.updateWindow;
                    goal.updatedTime = TimeSpan.Zero;
                    goal.goalPosition = new Vector2(message.position.X, message.position.Y);
                    goal.goalOrientation = message.orientation;

                    goal.startPosition = position.position;
                    goal.startOrientation = position.orientation;
                }
                if (entity.contains<Position>() && message.hasPosition)
                {
                    entity.get<Position>().position = message.position;
                    entity.get<Position>().orientation = message.orientation;

                    m_updatedEntities.Add(entity.id);
                }

                if (message.hasTurnPoints)
                {
                    entity.get<TurnPoints>().turnPoints = message.turnPoints;
                }
            }
        }

        
        
    }
}
