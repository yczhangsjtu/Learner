using System;
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

        public OffsetPropertyData position;

        public MovieClipObject(
            string id,
            int layer = 0,
            Offset position = null) {
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