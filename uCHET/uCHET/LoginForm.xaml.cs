using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace uCHET
{
    /// <summary>
    /// Логика взаимодействия для LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        int countTries = 3;
        bool firstTry = true;

        DispatcherTimer _timer;
        TimeSpan _time;

        public LoginForm()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void btnIn_Click(object sender, RoutedEventArgs e)
        {
            var users = uchetPR419Entities.GetContext().users.ToList();
            string pass = txtPass.Text;
            string hash;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] soucreBytes = Encoding.UTF8.GetBytes(pass);
                byte[] hashBytes = sha256Hash.ComputeHash(soucreBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }

            user user = users.Where(p => p.login == txtLogin.Text && p.password.ToLower() == hash.ToLower()).FirstOrDefault();
            if (user != null)
            {
                DateTime date = DateTime.Now;
                if (!firstTry)
                {
                    if(txtCapcha.Text == txtReadyCapcha.Text)
                    {
                        new MainWindow(user, date).Show();
                        this.Close();
                    }
                    else
                    {
                        triedLost();
                        MessageBox.Show("Неправильно введена капча", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    }

                }
                else
                {
                    new MainWindow(user, date).Show();
                    this.Close();
                }

            }
            else
            {
                txtCapcha.Visibility = Visibility.Visible;
                txtReadyCapcha.Visibility = Visibility.Visible;
                CountTry.Visibility = Visibility.Visible;
                triedLost();
                MessageBox.Show("Неправильно введены данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtLogin.Text.Length > 0 && txtPass.Text.Length > 0 && firstTry)
            {
                btnIn.IsEnabled = true;
            }
            else if (txtLogin.Text.Length > 0 && txtPass.Text.Length > 0 && !firstTry && txtReadyCapcha.Text.Length == 10) 
            {
                btnIn.IsEnabled = true;
            }
            else
            {
                btnIn.IsEnabled = false;
            }
        }

        public string generateCapcha()
        {
            char[] letters = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();
            string captcha = "";
            Random random = new Random();
            for (int u = 0; u < 10; u++)
                captcha += letters[random.Next(letters.Length)];
            return captcha;
        }

        public void triedLost()
        {
            firstTry = false;
            txtLogin.Clear();
            txtPass.Clear();
            txtCapcha.Text = "";
            txtReadyCapcha.Clear();
            txtLogin.Focus();

            txtCapcha.Text = generateCapcha();
            countTries--;
            if (countTries == 0)
            {
                //timerblock
                countTries = 3;
                this.IsEnabled = false;
                setTimer();
            }
            else
            {
                CountTry.Content = $"Осталось попыток: {countTries}";
            }
        }

        public void setTimer()
        {
            _time = TimeSpan.FromSeconds(30);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                _time = _time.Add(TimeSpan.FromSeconds(-1));
                CountTry.Content = $"Попробуйте снова через: {_time.Seconds.ToString()}";
                if (_time == TimeSpan.Zero) 
                {
                    _timer.Stop();
                    this.IsEnabled = true;
                    CountTry.Content = $"Осталось попыток: {countTries}";
                }        
        
            }, Application.Current.Dispatcher);

            _timer.Start();
        }


    }
}
