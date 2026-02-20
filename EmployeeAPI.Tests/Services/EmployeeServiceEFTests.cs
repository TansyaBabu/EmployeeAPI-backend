using EmployeeAPI.Data;
using EmployeeAPI.Model;
using EmployeeAPI.Services;
using System.Threading.Tasks;
using Xunit;

public class EmployeeServiceEFTests
{
    private EmployeeService_EF GetService(out AppDbContext context)
    {
        context = TestHelpers.GetInMemoryDb();
        return new EmployeeService_EF(context);
    }

    // ------------------ TESTS BELOW ------------------

    [Fact]
    public async Task Register_ValidEmployee_ReturnsTrue()
    {
        var service = GetService(out var context);

        var emp = new Employee
        {
            Name = "John",
            Username = "john123",
            Password = "pass",
            Address = "Test Address",
            Department = "IT",
            Designation = "Developer",
            Skillset = "C#",
            CreatedBy = "admin",
            Role = "Employee",
            Status = "Active"
        };

        var result = await service.Register(emp);

        Assert.True(result);
        Assert.Single(context.Employees);
    }

    [Fact]
    public async Task GetAllEmployees_ReturnsAllRecords()
    {
        var service = GetService(out var context);

        context.Employees.Add(new Employee
        {
            Name = "A",
            Username = "a",
            Password = "1",
            Address = "Addr1",
            Department = "Dept1",
            Designation = "Des1",
            Skillset = "Skill1",
            CreatedBy = "admin",
            Role = "Employee",
            Status = "Active"
        });
        context.Employees.Add(new Employee
        {
            Name = "B",
            Username = "b",
            Password = "2",
            Address = "Addr2",
            Department = "Dept2",
            Designation = "Des2",
            Skillset = "Skill2",
            CreatedBy = "admin",
            Role = "Employee",
            Status = "Active"
        });
        await context.SaveChangesAsync();

        var result = await service.GetAllEmployees();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsCorrectEmployee()
    {
        var service = GetService(out var context);

        var emp = new Employee
        {
            Name = "John",
            Username = "john",
            Password = "123",
            Address = "Test Address",
            Department = "IT",
            Designation = "Developer",
            Skillset = "C#",
            CreatedBy = "admin",
            Role = "Employee",
            Status = "Active"
        };

        context.Employees.Add(emp);
        await context.SaveChangesAsync();

        var result = await service.GetEmployeeById(emp.EmployeeId);

        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
    }

    [Fact]
    public async Task UpdateEmployee_UpdatesRecord()
    {
        var service = GetService(out var context);

        var emp = new Employee
        {
            Name = "Old",
            Username = "u",
            Password = "123",
            Address = "Initial Address",
            Department = "IT",
            Designation = "Engineer",
            Skillset = "C#",
            CreatedBy = "admin",
            Role = "Employee",
            Status = "Active"
        };

        context.Employees.Add(emp);
        await context.SaveChangesAsync();

        emp.Name = "New";
        emp.Department = "HR";
        emp.Designation = "Senior Engineer";
        emp.Skillset = "C#, SQL";

        var result = await service.UpdateEmployee(emp);

        Assert.True(result);

        var updated = await context.Employees.FindAsync(emp.EmployeeId);

        Assert.Equal("New", updated.Name);
        Assert.Equal("HR", updated.Department);
    }

    [Fact]
    public async Task ToggleStatus_ChangesActiveState()
    {
        var service = GetService(out var context);

        var emp = new Employee
        {
            Name = "Test",
            Username = "t",
            Password = "123",
            Address = "Address",
            Department = "Dept",
            Designation = "Developer",
            Skillset = "C#",
            CreatedBy = "admin",
            Role = "Employee",
            Status = "Active"
        };

        context.Employees.Add(emp);
        await context.SaveChangesAsync();

        var result = await service.ToggleEmployeeStatus(emp.EmployeeId);

        Assert.True(result);

        var dbEmp = await context.Employees.FindAsync(emp.EmployeeId);

        Assert.Equal("Inactive", dbEmp.Status);
    }
}
