using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Contexts;

public static class ContextsInitializer
{
    public static async Task Initialize(params DbContext[] contexts)
    {
        foreach (var dbContext in contexts)
            await dbContext.Database.EnsureCreatedAsync();
    }
}