# URL Shortener Platform

A production-style full-stack URL shortener built with .NET 8, PostgreSQL, Redis, JWT authentication, and React.

## Features

- User authentication with JWT
- Create short URLs
- Custom alias support
- Expiry options
- Public redirect endpoint
- Redis caching for faster redirects
- PostgreSQL database storage
- Click analytics
- QR code generation
- Delete/manage URLs
- Rate limiting
- Global error handling
- Serilog request logging
- Deployed backend and frontend

## Tech Stack

### Backend
- .NET 8 Web API
- Entity Framework Core
- PostgreSQL
- Redis
- JWT Authentication
- ASP.NET Rate Limiting
- Serilog

### Frontend
- React
- Tailwind CSS
- Vite
- Vercel

### Deployment
- Backend: Render
- Frontend: Vercel
- Database: PostgreSQL
- Cache: Redis

## Architecture Overview

The application follows a simple full-stack architecture:

- React frontend sends authenticated API requests
- .NET Web API handles authentication, URL creation, analytics, and redirects
- PostgreSQL stores users, URLs, and click data
- Redis is used as a cache layer for faster redirects
- Public short URLs redirect users to the original long URLs

## Core Flow

1. User creates a short URL.
2. Backend validates the request and generates a short code.
3. URL metadata is stored in PostgreSQL.
4. Redis is used to cache frequently accessed short URLs.
5. Public redirect endpoint resolves the short code and redirects to the original URL.

## What I Learned

- Designing REST APIs with authentication
- Handling database migrations from SQLite to PostgreSQL
- Using Redis with cache-aside strategy
- Adding rate limiting for API protection
- Deploying full-stack apps using Render and Vercel
- Debugging production deployment issues
