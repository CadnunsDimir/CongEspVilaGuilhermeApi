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

        private static bool PasswordHasNoValue(User? user) => string.IsNullOrEmpty(user?.PasswordHash);

        public async Task InitializeAdminUserAsync()
        {
            var admin = await repository.GetByUserName(AdminUserName);
            if (admin != null && PasswordHasNoValue(admin))
            {
                var tempPassword = Guid.NewGuid().ToString();
                admin.PasswordHash = tokenService.GeneratePasswordHash(tempPassword);
                await emailService.SendNewPasswordAsync(admin, tempPassword);
                await repository.Update(admin);
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
                await emailService.NotifyNewUserAsync(user);
                await repository.Create(user);
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

        public async Task StartResetPassword(string userName)
        {
            var user = await repository.GetByUserName(userName)
                ?? throw new ArgumentException($"Usuário {userName} não encontrado");

            user.RequestResetPassord();

            await emailService.SendResetPassordEmailAsync(user);
            await repository.Update(user);
        }

        public async Task<TransactionEntityStatus> FinishResetPassword(string userName, string resetPasswordId, string newPassword)
        {
            var user = await repository.GetByUserName(userName)
                ?? throw new ArgumentException($"Usuário {userName} não encontrado");

            if (user.CanResetPassword(resetPasswordId))
            {
                user.PasswordHash = tokenService.GeneratePasswordHash(newPassword);
                user.ResetPasswordId = null;
                user.ResetPasswordRequestedAt = null;
                await emailService.SendNewPasswordAsync(user, newPassword);
                await repository.Update(user);

                return TransactionEntityStatus.CreatedOrUpdated;
            }

            return TransactionEntityStatus.NotUpdated;
        }
    }
}
