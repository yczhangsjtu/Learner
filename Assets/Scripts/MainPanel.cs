using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
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
                                0.5f, snapshot => {
                                    var obj = new BasicMovieClipObject(
                                        "text",
                                        child: new Text("Hello World")
                                    );
                                    snapshot.createObject(obj,
                                        position: new Offset(100, 100),
                                        animation: AppearAnimation.fadeIn);
                                }
                            ),
                            new MovieClipDataFrame(
                                2, snapshot => {
                                    var obj = snapshot.getObject("text");
                                    obj.move(new Offset(100, 0), snapshot.timestamp, 1);
                                    obj?.scaleTo(new Size(2, 2), snapshot.timestamp, 2);
                                }
                            ),
                            new MovieClipDataFrame(
                                2, snapshot => {
                                    var obj = snapshot.getObject("text");
                                    obj.move(new Offset(0, 100), snapshot.timestamp, 1);
                                    obj?.rotateTo(2 * Mathf.PI, snapshot.timestamp, 2);
                                    snapshot.createObject(new BasicMovieClipObject(
                                            "rect",
                                            child: new Container(
                                                color: Colors.blue,
                                                child: new SizedBox(
                                                    width: 50,
                                                    height: 50
                                                )
                                            )
                                        ),
                                        position: new Offset(100, 100),
                                        animation: AppearAnimation.overScale
                                    );
                                }
                            ),
                            new MovieClipDataFrame(
                                2, snapshot => {
                                    snapshot.destroyObject("text", animation: DisappearAnimation.overScale);
                                    var obj = snapshot.getObject("rect");
                                    obj.move(new Offset(200, 0), snapshot.timestamp, 2);
                                }
                            ),
                        }
                    )
                )
            );
        }
    }
}
