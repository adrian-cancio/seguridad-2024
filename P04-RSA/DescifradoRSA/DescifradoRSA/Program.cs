using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace DescifradoRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda ayuda = new Ayuda();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            String rutaBase = "..\\..\\..\\..\\Compartido\\";
            String[] listaFicheros = Directory.GetFiles(rutaBase, "zz_BlobRSA*_Priv.bin");

            String nombreFichBlobPriv = listaFicheros[listaFicheros.Length - 1];

            byte[] fichBlobPriv = new byte[ayuda.BytesFichero(nombreFichBlobPriv)];

            ayuda.CargaBufer(nombreFichBlobPriv, fichBlobPriv);

            rsa.ImportCspBlob(fichBlobPriv);

            RSAParameters rsaParameters = rsa.ExportParameters(true);

            Console.WriteLine("Exponente público:");
            ayuda.WriteHex(rsaParameters.Exponent, rsaParameters.Exponent.Length);
            Console.WriteLine("Exponente privado:");
            ayuda.WriteHex(rsaParameters.D, rsaParameters.D.Length);  
            Console.WriteLine("Módulo:");
            ayuda.WriteHex(rsaParameters.Modulus, rsaParameters.Modulus.Length);
            Console.WriteLine("Primo P:");
            ayuda.WriteHex(rsaParameters.P, rsaParameters.P.Length);
            Console.WriteLine("Primo Q:");
            ayuda.WriteHex(rsaParameters.Q, rsaParameters.Q.Length);
            Console.WriteLine();

            String[] fichCifrados = Directory.GetFiles(rutaBase, "zz_TextoCifrado_*.bin");
            byte[] textoCifrado;
            byte[] textoPlano;
            // Recorrer todos los archivos cifrados y ver si ya estan descifrados
            for (int i = 0; i < fichCifrados.Length; i++)
            {
                // Comprobar si existe esa version del archivo archivo descifrado
                String nombreFichDescif = fichCifrados[i].Replace("TextoCifrado", "TextoDescifrado");
                nombreFichDescif = nombreFichDescif.Replace(".bin", ".txt");

                if (!File.Exists(nombreFichDescif))
                {
                    textoCifrado = new byte[ayuda.BytesFichero(fichCifrados[i])];
                    ayuda.CargaBufer(fichCifrados[i], textoCifrado);
                    Console.WriteLine("Texto cifrado:");
                    ayuda.WriteHex(textoCifrado, textoCifrado.Length);
                    Console.WriteLine();

                    textoPlano = rsa.Decrypt(textoCifrado, true);
                    Console.WriteLine("Texto descifrado:");
                    ayuda.WriteHex(textoPlano, textoPlano.Length);

                    ayuda.GuardaBufer(nombreFichDescif, textoPlano);
                }
            }
          

            /*
            byte[] textoCifrado = new byte[ayuda.BytesFichero(nombreFichCifr)];
            ayuda.CargaBufer(nombreFichCifr, textoCifrado);
            Console.WriteLine("Texto cifrado:");
            ayuda.WriteHex(textoCifrado, textoCifrado.Length);
            Console.WriteLine();

            byte[] textoPlano = rsa.Decrypt(textoCifrado, true);
            Console.WriteLine("Texto descifrado:");
            ayuda.WriteHex(textoPlano, textoPlano.Length);
            */

        }
    }
}
