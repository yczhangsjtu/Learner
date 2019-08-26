using System;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Learner.Components {
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
        public PropertyData<Offset> pivot;
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
            Offset pivot = null,
            float opacity = 1) {
            D.assert(id != null);
            this.id = id;
            _layer = layer;
            _index = currentIndex++;
            initConstants(
                position: position,
                pivot: pivot,
                scale: scale,
                rotation: rotation,
                opacity: opacity);
        }

        public void initConstants(
            Offset position = null,
            Size scale = null,
            float rotation = 0,
            Offset pivot = null,
            float opacity = 1) {
            initConstantPosition(position);
            initConstantPivot(pivot);
            initConstantScale(scale);
            initConstantRotation(rotation);
            initConstantOpacity(opacity);
        }

        public void initConstantPosition(Offset position) {
            if (position != null) {
                this.position = new OffsetProperty(position);
            }
            else {
                this.position = new OffsetProperty(Offset.zero);
            }
        }
        
        public void initConstantPivot(Offset pivot) {
            if (pivot != null) {
                this.pivot = new OffsetProperty(pivot);
            }
            else {
                this.pivot = new OffsetProperty(new Offset(0.5f, 0.5f));
            }
        }

        public void initConstantScale(Size scale) {
            if (scale != null) {
                this.scale = new SizeProperty(scale);
            }
            else {
                this.scale = new SizeProperty(originalSize);
            }
        }

        public void initConstantRotation(float rotation) {
            this.rotation = new FloatProperty(rotation);
        }

        public void initConstantOpacity(float opacity) {
            this.opacity = new FloatProperty(opacity);
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
        
        public void pivotTo(Offset pivot, float startTime, float duration, Offset fromPosition = null, Curve curve = null) {
            this.pivot = new OffsetProperty(
                startTime: startTime,
                endTime: startTime + duration,
                begin: fromPosition ?? this.pivot.evaluate(startTime),
                end: pivot,
                curve: curve
            );
        }

        public void pivotChangeBy(Offset offset, float startTime, float duration, Curve curve = null) {
            var fromPivot = pivot.evaluate(startTime);
            pivotTo(fromPivot + offset, startTime, duration, fromPivot, curve);
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

        public void rotateBy(float rotation, float startTime, float duration, Curve curve = null) {
            var fromRotation = this.rotation.evaluate(startTime);
            rotateTo(fromRotation + rotation, startTime, duration, fromRotation, curve);
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

        public void scaleBy(Size scale, float startTime, float duration, Curve curve = null) {
            var fromScale = this.scale.evaluate(startTime);
            scaleTo(new Size(fromScale.width * scale.width, fromScale.width * scale.height),
                startTime, duration, fromScale, curve);
        }

        public void opacityTo(float opacity, float startTime, float duration, float? fromOpacity = null, Curve curve = null) {
            this.opacity = new FloatProperty(
                startTime: startTime,
                endTime: startTime + duration,
                begin: fromOpacity ?? this.opacity.evaluate(startTime),
                end: opacity,
                curve: curve
            );
        }

        public void opacityChangeBy(float delta, float startTime, float duration, Curve curve = null) {
            var fromOpacity = opacity.evaluate(startTime);
            opacityTo((fromOpacity + delta).clamp(0.0f, 1.0f), startTime, duration, fromOpacity, curve);
        }

        public void dieAt(float t) {
            this.deathTime = t;
        }

        public abstract object Clone();

        protected virtual void copyInternalFrom(MovieClipObject obj) {
            _index = obj.index;
            _layer = obj.layer;
            position = obj.position;
            scale = obj.scale;
            rotation = obj.rotation;
            pivot = obj.pivot;
            opacity = obj.opacity;
        }

        public abstract Widget build(BuildContext context, float t);
    }
    
}