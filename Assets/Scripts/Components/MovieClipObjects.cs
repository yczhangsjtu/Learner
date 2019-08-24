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
        public float? deathTime = null;

        public MovieClipObject(
            string id,
            int layer = 0,
            Offset position = null,
            Size scale = null,
            float? rotation = null) {
            D.assert(id != null);
            this.id = id;
            _layer = layer;
            _index = currentIndex++;
            if (position != null) {
                this.position = new ConstantOffsetProperty(position);
            }
            else {
                this.position = new ConstantOffsetProperty(Offset.zero);
            }

            if (scale != null) {
                this.scale = new ConstantSizeProperty(scale);
            }
            else {
                this.scale = new ConstantSizeProperty(Size.zero);
            }

            if (rotation != null) {
                this.rotation = new ConstantFloatProperty(rotation.Value);
            }
            else {
                this.rotation = new ConstantFloatProperty(0);
            }
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

        public void dieAt(float t) {
            this.deathTime = t;
        }

        public abstract object Clone();

        protected void copyInternalFrom(MovieClipObject obj) {
            _index = obj.index;
            _layer = obj.layer;
            position = obj.position;
        }

        public abstract Widget build(BuildContext context, float t);
    }

    public class BasicMovieClipObject : MovieClipObject {
        public BasicMovieClipObject(
            string id,
            Widget child,
            int layer = 0,
            Offset position = null)
            : base(id: id,
                layer: layer,
                position: position) {
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