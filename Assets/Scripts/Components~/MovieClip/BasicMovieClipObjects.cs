using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Learner.Components {

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

        public override void debugAll() {
            base.debugAll();
            debugProperty("child");
        }

        public override void assembleDebugString(StringBuilder builder, float? t = null) {
            base.assembleDebugString(builder, t);
            if (propertyDebugged("child")) {
                builder.AppendLine($"Child: {child?.toStringShort() ?? "Null"}");
            }
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
                    begin: useFrom ? from : propertyT.evaluate(startTime),
                    end: target,
                    startTime: startTime,
                    endTime: startTime + duration,
                    curve: curve
                );
                return true;
            }
            return false;
        }

        public override Widget build(BuildContext context, float t) {
            return builder(context, getParameter, t);
        }

        public override void assembleDebugString(StringBuilder builder, float? t = null) {
            base.assembleDebugString(builder, t);
            foreach (var entry in parameters) {
                if (propertyDebugged(entry.Key)) {
                    builder.AppendLine($"{entry.Key}: {entry.Value.toString(t)}");
                }
            }
        }
    }

    public interface MovieClipObjectWithProperty<T> {
        T getProperty(float t);
        void animateTo(T target, float startTime, float duration, T from, Curve curve);
    }
    
    public abstract class MovieClipFloatListObject : BuilderMovieClipObject {
        public MovieClipFloatListObject(
            string id,
            int size,
            int layer = 0)
            : base(id, layer) {
            D.assert(size > 0);
            initConstantProgresses(size);
        }
        
        protected void initConstantProgresses(int size) {
            parameters["progress"] = new FloatListProperty(Enumerable.Repeat(0.0f, size).ToList());
        }

        public abstract Widget buildWithProgress(BuildContext context, List<float> progress, ParameterGetter getter, float t);

        public override Widget builder(BuildContext context, ParameterGetter getter, float t) {
            List<float> progresses = (parameters["progress"] as FloatListProperty).evaluate(t);
            return buildWithProgress(context, progresses, getter, t);
        }
    }

    public static partial class MovieClipUtils {
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

        public static void createBasicObjectWithTitle(
            this MovieClipSnapshot snapshot,
            string id,
            string title,
            Widget child,
            float titleDistance = 25,
            Alignment alignment = null,
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
            snapshot.createBasicObject(
                id: id,
                child: new Stack(children: new List<Widget> {
                    new Padding(
                        child: child,
                        padding: EdgeInsets.all(titleDistance)
                    ),
                    Positioned.fill(
                        new Align(
                            child: new Text(title, style: style ?? new TextStyle(fontWeight: FontWeight.bold)),
                            alignment: alignment ?? Alignment.topCenter
                        )
                    )
                }),
                animation: animation,
                layer: layer,
                position: position,
                pivot: pivot,
                scale: scale,
                rotation: rotation,
                opacity: opacity,
                delay: delay,
                appearTime: appearTime
            );
        }

        public static bool animateFloatListObject(
            this MovieClipSnapshot snapshot,
            string id,
            PropertyModifier<List<float>> modifier,
            float delay = 0,
            float? duration = null,
            PropertyModifier<List<float>> from = null,
            Curve curve = null
        ) {
            return snapshot.animateTo<List<float>>(
                id,
                paramName: "progress",
                target: progress => {
                    var ret = progress.ToList();
                    modifier(ret);
                    return ret;
                },
                delay: delay,
                duration: duration,
                from: from != null ? (PropertyProvider<List<float>>) (progress => {
                    var ret = progress.ToList();
                    from(ret);
                    return ret;
                }) : null,
                curve: curve
            );
        }
    }
}