using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificaContra
{
    internal class ListaU
    {

        public enum SUPORTED_FILE_TYPE { BIN, TXT, XML };

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
            switch (fileType)
            {
                case SUPORTED_FILE_TYPE.BIN:
                    CargaListaBin(nombreFich);
                    break;
                case SUPORTED_FILE_TYPE.TXT:
                    CargaListaTxt(nombreFich);
                    break;
                case SUPORTED_FILE_TYPE.XML:
                    CargaListaXML(nombreFich);
                    break;
                default:
                    throw new ArgumentException("Unsupported file type");
            }
        }

        public void IniUsu(int indice, User usuario)
        {
            if (ExisteUsuario(usuario.Name))
            {
                throw new ArgumentException("User already exists");
            }
            AseguraEspacio(indice);
            Lista[indice] = usuario;
        }

        public void IniUsu(User usuario)
        {
            if (ExisteUsuario(usuario.Name))
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
                    if (usuario.Name == name)
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
                    if (Lista[i].Name == name)
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
                    if (usuario.Name == name)
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


        public void GuardaListaXML(string NombreFich, bool writeHashMethod = false)
        {
            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(NombreFich, new System.Xml.XmlWriterSettings() { Indent = true, Encoding = Encoding.UTF8 }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Usuarios");
                foreach (User usuario in Lista)
                {
                    if (usuario != null)
                    {
                        writer.WriteStartElement("Usuario");
                        writer.WriteElementString("Nombre", usuario.Name);
                        writer.WriteElementString("Salt", Convert.ToBase64String(usuario.Salt));
                        writer.WriteElementString("Resumen", Convert.ToBase64String(usuario.Hash));
                        if (writeHashMethod)
                        {
                            writer.WriteElementString("HashMethod", usuario.HashMethod.ToString());
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void CargaListaBin(string NombreFich)
        {
            if (!System.IO.File.Exists(NombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(NombreFich, System.IO.FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    User usuario = new User(reader.ReadString(), reader.ReadBytes(User.SALT_BYTES), reader.ReadBytes(User.HASH_BYTES), this.DefaultHashMethod);
                    if (this.ExisteUsuario(usuario.Name)) { continue; }
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
                    String name = reader.ReadLine();
                    byte[] salt = Convert.FromBase64String(reader.ReadLine());
                    byte[] hash = Convert.FromBase64String(reader.ReadLine());
                    User usuario = new User(name, salt, hash, this.DefaultHashMethod);
                    if (this.ExisteUsuario(usuario.Name)) { continue; }
                    Lista.Add(usuario);
                }
            }
        }

        public void CargaListaXML(string NombreFich) {
            if (!System.IO.File.Exists(NombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(NombreFich))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        if (reader.Name == "Usuario")
                        {
                            String name = null;
                            byte[] salt = null;
                            byte[] hash = null;
                            User.HASH_METHOD hashMethod = this.DefaultHashMethod;
                            while (reader.Read())
                            {
                                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                                {
                                    switch (reader.Name)
                                    {
                                        case "Nombre":
                                            name = reader.ReadElementContentAsString();
                                            break;
                                        case "Salt":
                                            salt = Convert.FromBase64String(reader.ReadElementContentAsString());
                                            break;
                                        case "Resumen":
                                            hash = Convert.FromBase64String(reader.ReadElementContentAsString());
                                            break;
                                        case "HashMethod":
                                            hashMethod = (User.HASH_METHOD)Enum.Parse(typeof(User.HASH_METHOD), reader.ReadElementContentAsString());
                                            break;
                                    }
                                }
                                if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.Name == "Usuario")
                                {
                                    break;
                                }
                            }
                            User usuario = new User(name, salt, hash, hashMethod);
                            if (this.ExisteUsuario(usuario.Name)) { continue; }
                            this.IniUsu(usuario);
                        }
                    }
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

                    bool existeUsuario = name == usuario.Name;
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

