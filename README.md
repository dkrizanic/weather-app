# Weather App

Weather forecast app with JWT auth, search history, and stats. Built with React, TypeScript, .NET 9 (DDD), PostgreSQL, EF Core, and OpenWeather API.

## Tech Stack
**Frontend:** React 18 • TypeScript • Vite • Tailwind CSS • Recharts • Axios • React Router
**Backend:** .NET 9 • PostgreSQL 16 • EF Core • JWT • BCrypt • OpenWeather API

## Quick Start
Prereqs: Docker, Docker Compose, OpenWeather API key

```bash
git clone https://github.com/dkrizanic/weather-app.git
cd weather-app
cp .env.example .env    # add OPENWEATHER_API_KEY
docker-compose up --build
```
Frontend: http://localhost:3000  
API: http://localhost:5000

## API
Auth: POST /api/auth/register, POST /api/auth/login
Weather (JWT):
- GET /api/weather/current/{city}
- GET /api/weather/current/coordinates?lat={lat}&lon={lon}
- POST /api/weather/forecast
- GET /api/weather/history
- GET /api/weather/statistics

## License
MIT
