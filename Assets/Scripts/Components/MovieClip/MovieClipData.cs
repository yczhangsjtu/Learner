using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;
using Transform = Unity.UIWidgets.widgets.Transform;

namespace Learner.Components {
    public interface MovieClipProvider {
        float duration { get; }

        Widget build(BuildContext context, float t);
    }

    public class MovieClipData : MovieClipProvider {
        private readonly List<MovieClipSnapshot> snapshots = new List<MovieClipSnapshot>();
        private int lastFoundIndex = -1;
        private float lastQueriedTime = -1;

        public MovieClipData(
            List<MovieClipDataFrame> frames,
            bool debugEnabled = false
        ) {
            if (frames != null && frames.isNotEmpty())
                instantiateFrames(frames);
            if (debugEnabled) {
                enableDebug();
            }
        }

        public float duration {
            get { return _duration; }
            set { _duration = value; }
        }

        public float _duration;

        public Widget build(BuildContext context, float t) {
            D.assert(t >= 0.0f && t <= duration);
            if (snapshots.isEmpty()) {
                return new Container();
            }

            int snapshotIndex = findSnapshotIndex(t);
            var snapshot = snapshots[snapshotIndex];
            return snapshot.build(context, t);
        }

        public int findSnapshotIndex(float t) {
            int startIndex = 0, endIndex = snapshots.Count;
            if (lastFoundIndex >= 0) {
                if (lastQueriedTime <= t) {
                    startIndex = lastFoundIndex;
                }
                else {
                    endIndex = lastFoundIndex + 1;
                }
            }

            lastQueriedTime = t;
            for (int i = startIndex; i < endIndex; i++) {
                if (snapshots[i].timestamp > t) {
                    lastFoundIndex = i;
                    return lastFoundIndex;
                }
            }

            lastFoundIndex = snapshots.Count - 1;
            return lastFoundIndex;
        }

        private void instantiateFrames(List<MovieClipDataFrame> frames) {
            MovieClipSnapshot snapshot = new MovieClipSnapshot(0);
            snapshots.Clear();
            _duration = 0;
            foreach (var frame in frames) {
                snapshot = frame.applyTo(snapshot);
                snapshots.Add(snapshot);
                _duration += frame.duration;
            }
        }

        public void enableDebug() {
            foreach (var snapshot in snapshots) {
                snapshot.enableDebug();
            }
        }

        public void disableDebug() {
            foreach (var snapshot in snapshots) {
                snapshot.disableDebug();
            }
        }
    }


    public delegate void MovieClipDataSnapshotModifier(MovieClipSnapshot snapshot);

    public enum AppearAnimation {
        none,
        fadeIn,
        scale,
        overScale,
        fromLeft,
        fromRight,
        fromTop,
        fromBottom,
    }
    
    public enum DisappearAnimation {
        none,
        fadeOut,
        scale,
        overScale,
        toLeft,
        toRight,
        toTop,
        toBottom,
    }

    public class MovieClipSnapshot {
        private readonly Dictionary<string, MovieClipObject> objects =
            new Dictionary<string, MovieClipObject>();

        public float timestamp {
            get { return _timestamp; }
        }

        private float _timestamp;
        private float _defaultDuration;
        private bool _debugEnabled;

        private List<MovieClipObject> sortedObjects;

        public const float kDefaultAppearTime = 0.3f;
        public const float kDefaultAppearDistance = 1000;
        public const float kDefaultDisappearTime = 0.3f;
        public const float kDefaultDisappearDistance = 1000;

        public MovieClipSnapshot(float timestamp) {
            this._timestamp = timestamp;
        }

        public MovieClipSnapshot(Dictionary<string, MovieClipObject> objects, float timestamp) {
            this.objects = objects;
            this._timestamp = timestamp;
        }

        public bool tryGetObject(string id, out MovieClipObject obj) {
            if (id == null) {
                obj = null;
                return false;
            }

            return objects.TryGetValue(id, out obj);
        }

