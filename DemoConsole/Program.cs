using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Console;

namespace DemoConsole
{
    record Customer(string Name, DateTime BirthDate)
    {
        public bool IsAdult => BirthDate.AddYears(18) >= DateTime.Today;
    }

    record Order(int OrderId, string CustomerName, decimal Amount);

    class Program
    {
        static void Main(string[] args)
        {
            var customers = ReadCustomers(); // 50 customers
            var orders = GeneraterandomOrders(customers, 10_000);

            var sw = Stopwatch.StartNew();

            var ofuac = GetOrdersForUnderAgeCustomers(customers, orders);

            //var sapc = GetSpentAmountPerCustomer(customers, orders);
            //var cwo = GetCustomersWithoutOrder(customers, orders);

            WriteLine("{0:##,###.000} ms", sw.Elapsed.TotalMilliseconds);

             Time();
        }


        private static IList<Order> GetOrdersForUnderAgeCustomers(List<Customer> customers, List<Order> orders)
        {
            // O (n x m)
            return orders.Where(o => !customers.FirstOrDefault(customers => customers.Name == o.CustomerName)
                                            .IsAdult).ToList();
        }

        private static IList<Order> GetOrdersForUnderAgeCustomersDictionary(List<Customer> customers, List<Order> orders)
        {
            // O (n + m)
            var customersByNameDictionary = customers.ToDictionary(c => c.Name);

            return orders.Where(o => customersByNameDictionary[o.CustomerName].IsAdult).ToList();
        }

        private static IList<Order> GetOrdersForUnderAgeCustomersHashSet(List<Customer> customers, List<Order> orders)
        {
            var underAgedCustomerNames = customers.Where(c => !c.IsAdult).Select(c => c.Name).ToHashSet();

            return orders.Where(o => underAgedCustomerNames.Contains(o.CustomerName)).ToList();
        }


        private static IList<(string Name, decimal)> GetSpentAmountPerCustomer(List<Customer> customers, List<Order> orders)
        {
            return customers.Select(c =>
                (
                    c.Name,
                    orders.Where(o=>o.CustomerName == c.Name).Sum(o => o.Amount))
                ).ToList();
        }

        private static List<(string Name, decimal)> GetSpentAmountPerCustomerLookup(List<Customer> customers, List<Order> orders)
        {
            // Lookup
            var ordersByCustomerNameLookup = orders.ToLookup(o => o.CustomerName);
            return customers.Select(c =>
                (
                    c.Name,
                    ordersByCustomerNameLookup[c.Name].Sum(o => o.Amount))
                ).ToList();
        }

        private static IList<Customer> GetCustomersWithoutOrder(List<Customer> customers, List<Order> orders)
        {
            return customers.Where(c => !orders.Any(orders=>orders.CustomerName == c.Name)).ToList();
        }


        private static IList<Customer> GetCustomersWithoutOrderHashSet(List<Customer> customers, List<Order> orders)
        {
             var customerNamesInOrders = orders.Select(o => o.CustomerName).ToHashSet();
            return customers.Where(c => !customerNamesInOrders.Contains(c.Name)).ToList();
         }


        public static List<Customer> ReadCustomers() =>
            File.ReadLines(@"Customers.txt")
            .Select(l => l.Split(",")).Select(f => new Customer(f[0].Trim(), DateTime.Parse(f[1])))
            .ToList();

        static List<Order> GeneraterandomOrders(List<Customer> customers, int count)
        {
            var result = new List<Order>();
            var random = new Random();

            return Enumerable.Range(0, count)
                .Select(i => new Order(i, customers[random.Next(0, customers.Count - 1)].Name, random.Next(1, 200)))
                .ToList();
        }

        static void Time()
        {
            var customers = ReadCustomers(); // 50 customers
            var orders = GeneraterandomOrders(customers, 100_000);

            WriteLine();
            var sw = Stopwatch.StartNew();
            sw.Restart();
            var y1 = GetOrdersForUnderAgeCustomers(customers, orders);
            WriteLine("GetOrdersForUnderAgeCustomers           " + sw.ElapsedTicks);

            sw.Restart();
            var y2 = GetOrdersForUnderAgeCustomersDictionary(customers, orders);
            WriteLine("GetOrdersForUnderAgeCustomersDictionary " + sw.ElapsedTicks);

            sw.Restart();
            var y3 = GetOrdersForUnderAgeCustomersHashSet(customers, orders);
            WriteLine("GetOrdersForUnderAgeCustomersHashSet    " + sw.ElapsedTicks);
            WriteLine();


            var z1 = GetSpentAmountPerCustomer(customers, orders);
            WriteLine("GetSpentAmountPerCustomer               " + sw.ElapsedTicks);

            sw.Restart();
            var z2 = GetSpentAmountPerCustomerLookup(customers, orders);
            WriteLine("GetSpentAmountPerCustomerLookup         " + sw.ElapsedTicks);
            WriteLine();

            sw.Restart();
            var x1 = GetCustomersWithoutOrder(customers, orders);
            WriteLine("GetCustomersWithoutOrder                " + sw.ElapsedTicks);

            var x2 = GetCustomersWithoutOrderHashSet(customers, orders);
            WriteLine("GetCustomersWithoutOrderHashSet         " + sw.ElapsedTicks);
            WriteLine();
        }
    }
}
