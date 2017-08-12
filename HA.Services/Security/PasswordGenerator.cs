using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace HA.Services
{
    public class PasswordGenerator
    {
        private const string Chars = @"abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!#$%&'()*+,-./:;<=>?@[\]_";

        public PasswordGenerator(int lenght, string salt)
        {
            Password = null;
            GeneratePassword(lenght);
            CreateHash(salt);
        }

        private void CreateHash(string salt)
        {	        
            byte[] bytes = Encoding.UTF8.GetBytes(salt+Password); 
            MD5 md5 = MD5.Create();                
            SHA1 sha = SHA1.Create();
            Hash = Encoding.UTF8.GetString(sha.ComputeHash(md5.ComputeHash(bytes)));
        }

        private void GeneratePassword(int length)
        {
            Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            var password = new char[length];

            for (var i = 0; i < length; i++)
            {
                password[i] = Chars[rand.Next(0, Chars.Length)];
            }

            Password = new String(password);
        }


        public string Password
        {
            get;
            private set;
        }

        public string Hash
        {
            get;
            private set;
        }
    }
}
