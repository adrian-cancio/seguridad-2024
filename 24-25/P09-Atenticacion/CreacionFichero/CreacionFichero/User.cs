using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CreacionFichero
{
    internal class User
    {
        // Constantes
        private const int NAME_MAX_LENGTH = 16;
        private const int SALT_BYTES = 16;
        private const int HASH_BYTES = 32;

        // Atributos
        public char[] name { get; }
        public byte[] salt { get; private set; }
        public byte[] hash { get; private set; }

        public User()
        {

            name = new char[NAME_MAX_LENGTH];
            salt = new byte[SALT_BYTES];
            hash = new byte[HASH_BYTES];
        }

        public User(String name, String password) : this()
        {
            this.SetName(name);
            this.SetPassword(password);
        }

        public void SetName(String name)
        {
            if (name.Length > NAME_MAX_LENGTH)
            {
                throw new ArgumentException("Name too long");
            }
            for (int i = 0; i < name.Length; i++)
            {
                this.name[i] = name[i];
            }
        }

        public void SetPassword(String password)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(this.salt);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, this.salt);
            rfc.IterationCount = 1000;
            this.hash = rfc.GetBytes(HASH_BYTES);
        }

        override public String ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Name: ");
            stringBuilder.Append(this.name);
            stringBuilder.Append("\nSalt: ");
            stringBuilder.Append(BitConverter.ToString(this.salt).Replace("-", ""));
            stringBuilder.Append("\nHash: ");
            stringBuilder.Append(BitConverter.ToString(this.hash).Replace("-", ""));
            return stringBuilder.ToString();
        }


    }
}
