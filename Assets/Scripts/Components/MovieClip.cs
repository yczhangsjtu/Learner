using System;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.widgets;

/*
 * 
 */
public class MovieClip : StatefulWidget {
    public readonly float? width;
    public readonly float? height;

    public MovieClip(
        Key key = null,
        MovieClipData movieClipData = null,
        float? width = null,
        float? height = null
    ) : base(key: key) {
        this.width = width;
        this.height = height;
        this.data = movieClipData;
    }

    public readonly MovieClipData data;
    
    public override State createState() {
        return new MovieClipState();
    }
}

internal class MovieClipState : TickerProviderStateMixin<MovieClip> {
    private AnimationController controller;

    public override void initState() {
        base.initState();
        if (this.widget.data != null) {
            controller = new AnimationController(
                duration: TimeSpan.FromSeconds(widget.data.duration),
                vsync: this);
        }
        controller.forward();
    }

    public override void dispose() {
        controller?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
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
    public MovieClipData(List<MovieClipDataFrame> frames) {
        this.states = instantiateFrames(frames);
    }
    
    public float duration => _duration;

    readonly float _duration;

    private readonly List<MovieClipDataState> states;

    private List<MovieClipDataState> instantiateFrames(List<MovieClipDataFrame> frames) {
        return null;
    }

    public Widget build(BuildContext context, float t) {
        D.assert(t >= 0.0f && t <= duration);
        return new Container();
    }
}

public abstract class MovieClipObjectBuilder {
    public MovieClipObjectBuilder(string id) {
        D.assert(id != null);
        this.id = id;
    }
    
    public readonly string id;
    
    public abstract Widget build(BuildContext context);

    public void changeProperty() {
        
    }
}

public class MovieClipDataState {
    private Dictionary<string, MovieClipObjectBuilder> objects = new Dictionary<string, MovieClipObjectBuilder>();

    public bool addObject(string id, MovieClipObjectBuilder objectBuilder) {
        if (id == null || objectBuilder == null) {
            return false;
        }
        if (objects.ContainsKey(id)) {
            return false;
        }

        objects[id] = objectBuilder;
        return true;
    }

    public bool removeObject(string id) {
        if (id == null) {
            return false;
        }

        if (!objects.ContainsKey(id)) {
            return false;
        }

        objects.Remove(id);
        return true;
    }

    public bool updateObject(string id, MovieClipObjectBuilder objectBuilder) {
        if (id == null || objectBuilder == null) {
            return false;
        }
        if (!objects.ContainsKey(id)) {
            return false;
        }

        objects[id] = objectBuilder;
        return true;
    }
}

public class MovieClipDataFrame {
    
}