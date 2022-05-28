using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace P1_Monogame.Sprites
{
    public class Sprite : ICloneable
    {
        protected Texture2D _texture;

        protected float _rotation;
        protected MouseState _currentKey;
        protected MouseState _previousKey;

        public Vector2 Position;
        public Vector2 Origin;

        public Vector2 enemyDirection;
        public float enemyVelocity;
        public int randomSpawn;
        public float Speed;

        public Vector2 playerDirection;
        public float playerScore;
        
        public Vector2 distance;

        public Vector2 weaponDirection;
        public float weaponVelocity = 10f;
        public int weaponDurability;

        public float linearVelocity = 5f;

       // public Sprite Parent;

        public float weaponLifeSpan;
        public int lifepoints;
        public int enemyLifepoints;
        public bool isRemoved = false;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width/2, _texture.Height/2);
            }
        }


        public Sprite(Texture2D texture)
        {
            _texture = texture;
            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
        }

        public virtual void Update(GameTime gameTime, List<Sprite> sprites)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, 0, Origin, 1, SpriteEffects.None, 0);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
