using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EmployeeAPI.Model;
using EmployeeAPI.Services;

namespace EmployeeAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController_EF : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController_EF(IEmployeeService service)
        {
            _service = service;
        }

        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _service.GetAllEmployees();
            return Ok(employees);
        }

        // 👤 Any logged in user
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var emp = await _service.GetEmployeeById(id);

            if (emp == null)
                return NotFound();

            return Ok(emp);
        }

        // 🔥 UPDATE PROFILE
        // UPDATE PROFILE
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] Employee emp)
        {
            // Load existing employee first
            var existing = await _service.GetEmployeeById(emp.EmployeeId);

            if (existing == null)
                return BadRequest(new { message = "Employee not found" });

            // Handle profile image
            if (!string.IsNullOrEmpty(emp.ProfileImage))
            {
                // New image uploaded → convert and replace
                emp.ProfileImageBytes = Convert.FromBase64String(emp.ProfileImage);
            }
            else
            {
                // No new image → KEEP old one
                emp.ProfileImageBytes = existing.ProfileImageBytes;
            }

            bool updated = await _service.UpdateEmployee(emp);

            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Employee updated successfully" });
        }
        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            bool result = await _service.ToggleEmployeeStatus(id);

            if (!result)
                return BadRequest("Status update failed");

            return Ok("Status updated successfully");
        }

        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPut("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            bool result = await _service.SoftDeleteEmployee(id);

            if (!result)
                return BadRequest("Soft delete failed");

            return Ok("Employee deactivated");
        }
    }
}