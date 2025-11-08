import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { ForecastDay } from '../services/weatherService';

interface ForecastChartProps {
  forecast: ForecastDay[];
}

const ForecastChart: React.FC<ForecastChartProps> = ({ forecast }) => {
  const data = forecast.map((day) => ({
    date: new Date(day.date).toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' }),
    temperature: day.temperature,
    feelsLike: day.feelsLike,
  }));

  return (
    <div>
      <ResponsiveContainer width="100%" height={300}>
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="date" />
          <YAxis label={{ value: 'Temperature (°C)', angle: -90, position: 'insideLeft' }} />
          <Tooltip />
          <Legend />
          <Line type="monotone" dataKey="temperature" stroke="#3b82f6" strokeWidth={2} name="Temperature" />
          <Line type="monotone" dataKey="feelsLike" stroke="#10b981" strokeWidth={2} name="Feels Like" />
        </LineChart>
      </ResponsiveContainer>

      <div className="grid grid-cols-1 md:grid-cols-5 gap-4 mt-6">
        {forecast.map((day, index) => (
          <div key={index} className="bg-gray-50 p-4 rounded-lg text-center">
            <p className="font-semibold text-gray-700">
              {new Date(day.date).toLocaleDateString('en-US', { weekday: 'short' })}
            </p>
            <img
              src={`http://openweathermap.org/img/wn/${day.icon}@2x.png`}
              alt={day.description}
              className="w-16 h-16 mx-auto"
            />
            <p className="text-2xl font-bold text-gray-800">{day.temperature}°C</p>
            <p className="text-sm text-gray-600 capitalize">{day.description}</p>
            <div className="mt-2 text-xs text-gray-500">
              <p>💧 {day.humidity}%</p>
              <p>💨 {day.windSpeed} m/s</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ForecastChart;
