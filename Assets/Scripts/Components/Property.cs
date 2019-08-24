using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.painting;

namespace Components {
    public interface Property { }

    public interface PropertyData<T> {
        T lerp(float t);
        T evaluate(float t);
    }

    public abstract class Property<T> : Tween<T>, PropertyData<T>, Property {
        public readonly float duration;
        public readonly float startTime;
        public readonly float endTime;
        public readonly Curve curve;

        public Property(
            T begin,
            T end,
            float startTime,
            float endTime,
            Curve curve = null
        ) : base(begin, end) {
            D.assert(startTime < endTime);
            D.assert(startTime >= 0.0f);
            this.startTime = startTime;
            this.endTime = endTime;
            this.curve = curve;
            duration = endTime - startTime;
        }

        public T evaluate(float t) {
            if (t <= startTime) return begin;

            if (t >= endTime) return end;

            t = (t - startTime) / duration;

            if (curve != null) {
                t = curve.transform(t);
            }

            return lerp(t);
        }
    }

    public class ColorProperty : Property<Color> {
        public ColorProperty(float startTime, float endTime, Color begin, Color end, Curve curve = null) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override Color lerp(float t) {
            return Color.lerp(begin, end, t);
        }
    }

    public class SizeProperty : Property<Size> {
        public SizeProperty(float startTime, float endTime, Size begin, Size end, Curve curve = null) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override Size lerp(float t) {
            return Size.lerp(begin, end, t);
        }
    }

    public class RectProperty : Property<Rect> {
        public RectProperty(float startTime, float endTime, Rect begin, Rect end, Curve curve = null) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override Rect lerp(float t) {
            return Rect.lerp(begin, end, t);
        }
    }

    public class IntProperty : Property<int> {
        public IntProperty(float startTime, float endTime, int begin, int end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override int lerp(float t) {
            return (begin + (end - begin) * t).round();
        }
    }

    public class NullableFloatProperty : Property<float?> {
        public NullableFloatProperty(float startTime, float endTime, float? begin, float? end, Curve curve = null) :
            base(startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override float? lerp(float t) {
            D.assert(begin != null);
            D.assert(end != null);
            return begin + (end - begin) * t;
        }
    }

    public class FloatProperty : Property<float> {
        public FloatProperty(float startTime, float endTime, float begin, float end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override float lerp(float t) {
            return begin + (end - begin) * t;
        }
    }

    public class StepProperty : Property<int> {
        public StepProperty(float startTime, float endTime, int begin, int end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override int lerp(float t) {
            return (begin + (end - begin) * t).floor();
        }
    }

    public class OffsetProperty : Property<Offset> {
        public OffsetProperty(float startTime, float endTime, Offset begin, Offset end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override Offset lerp(float t) {
            return begin + (end - begin) * t;
        }
    }
    
    public class TextStyleProperty : Property<TextStyle> {
        public TextStyleProperty(float startTime, float endTime, TextStyle begin, TextStyle end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public override TextStyle lerp(float t) {
            return TextStyle.lerp(begin, end, t);
        }
    }

    public class ConstantProperty<T> : Property<T> {
        public ConstantProperty(T value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }

        public override T lerp(float t) {
            return begin;
        }

        public override string ToString() {
            return $"{GetType()}(value: {begin})";
        }
    }
}