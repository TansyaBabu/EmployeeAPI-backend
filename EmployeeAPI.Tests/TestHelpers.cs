using EmployeeAPI.Data;
using Microsoft.EntityFrameworkCore;

public static class TestHelpers
{
    public static AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
