using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3_1_1
{
    //public class ShoppingCartService
    //{
    //    public decimal CalculateTotalPrice(string customerType, List<decimal> itemPrices)
    //    {
    //        decimal baseTotal = 0;
    //        for (int i = 0; i < itemPrices.Count; i++)
    //        {
    //            baseTotal += itemPrices[i]; //Нарушение DRY (коммент в строке 51)
    //        }

    //        decimal discount = 0;

    //        if (customerType == "Regular")
    //        {
    //            discount = baseTotal * 0.05m; // 5%
    //        }
    //        else if (customerType == "Premium") // Нарушение KISS - на начальном этапе используется скидка только для клиентов "Regular"
    //        {
    //            discount = baseTotal * 0.15m; // 15%
    //            if (discount > 1000)
    //            {
    //                discount = 1000 + (discount - 1000) * 0.1m;
    //            }
    //        }
    //        else if (customerType == "VIP") // Нарушение KISS - на начальном этапе используется скидка только для клиентов "Regular"
    //        {
    //            discount = baseTotal * 0.20m; // 20%
    //        }

    //        decimal finalPrice = baseTotal - discount;

    //        Console.WriteLine($"Base: {baseTotal}, Discount: {discount}, Final: {finalPrice}");
    //        return finalPrice;
    //    }

    //    public decimal CalculateTotalPriceWithQuantities(string customerType, Dictionary<decimal, int> itemsWithQuantities) //Нарушение YAGNI - нет необходимости расписывать каждую цену
    //    {
    //        List<decimal> allPrices = new List<decimal>();
    //        foreach (var item in itemsWithQuantities)
    //        {
    //            for (int i = 0; i < item.Value; i++)
    //            {
    //                allPrices.Add(item.Key);  //Нарушение DRY - рассчет базовой суммы дублируется в двух методах (как в строке 14-17)
    //            }
    //        }
    //        return CalculateTotalPrice(customerType, allPrices);
    //    }
    //}


    //ПЕРЕПИСАННЫЙ КОД
    public class ShoppingCartService
    {
        public decimal CalculateTotalPrice(string customerType, List<decimal> itemPrices)
        {

            decimal baseTotal = CalculateBaseTotal(itemPrices);

            decimal discount = CalculateDiscount(customerType, baseTotal);

            decimal finalPrice = baseTotal - discount;

            return finalPrice;
        }

        private decimal CalculateBaseTotal(List<decimal> itemPrices) //метод для расчета общей суммы
        {
            return itemPrices.Sum();
        }
        private decimal CalculateDiscount(string customerType, decimal baseTotal) // метод для расчета скидки (с константой)
        {
            const decimal discountSize = 0.05m;
            if (customerType == "Regular")
                return baseTotal * discountSize;

            return 0m;
        }
    }
}
