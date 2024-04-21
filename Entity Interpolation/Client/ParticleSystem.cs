using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Client
{
    public class ParticleSystem
    {
        private Dictionary<long, Particle> m_particles = new Dictionary<long, Particle>();
        public Dictionary<long, Particle>.ValueCollection particles { get { return m_particles.Values; } }
        private MyRandom m_random = new MyRandom();

        private int m_sizeMean; // pixels
        private int m_sizeStdDev;   // pixels
        private float m_speedMean;  // pixels per millisecond
        private float m_speedStDev; // pixles per millisecond
        private float m_lifetimeMean; // milliseconds
        private float m_lifetimeStdDev; // milliseconds

        public ParticleSystem(int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, int lifetimeMean, int lifetimeStdDev)
        {
            m_sizeMean = sizeMean;
            m_sizeStdDev = sizeStdDev;
            m_speedMean = speedMean;
            m_speedStDev = speedStdDev;
            m_lifetimeMean = lifetimeMean;
            m_lifetimeStdDev = lifetimeStdDev;
        }

        private Particle create(Vector2 center, Vector2 direction)
        {
            float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
            

            
            var p = new Particle(
                    center,
                    direction,
                    (float)m_random.nextGaussian(m_speedMean, m_speedStDev),
                    new Vector2(size, size),
                    new System.TimeSpan(0, 0, 0, 0, (int)(m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev))));

            return p;
        }

        public void playerDeath(List<Vector2> centers)
        {

            foreach (Vector2 center in centers)
            {
                for (int i = 0; i < 50; i++)
                {
                    var particle = create(center, m_random.nextCircleVector());
                    m_particles.Add(particle.name, particle);
                }
            }
            /*for (int i = 0; i < 12; i++)
            {
                Random random = new Random();
                float offset = (float)Math.PI / 2;
                float angle = (float)(random.NextDouble()) ;
                angle *= offset;
                angle += playerAngle;
                angle += (float)Math.PI / 4;
                float x = (float)Math.Cos(angle);
                float y = (float)Math.Sin(angle);
                var particle = create(center, new Vector2(x,y));
                m_particles.Add(particle.name, particle);
            }*/

        }


        public void foodEaten(Vector2 center)
        {
            // Create a bunch of particles when the ship crashes!

            for (int i = 0; i < 50; i++)
            {
                var particle = create(center, m_random.nextCircleVector());
                m_particles.Add(particle.name, particle);
            }


        }

        

        public void update(GameTime gameTime)
        {
            // Update existing particles
            List<long> removeMe = new List<long>();
            foreach (Particle p in m_particles.Values)
            {
                if (!p.update(gameTime))
                {
                    removeMe.Add(p.name);
                }
            }

            // Remove dead particles
            foreach (long key in removeMe)
            {
                m_particles.Remove(key);
            }

            // Generate some new particles
            
        }
    }
}