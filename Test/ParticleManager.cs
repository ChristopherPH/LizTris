using Common;
using Common.MenuSystem;
using Common.Misc;
using Common.Interpolator;
using GameStateEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ParticleManager
    {
        private const int ForceRemoveTicks = 500;

        private class Particle
        {
            public bool Alive { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
            public Color Color { get; set; }
            public float Size { get; set; }
            public int TicksToLive { get; set; }

            public bool Fade { get; set; }

            public int Ticks { get; set; }
        }

        private List<Particle> Particles = new List<Particle>();

        public void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch)
        {
            foreach (var particle in Particles)
            {
                if (!particle.Alive)
                    continue;

                Color c = particle.Color;
                if (particle.Fade)
                {
                    int alpha = 255 * (100 - (particle.Ticks * 100 / particle.TicksToLive)) / 100;
                    c = Color.FromNonPremultiplied(particle.Color.R,
                        particle.Color.G, particle.Color.B, alpha);
                }

                spriteBatch.DrawPixel(particle.Position, c, particle.Size);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var particle in Particles)
            {
                particle.Ticks++;

                if (!particle.Alive)
                    continue;

                particle.Position += particle.Velocity;

                if (particle.Ticks >= particle.TicksToLive)
                    particle.Alive = false;
            }

            Particles.RemoveAll(x => !x.Alive && x.Ticks > ForceRemoveTicks);
        }

        public void ClearParticles()
        {
            Particles.Clear();
        }

        public void AddParticle(Vector2 Position, Vector2 Velocity, Color Color, float Size,
            int TicksToLive, bool Fade)
        {
            foreach (var particle in Particles)
            {
                if (!particle.Alive)
                {
                    particle.Position = Position;
                    particle.Velocity = Velocity;
                    particle.Color = Color;
                    particle.Size = Size;
                    particle.TicksToLive = TicksToLive;
                    particle.Fade = Fade;

                    particle.Alive = true;
                    particle.Ticks = 0;
                    return;
                }
            }

            Particles.Add(new Particle()
            {
                Position = Position,
                Velocity = Velocity,
                Color = Color,
                Size = Size,
                TicksToLive = TicksToLive,
                Fade = Fade,

                Alive = true,
                Ticks = 0,
            });
        }
    }
}
