using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Text;

namespace Cosmic_Crusader
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _playerTexture;
        private Texture2D _enemyTexture;

        private Vector2 _playerPosition;
        private float _acceleration = 0.10f;
        private Vector2 _velocity = Vector2.Zero;
        private float _playerMaxSpeed = 1.4f;
        private const float Friction = 0.035f;
        private float _playerRotation = 0f;

        private List<Enemiies> enemies;
        private int _enemiesTimer = 120;

        private RenderTarget2D _renderTarget2D;
        private Matrix _scale;
        private float _scaleX;
        private float _scaleY;

        private const int _targetWidth = (int)Width.Quarter;
        private const int _targetHeight = (int)Height.Quarter;

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

            _scaleX = _graphics.PreferredBackBufferWidth / (float)_targetWidth;
            _scaleY = _graphics.PreferredBackBufferHeight / (float)_targetHeight;
            _scale = Matrix.CreateScale(_scaleX, _scaleY, 1);
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

            enemies = new List<Enemiies>();
            

            _playerPosition = new Vector2(_targetWidth / 2, _targetHeight / 2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _playerTexture = Content.Load<Texture2D>("BlueDartShip");
            _enemyTexture = Content.Load<Texture2D>("bee");
        }

        protected override void Update(GameTime gameTime)
        {
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();

            mousePos.X /= _scaleX;
            mousePos.Y /= _scaleY;

            float xDiff = mousePos.X - _playerPosition.X;
            float yDiff = mousePos.Y - _playerPosition.Y;

            _playerRotation = MathF.Atan2(yDiff, xDiff);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
            {
                _velocity += new Vector2(MathF.Cos(_playerRotation), MathF.Sin(_playerRotation)) * _acceleration;
            }

            // apply friction
            float sign = Math.Sign(_velocity.Length());
            if (sign != 0)
            {
                float frictionDirection = MathF.Atan2(_velocity.Y, _velocity.X);

                _velocity -= new Vector2(MathF.Cos(frictionDirection), MathF.Sin(frictionDirection)) * sign * Friction;
            }

            // Clamp the velocity to the maximum speed
            if (_velocity.Length() > _playerMaxSpeed)
            {
                _velocity.Normalize();
                _velocity *= _playerMaxSpeed;
            }

            // Update the player's position based on the velocity
            _playerPosition += _velocity;

            // Wrap player position around the screen
            if (_playerPosition.X > _targetWidth - _playerTexture.Width / 2)
            {
                _playerPosition.X =  _playerTexture.Width / 2;
            }
            else if (_playerPosition.X < _playerTexture.Width / 2)
            {
                _playerPosition.X = _targetWidth - _playerTexture.Width / 2;
            }

            if (_playerPosition.Y > _targetHeight)
            {
                _playerPosition.Y = 0;
            }
            else if (_playerPosition.Y < 0)
            {
                _playerPosition.Y = _targetHeight;
            }
            // Clamp the velocity to the maximum speed
            _velocity.Y = MathHelper.Clamp(_velocity.Y, -_playerMaxSpeed, _playerMaxSpeed);

            // Update the player's position based on the velocity
            _playerPosition += _velocity;

            if (_playerRotation < -MathHelper.Pi)
            {
                _playerRotation += MathHelper.TwoPi;
            }
            else if (_playerRotation > MathHelper.Pi)
            {
                _playerRotation -= MathHelper.TwoPi;
            }

            if (_playerPosition.X > _targetWidth)
            {
                _playerPosition.X = 0;
            }
            else if (_playerPosition.X < 0)
            {
                _playerPosition.X = _targetWidth;
            }

            if (_playerPosition.Y > _targetHeight)
            {
                _playerPosition.Y = 0;
            }
            else if (_playerPosition.Y < 0)
            {
                _playerPosition.Y = _targetHeight;
            }

            _enemiesTimer--;
            if (_enemiesTimer <= 0 && enemies.Count <= 4)
            {
                Random rng = new Random();
                _enemiesTimer = 120;
                int side = rng.Next(1, 9);
                enemies.Add(new Enemiies(new Vector2(rng.Next(_targetWidth), -50), side, MathHelper.ToRadians(0), side));
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                //enemies[i] = enemies[i] + new Enemiies(-2, 0);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget2D);

            GraphicsDevice.Clear(new Color(28, 23, 41));

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap);

            _spriteBatch.Draw(
                _playerTexture,
                _playerPosition,
                null,
                Color.White,
                _playerRotation,
                new Vector2(_playerTexture.Width / 2, _playerTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
                );

            foreach (var enemy in enemies)
            {
                _spriteBatch.Draw(_enemyTexture, enemy.Position, Color.White);
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
    }
}

