﻿using InflationArchive.Contexts;
using InflationArchive.Models.Account;
using InflationArchive.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Services;

public class JoinedService
{
    private readonly ScraperContext _context;

    public JoinedService(ScraperContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserById(Guid userId)
    {
        return await _context.Users
            .Include(static u => u.FavoriteProducts)
            .SingleAsync(u => u.Id == userId);
    }

    public async Task<Product?> GetProduct(Guid id)
    {
        return await _context.Products
            .Include(static p => p.Category)
            .Include(static p => p.Manufacturer)
            .Include(static p => p.Store)
            .Include(static p => p.ProductPrices)
            .SingleOrDefaultAsync(p => p.Id == id);
    }
}