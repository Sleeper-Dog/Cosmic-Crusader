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
    internal class Explosion
    {
        Texture2D SpriteSheet;
        int TotalFramesX;
        int TotalFramesY;
        int Delay;
        Vector2 Postion;
        Rectangle Destination;
        Rectangle Source;
        public bool isAlive = true;
        int Time;


    }
}
