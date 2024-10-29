using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
namespace practica5B_AddDeleteCerts
{
    class Program
    {
        static void Main(string[] args)
        {
            //Declaramos un objeto de la clase X509Store
            X509Store My = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            //4)Abre el almacen para lectura y escritura
            My.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            //5)Crea 3 objetos de la clase X509Certificate2 y pasale en el constructor los ficheros
            X509Certificate2 Cert1 = new X509Certificate2("zzACjosecarlos.cer");
            X509Certificate2 Cert2 = new X509Certificate2("zzCLI_Completo.cer");
            X509Certificate2 Cert3 = new X509Certificate2("zzSER_Completo.cer");
            //6)Añadir Cer1 a almacen
            My.Add(Cert1);
            VerCerts(My);
            //7) crea un objeto ColeCert, coleccion de certificados vacia
            X509Certificate2Collection ColeCert = new X509Certificate2Collection();
            ColeCert.Add(Cert2);
            ColeCert.Add(Cert3);
            //8)Añade un rango de certificados a almacen 
            //el rango se definde con la coleccion coleCert
            //AddRangeS
            Console.WriteLine("Añadir CERT2-3 al Almacen");
            My.AddRange(ColeCert);
            VerCerts(My);

            //9) elimina el certificado con el metodo remove 
            Console.WriteLine("Eliminar CERT1 del Almacen");
            My.Remove(Cert1);
            VerCerts(My);

            //10)elimina un rango de certificados de almacen
            //el rango se define con la coleccion ColeCert
            //RemoveRange
            Console.WriteLine("Eliminar CERT2-3 del Almacen");
            My.RemoveRange(ColeCert);
            VerCerts(My);

            //11)cierra el almacen 
            My.Close();

        }



        //2) clase con el metodo VerCerts o metodo estatico junto al main
        //muestra en consola el nombre del almacen y debajo el nombre del sujeto de cada certificado

        static void VerCerts(X509Store AL)
        {
            Console.Out.WriteLine("Objeto X509Store");
            Console.Out.WriteLine("Name: " + AL.Name);
            Console.Out.WriteLine("Location: " + AL.Location);

            X509Certificate2Collection ColeCert = AL.Certificates;

            Console.Out.WriteLine("\nObjeto X509Certificate2Collection");
            foreach (X509Certificate2 item in ColeCert)
            {
                Console.Out.WriteLine("SubjectName: " + item.Subject);
            }
            Console.Out.WriteLine("Nº de certificados: " + ColeCert.Count);
        }


    }
}

