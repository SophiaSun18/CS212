using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace FernNamespace
{
    /*
     * this class draws a fractal fern when the constructor is called.
     * Written as sample C# code for a CS 212 assignment -- October 2011.
     */
    class Fern
    {
        private static double SEGLENGTH = 4;  // the unit length of each line segment
        private static double BRANCHES = 3;   // the standard sub_length of each branch
        private static int DOTMIN = 10;

        /* 
         * Fern constructor erases screen and draws a fern
         * 
         * Size: number of 3-pixel segments of tendrils
         * Redux: how much smaller children clusters are compared to parents
         * Turnbias: how likely to turn right vs. left (0=always left, 0.5 = 50/50, 1.0 = always right)
         * canvas: the canvas that the fern will be drawn on
         */
        public Fern(double size, double redux, double turnbias, Canvas canvas)
        {
            canvas.Children.Clear();                                // delete old canvas contents
            // draw a new fern branch at the top left corner of the canvas with given parameters
            branch(0, 0, size, redux, turnbias, canvas);
        }

        /*
         * branch draws a main fern branch at the given location and then draws multiple sub_branchses out in 
         * regularly-spaced directions out of the cluster.
         */
        private void branch(int x1, int y1, double size, double redux, double turnbias, Canvas canvas)
        {
            int x2 = x1, y2 = y1;
            Random random = new Random();

            for (double i = 0; i < size; i++)
            {
                // compute the direction of each small segments of the main branch
                double direction = random.NextDouble() + turnbias;
                x1 = x2; y1 = y2;
                // set up the end point of a branch - in this case, branching to the bottom right corner
                x2 = x1 + (int)(SEGLENGTH * Math.Sin(direction));
                y2 = y1 + (int)(SEGLENGTH * Math.Cos(direction));
                // set up the color for the branch
                byte red = (byte)(100 + size / 2);
                byte green = (byte)(220 - size / 3);
                //if (size>120) red = 138; green = 108;
                line(x1, y1, x2, y2, red, green, 0, 1 + size / 80, canvas);

                if (size > BRANCHES)
                {
                    // draw sub_branches with a constant interval
                    if ((i % 10) == 0)
                    {
                        left_branch(x2, y2, size / redux, redux, turnbias, canvas);
                        right_branch(x2, y2, size / redux, redux, turnbias, canvas);
                    }
                }
            }

            // draw the black dots on the fern leaves randomly
            if (size > DOTMIN)
            {
                Random rnd = new Random();
                double i = rnd.NextDouble();

                if (i%2 == 0)
                {
                    circle(x1, y1, 5, canvas);
                }
            }
        }

        /*
         * left_branch draws the left sub_branches of the fern on the certain interval
         * then recursively call the main branch function to keep drawing more sub_branches
         */
        private void left_branch(int x1, int y1, double size, double redux, double turnbias, Canvas canvas)
        {
            int x2 = x1; int y2 = y1;
            Random random = new Random();

            for (double i = 0; i < size; i++)
            {
                // compute the direction of each small segments with more randomness than the main branch
                double direction = random.NextDouble() * (random.NextDouble() + turnbias);
                x1 = x2; y1 = y2;
                x2 = x1 - (int)(SEGLENGTH * Math.Sin(direction));
                y2 = y1 + (int)(SEGLENGTH * Math.Cos(direction));
                byte red = (byte)(100 + size / 2);
                byte green = (byte)(220 - size / 3);
                //if (size>120) red = 138; green = 108;
                line(x1, y1, x2, y2, red, green, 0, 1 + size / 80, canvas);

                if (size > BRANCHES)
                {
                    // recursively call back to the main branch function and draw more sub_branches based on this sub_branch
                    if ((i % 15) == 0)
                    {
                        branch(x2, y2, size / redux, redux, turnbias, canvas);
                    }
                }
            }
        }

        /*
         * right_branch draws the right sub_branches of the fern on the certain interval
         * then recursively call the main branch function to keep drawing more sub_branches
         */
        private void right_branch(int x1, int y1, double size, double redux, double turnbias, Canvas canvas)
        {
            int x2 = x1; int y2 = y1;
            Random random = new Random();

            for (double i = 0; i < size; i++)
            {
                // compute the direction of each small segments with more randomness than the main branch
                double direction = random.NextDouble() * (random.NextDouble() + turnbias);
                x1 = x2; y1 = y2;
                x2 = x1 + (int)(SEGLENGTH * Math.Sin(direction));
                y2 = y1 - (int)(SEGLENGTH * Math.Cos(direction));
                byte red = (byte)(100 + size / 2);
                byte green = (byte)(220 - size / 3);
                //if (size>120) red = 138; green = 108;
                line(x1, y1, x2, y2, red, green, 0, 1 + size / 80, canvas);

                if (size > BRANCHES)
                {
                    // recursively call back to the main branch function and draw more sub_branches based on this sub_branch
                    if ((i % 15) == 0)
                    {
                        branch(x2, y2, size / redux, redux, turnbias, canvas);
                    }
                }
            }
        }

        /*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
        private void line(int x1, int y1, int x2, int y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }

        /*
         * draw a black circle centered at (x,y), radius radius, with a black edge, onto canvas
         */
        private void circle(int x, int y, double radius, Canvas canvas)
        {
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 1.5 * radius;
            myEllipse.Height = 1.5 * radius;
            myEllipse.SetCenter(x, y);
            canvas.Children.Add(myEllipse);
        }
    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
        Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
    }
}

