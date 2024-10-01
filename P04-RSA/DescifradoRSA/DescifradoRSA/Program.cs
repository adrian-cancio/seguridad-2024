using System;
using System.Collections.Generic;
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

            String nombreFichBlobPriv = "zz_BlobRSA_Priva.bin";

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

            String nombreFichCifr = "zz_TextoCifrado.bin";

            byte[] textoCifrado = new byte[ayuda.BytesFichero(nombreFichCifr)];
            ayuda.CargaBufer(nombreFichCifr, textoCifrado);
            Console.WriteLine("Texto cifrado:");
            ayuda.WriteHex(textoCifrado, textoCifrado.Length);
            Console.WriteLine();

            byte[] textoPlano = rsa.Decrypt(textoCifrado, true);
            Console.WriteLine("Texto descifrado:");
            ayuda.WriteHex(textoPlano, textoPlano.Length);


        }
    }
}
