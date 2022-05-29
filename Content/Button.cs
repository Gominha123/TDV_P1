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
        Rectangle buttonBox;

        Color color = new Color(255, 255, 255, 255);

        public Vector2 size;

        public Button(Texture2D texture, GraphicsDevice graphics)
        {
            _texture = texture;
            size = new Vector2(texture.Width ,texture.Height);
        }

        public bool isClicked;

        public void Update(MouseState mouse,MouseState pmouse)
        {
            buttonBox = new Rectangle((int)_position.X, (int)_position.Y, (int)size.X, (int)size.Y);
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);
            isClicked = false;
            if (mouseRectangle.Intersects(buttonBox))
            {
                if (mouse.LeftButton == ButtonState.Released && pmouse.LeftButton == ButtonState.Pressed ) isClicked = true;
            }
        }

        public void setPosition(Vector2 position)
        {
            _position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,buttonBox, color);

        }
    }
}
