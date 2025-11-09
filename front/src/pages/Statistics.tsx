import React, { useState, useEffect } from 'react';
import { BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { weatherService, Statistics as StatsType } from '../services/weatherService';

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];

const Statistics: React.FC = () => {
  const [stats, setStats] = useState<StatsType | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const data = await weatherService.getStatistics();
        setStats(data);
      } catch (err: any) {
        setError('Failed to load statistics');
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  if (loading) {
    return (
      <div className="text-center py-8">
        <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
      </div>
    );
  }

  if (error || !stats) {
    return (
      <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
        {error}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Top Cities */}
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h3 className="text-xl font-bold mb-4 text-gray-800">Top 3 Searched Cities</h3>
        {stats.topCities && stats.topCities.length > 0 ? (
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={stats.topCities} margin={{ top: 20, right: 30, left: 20, bottom: 5 }}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis 
                dataKey="city" 
                style={{ fontSize: '14px', fontWeight: 'bold' }}
              />
              <YAxis 
                allowDecimals={false}
              />
              <Tooltip 
                contentStyle={{ backgroundColor: '#f3f4f6', borderRadius: '8px' }}
                labelStyle={{ fontWeight: 'bold' }}
              />
              <Legend />
              <Bar 
                dataKey="searchCount" 
                fill="#3b82f6" 
                name="Searches"
                label={{ position: 'top', style: { fontWeight: 'bold', fontSize: '16px', fill: '#1f2937' } }}
                radius={[8, 8, 0, 0]}
              >
                {stats.topCities.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        ) : (
          <div className="text-center py-8 text-gray-500">
            <p>No search data available yet. Start searching for cities to see statistics!</p>
          </div>
        )}
      </div>

      {/* Weather Distribution */}
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h3 className="text-xl font-bold mb-4 text-gray-800">Weather Conditions Distribution</h3>
        {stats.weatherDistribution && stats.weatherDistribution.length > 0 ? (
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={stats.weatherDistribution}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ condition, percent }) => `${condition}: ${(percent * 100).toFixed(0)}%`}
                outerRadius={100}
                fill="#8884d8"
                dataKey="count"
                nameKey="condition"
              >
                {stats.weatherDistribution.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        ) : (
          <div className="text-center py-8 text-gray-500">
            <p>No weather condition data available yet.</p>
          </div>
        )}
      </div>

      {/* Recent Searches */}
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h3 className="text-xl font-bold mb-4 text-gray-800">Recent Searches</h3>
        {stats.recentSearches && stats.recentSearches.length > 0 ? (
          <div className="space-y-3">
            {stats.recentSearches.map((search) => (
              <div key={search.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                <div>
                  <p className="font-semibold text-gray-800">{search.city}, {search.country}</p>
                  <p className="text-sm text-gray-500">{new Date(search.searchDate).toLocaleString()}</p>
                </div>
                <div className="text-right">
                  <p className="text-2xl font-bold text-blue-600">{search.temperature}°C</p>
                  <p className="text-sm text-gray-600 capitalize">{search.description}</p>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div className="text-center py-8 text-gray-500">
            <p>No recent searches yet.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default Statistics;
