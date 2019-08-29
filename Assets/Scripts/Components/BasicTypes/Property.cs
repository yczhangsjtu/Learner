using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.painting;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Color = Unity.UIWidgets.ui.Color;
using Rect = Unity.UIWidgets.ui.Rect;

namespace Learner.Components {
    public interface Property {
        string toString(float? t = null);
    }

    public interface PropertyData<T> : Property {
        T lerp(float t);
        T evaluate(float t);
    }

    public abstract class Property<T> : Tween<T>, PropertyData<T>, ICloneable {
        public float duration => _duration;
        private float _duration;
    
        public float startTime => _startTime;
        private float _startTime;
        
        public float endTime => _endTime;
        private float _endTime;
        
        public Curve curve => _curve;
        private Curve _curve;

        public Property(
            T begin,
            T end,
            float startTime,
            float endTime,
            Curve curve = null
        ) : base(begin, end) {
            D.assert(startTime < endTime);
            D.assert(startTime >= 0.0f);
            _startTime = startTime;
            _endTime = endTime;
            _curve = curve;
            _duration = endTime - startTime;
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
        
        public abstract object Clone();

        public Property<T> copyWith(T begin, T end, float? startTime = null, float? endTime = null, Curve curve = null) {
            Property<T> ret = Clone() as Property<T>;
            ret.begin = begin;
            ret.end = end;
            ret._startTime = startTime ?? ret.startTime;
            ret._endTime = endTime ?? ret.endTime;
            ret._duration = ret._endTime - ret._startTime;
            ret._curve = curve ?? ret.curve;
            return ret;
        }

        public override string ToString() {
            return $"[{startTime} - {endTime}]: {begin} -> {end}{(curve == null ? "" : curve.ToString())}";
        }

        public string toString(float? t = null) {
            if (t == null) return ToString();
            return $"{evaluate(t.Value)} -- {ToString()}";
        }
    }

    public class ColorProperty : Property<Color> {
        public ColorProperty(float startTime, float endTime, Color begin, Color end, Curve curve = null) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }
        
        public ColorProperty(Color value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }

        public override Color lerp(float t) {
            return Color.lerp(begin, end, t);
        }

        public override object Clone() {
            return new ColorProperty(startTime, endTime, begin, end, curve);
        }
    }

    public class SizeProperty : Property<Size> {
        public SizeProperty(float startTime, float endTime, Size begin, Size end, Curve curve = null) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }
        
        public SizeProperty(Size value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }

        public override Size lerp(float t) {
            return Size.lerp(begin, end, t);
        }

        public override object Clone() {
            return new SizeProperty(startTime, endTime, begin, end, curve);
        }
    }

    public class RectProperty : Property<Rect> {
        public RectProperty(float startTime, float endTime, Rect begin, Rect end, Curve curve = null) : base(
            startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }
        
        public RectProperty(Rect value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }

        public override Rect lerp(float t) {
            return Rect.lerp(begin, end, t);
        }

        public override object Clone() {
            return new RectProperty(startTime, endTime, begin, end, curve);
        }
    }

    public class IntProperty : Property<int> {
        public IntProperty(float startTime, float endTime, int begin, int end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }
        
        public IntProperty(int value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }

        public override int lerp(float t) {
            return (begin + (end - begin) * t).round();
        }

        public override object Clone() {
            return new IntProperty(startTime, endTime, begin, end, curve);
        }
    }

    public class NullableFloatProperty : Property<float?> {
        public NullableFloatProperty(float startTime, float endTime, float? begin, float? end, Curve curve = null) :
            base(startTime: startTime, endTime: endTime, begin: begin, end: end, curve: curve) { }

        public NullableFloatProperty(float? value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }
        
        public override float? lerp(float t) {
            D.assert(begin != null);
            D.assert(end != null);
            return begin + (end - begin) * t;
        }

        public override object Clone() {
            return new NullableFloatProperty(startTime, endTime, begin, end, curve);
        }
    }

    public class FloatListProperty : Property<List<float>> {
        public FloatListProperty(float startTime, float endTime, List<float> begin, List<float> end,
            Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) {
            D.assert(begin.Count == end.Count);
        }

        public FloatListProperty(List<float> value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }
        
        public override List<float> lerp(float t) {
            List<float> result = new List<float>(begin.Count);
            for (int i = 0; i < begin.Count; i++) {
                result.Add(begin[i] + (end[i] - begin[i]) * t);
            }

            return result;
        }

        public override object Clone() {
            return new FloatListProperty(startTime, endTime, begin.ToList(), end.ToList(), curve);
        }
    }

    public class FloatProperty : Property<float> {
        public FloatProperty(float startTime, float endTime, float begin, float end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public FloatProperty(float value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }
        
        public override float lerp(float t) {
            return begin + (end - begin) * t;
        }

        public override object Clone() {
            return new FloatProperty(startTime, endTime, begin, end, curve);
        }
    }

    public class StepProperty : Property<int> {
        public StepProperty(float startTime, float endTime, int begin, int end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public StepProperty(int value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }
        
        public override int lerp(float t) {
            return (begin + (end - begin) * t).floor();
        }

        public override object Clone() {
            return new StepProperty(startTime, endTime, begin, end, curve);
        }
    }

    public class OffsetProperty : Property<Offset> {
        public OffsetProperty(float startTime, float endTime, Offset begin, Offset end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }
        
        public OffsetProperty(Offset value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }

        public override Offset lerp(float t) {
            return begin + (end - begin) * t;
        }

        public override object Clone() {
            return new OffsetProperty(startTime, endTime, begin, end, curve);
        }
    }
    
    public class TextStyleProperty : Property<TextStyle> {
        public TextStyleProperty(float startTime, float endTime, TextStyle begin, TextStyle end, Curve curve = null) : base(
            startTime: startTime,
            endTime: endTime, begin: begin, end: end, curve: curve) { }

        public TextStyleProperty(TextStyle value) : base(startTime: 0, endTime: 1, begin: value, end: value) { }
        
        public override TextStyle lerp(float t) {
            return TextStyle.lerp(begin, end, t);
        }

        public override object Clone() {
            return new TextStyleProperty(startTime, endTime, begin, end, curve);
        }
    }
}