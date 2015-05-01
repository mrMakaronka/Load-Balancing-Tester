using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary.Utils
{
    public static class EncryptionUtils
    {
        public static string HashSHA1String(string value)
        {
            using (HashAlgorithm algorithm = SHA1.Create())
            {
                byte[] valueBytes = Encoding.UTF8.GetBytes(value);
                byte[] hashedValue = algorithm.ComputeHash(valueBytes);
                string resultString = Encoding.UTF8.GetString(hashedValue);
                return resultString;
            }
        }

        public static string[] HashSHA1StringArray(string[] values)
        {
            if (values == null)
            {
                throw new ArgumentException("Values are null");
            }

            string[] hashes = new string[values.Count()];
            for (int i = 0; i < values.Count(); i++)
            {
                hashes[i] = HashSHA1String(values[i]);
            }
            return hashes;
        }
    }
}
