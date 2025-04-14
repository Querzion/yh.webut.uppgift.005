using Domain.Models;

namespace Business.Models;



public class StatusServiceResult<T> : ServiceResult
{
    public T? Result { get; set; }
}
public class StatusServiceResult : ServiceResult
{
}

