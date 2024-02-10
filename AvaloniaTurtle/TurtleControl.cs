using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Brushes = Avalonia.Media.Brushes;
using Pen = Avalonia.Media.Pen;
using Point = Avalonia.Point;

namespace AvaloniaTurtle;

class TurtleState
{
    public double x { get; set; } = 0;
    public double y { get; set; } = 0;
    public double dir_x { get; set; } = 0;
    public double dir_y { get; set; } = -1;
}

abstract class TurtleCommand
{
    public abstract void Execute(TurtleState state, DrawingContext context);
}

class Forward : TurtleCommand
{
    public double distance { get; set; }

    public Forward(double dist)
    {
        distance = dist;
    }

    public override void Execute(TurtleState state, DrawingContext context)
    {
        double old_x = state.x;
        double old_y = state.y;

        state.x += distance * state.dir_x;
        state.y += distance * state.dir_y;

        context.DrawLine(
            new Pen(Brushes.Black,2),
            new Point(old_x, old_y),
            new Point(state.x, state.y)
        );
    }
}

class Right : TurtleCommand
{
    public double degrees { get; set; }

    public Right(double deg)
    {
        degrees = deg;
    }

    public override void Execute(TurtleState state, DrawingContext context)
    {
        double rad = (degrees / 180.0 * Math.PI);
        double rad_new = rad + Math.Atan2(state.dir_y, state.dir_x);
        state.dir_x = Math.Cos(rad_new);
        state.dir_y = Math.Sin(rad_new);
    }
}

public enum TurtleSpeed
{
    SLOW,
    FAST
}

public class TurtleControl : Control
{
    public IBrush? Background { get; set; } = Brushes.White;
    private TurtleSpeed speed = TurtleSpeed.SLOW;
    private int nextCommandIdx = 0;
    private List<TurtleCommand> commands = new List<TurtleCommand>();

    public void Forward(double distance)
    {
        commands.Add(new Forward(distance));
    }

    public void Back(double distance)
    {
        commands.Add(new Forward(-distance));
    }

    public void Right(double degrees)
    {
        commands.Add(new Right(degrees));
    }

    public void Left(double degrees)
    {
        commands.Add(new Right(-degrees));
    }

    public void Init()
    {
        commands.Clear();
    }

    public void Start(TurtleSpeed speed = TurtleSpeed.SLOW)
    {
        this.speed = speed;
        nextCommandIdx = 0;
        InvalidateVisual();
    }

    public sealed override void Render(DrawingContext context)
    {
        var renderSize = Bounds.Size;
        if (speed == TurtleSpeed.FAST || nextCommandIdx == 0)
        {
            context.FillRectangle(Background, new Rect(renderSize));
        }

        TurtleState state = new TurtleState();
        state.x = renderSize.Height / 2;
        state.y = renderSize.Width / 2;

        if (speed == TurtleSpeed.FAST)
        {
            foreach (var command in commands)
            {
                command.Execute(state, context);
            }
        }
        else
        {
            if (nextCommandIdx < commands.Count)
            {
                for (int i = 0; i <= nextCommandIdx; i++)
                {
                    var command = commands[i];
                    command.Execute(state, context);
                }

                DrawTurtle(context, state);

                nextCommandIdx++;

                if (nextCommandIdx < commands.Count)
                {
                    Task.Delay(100).ContinueWith(
                        (t) =>
                            Dispatcher.UIThread.Invoke(() =>
                                InvalidateVisual()));
                }
            }
        }

        base.Render(context);
    }

    private static void DrawTurtle(DrawingContext context, TurtleState state)
    {

        //Kopf
        context.DrawEllipse(
            Brushes.Black,
            new Pen(Brushes.DarkGreen),
            new Point(
                state.x + state.dir_x * 13,
                state.y + state.dir_y * 13),
            5, 5);

        double ortho_dir_x1 = state.dir_y;
        double ortho_dir_y1 = -state.dir_x;
        double ortho_dir_x2 = -ortho_dir_x1;
        double ortho_dir_y2 = -ortho_dir_y1;

        //Fuss 1 und 2
        context.DrawEllipse(
            Brushes.Black,
            new Pen(Brushes.DarkGreen),
            new Point(
                state.x + state.dir_x * 7 + ortho_dir_x1 * 7,
                state.y + state.dir_y * 7 + ortho_dir_y1 * 7),
            3, 3);
        context.DrawEllipse(
            Brushes.Black,
            new Pen(Brushes.DarkGreen),
            new Point(
                state.x + state.dir_x * 7 + ortho_dir_x2 * 7,
                state.y + state.dir_y * 7 + ortho_dir_y2 * 7),
            3, 3);
        //Fuss 3 und 4
        context.DrawEllipse(
            Brushes.Black,
            new Pen(Brushes.DarkGreen),
            new Point(
                state.x - state.dir_x * 7 + ortho_dir_x1 * 7,
                state.y - state.dir_y * 7 + ortho_dir_y1 * 7),
            3, 3);
        context.DrawEllipse(
            Brushes.Black,
            new Pen(Brushes.DarkGreen),
            new Point(
                state.x - state.dir_x * 7 + ortho_dir_x2 * 7,
                state.y - state.dir_y * 7 + ortho_dir_y2 * 7),
            3, 3);
        //Turtle zeichnen
        context.DrawEllipse(
            Brushes.Green,
            new Pen(Brushes.DarkGreen),
            new Point(state.x, state.y),
            9, 9);
    }
}