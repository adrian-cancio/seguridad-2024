using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace CifradoRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Ayuda ayuda = new Ayuda();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            Console.WriteLine("Longitud de la clave: {0} bits", rsa.KeySize);

            String rutaBase = "..\\..\\..\\..\\Compartido\\";
            String rutaBaseAbsoluta = Path.GetFullPath(rutaBase);
            if (!Directory.Exists(rutaBase))
            {
                Console.WriteLine("No se encuentra la carpeta Compartido en la ruta: {0}", rutaBaseAbsoluta);
                return;
            }

            // Lista de ficheros de claves dentro de la carpeta ClavesGeneradas con el patron zz_BlobRSA*_Publi.bin
            String[] listaFicheros = Directory.GetFiles(rutaBase, "zz_BlobRSA*_Publi.bin");

            //String nombreFichBlobPubli = "zz_BlobRSA_Publi.bin";

            // Elegimos la ultima clave publica generada
            String nombreFichBlobPubli = listaFicheros[listaFicheros.Length - 1];


            byte[] blobFich = new byte[ayuda.BytesFichero(nombreFichBlobPubli)];

            ayuda.CargaBufer(nombreFichBlobPubli, blobFich);

            rsa.ImportCspBlob(blobFich);

            RSAParameters rsaParameters = rsa.ExportParameters(false);

            Console.WriteLine("Exponente público:");
            ayuda.WriteHex(rsaParameters.Exponent, rsaParameters.Exponent.Length);
            Console.WriteLine("Módulo:");
            ayuda.WriteHex(rsaParameters.Modulus, rsaParameters.Modulus.Length);
            Console.WriteLine();

            Console.WriteLine("Introduce el texto plano: ");
            String textoPlanoStr = Console.ReadLine();
            byte[] textoPlano = Encoding.UTF8.GetBytes(textoPlanoStr);

            Console.WriteLine();
            Console.WriteLine("Texto plano en bytes:");
            ayuda.WriteHex(textoPlano, textoPlano.Length);
            Console.WriteLine();

            byte[] textoCifrado = rsa.Encrypt(textoPlano, true);

            Console.WriteLine("Texto cifrado:");
            ayuda.WriteHex(textoCifrado, textoCifrado.Length);

            Console.WriteLine();

            DateTime dateTime = DateTime.Now;


            String nombreFichCifrado = "zz_TextoCifrado_"+ dateTime.ToString("yyyyMMddHHmmss") + ".bin";
            Console.WriteLine("Guardando el texto cifrado en el fichero:\n{0}", rutaBaseAbsoluta+nombreFichCifrado);

            ayuda.GuardaBufer(nombreFichCifrado, textoCifrado);



        }
    }
}
