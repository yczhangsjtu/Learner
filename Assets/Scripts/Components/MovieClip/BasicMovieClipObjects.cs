using System.Collections.Generic;
using Unity.UIWidgets.animation;
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
    }

    public interface MovieClipObjectWithProperty<T> {
        T getProperty(float t);
        void animateTo(T target, float startTime, float duration, T from, Curve curve);
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
    }
}