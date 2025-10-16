using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

/// <summary>
/// Programa que demuestra la generación de pares de claves RSA (pública y privada)
/// y su exportación en diferentes formatos.
/// 
/// RSA es un algoritmo de cifrado asimétrico (clave pública):
/// - Usa dos claves relacionadas matemáticamente: pública y privada
/// - Clave pública: para cifrar (puede compartirse)
/// - Clave privada: para descifrar (debe mantenerse secreta)
/// - Basado en la factorización de números primos grandes
/// 
/// COMPONENTES MATEMÁTICOS DE RSA:
/// - n (Módulo): producto de dos primos grandes (p * q)
/// - e (Exponente público): típicamente 65537
/// - d (Exponente privado): calculado matemáticamente
/// - p, q: números primos secretos
/// 
/// Este programa:
/// 1. Genera un par de claves RSA de 1024 bits
/// 2. Muestra los componentes de las claves en hexadecimal
/// 3. Exporta los componentes a archivos de texto individuales
/// 4. Exporta las claves en formato blob (binario) a archivos
/// 5. Demuestra la importación de claves desde blob
/// </summary>
namespace GeneraClavesRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda ayuda = new Ayuda();
            
            // Crear un proveedor RSA con claves de 1024 bits
            // Tamaños típicos: 1024, 2048, 4096 bits
            // Mayor tamaño = más seguro pero más lento
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            // ==================== MOSTRAR PROPIEDADES DEL PROVEEDOR ====================
            
            // Obtener los tamaños legales de clave soportados
            KeySizes legalKeySizes = rsa.LegalKeySizes[0];
            Console.WriteLine("Tamaños legales de las claves: [{0}, {1}]", legalKeySizes.MinSize, legalKeySizes.MaxSize);
            Console.WriteLine("Tamaño de la clave actual: {0}", rsa.KeySize);
            Console.WriteLine("¿La clave deve conservarse en el proveedor actual? {0}", rsa.PersistKeyInCsp);
            Console.WriteLine("Algoritmo de intercambio de claves: {0}", rsa.KeyExchangeAlgorithm);
            Console.WriteLine("¿El objeto contiene solo una clave publica? {0}", rsa.PublicOnly);
            Console.WriteLine();

            // Exportar las claves a formato XML (incluye todos los parámetros)
            String rsaXml = rsa.ToXmlString(true);  // true = incluir clave privada
            String rsaString = rsa.ToString();

            // ==================== EXPORTAR PARÁMETROS RSA ====================
            
            // Exportar los parámetros de la clave RSA
            // true = incluir parámetros privados (d, p, q)
            RSAParameters rsaParameters = rsa.ExportParameters(true);

            // Extraer cada componente de la clave
            byte[] PrivateExponent = rsaParameters.D;        // d: exponente privado
            byte[] PublicExponent = rsaParameters.Exponent;  // e: exponente público
            byte[] Module = rsaParameters.Modulus;           // n: módulo (p * q)
            byte[] PrimeP = rsaParameters.P;                 // p: primer primo
            byte[] PrimeQ = rsaParameters.Q;                 // q: segundo primo

            // Mostrar cada componente en formato hexadecimal
            Console.WriteLine("Exponente Privado:");
            ayuda.WriteHex(PrivateExponent, PrivateExponent.Length);
            Console.WriteLine("Exponente Público:");
            ayuda.WriteHex(PublicExponent, PublicExponent.Length);
            Console.WriteLine("Modulo:");
            ayuda.WriteHex(Module, Module.Length);
            Console.WriteLine("Número Primo P:");
            ayuda.WriteHex(PrimeP, PrimeP.Length);
            Console.WriteLine("Número Primo Q:");
            ayuda.WriteHex(PrimeQ, PrimeQ.Length);
            Console.WriteLine();

            // ==================== GUARDAR COMPONENTES EN ARCHIVOS DE TEXTO ====================
            
            // Nombres de archivos para cada componente
            String[] nombresFich = new string[] { "d.txt", "e.txt", "n.txt", "p.txt", "q.txt" };
            byte[][] buffers = new byte[][] { PrivateExponent, PublicExponent, Module, PrimeP, PrimeQ };
            
            // Guardar cada componente en un archivo de texto (formato hexadecimal)
            for (int i = 0; i < nombresFich.Length; i++)
            {
                FileStream Fs = new FileStream(nombresFich[i], FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter Sw = new StreamWriter(Fs);
                
                // Convertir los bytes a string hexadecimal sin guiones
                string resultado = BitConverter.ToString(buffers[i]).Replace("-", "");
                Sw.Write(resultado);
                Sw.Close();
                Fs.Close();
            }

            // ==================== EXPORTAR CLAVES EN FORMATO BLOB ====================
            
            // Preparar nombres de archivo con timestamp
            DateTime fecha = DateTime.Now;
            String rutaBase = "..\\..\\..\\..\\Compartido\\";
            String baseFich = "zz_BlobRSA_" + fecha.ToString("yyyyMMdd_HHmmss");

            // Crear el directorio si no existe
            if (!Directory.Exists(rutaBase))
            {
                Directory.CreateDirectory(rutaBase);
            }

            // BLOB CON CLAVE PRIVADA (incluye toda la información)
            // Exportar como blob binario (formato propietario de Microsoft)
            byte[] cspBlobPriv = rsa.ExportCspBlob(true);  // true = incluir clave privada
            String nombreFichBlobPriv = rutaBase + baseFich + "_Priv.bin";
            String rutaAbsolutaPriv = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + baseFich + "_Priv.bin";
            ayuda.GuardaBufer(nombreFichBlobPriv, cspBlobPriv);
            Console.WriteLine("Fichero con clave privada guardado en:\n{0}", rutaAbsolutaPriv);

            // BLOB CON CLAVE PÚBLICA (solo información pública: e y n)
            byte[] cspBlobPubli = rsa.ExportCspBlob(false);  // false = solo clave pública
            String nombreFichBlobPubli = rutaBase + baseFich + "_Publi.bin";
            String rutaAbsolutaPubli = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + baseFich + "_Publi.bin";
            ayuda.GuardaBufer(nombreFichBlobPubli, cspBlobPubli);
            Console.WriteLine("Fichero con clave pública guardado en:\n{0}", rutaAbsolutaPubli);

            // ==================== DEMOSTRACIÓN: IMPORTAR CLAVE DESDE BLOB ====================
            
            // Crear un nuevo proveedor RSA e importar la clave privada desde el blob
            RSACryptoServiceProvider rsaImport = new RSACryptoServiceProvider();
            rsaImport.ImportCspBlob(cspBlobPriv);
            // Ahora rsaImport contiene la misma clave que rsa
        }
    }
}
