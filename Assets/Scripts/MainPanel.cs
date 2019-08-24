using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Components;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.painting;

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
                                    snapshot.createTextObject("text", "Hello World",
                                        position: new Offset(300, 100),
                                        animation: AppearAnimation.fadeIn);
                                }
                            ),
                            new MovieClipDataFrame(
                                2, snapshot => {
                                    snapshot.moveObject("text", new Offset(100, 0), duration: 1);
                                    snapshot.scaleObjectTo("text", new Size(2, 2));
                                    snapshot.animateTo<TextStyle>("text", style => style.copyWith(color: Colors.green));
                                }
                            ),
                            new MovieClipDataFrame(
                                2, snapshot => {
                                    snapshot.moveObject("text", new Offset(0, 100), duration: 1);
                                    snapshot.rotateObjectTo("text", 2 * Mathf.PI, duration: 3);
                                    snapshot.animateTo<TextStyle>("text",
                                        style => style.copyWith(letterSpacing: 5.0f));
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
                                    snapshot.moveObject("rect", new Offset(200, 0));
                                }
                            ),
                        }
                    )
                )
            );
        }
    }
}
