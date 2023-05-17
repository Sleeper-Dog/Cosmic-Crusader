using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Cosmic_Crusader
{
    public class Enemy
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public int HP = 30;

        public Rectangle Hitbox;
        
        private const float Friction = 0.3f;
       
        private Texture2D _texture;
        private Game1 _root;
        
        public Enemy(Vector2 position, Texture2D texture, Game1 root)
        {
            Position = position;
            _texture = texture;
            _root = root;

            Hitbox = new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16);
        }

        public void Update(float deltaTime)
        {
            // Rotate towards player
            float xDiff = _root.Player.Position.X - Position.X;
            float yDiff = _root.Player.Position.Y - Position.Y;
            Rotation = MathF.Atan2(yDiff, xDiff);
            
            // Move the enemy towards the player
            Velocity += new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation)) * _root.EnemyAcceleration;
            Position += Velocity;
            
            // Wrap the enemy's position on the x axis
            if (Position.X < 0)
            {
                Position.X = Game1.TargetWidth;
            }
            else if (Position.X > Game1.TargetWidth)
            {
                Position.X = 0;
            }

            // Wrap the enemy's position on the y axis
            if (Position.Y < 0)
            {
                Position.Y = Game1.TargetHeight;
            }
            else if (Position.Y > Game1.TargetHeight)
            {
                Position.Y = 0;
            }
            
            // Move enemy hitbox
            Hitbox.X = (int)Position.X - 8;
            Hitbox.Y = (int)Position.Y - 8;
            
            // Friction
            float sign = Math.Sign(Velocity.Length());
            if (sign != 0)
            {
                float frictionDirection = MathF.Atan2(Velocity.Y, Velocity.X);
                Velocity -= new Vector2(MathF.Cos(frictionDirection), MathF.Sin(frictionDirection)) * sign * Friction * deltaTime;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, Rotation, new Vector2(8, 8), new Vector2(1, 1), SpriteEffects.None, 0);
        }
    }
}