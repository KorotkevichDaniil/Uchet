using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using CsvHelper;
using Syncfusion.Pdf.Interactive;
using Syncfusion.UI.Xaml.Grid.Converter;

namespace uCHET
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        user user;
        DateTime dateAutorization;

        public MainWindow(user user, DateTime dateAutorization)
        {
            InitializeComponent();

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.user = user;
            this.dateAutorization = dateAutorization;

            var userplategi = uchetPR419Entities.GetContext().v_Plategi.Where(p => p.User_id == user.id).ToList();
            dataGrid.ItemsSource = userplategi;

            var categories = uchetPR419Entities.GetContext().categories.ToList();
            categories.Insert(0, new category
            {
                category_name = "Все категории"
            });
            cmbCategory.ItemsSource = categories;

            

        }

   
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

 
        private void dateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void dateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddForm addForm = new AddForm(user);
            bool result = (bool)addForm.ShowDialog();
            if (!result) Update(); 
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var remove = dataGrid.SelectedItems.Cast<v_Plategi>().ToList();

            if(remove.Count() == 0)
            {
                MessageBox.Show("Элементы для удаления не выбраны!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {

                if (MessageBox.Show($"Вы точно хотите удалить эти {remove.Count()} элементы?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        DateTime dateTime = DateTime.Today;
                        for (int i = 0; i < remove.Count; i++)
                            uchetPR419Entities.GetContext().products_users.RemoveRange(uchetPR419Entities.GetContext().products_users.ToList().Where(p => p.id == remove[i].id));
                        var analys = uchetPR419Entities.GetContext().analys.Where(p => p.date == dateTime && p.user_id == user.id).ToList();
                        if (analys.Count == 0)
                            uchetPR419Entities.GetContext().sp_AddAnalys(DateTime.Now, user.id);
                        uchetPR419Entities.GetContext().sp_AddToDeleted(dateTime, user.id, remove.Count);
                        uchetPR419Entities.GetContext().SaveChanges();
                        MessageBox.Show("Данные удалены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                        Update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }

        public void Update()
        {
            var plat = uchetPR419Entities.GetContext().v_Plategi.Where(p => p.User_id == user.id).ToList(); 
            if (cmbCategory.SelectedIndex > 0)
            {
                plat = plat.Where(p => p.Category == cmbCategory.SelectedValue.ToString()).ToList();
            }

            plat = plat.Where(p => p.Payment_name.ToLower().Contains(txtSearch.Text.ToLower())).ToList();

            if (dateFrom.SelectedDate <= dateTo.SelectedDate)
                plat = plat.Where(p => p.date >= dateFrom.SelectedDate && p.date <= dateTo.SelectedDate).ToList();
            else if (dateFrom.SelectedDate > dateTo.SelectedDate)
            {
                MessageBox.Show("Неправильно указан диапазон дат", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                dateFrom.SelectedDate = null;
                dateTo.SelectedDate = null;
            }
                
            else if (dateFrom.SelectedDate == null && dateTo.SelectedDate != null)
            {
                plat = plat.Where(p => p.date <= dateTo.SelectedDate).ToList();
            }
            else if (dateFrom.SelectedDate != null && dateTo.SelectedDate == null)
                plat = plat.Where(p => p.date >= dateFrom.SelectedDate).ToList();


            dataGrid.ItemsSource = plat.ToList();
            
        }

        private void btnOtchet_Click(object sender, RoutedEventArgs e)
        {
            
            var options = new ExcelExportingOptions();
            options.ExcelVersion = Syncfusion.XlsIO.ExcelVersion.Excel2016;
            
            var excelEngine = dataGrid.ExportToExcel(dataGrid.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];
            workBook.SaveAs("Sample.xlsx");
            MessageBox.Show("Отчет сформирован", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            dateFrom.SelectedDate = null;
            dateTo.SelectedDate= null;
            cmbCategory.SelectedIndex = 0;
            txtSearch.Clear();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            new AnalysForm(user).Show();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            List<string> headers = new List<string> { "Авторизация", "Выход", "Добавлено", "Удалено", "Всего затронуто" };
            var anal = uchetPR419Entities.GetContext().analys.Where(p => p.date == DateTime.Today && p.user_id == user.id).ToList();
            int add = anal.First().added;
            int del = anal.First().deleted; 
            int all = add + del;
            List<string> values = new List<string> { dateAutorization.ToString(), DateTime.Now.ToString(), add.ToString(), del.ToString(), all.ToString()};
            string path = "C:\\Users\\dshbo\\Documents\\Payments\\log"+DateTime.Now.ToLongDateString()+".csv";


            using (StreamWriter streamWriter = new StreamWriter(path, true, Encoding.Default, 10))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.GetCultureInfo("ru-RU")))
                {
                    csvWriter.WriteField(headers);
                    csvWriter.NextRecord();
                    csvWriter.WriteField(values);
                }
            }
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnRedact_Click(object sender, RoutedEventArgs e)
        {
            
            bool result = (bool)new AddForm(user, (sender as Button).DataContext as v_Plategi).ShowDialog();
            if (!result) Update();
        }
    }
}
