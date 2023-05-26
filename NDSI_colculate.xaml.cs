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
            openFileDialog.InitialDirectory = path;
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
                    ndsi[i] = 255; // Белый цвет
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
            bmp.Save(path3+"test.tif", ImageFormat.Tiff);

            string photo = path3 + "test.tif";
            Console.WriteLine(path3);
            image.Source = new BitmapImage(new Uri(photo));
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
     