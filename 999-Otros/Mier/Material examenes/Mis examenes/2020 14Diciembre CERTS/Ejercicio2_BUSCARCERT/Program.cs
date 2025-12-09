using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio2_BUSCARCERT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            Console.WriteLine("Que tipo de certificado quieres buscar? Certificado raiz (AC) o Certificado de Usuario (USU): ");
            String tipocert = Console.ReadLine();

            while(!(tipocert.Equals("AC") || tipocert.Equals("USU")))
            {
                Console.WriteLine("Incorrecto. Los certificados solo pueden ser de tipo AC o USU: ");
                tipocert = Console.ReadLine();
            }

            Console.WriteLine("Introduzca el CN del certificado: ");
            string CN = Console.ReadLine();

            X509Store almacen = null;

            if(tipocert.Equals("AC"))
            {
                almacen = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            }
            if (tipocert.Equals("USU"))
            {
                almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            }

            // Abrimos el almacen
            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certificates = almacen.Certificates;

            // Imprimimos las propiedades del almacen
            Console.WriteLine("Nombre del almacen: " + almacen.Name);
            Console.WriteLine("Ubicacion del almacen: " + almacen.Location);

            // Buscamos el certificado
            X509Certificate2 certificate = null;

            bool encontrado = certificates.Find(X509FindType.FindBySubjectDistinguishedName, CN, false).Count == 1;

            if(encontrado)
            {
                certificate = certificates.Find(X509FindType.FindBySubjectDistinguishedName, CN, false)[0];
                Console.WriteLine("Certificado encontrado!!");
                Console.WriteLine("Certificado: " + certificate);
                Console.WriteLine("Numero de serie: " + certificate.SerialNumber);
                Console.WriteLine("¿Clave Privada?: " + certificate.HasPrivateKey);
            }
            else
            {
                Console.WriteLine("No se ha encontrado el certificado especificado :(");
                Environment.Exit(1);
            }

            int contCerts = certificates.Count;
            Console.WriteLine("Hay " + contCerts + "certificados en el almacén");

            Console.WriteLine("Hay " + certificates.Find(X509FindType.FindByTimeNotYetValid, DateTime.Now, false).Count + "certificados cuyo periodo de validez no se ha iniciado");
            Console.WriteLine("Hay " + certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false).Count + "certificados dentro del periodo de validez");
            Console.WriteLine("Hay " + certificates.Find(X509FindType.FindByTimeExpired, DateTime.Now, false).Count + "certificados que ya han expirado");


        }
    }
}
