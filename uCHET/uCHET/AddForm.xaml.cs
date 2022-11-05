using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace uCHET
{
    /// <summary>
    /// Логика взаимодействия для AddForm.xaml
    /// </summary>
    public partial class AddForm : Window
    {
        private user user;
        List<category> categories;
        List<product> products;
        bool REDACTING_MODE;
        v_Plategi plategi;

        public AddForm(user user)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.user = user;
            categories = uchetPR419Entities.GetContext().categories.ToList();
            cmbCategory.ItemsSource = categories;
            cmbCategory.SelectedIndex = -1;
            cmbName.IsEnabled = false;
            numCount.IsEnabled = false;
            txtPrice.IsEnabled = false;
            REDACTING_MODE = false;
        }
        public AddForm(user user, v_Plategi plategi)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.user = user;
            categories = uchetPR419Entities.GetContext().categories.ToList();
            cmbCategory.ItemsSource = categories;
            this.plategi = plategi;
            cmbCategory.SelectedValue = plategi.Category;
            cmbName.SelectedValue = plategi.Payment_name;
            txtPrice.Text = plategi.price.ToString();
            numCount.Text = plategi.count.ToString();
            REDACTING_MODE = true;
        }
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbName.IsEnabled = true;
            products = uchetPR419Entities.GetContext().products.Where(p => p.category_id == cmbCategory.SelectedIndex + 1).ToList();

            if (cmbName.ItemsSource != null)
            {
                cmbName.SelectedIndex = -1;
                cmbName.ItemsSource = null;
            }
            
            cmbName.ItemsSource = products;
            
            
            numCount.IsEnabled = false;
            txtPrice.IsEnabled = false;
        }

        private void cmbName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            numCount.IsEnabled = true;
            txtPrice.IsEnabled = true;
            if (cmbName.SelectedIndex > 0)
            {
                var price = from p in products
                            where p.category_id == cmbCategory.SelectedIndex + 1
                            where p.product_name == cmbName.SelectedValue.ToString()
                            select p;
                txtPrice.Text = price.FirstOrDefault().price.ToString();
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(Convert.ToDecimal(txtPrice.Text) < 0)
            {
                MessageBox.Show("Неверно введена цена!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPrice.Clear();
            }
            else
            {
                DateTime dateTime = DateTime.Today;
                var userplategi = uchetPR419Entities.GetContext().v_Plategi.Where(p => p.User_id == user.id).ToList();

                string index = String.Join("-", cmbCategory.SelectedValue.ToString().Substring(0, 1), (userplategi.Count() + 1).ToString(), dateTime.ToString("yyyy-MM-dd"));
                if (REDACTING_MODE)
                {
                    uchetPR419Entities.GetContext().sp_UpdPayment(plategi.id, cmbName.Text, user.id, Convert.ToInt32(numCount.Text), Convert.ToDecimal(txtPrice.Text), index);
                    var analys = uchetPR419Entities.GetContext().analys.Where(p => p.date == dateTime && p.user_id == user.id).ToList();
                    if (analys.Count == 0)
                        uchetPR419Entities.GetContext().sp_AddAnalys(DateTime.Now, user.id);
                    uchetPR419Entities.GetContext().sp_AddToUpdated(dateTime, user.id, 1);
                }
                else
                {
                    uchetPR419Entities.GetContext().sp_AddPayment(cmbName.Text, user.id, Convert.ToInt32(numCount.Text), Convert.ToDecimal(txtPrice.Text), index);
                    var analys = uchetPR419Entities.GetContext().analys.Where(p => p.date == dateTime && p.user_id == user.id).ToList();
                    if (analys.Count == 0)
                        uchetPR419Entities.GetContext().sp_AddAnalys(DateTime.Now, user.id);
                    uchetPR419Entities.GetContext().sp_AddToAdded(dateTime, user.id, 1);
                }
               


                uchetPR419Entities.GetContext().SaveChanges();
                MessageBox.Show("Запись успешно сохранена!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
           
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
