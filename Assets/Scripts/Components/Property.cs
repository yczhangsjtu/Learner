using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;

namespace Components {
    public interface Property {}
    
    public abstract class Property<T> : Tween<T>, Property {
        public readonly float duration;
        public readonly float endTime;

        public readonly float startTime;

        public Property(
            T begin,
            T end,
            float startTime,
            float endTime
        ) : base(begin, end) {
            D.assert(startTime < endTime);
            D.assert(startTime >= 0.0f);
            this.startTime = startTime;
            this.endTime = endTime;
            duration = endTime - startTime;
        }

        public T evaluate(float t) {
            if (t <= startTime) return begin;

            if (t >= endTime) return end;

            return lerp((t - startTime) / duration);
        }
    }

    public class ColorProperty : Property<Color> {
        public ColorProperty(float startTime, float endTime, Color begin, Color end) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end) { }

        public override Color lerp(float t) {
            return Color.lerp(begin, end, t);
        }
    }

    public class SizeProperty : Property<Size> {
        public SizeProperty(float startTime, float endTime, Size begin, Size end) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end) { }

        public override Size lerp(float t) {
            return Size.lerp(begin, end, t);
        }
    }

    public class RectProperty : Property<Rect> {
        public RectProperty(float startTime, float endTime, Rect begin, Rect end) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end) { }

        public override Rect lerp(float t) {
            return Rect.lerp(begin, end, t);
        }
    }

    public class IntProperty : Property<int> {
        public IntProperty(float startTime, float endTime, int begin, int end) : base(startTime: startTime,
            endTime: endTime, begin: begin, end: end) { }

        public override int lerp(float t) {
            return (begin + (end - begin) * t).round();
        }
    }

    public class NullableFloatProperty : Property<float?> {
        public NullableFloatProperty(float startTime, float endTime, float? begin, float? end) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end) { }

        public override float? lerp(float t) {
            D.assert(begin != null);
            D.assert(end != null);
            return begin + (end - begin) * t;
        }
    }

    public class FloatProperty : Property<float> {
        public FloatProperty(float startTime, float endTime, float begin, float end) : base(startTime: startTime,
            endTime: endTime, begin: begin, end: end) { }

        public override float lerp(float t) {
            return begin + (end - begin) * t;
        }
    }

    public class StepProperty : Property<int> {
        public StepProperty(float startTime, float endTime, int begin, int end) : base(startTime: startTime,
            endTime: endTime, begin: begin, end: end) { }

        public override int lerp(float t) {
            return (begin + (end - begin) * t).floor();
        }
    }

    public class OffsetProperty : Property<Offset> {
        public OffsetProperty(float startTime, float endTime, Offset begin, Offset end) : base(startTime: startTime,
            endTime: endTime, begin: begin, end: end) { }

        public override Offset lerp(float t) {
            return begin + (end - begin) * t;
        }
    }

    internal class ConstantProperty<T> : Property<T> {
        public ConstantProperty(float startTime, float endTime, T value) : base(startTime: startTime, endTime: endTime,
            begin: value, end: value) { }

        public override T lerp(float t) {
            return begin;
        }

        public override string ToString() {
            return $"{GetType()}(value: {begin})";
        }
    }
}