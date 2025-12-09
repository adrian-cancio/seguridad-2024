using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio1_CERTS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            // Cargar los certificados emisor y receptor de la clase X509Certificate2 
            // llamando dos veces al metodo extraeCertificado
            X509Certificate2 certEmi = ExtraeCertificado("zmEMI2201as.pfx");
            X509Certificate2 certRec = ExtraeCertificado("zmREC2201as.pfx");


            // ------- TAREA 1 DEL RECEPTOR: CARGAR EL MENSAJE Y LA FIRMA ------- //

            long tamMsgCifrado = a.BytesFichero("zzMsgCifrado.bin");
            byte[] MsgCifrado = new byte[tamMsgCifrado];
            a.CargaBufer("zzMsgCifrado.bin", MsgCifrado);

            long tamFirma = a.BytesFichero("zzFirma.bin");
            byte[] MsgFirma = new byte[tamFirma];
            a.CargaBufer("zzMsgCifrado.bin", MsgFirma);


            // ------- TAREA 2 DEL RECEPTOR: DESCIFRAR EL MENSAJE CIFRADO ------- //

            RSACryptoServiceProvider rsaRec = (RSACryptoServiceProvider)certRec.PrivateKey;
            Console.WriteLine("Sujeto del certificado receptor: " + certRec.Subject);

            byte[] MsgDescifrado = rsaRec.Decrypt(MsgCifrado, true);

            Console.WriteLine("Mensaje descifrado: ");
            a.WriteHex(MsgDescifrado, MsgDescifrado.Length);


            // ------- TAREA 3 DEL RECEPTOR: VERIFICAR LA FIRMA DEL MENSAJE CIFRADO ------- //
            SHA512CryptoServiceProvider SHA512 = new SHA512CryptoServiceProvider();
            byte[] resumen = SHA512.ComputeHash(MsgCifrado);

            RSACryptoServiceProvider rsaEmi = (RSACryptoServiceProvider)certEmi.PublicKey.Key;
            Console.WriteLine("Sujeto del certificado emisor: " + certEmi.Subject);

            bool verificado = rsaEmi.VerifyHash(resumen, "SHA512", MsgFirma);
            Console.WriteLine("Firma del mensaje cifrado verificada? " + verificado);
        }

        public static X509Certificate2 ExtraeCertificado(String nombre)
        {
            // Objeto de la clase X509Store
            X509Store almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Mostramos las propiedades name y location del objeto almacen
            Console.WriteLine("Propiedad Name" + almacen.Name);
            Console.WriteLine("Propiedad Location" + almacen.Location);

            // Abrimos el objeto almacen con el método Open() mediante dos flags
            // utilizando el operador OR bit a bit: |
            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            // Objeto de la clase X509Certificate2Collection
            X509Certificate2Collection ColaCert = almacen.Certificates;

            // Cerramos el almacen mediante el método close()
            almacen.Close();

            foreach (X509Certificate cert in ColaCert)
            {
                Console.WriteLine("Nombre del sujeto " + cert.Subject);
            }

            // Mostramos por consola el numero total de certificados que contiene el almacen
            Console.WriteLine("Numero total de certificados " + ColaCert.Count);

            X509Certificate2 certBuscado = null;

            if (ColaCert.Count == 1)
            {
                certBuscado = ColaCert[0]; // Asignamos a certBuscado el unico elemento de ColaCert
            }

            else if (ColaCert.Count == 0)
            {
                Console.WriteLine("No se han encontrado certificados");
                Environment.Exit(1); // Salimos del programa
            }

            else if (ColaCert.Count > 1)
            {
                Console.WriteLine("Se ha encontrado mas de un certificado");
                Environment.Exit(1); // Salimos del programa            

            }

            return certBuscado;
        }
    }
}
