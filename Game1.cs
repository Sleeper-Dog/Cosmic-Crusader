using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace Cosmic_Crusader
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D playerTexture;
        Vector2 playerPosition;
        float playerSpeed;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            playerPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,_graphics.PreferredBackBufferHeight / 2);
            playerSpeed = 100f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("BlueDartShip");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
            {
                playerPosition.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                playerPosition.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.A))
            {
                playerPosition.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                playerPosition.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (playerPosition.X > _graphics.PreferredBackBufferWidth - playerTexture.Width / 2)
            {
                playerPosition.X = _graphics.PreferredBackBufferWidth - playerTexture.Width / 2;
            }
            else if (playerPosition.X < playerTexture.Width / 2)
            {
                playerPosition.X = playerTexture.Width / 2;
            }

            if (playerPosition.Y > _graphics.PreferredBackBufferHeight - playerTexture.Height / 2)
            {
                playerPosition.Y = _graphics.PreferredBackBufferHeight - playerTexture.Height / 2;
            }
            else if (playerPosition.Y < playerTexture.Height / 2)
            {
                playerPosition.Y = playerTexture.Height / 2;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);


            _spriteBatch.Begin();

            _spriteBatch.Draw(
                playerTexture, 
                playerPosition,
                null,
                Color.White,
                0f,
                new Vector2(playerTexture.Width/2, playerTexture.Height/2),
                Vector2.One,
                SpriteEffects.None,
                0f
                );

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}