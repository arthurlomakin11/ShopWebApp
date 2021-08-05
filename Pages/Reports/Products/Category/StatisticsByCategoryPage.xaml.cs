using LiveCharts;
using LiveCharts.Wpf;
using ShopWebData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Separator = LiveCharts.Wpf.Separator;

namespace ShopWebApp.Pages
{

    public partial class StatisticsByCategoryPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }

        public DateTime Date { get; set; }

        public List<Label> Labels { get; set; }
        
        public StatisticsByCategoryPage()
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
            List<Category> Categories = Context.Categories.ToList();

            var AttendanceQuery =
                from statistics in Context.Statistics.AsQueryable()
                where statistics.DateTime.Date == Date
                where statistics.Category != null
                group statistics by new { statistics.CategoryId, statistics.DateTime.TimeOfDay.Hours }
                    into list
                select new
                {
                    Count = list.Count(),
                    list.Key.CategoryId,
                    Hour = list.Key.Hours
                };

            var AttendanceList = AttendanceQuery.ToList();

            SeriesCollection series = new();
            List<string> labels = new();

            foreach (var item in AttendanceList.GroupBy(i => i.CategoryId))
            {
                labels.AddRange(item.Select(c => c.Hour.ToString() + ":00"));
                series.Add(new LineSeries
                {
                    Title = Categories.Find(e => e.Id == item.Key).Name.ToString(),
                    Values = new ChartValues<int>(item.Select(e => e.Count)),
                    LineSmoothness = 0
                });                
            }

            SeriesCollection = series;

            LabelsAxis.Labels = labels.Distinct().ToArray();

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
