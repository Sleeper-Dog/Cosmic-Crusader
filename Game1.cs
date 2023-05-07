using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;

namespace Cosmic_Crusader
{
    public enum GameStates
    {
        Start,
        Game,
        GameOver
    }
    
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _playerTexture;
        private Texture2D _enemyTexture;
        public Texture2D BulletTexture;

        private SpriteFont _font;

        private GameStates _gameState = GameStates.Start;

        public Player Player;
        private List<Enemy> _enemies;
        private const float BaseEnemySpawnCooldown = 120;
        private float _enemySpawnCooldown = BaseEnemySpawnCooldown;
        private float _enemiesTimer;
        
        public float EnemyAcceleration = 0.01f;

        public int Score;

        private RenderTarget2D _renderTarget2D;
        private Matrix _scale;
        public float ScaleX;
        public float ScaleY;

        public static int TargetWidth = (int)Width.Quarter;
        public static int TargetHeight = (int)Height.Quarter;

        private int _scoreIncreaseCooldown = 60;
        
        public List<Bullet> Bullets = new();

        private enum Height
        {
            Full = 1080,
            Half = 540,
            Quarter = 270
        }

        private enum Width
        {
            Full = 1920,
            Half = 960,
            Quarter = 480
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = (int)Width.Full;
            _graphics.PreferredBackBufferHeight = (int)Height.Full;

            ScaleX = _graphics.PreferredBackBufferWidth / (float)TargetWidth;
            ScaleY = _graphics.PreferredBackBufferHeight / (float)TargetHeight;
            _scale = Matrix.CreateScale(ScaleX, ScaleY, 1);
        }