        public MovieClipObject getObject(string id) {
            if (tryGetObject(id, out var obj)) return obj;

            return null;
        }

        public bool addObject(MovieClipObject obj) {
            if (obj?.id == null) return false;
            if (objects.ContainsKey(obj.id)) return false;

            objects[obj.id] = obj;
            sortedObjects = null;
            return true;
        }

        public bool createObject(
            MovieClipObject obj,
            Offset position = null,
            Offset pivot = null,
            Size scale = null,
            float rotation = 0,
            float opacity = 1,
            float delay = 0,
            AppearAnimation animation = AppearAnimation.none,
            float appearTime = kDefaultAppearTime) {
            if (!addObject(obj)) return false;

            obj.initConstants(
                position: position,
                pivot: pivot,
                scale: scale,
                rotation: rotation,
                opacity: opacity);
            var targetPosition = position ?? Offset.zero;
            switch (animation) {
                case AppearAnimation.none:
                    break;
                case AppearAnimation.fadeIn:
                    obj.opacityTo(opacity, timestamp + delay, appearTime, 0);
                    break;
                case AppearAnimation.scale:
                    obj.scaleTo(scale ?? new Size(1, 1),
                        timestamp + delay,
                        appearTime,
                        new Size(0, 0));
                    break;
                case AppearAnimation.overScale:
                    obj.scaleTo(scale ?? new Size(1, 1),
                        timestamp + delay,
                        appearTime,
                        new Size(0, 0),
                        Curves.easeOutBack);
                    break;
                case AppearAnimation.fromTop:
                    obj.moveTo(targetPosition,
                        timestamp + delay,
                        appearTime,
                        targetPosition - new Offset(0, kDefaultAppearDistance));
                    break;
                case AppearAnimation.fromBottom:
                    obj.moveTo(targetPosition,
                        timestamp + delay,
                        appearTime,
                        targetPosition + new Offset(0, kDefaultAppearDistance));
                    break;
                case AppearAnimation.fromLeft:
                    obj.moveTo(targetPosition,
                        timestamp + delay,
                        appearTime,
                        targetPosition - new Offset(kDefaultAppearDistance, 0));
                    break;
                case AppearAnimation.fromRight:
                    obj.moveTo(targetPosition,
                        timestamp + delay,
                        appearTime,
                        targetPosition + new Offset(kDefaultAppearDistance, 0));
                    break;
            }

            return true;
        }

        public bool removeObject(string id) {
            if (id == null) return false;

            if (!objects.ContainsKey(id)) return false;

            objects.Remove(id);
            sortedObjects = null;
            return true;
        }

        public bool destroyObject(
            string id,
            float delay = 0,
            DisappearAnimation animation = DisappearAnimation.none,
            float disappearTime = kDefaultDisappearTime
        ) {
            var obj = getObject(id);
            if (obj == null) {
                return false;
            }
            switch (animation) {
                case DisappearAnimation.none:
                    break;
                case DisappearAnimation.fadeOut:
                    obj.opacityTo(0, timestamp + delay, disappearTime);
                    break;
                case DisappearAnimation.scale:
                    obj.scaleTo(Size.zero,
                        timestamp + delay,
                        disappearTime);
                    break;
                case DisappearAnimation.overScale:
                    obj.scaleTo(Size.zero,
                        timestamp + delay,
                        disappearTime,
                        curve: Curves.easeOutBack.flipped);
                    break;
                case DisappearAnimation.toTop:
                    obj.move(-new Offset(0, kDefaultDisappearDistance),
                        timestamp + delay,
                        disappearTime);
                    break;
                case DisappearAnimation.toBottom:
                    obj.moveTo(new Offset(0, kDefaultDisappearDistance),
                        timestamp + delay,
                        disappearTime);
                    break;
                case DisappearAnimation.toLeft:
                    obj.moveTo(-new Offset(kDefaultDisappearDistance, 0),
                        timestamp + delay,
                        disappearTime);
                    break;
                case DisappearAnimation.toRight:
                    obj.moveTo(new Offset(kDefaultDisappearDistance, 0),
                        timestamp + delay,
                        disappearTime);
                    break;
            }
            obj.dieAt(timestamp + disappearTime + delay);
            return true;
        }

