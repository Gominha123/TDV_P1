using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace P1_Monogame.Sprites
{
    public class Weapon : Sprite
    {
        private float _timer;

        public Weapon(Texture2D texture)
            : base(texture)
        {

        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > weaponLifeSpan)
                isRemoved = true;

            //if (weaponDurability == 0)
            //    isRemoved = true;

            Position += weaponDirection * weaponVelocity;
        }
    }
}
