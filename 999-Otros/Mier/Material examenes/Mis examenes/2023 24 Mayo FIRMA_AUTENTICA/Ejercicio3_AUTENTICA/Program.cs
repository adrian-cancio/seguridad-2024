using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ejercicio3_AUTENTICA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Introduce usuario: ");
            string nombre = Console.ReadLine();
            Console.WriteLine("Introduce contraseña: ");
            string contra = Console.ReadLine(); ;
            UsuContra usuContra = new UsuContra();
            //int resultado = usuContra.Verifica("Eduardo", "conE");
            int resultado = usuContra.Verifica(nombre, contra);
            switch (resultado)
            {
                case 0:
                    Console.WriteLine("Usuario correcto");
                    break;
                case 1:
                    Console.WriteLine("Usuario no valido");
                    break;
                case 2:
                    Console.WriteLine("Contraseña incorrecta");
                    break;
                default:
                    break;

            }
        }
    }

    class UsuContra
    {
        const int maxUsu = 9;
        const int longNombre = 16;
        const int longSalt = 16;
        const int longContra = 16;

        public char[] Nombre;
        public byte[] Salt;
        public byte[] ResuContra;

        public UsuContra()
        {
            Nombre = new char[longNombre];
            Salt = new byte[longSalt];
            ResuContra = new byte[longContra];
        }

        public int Verifica(string NombreIn, string ContraIn)
        {
            int contador = 0;
            bool encontrado = false;

            for(int i = 0; i < NombreIn.Length; i++)
            {
                Nombre[i] = NombreIn[i];
            }

            // Creamos el filstream
            FileStream fs = new FileStream("zz_ListaUsuarios.xml",FileMode.Open, FileAccess.Read);

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
            {
                CheckCharacters = true,
                DtdProcessing = DtdProcessing.Prohibit
            };

            XmlReader xmlreader = XmlReader.Create(fs, xmlReaderSettings);

            string nombreFich = "";
            string saltFich = "";
            string resumenFich = "";
            string tipoUsuFich = "";

            // Leer los usuarios XML
            xmlreader.Read();
            xmlreader.ReadStartElement("LISTA");

            do
            {
                xmlreader.ReadStartElement("USUARIO");

                xmlreader.ReadStartElement("NOMBRE");
                nombreFich = xmlreader.ReadElementString();
                Console.WriteLine(nombreFich);
                xmlreader.ReadEndElement();

                xmlreader.ReadStartElement("SALT");
                saltFich = xmlreader.ReadElementString();
                Console.WriteLine(saltFich);
                xmlreader.ReadEndElement();

                xmlreader.ReadStartElement("RESUMEN");
                resumenFich = xmlreader.ReadElementString();
                Console.WriteLine(resumenFich);
                xmlreader.ReadEndElement();

                xmlreader.ReadStartElement("TIPOUSU");
                nombreFich = xmlreader.ReadElementString();
                Console.WriteLine(nombreFich);
                xmlreader.ReadEndElement();

                xmlreader.ReadEndElement();

                encontrado = nombreFich.IsNormalized() && nombreFich == NombreIn;

                contador++;


            } while (contador < maxUsu && !encontrado);

            xmlreader.Close();
            fs.Close();

            if (!encontrado)
                return 1;

            byte[] salt = Convert.FromBase64String(saltFich);
            byte[] contra = Convert.FromBase64String(resumenFich);

            // Crear el resumen de la contraseña con 2000 iteraciones
            Rfc2898DeriveBytes derivebytes = new Rfc2898DeriveBytes(ContraIn, salt, 2000);
            byte[] contraIn = derivebytes.GetBytes(longContra);
            if (contra.SequenceEqual(contraIn))
                return 0;
            else
                return 2;
        }
    }

    
}
