using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoConsole
{
    class Sample
    {
        private static void Interfaces(List<Customer> customers, List<Order> orders)
        {

            // Hashset
            HashSet<string> hashSets = customers.Select(c => c.Name).ToHashSet();
            bool inSet = hashSets.Contains("Frank Bakker");

            // Dictionary 1 => 1
            Dictionary<string, Customer> dictionary = customers.ToDictionary(c => c.Name);
            Customer customer = dictionary["Frank Bakker"];

            // Lookup 1 => 0..*
            ILookup<string, Order> lookup = orders.ToLookup(o => o.CustomerName);
            IEnumerable<Order> enumerable = lookup["Frank Bakker"];
        }
    }
}
