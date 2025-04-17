using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity, Project>(context), IProjectRepository
{
    #region ChatGPT

        public async Task<IEnumerable<ProjectMemberEntity>> GetProjectMembersAsync(string projectId)
        {
            return await _context.ProjectMembers
                .Where(m => m.ProjectId == projectId)
                .ToListAsync(); // Async method already being used here
        }

        public async Task AddProjectMembersAsync(IEnumerable<ProjectMemberEntity> projectMembers)
        {
            await _context.ProjectMembers.AddRangeAsync(projectMembers); // Asynchronous operation, so keep 'await'
        }

        // No need for async here since it's a sync operation
        public async Task RemoveProjectMembersAsync(IEnumerable<ProjectMemberEntity> projectMembers)
        {
            // Use async method for removing project members
            _context.ProjectMembers.RemoveRange(projectMembers);
            await _context.SaveChangesAsync(); // Ensure async saving of changes
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync(); // Asynchronous operation, so keep 'await'
        }

    #endregion
}