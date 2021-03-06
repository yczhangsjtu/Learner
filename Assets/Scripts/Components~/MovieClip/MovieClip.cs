﻿using System;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.widgets;

/*
 * 
 */
namespace Learner.Components {
    public class MovieClip : StatefulWidget {
        public readonly MovieClipData data;
        public readonly float? height;
        public readonly float? width;
        public readonly float startFrom;

        public MovieClip(
            Key key = null,
            MovieClipData movieClipData = null,
            float startFrom = 0,
            float? width = null,
            float? height = null
        ) : base(key) {
            this.width = width;
            this.height = height;
            data = movieClipData;
            this.startFrom = startFrom;
        }

        public override State createState() {
            return new MovieClipState();
        }
    }

    internal class MovieClipState : TickerProviderStateMixin<MovieClip> {
        private AnimationController controller;
        private Animation<float> animation;

        public override void initState() {
            base.initState();
            if (widget.data != null) {
                controller = new AnimationController(
                    duration: TimeSpan.FromSeconds(widget.data.duration),
                    vsync: this);
                animation = new FloatTween(begin: 0, end: widget.data.duration).animate(controller);
                controller.forward(from: widget.startFrom / widget.data.duration);
            }
        }

        public override void dispose() {
            controller?.dispose();
            base.dispose();
        }

        public override Widget build(BuildContext context) {
            return new SizedBox(
                width: widget.width ?? MediaQuery.of(context).size.width,
                height: widget.height ?? MediaQuery.of(context).size.height,
                child: controller != null
                    ? new AnimatedBuilder(
                        animation: animation,
                        builder: (buildContext, child) => widget.data.build(buildContext, animation.value))
                    : null
            );
        }
    }
}