using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VerificaContra;

namespace AutenticaAyuda
{
    internal class User
    {
        // Constantes
        public const int NAME_MAX_LENGTH = 16;
        public const int SALT_BYTES = 16;
        public const int HASH_BYTES = 32;
        public const int ITERATION_COUNT = 1000;

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

        public User(char[] name, byte[] salt, byte[] hash)
        {
            if (name.Length > NAME_MAX_LENGTH)
            {
                throw new ArgumentException("Name too long");
            }
            if (salt.Length != SALT_BYTES)
            {
                throw new ArgumentException("Invalid salt length");
            }
            if (hash.Length != HASH_BYTES)
            {
                throw new ArgumentException("Invalid hash length");
            }
            this.name = name;
            this.salt = salt;
            this.hash = hash;
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
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, this.salt)
            {
                IterationCount = ITERATION_COUNT
            };
            this.hash = rfc.GetBytes(HASH_BYTES);
        }

        public bool Verifica(string password)
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, this.salt)
            {
                IterationCount = ITERATION_COUNT
            };
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

        public String GetName()
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


    }
    internal class ListaU
    {

        // Contantes
        private const int MAX_USU = 5;

        //Atributos
        public User[] lista { get; }

        public ListaU()
        {
            lista = new User[MAX_USU];
        }

        public ListaU(string NombreFich, bool isBinary = true) : this()
        {
            if (isBinary)
            {
                this.CargaListaBin(NombreFich);
            }
            else
            {
                this.CargaListaTxt(NombreFich);
            }
        }

        public void IniUsu(int indice, User usuario)
        {
            this.lista[indice] = usuario;
        }

        public void VerLista()
        {
            int i = 0;
            foreach (User usuario in lista)
            {
                if (usuario != null)
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("Usuario " + i + ":");
                    Console.WriteLine(usuario);

                }
                i++;
            }
            Console.WriteLine("----------------------------------------------------------------------");
        }

        public void GuardaListaBin(string NombreFich)
        {
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(NombreFich, System.IO.FileMode.Create)))
            {
                foreach (User usuario in lista)
                {
                    if (usuario != null)
                    {
                        writer.Write(usuario.name);
                        writer.Write(usuario.salt);
                        writer.Write(usuario.hash);
                    }
                }
            }
        }

        public void GuardaListaTxt(string NombreFich)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(NombreFich, false, Encoding.ASCII))
            {
                foreach (User usuario in lista)
                {
                    if (usuario != null)
                    {
                        writer.WriteLine(usuario.name);
                        writer.WriteLine(Convert.ToBase64String(usuario.salt));
                        writer.WriteLine(Convert.ToBase64String(usuario.hash));
                    }
                }
            }
        }

        public int Verifica(string name, string password)
        {
            // comprobar si existe el usuario
            foreach (User usuario in lista)
            {
                if (usuario != null)
                {
                    if (name.Equals(usuario.GetName()))
                    {
                        // comprobar si la contraseña es correcta
                        if (usuario.Verifica(password))
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

        private void CargaListaBin(string NombreFich)
        {
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(NombreFich, System.IO.FileMode.Open)))
            {
                int i = 0;
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    if (i >= MAX_USU)
                    {
                        throw new System.IO.IOException(String.Format("El fichero contiene más de {0} usuarios.", MAX_USU));
                    }
                    char[] name = reader.ReadChars(User.NAME_MAX_LENGTH);
                    byte[] salt = reader.ReadBytes(User.SALT_BYTES);
                    byte[] hash = reader.ReadBytes(User.HASH_BYTES);
                    User usuario = new User(name, salt, hash);
                    lista[i] = usuario;
                    i++;
                }
                reader.Close();
                reader.Dispose();
            }
        }

        private void CargaListaTxt(string NombreFich)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(NombreFich, Encoding.ASCII))
            {
                int i = 0;
                while (!reader.EndOfStream)
                {
                    if (i >= MAX_USU)
                    {
                        throw new System.IO.IOException(String.Format("El fichero contiene más de {0} usuarios.", MAX_USU));
                    }
                    char[] name = reader.ReadLine().ToCharArray();
                    byte[] salt = Convert.FromBase64String(reader.ReadLine());
                    byte[] hash = Convert.FromBase64String(reader.ReadLine());
                    User usuario = new User(name, salt, hash);
                    lista[i] = usuario;
                    i++;
                }
                reader.Close();
                reader.Dispose();
            }
        }
    }

    internal class VerificaUtils
    {
        public static int verificaFicheroBin(string NombreFich, string name, string password)
        {
            ListaU LU = new ListaU(NombreFich);
            return LU.Verifica(name, password);
        }

        public static int verificaFicheroTxt(string NombreFich, string name, string password)
        {
            ListaU LU = new ListaU(NombreFich, false);
            return LU.Verifica(name, password);
        }
    }
}
