using Data.Entities;
using Domain.DTOs.Adds;
using Domain.DTOs.Edits;
using Domain.Models;

namespace Business.Factories;

public static class ClientFactory
{
    public static ClientEntity CreateFromAddClientForm(AddClientFormData form)
    {
        var client = new ClientEntity
        {
            Id = Guid.NewGuid().ToString(),
            ClientName = form.ClientName,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber,
            IsActive = true,
            Date = DateTime.UtcNow,
            Address = form.AddressId != null ? null : new AddressEntity
            {
                StreetName = form.Address?.StreetName,
                City = form.Address?.City,
                PostalCode = form.Address?.PostalCode
            },
            AddressId = form.AddressId,
            ImageId = form.ImageId ?? null,
            Image = form.ImageId == null && form.Image != null
                ? new ImageEntity
                {
                    ImageUrl = form.Image.ImageUrl,
                    AltText = form.Image.AltText,
                    UploadedAt = DateTime.UtcNow
                }
                : null
        };

        return client;
    }

    public static void UpdateFromEditForm(ClientEntity existingClient, EditClientFormData form)
    {
        existingClient.ClientName = form.ClientName;
        existingClient.Email = form.Email;
        existingClient.PhoneNumber = form.PhoneNumber;

        if (form.Address != null)
        {
            existingClient.Address = new AddressEntity
            {
                StreetName = form.Address.StreetName,
                City = form.Address.City,
                PostalCode = form.Address.PostalCode
            };
        }

        if (form.Image != null)
        {
            existingClient.Image = new ImageEntity
            {
                ImageUrl = form.Image.ImageUrl,
                AltText = form.Image.AltText,
                UploadedAt = DateTime.UtcNow
            };
            existingClient.ImageId = form.ImageId;
        }
    }

    public static Client Create(ClientEntity entity)
    {
        return new Client
        {
            Id = entity.Id,
            ClientName = entity.ClientName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            IsActive = entity.IsActive,
            Date = entity.Date,
            Image = entity.Image != null ? new Image
            {
                Id = entity.Image.Id!,
                ImageUrl = entity.Image.ImageUrl,
                AltText = entity.Image.AltText,
                UploadedAt = entity.Image.UploadedAt
            } : null,
            Address = entity.Address != null ? new Address
            {
                StreetName = entity.Address.StreetName,
                City = entity.Address.City,
                PostalCode = entity.Address.PostalCode
            } : null
        };
    }
}
