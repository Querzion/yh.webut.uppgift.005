using Domain.Models;

namespace Business.Models;

public interface IUserServiceResult
{
    IEnumerable<User>? Result { get; set; }
}

public class UserServiceResult : ServiceResult, IUserServiceResult
{
    public IEnumerable<User>? Result { get; set; }
}