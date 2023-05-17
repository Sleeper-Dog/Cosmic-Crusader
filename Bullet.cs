using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cosmic_Crusader;

public class Bullet
{
    public Vector2 Position;
    public Vector2 Velocity;

    private Texture2D _texture; // Weak Bullet Texture
    private Texture2D _texture2; // Strong Bullet Texture
    
    public int _bulletDamage;

    public Bullet(Vector2 position, float rotation, Texture2D texture, int damage, Texture2D texture2)
    {
        Position = position;
        Velocity = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation)) * 3f;

        _texture = texture;
        _texture2 = texture2;
        _bulletDamage = damage;
    }
    
    public void Update()
    {
        Position += Velocity;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (_bulletDamage > 10)
        {
            spriteBatch.Draw(_texture2, Position, Color.White);
        }
        else
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}