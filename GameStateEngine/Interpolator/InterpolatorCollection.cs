using System.Collections.Generic;

namespace Common.Interpolator
{
    public abstract class InterpolatorCollectionBase : IInterpolator
    {
        List<IInterpolator> _Interpolators = new List<IInterpolator>();

        protected void Add(IInterpolator Interpolator)
        {
            _Interpolators.Add(Interpolator);
        }

        protected void AddRange(IEnumerable<IInterpolator> Interpolators)
        {
            _Interpolators.AddRange(Interpolators);
        }

        protected void Clear()
        {
            _Interpolators.Clear();
        }

        public void Reset()
        {
            _Interpolators.ForEach(x => x.Reset());
        }

        public void Update(double time)
        {
            _Interpolators.ForEach(x => x.Update(time));
        }
    }


    public class InterpolatorCollection : InterpolatorCollectionBase
    {
        public InterpolatorCollection() { }

        public InterpolatorCollection(IInterpolator Interpolator)
        {
            Add(Interpolator);
        }

        public InterpolatorCollection(IEnumerable<IInterpolator> Interpolators)
        {
            AddRange(Interpolators);
        }
    }

    public class MultiFloatInterpolator : InterpolatorCollectionBase
    {
        FloatInterpolator _f1;
        FloatInterpolator _f2;

        public float f1 => _f1.Interpolate();
        public float f2 => _f2.Interpolate();

        public MultiFloatInterpolator(float s1, float e1, float s2, float e2, double duration)
        {
            _f1 = new FloatInterpolator(s1, e1, duration);
            _f2 = new FloatInterpolator(s2, e2, duration);
            Add(_f1);
            Add(_f2);
        }
    }
}
