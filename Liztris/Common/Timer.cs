using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Timer
    {
        double value = 0;
        public double Delay { get; private set; } = 0;

        public Timer() { }

        public Timer(double Delay)
        {
            SetDelay(Delay);
        }

        public void SetDelay(double Delay)
        {
            this.Delay = Delay;
            Reset();
        }

        public bool UpdateAndCheck(GameTime gameTime)
        {
            if (Delay <= 0)
                return false;

            value += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (value < Delay)
                return false;

            value -= Delay;

            return true;
        }

        public void UpdateAndCheck(GameTime gameTime, 
            Action action, int MaximumUpdates = 5)
        {
            if (Delay <= 0)
                return;

            value += gameTime.ElapsedGameTime.TotalMilliseconds;

            int counter = 0;

            while ((value > Delay) && 
                (MaximumUpdates > 0) && (counter < MaximumUpdates))
            {
                value -= Delay;

                if (action != null)
                    action();
                counter++;
            }
        }

        public void Reset()
        {
            value = 0;
        }
    }
}
