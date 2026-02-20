using EmployeeAPI.Data;
using EmployeeAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Services
{
    public class EmployeeService_EF : IEmployeeService
    {
        private readonly AppDbContext _context;

        // ✅ Inject DbContext
        public EmployeeService_EF(AppDbContext context)
        {
            _context = context;
        }

        // 🔐 LOGIN
        public async Task<Employee?> Login(string username, string password)
        {
            var user = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return null;

            if (string.IsNullOrEmpty(user.Password) || !user.Password.StartsWith("$2"))
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            // Convert image byte[] → Base64
            if (user.ProfileImageBytes != null)
            {
                user.ProfileImage =
                    Convert.ToBase64String(user.ProfileImageBytes);
            }

            return user;
        }

        // 🔐 REGISTER
        public async Task<bool> Register(Employee emp)
        {
            emp.Status = "Active";
            emp.Password = BCrypt.Net.BCrypt.HashPassword(emp.Password);

            await _context.Employees.AddAsync(emp);
            await _context.SaveChangesAsync();

            return true;
        }

        // ✅ GET ALL
        public async Task<List<Employee>> GetAllEmployees()
        {
            var employees = await _context.Employees.ToListAsync();

            foreach (var emp in employees)
            {
                if (emp.ProfileImageBytes != null)
                {
                    emp.ProfileImage =
                        Convert.ToBase64String(emp.ProfileImageBytes);
                }
            }

            return employees;
        }

        // ✅ GET BY ID
        public async Task<Employee?> GetEmployeeById(int id)
        {
            var emp = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (emp == null)
                return null;

            if (emp.ProfileImageBytes != null)
            {
                emp.ProfileImage =
                    Convert.ToBase64String(emp.ProfileImageBytes);
            }

            return emp;
        }

        // ✅ UPDATE
        public async Task<bool> UpdateEmployee(Employee emp)
        {
            var existing = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == emp.EmployeeId);

            if (existing == null)
                return false;

            existing.Name = emp.Name;
            existing.Designation = emp.Designation;
            existing.Address = emp.Address;
            existing.Department = emp.Department;
            existing.JoiningDate = emp.JoiningDate;
            existing.Skillset = emp.Skillset;
            existing.Status = emp.Status;
            existing.Role = emp.Role;
            existing.ModifiedBy = emp.ModifiedBy;

            if (emp.ProfileImageBytes != null)
            {
                existing.ProfileImageBytes = emp.ProfileImageBytes;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // 🔄 TOGGLE STATUS
        public async Task<bool> ToggleEmployeeStatus(int id)
        {
            var emp = await _context.Employees.FindAsync(id);

            if (emp == null)
                return false;

            emp.Status = emp.Status == "Active"
                ? "Inactive"
                : "Active";

            await _context.SaveChangesAsync();
            return true;
        }

        // 🗑 SOFT DELETE
        public async Task<bool> SoftDeleteEmployee(int id)
        {
            var emp = await _context.Employees.FindAsync(id);

            if (emp == null)
                return false;

            emp.Status = "Inactive";

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
