using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using UniqueVisitorsCodingChallenge.Data;

namespace UniqueVisitorsCodingChallenge
{
    class Program
    {
        private static string _connectionString;
        private static string _pathToLogFile;

        public static void GetUniqueVisitors()
        {
            StreamReader reader = new(_pathToLogFile);
            string logContent = reader.ReadToEnd();

            // Creating a regular expression for the records
            var regEx = "^(\\S+) (\\S+) (\\S+) " +
                   "\\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(\\S+)" +
                   " (\\S+)\\s*(\\S+)?\\s*\" (\\d{3}) (\\S+)";

            // Instantiate the regular expression object.
            Regex r = new(regEx, RegexOptions.Multiline);

            // Match the regular expression pattern against a text string.
            MatchCollection matches = r.Matches(logContent);
            
            // Creating a Dictionary for storing the unique visitors.
            var uniqueAccesses = new Dictionary<string, UniqueVisitor>();
            
            foreach (Match m in matches)
            {   
                var IP = m.Groups[1].ToString();
                var username = m.Groups[3].ToString();

                if (String.IsNullOrWhiteSpace(username) || username == "-")
                    continue;
                
                var timeStr = m.Groups[4].ToString();
                var time = DateTime.ParseExact(timeStr, "dd/MMM/yyyy:HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture);
                
                var responseStr = m.Groups[8].ToString();
                var response = int.Parse(responseStr);

                // for each HTTP 200 code.
                if (response == 200)
                {
                    var uniqueVisitor = new UniqueVisitor()
                    {
                        Ip = IP,
                        Username = username,
                        Time = time
                    };

                    if (!uniqueAccesses.ContainsKey(IP))
                    {
                        uniqueAccesses.Add(IP, uniqueVisitor);
                    }
                }
            }
            if (uniqueAccesses.Count > 0)
            {
                using ApplicationContext db = new(_connectionString);
                db.UniqueVisitors.AddRange(uniqueAccesses.Select(x => x.Value));
                db.SaveChanges();
            }
        }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
            _connectionString = config["ConnectionString"];
            _pathToLogFile = config["PathToLogFile"];

            if (!File.Exists(_pathToLogFile))
            {
                Console.WriteLine("Log file does not exist.");
                return;
            }

            GetUniqueVisitors();

            using ApplicationContext db = new(_connectionString);
            
            //Last 5 seconds            
            ResultToScreen("unique visitors for last 5 sec:", GetVisitorsByTime(db, DateTime.Now.AddSeconds(-5)));

            //Last minute            
            ResultToScreen("unique visitors for last minute:", GetVisitorsByTime(db, DateTime.Now.AddMinutes(-1)));

            //Last 5 minutes          
            ResultToScreen("unique visitors for last 5 minutes:", GetVisitorsByTime(db, DateTime.Now.AddMinutes(-5)));

            //Last 30 minutes          
            ResultToScreen("unique visitors for last 30 minutes:", GetVisitorsByTime(db, DateTime.Now.AddMinutes(-30)));
            
            //Last 1 hour          
            ResultToScreen("unique visitors for last 1 hour:", GetVisitorsByTime(db, DateTime.Now.AddHours(-1)));

            //Last 1 day          
            ResultToScreen("unique visitors for last 1 day:", GetVisitorsByTime(db, DateTime.Now.AddDays(-1)));

            Console.ReadKey();
        }

        private static IEnumerable<UniqueVisitor> GetVisitorsByTime(ApplicationContext db, DateTime time)
        {   
            var dtNow = DateTime.Now;
            
            var visitors = db.UniqueVisitors.Where(x => x.Time >= time
                                                    && x.Time <= dtNow).AsEnumerable();
            return visitors;
        }

        private static void ResultToScreen(string condition, IEnumerable<UniqueVisitor> uniqueVisitors)
        {
            Console.WriteLine("+------------------------------------------------------------------+");
            Console.WriteLine(condition);
            Console.WriteLine("+------------------------------------------------------------------+");
            if (uniqueVisitors.Any())
            {
                foreach (var visitor in uniqueVisitors)
                {
                    Console.WriteLine($"{visitor.Ip}, {visitor.Username}, {visitor.Time}");
                }
            }
            else Console.WriteLine("No results");
            Console.WriteLine("+------------------------------------------------------------------+");
        }
    }
}
