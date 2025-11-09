import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, BarChart, Bar } from 'recharts';
import { ForecastDay } from '../services/weatherService';

interface ForecastChartProps {
  forecast: ForecastDay[];
}

const ForecastChart: React.FC<ForecastChartProps> = ({ forecast }) => {
  const data = forecast.map((day) => ({
    date: new Date(day.date).toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' }),
    temp: Math.round(day.temp * 10) / 10,
    tempMin: Math.round(day.tempMin * 10) / 10,
    tempMax: Math.round(day.tempMax * 10) / 10,
    humidity: day.humidity,
    windSpeed: Math.round(day.windSpeed * 10) / 10,
    precipitation: Math.round(day.precipitation * 10) / 10,
  }));

  return (
    <div className="space-y-6">
      {/* Temperature Chart */}
      <div>
        <h3 className="text-lg font-semibold mb-3 text-gray-700">Temperature Trend</h3>
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={data}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" />
            <YAxis label={{ value: 'Temperature (°C)', angle: -90, position: 'insideLeft' }} />
            <Tooltip />
            <Legend />
            <Line type="monotone" dataKey="temp" stroke="#3b82f6" strokeWidth={2} name="Avg Temperature" />
            <Line type="monotone" dataKey="tempMin" stroke="#60a5fa" strokeWidth={2} name="Min Temperature" strokeDasharray="5 5" />
            <Line type="monotone" dataKey="tempMax" stroke="#ef4444" strokeWidth={2} name="Max Temperature" strokeDasharray="5 5" />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* Precipitation and Wind Chart */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <h3 className="text-lg font-semibold mb-3 text-gray-700">Precipitation</h3>
          <ResponsiveContainer width="100%" height={200}>
            <BarChart data={data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" />
              <YAxis label={{ value: 'mm', angle: -90, position: 'insideLeft' }} />
              <Tooltip />
              <Bar dataKey="precipitation" fill="#10b981" name="Precipitation (mm)" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div>
          <h3 className="text-lg font-semibold mb-3 text-gray-700">Wind Speed</h3>
          <ResponsiveContainer width="100%" height={200}>
            <BarChart data={data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" />
              <YAxis label={{ value: 'm/s', angle: -90, position: 'insideLeft' }} />
              <Tooltip />
              <Bar dataKey="windSpeed" fill="#f59e0b" name="Wind Speed (m/s)" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Daily Cards */}
      <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
        {forecast.map((day, index) => (
          <div key={index} className="bg-gray-50 p-4 rounded-lg text-center border border-gray-200">
            <p className="font-semibold text-gray-700">
              {new Date(day.date).toLocaleDateString('en-US', { weekday: 'short' })}
            </p>
            <img
              src={`http://openweathermap.org/img/wn/${day.icon}@2x.png`}
              alt={day.description}
              className="w-16 h-16 mx-auto"
            />
            <p className="text-2xl font-bold text-gray-800">{day.temp.toFixed(1)}°C</p>
            <p className="text-xs text-gray-600">
              {day.tempMin.toFixed(1)}° / {day.tempMax.toFixed(1)}°
            </p>
            <p className="text-sm text-gray-600 capitalize mt-1">{day.description}</p>
            <div className="mt-2 text-xs text-gray-500 space-y-1">
              <p>💧 {day.humidity}%</p>
              <p>💨 {day.windSpeed.toFixed(1)} m/s</p>
              <p>🌧️ {day.precipitation.toFixed(1)} mm</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ForecastChart;
