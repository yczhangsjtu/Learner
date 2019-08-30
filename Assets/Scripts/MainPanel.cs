using System.Collections.Generic;
using System.Linq;
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

public class MainPanel : UIWidgetsPanel {

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
        public MainApp(Key key = null) : base(key) { }

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
                        new ComputeBufferMovieClip()
                    }
                )
            );
        }
    }

    class ComputeBufferMovieClip : StatelessWidget {
        readonly TextStyle descriptionStyle = new TextStyle(
            fontSize: 24,
            color: Colors.black
        );
        
        readonly TextStyle codeNodeStyle = new TextStyle(
            fontFamily: "Courier",
            fontSize: 24,
            color: Colors.black
        );
        private readonly BoxDecoration codeNodeDecoration = new BoxDecoration(
            color: Colors.lightBlue,
            border: Border.all(
                color: Colors.cyan
            )
        );
        
        readonly TextStyle codeStyle = new TextStyle(
            fontFamily: "Courier",
            fontSize: 20,
            color: Colors.white
        );

        private readonly BoxDecoration codeDecoration = new BoxDecoration(
            color: Colors.cyan,
            border: Border.all(
                color: Colors.lightBlue
            )
        );

        public override Widget build(BuildContext context) {
            return new MovieClip(
                width: 800,
                height: 600,
                startFrom: 5,
                movieClipData: new MovieClipData(
                    new List<MovieClipDataFrame> {
                        new MovieClipDataFrame(
                            2, snapshot => {
                                snapshot.createTextBox("draw_mesh",
                                    "Graphics.DrawMesh()",
                                    style: codeNodeStyle,
                                    maxWidth: 320,
                                    position: new Offset(10, 300),
                                    decoration: codeNodeDecoration,
                                    animation: AppearAnimation.scale);
                                snapshot.createTypingEffect(
                                    "draw_mesh_do",
                                    "draws a mesh directly on the screen.",
                                    style: descriptionStyle,
                                    position: new Offset(300, 300)
                                );
                                snapshot.debugObject("draw_mesh_do", "death");
                                snapshot.setDebugObjectOffset("draw_mesh_do", new Offset(0, 30));
                                snapshot.animateTyping("draw_mesh_do", duration: 1);
                            }
                        ),
                        new MovieClipDataFrame(
                            1, snapshot => {
                                snapshot.moveObjectTo("draw_mesh", new Offset(400, 50));
                                snapshot.objectPivotTo("draw_mesh", new Offset(0.5f, 0.5f));
                                snapshot.moveObject("draw_mesh_do", new Offset(0, -100));
                                snapshot.destroyObject("draw_mesh_do", animation: DisappearAnimation.fadeOut);
                            }
                        ),
                        new MovieClipDataFrame(
                            2, snapshot => {
                                snapshot.createTypingEffect("mesh_description",
                                    "A mesh specifies the shape of a 3D geometry.",
                                    position: new Offset(10, 300),
                                    pivot: new Offset(0, 0.5f));
                                snapshot.animateTyping("mesh_description");
                            }
                        ),
                        new MovieClipDataFrame(
                            2, snapshot => {
                                snapshot.destroyObject("mesh_description", animation: DisappearAnimation.scale, disappearTime: 1);
                                snapshot.createTypingEffect("mesh_contains", "A mesh contains:", style: descriptionStyle,
                                    position: new Offset(10, 100));
                                snapshot.animateTyping("mesh_contains", duration: 1);
                            }
                        ),
                        new MovieClipDataFrame(
                            2, snapshot => {
                                snapshot.createBasicObjectWithTitle("vertex_list", "Vertices", new TextList(
                                        new List<string> {"(0, 0)", "(1, 0)", "(1, 1)", "(0, 1)", "(0.5, 0.5)"},
                                        style: codeNodeStyle,
                                        decoration: new BoxDecoration(
                                            color: Colors.cyan,
                                            border: Border.all(color: Colors.lightBlue, width: 2)
                                        )
                                    ),
                                    position: new Offset(500, 400),
                                    animation: AppearAnimation.fromTop);
                            }
                        ),
                    },
                    debugEnabled: true
                )
            );
        }
    }

    class HelloMovieClip : StatelessWidget {
        public override Widget build(BuildContext context) {
            return new MovieClip(
                movieClipData: new MovieClipData(
                    new List<MovieClipDataFrame> {
                        new MovieClipDataFrame(
                            0.5f, snapshot => {
                                snapshot.createText("text", "Hello World",
                                    position: new Offset(300, 100),
                                    animation: AppearAnimation.fadeIn);
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
                                        direction: Axis.horizontal,
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
                                    "button",
                                    child: new RaisedButton(
                                        color: Colors.cyan,
                                        child: new Text("Press Me"),
                                        onPressed: () => { }
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
                                snapshot.createObject(new MovieClipTypingEffect("typing", new List<string> {
                                            "Hello World!",
                                            "\nIs this you?!",
                                            "\nOkay with that"
                                        }, style: new TextStyle(
                                            fontSize: 16,
                                            fontFamily: "Courier",
                                            color: Colors.white
                                        ), color: Colors.cyan,
                                        minHeight: 100,
                                        minWidth: 100,
                                        textAlign: TextAlign.left),
                                    position: new Offset(800, 200)
                                );
                                snapshot.animateTo<Color>("textbox", "color", color => Colors.blue);
                            }
                        ),
                        new MovieClipDataFrame(
                            2, snapshot => {
                                snapshot.destroyObject("text", animation: DisappearAnimation.overScale);
                                snapshot.moveObject("rect", new Offset(200, 0));
                                snapshot.animateTo<List<float>>("typing", "progress", list => {
                                    var ret = list.ToList();
                                    ret[0] = 1;
                                    return ret;
                                }, duration: 0.5f);
                            }
                        ),
                        new MovieClipDataFrame(
                            2, snapshot => {
                                snapshot.animateTo<List<float>>("typing", "progress", list => {
                                    var ret = list.ToList();
                                    ret[2] = 1;
                                    return ret;
                                }, duration: 0.5f);
                            }
                        ),
                        new MovieClipDataFrame(
                            20, snapshot => {
                                snapshot.animateTo<List<float>>("typing", "progress", list => {
                                    var ret = list.ToList();
                                    ret[1] = 1;
                                    return ret;
                                }, duration: 0.5f);
                            }
                        ),
                    }
                )
            );
        }
    }
}
