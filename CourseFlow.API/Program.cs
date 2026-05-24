
using CourseFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CourseFlow.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Configurating Services
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<CourseFlowContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DbContext")));

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            #endregion Configurating Services - End

            #region Configurating Middleware
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUi(options =>
                {
                    options.DocumentPath = "openapi/v1.json";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            #endregion Configurating Middleware - End

        }
    }
}
