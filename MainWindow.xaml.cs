using Microsoft.Win32;
using OSGeo.GDAL;
using System;
using System.Windows;
using System.Windows.Media.Media3D;
//using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
//using System.Drawing.Drawing2D;

namespace Iskitim
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
  
            NDSI_colculate nDSI_Colculate = new NDSI_colculate();
            nDSI_Colculate.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Colibration colib = new Colibration();
            colib.Show();
        }
    }
}
    
