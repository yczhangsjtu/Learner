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
using Color = Unity.UIWidgets.ui.Color;
using Image = Unity.UIWidgets.widgets.Image;
using ListView = Unity.UIWidgets.widgets.ListView;

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
        readonly TextStyle titleStyle = new TextStyle(
            height: 1.5f,
            fontSize: 48,
            color: Colors.black,
            fontWeight: FontWeight.bold
        );
        readonly TextStyle h1Style = new TextStyle(
            height: 1.4f,
            fontSize: 28,
            color: Colors.black,
            fontWeight: FontWeight.bold
        );
        readonly TextStyle h2Style = new TextStyle(
            height: 1.3f,
            fontSize: 24,
            color: Colors.black,
            fontWeight: FontWeight.bold
        );
        readonly TextStyle h3Style = new TextStyle(
            height: 1.2f,
            fontSize: 22,
            color: Colors.black
        );
        readonly TextStyle h4Style = new TextStyle(
            height: 1.1f,
            fontSize: 20,
            color: Colors.black
        );
        readonly TextStyle bodyStyle = new TextStyle(
            fontSize: 18,
            color: Colors.black
        );
        readonly TextStyle codeStyle = new TextStyle(
            fontSize: 14,
            color: Color.fromARGB(255, 10, 10, 10),
            fontFamily: "Courier"
        );
        
        readonly Color codeBorderColor = Color.fromARGB(255, 100, 100, 100);
        readonly Color codeBackgroundColor = Color.fromARGB(255, 200, 200, 200);

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
                    padding: EdgeInsets.symmetric(5, 15),
                    children: new List<Widget> {
                        new Text("UIWidgets源码系列: Hello World", style: titleStyle, textAlign: TextAlign.center),
                        new Text("什么是UIWidgets", style: h1Style),
                        new Text(@"UIWidgets是Unity上的一个UI解决方案。UIWidgets是将目前流行的跨平台移动开发框架flutter (https://github.com/flutter/flutter) 在Unity上的移植。UIWidgets有如下优势：

1. 借助于flutter的强大UI能力，UIWidgets使得Unity游戏开发者能够在Unity游戏中嵌入复杂性和移动APP比肩的游戏UI，甚至于脱离游戏，完全由UI界面组成的应用。
2. Unity强大的3D渲染引擎可以让开发者直接用来在应用中嵌入复杂的3D场景。
3. 借助于Unity自身的跨平台特性，UIWidgets使开发者可以使用Unity直接编写出跨Windows、Linux、MacOS、WebGL和移动端应用。

UIWidgets的使用也很简单，不依赖任何第三方库，支持2018.3及以上的Unity版本。将源码clone到工程目录中的Packages文件夹下就可以开始使用了。此外UIWidgets已在Package Manager(国内镜像)和Asset Store上发布。不过推荐在github上clone源码，以保证获取到最新的代码。

本文假设您已经了解Unity的基本操作（新建工程、在场景中创建各种GameObject等）。如果没有，这些技能学习起来也非常简单。您可以访问Unity Learn (https://learn.unity.com) 查看官方的免费教程。", style: bodyStyle),
                        new Text("使用UIWidgets做一个简单的APP", style: h1Style),
                        new Text(@"1. 打开2018.3或更高版本的Unity，新建工程

2. 获取UIWidgets：
    i. 使用git（命令行或GUI版本）将 https://github.com/UnityTech/UIWidgets.git clone到工程目录的Packages文件夹下
    ii. 打开Window > Packages Manager，点击搜索框左边的""Advanced""，确保""Show preview packages""为被选中状态。随后，在列表中找到UIWidgets，然后点击右下角的""Install""
    iii. 打开Asset Store，搜索UIWidgets，下载并导入工程中。具体步骤和在Asset Store下载其他资源类似，在此不细讲。
    
3. 在工程目录中的Assets目录下新建脚本，命名为UIWidgetsExample.cs。将以下内容粘贴进去：", style: bodyStyle),
                        new Container(
                            decoration: new BoxDecoration(
                                border: Border.all(color: codeBorderColor),
                                color: codeBackgroundColor
                            ),
                            padding: EdgeInsets.all(10),
                            margin: EdgeInsets.symmetric(10, 5),
                            child: new Text(@"using System.Collections.Generic;
using Unity.UIWidgets.animation
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;

namespace UIWidgetsSample {
    public class UIWidgetsExample : UIWidgetsPanel {
        protected override Widget createWidget() {
            return new ExampleApp();
        }

        class ExampleApp : StatefulWidget {
            public ExampleApp(Key key = null) : base(key) {
            }

            public override State createState() {
                return new ExampleState();
            }
        }

        class ExampleState : State<ExampleApp> {
            int counter = 0;

            public override Widget build(BuildContext context) {
                return new Column(
                    children: new List<Widget> {
                        new Text(""Counter: "" + this.counter),
                        new GestureDetector(
                            onTap: () => {
                                this.setState(() => {
                                this.counter++;
                            });
                        },
                        child: new Container(
                                padding: EdgeInsets.all(20),
                                color: Colors.blue,
                                child: new Text(""Click Me"")
                            )
                        )
                    }
                );
            }
        }
    }
}", style: codeStyle)
                        ),
                        new Text(@"4. 在场景中新建一个Canvas：在Hierachy窗口中右键 -> UI -> Canvas。点击Scene窗口左上角的2D按钮打开2D视角。

5. 将UIWidgetsExample脚本添加到Canvas上：从Project中将脚本拖到场景中的Canvas上，或点击Canvas，在Inspector窗口中点击“Add Component”，在搜索框中输入“UI Widgets Example”，点击搜索到的选项。

这个简单的UIWidgets应用效果如下。", style: bodyStyle),
                        Image.asset("Images/UIWidgetsHelloWorld",
                            fit: BoxFit.contain,
                            height: 300),
                        new Text(@"点击“Click Me”按钮，可以看到Counter后面的数字增加。

如果你觉得上述例子过于简单，可以打开场景“Samples/UIWidgetSample/UIWidgetsGallery/gallery.scene”看一下目前UIWidgets能够做出的界面。这个demo应用展示了UIWidgets中的常用组件。
", style: bodyStyle),
                        
                        new Text(@"关于目前已有的成熟的UIWidgets做的产品，可以参考https://github.com/liangxiegame/awesome-uiwidgets提供的列表。", style: bodyStyle),
                        new ComputeBufferMovieClip(),
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
