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
        double delay = 0;

        public Timer() { }

        public Timer(double Delay)
        {
            SetDelay(Delay);
        }

        public void SetDelay(double Delay)
        {
            delay = Delay;
            Reset();
        }

        public bool UpdateAndCheck(GameTime gameTime)
        {
            if (delay <= 0)
                return false;

            value += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (value < delay)
                return false;

            value -= delay;

            return true;
        }

        public void UpdateAndCheck(GameTime gameTime, 
            Action action, int MaximumUpdates = 5)
        {
            if (delay <= 0)
                return;

            value += gameTime.ElapsedGameTime.TotalMilliseconds;

            int counter = 0;

            while ((value > delay) && 
                (MaximumUpdates > 0) && (counter < MaximumUpdates))
            {
                value -= delay;

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
