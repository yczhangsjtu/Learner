using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

/*
 * 
 */
public class MovieClip : StatefulWidget {
    public readonly MovieClipData data;
    public readonly float? height;
    public readonly float? width;

    public MovieClip(
        Key key = null,
        MovieClipData movieClipData = null,
        float? width = null,
        float? height = null
    ) : base(key) {
        this.width = width;
        this.height = height;
        data = movieClipData;
    }

    public override State createState() {
        return new MovieClipState();
    }
}

internal class MovieClipState : TickerProviderStateMixin<MovieClip> {
    private AnimationController controller;

    public override void initState() {
        base.initState();
        if (widget.data != null)
            controller = new AnimationController(
                duration: TimeSpan.FromSeconds(widget.data.duration),
                vsync: this);
        controller.forward();
    }

    public override void dispose() {
        controller?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context) {
        return new SizedBox(
            width: widget.width,
            height: widget.height,
            child: controller != null
                ? new AnimatedBuilder(
                    animation: controller,
                    builder: (buildContext, child) => widget.data.build(buildContext, controller.value))
                : null
        );
    }
}

public interface MovieClipProvider {
    float duration { get; }

    Widget build(BuildContext context, float t);
}

public class MovieClipData : MovieClipProvider {
    private readonly List<MovieClipSnapshot> snapshots = new List<MovieClipSnapshot>();
    private int lastFoundIndex = -1;
    private float lastQueriedTime = -1;

    public MovieClipData(List<MovieClipDataFrame> frames) {
        if(frames != null && frames.isNotEmpty())
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


public abstract class MovieClipObjectBuilder : ICloneable {
    public readonly string id;

    public int layer
    {
        get { return _layer; }
    }

    private int _layer;

    public int index {
        get { return _index; }
    }

    private int _index;
    private static int currentIndex = 0;

    public OffsetPropertyData position;

    public MovieClipObjectBuilder(
        string id,
        int layer = 0,
        Offset position = null) {
        D.assert(id != null);
        this.id = id;
        this._layer = layer;
        this._index = currentIndex++;
        if (position != null) {
            this.position = new ConstantOffsetProperty(position);
        }
        else {
            this.position = new ConstantOffsetProperty(Offset.zero);
        }
    }

    public abstract object Clone();

    protected void copyInternalFrom(MovieClipObjectBuilder builder) {
        _index = builder.index;
        _layer = builder.layer;
        this.position = builder.position;
    }

    public abstract Widget build(BuildContext context, float t);
}

public delegate void MovieClipDataSnapshotModifier(MovieClipSnapshot snapshot);

public class MovieClipSnapshot {
    private readonly Dictionary<string, MovieClipObjectBuilder> objects =
        new Dictionary<string, MovieClipObjectBuilder>();

    public readonly float timestamp;

    private List<MovieClipObjectBuilder> sortedObjects;

    public MovieClipSnapshot(float timestamp) {
        this.timestamp = timestamp;
    }

    public MovieClipSnapshot(Dictionary<string, MovieClipObjectBuilder> objects, float timestamp) {
        this.objects = objects;
        this.timestamp = timestamp;
    }

    public bool tryGetObject(string id, out MovieClipObjectBuilder objectBuilder) {
        if (id == null) {
            objectBuilder = null;
            return false;
        }

        return objects.TryGetValue(id, out objectBuilder);
    }

    public MovieClipObjectBuilder getObject(string id) {
        if (tryGetObject(id, out var obj)) return obj;

        return null;
    }

    public bool addObject(string id, MovieClipObjectBuilder objectBuilder) {
        if (id == null || objectBuilder == null) return false;
        if (objects.ContainsKey(id)) return false;

        objects[id] = objectBuilder;
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

    public bool updateObject(string id, MovieClipObjectBuilder objectBuilder) {
        if (id == null || objectBuilder == null) return false;
        if (!objects.ContainsKey(id)) return false;

        objects[id] = objectBuilder;
        sortedObjects = null;
        return true;
    }

    public MovieClipSnapshot copyWith(MovieClipDataSnapshotModifier modifier, float duration) {
        var snapshot = new MovieClipSnapshot(
            objects.ToDictionary(
                entry => entry.Key,
                entry => (MovieClipObjectBuilder) entry.Value.Clone()
            ),
            timestamp + duration
        );
        modifier(snapshot);
        return snapshot;
    }

    public Widget build(BuildContext context, float t) {
        if (sortedObjects == null) {
            sortedObjects = objects.Values.ToList();
            sortedObjects.Sort((MovieClipObjectBuilder a, MovieClipObjectBuilder b) => {
                if (a.layer != b.layer) {
                    return a.layer - b.layer;
                }
                return a.index - b.index;
            });
        }
        return new Stack(
            children: sortedObjects.Select<MovieClipObjectBuilder, Widget>((builder) => {
                Offset position = builder.position.evaluate(t);
                return new Positioned(
                    child: builder.build(context, t),
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