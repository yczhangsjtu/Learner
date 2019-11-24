using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Learner.Components {

    public class TextBox : TextList {

        public TextBox(
            string text,
            TextStyle style,
            Color color = null,
            TextAlign textAlign = TextAlign.center,
            int? maxLines = null,
            float? maxWidth = null,
            float? maxHeight = null,
            float? minWidth = null,
            float? minHeight = null,
            EdgeInsets padding = null,
            BoxDecoration decoration = null) :
            base(texts: new List<string>{text},
                style: style,
                textAlign: textAlign,
                maxLines: maxLines,
                maxWidth: maxWidth,
                maxHeight: maxHeight,
                minWidth: minWidth,
                minHeight: minHeight,
                padding: padding,
                decoration: decoration ??
                    new BoxDecoration(
                        color: color ?? Colors.white,
                        border: Border.all(color: Colors.black)
                    )) {}
    }

    public class TextList : StatelessWidget {
        public TextList(
            List<string> texts,
            TextStyle style,
            TextAlign textAlign = TextAlign.center,
            int? maxLines = null,
            Axis direction = Axis.vertical,
            float? maxWidth = null,
            float? maxHeight = null,
            float? minWidth = null,
            float? minHeight = null,
            EdgeInsets padding = null,
            BoxDecoration decoration = null) {
            this.texts = texts;
            this.style = style;
            this.textAlign = textAlign;
            this.maxLines = maxLines;
            this.direction = direction;
            this.maxWidth = maxWidth;
            this.minWidth = minWidth;
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
            this.padding = padding ?? EdgeInsets.all(10);
            this.decoration = decoration;
        }


        public readonly List<string> texts;
        public readonly TextStyle style;
        public readonly TextAlign textAlign;
        public readonly int? maxLines;
        public readonly Axis direction;
        public readonly float? maxWidth;
        public readonly float? minWidth;
        public readonly float? maxHeight;
        public readonly float? minHeight;
        public readonly EdgeInsets padding;
        public readonly BoxDecoration decoration;

        public override Widget build(BuildContext context) {
            if (direction == Axis.horizontal) {
                List<Widget> children = texts.Select(
                    s => {
                        Widget child = new Text(s, style: style, textAlign: textAlign, maxLines: maxLines);
                        
                        child = new ConstrainedBox(
                            constraints: new BoxConstraints(
                                minWidth: minWidth ?? 0.0f,
                                maxWidth: maxWidth ?? MediaQuery.of(context).size.width,
                                minHeight: minHeight ?? 0.0f,
                                maxHeight: maxHeight ?? MediaQuery.of(context).size.height
                            ),
                            child: child
                        );
                        
                        if (decoration != null) {
                            child = new Container(
                                padding: padding,
                                child: child,
                                decoration: decoration
                            );
                        }

                        return child;
                    }
                ).ToList();
                return new Table(
                    children: new List<TableRow> {
                        new TableRow(children: children)
                    },
                    defaultColumnWidth: new IntrinsicColumnWidth()
                );
            }
            else {
                List<TableRow> children = texts.Select(
                    s => {
                        Widget child = new Text(s, style: style, textAlign: textAlign, maxLines: maxLines);

                        child = new ConstrainedBox(
                            constraints: new BoxConstraints(
                                minWidth: minWidth ?? 10.0f,
                                maxWidth: maxWidth ?? MediaQuery.of(context).size.width,
                                minHeight: minHeight ?? 10.0f,
                                maxHeight: maxHeight ?? MediaQuery.of(context).size.height
                            ),
                            child: child
                        );
                        
                        if (decoration != null) {
                            child = new Container(
                                padding: padding,
                                child: child,
                                decoration: decoration
                            );
                        }

                        return new TableRow(children: new List<Widget> {child});
                    }
                ).ToList();
                return new Table(
                    defaultColumnWidth: new IntrinsicColumnWidth(),
                    children: children
                );
            }
        }
    }
}