using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using computerclub.Models;

namespace computerclub
{
    public partial class LoginWindow : Window
    {
        private readonly ComputerClubContext _db = new();

        public Employee? CurrentUser { get; private set; }
        public string? UserRole { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            TxtLogin.Focus();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(login))
            {
                TxtStatus.Text = "Введите логин!";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                TxtStatus.Text = "Введите пароль!";
                return;
            }

            try
            {
                var user = _db.Employees.FirstOrDefault(u => u.Login == login && u.IsActive);

                if (user == null)
                {
                    TxtStatus.Text = "Неверный логин или пользователь не активен!";
                    return;
                }

                string hashedPassword = HashPassword(password);

                if (user.PasswordHash != hashedPassword)
                {
                    TxtStatus.Text = "Неверный пароль!";
                    return;
                }

                CurrentUser = user;
                UserRole = user.Role;

                // Создаем главное окно
                var mainWindow = new MainWindow(CurrentUser, UserRole);

                // Подписываемся на событие загрузки главного окна
                mainWindow.ContentRendered += (s, args) =>
                {
                    // Когда главное окно полностью загружено, закрываем окно входа
                    this.Close();
                };

                // Показываем главное окно
                mainWindow.Show();

                TxtStatus.Text = "Вход выполнен успешно! Загрузка...";
                TxtStatus.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _db.Dispose();
        }
    }
}