using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using OxyPlot;
using System.Windows.Media;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot.Annotations;
using System.ComponentModel;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace Iskitim
{
    /// <summary>
    /// Логика взаимодействия для Hcal.xaml
    /// </summary>
    public partial class Hcal : Window
    {
        
        public Hcal(string path3)
            {
                InitializeComponent();

            //// Load the image from file
            //BitmapImage bmp = new BitmapImage(new Uri(path3, UriKind.Relative));
            //Image img = new Image();
            //img.Source = bmp;

            //// Create a grid to hold the image and color bar
            //Grid grid = new Grid();
            //ColumnDefinition colDef1 = new ColumnDefinition();
            //ColumnDefinition colDef2 = new ColumnDefinition();
            //grid.ColumnDefinitions.Add(colDef1);
            //grid.ColumnDefinitions.Add(colDef2);

            //// Add the image to the first column of the grid
            //Grid.SetColumn(img, 0);
            //grid.Children.Add(img);

            //// Create the color bar using a rectangle with a linear gradient brush
            //Rectangle rect = new Rectangle();
            //LinearGradientBrush lgb = new LinearGradientBrush(
            //    Colors.YellowGreen, Colors.DeepSkyBlue, new Point(0, 0), new Point(0, 1));
            //lgb.GradientStops.Add(new GradientStop(Colors.YellowGreen, 0.0));
            //lgb.GradientStops.Add(new GradientStop(Colors.DeepSkyBlue, 1.0));
            //rect.Fill = lgb;
            //rect.Width = 20;
            //rect.Height = 200;
            //rect.Margin = new System.Windows.Thickness(5);

            //// Add the color bar to the second column of the grid
            //Grid.SetColumn(rect, 1);
            //grid.Children.Add(rect);

            //// Display the grid in a window
            //Window win = new Window();
            //win.Content = grid;
            //win.Show();

            //MessageBox.Show("Готово");

            

            

            image.Source = new BitmapImage(new Uri(path3));
        }
    }
 }

