// using Business.DTOs;
// using Business.Models;
// using Data.Entities;
//
// namespace Business.Factories;
//
// public static class ClientFactory
// {
//     public static ClientRegistrationForm CreateRegistrationForm() => new();
//     public static ClientUpdateForm CreateUpdateForm() => new();
//     
//     public static ClientEntity CreateEntityFrom(ClientRegistrationForm registrationForm) => new()
//     {
//         ClientName = registrationForm.ClientName,
//         Email = registrationForm.Email,
//         Location = registrationForm.Location,
//         PhoneNumber = registrationForm.PhoneNumber
//     };
//
//     public static Client CreateOutputModel(ClientEntity entity) => new()
//     {
//         Id = entity.Id,
//         ClientName = entity.ClientName,
//         Email = entity.Email,
//         Location = entity.Location,
//         PhoneNumber = entity.PhoneNumber
//     };
//     public static Client CreateOutputModelFrom(ClientEntity entity) => new()
//     {
//         Id = entity.Id,
//         ClientName = entity.ClientName,
//         Email = entity.Email,
//         Location = entity.Location,
//         PhoneNumber = entity.PhoneNumber
//     };
//
//     public static ClientUpdateForm CreateUpdateFrom(Client customer) => new()
//     {
//         Id = customer.Id,
//         ClientName = customer.ClientName,
//         Email = customer.Email,
//         Location = customer.Location,
//         PhoneNumber = customer.PhoneNumber
//     };
//
//     public static ClientEntity Update(ClientEntity customerEntity, ClientUpdateForm updateForm)
//     {
//         customerEntity.Id = customerEntity.Id;
//         customerEntity.ClientName = updateForm.ClientName;
//         customerEntity.Email = updateForm.Email;
//         customerEntity.Location = updateForm.Location;
//         customerEntity.PhoneNumber = updateForm.PhoneNumber;
//         
//         return customerEntity;
//     }
// }