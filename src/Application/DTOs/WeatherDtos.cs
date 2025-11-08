namespace Application.DTOs;

public class CurrentWeatherDto
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public DateTime DateTime { get; set; }
}

public class ForecastDayDto
{
    public DateTime Date { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public double Temp { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double Precipitation { get; set; }
}

public class ForecastDto
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<ForecastDayDto> Days { get; set; } = new();
}

public class WeatherSearchDto
{
    public string City { get; set; } = string.Empty;
    public string Period { get; set; } = "5days";
}
