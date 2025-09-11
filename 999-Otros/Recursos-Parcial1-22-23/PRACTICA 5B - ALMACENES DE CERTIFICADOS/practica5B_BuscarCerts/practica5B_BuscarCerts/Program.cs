using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
namespace practica5B_BuscarCerts
{
    class Program
    {
        static void Main(string[] args)
        {
            //FASE 1
            //Creamos un objeto Almacen
            X509Store Almacen = new X509Store(StoreName.Root,
                StoreLocation.CurrentUser);
            //Como comprobación, mostramos algunas propiedades
            Console.WriteLine("Nombre: " + Almacen.Name);
            Console.WriteLine("Location: " + Almacen.Location);
            //Abrimos el almacen en modo lectura y que unicamente los que existan
            Almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            //Colocamos todos los certificados del almacen en un objeto de la clase siguiente
            X509Certificate2Collection ColeCert = Almacen.Certificates;
            //Mostramos los certificados de la coleción
            Console.WriteLine("Certificados de la coleccion: ");
            foreach(X509Certificate2 cert in ColeCert)
            {
                Console.WriteLine(cert.Subject);
            }
            //Mostramos el total de certificados que contiene el almacen
            Console.WriteLine("Total de certificados: " + ColeCert.Count);

            //FASE 2
            //Ahora toca buscar un certificado, tales que a fecha actual
            //periodo de validez empezado y no expirado
            X509Certificate2Collection ColeFind =
                ColeCert.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            //El parámetro findValue del método Find deberá ser un valor DateTime en hora local. 
            //Puede utilizar DateTime.Now para buscar todos los certificados actualmente válidos.
            //true para que la búsqueda sólo pueda devolver certificados válidos; de lo contrario, false.
            Console.WriteLine("Número de certificados en periodo de validez: " + ColeFind.Count);
            Console.WriteLine();

            ColeFind = ColeCert.Find(X509FindType.FindByTimeNotYetValid, DateTime.Now, false);
            Console.WriteLine("Número de certificados en periodo no válido (aun no son válidos): " + ColeFind.Count);
            Console.WriteLine();

            ColeFind = ColeCert.Find(X509FindType.FindByTimeExpired, DateTime.Now, false);
            Console.WriteLine("Número de certificados en periodo no válido (ya no son válidos): " + ColeFind.Count);
            Console.WriteLine();

            // Buscar por String
            String Nombre = "Microsoft";
            ColeFind = ColeCert.Find(X509FindType.FindBySubjectName, Nombre, false);
            Console.WriteLine("Número de certificados de Microsoft: " + ColeFind.Count);
            Console.WriteLine();

            Nombre = "CN=Microsoft Root Certificate Authority, DC=Microsoft, DC=com";       // No es sensible a mayusculas/minusculas, pero si a espacios
            ColeFind = ColeCert.Find(X509FindType.FindBySubjectDistinguishedName, Nombre, false);
            Console.WriteLine("Certificado Microsoft Root Certificate Authority:");
            if(ColeFind.Count == 0)
            {
                //No se han encontrado certificados
                Console.WriteLine("No se han encontrado certificados para la busqueda");
            }
            else
            {
                //Entonces hay certificados
                for(int i = 0; i < ColeFind.Count; i++)
                {
                    Console.WriteLine(ColeFind[i]);
                }
            }
            Console.WriteLine();

            //Ceramos el almacen
            Almacen.Close();

        }
    }
}
