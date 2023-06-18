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
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot.Axes;

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


            if (!double.TryParse(txt.Text, out double ndsiValue))
            {
                MessageBox.Show("Ошибка: значение должно быть числом.");
            }
            else
            {
                double number;
                bool isNumeric = double.TryParse(txt.Text, out number);
                if (!isNumeric || number < 0.1 || number > 0.6)
                {
                    MessageBox.Show("Ошибка! Введите число от 0.1 до 0.6.");
                }
                else
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "(*.tif)|*.tif";


                    string path = "C:\\";
                    openFileDialog.InitialDirectory = path;
                    if (openFileDialog.ShowDialog() == true)
                        path = openFileDialog.FileName;


                    if (path == "C:\\")
                    {
                        return;
                    }
                    else
                    {
                        string path2 = "C:\\";
                        openFileDialog.InitialDirectory = path;
                        if (openFileDialog.ShowDialog() == true)
                            path2 = openFileDialog.FileName;

                        if (path2 == "C:\\")
                        {
                            return;
                        }
                        else
                        {
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
                                    Console.WriteLine(ndsi[i]);
                                    ndsi[i] = 255; // Белый цвет
                                }
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

                            // открываем изображение
                            Bitmap bmp = new Bitmap(path3);
                            // определяем цвет, который нужно заменить
                            Color replaceColor = Color.White;
                            // определяем цвет, на который нужно заменить
                            Color newColor = Color.Yellow;

                            // проходим по каждому пикселю изображения
                            for (int y = 0; y < bmp.Height; y++)
                            {
                                for (int x = 0; x < bmp.Width; x++)
                                {
                                    // получаем текущий цвет пикселя
                                    Color pixelColor = bmp.GetPixel(x, y);
                                    // проверяем, если цвет пикселя соответствует цвету, который нужно заменить
                                    if (pixelColor.ToArgb() == replaceColor.ToArgb())
                                    {
                                        // заменяем цвет пикселя на новый
                                        bmp.SetPixel(x, y, newColor);
                                    }
                                }
                            }

                            // сохраняем измененное изображение
                            bmp.Save(path3 + "test.tif", System.Drawing.Imaging.ImageFormat.Tiff);

                            bmp.Dispose();

                            string photo = path3 + "test.tif";
                            Console.WriteLine(path3);
                            image.Source = new BitmapImage(new Uri(photo));

                            string filePath = path3;

                            try
                            {
                                // Удаляем файл
                                File.Delete(filePath);
                            }
                            catch (IOException i)
                            {
                                Console.WriteLine("Произошла ошибка при удалении файла: " + i.Message);
                            }
                        }
                    }
                }
            }
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // создание папки для хранения изображений
            Directory.CreateDirectory("UserIMGS");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "(*.tif)|*.tif";

            string imgPath1 = "C:\\";
            openFileDialog.InitialDirectory = imgPath1;
            if (openFileDialog.ShowDialog() == true)
                imgPath1 = openFileDialog.FileName;


            if (imgPath1 == "C:\\")
            {
                return;
            }
            else
            {
                string imgPath2 = "C:\\";
                openFileDialog.InitialDirectory = imgPath1;
                if (openFileDialog.ShowDialog() == true)
                    imgPath2 = openFileDialog.FileName;

                if (imgPath2 == "C:\\")
                {
                    return;
                }
                else
                {
                    string imgPath3 = "C:\\";
                    openFileDialog.InitialDirectory = imgPath1;
                    if (openFileDialog.ShowDialog() == true)
                        imgPath3 = openFileDialog.FileName;

                    if (imgPath3 == "C:\\")
                    {
                        return;
                    }
                    else
                    {
                        // копирование изображений в папку UserIMGS
                        File.Copy(imgPath1, "UserIMGS/image1.tif", true);
                        File.Copy(imgPath2, "UserIMGS/image2.tif", true);
                        File.Copy(imgPath3, "UserIMGS/image3.tif", true);

                        // загрузка изображений
                        Bitmap image1 = new Bitmap("UserIMGS/image1.tif");
                        Bitmap image2 = new Bitmap("UserIMGS/image2.tif");
                        Bitmap image3 = new Bitmap("UserIMGS/image3.tif");

                        int[,] raster3 = new int[image1.Width, image1.Height];
                        int[,] raster6 = new int[image2.Width, image2.Height];
                        Color pixelColor3;
                        Color pixelColor6;

                        string str = txt.Text;
                        double userndsi = Convert.ToDouble(str);

                        for (int i = 0; i < image1.Width; i++)
                        {
                            for (int j = 0; j < image1.Height; j++)
                            {
                                pixelColor3 = image1.GetPixel(i, j);
                                raster3[i, j] = (int)pixelColor3.R + (int)pixelColor3.G + (int)pixelColor3.B;

                                pixelColor6 = image2.GetPixel(i, j);
                                raster6[i, j] = (int)pixelColor6.R + (int)pixelColor6.G + (int)pixelColor6.B;
                            }
                        }
                        //выделение пылевого загрязнения на панхроматичесом файле 
                        Bitmap cpandata8 = new Bitmap(image3.Width, image3.Height);
                        int[,] pandata8 = new int[image3.Width, image3.Height];
                        Color pixelColor8;
                        for (int i = 0; i < image3.Width; i++)
                        {
                            for (int j = 0; j < image3.Height; j++)
                            {
                                pixelColor8 = image3.GetPixel(i, j);
                                pandata8[i, j] = (int)(0.2989 * pixelColor8.R + 0.5870 * pixelColor8.G + 0.1140 * pixelColor8.B);
                                Console.WriteLine(pandata8[i, j]);

                                double NDSIp = (raster3[i, j] - raster6[i, j]) / (double)(raster3[i, j] + raster6[i, j]);
                                if (userndsi <= NDSIp && NDSIp <= 0.65)
                                {
                                    if (pandata8[i, j] <= 40)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 0, 0));
                                    }
                                    else if (40 < pandata8[i, j] && pandata8[i, j] <= 42)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 25, 0));
                                    }
                                    else if (42 < pandata8[i, j] && pandata8[i, j] <= 44)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 50, 0));
                                    }
                                    else if (44 < pandata8[i, j] && pandata8[i, j] <= 46)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 75, 0));
                                    }
                                    else if (48 < pandata8[i, j] && pandata8[i, j] <= 50)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 100, 0));
                                    }
                                    else if (50 < pandata8[i, j] && pandata8[i, j] <= 52)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 125, 0));
                                    }
                                    else if (52 < pandata8[i, j] && pandata8[i, j] <= 54)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 150, 0));
                                    }
                                    else if (54 < pandata8[i, j] && pandata8[i, j] <= 56)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 175, 0));
                                    }
                                    else if (56 < pandata8[i, j] && pandata8[i, j] <= 58)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 200, 0));
                                    }
                                    else if (58 < pandata8[i, j] && pandata8[i, j] <= 60)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 0));
                                    }
                                    else if (60 < pandata8[i, j] && pandata8[i, j] <= 62)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 0));
                                    }
                                    else if (62 < pandata8[i, j] && pandata8[i, j] <= 64)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 25));
                                    }
                                    else if (64 < pandata8[i, j] && pandata8[i, j] <= 66)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 50));
                                    }
                                    else if (66 < pandata8[i, j] && pandata8[i, j] <= 68)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 75));
                                    }
                                    else if (68 < pandata8[i, j] && pandata8[i, j] <= 70)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 100));
                                    }
                                    else if (70 < pandata8[i, j] && pandata8[i, j] <= 72)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 125));
                                    }
                                    else if (72 < pandata8[i, j] && pandata8[i, j] <= 74)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 150));
                                    }
                                    else if (74 < pandata8[i, j] && pandata8[i, j] <= 76)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 175));
                                    }
                                    else if (76 < pandata8[i, j] && pandata8[i, j] <= 78)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 200));
                                    }
                                    else if (78 < pandata8[i, j] && pandata8[i, j] <= 80)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                                    }
                                    else if (pandata8[i, j] > 80)
                                    {
                                        cpandata8.SetPixel(i, j, Color.FromArgb(200, 255, 255));
                                    }
                                }
                            }
                        }

                        // сохранение измененного изображения
                        cpandata8.Save("UserIMGS/processed_image.tif");

                        Bitmap combinateBitmap = new Bitmap(image3);

                        Graphics graphics = Graphics.FromImage(combinateBitmap);
                        graphics.DrawImage(image3, new System.Drawing.Rectangle(0, 0, image3.Width, image3.Height));
                        graphics.DrawImage(cpandata8, new System.Drawing.Rectangle(0, 0, cpandata8.Width, cpandata8.Height));

                        combinateBitmap.Save("UserIMGS/Final.tif");

                       
                        Console.WriteLine("Обработка изображений завершена.");

                        string path3 = "C:\\";
                        dlg.InitialDirectory = imgPath1;
                        if (dlg.ShowDialog() == true)
                            path3 = dlg.FileName;

                        combinateBitmap.Save(path3);

                        string photo = path3;

                        image.Source = new BitmapImage(new Uri(photo));
                                                                        
                        Hcal gr = new Hcal(path3);
                        gr.Show();
                    }
               }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Graph gr = new Graph();
            gr.Show();
        }
    }
}
     