using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public SearchHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SearchHistory> CreateAsync(SearchHistory searchHistory)
    {
        _context.SearchHistories.Add(searchHistory);
        await _context.SaveChangesAsync();
        return searchHistory;
    }

    public async Task<IEnumerable<SearchHistory>> GetByUserIdAsync(string userId)
    {
        return await _context.SearchHistories
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.SearchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SearchHistory>> GetRecentByUserIdAsync(string userId, int count)
    {
        return await _context.SearchHistories
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.SearchDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetTopCitiesByUserIdAsync(string userId, int count)
    {
        return await _context.SearchHistories
            .Where(sh => sh.UserId == userId)
            .GroupBy(sh => sh.City)
            .Select(g => new { City = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToDictionaryAsync(x => x.City, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetWeatherConditionDistributionAsync(string userId)
    {
        return await _context.SearchHistories
            .Where(sh => sh.UserId == userId && !string.IsNullOrEmpty(sh.WeatherCondition))
            .GroupBy(sh => sh.WeatherCondition)
            .Select(g => new { Condition = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToDictionaryAsync(x => x.Condition, x => x.Count);
    }
}
