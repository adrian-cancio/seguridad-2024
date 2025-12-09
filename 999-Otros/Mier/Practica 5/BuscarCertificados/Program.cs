using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BuscarCertificados
{
    internal class Program
    {
        static void Main(string[] args)
        {
            String nombre = "Microsoft";
            nombre = "Microsoft Root Certificate Authority";
            X509Certificate2 cert = ExtraeCert(nombre);
            AnadirEliminar();
        }

        static X509Certificate2 ExtraeCert(string Nombre)
        {



            //////////////////////////////////////////////////////////////////////////////////////////////
            /// --- PARTE 1: Acceso al almacén de certificados de las Autoridades de Certificación --- ///
            //////////////////////////////////////////////////////////////////////////////////////////////
            


            Console.WriteLine("\n\n ---------------------------");
            Console.WriteLine("| METODO EXTRAE CERTIFICADO |");
            Console.WriteLine(" ---------------------------\n\n");

            Console.WriteLine("\nPARTE 1: ACCESO AL ALMACEN DE CERTIFICADOS DE LAS AUTORIDADES DE CERTIFICACION\n\n");



            ///////////////////////////////////////////////////////////////////
            /// --- Fase 1: Abrir el almacen y extraer sus certificados --- ///
            ///////////////////////////////////////////////////////////////////
            


            Console.WriteLine("Fase 1: ABRIR EL ALMACEN Y EXTRAER SUS CERTIFICADOS\n\n");

            // 1) Crear un objeto almacen y certificado
            X509Store almacen = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            X509Certificate2 certi = null;

            // 2) Mostrar las propiedades
            Console.WriteLine("Propiedad Name del almacen: " + almacen.Name);
            Console.WriteLine("Propiedad Location del almacen: " + almacen.Location);

            // 3) Abrir almacen con el metodo open()
            // Usa dos flags, uno para abrirlo en solo lectura y otro para almacenes ya existentes
            // Debes combinar los flags con el operador OR bit a bit: |
            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            // 4) Colocar todos los certificados del almacen en un objeto (colecert)
            X509Certificate2Collection ColeCert = almacen.Certificates;

            // 5) Cerrar el almacen
            almacen.Close();

            // 6) Mostrar los certificados contenidos en la colección ColeCert con bucle foreach
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine("[Certificados de la coleccion]: \n\n");
            foreach (X509Certificate2 cert in ColeCert)
            {
                Console.WriteLine(cert.Subject + "\n");
            }

            // 7) Mostrar el total de certificados que contiene el almacen
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine("[Total de certificados]: " + ColeCert.Count);
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");



            //////////////////////////////////////////
            /// --- Fase 2: Buscar certificado --- ///
            //////////////////////////////////////////
            


            Console.WriteLine("\nFase 2: BUSCAR CERTIFICADO\n\n");

            // EJEMPLO 1: Certificados que cumplen alguna condicion
            Console.WriteLine("\nEJEMPLO 1: CERTIFICADOS QUE CUMPLEN ALGUNA CONDICION\n");

            // Objeto certificado que cumple la condicion nulo
            X509Certificate2Collection ColeFind = null;

            // Certificados a fecha actual, periodo de validez empezado y no expirado
            //DateTime.Now filtra por todos los certificados actualmente válidos.
            //true para que la búsqueda sólo pueda devolver certificados válidos;
            //false para que pueda devolver tanto certificados validos como no validos
            ColeFind = ColeCert.Find(X509FindType.FindByTimeValid, DateTime.Now, true);

            // Mostramos el total de certificados no expirados
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine("[Total de certificados no expirados]: " + ColeFind.Count);
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // Realizar la busqueda, pero con los certificados que hayan expirado
            // Certificados a fecha actual, periodo de validez empezado y expirado
            ColeFind = ColeCert.Find(X509FindType.FindByTimeExpired, DateTime.Now, false);

            // Mostramos el total de certificados expirados
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine("[Total de certificados expirados]: " + ColeFind.Count);
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // EJEMPLO 2: Todos los certificados de una entidad
            Console.WriteLine("\n\nEJEMPLO 2: TODOS LOS CERTIFICADOS DE UNA ENTIDAD\n");

            // Nombre de lo que se quiere buscar (no es case sensitive, pero si a blank spaces)
            Nombre = "Microsoft";

            // Certificados que contienen el nombre
            ColeFind = ColeCert.Find(X509FindType.FindBySubjectName, Nombre, false);

            // Mostrarlos por pantalla
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine("\n[Total de certificados de Microsoft]: " + ColeFind.Count + "\n\n");

            //Si la busqueda encuentra certificados, los imprimimos
            if (ColeFind.Count > 0)
            {
                foreach (X509Certificate2 cert in ColeFind)
                {
                    Console.WriteLine(cert.Subject + "\n");
                }
            }
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // EJEMPLO 3: Certificado concreto de una entidad
            Console.WriteLine("\n\nEJEMPLO 3: CERTIFICADO CONCRETO DE UNA ENTIDAD\n");

            // Nombre de lo que se quiere buscar (no es case sensitive, pero si a blank spaces)
            Nombre = "Microsoft Root Certificate Authority";

            // Certificados que contienen el nombre
            ColeFind = ColeCert.Find(X509FindType.FindBySubjectName, Nombre, false);

            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // Mostrarlos por pantalla
            Console.WriteLine("\n[Total de certificados de Microsoft Root Certificate Authority]: " + ColeFind.Count + "\n\n");

            //Si la busqueda encuentra certificados, los imprimimos
            if (ColeFind.Count > 0)
            {
                foreach (X509Certificate2 cert in ColeFind)
                {
                    Console.WriteLine(cert.Subject + "\n");
                }
            }
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // TODOS: Liberar recursos
            almacen.Close();
            return certi;
        }

        static void AnadirEliminar()
        {



            ////////////////////////////////////////////////////////
            /// --- PARTE 2: Anadir y eliiminar certificados --- ///
            ////////////////////////////////////////////////////////



            Console.WriteLine("\n\n --------------------------------------");
            Console.WriteLine("| METODO ANADIR Y ELIMINAR CERTIFICADO |");
            Console.WriteLine(" --------------------------------------\n\n");

            // 3) Crea un objeto Almacen de la clase X509Store 
            X509Store almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // 4) Abrimos el almacen en modo lectura y escritura
            almacen.Open(OpenFlags.ReadWrite);

            // 5) Crea los objetos, Cert1, Cert2 y Cert3 de la clase X509Certificate2
            X509Certificate2 cert1 = new X509Certificate2("zpACas.cer");
            X509Certificate2 cert2 = new X509Certificate2("zpSERas.cer");
            X509Certificate2 cert3 = new X509Certificate2("zpUSUas.cer");

            // 6) Añade el certificado Cert1 a Almacen usando el método Add() <--------------- AÑADIR
            almacen.Add(cert1);

            // Mostrar el contenido
            Console.WriteLine("[Certificados del almacen tras añadir cert1]:");
            Console.WriteLine("Nombre: " + almacen.Name);
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            foreach (X509Certificate2 cert in almacen.Certificates)
            {
                Console.WriteLine(cert.Subject + "\n");
            }
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // 7) Crear un objeto ColeCert
            X509Certificate2Collection ColeCert = new X509Certificate2Collection();

            // Añadir los certificados restantes
            ColeCert.Add(cert2);
            ColeCert.Add(cert3);

            // 8) Añade un rango de certificados a Almacen
            almacen.AddRange(ColeCert);

            // Mostrar el contenido de Almacen
            Console.WriteLine("[Certificados del almacen tras añadir el rango]:");
            Console.WriteLine("Nombre: " + almacen.Name);
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            foreach (X509Certificate2 cert in almacen.Certificates)
            {
                Console.WriteLine(cert.Subject + "\n");
            }
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // 9) Eliminar el certificado cert1    <-------------------------------- ELIMINAR
            almacen.Remove(cert1);

            // Mostrar el contenido del almacen
            Console.WriteLine("[Certificados del almacen tras eliminar cert1]:");
            Console.WriteLine("Nombre: " + almacen.Name);
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            foreach (X509Certificate2 cert in almacen.Certificates)
            {
                Console.WriteLine(cert.Subject + "\n");
            }
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // 10) Eliminar el rango de certificado
            almacen.RemoveRange(ColeCert);

            // Mostrar el contenido del almacen
            Console.WriteLine("[Certificados del almacen tras eliminar el rango]:");
            Console.WriteLine("Nombre: " + almacen.Name);
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");
            foreach (X509Certificate2 cert in almacen.Certificates)
            {
                Console.WriteLine(cert.Subject + "\n");
            }
            Console.WriteLine("\n---------------------------------------------------------------------------------------------------------------------\n");

            // 11) Liberar recursos del almacen
            almacen.Close();
        }
    }
}
