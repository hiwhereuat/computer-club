using computerclub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace computerclub
{
    public class AnalyticsQuery
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SqlQuery { get; set; }
        public bool HasParameters { get; set; }
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
    }
    public partial class MainWindow : Window
    {
        private readonly ComputerClubContext _db = new();

        private Employee? _currentUser;
        private string? _userRole;

        private ObservableCollection<Client> _clients;
        private ICollectionView _clientView;

        private ObservableCollection<Computer> _computers;
        private ICollectionView _computerView;

        private ObservableCollection<Product> _products;
        private ICollectionView _productView;

        private ObservableCollection<Employee> _employees;
        private ICollectionView _employeeView;

        private ObservableCollection<Game> _games;
        private ICollectionView _gameView;

        private ObservableCollection<Tariff> _tariffs;
        private ICollectionView _tariffView;

        private ObservableCollection<Session> _sessions;
        private ICollectionView _sessionView;

        private ObservableCollection<Hall> _halls;
        private ICollectionView _hallView;

        private ObservableCollection<ClientRank> _ranks;
        private ICollectionView _rankView;

        private ObservableCollection<PaymentMethod> _paymentMethods;
        private ICollectionView _methodView;

        private ObservableCollection<ProductCategory> _categories;
        private ICollectionView _categoryView;

        private ObservableCollection<Position> _positions;
        private ICollectionView _positionView;

        private ObservableCollection<ComputerGame> _computerGames;
        private ICollectionView _computerGameView;

        private ObservableCollection<Order> _orders;
        private ICollectionView _orderView;

        private ObservableCollection<OrderDetail> _orderDetails;
        private ICollectionView _orderDetailView;

        public MainWindow(Employee currentUser, string userRole)
        {
            try
            {
                InitializeComponent();
                _currentUser = currentUser;
                _userRole = userRole;

                ApplyAccessRights();
                LoadAllData();
                SetupAllFilters();
                InitializeAnalyticsQueries();

                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMinutes(1);
                timer.Tick += (s, e) => CheckAndEndExpiredSessions();
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке главного окна: {ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }



        private void LoadAllData()
        {
            try
            {
                _db.ClientRanks.Load();
                _db.Halls.Load();
                _db.PaymentMethods.Load();
                _db.ProductCategories.Load();
                _db.Positions.Load();
                _db.Games.Load();
                _db.Computers.Load();
                _db.Clients.Load();
                _db.Employees.Load();
                _db.Tariffs.Load();
                _db.Sessions.Load();
                _db.Orders.Load();
                _db.OrderDetails.Load();
                _db.Products.Load();

                _db.Clients.Include(c => c.Rank).Load();
                _clients = _db.Clients.Local.ToObservableCollection();
                _clientView = CollectionViewSource.GetDefaultView(_clients);
                DgClients.ItemsSource = _clientView;
                CmbClientRank.ItemsSource = _db.ClientRanks.Local.ToObservableCollection();

                _db.Computers.Include(c => c.Hall).Load();
                _computers = _db.Computers.Local.ToObservableCollection();
                _computerView = CollectionViewSource.GetDefaultView(_computers);
                DgComputers.ItemsSource = _computerView;
                CmbComputerHall.ItemsSource = _db.Halls.Local.ToObservableCollection();

                _db.Products.Include(p => p.Category).Load();
                _products = _db.Products.Local.ToObservableCollection();
                _productView = CollectionViewSource.GetDefaultView(_products);
                DgProducts.ItemsSource = _productView;
                CmbProductCategory.ItemsSource = _db.ProductCategories.Local.ToObservableCollection();

                _db.Employees.Include(e => e.Position).Load();
                _employees = _db.Employees.Local.ToObservableCollection();
                _employeeView = CollectionViewSource.GetDefaultView(_employees);
                DgEmployees.ItemsSource = _employeeView;
                CmbEmployeePosition.ItemsSource = _db.Positions.Local.ToObservableCollection();

                RefreshEmployeeComboBoxes();

                _db.Games.Load();
                _games = _db.Games.Local.ToObservableCollection();
                _gameView = CollectionViewSource.GetDefaultView(_games);
                DgGames.ItemsSource = _gameView;

                _db.Tariffs.Include(t => t.Hall).Load();
                _tariffs = _db.Tariffs.Local.ToObservableCollection();
                _tariffView = CollectionViewSource.GetDefaultView(_tariffs);
                DgTariffs.ItemsSource = _tariffView;
                CmbTariffHall.ItemsSource = _db.Halls.Local.ToObservableCollection();

                _db.Sessions.Include(s => s.Client).Include(s => s.Computer)
                            .Include(s => s.Employee).Include(s => s.Tariff)
                            .Include(s => s.PaymentMethod).Load();
                _sessions = _db.Sessions.Local.ToObservableCollection();
                _sessionView = CollectionViewSource.GetDefaultView(_sessions);
                DgSessions.ItemsSource = _sessionView;

                CmbSessionClient.ItemsSource = _db.Clients.Local.ToObservableCollection();
                CmbSessionComputer.ItemsSource = _db.Computers.Local.ToObservableCollection();
                CmbSessionEmployee.ItemsSource = _db.Employees.Local.ToObservableCollection();
                CmbSessionTariff.ItemsSource = _db.Tariffs.Local.ToObservableCollection();
                CmbSessionPayment.ItemsSource = _db.PaymentMethods.Local.ToObservableCollection();

                _db.Halls.Load();
                _halls = _db.Halls.Local.ToObservableCollection();
                _hallView = CollectionViewSource.GetDefaultView(_halls);
                DgHalls.ItemsSource = _hallView;

                _db.ClientRanks.Load();
                _ranks = _db.ClientRanks.Local.ToObservableCollection();
                _rankView = CollectionViewSource.GetDefaultView(_ranks);
                DgRanks.ItemsSource = _rankView;

                _db.PaymentMethods.Load();
                _paymentMethods = _db.PaymentMethods.Local.ToObservableCollection();
                _methodView = CollectionViewSource.GetDefaultView(_paymentMethods);
                DgPaymentMethods.ItemsSource = _methodView;

                _db.ProductCategories.Load();
                _categories = _db.ProductCategories.Local.ToObservableCollection();
                _categoryView = CollectionViewSource.GetDefaultView(_categories);
                DgCategories.ItemsSource = _categoryView;

                _db.Positions.Load();
                _positions = _db.Positions.Local.ToObservableCollection();
                _positionView = CollectionViewSource.GetDefaultView(_positions);
                DgPositions.ItemsSource = _positionView;

                _db.ComputerGames.Include(cg => cg.Computer).Include(cg => cg.Game).Load();
                _computerGames = _db.ComputerGames.Local.ToObservableCollection();
                _computerGameView = CollectionViewSource.GetDefaultView(_computerGames);
                DgComputerGames.ItemsSource = _computerGameView;

                CmbComputerGameComputer.ItemsSource = _db.Computers.Local.ToObservableCollection();
                CmbComputerGameComputer.DisplayMemberPath = "ComputerName";
                CmbComputerGameComputer.SelectedValuePath = "ComputerId";

                CmbComputerGameGame.ItemsSource = _db.Games.Local.ToObservableCollection();
                CmbComputerGameGame.DisplayMemberPath = "Title";
                CmbComputerGameGame.SelectedValuePath = "GameId";

                var now = DateTime.Now;
                var clientsWithActiveSessions = _db.Clients
                    .Where(c => _db.Sessions.Any(s => s.ClientId == c.ClientId && s.EndTime > now))
                    .ToList();

                CmbOrderClient.ItemsSource = clientsWithActiveSessions;
                CmbOrderClient.DisplayMemberPath = "Nickname";
                CmbOrderClient.SelectedValuePath = "ClientId";
                CmbOrderClient.SelectedIndex = -1;

                CmbOrderEmployee.ItemsSource = _db.Employees.Local.ToObservableCollection();
                CmbOrderEmployee.DisplayMemberPath = "FullName";
                CmbOrderEmployee.SelectedValuePath = "EmployeeId";

                CmbOrderPayment.ItemsSource = _db.PaymentMethods.Local.ToObservableCollection();
                CmbOrderPayment.DisplayMemberPath = "MethodName";
                CmbOrderPayment.SelectedValuePath = "PaymentMethodId";

                CmbOrderComputer.ItemsSource = new List<Computer>();
                CmbOrderComputer.IsEnabled = false;
                CmbOrderSession.ItemsSource = new List<object>();

                RefreshSessionComboBox();

                _db.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order)
                        .ThenInclude(o => o.Session)
                            .ThenInclude(s => s.Client)
                    .Load();

                _orderDetails = _db.OrderDetails.Local.ToObservableCollection();

                foreach (var od in _orderDetails)
                {
                    od.Total = od.Quantity * od.PriceAtPurchase;
                }

                _orderDetailView = CollectionViewSource.GetDefaultView(_orderDetails);
                DgOrderDetails.ItemsSource = _orderDetailView;

                CmbOrderDetailProduct.ItemsSource = _db.Products.Local.ToObservableCollection();

                RefreshSessionComboBox();
                RefreshOrderComboBoxes();

                RefreshClientsData();
                RefreshComputersData();
                RefreshProductsData();
                RefreshEmployeesData();
                RefreshGamesData();
                RefreshTariffsData();
                RefreshSessionsData();
                RefreshHallsData();
                RefreshRanksData();
                RefreshPaymentMethodsData();
                RefreshCategoriesData();
                RefreshPositionsData();
                RefreshComputerGamesData();
                RefreshOrdersData();
                RefreshOrderDetailsData();

                MessageBox.Show("Все данные успешно загружены!", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Dispatcher.Invoke(() =>
                {
                    RefreshSessionComboBox();
                    RefreshOrderComboBoxes();
                    RefreshEmployeeComboBoxes();
                    RefreshAllComboBoxes();
                    RefreshOrderFormComboBoxes();
                    DebugOrderComboBoxes(); 
                });


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }


        }

        private void RefreshOrderFormComboBoxes()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RefreshOrderFormComboBoxes started");

                var now = DateTime.Now;
                var clientsWithActiveSessions = _db.Clients
                    .Where(c => _db.Sessions.Any(s => s.ClientId == c.ClientId && s.EndTime > now && s.TotalCost == 0))
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {clientsWithActiveSessions.Count} clients with active sessions");

                if (CmbOrderClient != null)
                {
                    CmbOrderClient.ItemsSource = null; 
                    CmbOrderClient.ItemsSource = clientsWithActiveSessions;
                    CmbOrderClient.DisplayMemberPath = "Nickname";
                    CmbOrderClient.SelectedValuePath = "ClientId";
                    CmbOrderClient.SelectedIndex = -1;
                    System.Diagnostics.Debug.WriteLine($"CmbOrderClient set with {clientsWithActiveSessions.Count} items");
                }

                var allowedEmployees = _db.Employees
                    .Where(e => e.Role == "Admin" || e.Role == "Employee")
                    .ToList();

                if (CmbOrderEmployee != null)
                {
                    CmbOrderEmployee.ItemsSource = null;
                    CmbOrderEmployee.ItemsSource = allowedEmployees;
                    CmbOrderEmployee.DisplayMemberPath = "FullName";
                    CmbOrderEmployee.SelectedValuePath = "EmployeeId";
                    CmbOrderEmployee.SelectedIndex = -1;
                    System.Diagnostics.Debug.WriteLine($"CmbOrderEmployee set with {allowedEmployees.Count} items");
                }

                if (CmbOrderPayment != null)
                {
                    var payments = _db.PaymentMethods.Local.ToObservableCollection();
                    CmbOrderPayment.ItemsSource = null;
                    CmbOrderPayment.ItemsSource = payments;
                    CmbOrderPayment.DisplayMemberPath = "MethodName";
                    CmbOrderPayment.SelectedValuePath = "PaymentMethodId";
                    CmbOrderPayment.SelectedIndex = -1;
                    System.Diagnostics.Debug.WriteLine($"CmbOrderPayment set with {payments.Count} items");
                }

                if (CmbOrderComputer != null)
                {
                    CmbOrderComputer.ItemsSource = new List<Computer>();
                    CmbOrderComputer.IsEnabled = false;
                    CmbOrderComputer.SelectedIndex = -1;
                }

                if (CmbOrderSession != null)
                {
                    CmbOrderSession.ItemsSource = new List<object>();
                    CmbOrderSession.SelectedIndex = -1;
                }

                System.Diagnostics.Debug.WriteLine("RefreshOrderFormComboBoxes completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в RefreshOrderFormComboBoxes: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        private void CmbOrderSession_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void RefreshAllComboBoxes()
        {
            var now = DateTime.Now;
            var activeSessions = _db.Sessions
                .Include(s => s.Client)
                .Include(s => s.Computer)
                .Where(s => s.EndTime > now && s.TotalCost == 0)
                .ToList();

            var sessionList = activeSessions.Select(s => new
            {
                s.SessionId,
                SessionInfo = $"{s.SessionId} | {s.Client?.Nickname ?? "Нет клиента"} | {s.Computer?.ComputerName ?? "Нет ПК"} | До {s.EndTime:HH:mm}"
            }).ToList();

            CmbOrderSession.ItemsSource = sessionList;
            CmbOrderSession.DisplayMemberPath = "SessionInfo";
            CmbOrderSession.SelectedValuePath = "SessionId";

            var orders = _db.Orders
                .Include(o => o.Session)
                    .ThenInclude(s => s.Client)
                .ToList();

            var orderList = orders.Select(o => new
            {
                o.OrderId,
                OrderInfo = $"#{o.OrderId} | {o.Session?.Client?.Nickname ?? "Нет клиента"} | {o.OrderTime:dd.MM.yyyy HH:mm}"
            }).ToList();

            CmbOrderDetailOrder.ItemsSource = orderList;
            CmbOrderDetailOrder.DisplayMemberPath = "OrderInfo";
            CmbOrderDetailOrder.SelectedValuePath = "OrderId";

            CmbOrderDetailProduct.ItemsSource = _db.Products.Local.ToObservableCollection();
            CmbOrderDetailProduct.DisplayMemberPath = "ProductName";
            CmbOrderDetailProduct.SelectedValuePath = "ProductId";
        }

        private void SetupAllFilters()
        {
            ClientFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Никнейм", "Nickname"},
                {"Телефон", "Phone"},
                {"Баланс", "Balance"},
                {"Ранг", "Rank.RankName"}
            });
            ClientFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Никнейм", "Text:Nickname"},
                {"Телефон", "Text:Phone"},
                {"Баланс", "Numeric:Balance"},
                {"Ранг", "Text:Rank.RankName"}
            });

            ComputerFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Название ПК", "ComputerName"},
                {"IP адрес", "Ipaddress"},
                {"Зал", "Hall.Name"},
                {"Доступен", "IsAvailable"}
            });
            ComputerFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Название ПК", "Text:ComputerName"},
                {"IP адрес", "Text:Ipaddress"},
                {"Зал", "Text:Hall.Name"},
                {"Доступен", "Bool:IsAvailable"}
            });

            ProductFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Название", "ProductName"},
                {"Категория", "Category.CategoryName"},
                {"Цена", "Price"},
                {"Остаток", "StockQuantity"}
            });
            ProductFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Название", "Text:ProductName"},
                {"Категория", "Text:Category.CategoryName"},
                {"Цена", "Numeric:Price"},
                {"Остаток", "Numeric:StockQuantity"}
            });

            EmployeeFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"ФИО", "FullName"},
                {"Телефон", "Phone"},
                {"Должность", "Position.Title"}
            });
            EmployeeFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"ФИО", "Text:FullName"},
                {"Телефон", "Text:Phone"},
                {"Должность", "Text:Position.Title"}
            });

            GameFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Название", "Title"},
                {"Разработчик", "Developer"}
            });
            GameFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Название", "Text:Title"},
                {"Разработчик", "Text:Developer"}
            });

            TariffFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Название", "TariffName"},
                {"Зал", "Hall.Name"},
                {"Длительность", "DurationMinutes"},
                {"Цена", "Price"}
            });
            TariffFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Название", "Text:TariffName"},
                {"Зал", "Text:Hall.Name"},
                {"Длительность", "Numeric:DurationMinutes"},
                {"Цена", "Numeric:Price"}
            });

            SessionFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Клиент", "Client.Nickname"},
                {"ПК", "Computer.ComputerName"},
                {"Сотрудник", "Employee.FullName"},
                {"Тариф", "Tariff.TariffName"},
                {"Стоимость", "TotalCost"}
            });
            SessionFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Клиент", "Text:Client.Nickname"},
                {"ПК", "Text:Computer.ComputerName"},
                {"Сотрудник", "Text:Employee.FullName"},
                {"Тариф", "Text:Tariff.TariffName"},
                {"Стоимость", "Numeric:TotalCost"},
                {"Дата начала", "Date:StartTime"},
                {"Дата окончания", "Date:EndTime"}
            });

            HallFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Название", "Name"},
                {"Описание", "Description"}
            });
            HallFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Название", "Text:Name"},
                {"Описание", "Text:Description"}
            });

            RankFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Название", "RankName"},
                {"Скидка", "DiscountPercent"}
            });
            RankFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Название", "Text:RankName"},
                {"Скидка", "Numeric:DiscountPercent"}
            });

            PaymentMethodFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Метод оплаты", "MethodName"}
            });
            PaymentMethodFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Метод оплаты", "Text:MethodName"}
            });

            CategoryFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Категория", "CategoryName"}
            });
            CategoryFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Категория", "Text:CategoryName"}
            });

            PositionFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Должность", "Title"}
            });
            PositionFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Должность", "Text:Title"}
            });

            ComputerGameFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Компьютер", "Computer.ComputerName"},
                {"Игра", "Game.Title"},
                {"Разработчик", "Game.Developer"}
            });
            ComputerGameFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Компьютер", "Text:Computer.ComputerName"},
                {"Игра", "Text:Game.Title"},
                {"Разработчик", "Text:Game.Developer"}
            });

            OrderDetailFilter.SetSearchColumns(new Dictionary<string, string>
            {
                {"Клиент", "Order.Session.Client.Nickname"},
                {"Товар", "Product.ProductName"},
                {"Количество", "Quantity"},
                {"Цена", "PriceAtPurchase"},
                {"Сумма", "Total"}
            });
            OrderDetailFilter.SetFilterColumns(new Dictionary<string, string>
            {
                {"Клиент", "Text:Order.Session.Client.Nickname"},
                {"Товар", "Text:Product.ProductName"},
                {"Количество", "Numeric:Quantity"},
                {"Цена", "Numeric:PriceAtPurchase"},
                {"Сумма", "Numeric:Total"}
            });

            if (OrderFilter != null)
            {
                OrderFilter.SetSearchColumns(new Dictionary<string, string>
    {
        {"Клиент", "Session.Client.Nickname"},
        {"Компьютер", "Session.Computer.ComputerName"},
        {"Сотрудник", "Employee.FullName"},
        {"Метод оплаты", "PaymentMethod.MethodName"},
        {"Время заказа", "OrderTime"}
    });
                OrderFilter.SetFilterColumns(new Dictionary<string, string>
    {
        {"Клиент", "Text:Session.Client.Nickname"},
        {"Компьютер", "Text:Session.Computer.ComputerName"},
        {"Сотрудник", "Text:Employee.FullName"},
        {"Метод оплаты", "Text:PaymentMethod.MethodName"},
        {"Время заказа", "Date:OrderTime"}
    });
            }
        }

        private void ApplyAccessRights()
        {
            if (_userRole == "Admin")
            {
                return;
            }

            if (_userRole == "Employee")
            {
                DisableEditForTabByHeader("Продукты");
                DisableEditForTabByHeader("Сотрудники");
                DisableEditForTabByHeader("Тарифы");
                DisableEditForTabByHeader("Залы");
                DisableEditForTabByHeader("Ранги");
                DisableEditForTabByHeader("Методы оплаты");
                DisableEditForTabByHeader("Категории");
                DisableEditForTabByHeader("Должности");
                DisableEditForTabByHeader("Игры");
                DisableEditForTabByHeader("Игры на ПК");
                return;
            }

            if (_userRole == "Technician")
            {
                foreach (TabItem tab in MainTabControl.Items)
                {
                    string header = tab.Header.ToString() ?? "";
                    if (!header.Contains("Компьютеры") && !header.Contains("Залы"))
                    {
                        tab.Visibility = Visibility.Collapsed;
                    }
                    if (tab.Header.ToString()?.Contains("Аналитика") == true)
                    {
                        tab.Visibility = Visibility.Collapsed;
                        break;
                    }
                }


                SelectTabByHeader("Компьютеры");
                return;
            }
        }

        private void DisableEditForTabByHeader(string tabHeader)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (TabItem tab in MainTabControl.Items)
                    {
                        if (tab.Header.ToString()?.Contains(tabHeader) == true)
                        {
                            var grid = tab.Content as Grid;
                            if (grid != null)
                            {
                                var border = FindVisualChild<Border>(grid);
                                if (border != null)
                                {
                                    var wrapPanel = border.Child as WrapPanel;
                                    if (wrapPanel != null)
                                    {
                                        var buttonsPanel = wrapPanel.Children.OfType<StackPanel>()
                                            .LastOrDefault(sp => sp.Children.OfType<Button>().Any());
                                        if (buttonsPanel != null)
                                        {
                                            buttonsPanel.Visibility = Visibility.Collapsed;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при блокировке вкладки {tabHeader}: {ex.Message}");
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;

                var descendant = FindVisualChild<T>(child);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }


        private object? GetPropertyValue(object obj, string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath) || obj == null) return null;

            var parts = propertyPath.Split('.');
            object? current = obj;

            foreach (var part in parts)
            {
                if (current == null) return null;
                var prop = current.GetType().GetProperty(part);
                if (prop == null) return null;
                current = prop.GetValue(current);
            }
            return current;
        }

        private void ApplyFilter<T>(ICollectionView view,
            string searchText, string searchColumn,
            string filterColumn, string filterText,
            decimal? filterFrom, decimal? filterTo,
            DateTime? filterDateFrom, DateTime? filterDateTo,
            bool? filterBoolValue,
            ObservableCollection<T> items) where T : class
        {
            if (view == null) return;

            view.Filter = item =>
            {
                if (item is not T entity) return false;

                bool searchOk = true;
                if (!string.IsNullOrWhiteSpace(searchText) && !string.IsNullOrWhiteSpace(searchColumn))
                {
                    var value = GetPropertyValue(entity, searchColumn);
                    searchOk = value?.ToString()?.ToLower().Contains(searchText.ToLower()) ?? false;
                }

                bool filterOk = true;
                if (!string.IsNullOrWhiteSpace(filterColumn))
                {
                    var filterType = filterColumn.Split(':')[0];
                    var filterPath = filterColumn.Split(':')[1];
                    var value = GetPropertyValue(entity, filterPath);

                    if (filterType == "Numeric" && value != null)
                    {
                        decimal numValue = Convert.ToDecimal(value);
                        if (filterFrom.HasValue) filterOk = numValue >= filterFrom.Value;
                        if (filterOk && filterTo.HasValue) filterOk = numValue <= filterTo.Value;
                    }
                    else if (filterType == "Text")
                    {
                        string? strValue = value?.ToString();
                        string? filterValue = filterText?.ToLower();
                        if (!string.IsNullOrWhiteSpace(filterValue))
                        {
                            filterOk = strValue?.ToLower().Contains(filterValue) ?? false;
                        }
                    }
                    else if (filterType == "Date" && value is DateTime dateValue)
                    {
                        if (filterDateFrom.HasValue) filterOk = dateValue.Date >= filterDateFrom.Value.Date;
                        if (filterOk && filterDateTo.HasValue) filterOk = dateValue.Date <= filterDateTo.Value.Date;
                    }
                    else if (filterType == "Bool" && value is bool boolValue)
                    {
                        if (filterBoolValue.HasValue) filterOk = boolValue == filterBoolValue.Value;
                    }
                }

                return searchOk && filterOk;
            };
            view.Refresh();

            UpdateRowNumbers(view, items);
        }

        private void UpdateRowNumbers<T>(ICollectionView view, ObservableCollection<T> items)
        {
            int rowNumber = 1;
            foreach (var item in items)
            {
                var prop = item?.GetType().GetProperty("RowNumber");
                if (prop != null && item != null)
                {
                    prop.SetValue(item, rowNumber++);
                }
            }
            view.Refresh();
        }


        private void ClientFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Client>(_clientView,
                ClientFilter.SearchText, ClientFilter.SearchColumn,
                ClientFilter.FilterColumn, ClientFilter.FilterText,
                ClientFilter.FilterFrom, ClientFilter.FilterTo,
                ClientFilter.FilterDateFrom, ClientFilter.FilterDateTo,
                ClientFilter.FilterBoolValue, _clients);
        }

        private void ComputerFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Computer>(_computerView,
                ComputerFilter.SearchText, ComputerFilter.SearchColumn,
                ComputerFilter.FilterColumn, ComputerFilter.FilterText,
                ComputerFilter.FilterFrom, ComputerFilter.FilterTo,
                ComputerFilter.FilterDateFrom, ComputerFilter.FilterDateTo,
                ComputerFilter.FilterBoolValue, _computers);
        }

        private void ProductFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Product>(_productView,
                ProductFilter.SearchText, ProductFilter.SearchColumn,
                ProductFilter.FilterColumn, ProductFilter.FilterText,
                ProductFilter.FilterFrom, ProductFilter.FilterTo,
                ProductFilter.FilterDateFrom, ProductFilter.FilterDateTo,
                ProductFilter.FilterBoolValue, _products);
        }

        private void EmployeeFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Employee>(_employeeView,
                EmployeeFilter.SearchText, EmployeeFilter.SearchColumn,
                EmployeeFilter.FilterColumn, EmployeeFilter.FilterText,
                EmployeeFilter.FilterFrom, EmployeeFilter.FilterTo,
                EmployeeFilter.FilterDateFrom, EmployeeFilter.FilterDateTo,
                EmployeeFilter.FilterBoolValue, _employees);
        }

        private void GameFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Game>(_gameView,
                GameFilter.SearchText, GameFilter.SearchColumn,
                GameFilter.FilterColumn, GameFilter.FilterText,
                GameFilter.FilterFrom, GameFilter.FilterTo,
                GameFilter.FilterDateFrom, GameFilter.FilterDateTo,
                GameFilter.FilterBoolValue, _games);
        }

        private void TariffFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Tariff>(_tariffView,
                TariffFilter.SearchText, TariffFilter.SearchColumn,
                TariffFilter.FilterColumn, TariffFilter.FilterText,
                TariffFilter.FilterFrom, TariffFilter.FilterTo,
                TariffFilter.FilterDateFrom, TariffFilter.FilterDateTo,
                TariffFilter.FilterBoolValue, _tariffs);
        }

        private void SessionFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Session>(_sessionView,
                SessionFilter.SearchText, SessionFilter.SearchColumn,
                SessionFilter.FilterColumn, SessionFilter.FilterText,
                SessionFilter.FilterFrom, SessionFilter.FilterTo,
                SessionFilter.FilterDateFrom, SessionFilter.FilterDateTo,
                SessionFilter.FilterBoolValue, _sessions);
        }

        private void HallFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Hall>(_hallView,
                HallFilter.SearchText, HallFilter.SearchColumn,
                HallFilter.FilterColumn, HallFilter.FilterText,
                HallFilter.FilterFrom, HallFilter.FilterTo,
                HallFilter.FilterDateFrom, HallFilter.FilterDateTo,
                HallFilter.FilterBoolValue, _halls);
        }

        private void RankFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<ClientRank>(_rankView,
                RankFilter.SearchText, RankFilter.SearchColumn,
                RankFilter.FilterColumn, RankFilter.FilterText,
                RankFilter.FilterFrom, RankFilter.FilterTo,
                RankFilter.FilterDateFrom, RankFilter.FilterDateTo,
                RankFilter.FilterBoolValue, _ranks);
        }

        private void PaymentMethodFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<PaymentMethod>(_methodView,
                PaymentMethodFilter.SearchText, PaymentMethodFilter.SearchColumn,
                PaymentMethodFilter.FilterColumn, PaymentMethodFilter.FilterText,
                PaymentMethodFilter.FilterFrom, PaymentMethodFilter.FilterTo,
                PaymentMethodFilter.FilterDateFrom, PaymentMethodFilter.FilterDateTo,
                PaymentMethodFilter.FilterBoolValue, _paymentMethods);
        }

        private void CategoryFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<ProductCategory>(_categoryView,
                CategoryFilter.SearchText, CategoryFilter.SearchColumn,
                CategoryFilter.FilterColumn, CategoryFilter.FilterText,
                CategoryFilter.FilterFrom, CategoryFilter.FilterTo,
                CategoryFilter.FilterDateFrom, CategoryFilter.FilterDateTo,
                CategoryFilter.FilterBoolValue, _categories);
        }

        private void PositionFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Position>(_positionView,
                PositionFilter.SearchText, PositionFilter.SearchColumn,
                PositionFilter.FilterColumn, PositionFilter.FilterText,
                PositionFilter.FilterFrom, PositionFilter.FilterTo,
                PositionFilter.FilterDateFrom, PositionFilter.FilterDateTo,
                PositionFilter.FilterBoolValue, _positions);
        }

        private void ComputerGameFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<ComputerGame>(_computerGameView,
                ComputerGameFilter.SearchText, ComputerGameFilter.SearchColumn,
                ComputerGameFilter.FilterColumn, ComputerGameFilter.FilterText,
                ComputerGameFilter.FilterFrom, ComputerGameFilter.FilterTo,
                ComputerGameFilter.FilterDateFrom, ComputerGameFilter.FilterDateTo,
                ComputerGameFilter.FilterBoolValue, _computerGames);
        }

        private void OrderFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<Order>(_orderView,
                OrderFilter.SearchText, OrderFilter.SearchColumn,
                OrderFilter.FilterColumn, OrderFilter.FilterText,
                OrderFilter.FilterFrom, OrderFilter.FilterTo,
                OrderFilter.FilterDateFrom, OrderFilter.FilterDateTo,
                OrderFilter.FilterBoolValue,
                _orders);
        }

        private void OrderDetailFilter_FiltersChanged(object sender, EventArgs e)
        {
            ApplyFilter<OrderDetail>(_orderDetailView,
                OrderDetailFilter.SearchText, OrderDetailFilter.SearchColumn,
                OrderDetailFilter.FilterColumn, OrderDetailFilter.FilterText,
                OrderDetailFilter.FilterFrom, OrderDetailFilter.FilterTo,
                OrderDetailFilter.FilterDateFrom, OrderDetailFilter.FilterDateTo,
                OrderDetailFilter.FilterBoolValue,
                _orderDetails);
        }


        private void BtnClientAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление клиентов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpClientNick.Text))
            {
                MessageBox.Show("Введите никнейм!");
                return;
            }

            if (CmbClientRank.SelectedValue == null)
            {
                MessageBox.Show("Выберите ранг!");
                return;
            }

            string phone = string.IsNullOrWhiteSpace(InpClientPhone.Text) ? null : InpClientPhone.Text.Trim();

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var existingClientByPhone = _db.Clients.FirstOrDefault(c => c.Phone == phone);
                if (existingClientByPhone != null)
                {
                    MessageBox.Show($"Клиент с номером телефона '{phone}' уже существует!\n\n" +
                        $"Существующий клиент: {existingClientByPhone.Nickname}\n" +
                        $"Пожалуйста, используйте другой номер телефона.",
                        "Дубликат телефона", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (_db.Clients.Any(c => c.Nickname == InpClientNick.Text.Trim()))
            {
                MessageBox.Show($"Клиент с никнеймом '{InpClientNick.Text.Trim()}' уже существует!\n\n" +
                    "Пожалуйста, используйте другой никнейм.",
                    "Дубликат никнейма", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var client = new Client
            {
                Nickname = InpClientNick.Text.Trim(),
                Phone = phone,
                Balance = decimal.TryParse(InpClientBalance.Text, out var b) ? b : 0,
                RankId = (int)CmbClientRank.SelectedValue
            };

            try
            {
                _db.Clients.Add(client);
                _db.SaveChanges();
                RefreshClientsData();
                ClearClientForm();
                MessageBox.Show("Клиент добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.Entry(client).State = EntityState.Detached;

                if (ex.InnerException?.Message.Contains("UQ_Clients_Phone") == true ||
                    ex.InnerException?.Message.Contains("UNIQUE") == true ||
                    ex.InnerException?.Message.Contains("Phone") == true)
                {
                    MessageBox.Show("Ошибка: Клиент с таким номером телефона уже существует!",
                        "Дубликат телефона", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.InnerException?.Message.Contains("UQ_Clients_Nickname") == true ||
                         ex.InnerException?.Message.Contains("Nickname") == true)
                {
                    MessageBox.Show("Ошибка: Клиент с таким никнеймом уже существует!",
                        "Дубликат никнейма", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _db.Entry(client).State = EntityState.Detached;
                HandleError(ex, "добавлении клиента");
            }
        }

        private void BtnClientEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование клиентов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgClients.SelectedItem is not Client client)
            {
                MessageBox.Show("Выберите клиента!");
                return;
            }

            string oldPhone = client.Phone;
            string oldNickname = client.Nickname;
            string newPhone = string.IsNullOrWhiteSpace(InpClientPhone.Text) ? null : InpClientPhone.Text.Trim();
            string newNickname = InpClientNick.Text.Trim();

            if (!string.IsNullOrWhiteSpace(newPhone) && newPhone != oldPhone)
            {
                var existingClientByPhone = _db.Clients.FirstOrDefault(c => c.Phone == newPhone && c.ClientId != client.ClientId);
                if (existingClientByPhone != null)
                {
                    MessageBox.Show($"Клиент с номером телефона '{newPhone}' уже существует!\n\n" +
                        $"Существующий клиент: {existingClientByPhone.Nickname}\n" +
                        $"Пожалуйста, используйте другой номер телефона.",
                        "Дубликат телефона", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (newNickname != oldNickname && _db.Clients.Any(c => c.Nickname == newNickname && c.ClientId != client.ClientId))
            {
                MessageBox.Show($"Клиент с никнеймом '{newNickname}' уже существует!\n\n" +
                    "Пожалуйста, используйте другой никнейм.",
                    "Дубликат никнейма", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            client.Nickname = newNickname;
            client.Phone = newPhone;
            client.Balance = decimal.TryParse(InpClientBalance.Text, out var b) ? b : 0;
            client.RankId = (int)CmbClientRank.SelectedValue;

            try
            {
                _db.SaveChanges();
                RefreshClientsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                client.Phone = oldPhone;
                client.Nickname = oldNickname;
                _db.Entry(client).Reload();

                if (ex.InnerException?.Message.Contains("UQ_Clients_Phone") == true ||
                    ex.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    MessageBox.Show("Ошибка: Клиент с таким номером телефона уже существует!",
                        "Дубликат телефона", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.InnerException?.Message.Contains("UQ_Clients_Nickname") == true)
                {
                    MessageBox.Show("Ошибка: Клиент с таким никнеймом уже существует!",
                        "Дубликат никнейма", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Ошибка при обновлении: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении клиента");
            }
        }

        private void BtnClientDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление клиентов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgClients.SelectedItem is not Client client)
            {
                MessageBox.Show("Выберите клиента!");
                return;
            }

            if (MessageBox.Show($"Удалить {client.Nickname}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Clients.Remove(client);
                _db.SaveChanges();
                RefreshClientsData();
                ClearClientForm();
                MessageBox.Show("Клиент удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshClientsData();

                if (ex.InnerException?.Message.Contains("REFERENCE") == true ||
                    ex.InnerException?.Message.Contains("FK") == true)
                {
                    MessageBox.Show("Невозможно удалить клиента!\n\n" +
                        "У него есть активные сессии или заказы.\n" +
                        "Сначала удалите все связанные сессии и заказы.",
                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshClientsData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgClients.SelectedItem is Client client)
            {
                InpClientNick.Text = client.Nickname;
                InpClientPhone.Text = client.Phone;
                InpClientBalance.Text = client.Balance.ToString("F2");
                CmbClientRank.SelectedValue = client.RankId;
            }
        }

        private void ClearClientForm()
        {
            InpClientNick.Clear();
            InpClientPhone.Clear();
            InpClientBalance.Clear();
            CmbClientRank.SelectedIndex = -1;
        }

        private void RefreshClientsData()
        {
            _db.Clients.Include(c => c.Rank).Load();
            _clients = _db.Clients.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var client in _clients)
            {
                client.RowNumber = rowNumber++;
            }

            _clientView = CollectionViewSource.GetDefaultView(_clients);
            DgClients.ItemsSource = _clientView;
            _clientView.Refresh();
        }


        private void BtnComputerAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Technician")
            {
                MessageBox.Show("У вас нет прав на добавление компьютеров!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpComputerName.Text)) { MessageBox.Show("Введите имя компьютера!"); return; }
            if (CmbComputerHall.SelectedValue == null) { MessageBox.Show("Выберите зал!"); return; }

            var computer = new Computer
            {
                ComputerName = InpComputerName.Text.Trim(),
                Ipaddress = string.IsNullOrWhiteSpace(InpComputerIP.Text) ? "0.0.0.0" : InpComputerIP.Text,
                HallId = (int)CmbComputerHall.SelectedValue,
                IsAvailable = ChkComputerAvailable.IsChecked ?? true
            };

            try
            {
                _db.Computers.Add(computer);
                _db.SaveChanges();
                RefreshComputersData();
                ClearComputerForm();
                MessageBox.Show("Компьютер добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(computer).State = EntityState.Detached;
                HandleError(ex, "добавлении компьютера");
            }
        }

        private void BtnComputerEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Technician")
            {
                MessageBox.Show("У вас нет прав на редактирование компьютеров!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgComputers.SelectedItem is not Computer computer)
            {
                MessageBox.Show("Выберите компьютер!");
                return;
            }

            var activeSession = _db.Sessions.FirstOrDefault(s => s.ComputerId == computer.ComputerId && s.EndTime > DateTime.Now);

            if (activeSession != null && ChkComputerAvailable.IsChecked != computer.IsAvailable)
            {
                MessageBox.Show("Нельзя изменить статус доступности компьютера,\n" +
                    "так как на нем сейчас идет активная сессия!\n" +
                    $"Сессия началась: {activeSession.StartTime:HH:mm}\n" +
                    $"Окончится: {activeSession.EndTime:HH:mm}",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);

                ChkComputerAvailable.IsChecked = computer.IsAvailable;
                return;
            }

            computer.ComputerName = InpComputerName.Text.Trim();
            computer.Ipaddress = InpComputerIP.Text;
            computer.HallId = (int)CmbComputerHall.SelectedValue;
            computer.IsAvailable = ChkComputerAvailable.IsChecked ?? true;

            try
            {
                _db.SaveChanges();
                RefreshComputersData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении компьютера");
            }
        }

        private void BtnComputerDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Technician")
            {
                MessageBox.Show("У вас нет прав на удаление компьютеров!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgComputers.SelectedItem is not Computer computer)
            {
                MessageBox.Show("Выберите компьютер!");
                return;
            }

            var activeSession = _db.Sessions.FirstOrDefault(s => s.ComputerId == computer.ComputerId && (s.EndTime == null || s.EndTime > DateTime.Now));

            if (activeSession != null)
            {
                MessageBox.Show($"Нельзя удалить компьютер, так как на нем сейчас идет активная сессия!\n" +
                    $"Клиент: {activeSession.Client?.Nickname}\n" +
                    $"Начало: {activeSession.StartTime:HH:mm}",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить {computer.ComputerName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Computers.Remove(computer);
                _db.SaveChanges();
                RefreshComputersData();
                ClearComputerForm();
                MessageBox.Show("Компьютер удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshComputersData();

                if (ex.InnerException?.Message.Contains("REFERENCE") == true ||
                    ex.InnerException?.Message.Contains("FK") == true)
                {
                    MessageBox.Show("Невозможно удалить компьютер!\n\n" +
                        "У него есть связанные сессии или установленные игры.\n" +
                        "Сначала удалите все связанные данные.",
                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshComputersData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgComputers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgComputers.SelectedItem is Computer computer)
            {
                InpComputerName.Text = computer.ComputerName;
                InpComputerIP.Text = computer.Ipaddress;
                CmbComputerHall.SelectedValue = computer.HallId;
                ChkComputerAvailable.IsChecked = computer.IsAvailable;
            }
        }

        private void ClearComputerForm()
        {
            InpComputerName.Clear();
            InpComputerIP.Clear();
            CmbComputerHall.SelectedIndex = -1;
            ChkComputerAvailable.IsChecked = true;
        }

        private void RefreshComputersData()
        {
            _db.Computers.Include(c => c.Hall).Load();
            _computers = _db.Computers.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var computer in _computers)
            {
                computer.RowNumber = rowNumber++;
            }

            _computerView = CollectionViewSource.GetDefaultView(_computers);
            DgComputers.ItemsSource = _computerView;
            _computerView.Refresh();

            var availableComputers = _db.Computers.Local.Where(c => c.IsAvailable).ToList();
            CmbSessionComputer.ItemsSource = availableComputers;
            CmbSessionComputer.DisplayMemberPath = "ComputerName";
            CmbSessionComputer.SelectedValuePath = "ComputerId";
        }

        private void BtnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление продуктов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpProductName.Text)) { MessageBox.Show("Введите название продукта!"); return; }
            if (CmbProductCategory.SelectedValue == null) { MessageBox.Show("Выберите категорию!"); return; }

            var product = new Product
            {
                ProductName = InpProductName.Text.Trim(),
                CategoryId = (int)CmbProductCategory.SelectedValue,
                Price = decimal.TryParse(InpProductPrice.Text, out var price) ? price : 0,
                StockQuantity = int.TryParse(InpProductStock.Text, out var qty) ? qty : 0
            };

            try
            {
                _db.Products.Add(product);
                _db.SaveChanges();
                RefreshProductsData();
                ClearProductForm();
                MessageBox.Show("Продукт добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(product).State = EntityState.Detached;
                HandleError(ex, "добавлении продукта");
            }
        }

        private void BtnProductEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование продуктов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgProducts.SelectedItem is not Product product) { MessageBox.Show("Выберите продукт!"); return; }

            product.ProductName = InpProductName.Text.Trim();
            product.CategoryId = (int)CmbProductCategory.SelectedValue;
            product.Price = decimal.TryParse(InpProductPrice.Text, out var price) ? price : 0;
            product.StockQuantity = int.TryParse(InpProductStock.Text, out var qty) ? qty : 0;

            try
            {
                _db.SaveChanges();
                RefreshProductsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении продукта");
            }
        }

        private void BtnProductDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление продуктов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgProducts.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите продукт!");
                return;
            }

            if (MessageBox.Show($"Удалить {product.ProductName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
                RefreshProductsData();
                ClearProductForm();
                MessageBox.Show("Продукт удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshProductsData();

                if (ex.InnerException?.Message.Contains("REFERENCE") == true ||
                    ex.InnerException?.Message.Contains("FK") == true)
                {
                    MessageBox.Show("Невозможно удалить продукт!\n\n" +
                        "Он есть в заказах.\n" +
                        "Сначала удалите все заказы с этим продуктом.",
                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshProductsData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgProducts.SelectedItem is Product product)
            {
                InpProductName.Text = product.ProductName;
                CmbProductCategory.SelectedValue = product.CategoryId;
                InpProductPrice.Text = product.Price.ToString("F2");
                InpProductStock.Text = product.StockQuantity.ToString();
            }
        }

        private void ClearProductForm()
        {
            InpProductName.Clear();
            InpProductPrice.Clear();
            InpProductStock.Clear();
            CmbProductCategory.SelectedIndex = -1;
        }

        private void RefreshProductsData()
        {
            _db.Products.Include(p => p.Category).Load();
            _products = _db.Products.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var product in _products)
            {
                product.RowNumber = rowNumber++;
            }

            _productView = CollectionViewSource.GetDefaultView(_products);
            DgProducts.ItemsSource = _productView;
            _productView.Refresh();
        }

        private void BtnEmployeeAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление сотрудников!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpEmployeeName.Text)) { MessageBox.Show("Введите ФИО!"); return; }
            if (CmbEmployeePosition.SelectedValue == null) { MessageBox.Show("Выберите должность!"); return; }

            var employee = new Employee
            {
                FullName = InpEmployeeName.Text.Trim(),
                Phone = InpEmployeePhone.Text.Trim(),
                PositionId = (int)CmbEmployeePosition.SelectedValue
            };

            try
            {
                _db.Employees.Add(employee);
                _db.SaveChanges();
                RefreshEmployeesData();
                ClearEmployeeForm();
                MessageBox.Show("Сотрудник добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(employee).State = EntityState.Detached;
                HandleError(ex, "добавлении сотрудника");
            }
        }

        private void BtnEmployeeEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование сотрудников!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgEmployees.SelectedItem is not Employee employee) { MessageBox.Show("Выберите сотрудника!"); return; }

            employee.FullName = InpEmployeeName.Text.Trim();
            employee.Phone = InpEmployeePhone.Text.Trim();
            employee.PositionId = (int)CmbEmployeePosition.SelectedValue;

            try
            {
                _db.SaveChanges();
                RefreshEmployeesData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении сотрудника");
            }
        }

        private void BtnEmployeeDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление сотрудников!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgEmployees.SelectedItem is not Employee employee)
            {
                MessageBox.Show("Выберите сотрудника!");
                return;
            }

            if (MessageBox.Show($"Удалить {employee.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Employees.Remove(employee);
                _db.SaveChanges();
                RefreshEmployeesData();
                ClearEmployeeForm();
                MessageBox.Show("Сотрудник удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshEmployeesData();

                if (ex.InnerException?.Message.Contains("REFERENCE") == true ||
                    ex.InnerException?.Message.Contains("FK") == true)
                {
                    MessageBox.Show("Невозможно удалить сотрудника!\n\n" +
                        "У него есть связанные сессии или заказы.\n" +
                        "Сначала удалите все связанные данные.",
                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshEmployeesData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgEmployees.SelectedItem is Employee employee)
            {
                InpEmployeeName.Text = employee.FullName;
                InpEmployeePhone.Text = employee.Phone;
                CmbEmployeePosition.SelectedValue = employee.PositionId;
            }
        }

        private void ClearEmployeeForm()
        {
            InpEmployeeName.Clear();
            InpEmployeePhone.Clear();
            CmbEmployeePosition.SelectedIndex = -1;
        }

        private void RefreshEmployeesData()
        {
            _db.Employees.Include(e => e.Position).Load();
            _employees = _db.Employees.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var employee in _employees)
            {
                employee.RowNumber = rowNumber++;
            }

            _employeeView = CollectionViewSource.GetDefaultView(_employees);
            DgEmployees.ItemsSource = _employeeView;
            _employeeView.Refresh();
        }


        private void BtnGameAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление игр!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpGameTitle.Text)) { MessageBox.Show("Введите название игры!"); return; }

            var game = new Game
            {
                Title = InpGameTitle.Text.Trim(),
                Developer = InpGameDeveloper.Text.Trim()
            };

            try
            {
                _db.Games.Add(game);
                _db.SaveChanges();
                RefreshGamesData();
                ClearGameForm();
                MessageBox.Show("Игра добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(game).State = EntityState.Detached;
                HandleError(ex, "добавлении игры");
            }
        }

        private void BtnGameEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование игр!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgGames.SelectedItem is not Game game) { MessageBox.Show("Выберите игру!"); return; }

            game.Title = InpGameTitle.Text.Trim();
            game.Developer = InpGameDeveloper.Text.Trim();

            try
            {
                _db.SaveChanges();
                RefreshGamesData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении игры");
            }
        }

        private void BtnGameDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление игр!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgGames.SelectedItem is not Game game)
            {
                MessageBox.Show("Выберите игру!");
                return;
            }

            if (MessageBox.Show($"Удалить {game.Title}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Games.Remove(game);
                _db.SaveChanges();
                _games.Remove(game);
                RefreshGamesData();
                ClearGameForm();
                MessageBox.Show("Игра удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(game).State = EntityState.Detached;
                _db.ChangeTracker.Clear();
                RefreshGamesData();
                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}\n\n" +
                    "Возможно, игра установлена на некоторых компьютерах.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgGames.SelectedItem is Game game)
            {
                InpGameTitle.Text = game.Title;
                InpGameDeveloper.Text = game.Developer;
            }
        }

        private void ClearGameForm()
        {
            InpGameTitle.Clear();
            InpGameDeveloper.Clear();
        }

        private void RefreshGamesData()
        {
            _db.Games.Load();
            _games = _db.Games.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var game in _games)
            {
                game.RowNumber = rowNumber++;
            }

            _gameView = CollectionViewSource.GetDefaultView(_games);
            DgGames.ItemsSource = _gameView;
            _gameView.Refresh();
        }

        private void BtnTariffAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление тарифов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpTariffName.Text)) { MessageBox.Show("Введите название тарифа!"); return; }
            if (CmbTariffHall.SelectedValue == null) { MessageBox.Show("Выберите зал!"); return; }

            var tariff = new Tariff
            {
                TariffName = InpTariffName.Text.Trim(),
                HallId = (int)CmbTariffHall.SelectedValue,
                DurationMinutes = int.TryParse(InpTariffDuration.Text, out var duration) ? duration : 60,
                Price = decimal.TryParse(InpTariffPrice.Text, out var price) ? price : 0
            };

            try
            {
                _db.Tariffs.Add(tariff);
                _db.SaveChanges();
                RefreshTariffsData();
                ClearTariffForm();
                MessageBox.Show("Тариф добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(tariff).State = EntityState.Detached;
                HandleError(ex, "добавлении тарифа");
            }
        }

        private void BtnTariffEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование тарифов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgTariffs.SelectedItem is not Tariff tariff) { MessageBox.Show("Выберите тариф!"); return; }

            tariff.TariffName = InpTariffName.Text.Trim();
            tariff.HallId = (int)CmbTariffHall.SelectedValue;
            tariff.DurationMinutes = int.TryParse(InpTariffDuration.Text, out var duration) ? duration : 60;
            tariff.Price = decimal.TryParse(InpTariffPrice.Text, out var price) ? price : 0;

            try
            {
                _db.SaveChanges();
                RefreshTariffsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении тарифа");
            }
        }

        private void BtnTariffDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление тарифов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgTariffs.SelectedItem is not Tariff tariff)
            {
                MessageBox.Show("Выберите тариф!");
                return;
            }

            if (MessageBox.Show($"Удалить {tariff.TariffName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Tariffs.Remove(tariff);
                _db.SaveChanges();
                _tariffs.Remove(tariff);
                RefreshTariffsData();
                ClearTariffForm();
                MessageBox.Show("Тариф удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(tariff).State = EntityState.Detached;
                _db.ChangeTracker.Clear();
                RefreshTariffsData();
                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}\n\n" +
                    "Возможно, тариф используется в сессиях.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgTariffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgTariffs.SelectedItem is Tariff tariff)
            {
                InpTariffName.Text = tariff.TariffName;
                CmbTariffHall.SelectedValue = tariff.HallId;
                InpTariffDuration.Text = tariff.DurationMinutes.ToString();
                InpTariffPrice.Text = tariff.Price.ToString("F2");
            }
        }

        private void ClearTariffForm()
        {
            InpTariffName.Clear();
            InpTariffDuration.Clear();
            InpTariffPrice.Clear();
            CmbTariffHall.SelectedIndex = -1;
        }

        private void RefreshTariffsData()
        {
            _db.Tariffs.Include(t => t.Hall).Load();
            _tariffs = _db.Tariffs.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var tariff in _tariffs)
            {
                tariff.RowNumber = rowNumber++;
            }

            _tariffView = CollectionViewSource.GetDefaultView(_tariffs);
            DgTariffs.ItemsSource = _tariffView;
            _tariffView.Refresh();
        }


        private void BtnSessionAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Employee")
            {
                MessageBox.Show("У вас нет прав на начало сессий!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbSessionClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbSessionComputer.SelectedValue == null)
            {
                MessageBox.Show("Выберите компьютер!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbSessionEmployee.SelectedValue == null)
            {
                MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbSessionTariff.SelectedValue == null)
            {
                MessageBox.Show("Выберите тариф!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbSessionPayment.SelectedValue == null)
            {
                MessageBox.Show("Выберите метод оплаты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var client = _db.Clients.Find((int)CmbSessionClient.SelectedValue);
                var computer = _db.Computers.Find((int)CmbSessionComputer.SelectedValue);
                var tariff = _db.Tariffs.Find((int)CmbSessionTariff.SelectedValue);

                if (client == null)
                {
                    MessageBox.Show("Клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (computer == null)
                {
                    MessageBox.Show("Компьютер не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (tariff == null)
                {
                    MessageBox.Show("Тариф не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (client.Balance < tariff.Price)
                {
                    MessageBox.Show($"Недостаточно средств для начала сессии!\n\n" +
                        $"Клиент: {client.Nickname}\n" +
                        $"Текущий баланс: {client.Balance:F2} руб.\n" +
                        $"Стоимость тарифа: {tariff.Price:F2} руб.\n" +
                        $"Требуется пополнить: {tariff.Price - client.Balance:F2} руб.\n\n" +
                        $"Пополните баланс клиента в разделе \"Клиенты\".",
                        "Недостаточно средств", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!computer.IsAvailable)
                {
                    MessageBox.Show("Этот компьютер уже занят! Выберите другой.",
                        "Компьютер занят", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (computer.HallId != tariff.HallId)
                {
                    var hallName = _db.Halls.Find(computer.HallId)?.Name ?? "неизвестном зале";
                    MessageBox.Show($"Тариф \"{tariff.TariffName}\" не доступен для компьютера \"{computer.ComputerName}\"!\n\n" +
                        $"Этот компьютер находится в зале \"{hallName}\".\n" +
                        $"Выберите тариф, доступный для этого зала.",
                        "Неверный тариф", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime endTime = DateTime.Now.AddMinutes(tariff.DurationMinutes);

                var session = new Session
                {
                    ClientId = client.ClientId,
                    ComputerId = computer.ComputerId,
                    EmployeeId = (int)CmbSessionEmployee.SelectedValue,
                    TariffId = tariff.TariffId,
                    PaymentMethodId = (int)CmbSessionPayment.SelectedValue,
                    StartTime = DateTime.Now,
                    EndTime = endTime,
                    TotalCost = 0 
                };

                _db.Sessions.Add(session);
                _db.SaveChanges();

                computer.IsAvailable = false;
                _db.SaveChanges();

                RefreshSessionsData();
                RefreshComputersData();
                ClearSessionForm();
                RefreshOrderFormComboBoxes();
                RefreshSessionComboBox();
                RefreshOrderComboBoxes();

                MessageBox.Show($"Сессия успешно начата!\n\n" +
                    $"Клиент: {client.Nickname}\n" +
                    $"Компьютер: {computer.ComputerName}\n" +
                    $"Тариф: {tariff.TariffName}\n" +
                    $"Начало: {session.StartTime:HH:mm}\n" +
                    $"Окончание: {session.EndTime:HH:mm}\n" +
                    $"Баланс клиента: {client.Balance:F2} руб.\n\n" +
                    $"Оплата будет произведена при завершении сессии.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshSessionsData();
                RefreshComputersData();

                MessageBox.Show($"Ошибка при создании сессии: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshSessionsData();
                RefreshComputersData();

                MessageBox.Show($"Ошибка при начале сессии:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSessionEnd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Employee")
            {
                MessageBox.Show("У вас нет прав на завершение сессий!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgSessions.SelectedItem is not Session session)
            {
                MessageBox.Show("Выберите сессию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (session.EndTime <= DateTime.Now && session.TotalCost > 0)
            {
                MessageBox.Show("Эта сессия уже завершена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var tariff = _db.Tariffs.Find(session.TariffId);
                var client = _db.Clients.Find(session.ClientId);
                var computer = _db.Computers.Find(session.ComputerId);

                if (tariff == null)
                {
                    MessageBox.Show("Тариф не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (client == null)
                {
                    MessageBox.Show("Клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (computer == null)
                {
                    MessageBox.Show("Компьютер не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var actualMinutes = (DateTime.Now - session.StartTime).TotalMinutes;
                var plannedMinutes = tariff.DurationMinutes;

                decimal finalCost = 0;
                string costCalculationMessage = "";

                if (actualMinutes >= plannedMinutes)
                {
                    finalCost = tariff.Price;
                    costCalculationMessage = $"Сессия завершена полностью (фактически {actualMinutes:F0} мин. из {plannedMinutes} мин.)";
                }
                else if (actualMinutes <= plannedMinutes / 2)
                {
                    finalCost = tariff.Price / 2;
                    costCalculationMessage = $"Сессия завершена досрочно (отработано {actualMinutes:F0} мин. из {plannedMinutes} мин.)\nОплата: 50% от стоимости тарифа";
                }
                else
                {
                    finalCost = tariff.Price;
                    costCalculationMessage = $"Сессия завершена досрочно, но отработано больше половины времени ({actualMinutes:F0} мин. из {plannedMinutes} мин.)\nОплата: полная стоимость";
                }

                if (client.Balance < finalCost)
                {
                    var result = MessageBox.Show($"Недостаточно средств для оплаты сессии!\n\n" +
                        $"Клиент: {client.Nickname}\n" +
                        $"Текущий баланс: {client.Balance:F2} руб.\n" +
                        $"Требуется к оплате: {finalCost:F2} руб.\n" +
                        $"Необходимо пополнить: {(finalCost - client.Balance):F2} руб.\n\n" +
                        $"Пополните баланс клиента в разделе \"Клиенты\" и повторите завершение сессии.\n\n" +
                        $"Отменить завершение сессии?",
                        "Недостаточно средств", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                var confirmResult = MessageBox.Show($"Завершить сессию?\n\n" +
                    $"Клиент: {client.Nickname}\n" +
                    $"Компьютер: {computer.ComputerName}\n" +
                    $"Время начала: {session.StartTime:HH:mm}\n" +
                    $"Фактическое время: {actualMinutes:F0} мин.\n" +
                    $"Плановое время: {plannedMinutes} мин.\n\n" +
                    $"{costCalculationMessage}\n" +
                    $"Сумма к оплате: {finalCost:F2} руб.\n" +
                    $"Текущий баланс: {client.Balance:F2} руб.\n" +
                    $"Баланс после списания: {(client.Balance - finalCost):F2} руб.\n\n" +
                    $"Продолжить?",
                    "Подтверждение завершения", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmResult != MessageBoxResult.Yes) return;

                client.Balance -= finalCost;

                session.EndTime = DateTime.Now;
                session.TotalCost = finalCost;

                _db.SaveChanges();

                computer.IsAvailable = true;
                _db.SaveChanges();

                RefreshSessionsData();
                RefreshComputersData();
                RefreshClientsData();
                ClearSessionForm();

                string successMessage = $"Сессия успешно завершена!\n\n" +
                    $"Клиент: {client.Nickname}\n" +
                    $"Компьютер: {computer.ComputerName}\n" +
                    $"Время начала: {session.StartTime:HH:mm}\n" +
                    $"Время окончания: {session.EndTime:HH:mm}\n" +
                    $"Фактическое время: {actualMinutes:F0} мин.\n" +
                    $"Списано: {finalCost:F2} руб.\n" +
                    $"Остаток на балансе: {client.Balance:F2} руб.";

                if (client.Balance < 0)
                {
                    successMessage += $"\n\n⚠️ ВНИМАНИЕ: Баланс клиента стал отрицательным!";
                }

                MessageBox.Show(successMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshSessionsData();
                RefreshComputersData();
                RefreshClientsData();

                var innerMessage = ex.InnerException?.Message ?? ex.Message;

                if (innerMessage.Contains("FK") || innerMessage.Contains("REFERENCE"))
                {
                    MessageBox.Show("Ошибка при завершении сессии: нарушение ссылочной целостности.",
                        "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Ошибка при завершении сессии: {innerMessage}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshSessionsData();
                RefreshComputersData();
                RefreshClientsData();

                MessageBox.Show($"Ошибка при завершении сессии:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckAndEndExpiredSessions()
        {
            var now = DateTime.Now;
            var expiredSessions = _db.Sessions
                .Where(s => s.EndTime <= now && s.EndTime != null && s.TotalCost == 0)
                .ToList();

            foreach (var session in expiredSessions)
            {
                var tariff = _db.Tariffs.Find(session.TariffId);
                var client = _db.Clients.Find(session.ClientId);
                var computer = _db.Computers.Find(session.ComputerId);

                if (tariff != null && client != null && computer != null)
                {
                    decimal finalCost = tariff.Price;

                    session.TotalCost = finalCost;
                    session.EndTime = now;

                    if (client.Balance >= finalCost)
                    {
                        client.Balance -= finalCost;
                    }
                    else
                    {
     
                        client.Balance -= finalCost;
                    }

                    computer.IsAvailable = true;
                }
            }

            if (expiredSessions.Any())
            {
                _db.SaveChanges();
                RefreshSessionsData();
                RefreshComputersData();
                RefreshClientsData();
            }
        }

        private void BtnSessionEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование сессий!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgSessions.SelectedItem is not Session session) { MessageBox.Show("Выберите сессию!"); return; }

            session.ClientId = (int)CmbSessionClient.SelectedValue;
            session.ComputerId = (int)CmbSessionComputer.SelectedValue;
            session.EmployeeId = (int)CmbSessionEmployee.SelectedValue;
            session.TariffId = (int)CmbSessionTariff.SelectedValue;
            session.PaymentMethodId = (int)CmbSessionPayment.SelectedValue;

            try
            {
                _db.SaveChanges();
                RefreshSessionsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении сессии");
            }
        }

        private void BtnSessionDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление сессий!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgSessions.SelectedItem is not Session session)
            {
                MessageBox.Show("Выберите сессию!");
                return;
            }

            var hasOrders = _db.Orders.Any(o => o.SessionId == session.SessionId);

            if (hasOrders)
            {
                MessageBox.Show("Нельзя удалить сессию, так как у нее есть связанные заказы!\n" +
                    "Сначала удалите все заказы этой сессии.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить сессию от {session.StartTime:dd.MM.yyyy HH:mm}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                if (session.EndTime == null)
                {
                    var computer = _db.Computers.Find(session.ComputerId);
                    if (computer != null)
                    {
                        computer.IsAvailable = true;
                    }
                }

                _db.Sessions.Remove(session);
                _db.SaveChanges();
                RefreshSessionsData();
                RefreshComputersData();
                ClearSessionForm();
                MessageBox.Show("Сессия удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshSessionsData();
                RefreshComputersData();

                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshSessionsData();
                RefreshComputersData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgSessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgSessions.SelectedItem is Session session)
            {
                CmbSessionClient.SelectedValue = session.ClientId;
                CmbSessionComputer.SelectedValue = session.ComputerId;
                CmbSessionEmployee.SelectedValue = session.EmployeeId;
                CmbSessionTariff.SelectedValue = session.TariffId;
                CmbSessionPayment.SelectedValue = session.PaymentMethodId;
            }
        }

        private void ClearSessionForm()
        {
            CmbSessionClient.SelectedIndex = -1;
            CmbSessionComputer.SelectedIndex = -1;
            CmbSessionEmployee.SelectedIndex = -1;
            CmbSessionTariff.SelectedIndex = -1;
            CmbSessionPayment.SelectedIndex = -1;
        }

        private void RefreshSessionsData()
        {
            _db.Sessions.Include(s => s.Client).Include(s => s.Computer)
                        .Include(s => s.Employee).Include(s => s.Tariff)
                        .Include(s => s.PaymentMethod).Load();
            _sessions = _db.Sessions.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var session in _sessions)
            {
                session.RowNumber = rowNumber++;
            }

            _sessionView = CollectionViewSource.GetDefaultView(_sessions);
            DgSessions.ItemsSource = _sessionView;
            _sessionView.Refresh();


            var availableComputers = _db.Computers.Local.Where(c => c.IsAvailable).ToList();
            CmbSessionComputer.ItemsSource = availableComputers;
            CmbSessionComputer.DisplayMemberPath = "ComputerName";
            CmbSessionComputer.SelectedValuePath = "ComputerId";

            CmbSessionClient.ItemsSource = _db.Clients.Local.ToObservableCollection();

            RefreshEmployeeComboBoxes();

            CmbSessionTariff.ItemsSource = _db.Tariffs.Local.ToObservableCollection();
            CmbSessionTariff.DisplayMemberPath = "TariffName";
            CmbSessionTariff.SelectedValuePath = "TariffId";

            CmbSessionPayment.ItemsSource = _db.PaymentMethods.Local.ToObservableCollection();
            CmbSessionPayment.DisplayMemberPath = "MethodName";
            CmbSessionPayment.SelectedValuePath = "PaymentMethodId";
        }

        private void BtnHallAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Technician")
            {
                MessageBox.Show("У вас нет прав на добавление залов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpHallName.Text)) { MessageBox.Show("Введите название зала!"); return; }

            var hall = new Hall
            {
                Name = InpHallName.Text.Trim(),
                Description = InpHallDesc.Text.Trim()
            };

            try
            {
                _db.Halls.Add(hall);
                _db.SaveChanges();
                RefreshHallsData();
                ClearHallForm();
                MessageBox.Show("Зал добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(hall).State = EntityState.Detached;
                HandleError(ex, "добавлении зала");
            }
        }

        private void BtnHallEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Technician")
            {
                MessageBox.Show("У вас нет прав на редактирование залов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgHalls.SelectedItem is not Hall hall) { MessageBox.Show("Выберите зал!"); return; }

            hall.Name = InpHallName.Text.Trim();
            hall.Description = InpHallDesc.Text.Trim();

            try
            {
                _db.SaveChanges();
                RefreshHallsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении зала");
            }
        }

        private void BtnHallDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Technician")
            {
                MessageBox.Show("У вас нет прав на удаление залов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgHalls.SelectedItem is not Hall hall)
            {
                MessageBox.Show("Выберите зал!");
                return;
            }

            var hasComputers = _db.Computers.Any(c => c.HallId == hall.HallId);
            if (hasComputers)
            {
                MessageBox.Show($"Нельзя удалить зал \"{hall.Name}\"!\n\n" +
                    "В этом зале есть компьютеры.\n" +
                    "Сначала удалите все компьютеры из этого зала.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var hasTariffs = _db.Tariffs.Any(t => t.HallId == hall.HallId);
            if (hasTariffs)
            {
                MessageBox.Show($"Нельзя удалить зал \"{hall.Name}\"!\n\n" +
                    "Для этого зала есть тарифы.\n" +
                    "Сначала удалите все тарифы для этого зала.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить зал \"{hall.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Halls.Remove(hall);
                _db.SaveChanges();
                RefreshHallsData();
                ClearHallForm();
                MessageBox.Show("Зал удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshHallsData();
                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshHallsData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgHalls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgHalls.SelectedItem is Hall hall)
            {
                InpHallName.Text = hall.Name;
                InpHallDesc.Text = hall.Description;
            }
        }

        private void ClearHallForm()
        {
            InpHallName.Clear();
            InpHallDesc.Clear();
        }

        private void RefreshHallsData()
        {
            _db.Halls.Load();
            _halls = _db.Halls.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var hall in _halls)
            {
                hall.RowNumber = rowNumber++;
            }

            _hallView = CollectionViewSource.GetDefaultView(_halls);
            DgHalls.ItemsSource = _hallView;
            _hallView.Refresh();
        }

        private void BtnRankAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление рангов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpRankName.Text)) { MessageBox.Show("Введите название ранга!"); return; }

            var rank = new ClientRank
            {
                RankName = InpRankName.Text.Trim(),
                DiscountPercent = decimal.TryParse(InpRankDiscount.Text, out var disc) ? disc : 0
            };

            try
            {
                _db.ClientRanks.Add(rank);
                _db.SaveChanges();
                RefreshRanksData();
                ClearRankForm();
                MessageBox.Show("Ранг добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(rank).State = EntityState.Detached;
                HandleError(ex, "добавлении ранга");
            }
        }

        private void BtnRankEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование рангов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgRanks.SelectedItem is not ClientRank rank) { MessageBox.Show("Выберите ранг!"); return; }

            rank.RankName = InpRankName.Text.Trim();
            rank.DiscountPercent = decimal.TryParse(InpRankDiscount.Text, out var disc) ? disc : 0;

            try
            {
                _db.SaveChanges();
                RefreshRanksData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении ранга");
            }
        }

        private void BtnRankDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление рангов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgRanks.SelectedItem is not ClientRank rank)
            {
                MessageBox.Show("Выберите ранг!");
                return;
            }

            var hasClients = _db.Clients.Any(c => c.RankId == rank.RankId);

            if (hasClients)
            {
                MessageBox.Show($"Нельзя удалить ранг \"{rank.RankName}\"!\n\n" +
                    "Есть клиенты с этим рангом.\n" +
                    "Сначала измените ранг у всех клиентов.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить ранг \"{rank.RankName}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.ClientRanks.Remove(rank);
                _db.SaveChanges();
                RefreshRanksData();
                ClearRankForm();
                MessageBox.Show("Ранг удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshRanksData();
                MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshRanksData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgRanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgRanks.SelectedItem is ClientRank rank)
            {
                InpRankName.Text = rank.RankName;
                InpRankDiscount.Text = rank.DiscountPercent.ToString("F2");
            }
        }

        private void ClearRankForm()
        {
            InpRankName.Clear();
            InpRankDiscount.Clear();
        }

        private void RefreshRanksData()
        {
            _db.ClientRanks.Load();
            _ranks = _db.ClientRanks.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var rank in _ranks)
            {
                rank.RowNumber = rowNumber++;
            }

            _rankView = CollectionViewSource.GetDefaultView(_ranks);
            DgRanks.ItemsSource = _rankView;
            _rankView.Refresh();

            CmbClientRank.ItemsSource = _db.ClientRanks.Local.ToObservableCollection();
        }
        private void BtnMethodAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление методов оплаты!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpMethodName.Text)) { MessageBox.Show("Введите метод оплаты!"); return; }

            var method = new PaymentMethod { MethodName = InpMethodName.Text.Trim() };

            try
            {
                _db.PaymentMethods.Add(method);
                _db.SaveChanges();
                RefreshPaymentMethodsData();
                ClearMethodForm();
                MessageBox.Show("Метод оплаты добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(method).State = EntityState.Detached;
                HandleError(ex, "добавлении метода оплаты");
            }
        }

        private void BtnMethodEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование методов оплаты!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgPaymentMethods.SelectedItem is not PaymentMethod method) { MessageBox.Show("Выберите метод оплаты!"); return; }

            method.MethodName = InpMethodName.Text.Trim();

            try
            {
                _db.SaveChanges();
                RefreshPaymentMethodsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении метода оплаты");
            }
        }

        private void BtnMethodDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление методов оплаты!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgPaymentMethods.SelectedItem is not PaymentMethod method)
            {
                MessageBox.Show("Выберите метод оплаты!");
                return;
            }

            var hasSessions = _db.Sessions.Any(s => s.PaymentMethodId == method.PaymentMethodId);
            if (hasSessions)
            {
                MessageBox.Show($"Нельзя удалить метод оплаты \"{method.MethodName}\"!\n\n" +
                    "Есть сессии, оплаченные этим методом.\n" +
                    "Сначала измените метод оплаты у всех сессий.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var hasOrders = _db.Orders.Any(o => o.PaymentMethodId == method.PaymentMethodId);
            if (hasOrders)
            {
                MessageBox.Show($"Нельзя удалить метод оплаты \"{method.MethodName}\"!\n\n" +
                    "Есть заказы, оплаченные этим методом.\n" +
                    "Сначала измените метод оплаты у всех заказов.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить метод оплаты \"{method.MethodName}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.PaymentMethods.Remove(method);
                _db.SaveChanges();
                RefreshPaymentMethodsData();
                ClearMethodForm();
                MessageBox.Show("Метод оплаты удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshPaymentMethodsData();
                MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshPaymentMethodsData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgPaymentMethods.SelectedItem is PaymentMethod method)
                InpMethodName.Text = method.MethodName;
        }

        private void ClearMethodForm()
        {
            InpMethodName.Clear();
        }

        private void RefreshPaymentMethodsData()
        {
            _db.PaymentMethods.Load();
            _paymentMethods = _db.PaymentMethods.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var method in _paymentMethods)
            {
                method.RowNumber = rowNumber++;
            }

            _methodView = CollectionViewSource.GetDefaultView(_paymentMethods);
            DgPaymentMethods.ItemsSource = _methodView;
            _methodView.Refresh();

            CmbSessionPayment.ItemsSource = _db.PaymentMethods.Local.ToObservableCollection();
            CmbOrderPayment.ItemsSource = _db.PaymentMethods.Local.ToObservableCollection();
        }

        private void BtnCategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление категорий!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpCategoryName.Text)) { MessageBox.Show("Введите категорию!"); return; }

            var category = new ProductCategory { CategoryName = InpCategoryName.Text.Trim() };

            try
            {
                _db.ProductCategories.Add(category);
                _db.SaveChanges();
                RefreshCategoriesData();
                ClearCategoryForm();
                MessageBox.Show("Категория добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(category).State = EntityState.Detached;
                HandleError(ex, "добавлении категории");
            }
        }

        private void BtnCategoryEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование категорий!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgCategories.SelectedItem is not ProductCategory category) { MessageBox.Show("Выберите категорию!"); return; }

            category.CategoryName = InpCategoryName.Text.Trim();

            try
            {
                _db.SaveChanges();
                RefreshCategoriesData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении категории");
            }
        }

        private void BtnCategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление категорий!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgCategories.SelectedItem is not ProductCategory category)
            {
                MessageBox.Show("Выберите категорию!");
                return;
            }

            var hasProducts = _db.Products.Any(p => p.CategoryId == category.CategoryId);

            if (hasProducts)
            {
                MessageBox.Show($"Нельзя удалить категорию \"{category.CategoryName}\"!\n\n" +
                    "В этой категории есть продукты.\n" +
                    "Сначала удалите или переместите все продукты из этой категории.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить категорию \"{category.CategoryName}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.ProductCategories.Remove(category);
                _db.SaveChanges();
                RefreshCategoriesData();
                ClearCategoryForm();
                MessageBox.Show("Категория удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshCategoriesData();

                if (ex.InnerException?.Message.Contains("REFERENCE") == true ||
                    ex.InnerException?.Message.Contains("FK") == true)
                {
                    MessageBox.Show($"Невозможно удалить категорию \"{category.CategoryName}\"!\n\n" +
                        "В ней есть продукты.\n" +
                        "Сначала удалите все продукты из этой категории.",
                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshCategoriesData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgCategories.SelectedItem is ProductCategory category)
                InpCategoryName.Text = category.CategoryName;
        }

        private void ClearCategoryForm()
        {
            InpCategoryName.Clear();
        }

        private void RefreshCategoriesData()
        {
            _db.ProductCategories.Load();
            _categories = _db.ProductCategories.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var category in _categories)
            {
                category.RowNumber = rowNumber++;
            }

            _categoryView = CollectionViewSource.GetDefaultView(_categories);
            DgCategories.ItemsSource = _categoryView;
            _categoryView.Refresh();

            CmbProductCategory.ItemsSource = _db.ProductCategories.Local.ToObservableCollection();
        }


        private void BtnPositionAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление должностей!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InpPositionTitle.Text)) { MessageBox.Show("Введите должность!"); return; }

            var position = new Position { Title = InpPositionTitle.Text.Trim() };

            try
            {
                _db.Positions.Add(position);
                _db.SaveChanges();
                RefreshPositionsData();
                ClearPositionForm();
                MessageBox.Show("Должность добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(position).State = EntityState.Detached;
                HandleError(ex, "добавлении должности");
            }
        }

        private void BtnPositionEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование должностей!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgPositions.SelectedItem is not Position position) { MessageBox.Show("Выберите должность!"); return; }

            position.Title = InpPositionTitle.Text.Trim();

            try
            {
                _db.SaveChanges();
                RefreshPositionsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении должности");
            }
        }

        private void BtnPositionDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление должностей!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgPositions.SelectedItem is not Position position)
            {
                MessageBox.Show("Выберите должность!");
                return;
            }

            var hasEmployees = _db.Employees.Any(e => e.PositionId == position.PositionId);

            if (hasEmployees)
            {
                MessageBox.Show($"Нельзя удалить должность \"{position.Title}\"!\n\n" +
                    "Есть сотрудники с этой должностью.\n" +
                    "Сначала измените должность у всех сотрудников.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить должность \"{position.Title}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Positions.Remove(position);
                _db.SaveChanges();
                RefreshPositionsData();
                ClearPositionForm();
                MessageBox.Show("Должность удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshPositionsData();
                MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshPositionsData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgPositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgPositions.SelectedItem is Position position)
                InpPositionTitle.Text = position.Title;
        }

        private void ClearPositionForm()
        {
            InpPositionTitle.Clear();
        }

        private void RefreshPositionsData()
        {
            _db.Positions.Load();
            _positions = _db.Positions.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var position in _positions)
            {
                position.RowNumber = rowNumber++;
            }

            _positionView = CollectionViewSource.GetDefaultView(_positions);
            DgPositions.ItemsSource = _positionView;
            _positionView.Refresh();

            CmbEmployeePosition.ItemsSource = _db.Positions.Local.ToObservableCollection();
        }


        private void BtnComputerGameAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на добавление игр на компьютер!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbComputerGameComputer.SelectedValue == null) { MessageBox.Show("Выберите компьютер!"); return; }
            if (CmbComputerGameGame.SelectedValue == null) { MessageBox.Show("Выберите игру!"); return; }

            var computerGame = new ComputerGame
            {
                ComputerId = (int)CmbComputerGameComputer.SelectedValue,
                GameId = (int)CmbComputerGameGame.SelectedValue
            };

            try
            {
                _db.ComputerGames.Add(computerGame);
                _db.SaveChanges();
                RefreshComputerGamesData();
                ClearComputerGameForm();
                MessageBox.Show("Игра добавлена на компьютер!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(computerGame).State = EntityState.Detached;
                HandleError(ex, "добавлении игры на компьютер");
            }
        }

        private void BtnComputerGameDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление игр с компьютера!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgComputerGames.SelectedItem is not ComputerGame computerGame)
            {
                MessageBox.Show("Выберите запись!");
                return;
            }

            if (MessageBox.Show($"Удалить игру с компьютера?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.ComputerGames.Remove(computerGame);
                _db.SaveChanges();
                _computerGames.Remove(computerGame);
                RefreshComputerGamesData();
                ClearComputerGameForm();
                MessageBox.Show("Игра удалена с компьютера!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(computerGame).State = EntityState.Detached;
                _db.ChangeTracker.Clear();
                RefreshComputerGamesData();
                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgComputerGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgComputerGames.SelectedItem is ComputerGame computerGame)
            {
                CmbComputerGameComputer.SelectedValue = computerGame.ComputerId;
                CmbComputerGameGame.SelectedValue = computerGame.GameId;
            }
        }

        private void ClearComputerGameForm()
        {
            CmbComputerGameComputer.SelectedIndex = -1;
            CmbComputerGameGame.SelectedIndex = -1;
        }

        private void RefreshComputerGamesData()
        {
            _db.ComputerGames.Include(cg => cg.Computer).Include(cg => cg.Game).Load();
            _computerGames = _db.ComputerGames.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var computerGame in _computerGames)
            {
                computerGame.RowNumber = rowNumber++;
            }

            _computerGameView = CollectionViewSource.GetDefaultView(_computerGames);
            DgComputerGames.ItemsSource = _computerGameView;
            _computerGameView.Refresh();

            CmbComputerGameComputer.ItemsSource = _db.Computers.Local.ToObservableCollection();
            CmbComputerGameGame.ItemsSource = _db.Games.Local.ToObservableCollection();
        }


        private void BtnOrderAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Employee")
            {
                MessageBox.Show("У вас нет прав на добавление заказов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbOrderSession.SelectedValue == null) { MessageBox.Show("Выберите сессию!"); return; }
            if (CmbOrderEmployee.SelectedValue == null) { MessageBox.Show("Выберите сотрудника!"); return; }
            if (CmbOrderPayment.SelectedValue == null) { MessageBox.Show("Выберите метод оплаты!"); return; }

            var order = new Order
            {
                SessionId = (int)CmbOrderSession.SelectedValue,
                EmployeeId = (int)CmbOrderEmployee.SelectedValue,
                PaymentMethodId = (int)CmbOrderPayment.SelectedValue,
                OrderTime = DateTime.Now
            };

            try
            {
                _db.Orders.Add(order);
                _db.SaveChanges();
                RefreshOrdersData();
                ClearOrderForm();
                MessageBox.Show("Заказ добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(order).State = EntityState.Detached;
                HandleError(ex, "добавлении заказа");
            }
        }

        private void BtnOrderEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование заказов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgOrders.SelectedItem is not Order order) { MessageBox.Show("Выберите заказ!"); return; }

            order.SessionId = (int)CmbOrderSession.SelectedValue;
            order.EmployeeId = (int)CmbOrderEmployee.SelectedValue;
            order.PaymentMethodId = (int)CmbOrderPayment.SelectedValue;

            try
            {
                _db.SaveChanges();
                RefreshOrdersData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении заказа");
            }
        }

        private void BtnOrderDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление заказов!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgOrders.SelectedItem is not Order order)
            {
                MessageBox.Show("Выберите заказ!");
                return;
            }

            var hasDetails = _db.OrderDetails.Any(od => od.OrderId == order.OrderId);

            if (hasDetails)
            {
                MessageBox.Show("Нельзя удалить заказ, в котором есть товары!\n" +
                    "Сначала удалите все товары из этого заказа.",
                    "Действие запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить заказ #{order.OrderId} от {order.OrderTime:dd.MM.yyyy HH:mm}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.Orders.Remove(order);
                _db.SaveChanges();
                RefreshOrdersData();
                ClearOrderForm();
                MessageBox.Show("Заказ удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                _db.ChangeTracker.Clear();
                RefreshOrdersData();

                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                RefreshOrdersData();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgOrders.SelectedItem is Order order)
            {
                CmbOrderSession.SelectedValue = order.SessionId;
                CmbOrderEmployee.SelectedValue = order.EmployeeId;
                CmbOrderPayment.SelectedValue = order.PaymentMethodId;
            }
        }

        private void ClearOrderForm()
        {
            CmbOrderSession.SelectedIndex = -1;
            CmbOrderEmployee.SelectedIndex = -1;
            CmbOrderPayment.SelectedIndex = -1;
        }

        private void RefreshOrdersData()
        {
            _db.Orders
                .Include(o => o.Session)
                    .ThenInclude(s => s.Client)
                .Include(o => o.Session)
                    .ThenInclude(s => s.Computer)
                .Include(o => o.Employee)
                .Include(o => o.PaymentMethod)
                .Load();

            _orders = _db.Orders.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var order in _orders)
            {
                order.RowNumber = rowNumber++;
            }

            _orderView = CollectionViewSource.GetDefaultView(_orders);
            DgOrders.ItemsSource = _orderView;
            _orderView.Refresh();

            RefreshOrderComboBoxes();
            RefreshSessionComboBox();
            RefreshEmployeeComboBoxes();

            if (CmbOrderPayment != null)
            {
                CmbOrderPayment.ItemsSource = _db.PaymentMethods.Local.ToObservableCollection();
                CmbOrderPayment.DisplayMemberPath = "MethodName";
                CmbOrderPayment.SelectedValuePath = "PaymentMethodId";
            }

            if (CmbOrderClient != null)
            {
                var now = DateTime.Now;
                var clientsWithActiveSessions = _db.Clients
                    .Where(c => _db.Sessions.Any(s => s.ClientId == c.ClientId && s.EndTime > now))
                    .ToList();
                CmbOrderClient.ItemsSource = clientsWithActiveSessions;
                CmbOrderClient.DisplayMemberPath = "Nickname";
                CmbOrderClient.SelectedValuePath = "ClientId";
            }
        }


        private void BtnOrderDetailAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin" && _userRole != "Employee")
            {
                MessageBox.Show("У вас нет прав на добавление товаров в заказ!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbOrderDetailOrder.SelectedValue == null)
            {
                MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CmbOrderDetailProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var product = _db.Products.Find((int)CmbOrderDetailProduct.SelectedValue);
                var order = _db.Orders.Find((int)CmbOrderDetailOrder.SelectedValue);

                if (product == null) { MessageBox.Show("Товар не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (order == null) { MessageBox.Show("Заказ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                var session = _db.Sessions.Include(s => s.Client).ThenInclude(c => c.Rank).FirstOrDefault(s => s.SessionId == order.SessionId);
                if (session?.Client == null) { MessageBox.Show("Клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                var discountPercent = session.Client.Rank?.DiscountPercent ?? 0;

                decimal priceWithDiscount = product.Price * (1 - discountPercent / 100);

                int quantity = int.TryParse(InpOrderDetailQuantity.Text, out var qty) && qty > 0 ? qty : 1;

                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    PriceAtPurchase = priceWithDiscount
                };
                orderDetail.Total = orderDetail.Quantity * orderDetail.PriceAtPurchase;

                _db.OrderDetails.Add(orderDetail);
                _db.SaveChanges();
                RefreshOrderDetailsData();
                ClearOrderDetailForm();

                MessageBox.Show($"Товар добавлен в заказ!\n\n" +
                    $"Товар: {product.ProductName}\n" +
                    $"Количество: {quantity}\n" +
                    $"Цена: {product.Price:F2} руб.\n" +
                    $"Скидка: {discountPercent}%\n" +
                    $"Итого: {priceWithDiscount:F2} руб./шт.\n" +
                    $"Сумма: {orderDetail.Total:F2} руб.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "добавлении товара в заказ");
            }
        }

        private void BtnOrderDetailEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на редактирование деталей заказа!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgOrderDetails.SelectedItem is not OrderDetail orderDetail) { MessageBox.Show("Выберите деталь заказа!"); return; }

            orderDetail.OrderId = (int)CmbOrderDetailOrder.SelectedValue;
            orderDetail.ProductId = (int)CmbOrderDetailProduct.SelectedValue;
            orderDetail.Quantity = int.TryParse(InpOrderDetailQuantity.Text, out var qty) && qty > 0 ? qty : 1;
            orderDetail.PriceAtPurchase = decimal.TryParse(InpOrderDetailPrice.Text, out var price) ? price : orderDetail.PriceAtPurchase;

            try
            {
                _db.SaveChanges();
                RefreshOrderDetailsData();
                MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(ex, "обновлении детали заказа");
            }
        }

        private void BtnOrderDetailDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_userRole != "Admin")
            {
                MessageBox.Show("У вас нет прав на удаление товаров из заказа!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgOrderDetails.SelectedItem is not OrderDetail orderDetail) { MessageBox.Show("Выберите деталь заказа!"); return; }
            if (MessageBox.Show($"Удалить товар из заказа?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _db.OrderDetails.Remove(orderDetail);
                _db.SaveChanges();
                _orderDetails.Remove(orderDetail);
                RefreshOrderDetailsData();
                ClearOrderDetailForm();
                MessageBox.Show("Товар удален из заказа!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.Entry(orderDetail).State = EntityState.Detached;
                _db.ChangeTracker.Clear();
                RefreshOrderDetailsData();
                HandleError(ex, "удалении детали заказа");
            }
        }

        private void DgOrderDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgOrderDetails.SelectedItem is OrderDetail orderDetail)
            {
                CmbOrderDetailOrder.SelectedValue = orderDetail.OrderId;
                CmbOrderDetailProduct.SelectedValue = orderDetail.ProductId;
                InpOrderDetailQuantity.Text = orderDetail.Quantity.ToString();
                InpOrderDetailPrice.Text = orderDetail.PriceAtPurchase.ToString("F2");
            }
        }

        private void ClearOrderDetailForm()
        {
            CmbOrderDetailOrder.SelectedIndex = -1;
            CmbOrderDetailProduct.SelectedIndex = -1;
            InpOrderDetailQuantity.Text = "1";
            InpOrderDetailPrice.Clear();
        }

        private void RefreshOrderDetailsData()
        {
            _db.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Session)
                        .ThenInclude(s => s.Client)
                            .ThenInclude(c => c.Rank)
                .Load();

            _orderDetails = _db.OrderDetails.Local.ToObservableCollection();

            int rowNumber = 1;
            foreach (var orderDetail in _orderDetails)
            {
                orderDetail.Total = orderDetail.Quantity * orderDetail.PriceAtPurchase;
                orderDetail.RowNumber = rowNumber++;
            }

            _orderDetailView = CollectionViewSource.GetDefaultView(_orderDetails);
            DgOrderDetails.ItemsSource = _orderDetailView;
            _orderDetailView.Refresh();

            RefreshOrderComboBoxes();

            if (CmbOrderDetailProduct != null)
            {
                CmbOrderDetailProduct.ItemsSource = _db.Products.Local.ToObservableCollection();
                CmbOrderDetailProduct.DisplayMemberPath = "ProductName";
                CmbOrderDetailProduct.SelectedValuePath = "ProductId";
            }
        }

        private void CmbOrderDetailProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbOrderDetailProduct.SelectedValue != null && CmbOrderDetailOrder.SelectedValue != null)
            {
                var product = _db.Products.Find((int)CmbOrderDetailProduct.SelectedValue);
                var order = _db.Orders.Find((int)CmbOrderDetailOrder.SelectedValue);

                if (product != null && order != null)
                {
                    var session = _db.Sessions.Include(s => s.Client).ThenInclude(c => c.Rank)
                                .FirstOrDefault(s => s.SessionId == order.SessionId);

                    if (session?.Client?.Rank != null)
                    {
                        var discountPercent = session.Client.Rank.DiscountPercent;
                        decimal priceWithDiscount = product.Price * (1 - discountPercent / 100);
                        InpOrderDetailPrice.Text = priceWithDiscount.ToString("F2");

                        ToolTip = $"Цена со скидкой {discountPercent}%: {priceWithDiscount:F2} руб. (было: {product.Price:F2} руб.)";
                    }
                    else
                    {
                        InpOrderDetailPrice.Text = product.Price.ToString("F2");
                    }
                }
            }
        }


        private void CmbOrderClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbOrderClient?.SelectedValue == null)
            {
                if (CmbOrderComputer != null)
                {
                    CmbOrderComputer.IsEnabled = false;
                    CmbOrderComputer.ItemsSource = new List<Computer>();
                    CmbOrderComputer.SelectedIndex = -1;
                }
                if (CmbOrderSession != null)
                {
                    CmbOrderSession.ItemsSource = new List<object>();
                    CmbOrderSession.SelectedIndex = -1;
                }
                return;
            }

            try
            {
                int clientId = (int)CmbOrderClient.SelectedValue;
                var now = DateTime.Now;

                var computers = _db.Sessions
                    .Where(s => s.ClientId == clientId && s.EndTime > now && s.TotalCost == 0)
                    .Select(s => s.Computer)
                    .Where(c => c != null)
                    .Distinct()
                    .ToList();

                if (CmbOrderComputer != null)
                {
                    CmbOrderComputer.ItemsSource = computers;
                    CmbOrderComputer.DisplayMemberPath = "ComputerName";
                    CmbOrderComputer.SelectedValuePath = "ComputerId";
                    CmbOrderComputer.IsEnabled = computers.Any();
                    CmbOrderComputer.SelectedIndex = -1;

                    if (!computers.Any())
                    {
                        CmbOrderSession.ItemsSource = new List<object>();
                        MessageBox.Show("У данного клиента нет активных сессий.", "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в CmbOrderClient_SelectionChanged: {ex.Message}");
            }
        }

        private void CmbOrderComputer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbOrderComputer == null || CmbOrderClient == null) return;

            if (CmbOrderComputer.SelectedValue == null || CmbOrderClient.SelectedValue == null)
            {
                if (CmbOrderSession != null)
                {
                    CmbOrderSession.ItemsSource = new List<object>();
                    CmbOrderSession.SelectedIndex = -1;
                }
                return;
            }

            try
            {
                int clientId = (int)CmbOrderClient.SelectedValue;
                int computerId = (int)CmbOrderComputer.SelectedValue;
                var now = DateTime.Now;

                var sessions = _db.Sessions
                    .Where(s => s.ClientId == clientId &&
                               s.ComputerId == computerId &&
                               s.EndTime > now &&
                               s.TotalCost == 0)
                    .Include(s => s.Client)
                    .Include(s => s.Computer)
                    .ToList();

                var sessionList = sessions.Select(s => new
                {
                    s.SessionId,
                    SessionInfo = $"Сессия #{s.SessionId} | До {s.EndTime:HH:mm} | {((s.EndTime - now)?.TotalMinutes ?? 0):F0} мин"
                }).ToList();

                if (CmbOrderSession != null)
                {
                    CmbOrderSession.ItemsSource = sessionList;
                    CmbOrderSession.DisplayMemberPath = "SessionInfo";
                    CmbOrderSession.SelectedValuePath = "SessionId";
                    CmbOrderSession.SelectedIndex = -1;

                    if (!sessionList.Any())
                    {
                        MessageBox.Show("У данного клиента нет активных сессий на этом компьютере.", "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в CmbOrderComputer_SelectionChanged: {ex.Message}");
            }
        }

        private void DebugOrderComboBoxes()
        {
            System.Diagnostics.Debug.WriteLine("=== ОТЛАДКА ЗАКАЗОВ ===");

            try
            {
                var now = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"Текущее время: {now}");

                var activeSessions = _db.Sessions
                    .Where(s => s.EndTime > now && s.TotalCost == 0)
                    .Include(s => s.Client)
                    .Include(s => s.Computer)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Всего активных сессий: {activeSessions.Count}");

                foreach (var s in activeSessions)
                {
                    System.Diagnostics.Debug.WriteLine($"  Сессия {s.SessionId}: Клиент={s.Client?.Nickname}, ПК={s.Computer?.ComputerName}, До={s.EndTime}");
                }

                var clientsWithSessions = _db.Clients
                    .Where(c => _db.Sessions.Any(s => s.ClientId == c.ClientId && s.EndTime > now && s.TotalCost == 0))
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Клиентов с активными сессиями: {clientsWithSessions.Count}");

                foreach (var c in clientsWithSessions)
                {
                    System.Diagnostics.Debug.WriteLine($"  Клиент: {c.Nickname}, ID={c.ClientId}");
                }

                System.Diagnostics.Debug.WriteLine($"CmbOrderClient ItemsSource: {(CmbOrderClient?.ItemsSource != null ? CmbOrderClient.ItemsSource.GetType().ToString() : "NULL")}");
                System.Diagnostics.Debug.WriteLine($"CmbOrderComputer ItemsSource: {(CmbOrderComputer?.ItemsSource != null ? "NOT NULL" : "NULL")}");
                System.Diagnostics.Debug.WriteLine($"CmbOrderSession ItemsSource: {(CmbOrderSession?.ItemsSource != null ? "NOT NULL" : "NULL")}");

                if (CmbOrderClient?.ItemsSource is System.Collections.IEnumerable enumerable)
                {
                    var count = enumerable.Cast<object>().Count();
                    System.Diagnostics.Debug.WriteLine($"CmbOrderClient items count: {count}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка отладки: {ex.Message}");
            }
        }


        private void RefreshSessionComboBox()
        {
            var now = DateTime.Now;
            var sessions = _db.Sessions
                .Include(s => s.Client)
                .Include(s => s.Computer)
                .Where(s => s.EndTime > now && s.TotalCost == 0)
                .ToList();

            var sessionList = sessions.Select(s => new
            {
                s.SessionId,
                SessionInfo = $"{s.SessionId} | {s.Client?.Nickname ?? "Нет клиента"} | {s.Computer?.ComputerName ?? "Нет ПК"} | До {s.EndTime:HH:mm}"
            }).ToList();

            if (CmbOrderSession != null)
            {
                CmbOrderSession.ItemsSource = sessionList;
                CmbOrderSession.DisplayMemberPath = "SessionInfo";
                CmbOrderSession.SelectedValuePath = "SessionId";
            }
        }

        private void RefreshEmployeeComboBoxes()
        {
            var allowedEmployees = _db.Employees
                .Where(e => e.Role == "Admin" || e.Role == "Employee")
                .ToList();

            if (CmbSessionEmployee != null)
            {
                CmbSessionEmployee.ItemsSource = allowedEmployees;
                CmbSessionEmployee.DisplayMemberPath = "FullName";
                CmbSessionEmployee.SelectedValuePath = "EmployeeId";
            }

            if (CmbOrderEmployee != null)
            {
                CmbOrderEmployee.ItemsSource = allowedEmployees;
                CmbOrderEmployee.DisplayMemberPath = "FullName";
                CmbOrderEmployee.SelectedValuePath = "EmployeeId";
            }
        }

        private void RefreshOrderComboBoxes()
        {
            if (_db.Orders == null) return;

            var orders = _db.Orders
                .Include(o => o.Session)
                .ThenInclude(s => s.Client)
                .ToList();

            var orderList = orders.Select(o => new
            {
                o.OrderId,
                OrderInfo = $"#{o.OrderId} | {(o.Session?.Client?.Nickname ?? "Нет клиента")} | {o.OrderTime:dd.MM.yyyy HH:mm}"
            }).ToList();

            if (CmbOrderDetailOrder != null)
            {
                CmbOrderDetailOrder.ItemsSource = orderList;
                CmbOrderDetailOrder.DisplayMemberPath = "OrderInfo";
                CmbOrderDetailOrder.SelectedValuePath = "OrderId";
            }
        }


        private void HandleError(Exception ex, string action)
        {
            _db.ChangeTracker.Clear();
            MessageBox.Show($"Ошибка при {action}:\n{ex.InnerException?.Message ?? ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void DgClients_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Client client)
            {
                e.Row.Header = client.RowNumber.ToString();
            }
        }

        private void DgComputers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Computer computer)
            {
                e.Row.Header = computer.RowNumber.ToString();
            }
        }

        private void DgProducts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Product product)
            {
                e.Row.Header = product.RowNumber.ToString();
            }
        }

        private void DgEmployees_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Employee employee)
            {
                e.Row.Header = employee.RowNumber.ToString();
            }
        }

        private void DgGames_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Game game)
            {
                e.Row.Header = game.RowNumber.ToString();
            }
        }

        private void DgTariffs_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Tariff tariff)
            {
                e.Row.Header = tariff.RowNumber.ToString();
            }
        }

        private void DgSessions_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Session session)
            {
                e.Row.Header = session.RowNumber.ToString();
            }
        }

        private void DgHalls_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Hall hall)
            {
                e.Row.Header = hall.RowNumber.ToString();
            }
        }

        private void DgRanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is ClientRank rank)
            {
                e.Row.Header = rank.RowNumber.ToString();
            }
        }

        private void DgPaymentMethods_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is PaymentMethod method)
            {
                e.Row.Header = method.RowNumber.ToString();
            }
        }

        private void DgCategories_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is ProductCategory category)
            {
                e.Row.Header = category.RowNumber.ToString();
            }
        }

        private void DgPositions_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Position position)
            {
                e.Row.Header = position.RowNumber.ToString();
            }
        }

        private void DgComputerGames_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is ComputerGame computerGame)
            {
                e.Row.Header = computerGame.RowNumber.ToString();
            }
        }

        private void DgOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Order order)
            {
                e.Row.Header = order.RowNumber.ToString();
            }
        }

        private void DgOrderDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is OrderDetail orderDetail)
            {
                e.Row.Header = orderDetail.RowNumber.ToString();
            }
        }

        private AnalyticsQuery _currentQuery;

        private void InitializeAnalyticsQueries()
        {

            if (CmbAnalyticsQuery == null)
            {
                MessageBox.Show("Ошибка: CmbAnalyticsQuery не найден в XAML!", "Ошибка инициализации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var queries = new List<AnalyticsQuery>
    {
        new AnalyticsQuery
        {
            Id = 1,
            Name = "Топ-10 клиентов по балансу",
            Description = "Показывает 10 клиентов с наибольшим балансом",
            SqlQuery = @"SELECT TOP 10 
                            c.Nickname,
                            c.Phone,
                            c.Balance,
                            r.RankName,
                            r.DiscountPercent
                         FROM Clients c
                         JOIN ClientRanks r ON c.RankId = r.RankId
                         ORDER BY c.Balance DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 2,
            Name = "Самая популярная игра в клубе",
            Description = "Показывает игру, которая установлена на наибольшем количестве компьютеров",
            SqlQuery = @"SELECT TOP 1
                            g.Title,
                            g.Developer,
                            COUNT(cg.ComputerId) as ComputersCount
                         FROM Games g
                         JOIN ComputerGames cg ON g.GameId = cg.GameId
                         GROUP BY g.GameId, g.Title, g.Developer
                         ORDER BY ComputersCount DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 3,
            Name = "Выручка по дням за период",
            Description = "Показывает сумму выручки от сессий и заказов по дням",
            SqlQuery = @"SELECT 
                            CAST(s.StartTime AS DATE) as Date,
                            COUNT(DISTINCT s.SessionId) as SessionsCount,
                            SUM(s.TotalCost) as SessionsRevenue,
                            COUNT(DISTINCT o.OrderId) as OrdersCount,
                            SUM(od.Quantity * od.PriceAtPurchase) as OrdersRevenue,
                            SUM(s.TotalCost) + ISNULL(SUM(od.Quantity * od.PriceAtPurchase), 0) as TotalRevenue
                         FROM Sessions s
                         LEFT JOIN Orders o ON s.SessionId = o.SessionId
                         LEFT JOIN OrderDetails od ON o.OrderId = od.OrderId
                         WHERE s.StartTime >= @DateFrom AND s.StartTime <= @DateTo
                         GROUP BY CAST(s.StartTime AS DATE)
                         ORDER BY Date DESC",
            HasParameters = true,
            ParameterName = "DateFrom,DateTo",
            ParameterType = "DateRange"
        },
        new AnalyticsQuery
        {
            Id = 4,
            Name = "Загруженность компьютеров",
            Description = "Показывает общее время работы каждого компьютера",
            SqlQuery = @"SELECT 
                            comp.ComputerName,
                            h.Name as HallName,
                            COUNT(s.SessionId) as SessionsCount,
                            SUM(DATEDIFF(MINUTE, s.StartTime, ISNULL(s.EndTime, GETDATE()))) as TotalMinutes,
                            SUM(CASE WHEN s.EndTime IS NULL THEN DATEDIFF(MINUTE, s.StartTime, GETDATE()) ELSE 0 END) as CurrentSessionMinutes,
                            AVG(DATEDIFF(MINUTE, s.StartTime, ISNULL(s.EndTime, GETDATE()))) as AvgSessionMinutes
                         FROM Computers comp
                         JOIN Halls h ON comp.HallId = h.HallId
                         LEFT JOIN Sessions s ON comp.ComputerId = s.ComputerId
                         GROUP BY comp.ComputerId, comp.ComputerName, h.Name
                         ORDER BY TotalMinutes DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 5,
            Name = "Самые активные сотрудники",
            Description = "Показывает сотрудников по количеству проведённых сессий",
            SqlQuery = @"SELECT 
                            e.FullName,
                            p.Title as Position,
                            COUNT(DISTINCT s.SessionId) as SessionsCount,
                            COUNT(DISTINCT o.OrderId) as OrdersCount,
                            SUM(s.TotalCost) as TotalRevenue
                         FROM Employees e
                         JOIN Positions p ON e.PositionId = p.PositionId
                         LEFT JOIN Sessions s ON e.EmployeeId = s.EmployeeId
                         LEFT JOIN Orders o ON e.EmployeeId = o.EmployeeId
                         GROUP BY e.EmployeeId, e.FullName, p.Title
                         ORDER BY SessionsCount DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 6,
            Name = "Прибыль по тарифам",
            Description = "Показывает, какие тарифы приносят больше всего денег",
            SqlQuery = @"SELECT 
                            t.TariffName,
                            h.Name as HallName,
                            COUNT(s.SessionId) as SessionsCount,
                            SUM(s.TotalCost) as TotalRevenue,
                            AVG(s.TotalCost) as AvgRevenuePerSession
                         FROM Tariffs t
                         JOIN Halls h ON t.HallId = h.HallId
                         LEFT JOIN Sessions s ON t.TariffId = s.TariffId
                         GROUP BY t.TariffId, t.TariffName, h.Name
                         ORDER BY TotalRevenue DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 7,
            Name = "Самые продаваемые товары",
            Description = "Показывает топ-10 товаров по количеству продаж",
            SqlQuery = @"SELECT TOP 10
                            p.ProductName,
                            pc.CategoryName,
                            SUM(od.Quantity) as TotalQuantity,
                            SUM(od.Quantity * od.PriceAtPurchase) as TotalRevenue,
                            AVG(od.PriceAtPurchase) as AvgPrice
                         FROM Products p
                         JOIN ProductCategories pc ON p.CategoryId = pc.CategoryId
                         JOIN OrderDetails od ON p.ProductId = od.ProductId
                         GROUP BY p.ProductId, p.ProductName, pc.CategoryName
                         ORDER BY TotalQuantity DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 8,
            Name = "Активные сессии сейчас",
            Description = "Показывает все активные сессии в реальном времени",
            SqlQuery = @"SELECT 
                            c.Nickname as ClientName,
                            comp.ComputerName,
                            e.FullName as EmployeeName,
                            t.TariffName,
                            s.StartTime,
                            DATEDIFF(MINUTE, s.StartTime, GETDATE()) as DurationMinutes,
                            s.TotalCost
                         FROM Sessions s
                         JOIN Clients c ON s.ClientId = c.ClientId
                         JOIN Computers comp ON s.ComputerId = comp.ComputerId
                         JOIN Employees e ON s.EmployeeId = e.EmployeeId
                         JOIN Tariffs t ON s.TariffId = t.TariffId
                         WHERE s.EndTime IS NULL OR s.EndTime > GETDATE()
                         ORDER BY s.StartTime",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 9,
            Name = "Клиенты без сессий",
            Description = "Показывает клиентов, которые ни разу не играли",
            SqlQuery = @"SELECT 
                            c.Nickname,
                            c.Phone,
                            c.Balance,
                            r.RankName
                         FROM Clients c
                         JOIN ClientRanks r ON c.RankId = r.RankId
                         LEFT JOIN Sessions s ON c.ClientId = s.ClientId
                         WHERE s.SessionId IS NULL
                         ORDER BY c.Nickname",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 10,
            Name = "Часы пик в клубе",
            Description = "Показывает, в какое время суток больше всего сессий",
            SqlQuery = @"SELECT 
                            DATEPART(HOUR, s.StartTime) as Hour,
                            COUNT(s.SessionId) as SessionsCount,
                            AVG(s.TotalCost) as AvgRevenue
                         FROM Sessions s
                         GROUP BY DATEPART(HOUR, s.StartTime)
                         ORDER BY SessionsCount DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 11,
            Name = "Динамика регистрации клиентов",
            Description = "Показывает количество новых клиентов по месяцам",
            SqlQuery = @"SELECT 
                            YEAR(StartTime) as Year,
                            MONTH(StartTime) as Month,
                            COUNT(DISTINCT ClientId) as NewClientsCount
                         FROM Sessions
                         GROUP BY YEAR(StartTime), MONTH(StartTime)
                         ORDER BY Year DESC, Month DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 12,
            Name = "Повторные посещения клиентов",
            Description = "Показывает клиентов, которые играли больше 3 раз",
            SqlQuery = @"SELECT 
                            c.Nickname,
                            c.Phone,
                            COUNT(s.SessionId) as SessionsCount,
                            SUM(s.TotalCost) as TotalSpent,
                            AVG(s.TotalCost) as AvgSpent
                         FROM Clients c
                         JOIN Sessions s ON c.ClientId = s.ClientId
                         GROUP BY c.ClientId, c.Nickname, c.Phone
                         HAVING COUNT(s.SessionId) > 3
                         ORDER BY SessionsCount DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 13,
            Name = "Выручка по методам оплаты",
            Description = "Показывает, как часто используют разные методы оплаты",
            SqlQuery = @"SELECT 
                            pm.MethodName,
                            COUNT(s.SessionId) as SessionsCount,
                            SUM(s.TotalCost) as SessionsRevenue,
                            COUNT(o.OrderId) as OrdersCount,
                            SUM(od.Quantity * od.PriceAtPurchase) as OrdersRevenue,
                            SUM(s.TotalCost) + ISNULL(SUM(od.Quantity * od.PriceAtPurchase), 0) as TotalRevenue
                         FROM PaymentMethods pm
                         LEFT JOIN Sessions s ON pm.PaymentMethodId = s.PaymentMethodId
                         LEFT JOIN Orders o ON pm.PaymentMethodId = o.PaymentMethodId
                         LEFT JOIN OrderDetails od ON o.OrderId = od.OrderId
                         GROUP BY pm.PaymentMethodId, pm.MethodName
                         ORDER BY TotalRevenue DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 14,
            Name = "Долгосрочные клиенты",
            Description = "Показывает клиентов, которые играют больше 2 часов в среднем",
            SqlQuery = @"SELECT 
                            c.Nickname,
                            c.Phone,
                            COUNT(s.SessionId) as SessionsCount,
                            AVG(DATEDIFF(MINUTE, s.StartTime, ISNULL(s.EndTime, GETDATE()))) as AvgMinutesPerSession,
                            SUM(s.TotalCost) as TotalSpent
                         FROM Clients c
                         JOIN Sessions s ON c.ClientId = s.ClientId
                         GROUP BY c.ClientId, c.Nickname, c.Phone
                         HAVING AVG(DATEDIFF(MINUTE, s.StartTime, ISNULL(s.EndTime, GETDATE()))) > 120
                         ORDER BY AvgMinutesPerSession DESC",
            HasParameters = false
        },
        new AnalyticsQuery
        {
            Id = 15,
            Name = "Популярность по залам",
            Description = "Сравнивает загруженность разных залов",
            SqlQuery = @"SELECT 
                            h.Name as HallName,
                            COUNT(DISTINCT comp.ComputerId) as ComputersCount,
                            COUNT(s.SessionId) as TotalSessions,
                            SUM(s.TotalCost) as TotalRevenue,
                            AVG(DATEDIFF(MINUTE, s.StartTime, ISNULL(s.EndTime, GETDATE()))) as AvgSessionMinutes
                         FROM Halls h
                         JOIN Computers comp ON h.HallId = comp.HallId
                         LEFT JOIN Sessions s ON comp.ComputerId = s.ComputerId
                         GROUP BY h.HallId, h.Name
                         ORDER BY TotalRevenue DESC",
            HasParameters = false
        }
    };

            CmbAnalyticsQuery.ItemsSource = queries;
            CmbAnalyticsQuery.DisplayMemberPath = "Name";
            CmbAnalyticsQuery.SelectedValuePath = "Id";
            CmbAnalyticsQuery.SelectedIndex = 0;
        }

        private void CmbAnalyticsQuery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbAnalyticsQuery.SelectedItem is AnalyticsQuery query)
            {
                _currentQuery = query;

                if (query.HasParameters)
                {
                    PanelQueryParams.Visibility = Visibility.Visible;
                    BuildParameterPanel(query);
                }
                else
                {
                    PanelQueryParams.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BuildParameterPanel(AnalyticsQuery query)
        {
            QueryParamsPanel.Children.Clear();

            if (query.ParameterType == "DateRange")
            {
                var fromDatePicker = new DatePicker
                {
                    Width = 120,
                    Height = 32,
                    Margin = new Thickness(5),
                    Name = "DateFrom",
                    VerticalAlignment = VerticalAlignment.Center
                };
                fromDatePicker.SelectedDate = DateTime.Now.AddDays(-30);

                var toDatePicker = new DatePicker
                {
                    Width = 120,
                    Height = 32,
                    Margin = new Thickness(5),
                    Name = "DateTo",
                    VerticalAlignment = VerticalAlignment.Center
                };
                toDatePicker.SelectedDate = DateTime.Now;

                QueryParamsPanel.Children.Add(new TextBlock
                {
                    Text = "С:",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    FontSize = 13
                });
                QueryParamsPanel.Children.Add(fromDatePicker);
                QueryParamsPanel.Children.Add(new TextBlock
                {
                    Text = "По:",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    FontSize = 13
                });
                QueryParamsPanel.Children.Add(toDatePicker);
            }
        }

        private void BtnExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuery == null) return;

            try
            {
                string sql = _currentQuery.SqlQuery;

                if (_currentQuery.HasParameters && _currentQuery.ParameterType == "DateRange" && QueryParamsPanel.Children.Count >= 4)
                {
                    var dateFromPicker = QueryParamsPanel.Children[1] as DatePicker;
                    var dateToPicker = QueryParamsPanel.Children[3] as DatePicker;

                    var dateFrom = dateFromPicker?.SelectedDate ?? DateTime.Now.AddDays(-30);
                    var dateTo = dateToPicker?.SelectedDate ?? DateTime.Now;

                    sql = sql.Replace("@DateFrom", $"'{dateFrom:yyyy-MM-dd}'");
                    sql = sql.Replace("@DateTo", $"'{dateTo:yyyy-MM-dd}'");
                }

                var dataTable = new System.Data.DataTable();
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = System.Data.CommandType.Text;

                    if (_db.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                        _db.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }

                    if (_db.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        _db.Database.CloseConnection();
                }

                var columnsToRemove = new List<string>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    string colNameLower = column.ColumnName.ToLower();
                    if (colNameLower.EndsWith("id") || colNameLower == "id" ||
                        colNameLower.Contains("clientid") || colNameLower.Contains("computerid") ||
                        colNameLower.Contains("gameid") || colNameLower.Contains("employeeid") ||
                        colNameLower.Contains("sessionid") || colNameLower.Contains("orderid") ||
                        colNameLower.Contains("productid") || colNameLower.Contains("tariffid") ||
                        colNameLower.Contains("hallid") || colNameLower.Contains("rankid") ||
                        colNameLower.Contains("paymentmethodid") || colNameLower.Contains("positionid") ||
                        colNameLower.Contains("categoryid"))
                    {
                        columnsToRemove.Add(column.ColumnName);
                    }
                }

                foreach (var colName in columnsToRemove)
                {
                    dataTable.Columns.Remove(colName);
                }

                foreach (DataColumn column in dataTable.Columns)
                {
                    column.ColumnName = GetRussianColumnName(column.ColumnName);
                }

                DgAnalyticsResult.ItemsSource = dataTable.DefaultView;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    for (int i = 0; i < DgAnalyticsResult.Items.Count; i++)
                    {
                        var row = DgAnalyticsResult.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                        if (row != null)
                            row.Header = (i + 1).ToString();
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);

                MessageBox.Show($"Запрос выполнен успешно! Найдено записей: {dataTable.Rows.Count}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetRussianColumnName(string englishName)
        {
            var translations = new Dictionary<string, string>
    {
        {"Nickname", "Никнейм"},
        {"Phone", "Телефон"},
        {"Balance", "Баланс"},
        {"RankName", "Ранг"},
        {"DiscountPercent", "Скидка %"},
        {"Title", "Название"},
        {"Developer", "Разработчик"},
        {"ComputersCount", "Кол-во ПК"},
        {"Date", "Дата"},
        {"SessionsCount", "Кол-во сессий"},
        {"SessionsRevenue", "Выручка с сессий"},
        {"OrdersCount", "Кол-во заказов"},
        {"OrdersRevenue", "Выручка с заказов"},
        {"TotalRevenue", "Общая выручка"},
        {"ComputerName", "Компьютер"},
        {"HallName", "Зал"},
        {"TotalMinutes", "Всего минут"},
        {"CurrentSessionMinutes", "Текущая сессия (мин)"},
        {"AvgSessionMinutes", "Средняя длительность"},
        {"FullName", "ФИО"},
        {"Position", "Должность"},
        {"TariffName", "Тариф"},
        {"AvgRevenuePerSession", "Средний чек"},
        {"ProductName", "Товар"},
        {"CategoryName", "Категория"},
        {"TotalQuantity", "Продано шт."},
        {"AvgPrice", "Средняя цена"},
        {"MethodName", "Метод оплаты"},
        {"Hour", "Час"},
        {"AvgRevenue", "Средняя выручка"},
        {"Year", "Год"},
        {"Month", "Месяц"},
        {"NewClientsCount", "Новых клиентов"},
        {"ClientName", "Клиент"},
        {"EmployeeName", "Сотрудник"},
        {"StartTime", "Время начала"},
        {"DurationMinutes", "Длительность (мин)"},
        {"TotalCost", "Стоимость"},
        {"TotalSpent", "Всего потрачено"},
        {"AvgSpent", "Средний чек"},
        {"AvgMinutesPerSession", "Средняя длительность (мин)"}
    };

            return translations.ContainsKey(englishName) ? translations[englishName] : englishName;
        }

        private void DgAnalyticsResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void DgAnalyticsResult_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string header = e.Column.Header.ToString() ?? "";

            if (header.EndsWith("ID") || header.EndsWith("Id") ||
                header == "ID" || header == "Id" ||
                header.Contains("ClientId") || header.Contains("ComputerId") ||
                header.Contains("GameId") || header.Contains("EmployeeId") ||
                header.Contains("SessionId") || header.Contains("OrderId") ||
                header.Contains("ProductId") || header.Contains("TariffId") ||
                header.Contains("HallId") || header.Contains("RankId") ||
                header.Contains("PaymentMethodId") || header.Contains("PositionId") ||
                header.Contains("CategoryId"))
            {
                e.Column.Visibility = Visibility.Collapsed;
                return;
            }

            if (header.Contains("Никнейм") || header.Contains("ФИО") ||
                header.Contains("Клиент") || header.Contains("Сотрудник") ||
                header.Contains("Название") || header.Contains("Товар") ||
                header.Contains("Компьютер") || header.Contains("Игра") ||
                header.Contains("Разработчик") || header.Contains("Ранг") ||
                header.Contains("Должность") || header.Contains("Категория") ||
                header.Contains("Зал") || header.Contains("Метод") ||
                header.Contains("Комментарий") || header.Contains("Описание"))
            {
                e.Column.Width = new DataGridLength(2.5, DataGridLengthUnitType.Star);
            }
            else if (header.Contains("Дата") || header.Contains("Время") ||
                     header.Contains("Начало") || header.Contains("Окончание") ||
                     header.Contains("Start") || header.Contains("End"))
            {
                e.Column.Width = new DataGridLength(1.2, DataGridLengthUnitType.Star);
            }
            else if (header.Contains("Цена") || header.Contains("Стоимость") ||
                     header.Contains("Выручка") || header.Contains("Баланс") ||
                     header.Contains("Сумма") || header.Contains("Чек") ||
                     header.Contains("Revenue") || header.Contains("Cost") ||
                     header.Contains("Потрачено") || header.Contains("Spent"))
            {
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                if (e.Column is DataGridTextColumn textColumn && textColumn.Binding is System.Windows.Data.Binding binding)
                {
                    binding.StringFormat = "{0:F2}";
                }
            }
            else if (header.Contains("Кол-во") || header.Contains("Количество") ||
                     header.Contains("Час") || header.Contains("Год") ||
                     header.Contains("Месяц") || header.Contains("Количество") ||
                     header.Contains("Длительность") || header.Contains("Минут") ||
                     header.Contains("Minutes") || header.Contains("Count") ||
                     header.Contains("Скидка") || header.Contains("Discount"))
            {
                e.Column.Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            }
            else
            {
                e.Column.Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            }

            e.Column.MinWidth = 120;

            if (e.Column is DataGridTextColumn textCol && textCol.ElementStyle == null)
            {
                var style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.NoWrap));
                style.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
                textCol.ElementStyle = style;
            }
        }

        private void DgAnalyticsResult_AutoGeneratedColumns(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var column in DgAnalyticsResult.Columns)
                {
                    if (column.Visibility == Visibility.Visible)
                    {
                        string originalHeader = column.Header.ToString() ?? "";
                        if (originalHeader.Length > 20)
                        {
                            int breakPoint = originalHeader.Length > 30 ? 20 : 15;
                            if (breakPoint < originalHeader.Length)
                            {
                                int spaceIndex = originalHeader.LastIndexOf(' ', breakPoint);
                                if (spaceIndex > 0)
                                {
                                    column.Header = originalHeader.Insert(spaceIndex, Environment.NewLine);
                                }
                            }
                        }
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void CmbSessionComputer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSessionComputer.SelectedValue == null) return;

            var computer = _db.Computers.Find((int)CmbSessionComputer.SelectedValue);
            if (computer != null)
            {
                var tariffsForHall = _db.Tariffs.Where(t => t.HallId == computer.HallId).ToList();
                CmbSessionTariff.ItemsSource = tariffsForHall;
                CmbSessionTariff.DisplayMemberPath = "TariffName";
                CmbSessionTariff.SelectedValuePath = "TariffId";
                CmbSessionTariff.SelectedIndex = -1;
            }
        }




        private void SelectTabByHeader(string tabHeader)
        {
            try
            {
                foreach (TabItem tab in MainTabControl.Items)
                {
                    if (tab.Header.ToString()?.Contains(tabHeader) == true)
                    {
                        tab.IsSelected = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при выборе вкладки {tabHeader}: {ex.Message}");
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeWindow_Click(sender, e);
            }
            else
            {
                this.DragMove();
            }
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}