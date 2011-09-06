namespace ToDo.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Simple.Data;

    public class UsernameMapper : IUserService
    {
        private readonly dynamic _db = Database.OpenNamedConnection("todo");

        public string GetUsernameFromIdentifier(Guid indentifier)
        {
            var user = _db.Users.FindById(indentifier);

            return user == null ? string.Empty : user.Email;
        }

        public Guid? Authenticate(string email, string password)
        {
            var user = _db.Users.FindByEmail(email);

            if (user == null)
            {
                return null;
            }

            var storedBytes = (byte[])user.EncryptedPassword;
            var enteredBytes = GenerateSaltedHash(Encoding.UTF8.GetBytes(password), (byte[])user.Salt);

            return storedBytes.SequenceEqual(enteredBytes) ? user.Id : null;
        }

        public Guid Register(string email, string password, string confirmPassword)
        {
            if (email == null) throw new ArgumentNullException("email");
            if (password == null) throw new ArgumentNullException("password");
            if (confirmPassword == null) throw new ArgumentNullException("confirmPassword");
            if (!password.Equals(confirmPassword)) throw new ArgumentException("Password mismatch.");

            byte[] userSalt = GenerateUserSalt();
            byte[] encryptedPassword = GenerateSaltedHash(Encoding.UTF8.GetBytes(password), userSalt).ToArray();

            _db.Users.Insert(Email: email, EncryptedPassword: encryptedPassword, Salt: userSalt);
            return _db.Users.FindByEmail(email).Id;
        }

        private static byte[] GenerateUserSalt()
        {
            return Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray();
        }

        private static IEnumerable<byte> GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            var algorithm = new SHA256Managed();

            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
    }
}