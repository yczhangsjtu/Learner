using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

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

    public class MovieClipSnapshot {
        private readonly Dictionary<string, MovieClipObject> objects =
            new Dictionary<string, MovieClipObject>();

        public readonly float timestamp;

        private List<MovieClipObject> sortedObjects;

        public MovieClipSnapshot(float timestamp) {
            this.timestamp = timestamp;
        }

        public MovieClipSnapshot(Dictionary<string, MovieClipObject> objects, float timestamp) {
            this.objects = objects;
            this.timestamp = timestamp;
        }

        public bool tryGetObject(string id, out MovieClipObject @object) {
            if (id == null) {
                @object = null;
                return false;
            }

            return objects.TryGetValue(id, out @object);
        }

        public MovieClipObject getObject(string id) {
            if (tryGetObject(id, out var obj)) return obj;

            return null;
        }

        public bool addObject(string id, MovieClipObject @object) {
            if (id == null || @object == null) return false;
            if (objects.ContainsKey(id)) return false;

            objects[id] = @object;
            sortedObjects = null;
            return true;
        }

        public bool removeObject(string id) {
            if (id == null) return false;

            if (!objects.ContainsKey(id)) return false;

            objects.Remove(id);
            sortedObjects = null;
            return true;
        }

        public bool updateObject(string id, MovieClipObject @object) {
            if (id == null || @object == null) return false;
            if (!objects.ContainsKey(id)) return false;

            objects[id] = @object;
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
                newTimestamp
            );
            modifier(snapshot);
            
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
                    Size scale = obj.scale.evaluate(t);
                    float rotation = obj.rotation.evaluate(t);
                    Matrix3 transform = Matrix3.makeRotate(rotation);
                    transform.postScale(scale.width, scale.height);
                    return new Positioned(
                        child: new Transform(
                            child: obj.build(context, t),
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