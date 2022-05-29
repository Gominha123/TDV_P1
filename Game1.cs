using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P1_Monogame.Content;
using P1_Monogame.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;


namespace P1_Monogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont _font;
        private Vector2 _fontY;
        public static Random Random;

        private Button playButton;
        private Button resumeButton;
        private Button quitButton;
        private Button scoreButton;
        private Button menuButton;

        private Texture2D playTexture;
        private Texture2D resumeTexture;
        private Texture2D quitTexture;
        private Texture2D scoreTexture;
        private Texture2D menuTexture;

        private Vector2 Origin = Vector2.Zero;

        private KeyboardState _currentKey;
        private KeyboardState _previousKey;
        private MouseState _currentMouse;
        private MouseState _previousMouse;

        private ScoreManager _scoreManager;

        private Texture2D backGround;
        private Rectangle pGround;
        private Texture2D menuBground;

        public static int screenWidth;
        public static int screenHeight;

        private List<Sprite> sprites;
        public bool hasStarted = false;
        private int rounds;
        public float timer;
        private float roundTimer;
        private float nextRoundsTimer;
        private int RoundTime = 10;

        Song song;

        enum GameState
        {
            Menu,
            Score,
            Play,
            Pause,
            Lose,
            Quit,
        }

        GameState gameState = GameState.Menu;
        List<SoundEffect> soundEffects;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            soundEffects = new List<SoundEffect>();

            Random = new Random();

            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuBground = Content.Load<Texture2D>("menuBground");
            backGround = Content.Load<Texture2D>("cityBground");
            _font = Content.Load<SpriteFont>("Font");
            _scoreManager = ScoreManager.Load();

            playTexture = Content.Load<Texture2D>("Button/play");
            playButton = new Button(playTexture, graphics.GraphicsDevice);
            playButton.setPosition(new Vector2((screenWidth / 2) - 75, 150));

            scoreTexture = Content.Load<Texture2D>("Button/score");
            scoreButton = new Button(scoreTexture, graphics.GraphicsDevice);
            scoreButton.setPosition(new Vector2((screenWidth / 3) + 30, 160 + playTexture.Height));

            resumeTexture = Content.Load<Texture2D>("Button/back");
            resumeButton = new Button(resumeTexture, graphics.GraphicsDevice);
            resumeButton.setPosition(new Vector2(10, screenHeight - resumeTexture.Height - 10));

            quitTexture = Content.Load<Texture2D>("Button/back");
            quitButton = new Button(quitTexture, graphics.GraphicsDevice);
            quitButton.setPosition(new Vector2(10, screenHeight - quitTexture.Height - 10));

            menuTexture = Content.Load<Texture2D>("Button/menu");
            menuButton = new Button(menuTexture, graphics.GraphicsDevice);
            menuButton.setPosition(new Vector2((screenWidth / 2) - 75, 150));

            pGround = new Rectangle(0, 0, screenWidth, screenHeight);

            soundEffects.Add(Content.Load<SoundEffect>("Sound/hitMark"));
            this.song = Content.Load<Song>("Sound/soundtrack");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;

            soundEffects.Add(Content.Load<SoundEffect>("Sound/hitMark"));

            Restart();
        }

        private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            MediaPlayer.Volume -= 0.1f;
            MediaPlayer.Play(song);
        }

        protected override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            if (_currentKey.IsKeyUp(Keys.M) && _previousKey.IsKeyDown(Keys.M))
            {
                if (MediaPlayer.Volume == 0.0f)
                    MediaPlayer.Volume = 1f;
                else
                    MediaPlayer.Volume = 0f;
            }

            switch (gameState)
            {
                case GameState.Menu:
                    playButton.Update(_currentMouse, _previousMouse);
                    quitButton.Update(_currentMouse, _previousMouse);
                    scoreButton.Update(_currentMouse, _previousMouse);

                    if (playButton.isClicked == true)
                        gameState = GameState.Play;
                    if (quitButton.isClicked == true)
                        gameState = GameState.Quit;
                    if (scoreButton.isClicked == true)
                        gameState = GameState.Score;
                    Restart();
                    break;
                case GameState.Play:
                    #region play
                    if (_currentKey.IsKeyUp(Keys.P) && _previousKey.IsKeyDown(Keys.P))
                    {
                        gameState = GameState.Pause;
                        return;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        hasStarted = true;
                    if (!hasStarted)
                        return;

                    roundTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    nextRoundsTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (rounds % 5 == 0 && roundTimer > RoundTime)
                    {
                        sprites.Add(new Boss(Content.Load<Texture2D>("boss")));
                        rounds++;
                        roundTimer = 0;
                        nextRoundsTimer = RoundTime;
                    }
                    else
                    {
                        if (roundTimer > RoundTime)
                        {
                            for (int i = 0; i < rounds * 2; i++)
                            {
                                sprites.Add(new Enemy(Content.Load<Texture2D>("Mosca")));
                            }
                            roundTimer = 0;
                            rounds++;
                            nextRoundsTimer = RoundTime;
                            foreach (Sprite s in sprites)
                                if (s is Player && rounds != 1)
                                    s.lifepoints += 1;
                        }
                    }

                    enemyMove();

                    BossMove();

                    KillEnemy();

                    KillBoss();

                    PostUpdate();

                    foreach (Sprite sprite in sprites.ToArray())
                        sprite.Update(gameTime, sprites);

                    break;
                #endregion
                case GameState.Pause:
                    menuButton.Update(_currentMouse, _previousMouse);
                    resumeButton.Update(_currentMouse, _previousMouse);


                    if (resumeButton.isClicked == true)
                        gameState = GameState.Play;
                    if ((_currentKey.IsKeyDown(Keys.Escape) && _previousKey.IsKeyDown(Keys.Escape)) || menuButton.isClicked == true)
                        gameState = GameState.Menu;
                    break;
                case GameState.Score:
                    resumeButton.Update(_currentMouse, _previousMouse);

                    if (resumeButton.isClicked == true)
                        gameState = GameState.Menu;
                    break;
                case GameState.Lose:
                    menuButton.Update(_currentMouse, _previousMouse);
                    playButton.Update(_currentMouse, _previousMouse);

                    if (playButton.isClicked == true)
                    {
                        Restart();
                        gameState = GameState.Play;
                    }
                    if ((_currentKey.IsKeyDown(Keys.Escape) && _previousKey.IsKeyDown(Keys.Escape)) || menuButton.isClicked == true)
                    {

                        Restart();
                        gameState = GameState.Menu;
                    }
                    break;
                case GameState.Quit:
                    Exit();
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Menu:
                    spriteBatch.Draw(menuBground, pGround, Color.White);
                    playButton.Draw(spriteBatch);
                    scoreButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    break;
                case GameState.Play:
                    spriteBatch.Draw(backGround, pGround, Color.White);
                    foreach (Sprite sprite in sprites)
                        sprite.Draw(spriteBatch);
                    GameStateDraw();
                    break;
                case GameState.Pause:
                    spriteBatch.Draw(menuBground, pGround, Color.White);
                    GameStateDraw();
                    resumeButton.Draw(spriteBatch);
                    menuButton.Draw(spriteBatch);

                    break;
                case GameState.Score:
                    spriteBatch.Draw(menuBground, pGround, Color.White);
                    resumeButton.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "Highscores:\n" + string.Join("\n", _scoreManager.Highscores.Select(c => c.Value).ToArray()), new Vector2((screenWidth / 2) - 40, 50), Color.Black);
                    break;
                case GameState.Lose:
                    spriteBatch.Draw(menuBground, pGround, Color.Red);
                    GameStateDraw();
                    playButton.Draw(spriteBatch);
                    menuButton.Draw(spriteBatch);

                    break;
                case GameState.Quit:
                    break;
                default:
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #region Funcoes

        private void Restart()
        {
            Texture2D playerTexture = Content.Load<Texture2D>("saitama1");

            sprites = new List<Sprite>()
            {
                new Player(playerTexture)
                {
                    Position = new Vector2(screenWidth/2,screenHeight/2),
                    Weapon = new Weapon(Content.Load<Texture2D>("chinelo")),
                    lifepoints = 10,
                    playerScore = 0,
                }
            };

            rounds = 1;
            roundTimer = 0;
            nextRoundsTimer = 10;

            hasStarted = false;

        }

        private void BossMove()
        {
            foreach (Sprite s2 in sprites)
            {
                if (s2 is Player)
                {
                    foreach (Sprite s1 in sprites)
                    {
                        if (s1 is Boss)
                        {
                            s1.enemyDirection = s2.Position - s1.Position;
                            s1.enemyDirection.Normalize();
                            s1.Position += s1.enemyDirection * s1.enemyVelocity;
                        }
                    }
                }
            }
        }

        private void enemyMove()
        {
            foreach (Sprite s2 in sprites)
            {
                if (s2 is Player)
                {
                    foreach (Sprite s1 in sprites)
                    {
                        if (s1 is Enemy)
                        {
                            s1.enemyDirection = s2.Position - s1.Position;
                            s1.enemyDirection.Normalize();
                            s1.Position += s1.enemyDirection * s1.enemyVelocity;
                        }
                    }
                }
            }
        }

        private void KillBoss()
        {
            foreach (Sprite s2 in sprites)
            {
                if (s2 is Boss)
                {
                    foreach (Sprite s1 in sprites)
                    {
                        if (s1 is Weapon)
                        {
                            if (s1.Rectangle.Intersects(s2.Rectangle))
                            {
                                var instance = soundEffects[0].CreateInstance();
                                instance.IsLooped = false;
                                instance.Play();

                                s2.enemyLifepoints--;
                                s1.isRemoved = true;
                            }
                        }
                    }
                }
            }
        }

        private void KillEnemy()
        {

            foreach (Sprite s2 in sprites)
            {
                if (s2 is Enemy)
                {
                    foreach (Sprite s1 in sprites)
                    {
                        if (s1 is Weapon)
                        {
                            if (s1.Rectangle.Intersects(s2.Rectangle))
                            {
                                var instance = soundEffects[0].CreateInstance();
                                instance.IsLooped = false;
                                instance.Play();

                                s2.enemyLifepoints--;
                                s1.isRemoved = true;
                            }
                        }
                    }
                }
            }
        }

        private void PostUpdate()
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite sprites = this.sprites[i];

                if (this.sprites[i].isRemoved)
                {
                    this.sprites.RemoveAt(i);
                    i--;
                }

                if (sprites is Player)
                {
                    Player player = sprites as Player;

                    if (player.HasDied)
                    {
                        _scoreManager.Add(new Score()
                        {
                            Value = player.playerScore,
                            Rounds = rounds,
                        });

                        ScoreManager.Save(_scoreManager);

                            gameState = GameState.Lose;
                    }
                }
            }
        }

        private void GameStateDraw()
        {
            _fontY = new Vector2(screenWidth - 100, 5);
            foreach (Sprite sprite in sprites)
                if (sprite is Boss)
                    spriteBatch.DrawString(_font, string.Format("Boss Hp: {0}", sprite.enemyLifepoints), new Vector2(screenWidth - 200, _fontY.Y += 30), Color.Yellow, 0, Origin, 2, SpriteEffects.None, 1);

            foreach (Sprite s in sprites)
                if (s is Player)
                {
                    spriteBatch.DrawString(_font, string.Format("Score: {0}", ((Player)s).playerScore), new Vector2(10, 10), Color.Yellow, 0, Origin, 2, SpriteEffects.None, 1);
                    spriteBatch.DrawString(_font, string.Format("Hp: {0}", ((Player)s).lifepoints), new Vector2(10, 40), Color.Yellow, 0, Origin, 2, SpriteEffects.None, 1);
                }

            spriteBatch.DrawString(_font, string.Format("Round: {0}", rounds - 1), new Vector2(10, 70), Color.Yellow, 0, Origin, 2, SpriteEffects.None, 1);
            spriteBatch.DrawString(_font, string.Format("Next Round in {0:0.00}", nextRoundsTimer), new Vector2(10, 100), Color.Yellow, 0, Origin, 2, SpriteEffects.None, 1);

        }
        #endregion
    }
}
