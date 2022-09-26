using System.Text;
using System.Security.Cryptography;

namespace eda7k.Models.Common
{
    public static class Crypt
    {
        public static string GetHashPassword(string password)
        {
            using var crypt = SHA256.Create();//8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918
            var hash = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashString = new StringBuilder();
            foreach (var x in hash)
            {
                hashString.Append(x.ToString("x2"));
            }

            return hashString.ToString();
        }
    }
}
