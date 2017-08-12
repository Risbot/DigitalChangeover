using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HA.MVVMClient.Infrastructure
{
    public class Password
    {
        private string password;
        public Password(string username, string password)
        {
            this.password = password;
            CreateHash(username);
        }

        private void CreateHash(string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(salt + password);
            MD5 md5 = MD5.Create();
            SHA1 sha = SHA1.Create();
            Hash = Encoding.UTF8.GetString(sha.ComputeHash(md5.ComputeHash(bytes)));
        }

        public string Hash
        {
            get;
            private set;
        }
    }
}
