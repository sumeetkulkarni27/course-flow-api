using CourseFlow.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly CourseFlowContext _courseFlowContext;
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

       public  WeatherForecastController(CourseFlowContext _courseFlowContext)
        {
            this._courseFlowContext = _courseFlowContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();

             var model  = _courseFlowContext.UserProfiles.ToList();
            return Ok(model);
        }
    }
}
