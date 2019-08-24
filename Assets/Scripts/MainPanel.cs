using System;
using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Components;
using Unity.UIWidgets.ui;

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
        public override Widget build(BuildContext context) {
            return new Scaffold(
                body: new MovieClip(
                    movieClipData: new MovieClipData(
                        new List<MovieClipDataFrame>{
                            new MovieClipDataFrame(
                                1, snapshot => {
                                    var obj = new BasicMovieClipObject(
                                        "rect",
                                        child: new Text("Hello World", style: new TextStyle(
                                            color: Colors.red,
                                            fontSize: 32
                                        ))
                                    );
                                    snapshot.addObject("rect", obj);
                                    obj.moveTo(new Offset(100, 100), 0, 1);
                                }
                            ),
                            new MovieClipDataFrame(
                                2, snapshot => {
                                    var obj = snapshot.getObject("rect");
                                    obj?.scaleTo(new Size(2, 2), 2, 2);
                                }
                            ),
                            new MovieClipDataFrame(
                                2, snapshot => {
                                    var obj = snapshot.getObject("rect");
                                    obj?.rotateTo(2 * Mathf.PI, 3, 2);
                                }
                            ),
                        }
                    )
                )
            );
        }
    }
}
