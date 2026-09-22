using Microsoft.Data.SqlClient;
namespace RoleBasedTaskManagement.Application.Interfaces
{
    public interface IDbConnectionRepository
    {
        SqlConnection CreateConnection();
    }
}
