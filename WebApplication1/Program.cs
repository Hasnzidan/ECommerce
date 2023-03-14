using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


      
    }
    public static class SessionExtensions
        {
            public static void Set<T>(this ISession session, string key, T value)
            {
                session.Set(key, JsonSerializer.SerializeToUtf8Bytes(value));
            }

            public static T Get<T>(this ISession session, string key)
            {
                var value = session.Get(key);

                return value == null ? default(T) :
                    JsonSerializer.Deserialize<T>(value);
            }
        }
}



