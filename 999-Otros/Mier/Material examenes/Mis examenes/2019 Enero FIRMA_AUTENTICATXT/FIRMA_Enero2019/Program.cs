using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace FIRMA_Enero2019
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            String fichero = "zzUSU1901";

            // Hay que buscar el certificado
            X509Certificate2 cert = buscarCertificado(fichero);
            Console.WriteLine("Certificado: " + cert.Subject);
            

            // Calcular el resumen del fichero mediante el algoritmo SHA384
            SHA384CryptoServiceProvider sha = new SHA384CryptoServiceProvider();
            Console.WriteLine("Propiedad - tamaño hash generado: " + sha.HashSize);

            int n = (int)a.BytesFichero(fichero);
            FileStream fs = new FileStream("zzFichero.bin", FileMode.Open, FileAccess.Read);
            // Se crea el binary reader, que va a leer el filestream
            BinaryReader br = new BinaryReader(fs);

            // Se guarda en este vector de tipo byte el resumen
            byte[] resumen = sha.ComputeHash(br.ReadBytes(n));
            Console.WriteLine("Resumen: ");
            a.WriteHex(resumen, resumen.Length);


            // Cerramos el filestream y el binaryreader una vez se termina el resumen
            fs.Dispose();
            br.Dispose();



            // Crear un proveedor criptografico apropiado para el certificado con los siguientes pasos:
            // 1 - Exporta la clave privada asociada al certificado a un string en formato XML
            String claveXML = cert.PrivateKey.ToXmlString(true);

            // 2 - Crear el proveedor de tipo RSA
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            // 3 - Importar la clave en el proveedor
            rsa.FromXmlString(claveXML);


            // Generamos la firma digital llamando al metodo signHash del proveedor
            byte[] firma = rsa.SignHash(resumen, "SHA384");
            Console.WriteLine("Firma digital: ");
            a.WriteHex(firma, firma.Length);



            // Verificamos que la firma digital calculada es correcta
            // 1 - Crear un nuevo proveedor criptografico que emplee solo la clave publica contenida en el certificado
            RSACryptoServiceProvider rsa2 = (RSACryptoServiceProvider)cert.PublicKey.Key;

            // 2 - Introduce un par de sentencias para modificar el resumen o firma
            //firma[5] = 5;
            //firma[6] = 10;

            // Genera la verificacion llamando al metodo verifyHash del nuevo proveedor y mueestra en la consola el resultado de la verificacion
            Boolean verificacion = rsa2.VerifyHash(resumen, "SHA384", firma);
            Console.WriteLine("Firma valida VerifyHash?: " + verificacion);


        }

        static X509Certificate2 buscarCertificado(String nombre)
        {
            // Obtenemos el almacen en la localizacion CurrentUser
            X509Store almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Abrimos el almacen en modo lectura y solo existentes
            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            // Obtenemos todos los certificados del almacen
            X509Certificate2Collection certificates = almacen.Certificates;

            // Buscamos el certificado entre todos los certificados
            certificates = certificates.Find(X509FindType.FindBySubjectName, nombre, true);

            //Imprimimos por pantalla, es decir, para saber que hemos encontrado certs
            //con el sujeto en cuestion que se pasa por parametro
            Console.WriteLine("NumCertificados con nombre " + nombre + ":" +
                certificates.Find(X509FindType.FindBySubjectName, nombre, true).Count);

            return certificates[0];
        }
    }
}
