namespace ToDo.App
{
    using System;
    using System.Linq;
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

            var storedBytes = (byte[]) user.EncryptedPassword;
            var enteredBytes = EncryptionHelper.GenerateSaltedHash(Encoding.UTF8.GetBytes(password), (byte[])user.Salt);

            return storedBytes.SequenceEqual(enteredBytes) ? user.Id : null;
        }

        public Guid Register(string email, string password, string confirmPassword)
        {
            if (email == null) throw new ArgumentNullException("email");
            if (password == null) throw new ArgumentNullException("password");
            if (confirmPassword == null) throw new ArgumentNullException("confirmPassword");
            if (!password.Equals(confirmPassword)) throw new ArgumentException("Password mismatch.");

            byte[] userSalt = EncryptionHelper.GenerateUserSalt();
            byte[] encryptedPassword = EncryptionHelper.GenerateSaltedHash(Encoding.UTF8.GetBytes(password), userSalt).ToArray();

            _db.Users.Insert(Email: email, EncryptedPassword: encryptedPassword, Salt: userSalt);
            return _db.Users.FindByEmail(email).Id;
        }
    }
}