using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Media;

/// <summary>
///Assembly info 
/// </summary>
[assembly: AssemblyProduct("La Linea")]
[assembly: AssemblyCompany("Alan Marko")]
[assembly: AssemblyVersion("2.0.0.0")]


//Main namespace
namespace LaLinea
{
    namespace Core
    {
        //Game´s Core
        public class Core
        {

            //Constructor
            public Core(Window _area)
            {

                //Setup of variables

                //Drawing area
                area = _area;

                //Timer
                timer = _area.t;

                //Fps timer
                this.fps = new Timer();
                fps.Interval = 500;
                this.fps.Tick += this.FPS;
                fps.Enabled = true;

                //Load sounds
                //YouWin
                soundplayer1.SoundLocation = @"LaLinea-Sounds\LaLinea-YouWin.wav";
                soundplayer1.LoadAsync();
                //KillBlock
                soundplayer2.SoundLocation = @"LaLinea-Sounds\LaLinea-DestroyedBlock.wav";
                soundplayer2.LoadAsync();

                //Mouse handlers
                this.area.MouseDown += Down;
                this.area.MouseMove += Move;
                this.area.MouseUp += Up;
                this.area.MouseClick += OnClicking;

                //Graphics
                graphics = area.g;
            }

            //Variables

            //Level

            public Level Level
            {
                get { return level; }
                set
                {
                    level = value;

                    //Setup of Variables
                    drawing = level.Drawing;
                    blocks = level.Blocks;

                    //Number of Orange Blocks
                    NoOB = 0;
                    foreach (Blocks.Block b in blocks)
                    {
                        if (b.IOB) { NoOB++; }
                    }

                    //oldlevel is used by reset 
                    oldlevel = new Level(new Drawing.Drawing(level.Drawing.p1, level.Drawing.p2), new List<Blocks.Block>(level.Blocks.ToArray()));
                }
            }

            Level level;

            Level oldlevel;

            //Collection of barrier blocks
            List<Blocks.Block> blocks;


            //Drawing
            Drawing.Drawing drawing;


            //Drawing area
            Window area;

            //Number of Orange Blocks
            int NoOB = 0;




            //Lines
            //Collection of lines
            List<Line> lines = new List<Line>();

            //Actually drawed line 
            Line drawedline;

            //Timer

            //Timer
            Timer timer;

            //fps
            Timer fps;

            //fps
            int linespeed = 0;
            int _fps = 1;

            //Is not pause?
            public bool Activity = false;

            //speedcounter
            int speedcounter = 0;


            //Sounds

            //SoundPlayer
            private SoundPlayer soundplayer1 = new SoundPlayer();
            private SoundPlayer soundplayer2 = new SoundPlayer();


            //Graphics

            //Buffer
            Graphics graphics;

            //Background
            Graphic.Background background = new Graphic.Background();


            //Area Variables
            //Size
            //Width
            public int Width
            {
                get { return area.Size.Width; }
                set { area.Size = new Size(value, Height); }
            }

            //Height
            public int Height
            {
                get { return area.Size.Height; }
                set { area.Size = new Size(Width, value); }
            }

            //Position of game in window
            public Point Position
            {
                get
                {
                    return new Point(0, 0);
                }
            }

            //Size
            public Size Size
            {
                get
                {
                    return new Size(Width, Height);
                }
            }

            //Timer

            //Is pause?
            public bool pause = false;

            //Is level won?
            public bool win = false;

            //Is timer on?
            public bool WaitState
            {
                get { return !timer.Enabled; }
                set { timer.Enabled = !value; }
            }

            //Input
            List<MouseEventArgs> up = new List<MouseEventArgs>();
            List<MouseEventArgs> down = new List<MouseEventArgs>();
            List<MouseEventArgs> move = new List<MouseEventArgs>();

            //Methods

            //Main method-Loop
            public void Ticking(object s, EventArgs e)
            {
                speedcounter++;

                if (!pause && !win)
                {
                    //Input
                    Input();
                }

                if (!pause && !win)
                {
                    //Logic
                    Logic();
                }

                speedcounter--;

                //Drawing
                if (speedcounter == 0)
                {
                    Draw();
                }
            }

            //Fps
            void FPS(object s, EventArgs e)
            {
                linespeed = 100 / _fps;
                _fps = 1;
            }

            //Input processing

            //Main
            void Input()
            {
                //Down
                foreach (MouseEventArgs e in down)
                {
                    ODown(e);
                }
                down.Clear();

                //Move
                foreach (MouseEventArgs e in move)
                {
                    OMove(e);
                }
                move.Clear();

                //Up
                foreach (MouseEventArgs e in up)
                {
                    OUp(e);
                }
                up.Clear();

            }

            //Auxiliary methods for Input()
            //Pressed the left mouse button
            void ODown(MouseEventArgs e)
            {
                //Get axis of point where the left mouse button was pressed 
                Point p = new Point(e.X, e.Y);
                //new Point((int)((double)(e.X - Position.X) * (600d / (double)Size.Width)), (int)((e.Y - Position.Y) * (600d / (double)Size.Height)));

                //Creating new line
                if (e.Button == MouseButtons.Left && drawing.HitTest(p) && drawedline == null)
                {
                    drawedline = new Line(p);
                    this.lines.Add(drawedline);
                }
            }
            //Motion mouse
            void OMove(MouseEventArgs e)
            {
                //Get axis of point where the left mouse button was pressed 
                Point p = new Point(e.X, e.Y);
                //new Point((int)((double)(e.X - Position.X) * (600d / (double)Size.Width)), (int)((e.Y - Position.Y) * (600d / (double)Size.Height)));

                //Add next point to line
                if ((drawedline != null) && (e.Button == MouseButtons.Left) && (drawing.HitTest(p)))
                {
                    drawedline.Add(p);
                }
            }
            //Drop the left mouse button
            void OUp(MouseEventArgs e)
            {
                //If line has few points then don't draw it
                if (e.Button == MouseButtons.Left && drawedline != null && drawedline.Count < 4)
                {
                    lines.Remove(drawedline);
                }

                //Else line can move
                if (e.Button == MouseButtons.Left && drawedline != null)
                {
                    drawedline.PrepairForMove();
                }

                if (e.Button == MouseButtons.Left)
                { drawedline = null; }
            }

            //Handlers
            //Pressed the left mouse button
            void Down(object s, MouseEventArgs e)
            {
                if (Activity && !pause) { down.Add(e); }
            }

            //Motion mouse
            void Move(object s, MouseEventArgs e)
            {
                if (Activity && !pause) { move.Add(e); }
            }

            //Drop the left mouse button
            void Up(object s, MouseEventArgs e)
            {
                if (Activity && !pause) { up.Add(e); }
            }


            //On click
            void OnClicking(object s, MouseEventArgs e)
            {
                if (!pause && !win)
                {
                    //Pause button
                    if (e.X < 65 && e.X > 15 && e.Y < 61 && e.Y > 7)
                    {
                        Pause();
                    }
                    //Replay button
                    if (e.X < 122 && e.X > 74 && e.Y < 61 && e.Y > 7)
                    {
                        RePlay();
                    }
                }
                else
                {
                    //Play
                    if (e.X > 200 && !win)
                    {
                        Play();
                    }
                }
            }

            //Logic of a game
            void Logic()
            {

                //Logic of a game
                //Lines
                for (int i = 0; i < linespeed; i++)
                {
                    //Lines which must be deleted
                    List<Line> _deletedlines = new List<Line>();

                    //Orange Blocks which must be deleted
                    List<Blocks.Block> _deletedblocks = new List<Blocks.Block>();

                    //Move line
                    foreach (Line c in lines)
                    {
                        //Move line
                        c.NextPoint();
                        if (c.deleted) { _deletedlines.Add(c); }

                        //If must be controlled
                        if (c.controll)
                        {
                            //Detectect if the line touch the block
                            foreach (Blocks.Block b in this.blocks)
                            {
                                //Colision
                                if (b.Collision(c))
                                {
                                    soundplayer2.Play();
                                    NoOB--;
                                    _deletedblocks.Add(b);
                                }
                            }
                        }
                        //Controll
                        c.controll = true;
                    }

                    //Move blocks
                    foreach (Blocks.Block b in this.blocks)
                    {
                        b.Go();
                    }

                    //Delete lines
                    foreach (Line c in _deletedlines)
                    {
                        lines.Remove(c);
                    }

                    //Delete Orange Blocks
                    foreach (Blocks.Block b in _deletedblocks)
                    {
                        blocks.Remove(b);
                    }
                }

                //Check if the level is won?
                if (NoOB == 0)
                {
                    soundplayer1.Play();
                    Win();
                }

            }

            //Methods for drawing
            //Main
            public void Draw()
            {
                try
                {
                    //Draw background
                    DrawBackground();

                    //Draw drawing
                    DrawDrawing();

                    //Draw Lines
                    DrawLines();

                    //Draw Blocks
                    DrawBlocks();
                }
                catch
                { }

                //Fps
                _fps++;
            }

            //Background
            void DrawBackground()
            {
                background.Draw2(graphics);
            }

            //Blocks
            void DrawBlocks()
            {
                foreach (Blocks.Block b in this.blocks)
                {
                    b.Draw(graphics);
                }
            }

            //Drawing
            void DrawDrawing()
            {
                drawing.Draw(graphics);
            }

            //Lines
            void DrawLines()
            {
                foreach (Line c in lines)
                {
                    c.Draw(graphics);
                }
            }

            //Methods for controlling a game

            //Pause
            public void Pause()
            {
                //Stop game 
                this.pause = !false;

                //Show pause window
                area.t.Tick += area.pau.Ticking;
                area.pau.Activity = true;
            }

            //Play
            public void Play()
            {
                //Play
                this.pause = !true;

                //Remove pause window
                area.t.Tick -= area.pau.Ticking;
                area.pau.Activity = false;

                //Remove win window
                area.t.Tick -= area.w.Ticking;
                area.w.Activity = false;
            }

            //Replay
            public void RePlay()
            {
                //Set level
                Level = oldlevel;

                //Remove lines
                lines = new List<Line>();

                //No actuall drawed line
                drawedline = null;

                //Input
                up = new List<MouseEventArgs>();
                down = new List<MouseEventArgs>();
                move = new List<MouseEventArgs>();

                //Win
                win = false;

                //Play
                Play();
            }

            //You Win
            void Win()
            {
                //Win
                win = true;

                //Show Window
                area.t.Tick += area.w.Ticking;
                area.w.Activity = true;
            }
        }
    }

    namespace Drawing
    {
        //Drawing is area where the player must draw a line
        public class Drawing
        {
            //Constructor
            public Drawing(Point _p1, Point _p2)
            {
                //Points-corners of drawing
                p1 = _p1;
                p2 = _p2;

                //Size
                s = new Size(p2.X - p1.X, p2.Y - p1.Y);

            }

