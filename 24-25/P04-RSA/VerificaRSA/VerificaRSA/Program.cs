using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace VerificaRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda ayuda = new Ayuda();
            byte[] mensaje = new byte[64];

            for (int i = 0; i < mensaje.Length; i++)
            {
                mensaje[i] = (byte)i;
            }

            Console.WriteLine("Mensaje");
            ayuda.WriteHex(mensaje, mensaje.Length);
            Console.WriteLine();

            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            Console.WriteLine("HashSize: {0}", sha256.HashSize);

            byte[] resumen = sha256.ComputeHash(mensaje);

            Console.WriteLine("Resumen:");
            ayuda.WriteHex(resumen, resumen.Length);
            Console.WriteLine();

            sha256.Dispose();

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            String nombreFichBlobPriv = "zz_BlobRSA_Priva.bin";

            Console.WriteLine("Logitud de la clave: {0} bits", rsa.KeySize);
            Console.WriteLine();

            byte[] fichBlobPriv = new byte[ayuda.BytesFichero(nombreFichBlobPriv)];

            ayuda.CargaBufer(nombreFichBlobPriv, fichBlobPriv);

            rsa.ImportCspBlob(fichBlobPriv);

            byte[] firma = rsa.SignHash(resumen, "SHA256");
            Console.WriteLine("Firma:");
            ayuda.WriteHex(firma, firma.Length);
            Console.WriteLine();

            byte[] firma2 = rsa.SignHash(resumen, "SHA256");
            Console.WriteLine("Firma 2:");
            ayuda.WriteHex(firma2, firma2.Length);
            Console.WriteLine();

            byte[] firma3 = rsa.SignData(mensaje, "SHA256");
            Console.WriteLine("Firma 3:");
            ayuda.WriteHex(firma3, firma3.Length);
            Console.WriteLine();

            String nomFichMensaje = "zz_Mensaje.bin";
            ayuda.GuardaBufer(nomFichMensaje, mensaje);

            String nomFichFirma = "zz_Firma.bin";
            ayuda.GuardaBufer(nomFichFirma, firma);

            /*
            mensaje[32] = 0x00;
            resumen[16] = 0xff;
            firma[0] = 0x0f;
            */
            bool vfr = rsa.VerifyHash(resumen, "SHA256", firma);

            Console.WriteLine("¿hash verificado correctamente? {0}", vfr);

            bool vfm = rsa.VerifyData(mensaje, "SHA256", firma);
            Console.WriteLine("¿datos verificados correctamente? {0}", vfm);



        }
    }
}
