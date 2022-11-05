using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace uCHET
{
    /// <summary>
    /// Логика взаимодействия для AnalysForm.xaml
    /// </summary>
    public partial class AnalysForm : Window
    {
        public AnalysForm(user user)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            var analys = uchetPR419Entities.GetContext().analys.Where(p => p.user_id == user.id).OrderBy(n => n.date).ToList();
            dataGrid.ItemsSource = analys;
            chartAnalysAdded.ChartAreas.Add(new ChartArea("Main"));
            chartAnalysUpdated.ChartAreas.Add(new ChartArea("Main"));
            chartAnalysDeleted.ChartAreas.Add(new ChartArea("Main"));
            var currentSeriesAdded = new Series("analys")
            {
                IsValueShownAsLabel = true
            };
            var currentSeriesUpdated = new Series("analys")
            {
                IsValueShownAsLabel = true
            };
            var currentSeriesDeleted = new Series("analys")
            {
                IsValueShownAsLabel = true
            };
            chartAnalysAdded.Series.Add(currentSeriesAdded);
            chartAnalysDeleted.Series.Add(currentSeriesDeleted);
            chartAnalysUpdated.Series.Add(currentSeriesUpdated);

            var res = uchetPR419Entities.GetContext().analys.ToList().Where(p => p.user_id == user.id).ToList();
            for (int i =0; i<res.Count; i++)
            {
                currentSeriesAdded.Points.AddXY(res[i].date, res[i].added);
                currentSeriesDeleted.Points.AddXY(res[i].date, res[i].deleted);
                currentSeriesUpdated.Points.AddXY(res[i].date, res[i].updated);
            }

        }

    }
}
