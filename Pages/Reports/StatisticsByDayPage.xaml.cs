using DocumentFormat.OpenXml.Drawing;

using LiveCharts;
using LiveCharts.Wpf;
using ShopWebData;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

using Separator = LiveCharts.Wpf.Separator;

namespace ShopWebApp.Pages
{
    public partial class StatisticsByDayPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public StatisticsByDayPage()
        {
            InitializeComponent();            

            StartDate = DateTime.Now.Date.AddDays(-3);
            EndDate = DateTime.Now.Date.AddDays(1);

            PreRenderReport();
            GenerateReport();

            DataContext = this;
        }

        void PreRenderReport()
        {
            axisY.Separator = new Separator
            {
                Step = 1,
            };
            axisY.LabelFormatter = value => value.ToString();
        }

        void GenerateReport()
        {
            ShopWebContext Context = new();
            var AttendanceQuery =
                from statistics in Context.Statistics.AsQueryable()
                where statistics.DateTime >= StartDate
                where statistics.DateTime <= EndDate
                where statistics.Category == null // not including products page
                group statistics by statistics.DateTime.Date
                    into list
                select new
                {
                    Count = list.Count(),
                    DateTime = list.Max(e => e.DateTime)
                };

            var AttendanceList = AttendanceQuery.ToList();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Посещаемость",
                    Values = new ChartValues<int>(AttendanceList.Select(e => e.Count)),
                    LineSmoothness = 0,
                    Fill = Brushes.Red,
                    Stroke = Brushes.Black
                }
            };

            LabelsAxis.Labels = AttendanceList.Select(e => e.DateTime.Date.ToString("dd.MM")).ToArray();

            if(SeriesCollection != null)
            {
                Chart.Series = SeriesCollection;
            }
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDate = (sender as DatePicker).SelectedDate.Value;
            GenerateReport();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDate = (sender as DatePicker).SelectedDate.Value;
            GenerateReport();
        }
    }
}
