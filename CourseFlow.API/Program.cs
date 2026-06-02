
using CourseFlow.API.Filters;
using CourseFlow.API.Middlewares;
using CourseFlow.Application;
using CourseFlow.Application.DTOValidations;
using CourseFlow.Application.Interfaces.Courses;
using CourseFlow.Application.Interfaces.QuestionsChoice;
using CourseFlow.Application.Services;
using CourseFlow.Infrastructure;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Templates;
using System.Net;
using System.Text.Json.Serialization;

namespace CourseFlow.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // Configure Serilog with the settings
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .CreateBootstrapLogger();

            try
            {


                #region Configurating Services
                var builder = WebApplication.CreateBuilder(args);


                builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console(new ExpressionTemplate(
            "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}")));

                Log.Information("Starting the CourseFlow API...");


                // Add services to the container.
                builder.Services.AddDbContext<CourseFlowContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("DbContext")));

                builder.Services.AddControllers(options =>
                {
                    options.Filters.Add<ValidationFilter>(); // Add your custom validation filter
                    options.Filters.Add<GlobalExceptionFilter>();
                }).ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true; // Disable automatic validation
                })
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                 });

                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();

                builder.Services.AddScoped<ICourseRepository, CourseRepository>();
                builder.Services.AddScoped<ICourseService, CourseService>();

                builder.Services.AddTransient<RequestBodyLoggingMiddleware>();
                builder.Services.AddTransient<ResponseBodyLoggingMiddleware>();


                builder.Services.AddScoped<ICourseRepository, CourseRepository>();
                builder.Services.AddScoped<ICourseService, CourseService>();
                builder.Services.AddHttpContextAccessor();

                // Register all FluentValidation validators from the Application assembly
                builder.Services.AddValidatorsFromAssemblyContaining<CreateCourseValidator>();

                builder.Services.AddScoped<IQuestionService, QuestionService>();
                builder.Services.AddScoped<IChoiceService, ChoiceService>();
                builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
                builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();
                builder.Services.AddAutoMapper(typeof(MappingProfile));

                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddMicrosoftIdentityWebApi(options =>
      {
          builder.Configuration.Bind("EntraIdConfig", options);

          options.Events = new JwtBearerEvents
          {

              OnTokenValidated = context =>
              {
                  var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                  // Access the scope claim (scp) directly
                  var scopeClaim = context.Principal?.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;

                  if (scopeClaim != null)
                  {
                      logger.LogInformation("Scope found in token: {Scope}", scopeClaim);
                  }
                  else
                  {
                      logger.LogWarning("Scope claim not found in token.");
                  }


                  return Task.CompletedTask;
              },
              OnAuthenticationFailed = context =>
              {
                  var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                  logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                  return Task.CompletedTask;
              },
              OnChallenge = context =>
              {
                  var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                  logger.LogError("Challenge error: {ErrorDescription}", context.ErrorDescription);
                  return Task.CompletedTask;
              }
          };
      }, options => { builder.Configuration.Bind("EntraIdConfig", options); });

                // The following flag can be used to get more descriptive errors in development environments
                IdentityModelEventSource.ShowPII = true;

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

                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        Log.Error(exception, "Unhandled exception occurred. {ExceptionDetails}", exception?.ToString());
                        Console.WriteLine(exception?.ToString());
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
                    });
                });


                app.UseMiddleware<RequestResponseLoggingMiddleware>();
                app.UseMiddleware<RequestBodyLoggingMiddleware>();
                app.UseMiddleware<ResponseBodyLoggingMiddleware>();


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
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            #endregion Configurating Middleware - End

        }
    }
}
