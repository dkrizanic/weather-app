import React, { useState, useEffect } from 'react';
import { weatherService, CurrentWeather, ForecastDay } from '../services/weatherService';
import WeatherCard from '../components/WeatherCard';
import ForecastChart from '../components/ForecastChart';

interface SavedLocation {
  latitude: number;
  longitude: number;
  city: string;
}

const Dashboard: React.FC = () => {
  const [city, setCity] = useState('');
  const [currentWeather, setCurrentWeather] = useState<CurrentWeather | null>(null);
  const [forecast, setForecast] = useState<ForecastDay[]>([]);
  const [loading, setLoading] = useState(true); // Start with loading true
  const [error, setError] = useState('');
  const [hasLoadedInitial, setHasLoadedInitial] = useState(false);

  // Load saved location on component mount
  useEffect(() => {
    const loadSavedLocation = async () => {
      const savedLocationStr = localStorage.getItem('savedLocation');
      if (savedLocationStr) {
        try {
          const savedLocation: SavedLocation = JSON.parse(savedLocationStr);
          
          const weather = await weatherService.getCurrentWeatherByCoordinates(
            savedLocation.latitude,
            savedLocation.longitude
          );
          setCurrentWeather(weather);
          setCity(weather.city);
          const forecastData = await weatherService.getForecast(weather.city);
          setForecast(forecastData);
        } catch (err: any) {
          console.error('Failed to load saved location:', err);
          // Clear invalid saved location
          localStorage.removeItem('savedLocation');
        }
      }
      setLoading(false);
      setHasLoadedInitial(true);
    };

    loadSavedLocation();
  }, []);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!city.trim()) return;

    if (!hasLoadedInitial) return; // Prevent search during initial load

    setLoading(true);
    setError('');

    try {
      const [weather, forecastData] = await Promise.all([
        weatherService.getCurrentWeather(city),
        weatherService.getForecast(city),
      ]);
      setCurrentWeather(weather);
      setForecast(forecastData);
      // Clear saved location when manually searching
      localStorage.removeItem('savedLocation');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to fetch weather data');
    } finally {
      setLoading(false);
    }
  };

  const handleGetLocation = () => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        async (position) => {
          setLoading(true);
          setError('');
          try {
            const weather = await weatherService.getCurrentWeatherByCoordinates(
              position.coords.latitude,
              position.coords.longitude
            );
            setCurrentWeather(weather);
            setCity(weather.city);
            const forecastData = await weatherService.getForecast(weather.city);
            setForecast(forecastData);
            
            // Save location to localStorage
            const savedLocation: SavedLocation = {
              latitude: position.coords.latitude,
              longitude: position.coords.longitude,
              city: weather.city
            };
            localStorage.setItem('savedLocation', JSON.stringify(savedLocation));
          } catch (err: any) {
            setError('Failed to fetch weather data');
          } finally {
            setLoading(false);
          }
        },
        () => setError('Unable to get your location')
      );
    } else {
      setError('Geolocation is not supported by your browser');
    }
  };

  return (
    <div className="space-y-6">
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h2 className="text-2xl font-bold mb-4 text-gray-800">Search Weather</h2>
        <form onSubmit={handleSearch} className="flex gap-4">
          <input
            type="text"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            placeholder="Enter city name..."
            className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <button
            type="submit"
            disabled={loading}
            className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-lg transition disabled:opacity-50"
          >
            Search
          </button>
          <button
            type="button"
            onClick={handleGetLocation}
            className="bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded-lg transition"
          >
            Use My Location
          </button>
        </form>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {loading && (
        <div className="text-center py-8">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
        </div>
      )}

      {currentWeather && <WeatherCard weather={currentWeather} />}
      
      {forecast.length > 0 && (
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-2xl font-bold mb-4 text-gray-800">5-Day Forecast</h2>
          <ForecastChart forecast={forecast} />
        </div>
      )}
    </div>
  );
};

export default Dashboard;
