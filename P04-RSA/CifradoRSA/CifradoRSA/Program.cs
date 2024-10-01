using System;
using System.Collections.Generic;
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

            String nombreFichBlobPubli = "zz_BlobRSA_Publi.bin";

            byte[] blobFich = new byte[ayuda.BytesFichero(nombreFichBlobPubli)];

            ayuda.CargaBufer(nombreFichBlobPubli, blobFich);

            rsa.ImportCspBlob(blobFich);

            RSAParameters rsaParameters = rsa.ExportParameters(false);

            Console.WriteLine("Exponente público:");
            ayuda.WriteHex(rsaParameters.Exponent, rsaParameters.Exponent.Length);
            Console.WriteLine("Módulo:");
            ayuda.WriteHex(rsaParameters.Modulus, rsaParameters.Modulus.Length);
            Console.WriteLine();

            byte[] textoPlano = new byte[64];

            for (int i = 0; i < textoPlano.Length; i++)
            {
                textoPlano[i] = (byte) i;
            }

            Console.WriteLine("Texto plano:");
            ayuda.WriteHex(textoPlano, textoPlano.Length);
            Console.WriteLine();

            byte[] textoCifrado = rsa.Encrypt(textoPlano, true);

            Console.WriteLine("Texto cifrado:");
            ayuda.WriteHex(textoCifrado, textoCifrado.Length);

            String nombreFichCifrado = "zz_TextoCifrado.bin";

            ayuda.GuardaBufer(nombreFichCifrado, textoCifrado);



        }
    }
}
