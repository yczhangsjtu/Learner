using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Learner.Components;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.painting;
using UnityEngine.UIElements;
using Align = Unity.UIWidgets.widgets.Align;
using Color = Unity.UIWidgets.ui.Color;
using ListView = Unity.UIWidgets.widgets.ListView;
using TextField = Unity.UIWidgets.material.TextField;

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
        private TextEditingController controller;
        private static GlobalKey textKey = GlobalKey.key("textfield");
        public override void initState() {
            base.initState();
            controller = new TextEditingController();
        }

        public override void dispose() {
            controller.dispose();
            base.dispose();
        }

        public override Widget build(BuildContext context) {
            return new Scaffold(
                body: new ListView(
                    children: new List<Widget> {
                        new MovieClip(
                            movieClipData: new MovieClipData(
                                new List<MovieClipDataFrame>{
                                    new MovieClipDataFrame(
                                        0.5f, snapshot => {
                                            snapshot.createText("text", "Hello World",
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
                                            snapshot.rotateObjectBy("text", 10 * Mathf.PI, duration: 10);
                                            snapshot.animateTo<TextStyle>("text",
                                                style => style.copyWith(letterSpacing: 5.0f));
                                            snapshot.createBasicObject(
                                                "rect",
                                                new Container(
                                                    color: Colors.blue,
                                                    child: new SizedBox(
                                                        width: 50,
                                                        height: 50
                                                    )
                                                ),
                                                position: new Offset(100, 100),
                                                animation: AppearAnimation.overScale
                                            );
                                            snapshot.createBasicObject(
                                                "text_field",
                                                new SizedBox(
                                                    width: 100,
                                                    height: 100,
                                                    child: new TextField(
                                                        key: textKey,
                                                        controller: controller
                                                    )
                                                ),
                                                position: new Offset(200, 200),
                                                animation: AppearAnimation.fromLeft);
                                            snapshot.createBasicObject(
                                                "button",
                                                child: new RaisedButton(
                                                    color: Colors.cyan,
                                                    child: new Text("Press Me"),
                                                    onPressed: () => {}
                                                ),
                                                layer: 1,
                                                position: new Offset(300, 200),
                                                pivot: Offset.zero,
                                                animation: AppearAnimation.overScale
                                            );
                                            snapshot.createTextBox(
                                                "textbox",
                                                "Hello TextBox",
                                                maxWidth: 300,
                                                layer: -1,
                                                position: new Offset(600, 200),
                                                animation: AppearAnimation.overScale);
                                            snapshot.moveObject("button", new Offset(600, 0), delay: 0.3f, duration: 20);
                                        }
                                    ),
                                    new MovieClipDataFrame(
                                        2, snapshot => {
                                            snapshot.createBasicObject(
                                                "list",
                                                child: new TextList(
                                                    texts: new List<string> {
                                                        "(11, 2)",
                                                        "(22, 4)",
                                                        "(13, 45, 22)",
                                                        "(66, 13, 17)"
                                                    }, 
                                                    style: new TextStyle(
                                                        color: Colors.blue,
                                                        fontWeight: FontWeight.bold
                                                    ),
                                                    decoration: new BoxDecoration(
                                                        color: Colors.yellow,
                                                        border: Border.all(
                                                            color: Colors.green
                                                        )
                                                           
                                                    )
                                                ),
                                                position: new Offset(400, 200),
                                                animation: AppearAnimation.scale
                                            );
                                            snapshot.animateTo<Color>("textbox", "color", color => Colors.blue);
                                        }
                                    ),
                                    new MovieClipDataFrame(
                                        2, snapshot => {
                                            snapshot.destroyObject("text", animation: DisappearAnimation.overScale);
                                            snapshot.moveObject("rect", new Offset(200, 0));
                                        }
                                    ),
                                    new MovieClipDataFrame(
                                        20, snapshot => {}
                                    )
                                }
                            )
                        )
                    }
                )
            );
        }
    }
}
