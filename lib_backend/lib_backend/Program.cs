using lib_backend.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace lib_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalFunc.InitializeConnStr();
            GlobalFunc.InitializeSecret();
            GlobalFunc.RefreshBookState();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}