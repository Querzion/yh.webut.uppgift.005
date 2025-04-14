using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Adds;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectServiceResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectServiceResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectServiceResult<Project>> GetProjectAsync(string id);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;

    #region CRUD

        #region Create

            public async Task<ProjectServiceResult> CreateProjectAsync(AddProjectFormData formData)
            {
                if (formData == null)
                    return new ProjectServiceResult
                        { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };
                
                var projectEntity = formData.MapTo<ProjectEntity>();
                var statusResult = await _statusService.GetStatusByIdAsync(1);
                var status = statusResult.Result;
                
                projectEntity.StatusId = status!.Id;
                
                var result = await _projectRepository.AddAsync(projectEntity);
                return result.Succeeded
                    ? new ProjectServiceResult { Succeeded = true, StatusCode = 201 }
                    : new ProjectServiceResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
            }

        #endregion

        #region Read

            public async Task<ProjectServiceResult<IEnumerable<Project>>> GetProjectsAsync()
            {
                var response = await _projectRepository.GetAllAsync
                ( 
                    orderByDecending: true, 
                    sortBy: s => s.Created, 
                    where: null, 
                    include => include.User, 
                    include => include.Status, 
                    include => include.Client 
                );

                // return response.MapTo<ProjectServiceResult<IEnumerable<Project>>>();
                // or
                return new ProjectServiceResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = response.Result };
            }
        
            public async Task<ProjectServiceResult<Project>> GetProjectAsync(string id)
            {
                var response = await _projectRepository.GetAsync
                (  
                    where: x => x.Id == id, 
                    include => include.User, 
                    include => include.Status, 
                    include => include.Client 
                );

                return response.Succeeded
                    ? new ProjectServiceResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result }
                    : new ProjectServiceResult<Project> { Succeeded = true, StatusCode = 404, Error = $"Project '{id}' not found."};

                // return response.MapTo<ProjectServiceResult<IEnumerable<Project>>>();
                // or
                // return new ProjectServiceResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result };
            }

        #endregion

        #region Update

        

        #endregion

        #region Delete

        

        #endregion
        
    #endregion
}