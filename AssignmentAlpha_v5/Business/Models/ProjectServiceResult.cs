using Domain.Models;

namespace Business.Models;



public class ProjectServiceResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

public class ProjectServiceResult : ServiceResult
{
}

