using Microsoft.SqlServer.Server;
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



            // ----------------------------- 5. COMPARACION DE FIRMAS SHA-RSA DETERMINISTAS Y PROBABILISTAS -----------------------------
            Console.WriteLine("----- 5. COMPARACION DE FIRMAS SHA-RSA DETERMINISTAS Y PROBABILISTAS -----\n");



            // Crear un objeto ayuda
            Ayuda ayuda = new Ayuda();

            // Declarar un array de texto plano
            byte[] textoPlano = new byte[64];

            for (int i = 0; i < textoPlano.Length; i++)
            {
                textoPlano[i] = Convert.ToByte(i);
            }

            // Mostrar el texto plano
            Console.WriteLine("\nTexto Plano: ");
            ayuda.WriteHex(textoPlano, textoPlano.Length);

            // Proveedores SHA y RSA
            SHA256CryptoServiceProvider shaProvider = new SHA256CryptoServiceProvider();
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); // 1024 por defecto

            // Firmar
            byte[] resumen = new byte[shaProvider.HashSize];
            resumen = shaProvider.ComputeHash(textoPlano);

            // Algoritmo usado para calcular el hash (SHA1, SHA256, SHA384, SHA512)
            string AlgResumen = "SHA256";

            // Construir el algortimo hash con la cadena indicada
            HashAlgorithmName NomAlgRes = new HashAlgorithmName(AlgResumen);



            // --- PKCS1: firma determinista --- //
            Console.WriteLine("\n\n--- PKCS1: firma determinista ---\n");



            RSASignaturePadding RellenoFirma = RSASignaturePadding.Pkcs1;

            // Generar Firma
            byte[] firma = rsa.SignHash(resumen, AlgResumen);
            firma = rsaProvider.SignHash(resumen, NomAlgRes, RellenoFirma);

            Console.WriteLine("\nResultado Firma 1: ");
            ayuda.WriteHex(firma, firma.Length);

            // Generar Firma3
            byte[] firma3 = rsa.SignData(textoPlano, AlgResumen);
            firma3 = rsa.SignData(textoPlano, AlgResumen);
            firma3 = rsaProvider.SignData(resumen, NomAlgRes, RellenoFirma);

            Console.WriteLine("\nResultado Firma 3: ");
            ayuda.WriteHex(firma3, firma3.Length);

            // Verificar la validez de la firma contra el resumen
            bool VFR = rsa.VerifyHash(resumen, "SHA256", firma);

            // Verificar la validez de la firma contra el mensaje 
            bool VFM = rsa.VerifyData(textoPlano, "SHA256", firma3);

            // Verificar la validez de la firma contra el resumen
            VFR = rsaProvider.VerifyHash(resumen, firma, NomAlgRes, RellenoFirma);
            // Verificar la validez de la firma contra el mensaje 
            VFM = rsaProvider.VerifyData(textoPlano, firma, NomAlgRes, RellenoFirma);

            Console.WriteLine("\nResultado Verificacion Firma 1: " + VFR);
            Console.WriteLine("\nResultado Verificacion Firma 3: " + VFM + "\n");



            Console.WriteLine("\n----- Comparación de Firmas RSASSA-PKCS1 (Determinista) y RSASSA-PSS (Probabilista) -----\n");



            // Parámetro de configuración para el algoritmo de resumen
            AlgResumen = "SHA256";  // Cambiar aquí para usar SHA1, SHA256, SHA384, etc.
            NomAlgRes = new HashAlgorithmName(AlgResumen);

            // Generar un par de claves RSA (aleatorio en cada ejecución)
            using (rsaProvider = new RSACryptoServiceProvider(2048)) // Cambia la longitud de clave si es necesario
            {
                // Texto plano para firmar
                textoPlano = Encoding.UTF8.GetBytes("Mensaje de prueba para firma RSA");

                // Crear el resumen del texto plano
                using (HashAlgorithm hashAlg = HashAlgorithm.Create(AlgResumen))
                {
                    resumen = hashAlg.ComputeHash(textoPlano);

                    // Firma determinista (PKCS#1)
                    RellenoFirma = RSASignaturePadding.Pkcs1;
                    Console.WriteLine("\n--- Firma Determinista (RSASSA-PKCS1) ---\n");

                    // Generar la firma tres veces y comprobar que es la misma
                    byte[] firmaDeterminista1 = rsaProvider.SignHash(resumen, NomAlgRes, RellenoFirma);
                    byte[] firmaDeterminista2 = rsaProvider.SignHash(resumen, NomAlgRes, RellenoFirma);
                    byte[] firmaDeterminista3 = rsaProvider.SignHash(resumen, NomAlgRes, RellenoFirma);

                    Console.WriteLine("\n--- Firma Determinista 1 (RSASSA-PKCS1) ---");
                    ayuda.WriteHex(firmaDeterminista1, firmaDeterminista1.Length);
                    Console.WriteLine("\n--- Firma Determinista 2 (RSASSA-PKCS1) ---");
                    ayuda.WriteHex(firmaDeterminista2, firmaDeterminista2.Length);
                    Console.WriteLine("\n--- Firma Determinista 3 (RSASSA-PKCS1) ---");
                    ayuda.WriteHex(firmaDeterminista3, firmaDeterminista3.Length);
                   
                    // Verificar que las firmas deterministas son iguales
                    bool firmasIguales = (BitConverter.ToString(firmaDeterminista1) == BitConverter.ToString(firmaDeterminista2)) &&
                                         (BitConverter.ToString(firmaDeterminista2) == BitConverter.ToString(firmaDeterminista3));
                    
                    Console.WriteLine("\n¿Las firmas PKCS1 son iguales?: " + firmasIguales);

                    // Verificación de la firma determinista
                    bool verificacionFirmaDeterminista = rsaProvider.VerifyHash(resumen, firmaDeterminista1, NomAlgRes, RellenoFirma);
                    Console.WriteLine("Verificación Firma PKCS1: " + verificacionFirmaDeterminista);
                }
            }



            Console.WriteLine("\n\n--- Cambiando a Firma Probabilista (RSASSA-PSS) ---\n");



            // Cambiar a proveedor RSACng para soporte de PSS
            using (RSA rsaCngProvider = new RSACng())
            {
                rsaCngProvider.KeySize = 2048;  // Asegurarse de usar un tamaño de clave adecuado para PSS

                // Establecer el relleno a PSS para la firma probabilística
                RSASignaturePadding RellenoFirmaPss = RSASignaturePadding.Pss;

                textoPlano = Encoding.UTF8.GetBytes("Mensaje de prueba para firma RSA");
                using (HashAlgorithm hashAlg = HashAlgorithm.Create(AlgResumen))
                {
                    resumen = hashAlg.ComputeHash(textoPlano);

                    // Generar la firma tres veces y comprobar que son diferentes
                    byte[] firmaProbabilista1 = rsaCngProvider.SignHash(resumen, NomAlgRes, RellenoFirmaPss);
                    byte[] firmaProbabilista2 = rsaCngProvider.SignHash(resumen, NomAlgRes, RellenoFirmaPss);
                    byte[] firmaProbabilista3 = rsaCngProvider.SignHash(resumen, NomAlgRes, RellenoFirmaPss);

                    Console.WriteLine("\n--- Firma Probabilista 1 (RSASSA-PSS) ---");
                    ayuda.WriteHex(firmaProbabilista1, firmaProbabilista1.Length);
                    Console.WriteLine("\n--- Firma Probabilista 2 (RSASSA-PSS) ---");
                    ayuda.WriteHex(firmaProbabilista2, firmaProbabilista2.Length);
                    Console.WriteLine("\n--- Firma Probabilista 3 (RSASSA-PSS) ---");
                    ayuda.WriteHex(firmaProbabilista3, firmaProbabilista3.Length);

                    // Verificar que las firmas probabilísticas son diferentes
                    bool firmasDiferentes = !(BitConverter.ToString(firmaProbabilista1) == BitConverter.ToString(firmaProbabilista2)) ||
                                            !(BitConverter.ToString(firmaProbabilista2) == BitConverter.ToString(firmaProbabilista3));
                    Console.WriteLine("\n¿Las firmas PSS son diferentes?: " + firmasDiferentes);

                    // Verificación de cada firma probabilística
                    bool verificacionFirmaProbabilista1 = rsaCngProvider.VerifyHash(resumen, firmaProbabilista1, NomAlgRes, RellenoFirmaPss);
                    bool verificacionFirmaProbabilista2 = rsaCngProvider.VerifyHash(resumen, firmaProbabilista2, NomAlgRes, RellenoFirmaPss);
                    bool verificacionFirmaProbabilista3 = rsaCngProvider.VerifyHash(resumen, firmaProbabilista3, NomAlgRes, RellenoFirmaPss);

                    Console.WriteLine("Verificación Firma PSS (1): " + verificacionFirmaProbabilista1);
                    Console.WriteLine("Verificación Firma PSS (2): " + verificacionFirmaProbabilista2);
                    Console.WriteLine("Verificación Firma PSS (3): " + verificacionFirmaProbabilista3);
                }
            }

        Console.WriteLine("\nSolución: Usar una clave de al menos 2048 bits o cambiar a un algoritmo de resumen más corto, como SHA256.\n");

        // Liberar recursos
        rsaProvider.Dispose();
        shaProvider.Dispose();
        }
    }
}
