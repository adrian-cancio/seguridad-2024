using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSAySHA
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // --------------------   EJECUTAR ESTE CODIGO COMO ADMINISTRADOR   -------------------- //


            // FASE 1: Crear un contenedor de clave RSA y ver sus propiedades

            // 1) Crear el objeto ParamCSP de la clase CspParameters
            CspParameters ParamCSP = new CspParameters();

            // 2) Asignar valores a los campos de ParamCSP
            ParamCSP.ProviderType = 1; // Tipo de proveedor RSA por defecto
            ParamCSP.ProviderName = "Microsoft Strong Cryptographic Provider"; // Proveedor criptográfico
            ParamCSP.KeyContainerName = "PRACTICAS"; // Nombre del contenedor de la clave
            ParamCSP.KeyNumber = (int)KeyNumber.Exchange; // Tipo de clave (Exchange o Signature)

            // 3) Asignar Flag para usar el almacén de claves de la máquina en lugar del usuario
            ParamCSP.Flags = CspProviderFlags.UseMachineKeyStore;

            // 4) Crear el proveedor de servicios criptográficos RSA usando ParamCSP
            using (RSACryptoServiceProvider ProvRSA = new RSACryptoServiceProvider(1024, ParamCSP))
            {
                // 5) Establecer persistencia de la clave en el contenedor
                ProvRSA.PersistKeyInCsp = true;

                // 6) Mostrar propiedades del proveedor RSA en consola
                Console.WriteLine("Propiedades del objeto RSACryptoServiceProvider:");
                Console.WriteLine("Tamaño de clave: " + ProvRSA.KeySize);
                Console.WriteLine("PersistKeyInCsp: " + ProvRSA.PersistKeyInCsp);
                Console.WriteLine("PublicOnly: " + ProvRSA.PublicOnly);
                Console.WriteLine("Algoritmo de intercambio de clave: " + ProvRSA.KeyExchangeAlgorithm);
                Console.WriteLine("Algoritmo de firma: " + ProvRSA.SignatureAlgorithm);
                Console.WriteLine("UseMachineKeyStore: " + RSACryptoServiceProvider.UseMachineKeyStore);

                // 7) Mostrar la clave en formato XML
                Console.WriteLine("\nClave RSA en formato XML:\n" + ProvRSA.ToXmlString(true));

                // 8) Mostrar información sobre el contenedor de la clave
                VerContenedor(ProvRSA.CspKeyContainerInfo);
            }

            // FASE 2: Cargar la clave RSA de un contenedor en un nuevo proveedor RSA
            Console.WriteLine("\nCargando clave RSA del contenedor en un nuevo proveedor...");

            using (RSACryptoServiceProvider ProvRSA2 = new RSACryptoServiceProvider(1024, ParamCSP))
            {
                // 2) Mostrar la clave en formato XML para verificar que es idéntica a la original
                Console.WriteLine("\nClave RSA cargada en nuevo proveedor en formato XML:\n" + ProvRSA2.ToXmlString(true));
            }

            // FASE 3: Observar el almacén donde se creó el contenedor
            Console.WriteLine("\nPulsa una tecla para continuar y ver el contenedor en el directorio del sistema...");
            Console.ReadKey();

            // FASE 4: Eliminar el contenedor del almacén
            Console.WriteLine("\nEliminando contenedor de clave RSA...");

            using (RSACryptoServiceProvider ProvRSA3 = new RSACryptoServiceProvider(1024, ParamCSP))
            {
                // 2) Configurar la propiedad PersistKeyInCsp a false para permitir la eliminación del contenedor
                ProvRSA3.PersistKeyInCsp = false;

                // 3) Mostrar la clave en formato XML antes de eliminarla
                Console.WriteLine("\nClave RSA antes de eliminar contenedor:\n" + ProvRSA3.ToXmlString(true));
            }

            Console.WriteLine("\nContenedor eliminado. Presiona una tecla para salir.");
            Console.ReadKey();
        }

        // Método estático VerContenedor para mostrar propiedades de CspKeyContainerInfo
        static void VerContenedor(CspKeyContainerInfo infoConten)
        {
            Console.WriteLine("\nInformación del contenedor de claves:");
            Console.WriteLine("Nombre del contenedor: " + infoConten.KeyContainerName);
            Console.WriteLine("Nombre del proveedor: " + infoConten.ProviderName);
            Console.WriteLine("Tipo de proveedor: " + infoConten.ProviderType);
            Console.WriteLine("Es exportable: " + infoConten.Exportable);
            Console.WriteLine("Ubicación del contenedor de claves: " + (infoConten.MachineKeyStore ? "Almacén de la máquina" : "Almacén del usuario"));
        }
    }
}
