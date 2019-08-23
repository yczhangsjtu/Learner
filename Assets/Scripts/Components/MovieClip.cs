using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using NUnit.Framework.Internal.Commands;
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
    private readonly List<MovieClipDataState> states = new List<MovieClipDataState>();
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
        if (states.isEmpty()) {
            return new Container();
        }

        int stateIndex = findStateIndex(t);
        var state = states[stateIndex];
        return state.build(context, t);
    }

    public int findStateIndex(float t) {
        int startIndex = 0, endIndex = states.Count;
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
            if (states[i].timestamp > t) {
                lastFoundIndex = i;
                return lastFoundIndex;
            }
        }

        lastFoundIndex = states.Count - 1;
        return lastFoundIndex;
    }

    private void instantiateFrames(List<MovieClipDataFrame> frames) {
        MovieClipDataState state = new MovieClipDataState(0);
        states.Clear();
        _duration = 0;
        foreach (var frame in frames) {
            state = frame.applyTo(state);
            states.Add(state);
            _duration += frame.duration;
        }
    }
}


public abstract class MovieClipObjectBuilder : ICloneable {
    public readonly string id;
    public readonly int layer;
    public readonly int index;
    private static int currentIndex = 0;

    public OffsetPropertyData position;

    public MovieClipObjectBuilder(string id, int layer = 0, Offset position = null) {
        D.assert(id != null);
        this.id = id;
        this.layer = layer;
        this.index = currentIndex++;
        if (position != null) {
            this.position = new ConstantOffsetProperty(position);
        }
        else {
            this.position = new ConstantOffsetProperty(Offset.zero);
        }
    }

    public abstract object Clone();

    public abstract Widget build(BuildContext context, float t);
}

public delegate void MovieClipDataStateModifier(MovieClipDataState state);

public class MovieClipDataState {
    private readonly Dictionary<string, MovieClipObjectBuilder> objects =
        new Dictionary<string, MovieClipObjectBuilder>();

    public readonly float timestamp;

    private List<MovieClipObjectBuilder> sortedObjects;

    public MovieClipDataState(float timestamp) {
        this.timestamp = timestamp;
    }

    public MovieClipDataState(Dictionary<string, MovieClipObjectBuilder> objects, float timestamp) {
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

    public MovieClipDataState copyWith(MovieClipDataStateModifier modifier, float duration) {
        var state = new MovieClipDataState(
            objects.ToDictionary(
                entry => entry.Key,
                entry => (MovieClipObjectBuilder) entry.Value.Clone()
            ),
            timestamp + duration
        );
        modifier(state);
        return state;
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
    public MovieClipDataFrame(float duration, MovieClipDataStateModifier modifier) {
        D.assert(duration > 0);
        this.modifier = modifier;
        this.duration = duration;
    }

    public readonly float duration;

    public readonly MovieClipDataStateModifier modifier;

    public MovieClipDataState applyTo(MovieClipDataState state) {
        return state.copyWith(modifier, duration);
    }
}