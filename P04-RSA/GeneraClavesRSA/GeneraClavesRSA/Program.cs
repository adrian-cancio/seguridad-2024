using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace GeneraClavesRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda ayuda = new Ayuda();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            // Mostrar por consola las propiedades basicas del proveedor
            KeySizes legalKeySizes = rsa.LegalKeySizes[0];
            Console.WriteLine("Tamaños legales de las claves: [{0}, {1}]", legalKeySizes.MinSize, legalKeySizes.MaxSize);
            Console.WriteLine("Tamaño de la clave actual: {0}", rsa.KeySize);
            Console.WriteLine("¿La clave deve conservarse en el proveedor actual? {0}", rsa.PersistKeyInCsp);
            Console.WriteLine("Algoritmo de intercambio de claves: {0}", rsa.KeyExchangeAlgorithm);
            Console.WriteLine("¿El objeto contiene solo una clave publica? {0}", rsa.PublicOnly);
            Console.WriteLine();

            String rsaXml = rsa.ToXmlString(true);
            String rsaString = rsa.ToString();

            RSAParameters rsaParameters = rsa.ExportParameters(true);

            byte[] PrivateExponent = rsaParameters.D;
            byte[] PublicExponent = rsaParameters.Exponent;
            byte[] Module = rsaParameters.Modulus;
            byte[] PrimeP = rsaParameters.P;
            byte[] PrimeQ = rsaParameters.Q;

            Console.WriteLine("Exponente Privado:");
            ayuda.WriteHex(PrivateExponent, PrivateExponent.Length);
            Console.WriteLine("Exponente Público:");
            ayuda.WriteHex(PublicExponent, PublicExponent.Length);
            Console.WriteLine("Modulo:");
            ayuda.WriteHex(Module, Module.Length);
            Console.WriteLine("Número Primo P:");
            ayuda.WriteHex(PrimeP, PrimeP.Length);
            Console.WriteLine("Número Primo Q:");
            ayuda.WriteHex(PrimeQ, PrimeQ.Length);
            Console.WriteLine();

            // TODO: Guardar en fichero texto plano

            // Exportar a blob
            DateTime fecha = DateTime.Now;
            String rutaBase = "..\\..\\..\\..\\ClavesGeneradas\\";
            String baseFich = "zz_BlobRSA_" + fecha.ToString("yyyyMMdd_HHmmss");

            // Con clave privada
            byte[] cspBlobPriv = rsa.ExportCspBlob(true);
            String nombreFichBlobPriv = rutaBase + baseFich + "_Priv.bin";
            String rutaAbsolutaPriv = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + baseFich + "_Priv.bin";
            ayuda.GuardaBufer(nombreFichBlobPriv, cspBlobPriv);
            Console.WriteLine("Fichero con clave privada guardado en:\n{0}", rutaAbsolutaPriv);

            // Con clave pública
            byte[] cspBlobPubli = rsa.ExportCspBlob(false);
            String nombreFichBlobPubli = rutaBase + baseFich + "_Publi.bin";
            String rutaAbsolutaPubli = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + baseFich + "_Publi.bin";
            ayuda.GuardaBufer(nombreFichBlobPubli, cspBlobPubli);
            Console.WriteLine("Fichero con clave pública guardado en:\n{0}", rutaAbsolutaPubli);

            RSACryptoServiceProvider rsaImport = new RSACryptoServiceProvider();
            rsaImport.ImportCspBlob(cspBlobPriv);
        }
    }
}
