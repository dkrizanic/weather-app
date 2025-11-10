import React, { useState, useEffect } from 'react';
import { weatherService, CurrentWeather, ForecastDay, ForecastData } from '../services/weatherService';
import WeatherCard from '../components/WeatherCard';
import ForecastChart from '../components/ForecastChart';
import ForecastGrid from '../components/ForecastGrid';

interface SavedLocation {
  latitude: number;
  longitude: number;
  city: string;
}

const Dashboard: React.FC = () => {
  const [city, setCity] = useState('');
  const [currentWeather, setCurrentWeather] = useState<CurrentWeather | null>(null);
  const [forecastData, setForecastData] = useState<ForecastData | null>(null);
  const [filteredForecast, setFilteredForecast] = useState<ForecastDay[]>([]);
  const [loading, setLoading] = useState(true); // Start with loading true
  const [error, setError] = useState('');
  const [hasLoadedInitial, setHasLoadedInitial] = useState(false);
  
  // Filter states
  const [selectedCity, setSelectedCity] = useState('');
  const [dateFilter, setDateFilter] = useState<'all' | '3days' | '5days'>('5days');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [viewMode, setViewMode] = useState<'both' | 'grid' | 'chart'>('both');

  // Apply filters whenever forecast data or filter settings change
  useEffect(() => {
    if (!forecastData) {
      setFilteredForecast([]);
      return;
    }

    let filtered = [...forecastData.days];

    // Apply date filter
    if (dateFilter === '3days') {
      filtered = filtered.slice(0, 3);
    } else if (dateFilter === '5days') {
      filtered = filtered.slice(0, 5);
    }

    // Apply custom date range filter
    if (startDate && endDate) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      filtered = filtered.filter(day => {
        const dayDate = new Date(day.date);
        return dayDate >= start && dayDate <= end;
      });
    }

    setFilteredForecast(filtered);
  }, [forecastData, dateFilter, startDate, endDate]);

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
          setSelectedCity(weather.city);
          // Don't save to history when loading saved location
          const forecast = await weatherService.getForecast(weather.city, false);
          setForecastData(forecast);
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
      const [weather, forecast] = await Promise.all([
        weatherService.getCurrentWeather(city),
        weatherService.getForecast(city),
      ]);
      setCurrentWeather(weather);
      setForecastData(forecast);
      setSelectedCity(city);
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
            setSelectedCity(weather.city);
            // Don't save to history when getting location (it's not a deliberate search)
            const forecast = await weatherService.getForecast(weather.city, false);
            setForecastData(forecast);
            
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
      
      {forecastData && filteredForecast.length > 0 && (
        <div className="bg-white p-6 rounded-lg shadow-md">
          <div className="mb-6">
            <h2 className="text-2xl font-bold mb-4 text-gray-800">
              Weather Forecast - {selectedCity}
            </h2>
            
            {/* Filter Controls */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6 p-4 bg-gray-50 rounded-lg">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Time Period
                </label>
                <select
                  value={dateFilter}
                  onChange={(e) => {
                    setDateFilter(e.target.value as 'all' | '3days' | '5days');
                    setStartDate('');
                    setEndDate('');
                  }}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="3days">Next 3 Days</option>
                  <option value="5days">Next 5 Days</option>
                  <option value="all">All Available Days</option>
                </select>
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Start Date
                </label>
                <input
                  type="date"
                  value={startDate}
                  onChange={(e) => {
                    setStartDate(e.target.value);
                    setDateFilter('all');
                  }}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  End Date
                </label>
                <input
                  type="date"
                  value={endDate}
                  onChange={(e) => {
                    setEndDate(e.target.value);
                    setDateFilter('all');
                  }}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            {/* View Mode Toggle */}
            <div className="flex gap-2 mb-4">
              <button
                onClick={() => setViewMode('both')}
                className={`px-4 py-2 rounded-lg transition ${
                  viewMode === 'both'
                    ? 'bg-blue-500 text-white'
                    : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                }`}
              >
                Both Views
              </button>
              <button
                onClick={() => setViewMode('grid')}
                className={`px-4 py-2 rounded-lg transition ${
                  viewMode === 'grid'
                    ? 'bg-blue-500 text-white'
                    : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                }`}
              >
                Grid Only
              </button>
              <button
                onClick={() => setViewMode('chart')}
                className={`px-4 py-2 rounded-lg transition ${
                  viewMode === 'chart'
                    ? 'bg-blue-500 text-white'
                    : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                }`}
              >
                Chart Only
              </button>
            </div>

            <p className="text-sm text-gray-600">
              Showing {filteredForecast.length} day(s) of forecast data
            </p>
          </div>

          {/* Grid View */}
          {(viewMode === 'both' || viewMode === 'grid') && (
            <div className="mb-6">
              <h3 className="text-xl font-semibold mb-3 text-gray-700">Data Grid</h3>
              <ForecastGrid forecast={filteredForecast} />
            </div>
          )}

          {/* Chart View */}
          {(viewMode === 'both' || viewMode === 'chart') && (
            <div>
              <h3 className="text-xl font-semibold mb-3 text-gray-700">Data Visualization</h3>
              <ForecastChart forecast={filteredForecast} />
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default Dashboard;
