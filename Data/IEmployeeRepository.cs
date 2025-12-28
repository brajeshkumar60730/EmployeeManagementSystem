using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetPaged(string search, int page, int pageSize, out int totalCount);
        Employee GetById(int id);
        int Add(Employee emp);
        void Update(Employee emp);
        void Delete(int id);
    }
}
