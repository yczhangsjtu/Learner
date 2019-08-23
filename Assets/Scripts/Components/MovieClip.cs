using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal.Commands;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
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
    private readonly List<MovieClipDataState> states;

    public MovieClipData(List<MovieClipDataFrame> frames) {
        if(frames != null && frames.isNotEmpty())
            states = instantiateFrames(frames);
    }

    public float duration { get; }

    public Widget build(BuildContext context, float t) {
        D.assert(t >= 0.0f && t <= duration);
        return new Container();
    }

    private static List<MovieClipDataState> instantiateFrames(List<MovieClipDataFrame> frames) {
        MovieClipDataState state = new MovieClipDataState();
        List<MovieClipDataState> states = new List<MovieClipDataState>();
        for (int i = 0; i < frames.Count; i++) {
            state = frames[i].applyTo(state);
            states.Add(state);
        }
        return states;
    }
}


public abstract class MovieClipObjectBuilder : ICloneable {
    public readonly string id;

    public MovieClipObjectBuilder(string id) {
        D.assert(id != null);
        this.id = id;
    }

    public abstract object Clone();

    public abstract Widget build(BuildContext context, float t);
}

public delegate void MovieClipDataStateModifier(MovieClipDataState state);

public class MovieClipDataState {
    private readonly Dictionary<string, MovieClipObjectBuilder> objects =
        new Dictionary<string, MovieClipObjectBuilder>();

    public MovieClipDataState() {}

    public MovieClipDataState(Dictionary<string, MovieClipObjectBuilder> objects) {
        this.objects = objects;
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
        return true;
    }

    public bool removeObject(string id) {
        if (id == null) return false;

        if (!objects.ContainsKey(id)) return false;

        objects.Remove(id);
        return true;
    }

    public bool updateObject(string id, MovieClipObjectBuilder objectBuilder) {
        if (id == null || objectBuilder == null) return false;
        if (!objects.ContainsKey(id)) return false;

        objects[id] = objectBuilder;
        return true;
    }

    public MovieClipDataState copyWith(MovieClipDataStateModifier modifier) {
        var state = new MovieClipDataState(
            objects.ToDictionary(
                entry => entry.Key,
                entry => (MovieClipObjectBuilder) entry.Value.Clone()
            )
        );
        modifier(state);
        return state;
    }
}

public class MovieClipDataFrame {
    public MovieClipDataFrame(MovieClipDataStateModifier modifier) {
        this.modifier = modifier;
    }

    public readonly MovieClipDataStateModifier modifier;

    public MovieClipDataState applyTo(MovieClipDataState state) {
        return state.copyWith(modifier);
    }
}