            //Points
            public Point p1, p2;

            //Size
            Size s;

            //Hit test
            public bool HitTest(Point p)
            {
                //Hittest
                if (p.X < p2.X && p.X > p1.X && p.Y < p2.Y && p.Y > p1.Y)
                { return true; }
                return false;
            }

            //Check is drawn drawing opposite(in level editor).
            public void Test()
            {
                //X-axis
                if (p1.X > p2.X)
                {
                    int aux = p1.X;
                    p1.X = p2.X;
                    p2.X = aux;
                }
                //Y-axis
                if (p1.Y > p2.Y)
                {
                    int aux = p1.Y;
                    p1.Y = p2.Y;
                    p2.Y = aux;
                }

                //Size
                s = new Size(p2.X - p1.X, p2.Y - p1.Y);
            }

            //Textures
            TextureBrush background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-DrawingArea.gif"));
            TextureBrush frame = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-DrawingAreaFrame.gif"));

            //Draw
            public void Draw(Graphics g)
            {
                g.FillRectangle(background, new Rectangle(p1, s));
                g.DrawRectangle(new Pen(frame, 4), new Rectangle(p1, s));
            }
        }
    }


    //Blocks
    namespace Blocks
    {
        //Base block class
        public abstract class Block
        {
            //Constructor
            public Block()
            {
            }

            //Variables

            //Type of block(orange,green,red,gyrotary,motile...)
            public int Type;

            //Points of block-coordinates of the points block
            protected List<Point> points = new List<Point>();


            public virtual List<Point> Points
            {
                get
                {
                    return points;
                }
                set
                {
                    points = value;
                    Initialize(points);
                    Test();
                }
            }

            //Number of points
            public int NoP;

            //Texture
            protected Bitmap bmpBackground;
            protected TextureBrush background;

            //Check is drawn block opposite(in level editor).
            protected virtual void Test() { }

            //Create block from given points
            protected virtual void Initialize(List<Point> _body) { }

            //Is this block orange block?
            internal bool IOB = false;

            //Draw
            public abstract void Draw(Graphics g);

            //Collision with the line-returns whether the line must be removed
            internal abstract bool Collision(Line c);

            //If block is gyrotary or motile then move with it
            public virtual void Go() { }

            //Name of block
            public string Name;

            //To string
            public override string ToString()
            {
                return Name;
            }
        }

        //Rectangle block
        public class Rectangle : Block
        {
            //Constructors
            public Rectangle(Point _p1, Point _p2)
                : this()
            {
                //Create block
                Initialize(_p1, _p2);

                //body
                Points.Add(_p1);
                Points.Add(_p2);
            }


            public Rectangle(List<Point> _points) : this(_points[0], _points[1]) { }


            public Rectangle()
                : base()
            {
                //Type of block
                Type = 1;

                //Number of points
                NoP = 2;

                //Name
                Name = "Rectangle";

                //Texture
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground1.gif"));
            }

            //Points
            public Point p1, p2;

            //Size
            public Size s;

            //Create block from given points
            protected override void Initialize(List<Point> _body)
            {
                Initialize(_body[0], _body[1]);
            }

            protected void Initialize(Point _p1, Point _p2)
            {
                //Points
                p1 = _p1;
                p2 = _p2;

                //Size
                s = new Size(p2.X - p1.X, p2.Y - p1.Y);
            }

            //Check is drawn block opposite(in level editor).
            protected override void Test()
            {
                //X
                if (p1.X > p2.X)
                {
                    int aux = p1.X;
                    p1.X = p2.X;
                    p2.X = aux;
                }
                //Y
                if (p1.Y > p2.Y)
                {
                    int aux = p1.Y;
                    p1.Y = p2.Y;
                    p2.Y = aux;
                }

                //Reverse block
                Points[0] = p1;
                Points[1] = p2;
                Initialize(p1, p2);
            }

            //Collision with line
            internal override bool Collision(Line c)
            {
                //Hit test
                if ((HitTest(c.Start)) && (c.AddPointsToStart))
                {
                    //Absorb the line
                    c.AddPointsToStart = false;
                }
                return false;
            }

            //Hit test
            protected virtual bool HitTest(Point p)
            {
                //Hittest
                if ((p.X < p2.X) && (p.X > p1.X) && (p.Y < p2.Y) && (p.Y > p1.Y))
                { return true; }
                return false;
            }

            //Draw
            public override void Draw(Graphics g)
            {
                g.FillRectangle(background, new System.Drawing.Rectangle(p1, s));
                Pen blackpen = new Pen(Color.Black, 3);
                g.DrawRectangle(blackpen, new System.Drawing.Rectangle(p1, s));
            }
        }

        //Orange block
        public class OrangeBlock : Circle
        {
            //Constructor
            public OrangeBlock(Point _p1, Point _p2)
                : base(_p1, _p2)
            {
                //This is orange block
                IOB = true;

                //Type
                Type = 3;

                //Name
                Name = "OrangeBlock";

                //Texture
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground2.gif"));

            }


            public OrangeBlock(List<Point> _points) : this(_points[0], _points[1]) { }

            public OrangeBlock()
                : base()
            {
                //Type
                Type = 3;

                //This is orange block
                IOB = true;

                //Name
                Name = "OrangeBlock";

                //texture
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground2.gif"));
            }

            //Collision
            internal override bool Collision(Line c)
            {
                if ((HitTest(c.Start)))
                {
                    return true;
                }

                return false;
            }


            //Draw
            public override void Draw(Graphics g)
            {
                g.FillEllipse(background, new System.Drawing.Rectangle(p1, s));
                Pen mypen = new Pen(Color.Black, 3);
                g.DrawEllipse(mypen, new System.Drawing.Rectangle(p1, s));
            }

        }

        //OrangeBlock-Gyrotary
        public class OrangeBlockGyratory : OrangeBlock
        {
            //Constructor
            public OrangeBlockGyratory(Point _p1, Point _p2, Point _p3)
                : base(_p1, _p2)
            {

                //Axis
                axis = _p3;


                NoP = 3;

                //3
                Points.Add(_p3);

                //Type
                Type = 11;

                //Name
                Name = "OrangeBlock-Gyratory";

            }


            public OrangeBlockGyratory(List<Point> _points) : this(_points[0], _points[1], _points[2]) { }

            //init
            protected override void Initialize(List<Point> _points)
            {
                Initialize(_points[0], _points[1], _points[2]);
            }

            private void Initialize(Point _p1, Point _p2, Point _p3)
            {
                base.Initialize(_p1, _p2);

                axis = _p3;
            }

            //Radius
            int Radius;

            //Axis of ratation
            public Point axis;


            public OrangeBlockGyratory()
                : base()
            {

                NoP = 3;

                //Type
                Type = 11;

                //Name
                Name = "OrangeBlockGyratory";
            }

            //Angle
            double Angle = 0;

            //Go-move block
            public override void Go()
            {
                //First time
                if (Angle == 0)
                {
                    //Radius
                    Radius = (int)Math.Sqrt((Math.Pow(Math.Abs(centre.X - axis.X), 2) + Math.Pow(Math.Abs(centre.Y - axis.Y), 2)));
                }

                //Increase the angle
                Angle = (Angle + 0.01) % 6.283184;
                centre = new Point(
                                (int)(Math.Sin(Angle) * Radius + axis.X),
                                (int)(Math.Cos(Angle) * Radius + axis.Y)
                                );
                Points[0] = new Point(centre.X - radiusofcircle, centre.Y - radiusofcircle);
                Points[1] = new Point(centre.X + radiusofcircle, centre.Y + radiusofcircle);
                Initialize(Points[0], Points[1], Points[2]);
            }

        }

        //OrangeBlock-Motile
        public class OrangeBlockMotile : OrangeBlock
        {
            //Constructor
            public OrangeBlockMotile(Point _p1, Point _p2, Point _p3)
                : base(_p1, _p2)
            {

                //Direction
                destination = _p3;


                NoP = 3;

                //3
                Points.Add(_p3);

                //Type
                Type = 12;

                //Name
                Name = "OrangeBlock-Motile";

                //Calculate the size of movement
                int c = (int)Math.Sqrt((Math.Pow(Math.Abs(centre.X - destination.X), 2) + Math.Pow(Math.Abs(centre.Y - destination.Y), 2)));
                NumberOfMovements = (c / 2);
                MovementX = ((destination.X - centre.X) / NumberOfMovements);
                MovementY = ((destination.Y - centre.Y) / NumberOfMovements);
                PositionX = Points[0].X;
                PositionY = Points[0].Y;
            }


            public OrangeBlockMotile(List<Point> _points) : this(_points[0], _points[1], _points[2]) { }

            //Initialize
            protected override void Initialize(List<Point> _points)
            {
                Initialize(_points[0], _points[1], _points[2]);
            }

            private void Initialize(Point _p1, Point _p2, Point _p3)
            {
                base.Initialize(_p1, _p2);

                destination = _p3;
            }

            //Direction-point where block must come
            public Point destination;


            public OrangeBlockMotile()
                : base()
            {

                NoP = 3;

                //Type
                Type = 12;

                //Name
                Name = "OrangeBlockMotile";
            }

            //Go


            //Direction
            int direction = 1;

            //Number
            double NumberOfMovements;
            int Number = 0;

            //Movement
            double MovementX;
            double MovementY;

            //Position
            double PositionX = 0;
            double PositionY = 0;


            //Go
            public override void Go()
            {
                //Calculate new position
                Points[0] = new Point((int)((PositionX += direction * MovementX)), (int)((PositionY += direction * MovementY)));
                Points[1] = new Point((int)((PositionX + 2 * radiusofcircle)), (int)((PositionY + 2 * radiusofcircle)));
                Number++;

                //Change direction
                if (Number > NumberOfMovements) { Number = 0; direction *= -1; }

                //Move
                Initialize(Points[0], Points[1], Points[2]);
            }

        }

        //Circle block
        public class Circle : Block
        {
            //Constructor
            public Circle(Point _p1, Point _p2)
                : this()
            {
                //Initialize
                Initialize(_p1, _p2);

                //Points
                Points.Add(_p1);
                Points.Add(_p2);
            }


            public Circle(List<Point> _points) : this(_points[0], _points[1]) { }


            public Circle()
                : base()
            {
                //Type
                Type = 2;

                //
                NoP = 2;

                //Name
                Name = "Circle";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground1.gif"));
            }


            //Initialize
            protected override void Initialize(List<Point> _points)
            {
                Initialize(_points[0], _points[1]);
            }

            //Initialize
            protected virtual void Initialize(Point _p1, Point _p2)
            {
                //Points
                p1 = _p1;
                p2 = _p2;

                //Size
                s = new Size(p2.X - p1.X, p2.Y - p1.Y);

                //Centre
                centre = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);

