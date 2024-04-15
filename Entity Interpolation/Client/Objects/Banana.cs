using Microsoft.Xna.Framework;

namespace Client.Objects
{
    public class Banana : AnimatedSprite
    {
        private readonly double m_moveRate;
        private readonly double m_rotateRate;

        public Banana(Vector2 size, Vector2 center, double moveRate, double rotateRate) : base(size, center)
        {
            m_moveRate = moveRate;
            m_rotateRate = rotateRate;
        }

        public void moveForward(GameTime gameTime)
        {
            //
            // Create a normalized direction vector
            double vectorX = System.Math.Cos(m_rotation);
            double vectorY = System.Math.Sin(m_rotation);
            //
            // With the normalized direction vector, move the center of the sprite
            m_center.X += (float)(vectorX * m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
            m_center.Y += (float)(vectorY * m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        public void rotateLeft(GameTime gameTime)
        {
            m_rotation -= (float)(m_rotateRate * gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        public void rotateRight(GameTime gameTime)
        {
            m_rotation += (float)(m_rotateRate * gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}
