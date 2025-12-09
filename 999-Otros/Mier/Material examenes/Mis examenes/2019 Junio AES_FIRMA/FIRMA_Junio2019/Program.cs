using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace FIRMA_Junio2019
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            String fichero = "zzFichero.bin";

            // Buscar el certificado en el almacen
            X509Certificate2 certificado = buscarCertificado();
            Console.WriteLine("Nombre del certificado: " + certificado.SubjectName.Name);

            // Se calcula el resumen del fichero zzFichero
            int n = (int)a.BytesFichero(fichero);
            FileStream fs = new FileStream(fichero, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            SHA512CryptoServiceProvider sHA512CryptoServiceProvider = new SHA512CryptoServiceProvider();
            byte[] resumen = sHA512CryptoServiceProvider.ComputeHash(br.ReadBytes(n));

            // Mostrar el resumen por la consola
            Console.WriteLine("Resumen: ");
            a.WriteHex(resumen, resumen.Length);

            fs.Dispose();
            br.Dispose();


            // Crear un proovedor criptografico apropiado para el certificado:
            // 1 - Exporta la clave asociada al certificado a un string en formato xml
            String xml = certificado.PrivateKey.ToXmlString(true);

            // 2 - Crear el proveedor
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();

            // 3 - Importar la clave en el proveedor
            rSACryptoServiceProvider.FromXmlString(xml);


            // generar la firma llamando al metodo signhash
            byte[] firma = rSACryptoServiceProvider.SignHash(resumen, "SHA512");
            Console.WriteLine("Firma: ");
            a.WriteHex(firma, firma.Length);


            // Verifica que la firma digital calculada es correcta realizando los pasos siguientes:
            //  1 - Crea un nuevo proveedor criptografico que emplee solo la clave publica obtenida en el certificado
            RSACryptoServiceProvider rSACryptoServiceProvider2 = (RSACryptoServiceProvider)certificado.PublicKey.Key;

            // 2 - Introduce un par de sentencias para modificar el resumen o firma
            //firma[5] = 5;
            //firma[6] = 10;

            // 3 - Generar la verificación llamando al método verifyhash del nuevo proveedor y muestra por consola el estado
            bool verificado = rSACryptoServiceProvider2.VerifyHash(resumen, "SHA512", firma);
            Console.WriteLine("Verificado: " + verificado);

            Console.ReadLine();

        }

        public static X509Certificate2 buscarCertificado()
        {
            // Crear objeto almacen
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Abrir el almacen en modo lectura y solo existentes
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            // Coloca a todos los certificados del almacén en un objeto de clase X509Certificate2Collection
            X509Certificate2Collection CertUser = new X509Certificate2Collection(store.Certificates);

            CertUser = CertUser.Find(X509FindType.FindBySubjectName, "zzUSU1905", false);

            return CertUser[0];

        }
    }
}