                //Radius
                radiusofcircle = (p2.X - p1.X) / 2;
            }

            //Check is drawn block opposite(in level editor).
            protected override void Test()
            {
                //X
                if (p1.X > p2.X)
                {
                    int pom = p1.X;
                    p1.X = p2.X;
                    p2.X = pom;
                }

                //Y
                if (p1.Y > p2.Y)
                {
                    int pom = p1.Y;
                    p1.Y = p2.Y;
                    p2.Y = pom;
                }

                //No oval
                p2.Y = p1.Y + (p2.X - p1.X);

                //Reverse block
                Points[0] = p1;
                Points[1] = p2;
                Initialize(p1, p2);
            }

            //Centre
            public Point centre;

            //Radius of circle
            public int radiusofcircle;

            //Points
            public Point p1, p2;

            //Size
            public Size s;

            //Collision
            internal override bool Collision(Line c)
            {
                if ((HitTest(c.Start)) && (c.AddPointsToStart))
                {
                    c.AddPointsToStart = false;
                }

                return false;
            }

            //Hit test
            protected virtual bool HitTest(Point p)
            {
                //Calculate distance from centre of circle to p
                int x = Math.Abs(p.X - centre.X);


                int y = Math.Abs(p.Y - centre.Y);

                //Hittest
                if ((Math.Sqrt(x * x + y * y)) < radiusofcircle)
                {
                    return true;
                }
                return false;
            }

            //Draw
            public override void Draw(Graphics g)
            {
                g.FillEllipse(background, new System.Drawing.Rectangle(p1, s));
                Pen mypen = new Pen(Color.Black, 3);
                g.DrawEllipse(mypen, new System.Drawing.Rectangle(p1, s));
            }
        }


        //Red circle
        public class RedCircle : Circle
        {
            //Constructor
            public RedCircle(Point _p1, Point _p2)
                : base(_p1, _p2)
            {
                Type = 4;

                //Name
                Name = "RedCircle";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground3.gif"));
            }


            public RedCircle(List<Point> _points) : this(_points[0], _points[1]) { }


            public RedCircle()
                : base()
            {
                //Type
                Type = 4;

                //Name
                Name = "RedCircle";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground3.gif"));
            }

            //Collision
            internal override bool Collision(Line c)
            {
                if ((HitTest(c.Start)))
                {
                    //Reflect
                    c.reflection = new LaLinea.AuxiliaryClasses.Reflection(centre, c.Start);
                }

                return false;
            }
        }

        public class RedRectangle : RedPolygon
        {
            public RedRectangle(Point[] _p)
                : base(_p)
            {
                //Type
                Type = 8;

                //
                NoP = 4;

                //Name
                Name = "RedRectangle";
            }


            public RedRectangle(List<Point> _points) : this(_points.ToArray()) { }


            public RedRectangle()
                : base()
            {
                //Type
                Type = 8;

                //
                NoP = 4;

                //Name
                Name = "RedRectangle";
            }

            //Points -different processing
            public override List<Point> Points
            {
                get
                {
                    return points;
                }
                set
                {
                    points = Create(value[0], value[2]);
                    Initialize(points);
                }
            }

            //Create 4 points block from 2 points
            protected static List<Point> Create(Point p1, Point p2)
            {
                Point pp1 = new Point(p1.X < p2.X ? p1.X : p2.X, p1.Y < p2.Y ? p1.Y : p2.Y);
                Point pp2 = new Point(p1.X > p2.X ? p1.X : p2.X, p1.Y < p2.Y ? p1.Y : p2.Y);
                Point pp3 = new Point(p1.X > p2.X ? p1.X : p2.X, p1.Y > p2.Y ? p1.Y : p2.Y);
                Point pp4 = new Point(p1.X < p2.X ? p1.X : p2.X, p1.Y > p2.Y ? p1.Y : p2.Y);

                return new List<Point>() { pp1, pp2, pp3, pp4 };
            }
        }

        public class RedSquare : RedRectangle
        {
            public RedSquare(Point[] _p)
                : base(_p)
            {
                //Type
                Type = 9;

                //
                NoP = 4;

                //Name
                Name = "RedSquare";
            }


            public RedSquare(List<Point> _points) : this(_points.ToArray()) { }


            public RedSquare()
                : base()
            {
                //Type
                Type = 9;

                //
                NoP = 4;

                //Name
                Name = "RedSquare";
            }

            //Points
            public override List<Point> Points
            {
                get
                {
                    return points;
                }
                set
                {
                    points = Create(value[0], value[2]);
                    Initialize(points);
                }
            }

            //Create 4 points block from 2 points
            protected static List<System.Drawing.Point> Create(System.Drawing.Point p1, System.Drawing.Point p2)
            {
                List<Point> l = RedRectangle.Create(p1, p2);
                l[2] = new Point(l[2].X, l[0].Y + Math.Abs(l[0].X - l[1].X));
                l[3] = new Point(l[3].X, l[0].Y + Math.Abs(l[0].X - l[1].X));
                return l;
            }
        }

        public class Square : Rectangle
        {
            //Constructor
            public Square(Point _p1, Point _p2)
                : base(_p1, _p2)
            {
                Type = 10;

                //Name
                Name = "Square";
            }


            public Square(List<Point> _points) : this(_points[0], _points[1]) { }


            public Square()
                : base()
            {
                //Type
                Type = 10;

                //
                NoP = 2;

                //Name
                Name = "Square";
            }

            //Create block
            protected override void Initialize(List<Point> _points)
            {
                Initialize(_points[0], _points[1]);

            }

            //Points
            public override List<Point> Points
            {
                get
                {
                    return points;
                }
                set
                {
                    points = value;
                    points[1] = new Point(points[1].X, points[0].Y + Math.Abs(points[0].X - points[1].X));
                    Initialize(points);
                    Test();

                }
            }
        }

        //N-point block
        public class Polygon : Block
        {
            //Constructor
            public Polygon(Point[] _p)
                : this()
            {
                //Points - Array
                ArrayOfPoints = _p;

                //Points-List
                Points = new List<Point>(_p);
            }

            public Polygon(List<Point> _points) : this(_points.ToArray()) { }

            //bezparam konst
            public Polygon()
                : base()
            {
                //Type
                Type = 5;

                //N-points                
                NoP = -1;

                //Name
                Name = "Polygon";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground1.gif"));
            }

            //Create block
            protected override void Initialize(List<Point> _points)
            {
                Initialize(_points.ToArray());
            }

            private void Initialize(Point[] _p)
            {
                //Points-Array
                ArrayOfPoints = _p;
            }

            //Points-Array for easier processing
            public Point[] ArrayOfPoints;

            //Collision
            internal override bool Collision(Line c)
            {
                if ((HitTest(c)) && (c.AddPointsToStart))
                {
                    //Absorb the line
                    c.AddPointsToStart = false;
                }

                return false;
            }

            //Hit test
            bool HitTest(Line c)
            {
                //Go through all pairs of points in the polygon and check if they intersect the line.
                for (int i = 0; i < ArrayOfPoints.Length - 1; i++)
                {
                    if (Intersect(ArrayOfPoints[i], ArrayOfPoints[i + 1], c.fSecondPoint, c.fStart))
                    {
                        return true;
                    }
                }
                if (Intersect(ArrayOfPoints[ArrayOfPoints.Length - 1], ArrayOfPoints[0], c.fSecondPoint, c.fStart)) { return true; }

                return false;
            }

            protected bool Intersect(PointF p1, PointF p2, PointF p3, PointF p4)
            {
                //Intersect line
                bool inters = AuxiliaryClasses.Semicircle.Right(p2, p1, p3);

                bool inters2 = AuxiliaryClasses.Semicircle.Left(p2, p1, p4);

                bool intersect = (inters && inters2);

                //Intersect polygon
                inters = AuxiliaryClasses.Semicircle.Right(p3, p4, p1);

                inters2 = AuxiliaryClasses.Semicircle.Left(p3, p4, p2);

                bool intersect2 = (inters && inters2);

                //vysledok
                return (intersect && intersect2);
            }

            //Draw
            public override void Draw(Graphics g)
            {
                g.FillPolygon(this.background, ArrayOfPoints);
                g.DrawPolygon(new Pen(Color.Black, 3), ArrayOfPoints);
            }
        }

        public class RedPolygon : Polygon
        {
            public RedPolygon(Point[] _p)
                : base(_p)
            {
                //Type
                Type = 6;

                //Name
                Name = "RedPolygon";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground3.gif"));
            }


            public RedPolygon(List<Point> _points) : this(_points.ToArray()) { }


            public RedPolygon()
                : base()
            {
                //Type
                Type = 6;

                //Name
                Name = "RedPolygon";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground3.gif"));
            }

            //Reflection
            AuxiliaryClasses.Reflection reflection;

            //Collision
            internal override bool Collision(Line c)
            {
                if ((HitTest(c)))
                {
                    //Reflect
                    c.reflection = reflection;
                }

                return false;
            }

            //Hit test
            bool HitTest(Line c)
            {
                //Check whether the line intersect the block
                for (int i = 0; i < ArrayOfPoints.Length - 1; i++)
                {
                    if (Intersect(ArrayOfPoints[i], ArrayOfPoints[i + 1], c.fSecondPoint, c.fStart))
                    {
                        //Calculate reflection
                        double o = Math.Atan2(ArrayOfPoints[i + 1].Y - ArrayOfPoints[i].Y, ArrayOfPoints[i + 1].X - ArrayOfPoints[i].X);
                        if (o < 0) { o = 6.283184 - Math.Abs(o); }
                        reflection = new AuxiliaryClasses.Reflection(o);

                        //This is for too sharp polygons(line could go into block)
                        while (true)
                        {
                            if (c.fStart.X == c.fSecondPoint.X && c.fStart.Y == c.fSecondPoint.Y)
                            {
                                break;
                            }
                            else
                            {
                                PointF v = new PointF(0, 0);
                                v.X = (c.Points[0].X - (c.Points[c.Count - 2].X - c.fStart.X));
                                v.Y = (c.Points[0].Y - (c.Points[c.Count - 2].Y - c.fStart.Y));
                                c.Points.Insert(0, v);
                                c.Points.RemoveAt(c.Count - 1);
                            }

                        }
                        return true;
                    }

                }
                if (Intersect(ArrayOfPoints[ArrayOfPoints.Length - 1], ArrayOfPoints[0], c.fSecondPoint, c.fStart))
                {
                    //Calculate reflection
                    double o = Math.Atan2(ArrayOfPoints[0].Y - ArrayOfPoints[ArrayOfPoints.Length - 1].Y, ArrayOfPoints[0].X - ArrayOfPoints[ArrayOfPoints.Length - 1].X);
                    if (o < 0) { o = 6.283184 - Math.Abs(o); }
                    reflection = new AuxiliaryClasses.Reflection(o);

                    //This is for too sharp polygons(line could go into block)
                    while (true)
                    {
                        if (c.fStart.X == c.fSecondPoint.X && c.fStart.Y == c.fSecondPoint.Y)
                        {
                            break;
                        }
                        else
                        {
                            PointF v = new PointF(0, 0);
                            v.X = (c.Points[0].X - (c.Points[c.Count - 2].X - c.fStart.X));
                            v.Y = (c.Points[0].Y - (c.Points[c.Count - 2].Y - c.fStart.Y));
                            c.Points.Insert(0, v);
                            c.Points.RemoveAt(c.Count - 1);
                        }

                    }
                    return true;
                }

                return false;
            }
        }

