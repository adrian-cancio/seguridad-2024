using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificaContra
{
    internal class ListaU
    {

        public enum SUPORTED_FILE_TYPE { BIN, TXT };

        //Atributos
        public List<User> Lista { get; }
        public User.HASH_METHOD DefaultHashMethod { get; }


        public ListaU(User.HASH_METHOD defaultHashMethod = User.HASH_METHOD.RFC)
        {
            Lista = new List<User>();
            DefaultHashMethod = defaultHashMethod;
        }

        public ListaU(String nombreFich, SUPORTED_FILE_TYPE fileType = SUPORTED_FILE_TYPE.BIN, User.HASH_METHOD defaultHashMethod = User.HASH_METHOD.RFC) : this(defaultHashMethod)
        {
            switch (fileType) {
                case SUPORTED_FILE_TYPE.BIN:
                    CargaListaBin(nombreFich);
                    break;
                case SUPORTED_FILE_TYPE.TXT:
                    CargaListaTxt(nombreFich);
                    break;
                default:
                    throw new ArgumentException("Unsupported file type");
            }
        }

        public void IniUsu(int indice, User usuario)
        {
            if (ExisteUsuario(usuario.GetName()))
            {
                throw new ArgumentException("User already exists");
            }
            AseguraEspacio(indice);
            Lista[indice] = usuario;
        }

        public void IniUsu(User usuario)
        {
            if (ExisteUsuario(usuario.GetName()))
            {
                throw new ArgumentException("User already exists");
            }
            Lista.Add(usuario);
        }

        public void ModUsu(String name, String password)
        {
            foreach (User usuario in Lista)
            {
                if (usuario != null)
                {
                    if (usuario.GetName() == name)
                    {
                        usuario.SetPassword(password);
                        return;
                    }
                }
            }
        }

        public void DelUsu(String name)
        {
            for (int i = 0; i < Lista.Count; i++)
            {
                if (Lista[i] != null)
                {
                    if (Lista[i].GetName() == name)
                    {
                        Lista.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        private void AseguraEspacio(int indice)
        {
            while (Lista.Count <= indice)
            {
                Lista.Add(null);
            }
        }

        public bool ExisteUsuario(String name)
        {
            foreach (User usuario in Lista)
            {
                if (usuario != null)
                {
                    if (usuario.GetName() == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void VerLista()
        {
            int i = 0;
            foreach (User usuario in Lista)
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
                foreach (User usuario in Lista)
                {
                    if (usuario != null)
                    {
                        writer.Write(usuario.Name);
                        writer.Write(usuario.Salt);
                        writer.Write(usuario.Hash);
                    }
                }
            }
        }

        public void GuardaListaTxt(string NombreFich)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(NombreFich, false, Encoding.ASCII))
            {
                foreach (User usuario in Lista)
                {
                    if (usuario != null)
                    {
                        writer.WriteLine(usuario.Name);
                        writer.WriteLine(Convert.ToBase64String(usuario.Salt));
                        writer.WriteLine(Convert.ToBase64String(usuario.Hash));
                    }
                }
            }
        }

        public void CargaListaBin(string NombreFich)
        {
            if(!System.IO.File.Exists(NombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(NombreFich, System.IO.FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    User usuario = new User(reader.ReadChars(User.NAME_MAX_LENGTH), reader.ReadBytes(User.SALT_BYTES), reader.ReadBytes(User.HASH_BYTES));
                    if (this.ExisteUsuario(usuario.GetName())) { continue; }
                    Lista.Add(usuario);
                }
            }
        }

        public void CargaListaTxt(string NombreFich)
        {
            if (!System.IO.File.Exists(NombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.IO.StreamReader reader = new System.IO.StreamReader(NombreFich, Encoding.ASCII))
            {
                while (!reader.EndOfStream)
                {
                    char[] name = reader.ReadLine().ToCharArray();
                    byte[] salt = Convert.FromBase64String(reader.ReadLine());
                    byte[] hash = Convert.FromBase64String(reader.ReadLine());
                    User usuario = new User(name, salt, hash);
                    if (this.ExisteUsuario(usuario.GetName())) { continue; }
                    Lista.Add(usuario);
                }
            }
        }



        public int Verifica(string name, string password)
        {
            // comprobar si existe el usuario
            foreach (User usuario in Lista)
            {

                if (usuario != null)
                {
        
                    bool existeUsuario =  name == usuario.GetName();
                    if (existeUsuario)
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

    }
}

