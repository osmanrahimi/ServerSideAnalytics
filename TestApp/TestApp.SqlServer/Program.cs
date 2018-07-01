using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TestApp.SqlServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var a = 'A';
            var d = new List<char>();

            for (int i = 0; i < 15; i++)
            {
                d.Add(a++);
            }

            var q = d.Select(x => 
                        {
                            var row = $"public byte {x} {{ get; set; }} \r\n\r\n public byte {x}2 {{ get; set; }}\r\n\r\n";
                            return row;
                        });

            var tex = string.Join("", q);


            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
