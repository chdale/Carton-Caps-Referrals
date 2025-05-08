using CartonCapsAPI;
using CartonCapsAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartonCapsAPITest
{
    public class WeatherForecastControllerTest
    {
        ILogger _logger;
        WeatherForecastController _controller;

        public  WeatherForecastControllerTest()
        {
            _logger = Mock.Of<ILogger<WeatherForecastController>>();
            _controller = new WeatherForecastController((ILogger<WeatherForecastController>)_logger);
        }

        [Fact]
        public void Get_WeatherForecast_Success()
        {
            //Arrange

            //Act
            var result = _controller.Get();
            var resultType = result as OkObjectResult;
            var resultValue = resultType.Value as IEnumerable<WeatherForecast>;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(resultValue);
            Assert.NotEmpty(resultValue);
        }
    }
}