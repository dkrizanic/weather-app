import React from 'react';
import { CurrentWeather } from '../services/weatherService';

interface WeatherCardProps {
  weather: CurrentWeather;
}

const WeatherCard: React.FC<WeatherCardProps> = ({ weather }) => {
  return (
    <div className="bg-gradient-to-br from-blue-400 to-blue-600 text-white p-8 rounded-lg shadow-lg">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-4xl font-bold">{weather.city}, {weather.country}</h3>
          <p className="text-xl mt-2 capitalize">{weather.description}</p>
        </div>
        <img
          src={`http://openweathermap.org/img/wn/${weather.icon}@4x.png`}
          alt={weather.description}
          className="w-32 h-32"
        />
      </div>
      
      <div className="mt-6 grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="bg-white/20 p-4 rounded-lg">
          <p className="text-sm opacity-80">Temperature</p>
          <p className="text-3xl font-bold">{weather.temperature}°C</p>
        </div>
        <div className="bg-white/20 p-4 rounded-lg">
          <p className="text-sm opacity-80">Feels Like</p>
          <p className="text-3xl font-bold">{weather.feelsLike}°C</p>
        </div>
        <div className="bg-white/20 p-4 rounded-lg">
          <p className="text-sm opacity-80">Humidity</p>
          <p className="text-3xl font-bold">{weather.humidity}%</p>
        </div>
        <div className="bg-white/20 p-4 rounded-lg">
          <p className="text-sm opacity-80">Wind Speed</p>
          <p className="text-3xl font-bold">{weather.windSpeed} m/s</p>
        </div>
      </div>
    </div>
  );
};

export default WeatherCard;
