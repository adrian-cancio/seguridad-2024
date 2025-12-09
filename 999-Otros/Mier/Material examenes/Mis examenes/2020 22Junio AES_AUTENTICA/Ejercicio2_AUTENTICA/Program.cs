using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ejercicio2_AUTENTICA
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

        const int maxusu = 7;
        const int longnombre = 16;
        const int longsalt = 32;
        const int longcontra = 32;
        public char[] Nombre;
        public byte[] Salt;
        public byte[] ResuContra;


        public UsuContra()
        {
            Nombre = new char[longnombre];
            Salt = new byte[longsalt];
            ResuContra = new byte[longcontra];
        }


        public int Verifica(String nombreIn, String contraIn)
        {

            int contador = 0;
            bool encontrado = false;


            // llenamos el vector nombre hasta la longitud de entrada
            for(int i = 0; i < nombreIn.Length; i++)
            {
                Nombre[i] = nombreIn[i];
            }


            int numSaltChar = 44;
            char[] SaltChar = new char[numSaltChar];

            int numCarResuContra = 44;
            char[] resuContraChar = new char[numCarResuContra];

            // Creamos el filestream que lee el fichero
            FileStream fs = new FileStream("zz_ListaUsuarios.xml", FileMode.Open, FileAccess.Read);

            //Creamos el objeto XmlReaderSettings y el XmlReader que utilizará el anterior
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = true,
                DtdProcessing = DtdProcessing.Prohibit
            };
            XmlReader reader = XmlReader.Create(fs, settings);

            string nombreFich = "";
            string saltFich = "";
            string resumenFich = "";
            string adicionalFich = "";


            // Leemos las dos primeras lineas
            reader.Read();
            reader.ReadStartElement("LISTA");
            do
            {
                // Bucle while que lee cada usuario
                reader.ReadStartElement("USUARIO");

                reader.ReadStartElement("NOMBRE");
                nombreFich = reader.ReadElementString();
                Console.WriteLine(nombreFich);
                reader.ReadEndElement();

                reader.ReadStartElement("SALT");
                saltFich = reader.ReadElementString();
                Console.WriteLine(saltFich);
                reader.ReadEndElement();

                reader.ReadStartElement("RESUMEN");
                resumenFich = reader.ReadElementString();
                Console.WriteLine(resumenFich);
                reader.ReadEndElement();

                reader.ReadStartElement("ADICIONAL");
                adicionalFich = reader.ReadElementString();
                Console.WriteLine(adicionalFich);
                reader.ReadEndElement();

                reader.ReadEndElement();

                contador++;
                encontrado = nombreFich.IsNormalized() && nombreFich == nombreIn;

            } while(contador < maxusu && !encontrado);

            reader.Close();
            fs.Close();

            if (!encontrado)
                return 1;

            byte[] salt = Convert.FromBase64String(saltFich);
            byte[] resucontra = Convert.FromBase64String(resumenFich);

            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(contraIn, salt, 1000);
            byte[] resucontraIn = deriveBytes.GetBytes(longcontra);

            if(resucontra.SequenceEqual(resucontraIn)) return 0 ; else return 2;

        }






    }
}
