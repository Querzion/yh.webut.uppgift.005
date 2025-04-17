using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IProjectRepository : IBaseRepository<ProjectEntity, Project>
{
    Task<IEnumerable<ProjectMemberEntity>> GetProjectMembersAsync(string projectId);
    Task AddProjectMembersAsync(IEnumerable<ProjectMemberEntity> projectMembers);
    Task RemoveProjectMembersAsync(IEnumerable<ProjectMemberEntity> projectMembers);
    Task SaveChangesAsync();
}