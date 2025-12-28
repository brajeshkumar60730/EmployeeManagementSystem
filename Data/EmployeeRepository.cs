using System.Data;
using EmployeeManagementSystem.Data;
using Microsoft.Data.SqlClient;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly string _connectionString;

    public EmployeeRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    SqlConnection GetConnection() => new SqlConnection(_connectionString);

    public IEnumerable<Employee> GetPaged(string search, int page, int pageSize, out int totalCount)
    {
        List<Employee> list = new();
        using SqlConnection con = GetConnection();
        SqlCommand cmd = new("sp_GetEmployees", con);
        cmd.CommandType = CommandType.StoredProcedure;
        con.Open();

        DataTable dt = new();
        dt.Load(cmd.ExecuteReader());

        if (!string.IsNullOrEmpty(search))
            dt = dt.AsEnumerable()
                   .Where(r => r.Field<string>("FullName")
                   .Contains(search, StringComparison.OrdinalIgnoreCase))
                   .CopyToDataTable();

        totalCount = dt.Rows.Count;

        var rows = dt.AsEnumerable()
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize);

        foreach (var r in rows)
        {
            list.Add(new Employee
            {
                EmployeeId = r.Field<int>("EmployeeId"),
                FullName = r.Field<string>("FullName"),
                DateOfBirth = r.Field<DateTime>("DateOfBirth"),
                Email = r.Field<string>("Email"),
                PhoneNumber = r.Field<string>("PhoneNumber"),
                Gender = r.Field<string>("Gender"),
                Country = r.Field<string>("Country"),
                ProfileImage = r.Field<string>("ProfileImage")
            });
        }
        return list;
    }

    public int Add(Employee emp)
    {
        using SqlConnection con = GetConnection();
        using SqlCommand cmd = new("sp_InsertEmployee", con)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@FullName", emp.FullName);
        cmd.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
        cmd.Parameters.AddWithValue("@Email", emp.Email);
        cmd.Parameters.AddWithValue("@PhoneNumber", emp.PhoneNumber);
        cmd.Parameters.AddWithValue("@Gender", emp.Gender);
        cmd.Parameters.AddWithValue("@Country", emp.Country);
        cmd.Parameters.AddWithValue("@ProfileImage", emp.ProfileImage ?? "default.png");

        SqlParameter outputId = new("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(outputId);

        con.Open();
        cmd.ExecuteNonQuery();

        return outputId.Value != DBNull.Value ? (int)outputId.Value : 0;
    }

    public Employee GetById(int id)
    {
        Employee emp = null;
        using SqlConnection con = GetConnection();
        using SqlCommand cmd = new("sp_GetEmployeeById", con)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@EmployeeId", id);

        con.Open();
        using SqlDataReader dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            emp = new Employee
            {
                EmployeeId = id,
                FullName = dr["FullName"].ToString(),
                DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]),
                Email = dr["Email"].ToString(),
                PhoneNumber = dr["PhoneNumber"].ToString(),
                Gender = dr["Gender"].ToString(),
                Country = dr["Country"].ToString(),
                ProfileImage = dr["ProfileImage"].ToString()
            };
        }
        return emp;
    }


    public void Update(Employee emp)
    {
        using SqlConnection con = GetConnection();
        SqlCommand cmd = new("sp_UpdateEmployee", con);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
        cmd.Parameters.AddWithValue("@FullName", emp.FullName);
        cmd.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
        cmd.Parameters.AddWithValue("@Email", emp.Email);
        cmd.Parameters.AddWithValue("@PhoneNumber", emp.PhoneNumber);
        cmd.Parameters.AddWithValue("@Gender", emp.Gender);
        cmd.Parameters.AddWithValue("@Country", emp.Country);
        cmd.Parameters.AddWithValue("@ProfileImage", emp.ProfileImage);

        con.Open();
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using SqlConnection con = GetConnection();
        SqlCommand cmd = new("sp_DeleteEmployee", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@EmployeeId", id);
        con.Open();
        cmd.ExecuteNonQuery();
    }
}
