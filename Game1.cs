using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P1_Monogame.Content;
using P1_Monogame.Sprites;
using System;
using System.Collections.Generic;

namespace P1_Monogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont _font;
        private Vector2 _fontY;
        public static Random Random;
        private Button button;
        private Texture2D buttonTexture;
        private KeyboardState _currentKey;
        private KeyboardState _previousKey;

        public int Score;

        private Texture2D backGround;
        private Rectangle pGround;
        private Texture2D menuBground;

        public static int screenWidth;
        public static int screenHeight;


        private List<Sprite> sprites;
        public bool hasStarted = false;
        private float lifeTimerE;
        private float lifeTimerB;
        private int rounds;
        private float roundTimer;
        private float nextRoundsTimer;
        private int RoundTime = 10;

        enum GameState
        {
            Menu,
            Play,
            Pause,
            Quit,
        }

        GameState gameState = GameState.Menu;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

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

            buttonTexture = Content.Load<Texture2D>("Button");
            button = new Button(buttonTexture, graphics.GraphicsDevice);
            button.setPosition(new Vector2((screenWidth / 2) - (buttonTexture.Width / 5), 150));

            pGround = new Rectangle(0, 0, screenWidth, screenHeight);

            Restart();
        }

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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouse = Mouse.GetState();

            switch (gameState)
            {
                case GameState.Menu:
                    if (button.isClicked == true)
                        gameState = GameState.Play;
                    button.Update(mouse);
                    break;
                case GameState.Play:
                    _previousKey = _currentKey;
                    _currentKey = Keyboard.GetState();
                    if (_currentKey.IsKeyUp(Keys.P) && _previousKey.IsKeyDown(Keys.P))
                        gameState = GameState.Pause;
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        hasStarted = true;
                    if (gameState != GameState.Play)
                        return;
                    if (!hasStarted)
                        return;

                    lifeTimerE += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    lifeTimerB += (float)gameTime.ElapsedGameTime.TotalSeconds;
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

                    DmgReception();

                    enemyMove();

                    BossMove();

                    KillEnemy();

                    KillBoss();

                    PostUpdate();

                    foreach (Sprite sprite in sprites.ToArray())
                        sprite.Update(gameTime, sprites);
                    break;
                case GameState.Pause:
                    _previousKey = _currentKey;
                    _currentKey = Keyboard.GetState();
                    if (_currentKey.IsKeyUp(Keys.P) && _previousKey.IsKeyDown(Keys.P))
                        gameState = GameState.Play;
                    if (_currentKey.IsKeyDown(Keys.Escape) && _previousKey.IsKeyDown(Keys.Escape))
                        gameState = GameState.Menu;
                    break;
                case GameState.Quit:
                    Exit();
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
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
            //foreach(Sprite s in sprites)

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
                                s2.enemyLifepoints--;
                                s1.isRemoved = true;
                            }
                        }
                    }
                }
            }
        }

        private void DmgReception()
        {

            foreach (Sprite s2 in sprites)
            {
                if (s2 is Player)
                {
                    foreach (Sprite s1 in sprites)
                    {
                        if (s1 is Enemy)
                        {
                            if (s1.Rectangle.Intersects(s2.Rectangle))
                            {
                                if (lifeTimerE > 1f)
                                {
                                    lifeTimerE = 0;
                                    s2.lifepoints -= 2;
                                }
                            }
                        }
                    }
                }
            }

            foreach (Sprite s2 in sprites)
            {
                if (s2 is Player)
                {
                    foreach (Sprite s1 in sprites)
                    {
                        if (s1 is Boss)
                        {
                            if (s1.Rectangle.Intersects(s2.Rectangle))
                            {
                                if (lifeTimerB > 3f)
                                {
                                    lifeTimerB = 0;
                                    s2.lifepoints -= 10;
                                }
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

                        Restart();
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Menu:
                    spriteBatch.Draw(menuBground, pGround, Color.White);
                    button.Draw(spriteBatch);
                    break;
                case GameState.Play:
                    spriteBatch.Draw(backGround, pGround, Color.White);

                    foreach (Sprite sprite in sprites)
                        sprite.Draw(spriteBatch);

                    _fontY = new Vector2(screenWidth - 100, 10);
                    foreach (Sprite sprite in sprites)
                        if (sprite is Boss)
                            spriteBatch.DrawString(_font, string.Format("Boss Hp: {0}", sprite.enemyLifepoints), new Vector2(screenWidth - 100, _fontY.Y += 20), Color.Yellow);

                    foreach (Sprite s in sprites)
                        if (s is Player)
                        {
                            spriteBatch.DrawString(_font, string.Format("Score: {0}", ((Player)s).playerScore), new Vector2(10, 10), Color.Yellow);
                            spriteBatch.DrawString(_font, string.Format("Hp: {0}", ((Player)s).lifepoints), new Vector2(10, 30), Color.Yellow);
                        }

                    spriteBatch.DrawString(_font, string.Format("Round: {0}", rounds - 1), new Vector2(10, 50), Color.Yellow);
                    spriteBatch.DrawString(_font, string.Format("Next Round in {0:0.00}", nextRoundsTimer), new Vector2(10, 70), Color.Yellow);
                    break;
                case GameState.Pause:
                    spriteBatch.Draw(menuBground, pGround, Color.White);

                    spriteBatch.DrawString(_font, string.Format("Next Round in {0:0.00}", nextRoundsTimer), new Vector2(10, 70), Color.Yellow); _fontY = new Vector2(screenWidth - 100, 10);
                    foreach (Sprite sprite in sprites)
                        if (sprite is Boss)
                            spriteBatch.DrawString(_font, string.Format("Boss Hp: {0}", sprite.enemyLifepoints), new Vector2(screenWidth - 100, _fontY.Y += 20), Color.Yellow);

                    foreach (Sprite s in sprites)
                        if (s is Player)
                        {
                            spriteBatch.DrawString(_font, string.Format("Score: {0}", ((Player)s).playerScore), new Vector2(10, 10), Color.Yellow);
                            spriteBatch.DrawString(_font, string.Format("Hp: {0}", ((Player)s).lifepoints), new Vector2(10, 30), Color.Yellow);
                        }

                    spriteBatch.DrawString(_font, string.Format("Round: {0}", rounds - 1), new Vector2(10, 50), Color.Yellow);
                    spriteBatch.DrawString(_font, string.Format("Next Round in {0:0.00}", nextRoundsTimer), new Vector2(10, 70), Color.Yellow);
                    break;
                case GameState.Quit:
                    break;
                default:
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
