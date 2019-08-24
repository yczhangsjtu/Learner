using System;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Components {
    public abstract class MovieClipObject : ICloneable {
        public readonly string id;

        public int layer {
            get { return _layer; }
        }

        private int _layer;

        public int index {
            get { return _index; }
        }

        private int _index;
        private static int currentIndex = 0;

        public PropertyData<Offset> position;
        public PropertyData<Size> scale;
        public PropertyData<float> rotation;
        public PropertyData<float> opacity;
        public float? deathTime = null;
        public static Size originalSize = new Size(1, 1);

        protected MovieClipObject(
            string id,
            int layer = 0,
            Offset position = null,
            Size scale = null,
            float rotation = 0,
            float opacity = 1) {
            D.assert(id != null);
            this.id = id;
            _layer = layer;
            _index = currentIndex++;
            initConstantPosition(position);
            initConstantScale(scale);
            initConstantRotation(rotation);
            initConstantOpacity(opacity);
        }

        public void initConstantPosition(Offset position) {
            if (position != null) {
                this.position = new ConstantOffsetProperty(position);
            }
            else {
                this.position = new ConstantOffsetProperty(Offset.zero);
            }
        }

        public void initConstantScale(Size scale) {
            if (scale != null) {
                this.scale = new ConstantSizeProperty(scale);
            }
            else {
                this.scale = new ConstantSizeProperty(originalSize);
            }
        }

        public void initConstantRotation(float rotation) {
            this.rotation = new ConstantFloatProperty(rotation);
        }

        public void initConstantOpacity(float opacity) {
            this.opacity = new ConstantFloatProperty(opacity);
        }

        public void moveTo(Offset position, float startTime, float duration, Offset fromPosition = null, Curve curve = null) {
            this.position = new OffsetProperty(
                startTime: startTime,
                endTime: startTime + duration,
                begin: fromPosition ?? this.position.evaluate(startTime),
                end: position,
                curve: curve
            );
        }

        public void move(Offset offset, float startTime, float duration, Curve curve = null) {
            var fromPosition = position.evaluate(startTime);
            moveTo(fromPosition + offset, startTime, duration, fromPosition, curve);
        }

        public void rotateTo(float rotation, float startTime, float duration, float? fromRotation = null, Curve curve = null) {
            this.rotation = new FloatProperty(
                startTime: startTime,
                endTime: startTime + duration,
                begin: fromRotation ?? this.rotation.evaluate(startTime),
                end: rotation,
                curve: curve
            );
        }

        public void scaleTo(Size scale, float startTime, float duration, Size fromScale = null, Curve curve = null) {
            this.scale = new SizeProperty(
                startTime: startTime,
                endTime: startTime + duration,
                begin: fromScale ?? this.scale.evaluate(startTime),
                end: scale,
                curve: curve
            );
        }

        public void opacityTo(float opacity, float startTime, float duration, float? fromOpacity, Curve curve = null) {
            this.opacity = new FloatProperty(
                startTime: startTime,
                endTime: startTime + duration,
                begin: fromOpacity ?? this.opacity.evaluate(startTime),
                end: opacity,
                curve: curve
            );
        }

        public void dieAt(float t) {
            this.deathTime = t;
        }

        public abstract object Clone();

        protected void copyInternalFrom(MovieClipObject obj) {
            _index = obj.index;
            _layer = obj.layer;
            position = obj.position;
            scale = obj.scale;
            rotation = obj.rotation;
        }

        public abstract Widget build(BuildContext context, float t);
    }

    public class BasicMovieClipObject : MovieClipObject {
        internal BasicMovieClipObject(
            string id,
            Widget child,
            int layer = 0)
            : base(id: id, layer: layer) {
            this.child = child;
        }

        public readonly Widget child;
        public override object Clone() {
            var ret = new BasicMovieClipObject(this.id, this.child);
            ret.copyInternalFrom(this);
            return ret;
        }

        public override Widget build(BuildContext context, float t) {
            return child;
        }
    }
}