        protected override void Initialize()
        {
            _renderTarget2D = new(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _playerTexture = Content.Load<Texture2D>("BlueDartShip");
            _enemyTexture = Content.Load<Texture2D>("bee");
            BulletTexture = Content.Load<Texture2D>("Bullet");
            _font = Content.Load<SpriteFont>("Font");
            
            _enemies = new List<Enemy>();
            Player = new Player(new Vector2(TargetWidth / 2, TargetHeight / 2), _playerTexture, this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_gameState == GameStates.Game)
            {
                GameUpdate(gameTime);
            }
            else if (_gameState == GameStates.GameOver)
            {
                GameOverUpdate();
            }
            else if (_gameState == GameStates.Start)
            {
                StartUpdate();
            }

            base.Update(gameTime);
        }

        private void GameUpdate(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _enemiesTimer--;
            if (_enemiesTimer <= 0 && _enemies.Count <= 4)
            {
                Random rng = new();
                _enemiesTimer = _enemySpawnCooldown;
                
                int side = rng.Next(4);
                int x = 0;
                int y = 0;
                
                if (side == 0)
                {
                    x = rng.Next(TargetWidth);
                    y = 0;
                }
                else if (side == 1)
                {
                    x = TargetWidth;
                    y = rng.Next(TargetHeight);
                }
                else if (side == 2)
                {
                    x = rng.Next(TargetWidth);
                    y = TargetHeight;
                }
                else if (side == 3)
                {
                    x = 0;
                    y = rng.Next(TargetHeight);
                }

                _enemies.Add(new Enemy(new Vector2(x, y), _enemyTexture, this));
            }
            
            // Update all enemies
            foreach (var enemy in _enemies)
            {
                enemy.Update(deltaTime);
            }
            
            // Update all bullets
            foreach (var bullet in Bullets)
            {
                bullet.Update();
            }
            
            // Update player
            Player.Update(deltaTime);

            // Check for bullet collisions
            for (int i = 0; i < Bullets.Count; i++)
            {
                for (int j = 0; j < _enemies.Count; j++)
                {
                    
                    if (_enemies[j].Hitbox.Contains(Bullets[i].Position.ToPoint()))
                    {
                        _enemies[j].HP -= 5;
                        Bullets.RemoveAt(i);
                        if (i > 0) i--;
                        
                        if (Bullets.Count == 0) break;
                    }
                }
            }
            
            // Remove bullets that are outside the screen
            foreach (var bullet in Bullets)
            {
                if (bullet.Position.X < 0 || bullet.Position.X > TargetWidth || bullet.Position.Y < 0 || bullet.Position.Y > TargetHeight)
                {
                    Bullets.Remove(bullet);
                    break;
                }
            }
            
            // Check for enemy collisions and remove dead enemies
            List<Enemy> enemiesToRemove = new();
            foreach (var enemy in _enemies)
            {
                if (enemy.HP <= 0)
                {
                    enemiesToRemove.Add(enemy);
                    continue;
                }
                
                if (enemy.Hitbox.Intersects(Player.Hitbox))
                {
                    Player.HP -= 5;
                    enemiesToRemove.Add(enemy);
                }
            }
            
            foreach (var enemy in enemiesToRemove)
            {
                _enemies.Remove(enemy);
            }

            if (Player.HP <= 0)
            {
                _gameState = GameStates.GameOver;
            }
            
            // Increase score
            _scoreIncreaseCooldown--;
            if (_scoreIncreaseCooldown <= 0)
            {
                Score += 10;
                _scoreIncreaseCooldown = 60;

                EnemyAcceleration += 0.0001f;

                _enemySpawnCooldown = BaseEnemySpawnCooldown * MathF.Pow(0.8f, Score / 100f);
                Debug.WriteLine(_enemySpawnCooldown);
            }
        }

        private void StartUpdate()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _gameState = GameStates.Game;
            }
        }

        private void GameOverUpdate()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _gameState = GameStates.Game;
                
                _enemies.Clear();
                Bullets.Clear();
                Player.HP = Player.MaxHP;
                Score = 0;
                EnemyAcceleration = 0.01f;
                _enemiesTimer = 0;
                _scoreIncreaseCooldown = 60;
                _enemySpawnCooldown = BaseEnemySpawnCooldown;
                Player.Position = new Vector2(TargetWidth / 2, TargetHeight / 2);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget2D);

            GraphicsDevice.Clear(new Color(28, 23, 41));

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap);

            if (_gameState == GameStates.Game)
            {
                GameDraw();
            }
            else if (_gameState == GameStates.GameOver)
            {
                GameOverDraw();
            }
            else if (_gameState == GameStates.Start)
            {
                StartDraw();
            }
            
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null, null, _scale);
            _spriteBatch.Draw(
                _renderTarget2D,
                new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GameDraw()
        {
            // Draw player's HP
            string hp = "HP: " + Player.HP + " / " + Player.MaxHP;
            _spriteBatch.DrawString(_font, hp, new Vector2(10, 10), Color.White);
            
            // Draw player's score
            float textLength = _font.MeasureString(Score.ToString()).X;
            _spriteBatch.DrawString(_font, Score.ToString(), new Vector2(TargetWidth / 2 - MathF.Round(textLength / 2), 10), Color.White);
            
            foreach (var bullet in Bullets)
            {
                bullet.Draw(_spriteBatch);
            }
            
            Player.Draw(_spriteBatch);
            
            foreach (var enemy in _enemies)
            {
                enemy.Draw(_spriteBatch);
            }
        }

        private void GameOverDraw()
        {
            // Draw game over text
            float textLength = _font.MeasureString("Game Over").X;
            _spriteBatch.DrawString(_font, "Game Over", new Vector2(TargetWidth / 2 - MathF.Round(textLength / 2), TargetHeight / 2 - 10), Color.White);
            
            // Draw restart text
            textLength = _font.MeasureString("Press space to restart").X;
            _spriteBatch.DrawString(_font, "Press space to restart", new Vector2(TargetWidth / 2 - MathF.Round(textLength / 2), TargetHeight / 2 + 10), Color.White);
        }

        private void StartDraw()
        {
            // Draw title text
            float textLength = _font.MeasureString("Cosmic Crusader").X;
            _spriteBatch.DrawString(_font, "Cosmic Crusader", new Vector2(TargetWidth / 2 - MathF.Round(textLength / 2), TargetHeight / 2 - 10), Color.White);
            
            // Draw start text
            textLength = _font.MeasureString("Press space to start").X;
            _spriteBatch.DrawString(_font, "Press space to start", new Vector2(TargetWidth / 2 - MathF.Round(textLength / 2), TargetHeight / 2 + 10), Color.White);
        }
    }
}

