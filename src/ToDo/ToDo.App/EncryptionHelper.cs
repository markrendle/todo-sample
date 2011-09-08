using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ToDo.App
{
    static class EncryptionHelper
    {
        public static byte[] GenerateUserSalt()
        {
            return Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray();
        }

        public static IEnumerable<byte> GenerateSaltedHash(byte[] plainText, byte[] salt)
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