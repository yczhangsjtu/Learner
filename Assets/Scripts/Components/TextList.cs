using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Components {

    public class TextList : StatelessWidget {
        public TextList(
            List<string> texts,
            TextStyle style,
            TextAlign textAlign = TextAlign.center,
            int? maxLines = null,
            float? lineHeight = null,
            string ellipsis = null,
            Axis direction = Axis.vertical,
            float? maxWidth = 100,
            float? maxHeight = 100,
            float? minWidth = 100,
            float? minHeight = 10,
            EdgeInsets padding = null,
            BoxDecoration decoration = null) {
            this.texts = new List<Paragraph>(texts.Count);
            this.heights = new List<float>(texts.Count);
            this.widths = new List<float>(texts.Count);
            this.uniformWidth = 0.0f;
            this.uniformHeight = 0.0f;
            ParagraphStyle paragraphStyle = new ParagraphStyle(
                textAlign: textAlign,
                maxLines: maxLines,
                lineHeight: lineHeight,
                ellipsis: ellipsis
            );
            padding = padding ?? EdgeInsets.all(10.0f);
            for (int i = 0; i < texts.Count; i++) {
                string text = texts[i];
                TextSpan textSpan = new TextSpan(
                    text, style: style
                );
                ParagraphBuilder builder = new ParagraphBuilder(paragraphStyle);
                textSpan.build(builder);
                Paragraph paragraph = builder.build();
                paragraph.layout(maxWidth == null || maxWidth <= padding.left + padding.right
                    ? null
                    : new ParagraphConstraints(maxWidth.Value - padding.left - padding.right));
                this.texts.Add(paragraph);
                
                float height = paragraph.height + padding.top + padding.bottom;
                if (maxHeight != null) height = height.clamp(minHeight ?? 0.0f, maxHeight.Value);
                float width = paragraph.width + padding.left + padding.horizontal;
                if (maxWidth != null) width = width.clamp(minWidth ?? 0.0f, maxWidth.Value);

                heights.Add(height);
                widths.Add(width);
                if (this.uniformWidth < width) this.uniformWidth = width;
                if (this.uniformHeight < height) this.uniformHeight = height;
            }

            if (direction == Axis.vertical) {
                _totalHeight = heights.Sum();
                _totalWidth = this.uniformWidth;
            }
            else {
                _totalWidth = widths.Sum();
                _totalHeight = this.uniformHeight;
            }
            this.style = style;
            this.textAlign = textAlign;
            this.maxLines = maxLines;
            this.lineHeight = lineHeight;
            this.ellipsis = ellipsis;
            this.direction = direction;
            this.padding = padding;
            this.decoration = decoration;
        }

        private readonly List<float> heights;
        private readonly List<float> widths;
        private readonly float uniformHeight;
        private readonly float uniformWidth;

        public readonly List<Paragraph> texts;
        public readonly TextStyle style;
        public readonly TextAlign textAlign;
        public readonly int? maxLines;
        public readonly float? lineHeight;
        public readonly string ellipsis;
        public readonly Axis direction;
        public readonly EdgeInsets padding;
        public readonly BoxDecoration decoration;

        public float totalWidth => _totalWidth;
        private float _totalWidth;

        public float totalHeight => _totalHeight;
        private float _totalHeight;

        public override Widget build(BuildContext context) {
            return new CustomPaint(
                painter: new TextListPainter(
                    texts,
                    style,
                    textAlign: textAlign,
                    maxLines: maxLines,
                    lineHeight: lineHeight,
                    ellipsis: ellipsis,
                    direction: direction,
                    heights: heights,
                    widths: widths,
                    uniformWidth: uniformWidth,
                    uniformHeight: uniformHeight,
                    padding: padding,
                    decoration: decoration),
                child: new Container(
                    width: totalWidth,
                    height: totalHeight
                )
            );
        }
    }
    public class TextListPainter : AbstractCustomPainter {

        public TextListPainter(
            List<Paragraph> texts,
            TextStyle style,
            TextAlign textAlign = TextAlign.center,
            int? maxLines = null,
            float? lineHeight = null,
            string ellipsis = null,
            Axis direction = Axis.vertical,
            List<float> heights = null,
            List<float> widths = null,
            float? uniformWidth = null,
            float? uniformHeight = null,
            EdgeInsets padding = null,
            BoxDecoration decoration = null) {
            this.texts = texts;
            this.style = style;
            this.textAlign = textAlign;
            this.maxLines = maxLines;
            this.lineHeight = lineHeight;
            this.ellipsis = ellipsis;
            this.direction = direction;
            this.widths = widths;
            this.heights = heights;
            this.uniformWidth = uniformWidth.Value;
            this.uniformHeight = uniformHeight.Value;
            this.padding = padding;
            this.decoration = decoration;
        }

        public readonly List<Paragraph> texts;
        public readonly TextStyle style;
        public readonly TextAlign textAlign;
        public readonly int? maxLines;
        public readonly float? lineHeight;
        public readonly string ellipsis;
        public readonly Axis direction;
        public readonly List<float> widths;
        public readonly List<float> heights;
        public readonly BoxDecoration decoration;
        public readonly EdgeInsets padding;
        private readonly float uniformHeight;
        private readonly float uniformWidth;

        public override void paint(Canvas canvas, Size size) {
            if (texts?.isEmpty() ?? true) return;
            BoxPainter boxPainter = decoration?.createBoxPainter();
            Offset position = Offset.zero;
            for (int i = 0; i < texts.Count; i++) {
                Size boxSize = direction == Axis.vertical
                    ? new Size(uniformWidth, heights[i])
                    : new Size(widths[i], uniformHeight);
                if (position.dx >= size.width || position.dy >= size.height) {
                    break;
                }
                boxPainter?.paint(canvas, position, new ImageConfiguration(size: boxSize));
                texts[i].paint(canvas, position + padding.topLeft);
                if (direction == Axis.vertical) {
                    position += new Offset(0, heights[i]);
                }
                else {
                    position += new Offset(widths[i], 0);
                }
            }
        }

        public override bool shouldRepaint(CustomPainter oldDelegate) {
            TextListPainter old = oldDelegate as TextListPainter;
            return texts != old.texts
                   || style != old.style
                   || textAlign != old.textAlign
                   || maxLines != old.maxLines
                   || lineHeight != old.lineHeight
                   || ellipsis != old.ellipsis
                   || direction != old.direction
                   || widths != old.widths
                   || heights != old.heights
                   || decoration != old.decoration
                   || uniformWidth != old.uniformWidth
                   || uniformHeight != old.uniformHeight;
             ;
        }
    }
}