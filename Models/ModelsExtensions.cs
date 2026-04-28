using System.ComponentModel.DataAnnotations.Schema;

namespace computerclub.Models
{
    // 1. КЛИЕНТЫ
    public partial class Client
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 2. КОМПЬЮТЕРЫ
    public partial class Computer
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 3. ПРОДУКТЫ
    public partial class Product
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 4. СОТРУДНИКИ
    public partial class Employee
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 5. ИГРЫ
    public partial class Game
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 6. ТАРИФЫ
    public partial class Tariff
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 7. СЕССИИ
    public partial class Session
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 8. ЗАЛЫ
    public partial class Hall
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 9. РАНГИ
    public partial class ClientRank
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 10. МЕТОДЫ ОПЛАТЫ
    public partial class PaymentMethod
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 11. КАТЕГОРИИ ПРОДУКТОВ
    public partial class ProductCategory
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 12. ДОЛЖНОСТИ
    public partial class Position
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 13. ИГРЫ НА КОМПЬЮТЕРАХ
    public partial class ComputerGame
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 14. ЗАКАЗЫ
    public partial class Order
    {
        [NotMapped]
        public int RowNumber { get; set; }
    }

    // 15. ДЕТАЛИ ЗАКАЗОВ
    public partial class OrderDetail
    {
        [NotMapped]
        public int RowNumber { get; set; }

        [NotMapped]
        public decimal Total { get; set; }
    }


}