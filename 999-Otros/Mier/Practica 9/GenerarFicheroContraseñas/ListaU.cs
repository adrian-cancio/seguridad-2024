using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GenerarFicheroContraseñas
{
    internal class ListaU
    {
        // Declarar el maximo de usuarios
        private const int MaxUsu = 5;

        // Declarar el vector de usuarios
        private User[] users = new User[MaxUsu];

        // Metodo que asigna un usuario a un indice de la lista
        public void IniUsu(int Indice, User Usuario)
        {
            users[Indice] = Usuario;
        }

        // Metodo que muestra los usuarios de la lista
        public void VerLista()
        {
            for (int i = 0; i < users.Length; i++)
            {
                Console.WriteLine(users[i].getNombre());
            }
        }

        // Metodo que guarda los valores de los usuarios en binario
        public void GuardaListaBin(String NombreFich)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Create);
            BinaryWriter bin_writer = new BinaryWriter(fs, Encoding.Unicode);

            for (int i = 0; i < users.Length; i++)
            {
                bin_writer.Write(users[i].getNombre());
                bin_writer.Write(users[i].getSalt());
                bin_writer.Write(users[i].getResuContra());
            }

            bin_writer.Close();
            fs.Close();
        }

        // Metodo que guarda los valores de los usuarios en txt
        public void GuardaListaTxt(String NombreFich)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Create);
            StreamWriter stream_writer = new StreamWriter(fs, Encoding.ASCII);

            for (int i = 0; i < users.Length; i++)
            {
                stream_writer.Write(users[i].getNombre());
                stream_writer.Write(Convert.ToBase64String(users[i].getSalt()));
                stream_writer.Write(Convert.ToBase64String(users[i].getResuContra()));
                stream_writer.Write("\r\n");
            }

            stream_writer.Close();
            fs.Close();
        }

        // Metodo que guarda los valores de los usuarios en xml
        public void GuardaListaXml(string NombreFich)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (FileStream fs = new FileStream(NombreFich, FileMode.Create))
            using (XmlWriter writer = XmlWriter.Create(fs, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("LISTA");
                foreach (User usuario in users)
                {
                    writer.WriteStartElement("USUARIO");
                    writer.WriteElementString("NOMBRE", new string(usuario.getNombre()));
                    writer.WriteStartElement("SALT");
                    writer.WriteBase64(usuario.getSalt(), 0, usuario.getSalt().Length);
                    writer.WriteEndElement();
                    writer.WriteStartElement("RESUCONTRA");
                    writer.WriteBase64(usuario.getResuContra(), 0, usuario.getResuContra().Length);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}

