using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Shared.Entities;
using Shared;

namespace Client
{
    public class GamePlayView : GameStateView
    {
        private SpriteFont m_font;
        private string LEVELOVERMESSAGE = "";

        private const float RECTANGLE2_ROTATION_RATE = MathHelper.Pi / 4;  // radians per second
        private Texture2D backgroundImage;
        private Texture2D wallImage;
        //LunarLanderLevel m_level;
        private HashSet<Keys> m_previouslyDown = new HashSet<Keys>();
        private double playerAngle;
        private Vector2 playerPosition = new Vector2(100, 100);
        private Texture2D playerTexture;
        private Rectangle playerRectangle;
        float playerX;
        float playerY;
        double playerFuel = 20d;
        private bool isESCDown = true;
        //private KeyboardInput keyboardInput;
        private bool isUpPressed = false;
        private Keys up;
        private Keys left;
        private Keys right;
        private SoundEffect thrustSound;
        private SoundEffect explosionEffect;
        private BasicEffect m_effect;
        private SoundEffect levelClear;
        Texture2D t; //base for the line texture
        //private KeyControls m_loadedState = null;
        private HighScoresState m_highScoresState = null;
        private bool loading = false;

        private bool loadKeys = false;
        private bool firstUpdate = false;
        //private Circle playerCircle;
        
        private bool isCrashed = false;
        
        private SoundEffectInstance thrustInstance;

        TimeSpan timePlayed = TimeSpan.Zero;

        TimeSpan intervalBetweenLevels = TimeSpan.Zero;

        /*private ParticleSystem m_particleSystemFire;
        private ParticleSystem m_particleSystemSmoke;
        private ParticleSystemRenderer m_renderFire;
        private ParticleSystemRenderer m_renderSmoke;*/
        private bool isThrustUsed = false;
        GameModel m_gameModel = new GameModel();
        ContentManager contentManager1;

        private AnimatedSprite bananaRenderer;



      

 

        bool isPaused = false;
        public void ConnectToServer()
        {
            MessageQueueClient.shutdown();
            MessageQueueClient.instance.initialize("localhost", 3000);
            m_gameModel = new GameModel();
            m_gameModel.initialize(contentManager1);
        }
        public override void loadContent(ContentManager contentManager)
        {
            contentManager1 = contentManager;
            
            m_gameModel.initialize(contentManager);
            // create 1x1 texture for line drawing
            t = new Texture2D(m_graphics.GraphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { Color.White });
            m_font = contentManager.Load<SpriteFont>("Fonts/voicActivatedFont");
            playerTexture = contentManager.Load<Texture2D>("rocketShip");
            backgroundImage = contentManager.Load<Texture2D>("Cartoon_green_texture_grass_smaller");
            wallImage = contentManager.Load<Texture2D>("handpaintedwall2");
            thrustSound = contentManager.Load<SoundEffect>("smartsound_TRANSPORTATION_SPACE_Spaceshuttle_Rocket_Full_Power_Steady_01");
            levelClear = contentManager.Load<SoundEffect>("levelClearEffect");
            explosionEffect = contentManager.Load<SoundEffect>("mixkit-arcade-game-explosion-2759");
            thrustInstance = thrustSound.CreateInstance();
            thrustInstance.Volume = 0.25f;
            bananaRenderer = new AnimatedSprite(
                contentManager.Load<Texture2D>("spinning_banana"),
                new int[] { 40,40,40,40,40,40,40,40,40,40 }
            );


            playerX = m_graphics.PreferredBackBufferWidth / 6;
            playerY = m_graphics.PreferredBackBufferHeight / 8;
            playerRectangle = new Rectangle((int)playerX, (int)playerY, (int)(m_graphics.PreferredBackBufferWidth / 1920f * playerTexture.Width * 1.5f), (int)(m_graphics.PreferredBackBufferHeight / 1080f * playerTexture.Height * 1.5f));

            


        }



