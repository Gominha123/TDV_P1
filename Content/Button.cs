using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace P1_Monogame.Content
{
    public class Button
    {
        Texture2D _texture;
        Vector2 _position;
        Rectangle box;

        Color color = new Color(255, 255, 255, 255);

        public Vector2 size;

        public Button(Texture2D texture, GraphicsDevice graphics)
        {
            _texture = texture;
            size = new Vector2(texture.Width/3 ,texture.Height/4);
        }

       // bool down;
        public bool isClicked;

        public void Update(MouseState mouse)
        {
            box = new Rectangle((int)_position.X, (int)_position.Y, (int)size.X, (int)size.Y);
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(box))
            {
                if (mouse.LeftButton == ButtonState.Pressed) isClicked = true;
            }
        }

        public void setPosition(Vector2 position)
        {
            _position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,box, color);

        }
    }
}
