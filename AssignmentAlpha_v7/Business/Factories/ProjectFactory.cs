using Data.Entities;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.Models;

namespace Business.Factories;

public class ProjectFactory
{
    // Creates a ProjectEntity from AddProjectFormData
    public static ProjectEntity CreateFromAddProjectForm(AddProjectFormData form)
    {
        var project = new ProjectEntity
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = form.ProjectName,
            Description = form.Description,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            Budget = form.Budget,
            ClientId = form.ClientId,
            UserId = form.UserId,
            StatusId = form.StatusId,
            ImageId = form.ImageId ?? null,
            Image = form.ImageId == null && form.Image != null
                ? new ImageEntity
                {
                    ImageUrl = form.Image.ImageUrl,
                    AltText = form.Image.AltText,
                    UploadedAt = DateTime.UtcNow
                }
                : null,
            // Handle ProjectMembers as part of the project creation
            ProjectMembers = form.ProjectMembers?.Select(pm => new ProjectMemberEntity
            {
                ProjectId = pm.ProjectId,
                UserId = pm.UserId
            }).ToList() ?? []
        };

        return project;
    }

    // Updates an existing ProjectEntity from EditProjectFormData
    public static void UpdateFromEditProjectForm(ProjectEntity existingProject, EditProjectFormData form)
    {
        existingProject.ProjectName = form.ProjectName;
        existingProject.Description = form.Description;
        existingProject.StartDate = form.StartDate;
        existingProject.EndDate = form.EndDate;
        existingProject.Budget = form.Budget;

        if (form.Image != null)
        {
            existingProject.Image = new ImageEntity
            {
                ImageUrl = form.Image.ImageUrl,
                AltText = form.Image.AltText,
                UploadedAt = DateTime.UtcNow
            };
            existingProject.ImageId = form.ImageId;
        }

        // Update Client and User if needed
        existingProject.ClientId = form.ClientId;
        existingProject.UserId = form.UserId;
        existingProject.StatusId = form.StatusId;

        // Handle ProjectMembers from the form
        if (form.ProjectMembers != null)
        {
            existingProject.ProjectMembers = form.ProjectMembers.Select(pm => new ProjectMemberEntity
            {
                UserId = pm.UserId,
                ProjectId = existingProject.Id
            }).ToList();
        }
    }

    // Converts a ProjectEntity to a Project domain model
    public static Project Create(ProjectEntity entity)
    {
        return new Project
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            Image = entity.Image != null ? new Image
            {
                Id = entity.Image.Id!,
                ImageUrl = entity.Image.ImageUrl,
                AltText = entity.Image.AltText,
                UploadedAt = entity.Image.UploadedAt
            } : null,
            ImageId = entity.ImageId,
            Client = entity.Client != null ? new Client
            {
                Id = entity.Client.Id,
                ClientName = entity.Client.ClientName,
                Email = entity.Client.Email,
                PhoneNumber = entity.Client.PhoneNumber
            } : null,
            User = entity.User != null ? new User
            {
                Id = entity.User.Id,
                FirstName = entity.User.FirstName,
                LastName = entity.User.LastName,
                Email = entity.User.Email,
                PhoneNumber = entity.User.PhoneNumber
            } : null,
            Status = entity.Status != null ? new Status
            {
                Id = entity.Status.Id,
                StatusName = entity.Status.StatusName
            } : null,
            // Handle the ProjectMembers from the entity
            ProjectMembers = entity.ProjectMembers.Select(pm => new ProjectMember
            {
                Id = pm.Id,
                ProjectId = pm.ProjectId,
                UserId = pm.UserId,
                Project = pm.Project != null ? new Project
                {
                    Id = pm.Project.Id,
                    ProjectName = pm.Project.ProjectName
                } : null,
                User = pm.User != null ? new User
                {
                    Id = pm.User.Id,
                    FirstName = pm.User.FirstName,
                    LastName = pm.User.LastName
                } : null
            }).ToList()
        };
    }
}