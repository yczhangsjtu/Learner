using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using Color = Unity.UIWidgets.ui.Color;

namespace Learner.Components {
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
                decoration: decoration
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

    public class MovieClipTypingEffect : BuilderMovieClipObject {
        public MovieClipTypingEffect(
            string id,
            List<string> texts,
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
            D.assert(!(texts?.isEmpty() ?? true));
            D.assert(texts.TrueForAll((s => !string.IsNullOrEmpty(s))));
            this.texts = texts;
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
            initConstantProgresses();
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

        void initConstantProgresses() {
            parameters["progress"] = new FloatListProperty(Enumerable.Repeat(0.0f, texts.Count).ToList());
        }

        static string fractionalSubstring(string s, float t) {
            return s.Substring((s.Length * t).round().clamp(0, s.Length-1));
        }

        string getText(float t) {
            List<float> progresses = (parameters["progress"] as FloatListProperty).evaluate(t);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < texts.Count; i++) {
                builder.Append(fractionalSubstring(texts[i], progresses[i]));
            }

            return builder.ToString();
        }

        public override Widget builder(BuildContext context, ParameterGetter getter, float t) {
            TextStyle _style = (TextStyle) getter("text style", t);
            Color _color = (Color) getter("color", t);
            return new TextBox(
                getText(t),
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
                decoration: decoration
            );
        }
        
        public override object Clone() {
            var ret = new MovieClipTypingEffect(
                id: id,
                texts: texts,
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

        public readonly List<string> texts;
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

    public static partial class MovieClipUtils {
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