        //Teleport
        public class Teleport : Circle
        {
            //Constructor
            public Teleport(Point _p1, Point _p2, Point _p3)
                : base(_p1, _p2)
            {
                telepoint = _p3;

                //Type
                Type = 7;


                NoP = 3;

                //Telepoint
                Points.Add(_p3);

                //Name
                Name = "Teleport";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground4.gif"));
                telepointbrush = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground5.gif"));
            }

            //Telepoint brush
            TextureBrush telepointbrush;


            public Teleport()
                : base()
            {
                //Type
                Type = 7;


                NoP = 3;

                //Name
                Name = "Teleport";

                //
                background = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground4.gif"));
                telepointbrush = new TextureBrush(Bitmap.FromFile(@"LaLinea-Images\LaLinea-BlockBackground5.gif"));
            }

            //Create block
            protected override void Initialize(List<Point> _points)
            {
                Initialize(_points[0], _points[1], _points[2]);
            }

            private void Initialize(Point _p1, Point _p2, Point _p3)
            {
                base.Initialize(_p1, _p2);

                telepoint = _p3;
            }

            public Teleport(List<Point> _points) : this(_points[0], _points[1], _points[2]) { }

            //Point where line will appear
            public Point telepoint;

            internal override bool Collision(Line c)
            {
                if ((HitTest(c.Start)))
                {
                    //Teleport the line
                    c.AddTelepoint(telepoint);
                }
                return false;
            }