        private void loadControlsAndHighScores()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
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
                                    //DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(KeyControls));
                                    //m_loadedState = (KeyControls)mySerializer.ReadObject(fs);
                                }


                            }
                        }

                        if (storage.FileExists("HighScores.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(HighScoresState));
                                    m_highScoresState = (HighScoresState)mySerializer.ReadObject(fs);
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

   
        public override GameStateEnum processInput(GameTime gameTime)
        {
           
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !isESCDown)
            {
                isPaused = true;
                isESCDown = true;
                loadKeys = true;
                firstUpdate = true;
                MessageQueueClient.instance.sendMessage(new Shared.Messages.Disconnect());

                return GameStateEnum.MainMenu;
                
                
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                isESCDown = false;
            }

            if (isPaused)
            {
                
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                { 
                    isPaused = false;
                }

            }
            if (this.m_gameModel.isDead)
            {
                // If our own snake has died, display some important information
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    MessageQueueClient.instance.sendMessage(new Shared.Messages.Disconnect());

                    return GameStateEnum.MainMenu;
                }
            }




            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            
            m_gameModel.render(gameTime.ElapsedGameTime, m_spriteBatch, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight, backgroundImage, wallImage, bananaRenderer, m_font);
            

        
            m_spriteBatch.Begin();
            //m_spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), Color.White);
            float scale = m_graphics.PreferredBackBufferWidth / 1920f;

            int totalDone = 0;
            float buffer = 0f;
            // Draw the top players leaderboard in the top right.
            foreach (Tuple<string, int> score in m_gameModel.getScores())
            {
                // Render the top 5 in the top right of the screen.
                if (totalDone == 5)
                {
                    break;
                }
                totalDone ++;
                Vector2 stringSize1 = m_font.MeasureString((score.Item1+": "+ score.Item2.ToString())) * scale;
                m_spriteBatch.DrawString(
                          m_font,
                          (score.Item1 + ": " + score.Item2.ToString()),
                          new Vector2(m_graphics.PreferredBackBufferWidth * 0.75f - stringSize1.X / 2,
           m_graphics.PreferredBackBufferHeight / 4f - stringSize1.Y + buffer),
                          playerFuel > 0 ? Color.Green : Color.White,
                          0,
                          Vector2.Zero,
                          scale,
                          SpriteEffects.None,
                          0);
                buffer += stringSize1.Y;



                
            }




            // Draw our own score:
            // Render the top left message to quit the game and go to the main menu
            float scale1 = m_graphics.PreferredBackBufferWidth / 1920f;

            Vector2 stringSize2 = m_font.MeasureString("Press ESC To Return To Main Menu") * scale1;

           /* m_spriteBatch.Draw(whiteBackground, new Rectangle((int)(m_graphics.PreferredBackBufferWidth / 5 - stringSize2.X / 2),
            (int)(m_graphics.PreferredBackBufferHeight / 10f - stringSize2.Y), (int)stringSize2.X, (int)stringSize2.Y), Color.White);*/

            m_spriteBatch.DrawString(
                           m_font,
                           "Press ESC To Return To Main Menu",
                           new Vector2(m_graphics.PreferredBackBufferWidth / 5 - stringSize2.X / 2,
            m_graphics.PreferredBackBufferHeight / 10f - stringSize2.Y),
                           Color.Black,
                           0,
                           Vector2.Zero,
                           scale1,
                           SpriteEffects.None,
                           0);

            if (this.m_gameModel.isDead)
            {
                // Render button to rerturn to menu
            }


            m_spriteBatch.End();

            // Render your score at the top of the screen




        }



        public override void update(GameTime gameTime)
        {
            bananaRenderer.update(gameTime);


            foreach (var key in m_previouslyDown)
            {
                if (Keyboard.GetState().IsKeyUp(key))
                {
                    m_gameModel.signalKeyReleased(key);
                    m_previouslyDown.Remove(key);
                }
            }

            foreach (var key in Keyboard.GetState().GetPressedKeys())
            {
                if (!m_previouslyDown.Contains(key))
                {
                    m_gameModel.signalKeyPressed(key);
                    m_previouslyDown.Add(key);
                }
            }

            


            m_gameModel.update(gameTime);

           
        }

        

      

        private void saveHighScore(HighScoresState highScore)
        {
            lock (this)
            {
                finalizeSaveAsyncHighScores(highScore);
            }
        }

        private async Task finalizeSaveAsyncHighScores(HighScoresState state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.json", FileMode.Open))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(HighScoresState));
                                mySerializer.WriteObject(fs, state);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                    }
                }

            });
        }



    }
}
