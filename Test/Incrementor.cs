using Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public interface Incrementor
    {
        void Update(double Units);
    }

    public abstract class Incrementor<T, U> : Incrementor
    {
        public Incrementor(T StartValue, T EndValue, U TotalUnits, U UpdateUnits)
        {
            this.StartValue = StartValue;
            this.EndValue = EndValue;
            this.TotalUnits = TotalUnits;
            this.UpdateUnits = UpdateUnits;

            Reset();
        }

        public virtual void Reset()
        {
            CurrentValue = StartValue;
            CurrentUnits = default(U);
        }

        public T StartValue { get; private set; }
        public T EndValue { get; private set; }
        public T CurrentValue { get; protected set; }

        public U TotalUnits { get; private set; }
        public U UpdateUnits { get; private set; }
        public U CurrentUnits { get; private set; } = default(U);

        protected abstract T GetCurrentValue(T StartValue, T EndValue, double Percent);
        protected abstract T GetCurrentSteps(T CurrentUnits, T EndValue, double Percent);

        public void Update(double Units)
        {
            while (CurrentUnits + UpdateUnits < TotalUnits)
                CurrentUnits += UpdateUnits;

            CurrentValue = GetCurrentValue(StartValue, EndValue, TotalUnits / CurrentUnits);
        }
    }
    
    public class IntIncrementor : Incrementor<int>
    {
        public IntIncrementor(int StartValue, int EndValue, double TotalUnits, double UpdateUnits = 1)
            : base(StartValue, EndValue, TotalUnits, UpdateUnits) { }

        protected override int GetCurrentValue(int StartValue, int EndValue, double Percent)
        {
            return (int)((EndValue - StartValue) * Percent) + StartValue;
        }
    }

    public class ColorIncrementor
    {
        public ColorIncrementor(Color StartColor, Color EndColor, double DurationMS, double UpdateMS)
        {
            A = new IntIncrementor(StartColor.A, EndColor.A, DurationMS, UpdateMS);
            R = new IntIncrementor(StartColor.R, EndColor.R, DurationMS, UpdateMS);
            G = new IntIncrementor(StartColor.G, EndColor.G, DurationMS, UpdateMS);
            B = new IntIncrementor(StartColor.B, EndColor.B, DurationMS, UpdateMS);

            curve = new Curve();
            curve.Keys.Add(new CurveKey(0, StartColor.A));
            curve.Keys.Add(new CurveKey((float)DurationMS, EndColor.A));
            curve.PreLoop = curve.PostLoop = CurveLoopType.Constant;
        }

        Curve curve;

        public void Update(GameTime time)
        {
            A.Update(time.ElapsedGameTime);
            R.Update(time.ElapsedGameTime);
            G.Update(time.ElapsedGameTime);
            B.Update(time.ElapsedGameTime);

            CurrentValue = Color.FromNonPremultiplied(
                R.CurrentValue, G.CurrentValue, B.CurrentValue, A.CurrentValue);

            var rc = curve.Evaluate((float)A.CurrentMS);

            System.Diagnostics.Debug.Print("{0} {1}", A.CurrentValue, rc);
        }

        IntIncrementor A, R, G, B;

        public Color CurrentValue { get; private set; }
    }
}
