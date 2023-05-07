using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cosmic_Crusader;

public class Bullet
{
    public Vector2 Position;
    public Vector2 Velocity;

    private Texture2D _texture;
    
    public Bullet(Vector2 position, float rotation, Texture2D texture)
    {
        Position = position;
        Velocity = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation)) * 3f;
        
        _texture = texture;
    }
    
    public void Update()
    {
        Position += Velocity;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, Color.White);
    }
}