            //Draw
            public override void Draw(Graphics g)
            {
                base.Draw(g);
                g.FillEllipse(telepointbrush, new System.Drawing.Rectangle(new Point(telepoint.X - 40, telepoint.Y - 40), new Size(80, 80)));
                Pen mypen = new Pen(Color.Black, 3);
                g.DrawEllipse(mypen, new System.Drawing.Rectangle(new Point(telepoint.X - 40, telepoint.Y - 40), new Size(80, 80)));
            }
        }
    }

    namespace Graphic
    {
        //Background
        class Background
        {
            //Constructor
            public Background()
            {
                //Textures
                background = Bitmap.FromFile(@"LaLinea-Images\LaLinea-Background.gif");
                background2 = Bitmap.FromFile(@"LaLinea-Images\LaLinea-Background-Game.gif");

            }

            //Textures
            Image background, background2;

            //Draw 1
            public void Draw(Graphics grfx)
            {
                //Draw
                grfx.DrawImageUnscaled(background, 0, 0, 600, 600);
            }

            //Draw 2
            public void Draw2(Graphics grfx)
            {
                //Draw
                grfx.DrawImageUnscaled(background2, 0, 0, 600, 600);
            }
        }
    }

    //Line
    internal class Line
    {
        //Constructor
        internal Line(PointF p)
        {

            //Initialization(List of Points)
            Points = new List<PointF>();

            //First point
            Add(p);

        }

        //Variables

        //Points

        //Points
        public List<PointF> Points;

        //Number of points
        public int Count
        {
            get { return Points.Count; }
        }

        //First point(It is reversed)-float
        public PointF fStart
        {
            get { return Points[Count - 1]; }
        }

        //First point-int
        public Point Start
        {
            get { return new Point((int)Points[Count - 1].X, (int)Points[Count - 1].Y); }
        }

        //Second point-float
        public PointF fSecondPoint;


        //

        //Must line be deleted?-(it has only one point,it is out of screen)
        public bool deleted;

        //Must line move?
        public bool AddPointsToStart;

        //Controlling collision
        public bool controll = true;

        //Is line drawn?
        public bool drawn;

        //Speed of line
        int speed = 2;


        //Transformations

        //Reflection
        public AuxiliaryClasses.Reflection reflection;

        //Teleport
        //Place of teleport
        List<PointF> tplace = new List<PointF>();

        //How much must be line moved?
        PointF movement = new PointF(0, 0);

        //Point where line must be moved
        public List<PointF> telepoint = new List<PointF>();



        //Methods

        //Add new point
        public void Add(PointF p)
        {
            Points.Add(p);
        }

        //Add new telepoint
        public void AddTelepoint(PointF p)
        {
            //Telepoint
            telepoint.Add(p);
            //Calculate Movement
            movement = new PointF(p.X - fStart.X, p.Y - fStart.Y);
            //Place of teleport
            tplace.Add(fStart);
        }

        //Main method
        //Move line
        public void NextPoint()
        {
            //Second point
            fSecondPoint = this.fStart;

            //Length >= speed
            int length = 0;

            //while length is lower then speed
            while (length < speed && Count > 1)
            {
                //Denominator calculate length and return denominator of line segment which split the line segment to next two line segments.
                int denominator = Denominator(ref length);

                //Add point
                Add(denominator);

                //Delete point
                Delete(denominator);

            }
        }

        //Auxilliary methods for NextPoint()
        //Calate denominator
        int Denominator(ref int length)
        {
            //Result
            int denominator = 0;

            //Points-last,last but one
            PointF p = Points[0];
            PointF p2 = Points[1];

            //Difference between p.X/Y and p2.X/Y
            int width = (int)Math.Abs(p.X - p2.X);
            int height = (int)Math.Abs(p.Y - p2.Y);

            //Calculate difference(length) (p,p2)
            int _length = (int)Math.Sqrt((double)(width * width + height * height));

            //if _length is lower  then what  I need , add full line segment(denominator == 0)
            if ((speed - length) > _length)
            {
                length += _length;
            }

            //else calculate the denominator of next line segment
            else
            {
                denominator = _length / (speed - length);
                length = speed;
            }


            return denominator;
        }


        //Add line segment
        //Main
        void Add(int denominator)
        {
            //Full line segment
            if (this.AddPointsToStart && drawn && denominator == 0)
            {
                //Calculate point
                PointF v;
                AddA(out v);

                //Add
                AddC(v);
            }
            //Part of line segment
            else if (this.AddPointsToStart && drawn && denominator != 0)
            {
                //Calculate
                PointF v;
                AddB(out v, denominator);

                //Add
                AddC(v);
            }
        }

        //Calculate new point
        void AddA(out PointF v)
        {
            v = new PointF(0, 0);
            v.X = (fStart.X + Points[1].X - Points[0].X);
            v.Y = (fStart.Y + Points[1].Y - Points[0].Y);
        }

        //Calculate new point
        void AddB(out PointF v, int denominator)
        {
            v = new PointF(0, 0);
            v.X = (fStart.X + (Points[1].X - Points[0].X) / denominator);
            v.Y = (fStart.Y + (Points[1].Y - Points[0].Y) / denominator);
        }

        //Add point
        void AddC(PointF v)
        {
            //Reflection
            if (reflection != null)
            { v = reflection.Reflect(fStart, v); }

            //Add point
            Points.Add(v);

            //Is point out off screen?
            if (!((fStart.X) < 600 && (fStart.X) > 0 && (fStart.Y) < 600 && (fStart.Y) > 0))
            {
                AddPointsToStart = false;
            }

            //Teleport
            if (movement.X != 0f && movement.Y != 0f)
            {
                Points[Points.Count - 1] = new PointF(fStart.X + movement.X, fStart.Y + movement.Y);
                movement = new PointF(0f, 0f);
                controll = false;
            }
        }

        //Delete line segment
        void Delete(int denominator)
        {
            //Full line segment
            if (drawn && denominator == 0)
            {
                //Delete point
                Points.RemoveAt(0);

                //Delete line if line has only one point
                DeleteB();

                //if line is teleported, delete teleport line
                if (tplace.Count > 0 && Points.Count > 0 && Points[0].X == tplace[0].X && Points[0].Y == tplace[0].Y)
                {
                    Points.RemoveAt(0); telepoint.RemoveAt(0); tplace.RemoveAt(0);
                }
            }
            //Part of line segment 
            else if (drawn && denominator != 0)
            {
                //Delete point
                PointF _p = Points[0];
                Points[0] = new PointF(_p.X + ((Points[1].X - _p.X) / denominator), _p.Y + ((Points[1].Y - _p.Y) / denominator));

                //Delete line if line has only one point
                DeleteB();
            }
        }

        //Delete line if line has only one point
        void DeleteB()
        {
            //Delete line
            if (Count == 1)
            {
                this.deleted = true;
            }
        }


        //Line can move
        public void PrepairForMove()
        {
            AddPointsToStart = true;
            drawn = true;
        }


        //Draw
        public void Draw(Graphics grfx)
        {
            //Points to draw
            List<PointF> points = new List<PointF>();

            //For all points in line
            for (int i = 0; i < Count; i++)
            {
                //Add point to points
                points.Add(Points[i]);

                //If point point is end of line or teleport
                if (points.Count > 0 && ((tplace.Count > 0 && IsHereTeleport(points[points.Count - 1])) || i == Count - 1))
                {
                    //Draw line
                    if (points.Count > 2)//more than 2 points
                    {
                        grfx.DrawLines(new Pen(new SolidBrush(Color.Black), 2.5f), points.ToArray());
                    }

                    //Clear points for new part of line
                    points.Clear();
                }
            }
        }

        //Check if the point is teleport
        bool IsHereTeleport(PointF b)
        {
            //For all teleports
            foreach (PointF g in tplace)
            {
                //Is b teleport
                if (b.X == g.X && b.Y == g.Y) return true;
            }
            return false;
        }

    }

    namespace AuxiliaryClasses
    {
        internal class Reflection
        {
            //Constructor
            //Angle
            public Reflection(double u)
            {
                Angle = u;
                if (Angle < 0) { Angle = 6.283184 - Math.Abs(Angle); }
            }

            //Two points
            public Reflection(PointF p1, PointF p2)
            {
                Angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);

                Angle += 1.5708;
                if (Angle < 0) { Angle = 6.283184 - Math.Abs(Angle); }

            }

            //Angle
            double Angle;

            public PointF Reflect(PointF p1, PointF p2)
            {
                //Width height
                float width = p2.X - p1.X;
                float height = p2.Y - p1.Y;

                //Angle (p1,p2)
                double Angle2 = Math.Atan2(height, width);
                if (Angle2 < 0) { Angle2 = 6.283184 - Math.Abs(Angle2); }

                //Result
                double result = 0;

                //If angle is bad, reverse it
                if (AuxiliaryClasses.Semicircle.AngleRight(Angle, Angle2))
                {
                    //Calculate movement
                    double anglemovement = Angle - Angle2;

                    //Calculate result angle
                    result = Angle + anglemovement;


                }
                //If angle is right,l eave it as it
                else
                {
                    result = Angle2;
                }

                //Side c
                double c = Math.Sqrt(Math.Abs(width * width) + Math.Abs(height * height));

                //Calculate new point from angle2(after reflection)
                PointF p = new PointF(
                  (float)(p1.X + (Math.Cos(result) * c)),
                  (float)(p1.Y + (Math.Sin(result) * c))
                );


                return p;
            }
        }

        internal static class Semicircle
        {
            //Is point p3 on the left side from straight line(p1,p2)
            internal static bool Left(PointF p2, PointF p1, PointF p3)
            {
                //Calculate first angle
                double Angle1 = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
                if (Angle1 < 0) { Angle1 = 6.283184 - Math.Abs(Angle1); }

                //Calculate second angle
                double Angle2 = Math.Atan2(p3.Y - p2.Y, p3.X - p2.X);
                if (Angle2 < 0) { Angle2 = 6.283184 - Math.Abs(Angle2); }

                //Result
                bool result = AngleLeft(Angle1, Angle2);

                //vysledok
                return result;
            }

            //Is point p3 on the right side from straight line(p1,p2)
            internal static bool Right(PointF p2, PointF p1, PointF p3)
            {
                return !Left(p2, p1, p3);
            }

            //Is Angle2 on the left side from Angle1(
            internal static bool AngleLeft(double Angle1, double Angle2)
            {
                //If point Angle2 is on the left side from Angle1
                if (Angle2 < Angle1)
                {
                    //more than 180°
                    if (Angle2 > (Angle1 - 3.141592))
                    { return true; }
                    else
                    { return false; }
                }

                //If point Angle2 is on the right side from Angle1
                else if (Angle2 > Angle1)
                {
                    //more than 180°
                    double distance = Angle1 + (6.283184 - Angle2);
                    if (distance < 3.141592)
                    { return true; }
                    else
                    { return false; }
                }
                return false;
            }

            //Is Angle2 on the right side from Angle1
            internal static bool AngleRight(double Angle1, double Angle2)
            {
                return !AngleLeft(Angle1, Angle2);
            }
        }

        public static class Serializer
        {
            //Save level
            public static void Serialize(StreamWriter sw, Level l)
            {
                //Drawing
                sw.Write(WritePoint(l.Drawing.p1));
                sw.Write("*");
                sw.Write(WritePoint(l.Drawing.p2));

                //Separator
                sw.Write("|");

                //Blocks
                foreach (Blocks.Block b in l.Blocks)
                {
                    //Type of block
                    sw.Write(b.Type.ToString());
                    sw.Write("+");

                    //Points of block
                    foreach (Point p in b.Points)
                    {
                        //Write
                        sw.Write(WritePoint(p));
                        sw.Write("*");
                    }

                    //Separator
                    sw.Write("/");
                }

                //Close file
                sw.Close();

            }

            //Write point
            static string WritePoint(Point p)
            {
                return p.X.ToString() + ";" + p.Y.ToString();
            }

            //Load level
            public static Level Deserialize(StreamReader sr)
            {
                //Level
                Level l = new Level();
                l.Blocks = new List<Blocks.Block>();

                //Load file
                string r = sr.ReadLine();

                //Get drawing
                string[] s = r.Split('|');
                string[] s1 = s[0].Split('*');
                l.Drawing = new Drawing.Drawing(ReadPoint(s1[0]), ReadPoint(s1[1]));

                //Blocks
                string[] blocks = s[1].Split('/');

                //Add blocks
                foreach (string block in blocks)
                {
                    if (block != "")
                    {
                        //Type and points
                        string[] a = block.Split('+');

                        //Points
                        List<Point> points = new List<Point>();
                        string[] _points = a[1].Split('*');
                        foreach (string point in _points)
                        {
                            if (point != "")
                            {
                                points.Add(ReadPoint(point));
                            }
                        }

                        //Type
                        switch (Convert.ToInt32(a[0]))
                        {
                            case 1:
                                {
                                    l.Blocks.Add(new Blocks.Rectangle(points));
                                    break;
                                }
                            case 2:
                                {
                                    l.Blocks.Add(new Blocks.Circle(points));
                                    break;
                                }
                            case 3:
                                {
                                    l.Blocks.Add(new Blocks.OrangeBlock(points));
                                    break;
                                }
                            case 4:
                                {
                                    l.Blocks.Add(new Blocks.RedCircle(points));
                                    break;
                                }
                            case 5:
                                {
                                    l.Blocks.Add(new Blocks.Polygon(points));
                                    break;
                                }
                            case 6:
                                {
                                    l.Blocks.Add(new Blocks.RedPolygon(points));
                                    break;
                                }
                            case 7:
                                {
                                    l.Blocks.Add(new Blocks.Teleport(points));
                                    break;
                                }

                            case 8:
                                {
                                    l.Blocks.Add(new Blocks.RedRectangle(points));
                                    break;
                                }
                            case 9:
                                {
                                    l.Blocks.Add(new Blocks.RedSquare(points));
                                    break;
                                }
                            case 10:
                                {
                                    l.Blocks.Add(new Blocks.Square(points));
                                    break;
                                }
                            case 11:
                                {
                                    l.Blocks.Add(new Blocks.OrangeBlockGyratory(points));
                                    break;
                                }
                            case 12:
                                {
                                    l.Blocks.Add(new Blocks.OrangeBlockMotile(points));
                                    break;
                                }
                        }

                    }
                }

                //Return level
                return l;

            }

            //Read point
            static Point ReadPoint(string s)
            {
                string[] s1 = s.Split(';');
                return new Point(Convert.ToInt32(s1[0]), Convert.ToInt32(s1[1]));

            }
        }

        static class Bloker
        {

            //Create block from name
            public static Blocks.Block FromName(string name)
            {
                //Type
                switch (name)
                {
                    case "Rectangle(2 p.)":
                        {
                            return new Blocks.Rectangle();

                        }
                    case "Circle(2 p.)":
                        {
                            return new Blocks.Circle();

                        }
                    case "OrangeBlock(2 p.)":
                        {
                            return new Blocks.OrangeBlock();

                        }
                    case "RedCircle(2 p.)":
                        {
                            return new Blocks.RedCircle();

                        }
                    case "Polygon(n p.,n>2)":
                        {
                            return new Blocks.Polygon();

                        }
                    case "RedPolygon(n p.,n>2)":
                        {
                            return new Blocks.RedPolygon();

                        }
                    case "Teleport(3 p.)":
                        {
                            return new Blocks.Teleport();

                        }

                    case "RedRectangle(4 p.)":
                        {
                            return new Blocks.RedRectangle();
                        }
                    case "RedSquare(4 p.)":
                        {
                            return new Blocks.RedSquare();
                        }
                    case "Square(2 p.)":
                        {
                            return new Blocks.Square();
                        }
                    case "OrangeBlock-Gyratory(3 p.)":
                        {
                            return new Blocks.OrangeBlockGyratory();
                        }
                    case "OrangeBlock-Motile(3 p.)":
                        {
                            return new Blocks.OrangeBlockMotile();
                        }
                }
                return null;
            }

        }
    }
    //Level class for serialization 
    public class Level
    {
        //Constructors
        public Level()
        {

        }
        public Level(Drawing.Drawing _drawing, List<Blocks.Block> _blocks)
        {
            Drawing = _drawing;
            Blocks = _blocks;
        }

        //Variables
        public Drawing.Drawing Drawing { get; set; }
        public List<Blocks.Block> Blocks { get; set; }

    }

    namespace LevelEditorComponents
    {
        internal class WhitePanel : Panel
        {
            protected override void OnPaintBackground(PaintEventArgs e) { }
        }

        //
        //Save
        //
        
        public partial class Save : Form
        {
            public Save()
            {
                InitializeComponent();
            }
        }

        partial class Save
        {
            private System.ComponentModel.IContainer components = null;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
                this.label1 = new System.Windows.Forms.Label();
                this.textBox1 = new System.Windows.Forms.TextBox();
                this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
                this.button1 = new System.Windows.Forms.Button();
                this.flowLayoutPanel1.SuspendLayout();
                this.flowLayoutPanel2.SuspendLayout();
                this.SuspendLayout();
                // 
                // flowLayoutPanel1
                // 
                this.flowLayoutPanel1.AutoSize = true;
                this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.flowLayoutPanel1.Controls.Add(this.label1);
                this.flowLayoutPanel1.Controls.Add(this.textBox1);
                this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
                this.flowLayoutPanel1.Name = "flowLayoutPanel1";
                this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
                this.flowLayoutPanel1.Size = new System.Drawing.Size(188, 49);
                this.flowLayoutPanel1.TabIndex = 0;
                // 
                // label1
                // 
                this.label1.AutoSize = true;
                this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                this.label1.Location = new System.Drawing.Point(13, 10);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(126, 29);
                this.label1.TabIndex = 0;
                this.label1.Text = "Number of level:";
                // 
                // textBox1
                // 
                this.textBox1.Location = new System.Drawing.Point(145, 13);
                this.textBox1.MaximumSize = new System.Drawing.Size(30, 20);
                this.textBox1.MaxLength = 3;
                this.textBox1.Name = "textBox1";
                this.textBox1.Size = new System.Drawing.Size(30, 20);
                this.textBox1.TabIndex = 1;
                // 
                // flowLayoutPanel2
                // 
                this.flowLayoutPanel2.AutoSize = true;
                this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
                this.flowLayoutPanel2.Controls.Add(this.button1);
                this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
                this.flowLayoutPanel2.Location = new System.Drawing.Point(1, 0);
                this.flowLayoutPanel2.Name = "flowLayoutPanel2";
                this.flowLayoutPanel2.Size = new System.Drawing.Size(194, 84);
                this.flowLayoutPanel2.TabIndex = 1;
                // 
                // button1
                // 
                this.button1.AutoSize = true;
                this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.button1.Dock = System.Windows.Forms.DockStyle.Right;
                this.button1.Location = new System.Drawing.Point(128, 58);
                this.button1.Name = "button1";
                this.button1.Size = new System.Drawing.Size(63, 23);
                this.button1.TabIndex = 2;
                this.button1.Text = "Save level";
                this.button1.UseVisualStyleBackColor = true;
                // 
                // Save
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.AutoSize = true;
                this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.ClientSize = new System.Drawing.Size(284, 262);
                this.Controls.Add(this.flowLayoutPanel2);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "Save";
                this.ShowInTaskbar = false;
                this.Text = "Save";
                this.flowLayoutPanel1.ResumeLayout(false);
                this.flowLayoutPanel1.PerformLayout();
                this.flowLayoutPanel2.ResumeLayout(false);
                this.flowLayoutPanel2.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();

            }

            private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
            private System.Windows.Forms.Label label1;
            public System.Windows.Forms.TextBox textBox1;
            private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
            private System.Windows.Forms.Button button1;
        }

        //
        //Open
        //

        public partial class Open : Form
        {
            public Open()
            {
                InitializeComponent();
            }
        }

        partial class Open
        {
            private System.ComponentModel.IContainer components = null;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
                this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
                this.label1 = new System.Windows.Forms.Label();
                this.textBox1 = new System.Windows.Forms.TextBox();
                this.button1 = new System.Windows.Forms.Button();
                this.flowLayoutPanel1.SuspendLayout();
                this.flowLayoutPanel2.SuspendLayout();
                this.SuspendLayout();
                // 
                // flowLayoutPanel1
                // 
                this.flowLayoutPanel1.AutoSize = true;
                this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
                this.flowLayoutPanel1.Controls.Add(this.button1);
                this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
                this.flowLayoutPanel1.Location = new System.Drawing.Point(1, 0);
                this.flowLayoutPanel1.Name = "flowLayoutPanel1";
                this.flowLayoutPanel1.Size = new System.Drawing.Size(194, 84);
                this.flowLayoutPanel1.TabIndex = 0;
                // 
                // flowLayoutPanel2
                // 
                this.flowLayoutPanel2.AutoSize = true;
                this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.flowLayoutPanel2.Controls.Add(this.label1);
                this.flowLayoutPanel2.Controls.Add(this.textBox1);
                this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
                this.flowLayoutPanel2.Name = "flowLayoutPanel2";
                this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(10);
                this.flowLayoutPanel2.Size = new System.Drawing.Size(188, 49);
                this.flowLayoutPanel2.TabIndex = 0;
                // 
                // label1
                // 
                this.label1.AutoSize = true;
                this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                this.label1.Location = new System.Drawing.Point(13, 10);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(126, 29);
                this.label1.TabIndex = 0;
                this.label1.Text = "Number of level:";
                // 
                // textBox1
                // 
                this.textBox1.Location = new System.Drawing.Point(145, 13);
                this.textBox1.MaximumSize = new System.Drawing.Size(30, 20);
                this.textBox1.MaxLength = 3;
                this.textBox1.Name = "textBox1";
                this.textBox1.Size = new System.Drawing.Size(30, 20);
                this.textBox1.TabIndex = 1;
                // 
                // button1
                // 
                this.button1.AutoSize = true;
                this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.button1.Dock = System.Windows.Forms.DockStyle.Right;
                this.button1.Location = new System.Drawing.Point(123, 58);
                this.button1.Name = "button1";
                this.button1.Size = new System.Drawing.Size(68, 23);
                this.button1.TabIndex = 2;
                this.button1.Text = "Open level";
                this.button1.UseVisualStyleBackColor = true;
                // 
                // Open
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.AutoSize = true;
                this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.ClientSize = new System.Drawing.Size(284, 262);
                this.Controls.Add(this.flowLayoutPanel1);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "Open";
                this.Text = "Open level";
                this.flowLayoutPanel1.ResumeLayout(false);
                this.flowLayoutPanel1.PerformLayout();
                this.flowLayoutPanel2.ResumeLayout(false);
                this.flowLayoutPanel2.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();

            }
            
            private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
            private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
            private System.Windows.Forms.Label label1;
            public System.Windows.Forms.TextBox textBox1;
            public System.Windows.Forms.Button button1;
        }
        public partial class Help : Form
        {
            public Help()
            {
                InitializeComponent();
            }
        }
        
        //
        //Help
        //
        
        partial class Help
        {
            private System.ComponentModel.IContainer components = null;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
                this.label1 = new System.Windows.Forms.Label();
                this.label5 = new System.Windows.Forms.Label();
                this.tableLayoutPanel1.SuspendLayout();
                this.SuspendLayout();
                // 
                // tableLayoutPanel1
                // 
                this.tableLayoutPanel1.AutoSize = true;
                this.tableLayoutPanel1.ColumnCount = 1;
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
                this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
                this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
                this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
                this.tableLayoutPanel1.Name = "tableLayoutPanel1";
                this.tableLayoutPanel1.RowCount = 2;
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
                this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 262);
                this.tableLayoutPanel1.TabIndex = 0;
                // 
                // label1
                // 
                this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
                this.label1.AutoSize = true;
                this.label1.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                this.label1.Location = new System.Drawing.Point(53, 0);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(177, 45);
                this.label1.TabIndex = 0;
                this.label1.Text = "Instruction";
                // 
                // label5
                // 
                this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
                this.label5.AutoSize = true;
                this.label5.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                this.label5.Location = new System.Drawing.Point(4, 45);
                this.label5.Name = "label5";
                this.label5.Size = new System.Drawing.Size(276, 45);
                this.label5.TabIndex = 4;
                this.label5.Text = "Select block from left listbox and draw points into the blue area(count is in bracket)\n(polygon must be drawn in a clockwise direction).\nYou can move points of block. \nIf you want delete block select block from right listbox which you want to delete and click on \"DeleteBlock\" button. \nIf you want to save(open) level click on \"SaveLevel\"(\"LoadLevel\") button.";
                // 
                // Help
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.AutoSize = true;
                this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                this.ClientSize = new System.Drawing.Size(284, 262);
                this.Controls.Add(this.tableLayoutPanel1);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.Name = "Help";
                this.Text = "Help";
                this.tableLayoutPanel1.ResumeLayout(false);
                this.tableLayoutPanel1.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();

            }

            private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            private System.Windows.Forms.Label label1;
            private System.Windows.Forms.Label label5;
        }
    }

    //
    //Graphical components
    //

    //Menu
    public class Menu
    {
        //Constructor
        public Menu(Window _area)
        {
            //Drawing area(Window)
            area = _area;

            //Clicking
            area.MouseUp += Click;
        }

        //Methods
        //Tick
        public void Ticking(object s, EventArgs e)
        {
            ProcessingInputs = true;
            Draw();
        }

        //Background
        Image b = Bitmap.FromFile(@"LaLinea-Images\LaLinea-Background-Menu.gif");

        //Draw
        void Draw()
        {
            area.g.DrawImage(b, 0, 0, 600, 600);
        }


        //Clicking
        void Click(object s, MouseEventArgs e)
        {
            if (Activity && ProcessingInputs)
            {
                //If user clicked on Play button
                if (e.X < 385 && e.X > 196 && e.Y < 385 && e.Y > 196)
                {
                    //Open select level component
                    area.t.Tick -= this.Ticking;
                    area.t.Tick += area.sl.Ticking;
                    Activity = false;
                    area.sl.Activity = true;
                }
                //If user clicked on Level Editor button
                if (e.X < 80 && e.X > 20 && e.Y < 586 && e.Y > 523)
                {
                    //Open Level Editor
                    LaLinea.LevelEditor le = new LevelEditor();
                    le.Show();
                }
                //If user clicked on Exit button
                if (e.X < 80 && e.X > 20 && e.Y < 518 && e.Y > 450)
                {
                    //Exit Application
                    Application.Exit();
                }
            }
        }

        //Can component process inputs?
        bool ProcessingInputs = false;

        //Is component active?
        private bool _activity;

        public bool Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                _activity = value;
                if (_activity)
                {
                    //Don't handle input from the previous component
                    ProcessingInputs = false;
                }
            }
        }

        //Window
        Window area;
    }

    //Select level
    public class SelectLevel
    {
        //Constructor
        public SelectLevel(Window _area)
        {
            //Window
            area = _area;

            //Count levels
            CountLevels();

            //Inputs
            area.MouseUp += Click;
        }

        //Methods
        //Count levels
        public static void CountLevels()
        {
            bool next = true;
            while (next)
            {
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, @"LaLinea-Levels\LaLinea-" + (_Count + 1).ToString())))
                { _Count++; }
                else
                { next = false; }
            }
        }

        //Tick
        public void Ticking(object s, EventArgs e)
        {
            ProcessingInputs = true;
            Draw();
        }

        //Images
        Image b = Bitmap.FromFile(@"LaLinea-Images\LaLinea-Background-SelectLevel.gif");
        Image l = Bitmap.FromFile(@"LaLinea-Images\LaLinea-SelectLevel-Square.gif");//Level image

        //Movement
        //Size
        int sizeofmovement = 0;
        //Axis
        int axis = 0;

        //Draw
        void Draw()
        {
            //Background
            area.g.DrawImage(b, 0, 0, 600, 600);

            //Move levels
            if (sizeofmovement != 0)
            {
                axis += sizeofmovement;
                if (Math.Abs(axis) == 600) { FirstLevel += (sizeofmovement == 20) ? -3 : 3; sizeofmovement = 0; axis = 0; }
            }

            //Draw levels
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //Number of level
                    int level = FirstLevel - 3 + (i * 3) + j;

                    //Draw levels-not unnecessarily
                    if (level <= Count && (!(sizeofmovement == 20 && (level > FirstLevel + 2)) || !(sizeofmovement == -20 && (level < FirstLevel)) || !(sizeofmovement == 0 && (level < FirstLevel || level > FirstLevel + 2))))
                    {
                        //Square
                        area.g.DrawImage(l, -530 + (600 * i) + (160 * j) + axis, 230, 130, 130);
                        //Number
                        area.g.DrawString(level.ToString(), new Font("Comic Sans MS", 48), new SolidBrush(Color.Black), -530 + (600 * i) + (160 * j) + axis, 247);
                    }
                }
            }
        }

        //Click
        void Click(object s, MouseEventArgs e)
        {
            if (Activity && ProcessingInputs)
            {
                //If user clicked on level
                if (e.Y < 360 && e.Y > 230)
                {
                    //Which level?
                    for (int i = 0; i < 3; i++)
                    {
                        //Hittest
                        if (FirstLevel + i <= Count && e.X > (70 + i * 160) && e.X < (70 + i * 160 + 130))
                        {
                            SelectedLevel = FirstLevel + i;
                            area.t.Tick -= this.Ticking;
                            //Get level
                            StreamReader sr = new StreamReader(@"LaLinea-Levels\LaLinea-" + (FirstLevel + i).ToString());
                            area.l.Level = LaLinea.AuxiliaryClasses.Serializer.Deserialize(sr);
                            sr.Close();
                            area.l.RePlay();
                            area.t.Tick += area.l.Ticking;
                            area.l.Activity = true;
                            Activity = false;
                        }
                    }
                }

                //Move to left
                if (e.Y < 330 && e.Y > 270 && e.X < 40 && e.X > 20)
                {
                    if (FirstLevel != 1) { if (sizeofmovement == 0)sizeofmovement = 20; }
                }

                //Move to right
                if (e.Y < 330 && e.Y > 270 && e.X < 580 && e.X > 560)
                {
                    if (FirstLevel + 3 <= Count) { if (sizeofmovement == 0)sizeofmovement = -20; }
                }

                //Menu
                else if (e.Y < 560 && e.Y > 488 && e.X < 335 && e.X > 265)
                {
                    //Open menu
                    area.t.Tick -= this.Ticking;
                    area.m.Activity = true;
                    area.t.Tick += area.m.Ticking;
                    Activity = false;
                }
            }
        }

        //Can component process inputs?
        bool ProcessingInputs = false;

        //Activity
        private bool _activity;

        public bool Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                _activity = value;
                if (_activity)
                {
                    ProcessingInputs = false;
                }
            }
        }

        //Number of levels
        static int _Count = 0;
        public int Count
        {
            get
            {
                return _Count;
            }
            set
            {
                _Count = value;
            }
        }

        //First level
        private int FirstLevel = 1;

        //Selected level
        private int _SelectedLevel = 1;

        public int SelectedLevel
        {
            get
            {
                return _SelectedLevel;
            }
            set
            {
                _SelectedLevel = value;
                FirstLevel = (int)(Math.Ceiling((double)SelectedLevel / (double)3) * 3) - 2;
            }
        }

        Window area;
    }

    //Pause
    public class Pause
    {
        //Constructor
        public Pause(Window _area)
        {
            //Window
            area = _area;

            //Inputs
            area.MouseUp += Click;
        }

        //Methods
        //Tick
        public void Ticking(object s, EventArgs e)
        {
            ProcessingInputs = true;
            Move();
            Draw();
        }

        //Move block
        void Move()
        {
            //Move
            if (axis < 0) { axis += sizeofmovement; }
        }

        //Image
        Image b = Bitmap.FromFile(@"LaLinea-Images\LaLinea-Background-Pause.gif");

        //Movement
        int axis = -200;
        int sizeofmovement = 10;

        //Draw
        void Draw()
        {
            area.g.DrawImage(b, 0 + axis, 0, 174, 600);
        }


        //Click
        void Click(object s, MouseEventArgs e)
        {
            if (Activity && ProcessingInputs)
            {
                //Replay button
                if (e.Y < 253 && e.Y > 185 && e.X < 90 && e.X > 22)
                {
                    area.l.RePlay();
                }
                //Continue button
                else if (e.Y < 314 && e.Y > 246 && e.X < 154 && e.X > 86)
                {
                    area.l.Play();
                }
                //Select level button
                else if (e.Y < 379 && e.Y > 311 && e.X < 89 && e.X > 21)
                {
                    //Open select level component
                    Cursor.Position = new Point(this.area.Location.X + 300, Cursor.Position.Y);
                    area.t.Tick += this.Ticking;
                    area.t.Tick -= area.l.Ticking;
                    area.l.RePlay();
                    area.sl.Activity = true;
                    area.t.Tick += area.sl.Ticking;
                    Activity = false;
                    area.l.pause = true;
                }
                //Instructions
                else if (e.Y < 550 && e.Y > 510 && e.X < 70 && e.X > 30)
                {
                    area.t.Tick += area.ins.Ticking;
                    area.ins.Activity = true;
                }
            }
        }

        //Can component process inputs?
        bool ProcessingInputs = false;

        //Is component active?
        private bool _activity;

        public bool Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                _activity = value;
                if (_activity)
                {
                    axis = -200;
                    ProcessingInputs = false;
                }
            }
        }

        //Window
        Window area;
    }

    //If player won the game
    public class Win
    {
        //Constructor
        public Win(Window _area)
        {
            //Window
            area = _area;

            //Inputs
            area.MouseUp += Click;
        }

        //Methods
        //Tick
        public void Ticking(object s, EventArgs e)
        {
            ProcessingInputs = true;
            Move();
            Draw();
        }

        //Move component
        void Move()
        {
            //Move
            if (axis < 0) { axis += sizeofmovement; }
        }

        //Load background
        Image b = Bitmap.FromFile(@"LaLinea-Images\LaLinea-Background-Win.gif");

        //Movement
        int axis = -600;
        int sizeofmovement = 10;

        //Draw
        void Draw()
        {
            //Draw background
            area.g.DrawImage(b, 150, 0 + axis, 300, 600);
        }


        //Click
        void Click(object s, MouseEventArgs e)
        {
            if (Activity && ProcessingInputs)
            {
                //Replay button
                if (e.Y < 570 && e.Y > 502 && e.X < 99 + 150 && e.X > 31)
                {
                    area.l.RePlay();
                }
                //Next level button
                else if (e.Y < 570 && e.Y > 502 && e.X < 275 + 150 && e.X > 207 + 150)
                {
                    //Next level
                    if (area.sl.Count > area.sl.SelectedLevel)
                    {
                        StreamReader sr = new StreamReader(@"LaLinea-Levels\LaLinea-" + (++area.sl.SelectedLevel).ToString());
                        //Load level
                        area.l.Level = LaLinea.AuxiliaryClasses.Serializer.Deserialize(sr);
                        sr.Close();
                        area.l.RePlay();
                        area.l.Activity = true;
                    }
                    //If isn't next level go to menu
                    else
                    {
                        area.t.Tick -= area.l.Ticking;
                        area.l.RePlay();
                        area.m.Activity = true;
                        area.t.Tick += area.m.Ticking;
                        area.l.pause = true;
                    }
                }
                //Select level
                else if (e.Y < 570 && e.Y > 502 && e.X < 189 + 150 && e.X > 121 + 150)
                {
                    area.t.Tick -= area.l.Ticking;
                    area.sl.Activity = true;
                    area.t.Tick += area.sl.Ticking;
                    Activity = false;
                    area.l.pause = true;
                }
            }
        }

        //Can component process inputs?
        bool ProcessingInputs = false;

        //Is component active?
        private bool _activity;

        public bool Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                _activity = value;
                if (_activity)
                {
                    axis = -600;
                    ProcessingInputs = false;
                }
            }
        }

        Window area;
    }

    //Instructions
    public class Instructions
    {
        //Constructor
        public Instructions(Window _area)
        {
            //Window(with this object you can run components)
            area = _area;

            //Inputs 
            area.MouseUp += Click;
        }

        //Methods
        //Tick
        public void Ticking(object s, EventArgs e)
        {
            ProcessingInputs = true;
            Draw();
        }

        //Load background
        Image o = Bitmap.FromFile(@"LaLinea-Images\LaLinea-Background-Help.gif");

        //Draw background
        void Draw()
        {
            area.g.DrawImage(o, 100, 200, 400, 200);
        }


        //Click
        void Click(object s, MouseEventArgs e)
        {
            if (Activity && ProcessingInputs)
            {
                //Ok button
                if (e.Y < 400 && e.Y > 350 && e.X < 500 && e.X > 450)
                {
                    //Close instruction
                    area.t.Tick -= this.Ticking;
                    Activity = false;
                }
            }
        }

        //Can component process inputs?
        bool ProcessingInputs = false;

        //Is component active
        private bool _activity;

        public bool Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                _activity = value;
                if (_activity)
                {
                    //Don't handle input from the previous component
                    ProcessingInputs = false;
                }
            }
        }

        Window area;
    }

    //Start screen
    public class StartScreen
    {

        //Constructor
        public StartScreen(Window _area)
        {
            //Window(with this object you can run components)
            area = _area;

            //Load images
            for (int i = 1; i < 9; i++)
            {
                a.Add((Bitmap)Bitmap.FromFile(@"LaLinea-Images\LaLinea-Introducing\LaLinea-" + i.ToString() + ".gif"));
            }

            //Input handlers
            area.KeyDown += Press;
            area.MouseUp += Press2;
        }

        //Images
        List<Bitmap> a = new List<Bitmap>();

        //Index of image
        int e = 0;
        int d = 0;


        //Methods
        //Tick
        public void Ticking(object s, EventArgs e)
        {
            //Draw next image
            if (Activity)
            {
                Draw();
            }
        }

        //Draw image
        void Draw()
        {
            e++;

            //Draw image
            area.g.DrawImage(a[d], 0, 0, 600, 600);

            //Calculate next index of image
            if (e % 3 == 0)
            {
                d++;
                if (d == 7) { d = 0; }
            }
        }


        //Key down
        void Press(object s, KeyEventArgs e)
        {
            //Run menu
            if (Activity)
            {
                area.t.Tick -= this.Ticking;
                area.t.Tick += area.m.Ticking;
                area.m.Activity = true;
                Activity = false;
            }
        }

        //Click
        void Press2(object s, MouseEventArgs e)
        {
            //Run menu
            if (Activity)
            {
                area.t.Tick -= this.Ticking;
                area.t.Tick += area.m.Ticking;
                area.m.Activity = true;
                Activity = false;
            }
        }

        //Is component active?
        bool Activity = true;

        Window area;
    }

    //Level editor
    public partial class LevelEditor : Form
    {
        //Variables
        string type;//type of the selected block
        Level level;
        List<Point> drawedpoints = new List<Point>();
        int selectedpoint = -1;
        Blocks.Block selectedblock;
        bool Cancel = false;
        Graphic.Background background = new Graphic.Background();

        //Methods
        //Constructor
        public LevelEditor()
        {
            //Designer
            InitializeComponent();

            //Level
            level = new Level();
            level.Blocks = new List<Blocks.Block>();

            //
            Form3_Resize(null, null);
        }


        #region Logic

        //Select type of block
        private void listBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            //Change type of block and delete drawed points
            ListBox s = (ListBox)sender;
            drawedpoints.Clear();
            type = (string)s.SelectedItem;
            panel1.Invalidate();
        }

        //Draw new point
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!Cancel)
            {
                //Calculate new point
                Point p = new Point((int)(e.X * Scale), (int)(e.Y * Scale));

                //Ad new point
                drawedpoints.Add(p);
                panel1.Invalidate();
            }
            Cancel = false;
        }

        //Create block
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.listBox2.SelectedItem != null)
            {
                //Drawing
                if ((string)this.listBox2.SelectedItem == "Drawing(2 points)")
                {
                    if (drawedpoints.Count == 2)
                    {
                        //Create drawing
                        level.Drawing = new Drawing.Drawing(drawedpoints[0], drawedpoints[1]);

                        //Test
                        level.Drawing.Test();

                        //Delete drawed points
                        drawedpoints = new List<Point>();

                        panel1.Invalidate();
                    }
                }
                //Block
                else
                {

                    //Create block
                    Blocks.Block b = AuxiliaryClasses.Bloker.FromName((string)this.listBox2.SelectedItem);

                    //It must be accurate number of points.
                    if ((b.NoP == -1 && drawedpoints.Count > 2) || (b.NoP == drawedpoints.Count))
                    {
                        //Add points and blocks
                        b.Points = drawedpoints;
                        level.Blocks.Add(b);

                        //Add block to listbox
                        listBox1.Items.Add(b);

                        //Delete drawed points
                        drawedpoints = new List<Point>();

                        panel1.Invalidate();
                    }
                }
            }
        }

        //Select block
        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        //Delete selected block
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                bool b = level.Blocks.Remove((Blocks.Block)listBox1.SelectedItem);
                listBox1.Items.Remove(listBox1.SelectedItem);
                panel1.Invalidate();
            }
        }

        //Move point of block
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Calculate point
            Point p = new Point((int)(e.X * Scale), (int)(e.Y * Scale));

            if (e.Button == MouseButtons.Left)
            {
                //if is some point selected
                if (selectedpoint != -1)
                {
                    //Drawing
                    if (selectedblock == null)
                    {
                        if (selectedpoint == 1) { level.Drawing.p1 = p; level.Drawing.Test(); }
                        else if (selectedpoint == 2) { level.Drawing.p2 = p; level.Drawing.Test(); }
                        panel1.Invalidate();
                    }
                    //Block
                    else
                    {
                        //Change point
                        List<Point> k = selectedblock.Points;
                        k[selectedpoint] = p;
                        selectedblock.Points = selectedblock.Points;

                        panel1.Invalidate();
                    }
                }
            }
        }

        //Point is moved.
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            selectedblock = null;
            selectedpoint = -1;
            panel1.Invalidate();
        }

        //Select point
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Calculate point
                Point p = new Point((int)(e.X * Scale), (int)(e.Y * Scale));

                //Which point?
                foreach (Blocks.Block b in level.Blocks)
                {
                    int i = -1;
                    foreach (Point p2 in b.Points)
                    {
                        i++;
                        if (Hittest(p, p2))
                        {
                            //Select point
                            selectedpoint = i;
                            selectedblock = b;
                            panel1.Invalidate();

                            Cancel = true;

                            return;
                        }
                    }
                }

                //Point is from drawing
                if (level.Drawing != null)
                {
                    if (Hittest(p, level.Drawing.p1))
                    {
                        //Select point
                        selectedpoint = 1;
                        selectedblock = null;
                        panel1.Invalidate();

                        Cancel = true;

                        return;
                    }
                    else if (Hittest(p, level.Drawing.p2))
                    {
                        //Select point
                        selectedpoint = 2;
                        selectedblock = null;
                        panel1.Invalidate();

                        Cancel = true;

                        return;
                    }
                }

            }
        }

        //Save level
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LaLinea.LevelEditorComponents.Save u = new LaLinea.LevelEditorComponents.Save();
            if (u.ShowDialog() == DialogResult.OK && (level.Drawing != null))
            {
                string a = u.textBox1.Text;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(@"LaLinea-Levels\LaLinea-" + a);
                LaLinea.AuxiliaryClasses.Serializer.Serialize(sw, level);
                sw.Close();
                SelectLevel.CountLevels();
            }
        }

        //Open level
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            LaLinea.LevelEditorComponents.Open u = new LaLinea.LevelEditorComponents.Open();
            if (u.ShowDialog() == DialogResult.OK)
            {
                string a = u.textBox1.Text;
                try
                {
                    //Open level
                    System.IO.StreamReader sr = new System.IO.StreamReader(@"LaLinea-Levels\LaLinea-" + a);
                    level = LaLinea.AuxiliaryClasses.Serializer.Deserialize(sr);
                    sr.Close();
                    //Add blocks to listbox
                    foreach (LaLinea.Blocks.Block b in level.Blocks)
                    {
                        listBox1.Items.Add(b);
                    }

                }
                catch { }
            }
            panel1.Invalidate();
        }

        //Help
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            LaLinea.LevelEditorComponents.Help h = new LaLinea.LevelEditorComponents.Help();
            h.Show();
        }

        #endregion


        #region Graphics

        //Draw
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics
            Bitmap bmp = new Bitmap(600, 600);
            Graphics g = Graphics.FromImage(bmp);

            //Draw background
            background.Draw(g);

            //Drawing
            if (level.Drawing != null)
            {
                level.Drawing.Draw(g);
                DrawPoints(new List<Point>() { level.Drawing.p1, level.Drawing.p2 }, g, Color.Chocolate);
            }

            //Blocks
            foreach (Blocks.Block b in level.Blocks)
            {
                //Block
                b.Draw(g);

                //Points
                DrawPoints(b.Points, g, Color.Blue);
            }

            //Selected block
            if (listBox1.SelectedItem != null)
            {
                DrawPoints(((Blocks.Block)listBox1.SelectedItem).Points, g, Color.Red);
            }

            //Drawed pints
            DrawPoints(drawedpoints, g, Color.Green);

            //Selected point
            if (selectedpoint != -1)
            {
                //Block
                if (selectedblock != null)
                {
                    DrawPoints(new List<Point>() { selectedblock.Points[selectedpoint] }, g, Color.Red);
                }
                //Drawing
                else
                {
                    if (selectedpoint == 1) { DrawPoints(new List<Point>() { level.Drawing.p1 }, g, Color.Red); }
                    else if (selectedpoint == 2) { DrawPoints(new List<Point>() { level.Drawing.p2 }, g, Color.Red); }
                }
            }

            //Grid
            for (int i = 0; i < 600; i += 10)
            {
                g.DrawLine(new Pen(Color.Black, 1), new Point(0, i), new Point(600, i));
                g.DrawLine(new Pen(Color.Black, 1), new Point(i, 0), new Point(i, 600));
            }

            //Draw
            e.Graphics.DrawImage(bmp, 0, 0, panel1.Size.Width, panel1.Size.Height);
        }

        //Draw points
        private void DrawPoints(List<Point> points, Graphics g, Color c)
        {
            foreach (Point b in points)
            {
                g.FillRectangle(new SolidBrush(c), b.X - 5, b.Y - 5, 10, 10);
            }
        }

        //Set size of panel
        private void Form3_Resize(object sender, EventArgs e)
        {
            this.panel1.Width = this.panel1.Height;
            this.panel1.Invalidate();
        }

        #endregion


        //Hittest
        bool Hittest(Point p1, Point p2)
        {
            if ((Math.Abs(p1.X - p2.X) < 5) && (Math.Abs(p1.Y - p2.Y) < 5))
            {
                return true;
            }
            return false;
        }

        //Scale of panel1
        double Scale
        {
            get
            {
                return 600d / panel1.Size.Height;
            }
        }
    }

    partial class LevelEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.panel1 = new LaLinea.LevelEditorComponents.WhitePanel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3,
            this.toolStripSeparator2,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripButton4,
            this.toolStripSeparator3,
            this.toolStripButton5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1184, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Image = (Bitmap.FromFile(@"LaLinea-Images\LaLinea-CreateBlock.gif"));
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "CreateBlock";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Image = (Bitmap.FromFile(@"LaLinea-Images\LaLinea-DeleteBlock.gif"));
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "DeleteBlock";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Image = (Bitmap.FromFile(@"LaLinea-Images\LaLinea-SaveLevel.gif"));
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "SaveLevel";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Image = (Bitmap.FromFile(@"LaLinea-Images\LaLinea-OpenLevel.gif"));
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "OpenLevel";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(1064, 25);
            this.listBox1.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(120, 437);
            this.listBox1.TabIndex = 5;
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Items.AddRange(new object[] {
            "Drawing(2 points)",
            "OrangeBlock(2 p.)",
            "Square(2 p.)",
            "RedSquare(4 p.)",
            "Rectangle(2 p.)",
            "RedRectangle(4 p.)",
            "Circle(2 p.)",
            "RedCircle(2 p.)",
            "Polygon(n p.,n>2)",
            "RedPolygon(n p.,n>2)",
            "Teleport(3 p.)",
            "OrangeBlock-Gyratory(3 p.)",
            "OrangeBlock-Motile(3 p.)"
            });
            this.listBox2.Location = new System.Drawing.Point(884, 25);
            this.listBox2.Margin = new System.Windows.Forms.Padding(10);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(180, 437);
            this.listBox2.TabIndex = 6;
            this.listBox2.SelectedValueChanged += new System.EventHandler(this.listBox2_SelectedValueChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(809, 437);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = (Bitmap.FromFile(@"LaLinea-Images\LaLinea-Help.gif"));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Text = "Help";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1184, 462);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(1200, 500);
            this.Name = "LevelEditor";
            this.Text = "Level Editor";
            this.Resize += new System.EventHandler(this.Form3_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private LevelEditorComponents.WhitePanel panel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton4;


    }


    //Window
    public class Window : Form
    {
        public Window()
        {
            //Properties
            FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(600, 600);
            this.MinimumSize = new Size(600, 600);

            //Icon
            this.Icon = new Icon(@"LaLinea.ico");
            this.ShowIcon = true;

            //Graphics-all compnents will draw images here
            b = new Bitmap(600, 600);
            g = Graphics.FromImage(b);

            //Timer
            this.t = new Timer();
            this.t.Interval = 1;
            this.t.Enabled = true;

            //Drawing
            t.Tick += delegate { try { this.CreateGraphics().DrawImageUnscaled(b, 0, 0, this.Size.Width, this.Size.Height); } catch { } };

            //Intialize variables

            //Game's core
            l = new LaLinea.Core.Core(this);

            //Start screen
            u = new LaLinea.StartScreen(this);
            this.t.Tick += this.u.Ticking;

            //Menu
            m = new LaLinea.Menu(this);

            //Select level
            sl = new LaLinea.SelectLevel(this);

            //Pause
            pau = new LaLinea.Pause(this);

            //Win
            w = new LaLinea.Win(this);

            //Instruction
            ins = new LaLinea.Instructions(this);

        }

        //Variables

        //Graphics
        public Graphics g;
        public Bitmap b;

        //Timer
        public Timer t;

        //Components

        //La linea
        public LaLinea.Core.Core l;

        //Menu
        public LaLinea.Menu m;

        //Level select
        public LaLinea.SelectLevel sl;

        //Pause
        public LaLinea.Pause pau;

        //Win
        public LaLinea.Win w;

        //Instuction
        public LaLinea.Instructions ins;

        //Start screen
        public LaLinea.StartScreen u;

        //No automatic painting
        protected override void OnPaint(PaintEventArgs e) { }
        protected override void OnPaintBackground(PaintEventArgs e) { }

        //Main method
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Window());
        }
    }
}
