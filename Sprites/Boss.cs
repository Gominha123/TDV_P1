using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace P1_Monogame.Sprites
{
    public class Boss : Sprite
    {
        public Boss(Texture2D texture)
           : base(texture)
        {
            Position = new Vector2(Game1.Random.Next(0, Game1.screenWidth - _texture.Width), -_texture.Height);
            enemyLifepoints = 50;
            enemyVelocity = .5f;
        }


        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (enemyLifepoints == 0)
            {
                this.isRemoved = true;

                foreach (Sprite s in sprites)
                {
                    if (s is Player)
                    {
                        s.playerScore += 50;
                    }
                }
            }

        }
    }
}
