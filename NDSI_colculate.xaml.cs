using Microsoft.Win32;
using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;
using System.Runtime.InteropServices;
//using System.Drawing.Drawing2D;



namespace Iskitim
{

    public partial class NDSI_colculate : Window
    {
        public NDSI_colculate()
        {
            InitializeComponent();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            GdalConfiguration.ConfigureGdal();//нужная строка без которой все пойдет по накатаной 


            OpenFileDialog openFileDialog = new OpenFileDialog();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "(*.tif)|*.tif";

            string path = "C:\\";
            openFileDialog.InitialDirectory = path;
            if (openFileDialog.ShowDialog() == true)
                path = openFileDialog.FileName;

            string path2 = "C:\\";
            openFileDialog.InitialDirectory = path2;
            if (openFileDialog.ShowDialog() == true)
                path2 = openFileDialog.FileName;


            Gdal.AllRegister();

            // Загружаем данные красного канала.
            Dataset redDataset = Gdal.Open(path, Access.GA_ReadOnly);
            Band redBand = redDataset.GetRasterBand(1);

            // Загружаем данные инфракрасного канала.
            Dataset nirDataset = Gdal.Open(path2, Access.GA_ReadOnly);
            Band nirBand = nirDataset.GetRasterBand(1);

            // Проверяем, что размеры образцов совпадают и что они не слишком велики.
            int width = redBand.XSize;
            int height = redBand.YSize;
            if (width != nirBand.XSize || height != nirBand.YSize)
            {
                throw new ArgumentException("Размеры образцов не совпадают.");
            }
            if (width > 5000 || height > 5000)
            {
                throw new ArgumentException("Размер образцов слишком велик, это может привести к проблемам с производительностью.");
            }

            // Выделяем память под массивы красного и инфракрасного каналов.
            float[] red = new float[width * height];
            float[] nir = new float[width * height];

            // Читаем данные из файлов в массивы.
            redBand.ReadRaster(0, 0, width, height, red, width, height, 0, 0);
            nirBand.ReadRaster(0, 0, width, height, nir, width, height, 0, 0);

            // Рассчитываем NDSI в каждой точке.
            double[] ndsi = new double[width * height];
            string str = txt.Text;
            double porog = Convert.ToDouble(str);
            for (int i = 0; i < width * height; i++)
            {
                ndsi[i] = (red[i] - nir[i]) / (red[i] + nir[i]);
                if (ndsi[i] > porog)
                {
                    ndsi[i] = porog;
                }

                Console.WriteLine(ndsi[i]);
            }

            string path3 = "C:\\";
            dlg.InitialDirectory = path2;
            if (dlg.ShowDialog() == true)
                path3 = dlg.FileName;

            // Сохраняем результат в новый файл.
            Gdal.AllRegister();
            Driver driver = Gdal.GetDriverByName("GTiff");
            Dataset ndsiDataset = driver.Create(path3, width, height, 1, DataType.GDT_Float32, null);
            ndsiDataset.SetProjection(redDataset.GetProjection());
            //ndsiDataset.SetGeoTransform(redDataset.GetGeoTransform());
            Band ndsiBand = ndsiDataset.GetRasterBand(1);
            ndsiBand.WriteRaster(0, 0, width, height, ndsi, width, height, 0, 0);
            ndsiDataset.FlushCache();
            // Освобождаем ресурсы.
            redBand.Dispose();
            redDataset.Dispose();
            nirBand.Dispose();
            nirDataset.Dispose();
            ndsiBand.Dispose();
            ndsiDataset.Dispose();

            MessageBoxResult result = MessageBox.Show("файл успешно сохранен");

            //Bitmap image = new Bitmap(path3);
            //Bitmap newImage = new Bitmap(image.Width, image.Height);

            //for (int i = 0; i < image.Width; i++)
            //{
            //    for (int j = 0; j < image.Height; j++)
            //    {
            //        Color pixelColor = image.GetPixel(i, j);

            //        int grayValue = (int)((pixelColor.R * 0.3f) + (pixelColor.G * 0.59f) + (pixelColor.B * 0.11f));
            //        int newRed = 255 - grayValue;
            //        int newGreen = 255 - grayValue;
            //        int newBlue = 0;

            //        Color newColor = Color.FromArgb(pixelColor.A, newRed, newGreen, newBlue);
            //        newImage.SetPixel(i, j, newColor);
            //    }
            //}

            //newImage.Save(path3 + "1.tif", ImageFormat.Tiff);

            //// Открыть TIF файл
            //Bitmap tifImage = new Bitmap(path3);
            //byte[,] pixels = new byte[width, height];

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        Color color = tifImage.GetPixel(x, y);
            //        byte intensity = (byte)((color.R + color.G + color.B) / 3);
            //        pixels[x, y] = intensity;
            //    }
            //}

            //// Изменить градиент
            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        byte intensity = pixels[x, y];
            //        byte yellow = 255;
            //        byte black = 0;

            //        if (intensity < 128)
            //        {
            //            yellow = (byte)(255 - 2 * intensity);
            //        }
            //        else
            //        {
            //            black = (byte)(2 * (intensity - 128));
            //            yellow = (byte)(255 - black);
            //        }

            //        tifImage.SetPixel(x, y, Color.FromArgb(yellow, yellow, black));
            //    }
            //}
            //// Сохранить измененный файл
            //tifImage.Save(path3 + "1.tif", ImageFormat.Tiff);

            //Bitmap image = new Bitmap(path3);
            //Bitmap newImage = new Bitmap(image.Width, image.Height);
            //for (int i = 0; i < image.Width; i++)
            //{
            //    for (int j = 0; j < image.Height; j++)
            //    {
            //        Color pixelColor = image.GetPixel(i, j);

            //        int grayValue = (int)((pixelColor.R * 0.3f) + (pixelColor.G * 0.59f) + (pixelColor.B * 0.11f));
            //        int newRed = 255 - grayValue;
            //        int newGreen = 255 - grayValue;
            //        int newBlue = 0;

            //        Color newColor = Color.FromArgb(pixelColor.A, newRed, newGreen, newBlue);
            //        newImage.SetPixel(i, j, newColor);
            //    }
            //}
            //newImage.Save(path3 + "1.tif", ImageFormat.Tiff);

            //Bitmap bmp = new Bitmap(path3);
            //for (int y = 0; y < bmp.Height; y++)
            //{
            //    for (int x = 0; x < bmp.Width; x++)
            //    {
            //        Color pixelColor = bmp.GetPixel(x, y);
            //        int gray = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);

            //        int redd = (int)(255 * Math.Min(1.0, gray / 128.0));
            //        int green = (int)(255 * Math.Max(0.0, (gray - 128.0) / 128.0));
            //        int blue = 255;

            //        Color newColor = Color.FromArgb(redd, green, blue);
            //        bmp.SetPixel(x, y, newColor);
            //    }
            //}
            //bmp.Save(path3 + "1.tif", ImageFormat.Tiff);
            string photo = path3+ "1.tif";
            Bitmap bmp = new Bitmap(path3);
            List<Color> colors = new List<Color>();
            for (int x = 0; x < bmp.Width; x++)
            {
                Color color = bmp.GetPixel(x, 0);
                if (!colors.Contains(color))
                {
                    colors.Add(color);
                }
            }

            foreach (Color color in colors)
            {
                if (color.R < 128 && color.G < 128 && color.B < 128)
                {
                    bmp = ReplaceColor(bmp, color, Color.Black);
                }
                else if (color.R >= 128 && color.G >= 128 && color.B < 128)
                {
                    bmp = ReplaceColor(bmp, color, Color.Yellow);
                }
            }

            bmp.Save(path3 + "1.tif");

            Bitmap ReplaceColor(Bitmap bmp2, Color fromColor, Color toColor)
            {
                Bitmap result2 = new Bitmap(bmp2.Width, bmp2.Height);
                for (int x = 0; x < bmp2.Width; x++)
                {
                    for (int y = 0; y < bmp2.Height; y++)
                    {
                        if (bmp2.GetPixel(x, y) == fromColor)
                        {
                            result2.SetPixel(x, y, toColor);
                        }
                        else
                        {
                            result2.SetPixel(x, y, bmp2.GetPixel(x, y));
                        }
                    }
                }
                return result2;
            }


            Console.WriteLine(path3);
            image.Source = new BitmapImage(new Uri(photo));
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
