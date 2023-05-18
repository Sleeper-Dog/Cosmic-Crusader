using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cosmic_Crusader;

public class Player
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Rotation;
    public int HP = 100;
    public const int MaxHP = 100;

    private float Acceleration = 5f;
    private float Friction = 0.3f;
    private const float MaxSpeed = 1.4f;
    private const float BoostSpeed = 2.1f;
    private bool IsBoosting;

    private int _shotCooldown;

    public Rectangle Hitbox;

    private Texture2D _texture;
    private Game1 _root;

    public Player(Vector2 position, Texture2D texture, Game1 root)
    {
        Position = position;
        _texture = texture;
        _root = root;

        Hitbox = new Rectangle((int)Position.X - 12, (int)Position.Y - 12, 24, 24);
    }

    public void Update(float deltaTime)
    {
        if (IsBoosting)
        {
            // Move the player along its velocity
            if (Velocity.Length() > BoostSpeed)
            {
                Velocity.Normalize();
                Velocity *= BoostSpeed;
            }
        }
        else
        {
            // Move the player along its velocity
            if (Velocity.Length() > MaxSpeed)
            {
                Velocity.Normalize();
                Velocity *= MaxSpeed;
            }

        }
        Position += Velocity;

        // Wrap the player's position on the x axis
        if (Position.X < 0)
        {
            Position.X = Game1.TargetWidth;
        }
        else if (Position.X > Game1.TargetWidth)
        {
            Position.X = 0;
        }

        // Wrap the player's position on the y axis
        if (Position.Y < 0)
        {
            Position.Y = Game1.TargetHeight;
        }
        else if (Position.Y > Game1.TargetHeight)
        {
            Position.Y = 0;
        }

        // Handle player inputs
        KeyboardState keyboardState = Keyboard.GetState();

        // Acceleration and deceleration and boost
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            IsBoosting = true;
            Acceleration = 12f;
            Friction = 0.6f;
        }
        else
        {
            Acceleration = 4f;
            IsBoosting = false;
            Friction = 0.3f;
        }

        if (keyboardState.IsKeyDown(Keys.W))
        {
            Velocity += new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation)) * Acceleration * deltaTime;
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            Velocity -= new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation)) * Acceleration * deltaTime;
        }

        // Rotate the player towards the cursor
        Vector2 mousePos = Mouse.GetState().Position.ToVector2();

        mousePos.X /= _root.ScaleX;
        mousePos.Y /= _root.ScaleY;

        float xDiff = mousePos.X - Position.X;
        float yDiff = mousePos.Y - Position.Y;

        Rotation = MathF.Atan2(yDiff, xDiff);

        // Shoot bullets
        if (Mouse.GetState().LeftButton == ButtonState.Pressed && !IsBoosting) // Weak but fast
        {
            if (_shotCooldown <= 0)
            {
                _root.laserSound.Play(1f, 1f, 0f);
                _root.Bullets.Add(new Bullet(Position, Rotation, _root.BulletTexture, 10, _root.BulletTexture2));
                _shotCooldown = 23;
            }
        }
        else if (Mouse.GetState().RightButton == ButtonState.Pressed && !IsBoosting) // Strong but slow
        {
            if (_shotCooldown <= 0)
            {
                _root.laserSound.Play();
                _root.Bullets.Add(new Bullet(Position, Rotation, _root.BulletTexture, 15, _root.BulletTexture2));
                _shotCooldown = 32;
            }
        }

        _shotCooldown -= 1;

        // Move player hitbox
        Hitbox.X = (int)Position.X - 12;
        Hitbox.Y = (int)Position.Y - 12;

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
        spriteBatch.Draw(_texture, Position, null, Color.White, Rotation, new Vector2(16, 16), new Vector2(1, 1), SpriteEffects.None, 0);
    }
}