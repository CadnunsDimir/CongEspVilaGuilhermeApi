using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Models;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using CongEspVilaGuilhermeApi.Domain.Services;
using CongEspVilaGuilhermeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CongEspVilaGuilhermeApi.Domain.UseCases
{
    public enum TransactionEntityStatus
    {
        CreatedOrUpdated,
        UserNameAlreadyExists,
        NotUpdated
    }
    public class UserUseCases
    {
        private readonly IUserRepository repository;
        private readonly IEmailService emailService;
        private readonly ITokenService tokenService;
        private readonly string AdminUserName = "admin";

        public UserUseCases(IUserRepository repository, IEmailService emailService, ITokenService tokenService)
        {
            this.repository = repository;
            this.emailService = emailService;
            this.tokenService = tokenService;
        }

        private bool PasswordHasNoValue(User? user) => string.IsNullOrEmpty(user?.PasswordHash);

        public async Task InitializeAdminUserAsync()
        {
            var admin = await repository.GetByUserName(AdminUserName);
            if (admin != null && PasswordHasNoValue(admin))
            {
                var tempPassword = Guid.NewGuid().ToString();
                admin.PasswordHash = tokenService.GeneratePasswordHash(tempPassword);
                await repository.Update(admin);
                emailService.SendNewPassword(admin, tempPassword);
                Console.WriteLine($"senha admin enviada para o email {admin.Email}");
            }
        }

        public async Task<bool> PasswordIsValid(string userName, string password)
        {
            var user = await repository.GetByUserName(userName);
            var typedPasswordHash = tokenService.GeneratePasswordHash(password);
            return user?.PasswordHash == typedPasswordHash;
        }

        public async Task<TransactionEntityStatus> CreateNewUser(NewAccount newAccount)
        {
            if (await repository.UserNameIsAvailable(newAccount.UserName))
            {
                var user = newAccount.CreateUserEntity(tokenService.GeneratePasswordHash(newAccount.Password));
                await repository.Create(user);
                emailService.NotifyNewUser(user);
                return TransactionEntityStatus.CreatedOrUpdated;
            }
            return TransactionEntityStatus.UserNameAlreadyExists;
        }

        public async Task<string?> GenerateToken(string userName)
        {
            var user = await repository.GetByUserName(userName);
            return user != null ? tokenService.GenerateToken(user) : null;
        }

        public async Task AddRole(string userName, string role)
        {
            if(RoleTypes.IsValid(role))
            {
                var user = await repository.GetByUserName(userName);
                if(user != null && !user.Role.Contains(role))
                {
                    user.Role = $"{user.Role},{role}";
                    await repository.Update(user);
                }
            } 
            else 
            {
                throw new ArgumentException("ValidTypes: " + RoleTypes.ValidRolesSeparedByColma());
            }
        }
    }
}
