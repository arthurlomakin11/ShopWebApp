using LiveCharts;
using LiveCharts.Wpf;
using ShopWeb.Data;
using System;
using System.Linq;
using System.Windows.Controls;
using Separator = LiveCharts.Wpf.Separator;

namespace ShopWebApp.Pages
{
    public partial class StatisticsByHourPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }

        public DateTime Date { get; set; }

        public StatisticsByHourPage()
        {
            InitializeComponent();            

            Date = DateTime.Now.Date;

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
                where statistics.DateTime.Date == Date
                where statistics.Category == null // not including products page
                group statistics by statistics.DateTime.TimeOfDay.Hours
                    into list
                select new
                {
                    Count = list.Count(),
                    Time = list.Max(e => e.DateTime.TimeOfDay.Hours)
                };

            var AttendanceList = AttendanceQuery.ToList();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Посещаемость",
                    Values = new ChartValues<int>(AttendanceList.Select(e => e.Count)),
                    LineSmoothness = 0
                }
            };

            LabelsAxis.Labels = AttendanceList.Select(e => e.Time.ToString() + ":00").ToArray();

            if(SeriesCollection != null)
            {
                Chart.Series = SeriesCollection;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Date = (sender as DatePicker).SelectedDate.Value;
            GenerateReport();
        }
    }
}
