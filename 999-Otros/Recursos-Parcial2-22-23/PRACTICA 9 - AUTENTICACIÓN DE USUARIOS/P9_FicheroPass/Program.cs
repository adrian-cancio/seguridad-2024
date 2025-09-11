using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace P9_FicheroPass
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
        public byte[] Salt=new byte[lSalt];
        public byte[] ResuContra = new byte[lResu];

        public User(string NuevoNombre, string NuevaContra)
        {
            //Asignar el nombre
            for(int i = 0; i < Nombre.Length; i++)
            {
                Nombre[i] = '\0';
            }
            for(int j = 0; j < NuevoNombre.Length; j++)
            {
                Nombre[j] = NuevoNombre[j];
            }
            //Generar Salt
            RNGCryptoServiceProvider proveedor = new RNGCryptoServiceProvider();
            proveedor.GetBytes(Salt);
            //Genera el resumen
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(NuevaContra, Salt);
            rfc.IterationCount = 1000;
            ResuContra=rfc.GetBytes(ResuContra.Length);

        }
    }
    public class ListaU
    {
        static int MaxUsu=5;
        User[] Lista = new User[MaxUsu];

        public void IniUsu(int Indice, User Usuario)
        {
            Lista[Indice] = Usuario;
        }

        public void VerLista()
        {
            for(int i = 0; i < Lista.Length; i++)
            {
                Console.WriteLine(Lista[i].Nombre);
            }
        }

        public void GuardaListaBin(string NombreFich)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs,Encoding.Unicode);
            for(int i = 0; i < Lista.Length; i++)
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
            FileStream fs=new FileStream(NombreFich, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
            for (int i = 0; i < Lista.Length; i++)
            {
                sw.Write(Lista[i].Nombre);
                sw.Write(Convert.ToBase64String(Lista[i].Salt));
                sw.Write(Convert.ToBase64String(Lista[i].ResuContra));
                sw.WriteLine();
            }
            sw.Close();
            fs.Close();
        }
    }
}
