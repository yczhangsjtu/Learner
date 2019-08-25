using System;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using Color = Unity.UIWidgets.ui.Color;

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

    public delegate object ParameterGetter(string paramName, float t);

    public abstract class BuilderMovieClipObject : MovieClipObject {
        
        internal BuilderMovieClipObject(
            string id,
            float startTime = 0,
            int layer = 0)
            : base(id: id, layer: layer) {
            this.startTime = startTime;
        }

        public readonly float startTime;
        public readonly Dictionary<string, Property> parameters =
            new Dictionary<string, Property>();

        public abstract Widget builder(BuildContext context, ParameterGetter getter, float t);
        
        protected override void copyInternalFrom(MovieClipObject obj) {
            base.copyInternalFrom(obj);
            if (obj is BuilderMovieClipObject builderObject) {
                parameters.Clear();
                foreach (var entry in builderObject.parameters) {
                    parameters.Add(entry.Key, entry.Value);
                }
            }
        }

        public object getParameter(string paramName, float t) {
            return !parameters.TryGetValue(paramName, out var ret) ? null : (ret as dynamic).evaluate(t);
        }

        public bool animateTo<T>(string paramName, T target, float startTime, float duration,
            bool useFrom = false, T from = default(T), Curve curve = null) {
            if(!parameters.TryGetValue(paramName, out var property))
                return false;
            if (property is Property<T> propertyT) {
                parameters[paramName] = propertyT.copyWith(
                    startTime: startTime,
                    endTime: startTime + duration,
                    begin: useFrom ? from : propertyT.evaluate(startTime),
                    end: target,
                    curve: curve
                );
                return true;
            }
            return false;
        }

        public override Widget build(BuildContext context, float t) {
            return builder(context, getParameter, t);
        }
    }

    public interface MovieClipObjectWithProperty<T> {
        T getProperty(float t);
        void animateTo(T target, float startTime, float duration, T from, Curve curve);
    }

    public class MovieClipTextObject : MovieClipObject, MovieClipObjectWithProperty<TextStyle> {
        public MovieClipTextObject(
            string id,
            string text,
            TextStyle style = null,
            int layer = 0) : base(id, layer) {
            this.text = text;
            initProperty(style);
        }
        
        static readonly TextStyle defaultTextStyle = new TextStyle(
            color: Colors.black,
            fontSize: 32,
            letterSpacing: 10
        );
        
        public void initProperty(TextStyle textStyle) {
            this.textStyle = new TextStyleProperty(textStyle?.merge(defaultTextStyle) ?? defaultTextStyle);
        }

        public override object Clone() {
            var ret = new MovieClipTextObject(id, text);
            ret.copyInternalFrom(this);
            ret.textStyle = textStyle;
            return ret;
        }

        public TextStyle getProperty(float t) {
            return textStyle.evaluate(t);
        }

        public override Widget build(BuildContext context, float t) {
            return new Text(text, style: textStyle.evaluate(t));
        }

        public readonly string text;
        public PropertyData<TextStyle> textStyle;
        
        public void animateTo(
            TextStyle target,
            float startTime,
            float duration,
            TextStyle from = null,
            Curve curve = null) {
            textStyle = new TextStyleProperty(
                startTime: startTime,
                endTime: startTime + duration,
                begin: from ?? this.textStyle.evaluate(startTime),
                end: target,
                curve: curve
            );
        }
    }

    public class MovieClipTextBoxObject : BuilderMovieClipObject {
        internal MovieClipTextBoxObject(
            string id,
            string text,
            TextStyle style = null,
            Color color = null,
            TextAlign textAlign = TextAlign.center,
            int? maxLines = null,
            float? lineHeight = null,
            string ellipsis = null,
            Axis direction = Axis.vertical,
            float  maxWidth = 200,
            float? maxHeight = null,
            float? minWidth = 100,
            float? minHeight = 10,
            EdgeInsets padding = null,
            BoxDecoration decoration = null,
            int layer = 0)
            : base(id, layer) {
            this.text = text;
            this.textAlign = textAlign;
            this.maxLines = maxLines;
            this.lineHeight = lineHeight;
            this.ellipsis = ellipsis;
            this.direction = direction;
            this.padding = padding;
            this.decoration = decoration;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
            this.minWidth = minWidth;
            this.minHeight = minHeight;
            initConstantTextStyle(style);
            initConstantColor(color);
        }
        
       public static readonly TextStyle defaultTextStyle = new TextStyle(
            color: Colors.black,
            fontSize: 32,
            letterSpacing: 10
        );
        
        void initConstantTextStyle(TextStyle style) {
            parameters["text style"] =
                new TextStyleProperty(style?.merge(defaultTextStyle) ?? defaultTextStyle);
        }

        void initConstantColor(Color color) {
            parameters["color"] = new ColorProperty(color ?? Colors.white);
        }

        public override Widget builder(BuildContext context, ParameterGetter getter, float t) {
            TextStyle _style = (TextStyle) getter("text style", t);
            Color _color = (Color) getter("color", t);
            return new TextBox(
                text,
                style: _style,
                color: _color,
                textAlign: textAlign,
                maxLines: maxLines,
                lineHeight: lineHeight,
                ellipsis: ellipsis,
                direction: direction,
                maxWidth: maxWidth,
                maxHeight: maxHeight,
                minWidth: minWidth,
                minHeight: minHeight,
                padding: padding,
                decoration: this.decoration
            );
        
        }
        
        public override object Clone() {
            var ret = new MovieClipTextBoxObject(
                id: id,
                text: text,
                textAlign: textAlign,
                maxLines: maxLines,
                lineHeight: lineHeight,
                ellipsis: ellipsis,
                direction: direction,
                maxWidth: maxWidth,
                maxHeight: maxHeight,
                minWidth: minWidth,
                minHeight: minHeight,
                padding: padding,
                decoration: this.decoration
            );
            ret.copyInternalFrom(this);
            return ret;
        }

        public readonly string text;
        public readonly TextAlign textAlign;
        public readonly int? maxLines;
        public readonly float? lineHeight;
        public readonly string ellipsis;
        public readonly Axis direction;
        public readonly EdgeInsets padding;
        public readonly BoxDecoration decoration;
        public readonly float  maxWidth;
        public readonly float? maxHeight;
        public readonly float? minWidth;
        public readonly float? minHeight;
    }
    

    public static class MovieClipUtils {
        public static void createText(
            this MovieClipSnapshot snapshot,
            string id, string text,
            TextStyle style = null,
            AppearAnimation animation = AppearAnimation.none,
            int layer = 0,
            Offset position = null,
            Offset pivot = null,
            Size scale = null,
            float rotation = 0,
            float opacity = 1,
            float delay = 0,
            float appearTime = MovieClipSnapshot.kDefaultAppearTime) {
            snapshot.createObject(new MovieClipTextObject(
                    id, text, style, layer: layer),
                position: position,
                pivot: pivot,
                scale: scale,
                rotation: rotation,
                opacity: opacity,
                delay: delay,
                animation: animation,
                appearTime: appearTime);
        }

        public static void createBasicObject(
            this MovieClipSnapshot snapshot,
            string id,
            Widget child,
            AppearAnimation animation = AppearAnimation.none,
            int layer = 0,
            Offset position = null,
            Offset pivot = null,
            Size scale = null,
            float rotation = 0,
            float opacity = 1,
            float delay = 0,
            float appearTime = MovieClipSnapshot.kDefaultAppearTime) {
            snapshot.createObject(new BasicMovieClipObject(id, child, layer),
                position: position,
                pivot: pivot,
                scale: scale,
                rotation: rotation,
                opacity: opacity,
                delay: delay,
                animation: animation,
                appearTime: appearTime
            );
        }
        
        public static void createTextBox(
            this MovieClipSnapshot snapshot,
            string id,
            string text,
            TextStyle style = null,
            Color color = null,
            TextAlign textAlign = TextAlign.center,
            int? maxLines = null,
            float? lineHeight = null,
            string ellipsis = null,
            Axis direction = Axis.vertical,
            float  maxWidth = 200,
            float? maxHeight = null,
            float? minWidth = 100,
            float? minHeight = 10,
            EdgeInsets padding = null,
            BoxDecoration decoration = null,
            AppearAnimation animation = AppearAnimation.none,
            int layer = 0,
            Offset position = null,
            Offset pivot = null,
            Size scale = null,
            float rotation = 0,
            float opacity = 1,
            float delay = 0,
            float appearTime = MovieClipSnapshot.kDefaultAppearTime) {
            snapshot.createObject(new MovieClipTextBoxObject(id,
                    text,
                    style,
                    color: color,
                    textAlign: textAlign,
                    maxLines: maxLines,
                    lineHeight: lineHeight,
                    ellipsis: ellipsis,
                    direction: direction,
                    maxWidth: maxWidth,
                    maxHeight: maxHeight,
                    minWidth: minWidth,
                    minHeight: minHeight,
                    padding: padding,
                    decoration: decoration,
                    layer: layer
                ),
                position: position,
                pivot: pivot,
                scale: scale,
                rotation: rotation,
                opacity: opacity,
                delay: delay,
                animation: animation,
                appearTime: appearTime
            );
        }
    }
}