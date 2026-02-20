namespace EmployeeAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using EmployeeAPI.Model;
    using EmployeeAPI.Services;

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _service;

        public EmployeeController(EmployeeService service)
        {
            _service = service;
        }

        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var employees = _service.GetAllEmployees();
            return Ok(employees);
        }

        // 👤 Any logged in user
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var emp = _service.GetEmployeeById(id);
            if (emp == null)
                return NotFound();

            return Ok(emp);
        }

        // 🔥 MODIFY HERE
        [HttpPut("update")]
        public IActionResult Update([FromBody] Employee emp)
        {
            // 🔥 Convert Base64 → byte[]
            if (!string.IsNullOrEmpty(emp.ProfileImageBase64))
            {
                emp.ProfileImageBytes =
                    Convert.FromBase64String(emp.ProfileImageBase64);
            }

            bool updated = _service.UpdateEmployee(emp);

            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Employee updated successfully" });
        }


        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPut("toggle/{id}")]
        public IActionResult ToggleStatus(int id)
        {
            bool result = _service.ToggleEmployeeStatus(id);

            if (!result)
                return BadRequest("Status update failed");

            return Ok("Status updated successfully");
        }

        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPut("softdelete/{id}")]
        public IActionResult SoftDelete(int id)
        {
            bool result = _service.SoftDeleteEmployee(id);

            if (!result)
                return BadRequest("Soft delete failed");

            return Ok("Employee deactivated");
        }
    }
}