        public bool updateObject(MovieClipObject obj) {
            if (obj?.id == null) return false;
            if (!objects.ContainsKey(obj.id)) return false;

            objects[obj.id] = obj;
            sortedObjects = null;
            return true;
        }
        
        public bool moveObjectTo(
            string id,
            Offset position,
            float delay = 0,
            float? duration = null,
            Offset fromPosition = null,
            Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.moveTo(position,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                fromPosition: fromPosition,
                curve: curve);
            return true;
        }

        public bool moveObject(
            string id,
            Offset offset,
            float delay = 0,
            float? duration = null,
            Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.move(offset,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                curve: curve);
            return true;
        }
        
        public bool objectPivotTo(string id, Offset pivot, float delay = 0, float? duration = null, Offset fromPosition = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.pivotTo(pivot,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                fromPosition: fromPosition,
                curve: curve);
            return true;
        }

        public bool objectPivotChangeBy(string id, Offset offset, float delay = 0, float? duration = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.pivotChangeBy(offset,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                curve: curve);
            return true;
        }

        public bool rotateObjectTo(string id, float rotation, float delay = 0, float? duration = null, float? fromRotation = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.rotateTo(rotation,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                fromRotation: fromRotation,
                curve: curve);
            return true;
        }

        public bool rotateObjectBy(string id, float rotation, float delay = 0, float? duration = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.rotateBy(rotation,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                curve: curve);
            return true;
        }

        public bool scaleObjectTo(string id, Size scale, float delay = 0, float? duration = null, Size fromScale = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.scaleTo(scale,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                fromScale: fromScale,
                curve: curve);
            return true;
        }

        public bool scaleObjectBy(string id, Size scale, float delay = 0, float? duration = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.scaleBy(scale,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                curve: curve);
            return true;
        }

        public bool objectOpacityTo(string id, float opacity, float delay = 0, float? duration = null, float? fromOpacity = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.opacityTo(opacity,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                fromOpacity: fromOpacity,
                curve: curve);
            return true;
        }

        public bool objectOpacityChangeBy(string id, float delta, float delay = 0, float? duration = null, Curve curve = null) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.opacityChangeBy(delta,
                startTime: timestamp + delay,
                duration: duration ?? Mathf.Max(0.1f, _defaultDuration - delay),
                curve: curve);
            return true;
        }

        public delegate T PropertyProvider<T>(T value);

        public bool animateTo<T>(
            string id,
            PropertyProvider<T> target,
            float delay = 0,
            float? duration = null,
            PropertyProvider<T> from = null,
            Curve curve = null) where T : class {
            var obj = getObject(id);
            if (obj == null) return false;
            if (obj is MovieClipObjectWithProperty<T> objWithProperty) {
                var f = objWithProperty.getProperty(timestamp + delay);
                objWithProperty.animateTo(
                    target: target(f),
                    startTime: timestamp + delay,
                    duration: duration ?? _defaultDuration,
                    from: @from?.Invoke(f),
                    curve: curve);
                return true;
            }

            return false;
        }

        public bool animateTo<T>(
            string id,
            string paramName,
            PropertyProvider<T> target,
            float delay = 0,
            float? duration = null,
            PropertyProvider<T> from = null,
            Curve curve = null) where T : class {
            var obj = getObject(id);
            if (obj == null) return false;
            if (obj is BuilderMovieClipObject builderObject) {
                var f = builderObject.getParameter(paramName, timestamp + delay) as T;
                return builderObject.animateTo(
                    paramName,
                    target: target(f),
                    startTime: timestamp + delay,
                    duration: duration ?? _defaultDuration,
                    useFrom: from != null,
                    from: @from?.Invoke(f),
                    curve: curve);
            }

            return false;
        }

