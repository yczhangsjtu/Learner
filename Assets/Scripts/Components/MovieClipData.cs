using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Transform = Unity.UIWidgets.widgets.Transform;

namespace Components {
    public interface MovieClipProvider {
        float duration { get; }

        Widget build(BuildContext context, float t);
    }

    public class MovieClipData : MovieClipProvider {
        private readonly List<MovieClipSnapshot> snapshots = new List<MovieClipSnapshot>();
        private int lastFoundIndex = -1;
        private float lastQueriedTime = -1;

        public MovieClipData(List<MovieClipDataFrame> frames) {
            if (frames != null && frames.isNotEmpty())
                instantiateFrames(frames);
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

    public class MovieClipSnapshot {
        private readonly Dictionary<string, MovieClipObject> objects =
            new Dictionary<string, MovieClipObject>();

        public float timestamp {
            get { return _timestamp; }
        }

        private float _timestamp;

        private List<MovieClipObject> sortedObjects;

        public const float kDefaultAppearTime = 0.3f;
        public const float kDefaultAppearDistance = 1000;

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
            Size scale = null,
            float rotation = 0,
            float opacity = 1,
            AppearAnimation animation = AppearAnimation.none,
            float appearTime = kDefaultAppearTime) {
            if (!addObject(obj)) return false;

            obj.initConstantPosition(position);
            obj.initConstantScale(scale);
            obj.initConstantRotation(rotation);
            var targetPosition = position ?? Offset.zero;
            switch (animation) {
                case AppearAnimation.none:
                    break;
                case AppearAnimation.fadeIn:
                    obj.opacityTo(opacity, timestamp, appearTime, 0);
                    break;
                case AppearAnimation.scale:
                    obj.scaleTo(scale ?? new Size(1, 1),
                        timestamp,
                        appearTime,
                        new Size(0, 0));
                    break;
                case AppearAnimation.overScale:
                    obj.scaleTo(scale ?? new Size(1, 1),
                        timestamp,
                        appearTime,
                        new Size(0, 0),
                        Curves.easeOutBack);
                    break;
                case AppearAnimation.fromTop:
                    obj.moveTo(targetPosition,
                        timestamp,
                        appearTime,
                        targetPosition - new Offset(0, kDefaultAppearDistance));
                    break;
                case AppearAnimation.fromBottom:
                    obj.moveTo(targetPosition,
                        timestamp,
                        appearTime,
                        targetPosition + new Offset(0, kDefaultAppearDistance));
                    break;
                case AppearAnimation.fromLeft:
                    obj.moveTo(targetPosition,
                        timestamp,
                        appearTime,
                        targetPosition - new Offset(kDefaultAppearDistance, 0));
                    break;
                case AppearAnimation.fromRight:
                    obj.moveTo(targetPosition,
                        timestamp,
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

        public bool updateObject(MovieClipObject obj) {
            if (obj?.id == null) return false;
            if (!objects.ContainsKey(obj.id)) return false;

            objects[obj.id] = obj;
            sortedObjects = null;
            return true;
        }

        public MovieClipSnapshot copyWith(MovieClipDataSnapshotModifier modifier, float duration) {
            D.assert(duration > 0);
            Dictionary<string, MovieClipObject> updatedDictionary = new Dictionary<string, MovieClipObject>();
            float newTimestamp = timestamp + duration;
            foreach (var id in objects.Keys) {
                var obj = objects[id];
                if(obj.deathTime == null || obj.deathTime > newTimestamp)
                    updatedDictionary[id] = obj;
            }
            var snapshot = new MovieClipSnapshot(
                objects.ToDictionary(
                    entry => entry.Key,
                    entry => (MovieClipObject) entry.Value.Clone()
                ),
                timestamp
            );
            modifier(snapshot);
            snapshot._timestamp = newTimestamp;
            
            return snapshot;
        }

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

            return new Stack(
                children: sortedObjects.Select<MovieClipObject, Widget>((obj) => {
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
                }).ToList()
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