using System.Security;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Smart_Document_Management_System.Helpers;

namespace Smart_Document_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Mutex mutex = new Mutex(false, "Smart_Document_Management_System", out bool IsFreshInstance);
            if (!IsFreshInstance)
            {
                Console.WriteLine("Another instance of the application is already running.");
                return;
            }
            CommonMethods.Initialize();
            var builder = WebApplication.CreateBuilder(args);
            string IP = CommonMethods.GetFromConfig("Server", "IP") == "" ? "http://localhost:8000" : CommonMethods.GetFromConfig("Server", "IP");
            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();
            builder.WebHost.UseUrls(IP);
            app.UseCors(policy=>policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin=>true)
            .AllowCredentials());
            // Configure the HTTP request pipeline.
            
            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            
            app.Run();

        }
    }
}