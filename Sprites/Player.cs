using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace P1_Monogame.Sprites
{
    public class Player : Sprite
    {
        public Weapon Weapon;
        public Enemy Enemy;

        public bool HasDied = false;
        public Vector2 newposition;
        public float weaponTimer;

        public Player(Texture2D texture)
            : base(texture)
        {
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            newposition = Position;
            _previousKey = _currentKey;
            _currentKey = Mouse.GetState();

            weaponTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


            MouseState mouse = Mouse.GetState();

            distance.X = mouse.X - Position.X;
            distance.Y = mouse.Y - Position.Y;

            _rotation = (float)Math.Atan2(distance.Y, distance.X);

            weaponDirection = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));

            playerDirection = new Vector2(0, 0);

            Move();

            Die(sprites);

            newposition += playerDirection * linearVelocity;

            if (newposition.X > Game1.screenWidth - Rectangle.Width / 2 || newposition.X < 0 + Rectangle.Width / 2)
            {
                return;
            }
            if (newposition.Y > Game1.screenHeight - Rectangle.Height / 2 || newposition.Y < 0 + Rectangle.Height / 2)
            {
                return;
            }

            Position = newposition;

            if (_currentKey.LeftButton == ButtonState.Pressed && _previousKey.LeftButton == ButtonState.Released)
            {
                AddWeapon(sprites);
            }

        }

        private void Die(List<Sprite> sprites)
        {
            foreach (Sprite sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if (lifepoints <= 0)
                {
                    this.HasDied = true;
                }
            }
        }

        private void Move()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                playerDirection.Y += -1;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                playerDirection.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                playerDirection.X += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                playerDirection.X += -1;

        }

        private void AddWeapon(List<Sprite> sprites)
        {
            Weapon weapon = Weapon.Clone() as Weapon;
            weapon.weaponDirection = this.weaponDirection;
            weapon.Position = this.Position;
            weapon.weaponVelocity = this.weaponVelocity;
            weapon.weaponLifeSpan = .5f;
            //weapon.Parent = this;

            sprites.Add(weapon);
        }
    }
}