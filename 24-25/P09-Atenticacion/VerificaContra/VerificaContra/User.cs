using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VerificaContra
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

        public bool Verifica(string password)
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, this.salt);
            rfc.IterationCount = 1000;
            byte[] hash = rfc.GetBytes(HASH_BYTES);
            for (int i = 0; i < HASH_BYTES; i++)
            {
                if (this.hash[i] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

        public String getName()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < NAME_MAX_LENGTH; i++)
            {
                if (this.name[i] == '\0')
                {
                    break;
                }
                stringBuilder.Append(this.name[i]);
            }
            return stringBuilder.ToString();
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

        public static int VerificaTxt(String nombreFich, String name, String password)
        {
            if (!System.IO.File.Exists(nombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.IO.StreamReader reader = new System.IO.StreamReader(nombreFich, Encoding.ASCII))
            {
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    line = line.Replace("\0", "");

                    if (line == name)
                    {
                        byte[] salt = Convert.FromBase64String(reader.ReadLine());
                        byte[] hash = Convert.FromBase64String(reader.ReadLine());
                        User user = new User(name, password);
                        user.salt = salt;
                        user.hash = hash;
                        if (user.Verifica(password))
                        {
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    reader.ReadLine();
                    reader.ReadLine();
                }
            }
            return 1;
        }

        public static int VerificaBin(String nombreFich, String name, String password)
        {
            if (!System.IO.File.Exists(nombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(nombreFich, System.IO.FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    byte[] lineBytes = reader.ReadBytes(NAME_MAX_LENGTH);
                    String line = Encoding.UTF8.GetString(lineBytes);
                    line = line.Replace("\0", "");

                    if (line == name)
                    {
                        byte[] salt = reader.ReadBytes(SALT_BYTES);
                        byte[] hash = reader.ReadBytes(HASH_BYTES);
                        User user = new User(name, password);
                        user.salt = salt;
                        user.hash = hash;
                        if (user.Verifica(password))
                        {
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
            }
            return 1;
        }


    }
}
