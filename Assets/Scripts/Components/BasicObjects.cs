using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Components {
    public class BasicMovieClipObject : MovieClipObjectBuilder {
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

        public BasicMovieClipObject(string id, int layer, int index, Widget child) : base(id: id, layer: layer,
            index: index) {
            this.child = child;
        }

        public readonly Widget child;
        public override object Clone() {
            var ret = new BasicMovieClipObject(
                this.id, this.layer, this.index, this.child
            );
            ret.position = this.position;
            return ret;
        }

        public override Widget build(BuildContext context, float t) {
            return child;
        }
    }
}