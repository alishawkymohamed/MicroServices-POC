using System;
using System.Net.Mail;
using Actio.Common.Exceptions;
using Actio.Services.Identity.Domain.Services;

namespace Actio.Services.Identity.Domain.Models
{
    public class User
    {
        public Guid Id { get; protected set; }
        public string Email { get; protected set; }
        public string Password { get; protected set; }
        public string Name { get; protected set; }
        public string Salt { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected User()
        {
        }

        public User(string email, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ActioException("empty_user_name", $"User name can't be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new ActioException("empty_user_email", $"User email can't be empty");

            try
            {
                new MailAddress(email);
            }
            catch
            {
                throw new ActioException("invalid_user_email", $"User email is invalid");
            }

            Id = Guid.NewGuid();
            Email = email.ToLowerInvariant();
            Name = name.ToLowerInvariant();
            CreatedAt = DateTime.UtcNow;
        }

        public void SetPassword(string password, IEncrypter encrypter)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ActioException("empty_password", "Password can not be empty.");

            Salt = encrypter.GetSalt();
            Password = encrypter.GetHash(password, Salt);
        }

        public bool ValidatePassword(string password, IEncrypter encrypter)
            => Password.Equals(encrypter.GetHash(password, Salt));
    }
}