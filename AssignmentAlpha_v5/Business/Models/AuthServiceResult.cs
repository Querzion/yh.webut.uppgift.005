namespace Business.Models;

public class AuthServiceResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

public class AuthServiceResult : ServiceResult
{
}