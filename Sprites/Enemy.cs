using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace P1_Monogame.Sprites
{
    public class Enemy : Sprite
    {
        public Enemy(Texture2D texture)
            : base(texture)
        {
            randomSpawn = Game1.Random.Next(0, 4);
            if (randomSpawn == 0)
            {
                //spawn esquerda
                Position = new Vector2(-_texture.Width, Game1.Random.Next(0, Game1.screenHeight - _texture.Height));
            }
            if (randomSpawn == 1)
            {
                //spawn direita
                Position = new Vector2(Game1.screenWidth + _texture.Width, Game1.Random.Next(0, Game1.screenHeight - _texture.Height));
            }
            if (randomSpawn == 2)
            {
                //spawn cima
                Position = new Vector2(Game1.Random.Next(0, Game1.screenWidth - _texture.Width), -_texture.Height);
            }
            if (randomSpawn == 3)
            {
                //spawn baixo
                Position = new Vector2(Game1.Random.Next(0, Game1.screenWidth - _texture.Width), Game1.screenHeight + _texture.Height);
            }

            enemyLifepoints = 1;
            enemyVelocity = 1f;
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
                        s.playerScore += 1;
                    }
                }
            }
        }
    }
}