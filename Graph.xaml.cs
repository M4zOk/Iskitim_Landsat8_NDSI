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

            // Определение переменной model и инициализация
            double[] x = {36,32,40,17,32,21,39,15,17,42,12,14,25,24,35,24,32,28,30,33};
            double[] y = {50,47,58,28,51,40,52,16,32,55,18,27,44,31,43,25,43,31,38,38};
            
            model = new PlotModel { Title = "График корреляции" };
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

            var n = x.Length;
            var sumX = x.Sum();
            var sumY = y.Sum();
            var sumXY = x.Zip(y, (a, b) => a * b).Sum();
            var sumX2 = x.Select(xi => xi * xi).Sum();
            var sumY2 = y.Select(yi => yi * yi).Sum();

            var beta = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            var alpha = (sumY - beta * sumX) / n;

            double correlation = (n * sumXY - sumX * sumY) / Math.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY));

            double R = correlation ;

            var lineSeries = new LineSeries
            {
                Title = "Least Squares Regression",
                Color = OxyColors.Red,
                StrokeThickness = 1,
                MarkerType = MarkerType.None,
                ItemsSource = new[]
                 {
                   new DataPoint(x.Min(), alpha + beta * x.Min()),
                   new DataPoint(x.Max(), alpha + beta * x.Max())
                 }.ToList()
            };

            string formula = string.Format("y = {0:0.##}x + {1:0.##}", beta, alpha);
            lable1.Content = formula;

            string znac = string.Format("R² = {0:0.##}", R);
            lable2.Content = znac;

            model.Series.Add(lineSeries);

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
