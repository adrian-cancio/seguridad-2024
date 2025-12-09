using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio1_FIRMA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            String nombreCert = "zzUSU1905";

            X509Certificate2 cert = ExtraeCertificado(nombreCert);
            Console.WriteLine("Nombre del certificado: " + cert.Subject);

            // Calcular el resumen del fichero
            SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider();
            FileStream fs = File.OpenRead("zzFichero.bin");
            byte[] resumen = sha.ComputeHash(fs);
            a.WriteHex(resumen, resumen.Length);


            // Creamos un proveedor criptografico con los pasos siguientes:
            // exporta la clave privada asociada al certificado a un string en formato xml
            string xml = cert.PrivateKey.ToXmlString(true);
            // crea el proveedor
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            // Importar la clave en el proveedor
            rsa.FromXmlString(xml);


            // Generar la firma digital llamando al metodo signhash del proveedor
            byte[] firma = rsa.SignHash(resumen, "SHA512");
            Console.WriteLine("Firma: ");
            a.WriteHex(firma, firma.Length);


            // Para verificar que la firma digital calculada es correcta se realizan los siguientes pasos:
            // Crear un nuevo proveedor criptografico que emplee solo la clave publica
            RSACryptoServiceProvider rsa_public = (RSACryptoServiceProvider)cert.PublicKey.Key;
            // Verificamos llamando al metodo verifyhash
            bool verificacion = rsa_public.VerifyHash(resumen, "SHA512", firma);
            Console.WriteLine("Verificacion: " + verificacion);

            // Cerramos todo
            rsa.Dispose();
            rsa_public.Dispose();


        }

        static X509Certificate2 ExtraeCertificado(string nombreCertificado)
        {
            X509Store AlmaUser = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            AlmaUser.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection CertsUser = AlmaUser.Certificates;
            X509Certificate2 n = CertsUser.Find(X509FindType.FindBySubjectName, nombreCertificado, true)[0];

            return n;

        }
    }


}
