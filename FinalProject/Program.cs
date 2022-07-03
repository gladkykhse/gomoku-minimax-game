using Cairo;
using FinalProject;
using Gdk;
using Gtk;
using Color = Cairo.Color;
using static Gdk.EventMask;
static class Globals
{
    public const int ScreenWidth = 850;
    public const int ScreenHeight = 850;
}

class Area : DrawingArea
{
    // Properties
    private readonly Color lineColor = new Color(0, 0, 0),
        deskColor = new Color(236, 200, 0),
        firstPlayerColor = new Color(1, 0, 0),
        secondPlayerColor = new Color(0, 0, 1);

    private Game game;
    
    // Constructor
    public Area(Game g) {
        AddEvents((int) ButtonPressMask);
        game = g;
    }

    // Methods
    protected override bool OnDrawn (Context c)
    {
        // Screen background color
        c.SetSourceColor(deskColor);
        c.Rectangle(0,0,Globals.ScreenWidth, Globals.ScreenHeight);
        c.Fill();
        
        // Playing grid
        c.SetSourceColor(lineColor);
        c.LineWidth = 1;
        for (int i = 100; i < 800; i += 50) 
        {
            c.MoveTo(i, 50);
            c.LineTo(i, 800);
        }
        for (int i = 100; i < 800; i += 50)
        {
            c.MoveTo(50, i);
            c.LineTo(800, i);
        }
        c.ClosePath();
        c.Stroke();

        
        for (int i = 0; i < game.sizeX; i++)
        {
            for (int j = 0; j < game.sizeY; j++)
            {
                if (game.grid[i, j] == 1)
                {
                    // Circle player 1
                    c.SetSourceColor(firstPlayerColor);
                    c.Arc(i * 50 + 75, j * 50 + 75, 20, 0,2 * Math.PI);
                    c.Fill();
                }
                else if (game.grid[i, j] == 2)
                {
                    // Circle player 2
                    c.SetSourceColor(secondPlayerColor);
                    c.Arc(i * 50 + 75, j * 50 + 75, 20, 0,2 * Math.PI);
                    c.Fill();
                }
                    
            }
        }

        // If there is a winner
        if (game.winner > 0)
        {
            c.SetSourceColor(lineColor);
            c.LineWidth = 1;
            c.MoveTo(75 + game.winnerData.Item1 * 50, 75 + game.winnerData.Item2 * 50);
            c.LineTo(75 + game.winnerData.Item1 * 50 + game.winnerData.Item3 * (game.winSize - 1) * 50, 75 + game.winnerData.Item2 * 50 + game.winnerData.Item4 * (game.winSize - 1) * 50);
            c.ClosePath();
            c.Stroke();
        }

        return true;
    }
    protected override bool OnButtonPressEvent (EventButton e)
    {
        if (game.newGame)
            game = new Game();
        if (e.X > 50 && e.X < Globals.ScreenHeight && e.Y > 50 && e.Y < Globals.ScreenWidth &&
            game.CanMove((int) e.X / 50 - 1, (int) e.Y / 50 - 1))
        {
            
            // Player's move
            game.Move((int) e.X / 50 - 1, (int) e.Y / 50 - 1);

            // Computer's move
            (int, int)? AImove;
            Minimax.Predict(game, out AImove, int.MinValue, int.MaxValue);
            
            if (AImove.HasValue)
            {
                (int, int) m = AImove.Value;
                game.Move(m.Item1, m.Item2);
            }
        }

        QueueDraw();
        return true;
    }
}

class MyWindow : Gtk.Window {
    public MyWindow(Game game) : base("Gomoku") {
        Resize(Globals.ScreenWidth, Globals.ScreenHeight);
        Add(new Area(game));
    }

    // Event handler
    protected override bool OnDeleteEvent(Event ev) {
        Application.Quit();
        return true;
    }
}

class Program {
    static void Main() {
        Application.Init();
        
        Game game = new Game();
        MyWindow w = new MyWindow(game);
        
        w.ShowAll();
        Application.Run();
    }
}