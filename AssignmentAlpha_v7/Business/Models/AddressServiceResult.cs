using Domain.Models;

namespace Business.Models;

public interface IAddressServiceResult
{
    IEnumerable<Address>? Result { get; set; }
}

public class AddressServiceResult : ServiceResult, IAddressServiceResult
{
    public IEnumerable<Address>? Result { get; set; }
}