        public MovieClipSnapshot copyWith(MovieClipDataSnapshotModifier modifier, float duration) {
            D.assert(duration > 0);
            Dictionary<string, MovieClipObject> updatedDictionary = new Dictionary<string, MovieClipObject>();
            float newTimestamp = timestamp + duration;
            foreach (var entry in objects) {
                var obj = entry.Value;
                if(obj.deathTime == null || obj.deathTime > newTimestamp)
                    updatedDictionary.Add(entry.Key, entry.Value);
            }
            var snapshot = new MovieClipSnapshot(
                objects.ToDictionary(
                    entry => entry.Key,
                    entry => (MovieClipObject) entry.Value.Clone()
                ),
                timestamp
            );
            snapshot._defaultDuration = duration;
            modifier(snapshot);
            snapshot._timestamp = newTimestamp;
            
            return snapshot;
        }

        public bool debugObject(string id, string propertyName) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.debugProperty(propertyName);
            return true;
        }

        public bool setDebugObjectOffset(string id, Offset offset) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.debugOffset = offset;
            return true;
        }
        
        public bool debugObjectAll(string id) {
            var obj = getObject(id);
            if (obj == null) return false;
            obj.debugAll();
            return true;
        }

        public void enableDebug() {
            _debugEnabled = true;
        }

        public void disableDebug() {
            _debugEnabled = false;
        }
        
        private static readonly TextStyle _debugStyle = new TextStyle(
            color: Colors.red,
            fontSize: 12
        );

        public Widget build(BuildContext context, float t) {
            if (sortedObjects == null) {
                sortedObjects = objects.Values.ToList();
                sortedObjects.Sort((MovieClipObject a, MovieClipObject b) => {
                    if (a.layer != b.layer) {
                        return a.layer - b.layer;
                    }

                    return a.index - b.index;
                });
            }

            var children = sortedObjects
                .Where(obj => obj.deathTime == null || obj.deathTime.Value > t)
                .Select<MovieClipObject, Widget>((obj) => {
                    Offset position = obj.position.evaluate(t);
                    Offset pivot = obj.pivot.evaluate(t);
                    Size scale = obj.scale.evaluate(t);
                    float rotation = obj.rotation.evaluate(t);
                    float opacity = obj.opacity.evaluate(t);
                    Matrix3 transform = Matrix3.makeRotate(rotation);
                    transform.postScale(scale.width, scale.height);
                    return new Positioned(
                        child: new Transform(
                            child: new Opacity(
                                child: new FractionalTranslation(
                                    translation: -pivot,
                                    child: obj.build(context, t)
                                ),
                                opacity: opacity
                            ),
                            transform: transform
                        ),
                        left: position.dx,
                        top: position.dy
                    );
            }).ToList();

            if (_debugEnabled) {
                foreach(var obj in sortedObjects) {
                    Offset position = obj.position.evaluate(t);
                    if (obj.deathTime != null && obj.deathTime <= t) {
                        continue;
                    }
                    children.Add(new Positioned(
                        child: new Text(obj.debugString(t), style: _debugStyle),
                        left: position.dx + obj.debugOffset.dx,
                        top: position.dy + obj.debugOffset.dy
                    ));
                }
                children.Add(new Text(
                    $"Current time is: {t}",
                    style: _debugStyle
                ));
            }

            return new Stack(
                children: children
            );
        }
    }

    public class MovieClipDataFrame {
        public MovieClipDataFrame(float duration, MovieClipDataSnapshotModifier modifier) {
            D.assert(duration > 0);
            this.modifier = modifier;
            this.duration = duration;
        }

        public readonly float duration;

        public readonly MovieClipDataSnapshotModifier modifier;

        public MovieClipSnapshot applyTo(MovieClipSnapshot snapshot) {
            return snapshot.copyWith(modifier, duration);
        }
    }
}