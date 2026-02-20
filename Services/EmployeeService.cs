namespace EmployeeAPI.Services
{
    using System.Data;
    using Microsoft.Data.SqlClient;
    using EmployeeAPI.Model;
    using BCrypt.Net;

    public class EmployeeService
    {
        private readonly string _conn;

        public EmployeeService(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }

        // 🔐 LOGIN
        public Employee Login(string username, string password)
        {
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Employees WHERE Username=@Username", con);

            cmd.Parameters.AddWithValue("@Username", username);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            string storedHash = reader["Password"].ToString();

            if (string.IsNullOrEmpty(storedHash) || !storedHash.StartsWith("$2"))
                return null;

            if (!BCrypt.Verify(password, storedHash))
                return null;

            return new Employee
            {
                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                Name = reader["Name"].ToString(),
                Username = reader["Username"].ToString(),
                Department = reader["Department"].ToString(),
                Designation = reader["Designation"].ToString(),
                Skillset = reader["Skillset"].ToString(),
                Address = reader["Address"]?.ToString(),
                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),

                // ✅ Convert byte[] → Base64
                ProfileImageBase64 = reader["ProfileImage"] == DBNull.Value
                    ? null
                    : Convert.ToBase64String((byte[])reader["ProfileImage"]),

                Role = reader["Role"].ToString(),
                Status = reader["Status"].ToString()
            };
        }

        // 🔐 REGISTER
        public bool Register(Employee emp)
        {
            emp.Status = "Active";
            emp.Password = BCrypt.HashPassword(emp.Password);

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("sp_RegisterEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", emp.Name);
            cmd.Parameters.AddWithValue("@Designation", emp.Designation);
            cmd.Parameters.AddWithValue("@Address", emp.Address);
            cmd.Parameters.AddWithValue("@Department", emp.Department);
            cmd.Parameters.AddWithValue("@JoiningDate", emp.JoiningDate);
            cmd.Parameters.AddWithValue("@Skillset", emp.Skillset);
            cmd.Parameters.AddWithValue("@Username", emp.Username);
            cmd.Parameters.AddWithValue("@Password", emp.Password);
            cmd.Parameters.AddWithValue("@Role", emp.Role);
            cmd.Parameters.AddWithValue("@CreatedBy", emp.CreatedBy ?? "system");

            cmd.Parameters.Add("@ProfileImage", SqlDbType.VarBinary)
                .Value = emp.ProfileImageBytes ?? (object)DBNull.Value;

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // ✅ GET ALL EMPLOYEES
        public List<Employee> GetAllEmployees()
        {
            List<Employee> list = new List<Employee>();

            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Employee
                {
                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                    Name = reader["Name"].ToString(),
                    Username = reader["Username"].ToString(),
                    Department = reader["Department"].ToString(),
                    Designation = reader["Designation"].ToString(),
                    Skillset = reader["Skillset"].ToString(),
                    Address = reader["Address"].ToString(),
                    JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),

                    // ✅ Convert byte[] → Base64
                    ProfileImageBase64 = reader["ProfileImage"] == DBNull.Value
                        ? null
                        : Convert.ToBase64String((byte[])reader["ProfileImage"]),

                    Role = reader["Role"].ToString(),
                    Status = reader["Status"].ToString()
                });
            }

            return list;
        }

        // ✅ GET BY ID
        public Employee GetEmployeeById(int id)
        {
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Employees WHERE EmployeeId=@Id", con);

            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new Employee
            {
                EmployeeId = id,
                Name = reader["Name"].ToString(),
                Designation = reader["Designation"].ToString(),
                Address = reader["Address"].ToString(),
                Department = reader["Department"].ToString(),
                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                Skillset = reader["Skillset"].ToString(),
                Username = reader["Username"].ToString(),
                Status = reader["Status"].ToString(),
                Role = reader["Role"].ToString(),

                // ✅ Convert byte[] → Base64
                ProfileImageBase64 = reader["ProfileImage"] == DBNull.Value
                    ? null
                    : Convert.ToBase64String((byte[])reader["ProfileImage"])
            };
        }

        // ✅ UPDATE
        public bool UpdateEmployee(Employee emp)
        {
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
            cmd.Parameters.AddWithValue("@Name", emp.Name);
            cmd.Parameters.AddWithValue("@Designation", emp.Designation);
            cmd.Parameters.AddWithValue("@Address", emp.Address);
            cmd.Parameters.AddWithValue("@Department", emp.Department);
            cmd.Parameters.AddWithValue("@JoiningDate", emp.JoiningDate);
            cmd.Parameters.AddWithValue("@Skillset", emp.Skillset);
            cmd.Parameters.AddWithValue("@Status", emp.Status);
            cmd.Parameters.AddWithValue("@Role", emp.Role);
            cmd.Parameters.AddWithValue("@ModifiedBy", emp.ModifiedBy ?? "system");

            cmd.Parameters.Add("@ProfileImage", SqlDbType.VarBinary)
                .Value = emp.ProfileImageBytes ?? (object)DBNull.Value;

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ToggleEmployeeStatus(int id)
        {
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("sp_ToggleEmployeeStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", id);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool SoftDeleteEmployee(int id)
        {
            using SqlConnection con = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand("sp_SoftDeleteEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", id);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
