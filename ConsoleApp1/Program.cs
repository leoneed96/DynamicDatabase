using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataContext = GetDataContext();

            dynamic database = new DynamicDatabase(dataContext);

            Console.WriteLine("---database.DemoObjects.SearchByNumber(500)---");
            foreach (var item in database.DemoObjects.SearchByNumber(500))
            {
                Console.WriteLine("- " + JsonConvert.SerializeObject(item));
            }

            Console.WriteLine("---database.DemoObjects.SearchByTitle(DemoObject 99)---");
            foreach (var item in database.DemoObjects.SearchByTitle("DemoObject 99"))
            {
                Console.WriteLine("- " + JsonConvert.SerializeObject(item));
            }
        }

        static DataContext GetDataContext()
        {
            var jSettings = File.ReadAllText("appsettings.json");
            var j = JObject.Parse(jSettings);
            var connectionString = j["ConnectionStrings"][nameof(DataContext)];
            return new DataContext(connectionString.Value<string>());
        }
    }

}
