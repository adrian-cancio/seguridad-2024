using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace Ejercicio2_FIRMA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            byte[] mensaje = new byte[64];

            // Rellenar mensaje con los valores 0x00, 0x01, 0x02...
            for(int i = 0; i < mensaje.Length; i++)
            {
                mensaje[i] = (byte)i;
            }

            string nombreCert = "CN=zzCertUsuMayo23.as";

            // Extraer el certificado
            X509Certificate2 cert = extraeCertificado(nombreCert);
            Console.WriteLine("Nombre del sujeto: {0}", cert.Subject);
            Console.WriteLine("Tiene clave privada: {0}", cert.HasPrivateKey);


            // Crear el objeto proveedor asignandole la clave privada
            RSACryptoServiceProvider proveedor2 = (RSACryptoServiceProvider)cert.PrivateKey;

            // Calcular el hash del mensaje
            SHA1CryptoServiceProvider hash = new SHA1CryptoServiceProvider();
            byte[] msgHash = hash.ComputeHash(mensaje);

            // Obtener la firma del mensaje
            byte[] firma = proveedor2.SignData(msgHash, "SHA1");
            Console.WriteLine("Firma:");
            Console.WriteLine(BitConverter.ToString(firma));


            // Crear proveedor asignandole la clave publica
            RSACryptoServiceProvider proveedor1 = (RSACryptoServiceProvider)cert.PublicKey.Key;
            bool verificado = proveedor1.VerifyData(mensaje, hash, firma);

            // Mostramos el resultado de la verificacion
            Console.WriteLine("Verificado? {0}", verificado);

        }

        public static X509Certificate2 extraeCertificado(string sujeto)
        {   
            // Variables
            X509Certificate2 certificado = null;
            X509Certificate2Collection certsEncontrados = new X509Certificate2Collection();
            X509Store almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            // Buscar el certificado en el almacen
            X509Certificate2Collection colacert = almacen.Certificates;
            certsEncontrados = colacert.Find(X509FindType.FindBySubjectDistinguishedName, sujeto, true);

            if (certsEncontrados.Count == 1)
            {
                certificado = certsEncontrados[0];
            }
            else if(certsEncontrados.Count > 1)
            {
                // Se han encontrado varios certificados
                Console.WriteLine("Se han encontrado varios certificados con el nombre '{0}'", sujeto);
                Environment.Exit(1);
            }
            else
            {
                // No se ha encontrado ningún certificado
                Console.WriteLine("No se ha encontrado ningún certificado con el nombre '{0}'", sujeto);
                Environment.Exit(1);
            }

            almacen.Close();

            // Devolver el certificado
            return certificado;
        }
    }
}
