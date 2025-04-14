using Business.Models;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IStatusService
{
    Task<StatusServiceResult<IEnumerable<Status>>> GetStatusesAsync();
    Task<StatusServiceResult<Status>> GetStatusByNameAsync(string statusName);
    Task<StatusServiceResult<Status>> GetStatusByIdAsync(int id);
}

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    #region CRUD

        #region Read

            public async Task<StatusServiceResult<IEnumerable<Status>>> GetStatusesAsync()
            {
                var result = await _statusRepository.GetAllAsync();
                return result.Succeeded
                    ? new StatusServiceResult<IEnumerable<Status>> { Succeeded = true, StatusCode = 200, Result = result.Result }
                    : new StatusServiceResult<IEnumerable<Status>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
            }

            public async Task<StatusServiceResult<Status>> GetStatusByNameAsync(string statusName)
            {
                var result = await _statusRepository.GetAsync(x => x.StatusName == statusName);
                return result.Succeeded
                    ? new StatusServiceResult<Status> { Succeeded = true, StatusCode = 200, Result = result.Result }
                    : new StatusServiceResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
            }
                
            public async Task<StatusServiceResult<Status>> GetStatusByIdAsync(int id)
            {
                var result = await _statusRepository.GetAsync(x => x.Id == id);
                return result.Succeeded
                    ? new StatusServiceResult<Status> { Succeeded = true, StatusCode = 200, Result = result.Result }
                    : new StatusServiceResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
            }

        #endregion

    #endregion
}