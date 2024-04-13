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

     

        public enum Level
        {
            LEVELONE,
            LEVELTWO,

            
        }

        public enum Stage
        {
            PLAYING,
            COMPLETED,
        }

        

        private Level currentLevel = Level.LEVELONE;
        private Stage currentStage = Stage.PLAYING;

        bool isPaused = false;
        public void ConnectToServer()
        {
            //MessageQueueClient.instance.initialize("localhost", 3000);
            MessageQueueClient.shutdown();
            MessageQueueClient.instance.initialize("localhost", 3000);
            //MessageQueueClient.instance.sendMessage(new Shared.Messages.Join());
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



            playerX = m_graphics.PreferredBackBufferWidth / 6;
            playerY = m_graphics.PreferredBackBufferHeight / 8;
            playerRectangle = new Rectangle((int)playerX, (int)playerY, (int)(m_graphics.PreferredBackBufferWidth / 1920f * playerTexture.Width * 1.5f), (int)(m_graphics.PreferredBackBufferHeight / 1080f * playerTexture.Height * 1.5f));

            /*keyboardInput = new KeyboardInput();


            loadControlsAndHighScores();


            keyboardInput.registerCommand(m_loadedState.Up, false, new IInputDevice.CommandDelegate(onMoveUp));
            keyboardInput.registerCommand(m_loadedState.Left, false, new IInputDevice.CommandDelegate(onMoveLeft));
            keyboardInput.registerCommand(m_loadedState.Right, false, new IInputDevice.CommandDelegate(onMoveRight));
            up = m_loadedState.Up;
            left = m_loadedState.Left;
            right = m_loadedState.Right;
            m_level =  new LunarLanderLevel(1, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);*/

            /*m_graphics.GraphicsDevice.RasterizerState = new RasterizerState
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.CullCounterClockwiseFace,   // CullMode.None If you want to not worry about triangle winding order
                MultiSampleAntiAlias = true,
            };
            m_effect = new BasicEffect(m_graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),

                Projection = Matrix.CreateOrthographicOffCenter(
                    0, m_graphics.GraphicsDevice.Viewport.Width,
                    m_graphics.GraphicsDevice.Viewport.Height, 0,   // doing this to get it to match the default of upper left of (0, 0)
                    0.1f, 2)
            };*/
         
           /* playerCircle = new Circle(new Tuple<double,double>(playerX , playerY), playerRectangle.Height / 2);

            m_particleSystemFire = new ParticleSystem(
                (int)(m_graphics.PreferredBackBufferWidth / 1920f * 10), (int)(m_graphics.PreferredBackBufferWidth / 1920f * 4),
                (m_graphics.PreferredBackBufferWidth / 1920f * 0.12f), (m_graphics.PreferredBackBufferWidth / 1920f * 0.03f),
                650, 100);
            
            m_renderFire = new ParticleSystemRenderer("fire");
            m_particleSystemSmoke = new ParticleSystem(
                (int)(m_graphics.PreferredBackBufferWidth / 1920f * 10), (int)(m_graphics.PreferredBackBufferWidth / 1920f * 4),
                (m_graphics.PreferredBackBufferWidth / 1920f * 0.12f), (m_graphics.PreferredBackBufferWidth / 1920f * 0.03f),
                650, 100);
            m_renderSmoke = new ParticleSystemRenderer("smoke-2");


            m_renderFire.LoadContent(contentManager);
            m_renderSmoke.LoadContent(contentManager);*/


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

        private void onMoveRight(GameTime gameTime)
        {
            if (currentStage == Stage.PLAYING)
            {
                

               /* m_level.playerAngle += (RECTANGLE2_ROTATION_RATE * gameTime.ElapsedGameTime.TotalMilliseconds / 250.0f);
                if (m_level.playerAngle > 2 * Math.PI)
                {
                    m_level.playerAngle -= 2 * Math.PI;
                }*/
            }
           
        }

        private void onMoveLeft(GameTime gameTime)
        {
            if (currentStage == Stage.PLAYING)
            {
                
                /*m_level.playerAngle -= (RECTANGLE2_ROTATION_RATE * gameTime.ElapsedGameTime.TotalMilliseconds / 250.0f);
                if (m_level.playerAngle < 0)
                {
                    m_level.playerAngle += 2 * Math.PI;
                }*/
            }

          
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            
            /*keyboardInput.Update(gameTime);
            if (!isThrustUsed)
            {
                thrustInstance.Pause();
            }

            if (Keyboard.GetState().IsKeyUp(m_loadedState.Up))
            {
                isThrustUsed = false;
            }*/
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !isESCDown)
            {
                isPaused = true;
                isESCDown = true;
                loadKeys = true;
                firstUpdate = true;
                return GameStateEnum.Paused;
                
                
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                isESCDown = false;
            }




            
            

            // Reset the thrust vector
            /*if (!isUpPressed) 
            {
                m_level.thrustVector.Y = 0;
                m_level.thrustVector.X = 0;
            }
            isUpPressed = false;

            if (playerFuel <= 0)
            {
                m_level.thrustVector.Y = 0;
                m_level.thrustVector.X = 0;
            }
            */
            if (isPaused)
            {
                
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                { 
                    isPaused = false;
                }

            }




            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            //m_spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), Color.White);
            m_spriteBatch.End();
            m_gameModel.render(gameTime.ElapsedGameTime, m_spriteBatch, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight, backgroundImage, wallImage);


            // Render the background:
            //m_spriteBatch.Begin();
           // m_spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), currentStage == Stage.PLAYING ? Color.White : Color.Gray);


            // Render the lines
            
            /*foreach (Line line in m_level.lines)
            {
                Vector2 start = new Vector2(line.x1, line.y1);
                Vector2 end = new Vector2(line.x2, line.y2);

                Vector2 edge = end - start;
                float angle =
                    (float)Math.Atan2(edge.Y, edge.X);


                m_spriteBatch.Draw(t,
                    new Rectangle(
                        (int)start.X,
                        (int)start.Y - 2,
                        (int)edge.Length() + 1,
                        4),
                    null,
                    line.isSafe ? Color.Blue : Color.White,
                    angle,
                    new Vector2(0, 0),
                    SpriteEffects.None,
                    0);


            }*/

            //m_spriteBatch.End();

           /* // Render triangles: 
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    m_level.m_vertsTris, 0, m_level.m_vertsTris.Length,
                    m_level.m_indexTris, 0, m_level.m_indexTris.Length / 3);
            }*/
            //m_spriteBatch.Begin();
            //float scale = m_graphics.PreferredBackBufferWidth / 1920f;

            //Vector2 stringSize1 = m_font.MeasureString(("Fuel   : " + string.Format("{0:0.00}", playerFuel) + " s").PadRight(2)) * scale;



           /* m_spriteBatch.DrawString(
                           m_font,
                           ("Fuel   : " + string.Format("{0:0.00}", playerFuel) + " s").PadRight(2),
                           new Vector2(m_graphics.PreferredBackBufferWidth * 0.75f - stringSize1.X / 2,
            m_graphics.PreferredBackBufferHeight / 4f - stringSize1.Y),
                           playerFuel > 0 ? Color.Green : Color.White,
                           0,
                           Vector2.Zero,
                           scale,
                           SpriteEffects.None,
                           0);*/


           /* stringSize1 = m_font.MeasureString(("Speed  : " + string.Format("{0:0.00}", Math.Abs(m_level.playerVectorVelocity.Y)) + " m/s").PadRight(2)) * scale;

            m_spriteBatch.DrawString(
                          m_font,
                          ("Speed  : " + string.Format("{0:0.00}", Math.Abs(m_level.playerVectorVelocity.Y)) + " m/s").PadRight(2),
                         new Vector2(m_graphics.PreferredBackBufferWidth * 0.75f - stringSize1.X / 2,
            m_graphics.PreferredBackBufferHeight / 4f - stringSize1.Y + stringSize1.Y),
                          Math.Abs(m_level.playerVectorVelocity.Y) > 2 ? Color.White : Color.Green,
                          0,
                          Vector2.Zero,
                          scale,
                          SpriteEffects.None,
                          0);*/


           /* stringSize1 = m_font.MeasureString(("Angle  : " + string.Format("{0:0.00}", MathHelper.ToDegrees((float)m_level.playerAngle)) + "").PadRight(2)) * scale;

            m_spriteBatch.DrawString(
                          m_font,
                          ("Angle  : " + string.Format("{0:0.00}", MathHelper.ToDegrees((float)m_level.playerAngle)) + "").PadRight(2),
                          new Vector2(m_graphics.PreferredBackBufferWidth * 0.75f - stringSize1.X / 2,
            m_graphics.PreferredBackBufferHeight / 4f - stringSize1.Y + 2 * stringSize1.Y),
                         MathHelper.ToDegrees((float)m_level.playerAngle) < 5 || MathHelper.ToDegrees((float)m_level.playerAngle) > 355 ? Color.Green : Color.White,
                          0,
                          Vector2.Zero,
                          scale,
                          SpriteEffects.None,
                          0);*/
            if (!isCrashed)
            {
              /*  m_spriteBatch.Draw(
                        playerTexture,
                        new Rectangle((int)playerX, (int)playerY, playerRectangle.Width, playerRectangle.Height),
                        null, // Drawing the whole texture, not a part
                        Color.White,
                        (float)m_level.playerAngle,
                        new Vector2(playerTexture.Width / 2, playerTexture.Height / 2),
                        SpriteEffects.None,
                        0);*/
            }

           /* if (currentStage == Stage.COMPLETED)
            {
                // Render the level over message & time until 'next' level.
                float scale1 = m_graphics.PreferredBackBufferWidth / 1920f;

                Vector2 stringSize2 = m_font.MeasureString(LEVELOVERMESSAGE) * scale1;



                m_spriteBatch.DrawString(
                               m_font,
                               LEVELOVERMESSAGE,
                               new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize2.X / 2,
                m_graphics.PreferredBackBufferHeight / 1.5f - stringSize2.Y),
                               Color.White,
                               0,
                               Vector2.Zero,
                               scale1,
                               SpriteEffects.None,
                               0);


                stringSize2 = m_font.MeasureString(intervalBetweenLevels.Seconds.ToString()) * scale;

                m_spriteBatch.DrawString(
                              m_font,
                             intervalBetweenLevels.Seconds.ToString(),
                             new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize2.X / 2,
                m_graphics.PreferredBackBufferHeight / 1.5f - stringSize2.Y + stringSize2.Y),
                              Color.White,
                              0,
                              Vector2.Zero,
                              scale,
                              SpriteEffects.None,
                              0);


            }*/

          /*  else 
            {
                // Render the Level in the top left
                float scale1 = m_graphics.PreferredBackBufferWidth / 1920f;

                Vector2 stringSize2 = m_font.MeasureString(currentLevel == Level.LEVELONE ? "Level: 1": "Level: 2") * scale1;

                m_spriteBatch.DrawString(
                               m_font,
                               currentLevel == Level.LEVELONE ? "Level: 1" : "Level: 2",
                               new Vector2(m_graphics.PreferredBackBufferWidth / 10 - stringSize2.X / 2,
                m_graphics.PreferredBackBufferHeight / 10f - stringSize2.Y),
                               Color.Black,
                               0,
                               Vector2.Zero,
                               scale1,
                               SpriteEffects.None,
                               0);
            }
            m_spriteBatch.End();*/
           /* m_renderSmoke.draw(m_spriteBatch, m_particleSystemSmoke);

            m_renderFire.draw(m_spriteBatch, m_particleSystemFire);*/




        }



        public override void update(GameTime gameTime)
        {

            /*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                MessageQueueClient.instance.sendMessage(new Shared.Messages.Disconnect());
                MessageQueueClient.instance.shutdown();
            }*/

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

            

            //m_gameModel.signalKeyPressed(Keys.W);

            m_gameModel.update(gameTime.ElapsedGameTime);
            /* m_particleSystemFire.update(gameTime);
             m_particleSystemSmoke.update(gameTime);*/

            // If we are 'playing' the game
            if (currentStage == Stage.PLAYING)
            {
                timePlayed += gameTime.ElapsedGameTime;



                // Reload the keys, if needed
               /* if (loadKeys && !firstUpdate)
                {
                    loadControlsAndHighScores();

                    ModifyKey(KeyEnum.Up, m_loadedState.Up);
                    ModifyKey(KeyEnum.Left, m_loadedState.Left);
                    ModifyKey(KeyEnum.Right, m_loadedState.Right);
                    loadKeys = false;
                }
*/
                firstUpdate = false;

                // Checks collision of all lines with the player circle. Applies gravity and thrust vector, if there is no collision.
                /*if (!isCollision().Item1)
                {
*//*
                    m_level.playerVectorVelocity +=  m_level.gravityVector * (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
                    m_level.playerVectorVelocity += m_level.thrustVector * 0.27f; ;
                    Debug.WriteLine(m_level.playerVectorVelocity.X);

                    playerX += (float)(m_graphics.PreferredBackBufferWidth / 1920f) * (float)(m_level.playerVectorVelocity.X * 0.1);
                    playerY -= (float)(m_graphics.PreferredBackBufferHeight / 1080f) * (float)(m_level.playerVectorVelocity.Y * 0.1);
                    playerCircle.setCenter(new Tuple<double, double>(playerCircle.center.Item1 + (float)(m_graphics.PreferredBackBufferWidth / 1920f) * (m_level.playerVectorVelocity.X * 0.1), playerCircle.center.Item2 - (float)(m_graphics.PreferredBackBufferHeight / 1080f) * (m_level.playerVectorVelocity.Y * 0.1)));
*//*
                }*/

                // If we collided with something and it is a safe zone
              /*  else if (isCollision().Item2)
                {
                    thrustInstance.Pause();

                    currentStage = Stage.COMPLETED;


                    if (MathHelper.ToDegrees((float)m_level.playerAngle) > 355 || MathHelper.ToDegrees((float)m_level.playerAngle) < 5)
                    {
                        if (Math.Abs(m_level.playerVectorVelocity.Y) <= 2)
                        {
                            levelClear.Play();
                            // Once we reach here, we have successfully completed the level and the game is over or we are on to level 2!

                            if (currentLevel == Level.LEVELONE)
                            {
                                // Give a three second counter (3,2,1), and then transition to the second level
                                intervalBetweenLevels += new TimeSpan(0, 0, 4);


                                currentLevel = Level.LEVELTWO;
                                LEVELOVERMESSAGE = "Level 1 complete, onto level 2!";


                            }

                            else
                            {
                                // We completed the game, and should add to the highscores. Reset the gameplay after 5 seconds.
                                m_highScoresState.addHighScore(new Tuple<int, DateTime>((int)(timePlayed.TotalMilliseconds), DateTime.Now));
                                intervalBetweenLevels += new TimeSpan(0, 0, 6);

                                saveHighScore(m_highScoresState);
                                currentLevel = Level.LEVELONE;


                                LEVELOVERMESSAGE = "Level 2 complete, score: " + (int)(timePlayed.TotalMilliseconds);
                                timePlayed = TimeSpan.Zero;

                            }

                        }
                        else
                        {
                            LEVELOVERMESSAGE = "Try going a little slower next time!";
                            shipBlowup();
                        }
                    }
                    else
                    {
                        LEVELOVERMESSAGE = "You can't land a ship at that angle!";
                        shipBlowup();
                    }



                }

                // If we collided with something and it is not a safe zone.
                else
                {
                    thrustInstance.Pause();

                    LEVELOVERMESSAGE = "You're not supposed to do that!";
                    shipBlowup();
                }
            }

            else 
            {
                intervalBetweenLevels -= gameTime.ElapsedGameTime;

                if (intervalBetweenLevels.TotalMilliseconds <= 0)
                {
                    intervalBetweenLevels = TimeSpan.Zero;
                    currentStage = Stage.PLAYING;
                    playerFuel = 20d;
                    
                    m_level = new LunarLanderLevel(currentLevel == Level.LEVELONE ? 1: 2, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
                    playerX = m_graphics.PreferredBackBufferWidth / 6;
                    playerY = m_graphics.PreferredBackBufferHeight / 8;
                    playerCircle = new Circle(new Tuple<double, double>(playerX, playerY), playerRectangle.Height / 2);
                    isCrashed = false;

                }*/
            }
        }

        

      /*  private void shipBlowup()
        {
            explosionEffect.Play();
            currentStage = Stage.COMPLETED;
            intervalBetweenLevels += new TimeSpan(0, 0, 4);
            currentLevel = Level.LEVELONE;
            timePlayed = TimeSpan.Zero;
            isCrashed = true;
            m_particleSystemFire.shipCrash(new Vector2((float)playerCircle.center.Item1, (float)playerCircle.center.Item2));
            m_particleSystemSmoke.shipCrash(new Vector2((float)playerCircle.center.Item1, (float)playerCircle.center.Item2));
        }*/

       /* private void onMoveUp(GameTime gameTime)
        {
            if (currentStage == Stage.PLAYING)
            {


                if (playerFuel > 0)
                {
                    isThrustUsed = true;
                    
                    if (thrustInstance.State == SoundState.Paused || thrustInstance.State == SoundState.Stopped)
                    {
                        thrustInstance.Play();
                    }
                    
                    // Add 90 to the degrees to get the 'correct' degrees
                    m_particleSystemFire.shipThrust((float)m_level.playerAngle, new Vector2((float)(playerCircle.center.Item1 - playerCircle.radius * Math.Cos(m_level.playerAngle - Math.PI/2)), (float)(playerCircle.center.Item2 - playerCircle.radius * Math.Sin(m_level.playerAngle - Math.PI/2))));
                    m_particleSystemSmoke.shipThrust((float)m_level.playerAngle, new Vector2((float)(playerCircle.center.Item1 - playerCircle.radius * Math.Cos(m_level.playerAngle - Math.PI / 2)), (float)(playerCircle.center.Item2 - playerCircle.radius * Math.Sin(m_level.playerAngle - Math.PI / 2))));
                    
                    m_level.thrustVector.X = (float)Math.Cos(m_level.playerAngle - Math.PI / 2);
                    m_level.thrustVector.Y = -(float)Math.Sin(m_level.playerAngle - Math.PI / 2);
                    playerFuel -= gameTime.ElapsedGameTime.TotalSeconds;
                }
                isUpPressed = true;
            }
        }*/

       /* public void ModifyKey(KeyEnum keyType, Keys newKey)
        {
            if (keyType == KeyEnum.Left)
            {
                keyboardInput.removeKey(left);
                keyboardInput.registerCommand(newKey, false, new IInputDevice.CommandDelegate(onMoveLeft));
                left  = newKey;
            }
            else if (keyType == KeyEnum.Right)
            {
                keyboardInput.removeKey(right);
                keyboardInput.registerCommand(newKey, false, new IInputDevice.CommandDelegate(onMoveRight));
                right = newKey;
            }
            else if (keyType == KeyEnum.Up)
            {
                keyboardInput.removeKey(up);
                keyboardInput.registerCommand(newKey, false, new IInputDevice.CommandDelegate(onMoveUp));
                up = newKey;
            }
        }*/
        /// <summary>
        ///     Resets the gameplay without having to remake a gameplayview object.
        /// </summary>
      /*  public void resetGameplay()
        {
            
            timePlayed = TimeSpan.Zero;
            intervalBetweenLevels = TimeSpan.Zero;
            currentLevel = Level.LEVELONE;
            currentStage = Stage.PLAYING;
            m_level = new LunarLanderLevel(1, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
            playerFuel = 20d;

            loadControlsAndHighScores();

            ModifyKey(KeyEnum.Up, m_loadedState.Up);
            ModifyKey(KeyEnum.Left, m_loadedState.Left);
            ModifyKey(KeyEnum.Right, m_loadedState.Right);
            playerX = m_graphics.PreferredBackBufferWidth / 6;
            playerY = m_graphics.PreferredBackBufferHeight / 8;
            playerCircle = new Circle(new Tuple<double, double>(playerX , playerY), playerRectangle.Height / 2);
            isCrashed = false;


        }
*/
        /// <summary>
        ///     Checks collision of all lines with player circle.
        /// </summary>
        /// <returns> Tuple, with Item 1 being if there was a line that collided with player circle. Item 2 is if it is a safe zone.</returns>
       /* public Tuple<bool, bool> isCollision()
        {
            for (int i = 0; i < m_level.lines.Count; i++)
            {
                if (lineCircleInterSection(m_level.lines[i], playerCircle))
                { 
                    return new Tuple<bool,bool>(true, m_level.lines[i].isSafe);
                }

            }

            return new Tuple<bool,bool>(false,false); 
        }
*/
      /*  public bool lineCircleInterSection(Line line, Circle circle)
        {
            var v1 = new { X = line.x2 - line.x1, Y = line.y2 - line.y1 };
            var v2 = new { X = line.x1 - circle.center.Item1, Y = line.y1 - circle.center.Item2 };
            var b = -2 * (v1.X * v2.X + v1.Y * v2.Y);
            var c = 2 * (v1.X * v1.X + v1.Y * v1.Y);
            var d = Math.Sqrt(b * b - 2 * c * (v2.X * v2.X + v2.Y * v2.Y - circle.radius * circle.radius));

            if (double.IsNaN(d)) // no intercept
            {
                return false;
            }

            // These represent the unit distance of point one and two on the line
            var u1 = (b - d) / c;
            var u2 = (b + d) / c;

            if (u1 <= 1 && u1 >= 0) // If point on the line segment
            {
                return true;
            }

            if (u2 <= 1 && u2 >= 0) // If point on the line segment
            {
                return true;
            }

            return false;
        }*/

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
