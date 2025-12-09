using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace RSAySHA
{
    internal class Program
    {
        static void Main(string[] args)
        {



            // ----------------------------- 4. CREAR UNA FIRMA DIGITAL CON SHA y RSA Y VERIFICARLA -----------------------------
            Console.WriteLine("----- 4. CREAR UNA FIRMA DIGITAL CON SHA y RSA Y VERIFICARLA -----\n");



            // Crear un objeto ayuda
            Ayuda ayuda = new Ayuda();

            // Declarar un array de texto plano
            byte[] textoPlano = new byte[64];

            textoPlano = new byte[64];
            for (int i = 0; i < textoPlano.Length; i++)
            {
                textoPlano[i] = Convert.ToByte(i);
            }

            // Mostrar el texto plano
            Console.WriteLine("\nTexto plano: ");
            ayuda.WriteHex(textoPlano, textoPlano.Length);



            //----------FASE 1. CÁLCULO DEL HASH ----------//
            Console.WriteLine("\n\nFASE 1. CÁLCULO DEL HASH\n");



            // 1) Crear un objeto Proveedor de servicios SHA256
            SHA256CryptoServiceProvider shaProvider = new SHA256CryptoServiceProvider();

            // 2) Mostrar por consola algunas propiedades
            Console.WriteLine("[Propiedades del proveedor de servicios criptograficos HASH]:");
            Console.WriteLine("Tamaño del hash: " + shaProvider.HashSize);

            // 3) Declarar un array de bytes denominado Resumen 
            byte[] resumen = new byte[shaProvider.HashSize];

            // Cargarlo con el valor del hash obtenido llamando al método ComputeHash() del objeto proveedor de SHA256
            resumen = shaProvider.ComputeHash(textoPlano);

            // Mostrar en la consola el hash calculado.
            Console.WriteLine("\nHash: ");
            ayuda.WriteHex(resumen, resumen.Length);

            // 4) Liberar recursos
            shaProvider.Dispose();
            


            //----------FASE 2: CREAR UN PROVEDOR RSA PARA CIFRAR EL VALOR DEL HASH----------//+
            Console.WriteLine("\n\nFASE 2: CREAR UN PROVEDOR RSA PARA CIFRAR EL VALOR DEL HASH\n");



            // 1) Crear un objeto Proveedor de servicios RSA
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); // 1024 por defecto

            // 2) Mostrar por consola algunas propiedades
            Console.WriteLine("[Propiedades del proveedor de servicios criptograficos RSA]:");
            Console.WriteLine("Tamaño actual de la clave: " + rsa.KeySize);

            // 3) Cargar el Blob en un array de bytes
            // Obtener tamaño en bytes del fichero publico y declarar un array de bytes con ese tamaño para almacenar el contenido
            long size = ayuda.BytesFichero("zz_BlobRSA_Priva.bin");
            byte[] buffer = new byte[size];

            // Cargar array de bytes con el contenido del fichero
            ayuda.CargaBufer("zz_BlobRSA_Priva.bin", buffer);

            // Importar el array de bytes
            rsa.ImportCspBlob(buffer);



            //----------FASE 3: GENERAR LA FIRMA ----------//
            Console.WriteLine("\n\nFASE 3: GENERAR LA FIRMA");



            // 1) Declarar un array de bytes denominado Firma e inicializarlo con la firma devuelta por el método SignHash() del objeto proveedor RSA
            byte[] firma = rsa.SignHash(resumen, "SHA256");

            // Mostrar la Firma en la consola.
            Console.WriteLine("\nFirma SignHash: ");
            ayuda.WriteHex(firma, firma.Length);

            // 2) Declarar un array de bytes denominado Firma2 e inicializarlo con la firma devuelta por el método SignHash() del objeto proveedor RSA
            Console.WriteLine("\nLa siguiente firma debe ser igual a la anterior, pues este esquema de firma es determinista:\r\npara el mismo resumen y la misma clave, la firma siempre es la misma. ");
            byte[] firma2 = rsa.SignHash(resumen, "SHA256");

            // Mostrar la Firma en la consola.
            Console.WriteLine("\nFirma2 SignHash: ");
            ayuda.WriteHex(firma2, firma2.Length);

            // 3) Declarar un array de bytes denominado Firma3 e inicializarlo con la firma devuelta por el método SignData() y recibiendo el textoplano
            Console.WriteLine("\nComprobar que se puede obtener la firma en un solo paso, llamando al método SignData() del objeto proveedor RSA");
            byte[] firma3 = rsa.SignData(textoPlano, "SHA256");

            // Mostrar la firma en la consola
            Console.WriteLine("\nFirma SignData: ");
            ayuda.WriteHex(firma3, firma3.Length);



            //----------FASE 4: COMBINAR EL MENSAJE CON LA FIRMA----------//
            Console.WriteLine("\n\nFASE 4: COMBINAR EL MENSAJE CON LA FIRMA\n");



            // Volcar el mensaje a un fichero
            ayuda.GuardaBufer("zz_Mensaje.bin", textoPlano);
            Console.WriteLine("\nMensaje guardado en zz_Mensaje.bin");
            // Volcar la firma a un fichero

            ayuda.GuardaBufer("zz_Firma.bin", firma);
            Console.WriteLine("\nFirma guardada en zz_Firma.bin");



            //----------FASE 5: VERIFICAR LA FIRMA ANTES DE ENVIARLA CON EL MENSAJE ---------//
            Console.WriteLine("\n\nFASE 5: VERIFICAR LA FIRMA ANTES DE ENVIARLA CON EL MENSAJE\n");



            // 1) Estas tres instrucciones haran que la firma no sea valida
            //textoPlano[15] = 00;
            //firma[7] = 00;
            //resumen[6] = 00;

            // 2) Verificar la validez de la firma contra el resumen
            bool VFR = rsa.VerifyHash(resumen, "SHA256", firma);

            // 3) Verificar la validez de la firma contra el mensaje 
            bool VFM = rsa.VerifyData(textoPlano, "SHA256", firma3);

            // 4) Mostrar el resultado de la verificación
            Console.WriteLine("\nVerificación de la firma contra el resumen: " + VFR);
            Console.WriteLine("\nVerificación de la firma contra el mensaje: " + VFM + "\n");

            // 4) Liberar recursos
            rsa.Dispose();

        }
    }
}
