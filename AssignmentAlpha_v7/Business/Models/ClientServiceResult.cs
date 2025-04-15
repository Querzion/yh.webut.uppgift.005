using Domain.Models;

namespace Business.Models;

public interface IClientServiceResult
{
    IEnumerable<Client>? Result { get; set; }
}

public class ClientServiceResult : ServiceResult, IClientServiceResult
{
    public IEnumerable<Client>? Result { get; set; }
}