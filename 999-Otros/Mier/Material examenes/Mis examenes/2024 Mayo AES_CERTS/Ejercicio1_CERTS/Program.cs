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

            // Crear un array de bytes denominado mensaje de 64 bytes
            byte[] Msg = new byte[64];
            for (int i = 0; i < Msg.Length; i++)
            {
                Msg[i] = (byte)(i % 256);
            }
            Console.WriteLine("Mensaje");
            a.WriteHex(Msg, Msg.Length);


            // Cargar los certificados emisor y receptor de la clase X509Certificate2 
            // llamando dos veces al metodo extraeCertificado
            X509Certificate2 certEmi = ExtraeCertificado("CN=zp24EMIas.pfx");
            X509Certificate2 certRec = ExtraeCertificado("zp24RECas.pfx");



            // ------- TAREA 1 DEL EMISOR: FIRMAR EL MENSAJE PLANO ORIGINAL -------
            // Crear un proveedor criptográfico
            RSACryptoServiceProvider rsaEmi = (RSACryptoServiceProvider)certEmi.PrivateKey;

            // Crear un proveedor para obtener la firma del mensaje usando algoritmo "SHA512"
            SHA512CryptoServiceProvider shaEmi = new SHA512CryptoServiceProvider();

            // Obtenemos el resumen
            byte[] resumen = shaEmi.ComputeHash(Msg);

            // Firmar el resumen
            byte[] firma = rsaEmi.SignHash(resumen, "SHA512");

            Console.WriteLine("Firma del mensaje");
            a.WriteHex(firma, firma.Length);


            // ------- TAREA 2 DEL EMISOR: CIFRAR EL MENSAJE PLANO -------
            RSACryptoServiceProvider rsaRec = (RSACryptoServiceProvider)certRec.PublicKey.Key;
            Console.WriteLine("Nombre del certificado: " + certRec.Subject);

            byte[] MsgCifrado = rsaRec.Encrypt(Msg, true);

            // Mostrar el mensaje cifrado en consola
            Console.WriteLine("Mensaje cifrado:");
            a.WriteHex(MsgCifrado, MsgCifrado.Length);


            // ------- TAREA 1 DEL RECEPTOR: DESCIFRAR EL MENSAJE PLANO -------
            RSACryptoServiceProvider rsaRecPriv = (RSACryptoServiceProvider)certRec.PrivateKey;
            Console.WriteLine("Nombre del certificado: " + certRec.Subject);

            byte[] MsgDescifrado = rsaRecPriv.Decrypt(MsgCifrado, true);

            // Mostrar el mensaje descifrado en consola
            Console.WriteLine("Mensaje descifrado:");
            a.WriteHex(MsgDescifrado, MsgDescifrado.Length);


            // ------- TAREA 2 DEL RECEPTOR: VERIFICAR EL MENSAJE DESCIFRADO -------
            byte[] resumenDescifrado = shaEmi.ComputeHash(MsgDescifrado);

            bool vertificado = rsaEmi.VerifyHash(resumenDescifrado, "SHA512", firma);

            Console.WriteLine(("Firma verificada: " + vertificado);

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
