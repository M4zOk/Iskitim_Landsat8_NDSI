using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Iskitim
{
    /// <summary>
    /// Логика взаимодействия для Colibration.xaml
    /// </summary>
    public partial class Colibration : Window
    {
        public Colibration()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            string path3 = "C:\\";
            openFileDialog.InitialDirectory = path3;
            if (openFileDialog.ShowDialog() == true)
                path3 = openFileDialog.FileName;


            // Открыть TIF-файл
            Image image = Image.FromFile(path3);

            // Новое разрешение
            int newWidth = image.Width * 2;
            int newHeight = image.Height * 2;

            // Создать новый Bitmap с новым размером
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            // Использовать Graphics для настройки интерполяции
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));
            }

            // Сохранить новое изображение в TIF-файле
            newImage.Save("C:\\Users\\Игнат\\Desktop\\LC08_L1TP_135023_20150130_B3-6-8\\NDSI_TEST.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            MessageBox.Show("Изображение успешно откалибровано.");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();

            string path5 = "C:\\";
            openFileDialog2.InitialDirectory = path5;
            if (openFileDialog2.ShowDialog() == true)
                path5 = openFileDialog2.FileName;


            OpenFileDialog openFileDialog = new OpenFileDialog();

            string path4 = "C:\\";
            openFileDialog.InitialDirectory = path4;
            if (openFileDialog.ShowDialog() == true)
                path4 = openFileDialog.FileName;


            // Открыть TIF-файл
            Image image = Image.FromFile(path4);

            // Новое разрешение
            int newWidth = image.Width / 2;
            int newHeight = image.Height / 2;

            // Создать новый Bitmap с новым размером
            Bitmap newImage1 = new Bitmap(newWidth, newHeight);

            // Использовать Graphics для настройки интерполяции
            using (Graphics graphics = Graphics.FromImage(newImage1))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));
            }

            // Сохранить новое изображение в TIF-файле
            newImage1.Save("C:\\Users\\Игнат\\Desktop\\LC08_L1TP_135023_20150130_B3-6-8\\NDSI_TEST.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            MessageBox.Show("Изображение успешно откалибровано.");
        }
    }
}
