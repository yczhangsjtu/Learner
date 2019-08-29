using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEngine;
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
            fontSize: 32
        );
        
        public void initProperty(TextStyle textStyle) {
            this.textStyle = new TextStyleProperty(defaultTextStyle.merge(textStyle));
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
            float? maxWidth = null,
            float? maxHeight = null,
            float? minWidth = null,
            float? minHeight = null,
            EdgeInsets padding = null,
            BoxDecoration decoration = null,
            int layer = 0)
            : base(id, layer) {
            this.text = text;
            this.textAlign = textAlign;
            this.maxLines = maxLines;
            this.padding = padding ?? EdgeInsets.all(10);
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
            fontSize: 32
        );
        
        void initConstantTextStyle(TextStyle style) {
            parameters["text style"] =
                new TextStyleProperty(defaultTextStyle.merge(style));
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
        public readonly Axis direction;
        public readonly EdgeInsets padding;
        public readonly BoxDecoration decoration;
        public readonly float?  maxWidth;
        public readonly float? maxHeight;
        public readonly float? minWidth;
        public readonly float? minHeight;
    }

    public class MovieClipTypingEffect : MovieClipFloatListObject {
        public MovieClipTypingEffect(
            string id,
            List<string> texts,
            TextStyle style = null,
            Color color = null,
            TextAlign textAlign = TextAlign.left,
            int? maxLines = null,
            Axis direction = Axis.vertical,
            float? maxWidth = null,
            float? maxHeight = null,
            float? minWidth = null,
            float? minHeight = null,
            EdgeInsets padding = null,
            BoxDecoration decoration = null,
            int layer = 0)
            : base(id, texts.Count, layer) {
            D.assert(!(texts?.isEmpty() ?? true));
            D.assert(texts.TrueForAll((s => !string.IsNullOrEmpty(s))));
            this.texts = texts;
            this.textAlign = textAlign;
            this.maxLines = maxLines;
            this.direction = direction;
            this.padding = padding ?? EdgeInsets.all(10);
            this.decoration = decoration;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
            this.minWidth = minWidth;
            this.minHeight = minHeight;
            initConstantTextStyle(style);
            initConstantColor(color);
            initConstantProgresses(texts.Count);
        }
        
       public static readonly TextStyle defaultTextStyle = new TextStyle(
            color: Colors.black,
            fontSize: 32
        );
        
        void initConstantTextStyle(TextStyle style) {
            parameters["text style"] =
                new TextStyleProperty(defaultTextStyle.merge(style));
        }

        void initConstantColor(Color color) {
            parameters["color"] = new ColorProperty(color ?? Colors.white);
        }

        static string fractionalSubstring(string s, float t) {
            return s.Substring(0, (s.Length * t).round().clamp(0, s.Length));
        }

        string getText(List<float> progresses) {
            D.assert(progresses.Count == texts.Count);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < texts.Count; i++) {
                builder.Append(fractionalSubstring(texts[i], progresses[i]));
            }

            return builder.ToString();
        }

        public override Widget buildWithProgress(BuildContext context, List<float> progress, ParameterGetter getter, float t) {
            TextStyle _style = (TextStyle) getter("text style", t);
            Color _color = (Color) getter("color", t);
            return new TextBox(
                getText(progress),
                style: _style,
                color: _color,
                textAlign: textAlign,
                maxLines: maxLines,
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
        public readonly Axis direction;
        public readonly EdgeInsets padding;
        public readonly BoxDecoration decoration;
        public readonly float? maxWidth;
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
            float? maxWidth = null,
            float? maxHeight = null,
            float? minWidth = null,
            float? minHeight = null,
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
                    maxWidth: maxWidth,
                    maxHeight: maxHeight,
                    minWidth: minWidth,
                    minHeight: minHeight,
                    padding: padding ?? EdgeInsets.all(10),
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

        public static void createTypingEffect(
            this MovieClipSnapshot snapshot,
            string id,
            List<string> texts,
            TextStyle style = null,
            Color color = null,
            TextAlign textAlign = TextAlign.left,
            int? maxLines = null,
            float? maxWidth = null,
            float? maxHeight = null,
            float? minWidth = null,
            float? minHeight = null,
            EdgeInsets padding = null,
            BoxDecoration decoration = null,
            int layer = 0,
            Offset position = null,
            Offset pivot = null,
            Size scale = null,
            float rotation = 0,
            float opacity = 1,
            float delay = 0,
            float appearTime = MovieClipSnapshot.kDefaultAppearTime) {
            snapshot.createObject(new MovieClipTypingEffect(
                    id: id,
                    texts: texts,
                    style: style,
                    color: color,
                    textAlign: textAlign,
                    maxLines: maxLines,
                    maxWidth: maxWidth,
                    maxHeight: maxHeight,
                    minWidth: minWidth,
                    minHeight: minHeight,
                    padding: padding ?? EdgeInsets.all(10),
                    decoration: decoration ?? new BoxDecoration(),
                    layer: layer
                ),
                position: position,
                pivot: pivot,
                scale: scale,
                rotation: rotation,
                opacity: opacity,
                delay: delay,
                appearTime: appearTime
            );
        }
    }
}