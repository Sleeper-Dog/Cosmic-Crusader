using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmic_Crusader
{
    internal class Enemiies
    {
        public Vector2 Position;
        public int Side;
        public float Rotation;
        public int EnemyMaxHP = 100;
        public int enemyHP = 10;
        
        public Rectangle EnemyHitBox;
       
        public Enemiies(Vector2 position, int side, float rotation, float enemyDirection)
        {
            Position = position;
            Side = side;
            Rotation = rotation;
            EnemyHitBox = new Rectangle((int)Position.X, (int)Position.Y, 75, 75);
        }
    }
}