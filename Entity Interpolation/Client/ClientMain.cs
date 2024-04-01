using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Client
{
    public class ClientMain : Game
    {
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;
        private IGameState m_currentState;
        private IGameState m_prevState;
        private Dictionary<GameStateEnum, IGameState> m_gameStates;
        private IGameState savedGamePlay;

        private SettingsView m_settings;
        private GamePlayView m_gamePlayView;
        private GameStateEnum m_gameState;
        private HelpView m_helpView;

        
        private GameModel m_gameModel = new GameModel();

        public ClientMain()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
           /* m_graphics.PreferredBackBufferWidth = 1920;
            m_graphics.PreferredBackBufferHeight = 1080;
            m_graphics.ApplyChanges();*/

            m_gameModel.initialize(this.Content);
            //setUpFiles();


            //m_graphics.IsFullScreen = true;
            /*m_graphics.PreferredBackBufferWidth = 1920;
            m_graphics.PreferredBackBufferHeight = 1080;
            m_graphics.ApplyChanges();*/


            // TODO: Add your initialization logic here
            m_settings = new SettingsView();
            m_gamePlayView = new GamePlayView();
            m_helpView = new HelpView();
            m_gameStates = new Dictionary<GameStateEnum, IGameState>();
            m_gameStates.Add(GameStateEnum.About, new AboutView());
            m_gameStates.Add(GameStateEnum.MainMenu, new MainMenuView());
            m_gameStates.Add(GameStateEnum.GamePlay, m_gamePlayView);
            m_gameStates.Add(GameStateEnum.Paused, new PauseView());
            m_gameStates.Add(GameStateEnum.Settings, m_settings);
            m_gameStates.Add(GameStateEnum.HighScores, new HighScoresView());
            m_gameStates.Add(GameStateEnum.Help, m_helpView);

            foreach (var item in m_gameStates)
            {
                item.Value.initialize(this.GraphicsDevice, m_graphics);
            }

            m_currentState = m_gameStates[GameStateEnum.MainMenu];
            m_prevState = m_gameStates[GameStateEnum.MainMenu];
            m_gameState = GameStateEnum.MainMenu;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (var item in m_gameStates)
            {
                item.Value.loadContent(this.Content);
            }
            MessageQueueClient.instance.initialize("localhost", 3000);
        }

        private HashSet<Keys> m_previouslyDown = new HashSet<Keys>();
        protected override void Update(GameTime gameTime)
        {

            GameStateEnum nextStateEnum = m_currentState.processInput(gameTime);

            if (nextStateEnum == GameStateEnum.Exit)
            {
                MessageQueueClient.instance.sendMessage(new Shared.Messages.Disconnect());
                MessageQueueClient.instance.shutdown();
                Exit();
            }

            else
            {
                m_currentState.update(gameTime);


                /*if (m_prevState == m_gameStates[GameStateEnum.MainMenu] && nextStateEnum == GameStateEnum.GamePlay)
                {
                    m_gamePlayView.resetGameplay();

                }*/

                if (m_prevState == m_gameStates[GameStateEnum.GamePlay] && nextStateEnum == GameStateEnum.Paused)
                {
                    savedGamePlay = m_currentState;
                }





                if (nextStateEnum == GameStateEnum.Settings && m_gameState != GameStateEnum.Settings)
                {

                    m_settings.prevState = m_gameState;


                }


                if (nextStateEnum == GameStateEnum.Help && m_gameState != GameStateEnum.Help)
                {

                    m_helpView.helpPrevState = m_gameState;


                }

                if (nextStateEnum == GameStateEnum.HighScores)
                {
                    m_gameStates[nextStateEnum] = null;
                    m_gameStates[nextStateEnum] = new HighScoresView();
                    m_gameStates[nextStateEnum].initialize(this.GraphicsDevice, m_graphics);
                    m_gameStates[nextStateEnum].loadContent(this.Content);


                }


                m_currentState = m_gameStates[nextStateEnum];
                m_prevState = m_gameStates[nextStateEnum];
                m_gameState = nextStateEnum;

            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_currentState.render(gameTime);


            base.Draw(gameTime);
        }
    }
}
