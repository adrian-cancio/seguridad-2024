using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
namespace P9_Variacion
{
    class Program
    {
        static void Main(string[] args)
        {
            //Crear lista de usuarios
            ListaU LU = new ListaU();
            LU.IniUsu(0, new User("Antonio", "conA"));
            LU.IniUsu(1, new User("Benito", "conB"));
            LU.IniUsu(2, new User("Carlos", "conC"));
            LU.IniUsu(3, new User("David", "conD"));
            LU.IniUsu(4, new User("Eduardo", "conE"));
            Console.WriteLine("Lista de usuarios:");
            LU.VerLista();
            //Almacenar en ficheros
            LU.GuardaListaBin(" zz_Usuarios.bin");
            LU.GuardaListaTxt("zz_Usuarios.txt");
            Console.ReadKey();
        }
    }
    public class User
    {
        private static int lNombre = 16;
        private static int lSalt = 16;
        private static int lResu = 32;
        public char[] Nombre = new char[lNombre];
        public byte[] Salt = new byte[lSalt];
        public byte[] ResuContra = new byte[lResu];

        public User(string NuevoNombre, string NuevaContra)
        {
            //Asignar el nombre
            for (int i = 0; i < Nombre.Length; i++)
            {
                Nombre[i] = '\0';
            }
            for (int j = 0; j < NuevoNombre.Length; j++)
            {
                Nombre[j] = NuevoNombre[j];
            }
            //Generar Salt
            RNGCryptoServiceProvider proveedor = new RNGCryptoServiceProvider();
            proveedor.GetBytes(Salt);
            //Genera el resumen
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(NuevaContra, Salt);
            rfc.IterationCount = 1000;
            ResuContra = rfc.GetBytes(ResuContra.Length);

        }
    }
    public class ListaU
    {
        static int MaxUsu = 5;
        List<User> Lista = new List<User>();

        public void IniUsu(int Indice, User Usuario)
        {
            Lista[Indice] = Usuario;
        }

        public void VerLista()
        {
            for (int i = 0; i < Lista.Capacity; i++)
            {
                Console.WriteLine(Lista[i].Nombre);
            }
        }

        public void GuardaListaBin(string NombreFich)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.Unicode);
            for (int i = 0; i < Lista.Capacity; i++)
            {
                bw.Write(Lista[i].Nombre);
                bw.Write(Lista[i].Salt);
                bw.Write(Lista[i].ResuContra);
            }
            bw.Close();
            fs.Close();
        }

        public void GuardaListaTxt(string NombreFich)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
            for (int i = 0; i < Lista.Capacity; i++)
            {
                sw.Write(Lista[i].Nombre);
                sw.Write(Convert.ToBase64String(Lista[i].Salt));
                sw.Write(Convert.ToBase64String(Lista[i].ResuContra));
                sw.WriteLine();
            }
            sw.Close();
            fs.Close();
        }
        public void GuardaListaXml(string NombreFich)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Create, FileAccess.ReadWrite);
            XmlWriterSettings xmlws = new XmlWriterSettings();
            xmlws.Indent = true;
            xmlws.Encoding = Encoding.UTF8;
            XmlWriter xmlw = XmlWriter.Create(fs, xmlws);
            xmlw.WriteStartElement("LISTA");
            for (int i = 0; i < Lista.Capacity; i++)
            {
                xmlw.WriteElementString("Nombre", Lista[i].Nombre.ToString());
                xmlw.WriteBase64(Lista[i].Salt, 0, Lista[i].Salt.Length);
                xmlw.WriteBase64(Lista[i].ResuContra, 0, Lista[i].ResuContra.Length);


            }
            xmlw.WriteEndElement();
        }
    }
}
