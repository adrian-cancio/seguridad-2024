using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Apoyo;


namespace practica4_FirmaDigital_RSA_SHA
{
    class Program
    {
        static void Main(string[] args)
        {
            Ayuda help = new Ayuda();
            byte[] Mensaje = new byte[64];
            for(int i = 0; i < Mensaje.Length; i++)
            {
                Mensaje[i] = (byte) (i % 256);
            }

            //FASE 1
            //Creamos un objeto proveedor de servicios criptográficos para SHA256
            SHA256CryptoServiceProvider sha_provider = new SHA256CryptoServiceProvider();
            Console.WriteLine("Propiedad - Tamaño del Hash generado: " + sha_provider.HashSize);
            //Declaramos el array resumen y lo cargamos con el valor del hash obtenido del objeto
            //proveedor
            byte[] Resumen = sha_provider.ComputeHash(Mensaje);
            Console.WriteLine("Hash calculado: ");
            help.WriteHex(Resumen, Resumen.Length);
            //Liberar los recursos
            sha_provider.Dispose();


            //FASE 2
            //Creamos un objeto preoveedor de servicios criptográficos RSA  partir de la clase
            RSACryptoServiceProvider rsa_provider = new RSACryptoServiceProvider();
            //Elegimos una longitud para la clave que coincida con la long de las claves almacenadas
            //en zz_...
            rsa_provider.KeySize = 512;
            Console.WriteLine("Longitud de clave proveedor RSA: " + rsa_provider.KeySize);
            //Cargar el Blob contenido en el fichero array de bytes
            byte[] BlobRSA_Priva = new byte[help.BytesFichero("zz_BlobRSA_Priva.bin")];
            help.CargaBufer("zz_BlobRSA_Priva.bin", BlobRSA_Priva);
            rsa_provider.ImportCspBlob(BlobRSA_Priva);


            //FASE 3
            //Declaramos un array de bytes que contendrá la firma devuelta del metodo SignHash
            byte[] Firma = rsa_provider.SignHash(Resumen, "SHA256");
            Console.WriteLine("Firma: "); help.WriteHex(Firma, Firma.Length);
            //Firma devuelva por SignHash del objeto proveedor RSA a partir del resumen obtenido
            //anteriormente
            byte[] Firma2 = rsa_provider.SignHash(Resumen, "SHA256");
            Console.WriteLine("Firma2: "); help.WriteHex(Firma2, Firma2.Length);
            //Comprobar que se puede obtener la firma de un solo paso con SignData del objeto
            //proveedor RSA
            byte[] Firma3 = rsa_provider.SignData(Mensaje, "SHA256");
            Console.WriteLine("Firma3: "); help.WriteHex(Firma3, Firma3.Length);


            Console.WriteLine();
            
            
            //FASE 5
            //Modificaciones, tres instrucciones para ver si modificamos algun dato, como afecta
            //a las comprobaciones
            //Mensaje[0] = (byte)(128);
            //Resumen[0] = (byte) 00;
            //Firma[0] = (byte) 00;
            //Verificamos la firma recibida contra el resumen de datos obtenido anteriormente
            Boolean VFR = rsa_provider.VerifyHash(Resumen, "SHA256", Firma);
            Console.WriteLine("Firma valida VerifyHash?: " + VFR);
            //verificar la firma contra el mensaje a partir del cual se ha obtenido el resumen
            Boolean VFR2 = rsa_provider.VerifyData(Mensaje, "SHA256", Firma);
            Console.WriteLine("Firma valida VerifyData?: " + VFR2);
            //Liberamos recursos
            rsa_provider.Dispose();
            sha_provider.Dispose();
        }
    }
}
