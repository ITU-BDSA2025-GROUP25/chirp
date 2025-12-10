using System;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http;
using Chirp.Razor;

namespace Chirp.Tests;

public class WebDatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private WebApplicationFactory<Program>? _webAppFactory;
    private HttpClient? _httpClient;
    private IServiceScope? _serviceScope;
    private ChirpDbContext? _context;

    public WebDatabaseFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public HttpClient CreateHttpClient()
    {
        if (_httpClient != null)
            return _httpClient;

        _webAppFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Use our connection
                    services.AddDbContext<ChirpDbContext>(options =>
                    {
                        options.UseSqlite(_connection);
                    });
                });
            });

        _httpClient = _webAppFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });

        // Create a scope that lives as long as the fixture
        _serviceScope = _webAppFactory.Services.CreateScope();
        _context = _serviceScope.ServiceProvider.GetRequiredService<ChirpDbContext>();
        
        return _httpClient;
    }

    // Property to access the DbContext
    public ChirpDbContext Context 
    {
        get
        {
            if (_context == null)
                throw new InvalidOperationException("Call CreateHttpClient() first to initialize the DbContext");
            return _context;
        }
    }

    public void Dispose()
    {
        _serviceScope?.Dispose();
        _httpClient?.Dispose();
        _webAppFactory?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}