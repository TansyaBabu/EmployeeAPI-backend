using EmployeeAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeAPI.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> Login(string username, string password);
        Task<bool> Register(Employee emp);

        Task<List<Employee>> GetAllEmployees();
        Task<Employee?> GetEmployeeById(int id);

        Task<bool> UpdateEmployee(Employee emp);
        Task<bool> ToggleEmployeeStatus(int id);
        Task<bool> SoftDeleteEmployee(int id);
    }
}