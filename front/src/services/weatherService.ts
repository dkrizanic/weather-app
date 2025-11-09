import api from './api';

export interface CurrentWeather {
  city: string;
  country: string;
  temperature: number;
  feelsLike: number;
  description: string;
  humidity: number;
  windSpeed: number;
  icon: string;
}

export interface ForecastDay {
  date: string;
  temp: number;
  tempMin: number;
  tempMax: number;
  description: string;
  condition: string;
  humidity: number;
  windSpeed: number;
  precipitation: number;
  icon: string;
}

export interface ForecastData {
  city: string;
  country: string;
  days: ForecastDay[];
}

export interface SearchHistory {
  id: number;
  city: string;
  country: string;
  searchDate: string;
  weatherCondition: string;
  temperature: number;
  description: string;
}

export interface Statistics {
  topCities: Array<{ city: string; searchCount: number }>;
  recentSearches: SearchHistory[];
  weatherDistribution: Array<{ condition: string; count: number }>;
}

export const weatherService = {
  async getCurrentWeather(city: string): Promise<CurrentWeather> {
    const response = await api.get(`/weather/current/${city}`);
    return response.data;
  },

  async getCurrentWeatherByCoordinates(lat: number, lon: number): Promise<CurrentWeather> {
    const response = await api.get(`/weather/current/coordinates?lat=${lat}&lon=${lon}`);
    return response.data;
  },

  async getForecast(city: string): Promise<ForecastData> {
    const response = await api.post('/weather/forecast', { city });
    return response.data;
  },

  async getHistory(): Promise<SearchHistory[]> {
    const response = await api.get('/weather/history');
    return response.data;
  },

  async getStatistics(): Promise<Statistics> {
    const response = await api.get('/weather/statistics');
    return response.data;
  },
};
