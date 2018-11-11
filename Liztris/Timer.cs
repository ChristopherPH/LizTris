﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liztris
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

        public void Reset()
        {
            value = 0;
        }
    }
}
