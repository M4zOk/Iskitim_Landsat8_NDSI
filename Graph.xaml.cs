using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot.Axes;
using Microsoft.Win32;
using System.IO;

namespace Iskitim
{
    /// <summary>
    /// Логика взаимодействия для Graph.xaml
    /// </summary>
    public partial class Graph : Window
    {
        private PlotModel model;
        private void PlotCorrelation(double[] xData, double[] yData)
        {
            // Создаем модель графика.
            var plotModel = new OxyPlot.PlotModel();

            // Создаем серию точек для графика.
            var scatterSeries = new OxyPlot.Series.ScatterSeries();
            for (int i = 0; i < xData.Length; i++)
            {
                scatterSeries.Points.Add(new OxyPlot.Series.ScatterPoint(xData[i], yData[i]));
            }

            // Добавляем серию в модель.
            plotModel.Series.Add(scatterSeries);

            // Создаем ось X и добавляем ее в модель.
            var xAxis = new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom };
            plotModel.Axes.Add(xAxis);

            // Создаем ось Y и добавляем ее в модель.
            var yAxis = new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left };
            plotModel.Axes.Add(yAxis);

            // Устанавливаем заголовок графика.
            plotModel.Title = "Correlation Plot";

            // Отображаем график на элементе PlotView.
            plotView.Model = plotModel;
        }

        public Graph()
        {
            InitializeComponent();

            InitializeComponent();

            // Определение переменной model и инициализация
            double[] x = { 15, 12, 8, 8, 7, 7, 7, 6, 5, 3 };
            double[] y = { 10, 25, 17, 11, 13, 17, 20, 13, 9, 15 };

            model = new PlotModel { Title = "Pearson Correlation" };
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

            var scatterSeries = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStrokeThickness = 1,
                MarkerFill = OxyColors.Blue,
                Title = "Data",
                ItemsSource = Enumerable.Range(0, x.Length)
                    .Select(i => new ScatterPoint(x[i], y[i]))
            };

            model.Series.Add(scatterSeries);

            double correlation = Enumerable.Range(0, x.Length)
                .Select(i => (x[i] - x.Average()) * (y[i] - y.Average()))
                .Sum() / ((x.Length - 1) * x.Select(xi => Math.Pow(xi - x.Average(), 2)).Sum());

            MessageBox.Show($"Коэффициент корреляции: {correlation:F2}");

            plotView.Model = model;


            //double[] xData = { 1, 2, 3, 4, 5 };
            //double[] yData = { 2, 4, 6, 8, 10 };

            //PlotCorrelation(xData, yData);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*",
                Title = "Save graph"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    var pngExporter = new PngExporter { Width = 600, Height = 400 };
                    pngExporter.Export(model, fileStream);
                }
            }

        }
    }
}
