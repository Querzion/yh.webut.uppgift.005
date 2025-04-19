using System.Text.Json;
using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.Extensions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectServiceResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectServiceResult<IEnumerable<Project>>> GetAllProjectsAsync();
    Task<ProjectServiceResult<Project>> GetProjectAsync(string id);
    Task<ProjectServiceResult<Project>> UpdateProjectAsync(EditProjectFormData model);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService, AppDbContext context) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;
    private readonly AppDbContext _context = context;
    
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

            public async Task<ProjectServiceResult<IEnumerable<Project>>> GetAllProjectsAsync()
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
                    : new ProjectServiceResult<Project>
                        { Succeeded = true, StatusCode = 404, Error = $"Project '{id}' not found." };

                // return response.MapTo<ProjectServiceResult<IEnumerable<Project>>>();
                // or
                // return new ProjectServiceResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result };
            }

            #endregion

        #region Update

            public async Task<ProjectServiceResult<Project>> UpdateProjectAsync(EditProjectFormData model)
            {
                var result = new ProjectServiceResult<Project>();

                if (model == null || string.IsNullOrEmpty(model.Id))
                {
                    result.Succeeded = false;
                    result.StatusCode = 400;
                    result.Error = "Invalid input: Project data is missing.";
                    return result;
                }

                try
                {
                    // Fetch the existing project to update
                    var projectEntityResult = await _projectRepository.GetEntityAsync(p => p.Id == model.Id);

                    if (!projectEntityResult.Succeeded)
                    {
                        result.Succeeded = false;
                        result.StatusCode = 404;
                        result.Error = "Project not found.";
                        return result;
                    }

                    var projectEntity = projectEntityResult.Result;

                    // Update project properties
                    projectEntity!.ProjectName = model.ProjectName;
                    projectEntity.Description = model.Description;
                    projectEntity.Budget = model.Budget;
                    projectEntity.StartDate = model.StartDate;
                    projectEntity.EndDate = model.EndDate;

                    // Update ClientId (which links to the Client entity)
                    projectEntity.ClientId = model.ClientId;  // The model should pass a ClientId, not a ClientName

                    // If necessary, you could also set the `Client` property based on the ID, like so:
                    // projectEntity.Client = await _clientRepository.GetClientByIdAsync(model.ClientId);

                    // Call the repository update method
                    var updateResult = await _projectRepository.UpdateAsync(projectEntity);

                    if (!updateResult.Succeeded)
                    {
                        result.Succeeded = false;
                        result.StatusCode = updateResult.StatusCode;
                        result.Error = updateResult.Error;
                        return result;
                    }

                    result.Succeeded = true;
                    result.StatusCode = 200;
                    result.Result = projectEntity.MapTo<Project>(); // Assuming you are mapping the entity to your project model

                    return result;
                }
                catch (Exception ex)
                {
                    result.Succeeded = false;
                    result.StatusCode = 500;
                    result.Error = $"An error occurred while updating the project: {ex.Message}";
                    return result;
                }
            }

        #endregion

        #region Delete

        

        #endregion
        
    #endregion

    
}