using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaTurtle;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Refresh_OnClick(object? sender, RoutedEventArgs e)
    {
        turtle.Init();
        
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                turtle.Forward(100);
                turtle.Right(60);
            }

            turtle.Right(120);
        }

        turtle.Start();
    }

    private void Fastdraw_OnClick(object? sender, RoutedEventArgs e)
    {
        turtle.Init();
        for (int i = 0; i < 4; i++)
        {
            turtle.Forward(120);
            turtle.Right(90);
        }

        turtle.Start(TurtleSpeed.FAST);
    }

    private void Circle_OnClick(object? sender, RoutedEventArgs e)
    {
        turtle.Init();
        for (int i = 0; i < 180; i++)
        {
            turtle.Forward(5);
            turtle.Right(2);
        }
        turtle.Start(TurtleSpeed.FAST);
    }

    private void CircleSlow_OnClick(object? sender, RoutedEventArgs e)
    {
        turtle.Init();
        for (int i = 0; i < 45; i++)
        {
            turtle.Forward(20);
            turtle.Right(8);
        }
        turtle.Start(TurtleSpeed.SLOW);
    }
}