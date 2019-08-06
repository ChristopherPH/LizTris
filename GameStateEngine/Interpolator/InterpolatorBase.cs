namespace Common.Interpolator
{
    public interface IInterpolator
    {
        void Update(double time);
        void Reset();
    }

    /// <summary>
    /// Interpolates between a start and end value (or values) based on a
    /// time duration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InterpolatorBase<T> : IInterpolator 
        where T : struct
    {
        double _duration = 0;
        T _start;
        T _end;

        double _curTime = 0;
        bool _recalc;
        T _cache;

        public InterpolatorBase(T start, T end, double duration)
        {
            _duration = duration;
            _start = start;
            _end = end;

            Reset();
        }

        public virtual void Reset()
        {
            _curTime = 0;
            _recalc = false;
            _cache = _start;
        }

        public void Update(double time)
        {
            _curTime += time;
            if (_curTime > _duration)
                _curTime = _duration;

            _recalc = true;
        }

        public T Interpolate()
        {
            if (_recalc)
            {
                _cache = Interpolate(_start, _end, _curTime / _duration);
                _recalc = false;
            }

            return _cache;
        }

        public T InterpolateAt(double time)
        {
            if (time > _duration)
                time = _duration;

            return Interpolate(_start, _end, time / _duration);
        }
            
        protected abstract T Interpolate(T start, T end, double percent);

        public override string ToString()
        {
            return string.Format("{0}   From:{1}  To:{2}  In:{3}",
                _cache, _start, _end, _duration);
        }
    }
}
