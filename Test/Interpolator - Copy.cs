using Common;
using Common.MenuSystem;
using Common.Misc;
using GameStateEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Test
{
    public abstract class InterpolatorBase<T>
    {
        double _curTime = 0;
        double _duration = 0;
        T _start;
        T _end;
        bool _recalc;
        T _cache;

        public InterpolatorBase(T start, T end, double duration)
        {
            _start = start;
            _end = end;
            _duration = duration;

            _recalc = false;
            _cache = start;
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

        public T Interpolate(double time)
        {
            if (time > _duration)
                time = _duration;

            return Interpolate(_start, _end, time / _duration);
        }
            
        protected abstract T Interpolate(T start, T end, double percent);
    }

    public class FloatInterpolator : InterpolatorBase<float>
    {
        public FloatInterpolator(float start, float end, double duration)
            : base(start, end, duration) { }

        protected override float Interpolate(float start, float end, double percent)
        {
            return MathHelper.Lerp(start, end, (float)percent);
        }
    }

    public class Vector2Interpolator : InterpolatorBase<Vector2>
    {
        public Vector2Interpolator(Vector2 start, Vector2 end, double duration)
            : base(start, end, duration) { }

        public Vector2Interpolator(float startX, float startY, float endX, float endY, double duration)
            : base(new Vector2(startX, startY), new Vector2(endX, endY), duration) { }

        protected override Vector2 Interpolate(Vector2 start, Vector2 end, double percent)
        {
            var x = MathHelper.Lerp(start.X, end.X, (float)percent);
            var y = MathHelper.Lerp(start.Y, end.Y, (float)percent);

            return new Vector2(x, y);
        }
    }

    public class ColorInterpolator : InterpolatorBase<Color>
    {
        public ColorInterpolator(Color start, Color end, double duration) 
            : base(start, end, duration) { }

        protected override Color Interpolate(Color start, Color end, double percent)
        {
            //r = MathHelper.SmoothStep();
            var r = (int)MathHelper.Clamp(MathHelper.Lerp(start.R, end.R, (float)percent), 0, 255);
            var g = (int)MathHelper.Clamp(MathHelper.Lerp(start.G, end.G, (float)percent), 0, 255);
            var b = (int)MathHelper.Clamp(MathHelper.Lerp(start.B, end.B, (float)percent), 0, 255);
            var a = (int)MathHelper.Clamp(MathHelper.Lerp(start.A, end.A, (float)percent), 0, 255);

            return new Color(r, g, b, a);
        }

        public Color InterpolateNonPremultiplied()
        {
            return Color.FromNonPremultiplied(Interpolate().ToVector4()); 
        }

        public Color InterpolateNonPremultiplied(double time)
        {
            return Color.FromNonPremultiplied(Interpolate(time).ToVector4());
        }

        /*
         * FromNonPremultiplied
         * 
        Based on this you should use either  BlendState.NonPremultiplied and Color(12, 34, 56, 78)

        OR

        BlendState.AlphaBlend and Color.FromNonPremultiplied(12, 34, 56, 78)
        */
    }


    public class CurveInterpolator : InterpolatorBase<Vector2>
    {
        Curve _curveX = new Curve();
        Curve _curveY = new Curve();
        CurveKey _endCurveX, _endCurveY;
        double _duration;

        public CurveInterpolator(Vector2 start, Vector2 end, double duration)
            : base(start, end, duration)
        {
            _curveX.PreLoop = _curveX.PostLoop = _CurveLoopType;
            _curveY.PreLoop = _curveY.PostLoop = _CurveLoopType;
            _duration = duration;

            AddPoint(start, 0);
            AddPoint(end, duration);

            _endCurveX = _curveX.Keys[1];
            _endCurveY = _curveY.Keys[1];
        }

        public CurveContinuity CurveContinuity
        {
            get => _CurveContinuity;
            set
            {
                _CurveContinuity = value;

                _curveX.Keys.ToList().ForEach(x => x.Continuity = value);
                _curveY.Keys.ToList().ForEach(y => y.Continuity = value);

                _curveX.ComputeTangents(_CurveTangent);
                _curveY.ComputeTangents(_CurveTangent);
            }
        }
        CurveContinuity _CurveContinuity = CurveContinuity.Smooth;

        public CurveLoopType CurveLoopType
        {
            get => _CurveLoopType;
            set
            {
                _CurveLoopType = value;
                _curveX.PreLoop = _curveX.PostLoop = value;
                _curveX.PreLoop = _curveX.PostLoop = value;

                _curveX.ComputeTangents(_CurveTangent);
                _curveY.ComputeTangents(_CurveTangent);
            }
        }
        CurveLoopType _CurveLoopType = CurveLoopType.Constant;

        public CurveTangent CurveTangent
        {
            get => _CurveTangent;
            set
            {
                _CurveTangent = value;
                _curveX.ComputeTangents(_CurveTangent);
                _curveY.ComputeTangents(_CurveTangent);
            }
        }
        CurveTangent _CurveTangent = CurveTangent.Smooth;


        public void AddPoint(Vector2 point, double duration)
        {
            if ((duration < 0) || (duration > _duration))
                return;

            if (_endCurveX != null)
            {
                _curveX.Keys.RemoveAt(_curveX.Keys.Count - 1);
                _curveY.Keys.RemoveAt(_curveY.Keys.Count - 1);
            }

            _curveX.Keys.Add(new CurveKey((float)duration, point.X, 0, 0, _CurveContinuity));
            _curveY.Keys.Add(new CurveKey((float)duration, point.Y, 0, 0, _CurveContinuity));

            if (_endCurveX != null)
            {
                _curveX.Keys.Add(_endCurveX);
                _curveY.Keys.Add(_endCurveY);
            }

            _curveX.ComputeTangents(_CurveTangent);
            _curveY.ComputeTangents(_CurveTangent);
        }

        protected override Vector2 Interpolate(Vector2 start, Vector2 end, double percent)
        {
            var x = _curveX.Evaluate((float)(_duration * percent));
            var y = _curveY.Evaluate((float)(_duration * percent));

            return new Vector2(x, y);
        }
    }
}
