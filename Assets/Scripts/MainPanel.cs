using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEngine;

public class MainPanel : UIWidgetsPanel
{
    
    protected override Widget createWidget() {
        return new WidgetsApp(
            home: new MainApp(),
            pageRouteBuilder: (RouteSettings settings, WidgetBuilder builder) =>
                new PageRouteBuilder(
                    settings: settings,
                    pageBuilder: (BuildContext context, Animation<float> animation,
                        Animation<float> secondaryAnimation) => builder(context)
                )
        );
    }

    class MainApp : StatefulWidget {
        public MainApp(Key key = null) : base(key) {
        }

        public override State createState() {
            return new MainAppState();
        }
    }

    class MainAppState : State<MainApp> {
        int counter = 0;

        public override Widget build(BuildContext context) {
            return new Scaffold(
                appBar: new AppBar()
            );
        }
    }
}
