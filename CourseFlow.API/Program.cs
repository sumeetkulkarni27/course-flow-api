
using CourseFlow.API.Filters;
using CourseFlow.Application;
using CourseFlow.Application.DTOValidations;
using CourseFlow.Application.Interfaces.Courses;
using CourseFlow.Application.Interfaces.QuestionsChoice;
using CourseFlow.Application.Services;
using CourseFlow.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>(); // Add your custom validation filter
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // Disable automatic validation
            })
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
               });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();

            // Register all FluentValidation validators from the Application assembly
            builder.Services.AddValidatorsFromAssemblyContaining<CreateCourseValidator>();

            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IChoiceService, ChoiceService>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            #endregion Configurating Services - End

            #region Configurating Middleware

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseCors("default");

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
