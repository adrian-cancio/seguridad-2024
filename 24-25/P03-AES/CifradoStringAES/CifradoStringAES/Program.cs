using Apoyo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Programa que demuestra el cifrado y descifrado de cadenas de texto usando AES
/// con StreamWriter y StreamReader para trabajar con texto en lugar de bytes.
/// 
/// AES (Advanced Encryption Standard) es el algoritmo de cifrado simétrico estándar:
/// - Mismo clave para cifrar y descifrar
/// - Tamaños de clave: 128, 192 o 256 bits
/// - Tamaño de bloque: siempre 128 bits (16 bytes)
/// 
/// Este programa:
/// 1. Configura un proveedor AES con clave y configuración personalizadas
/// 2. Cifra una cadena de texto a un archivo
/// 3. Lee y descifra el archivo para recuperar el texto original
/// 4. Demuestra el uso de CryptoStream con StreamWriter/StreamReader
/// </summary>
namespace CifradoStringAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear una clave AES de 32 bytes (256 bits)
            const int TamClave = 32;
            byte[] Clave = new byte[TamClave];
            // Inicializar la clave con valores secuenciales (solo demostración, no usar en producción)
            for (int i = 0; i < Clave.Length; i++) Clave[i] = (byte)(i % 256);

            // Crear un vector de inicialización (IV) de 16 bytes
            // El IV hace que el mismo texto cifrado con la misma clave produzca resultados diferentes
            byte[] VI = new byte[16];
            for (int i = 0; i < VI.Length; i++) VI[i] = (byte)((i + 160) % 256);

            // Crear el proveedor AES (AesManaged es la implementación gestionada de AES)
            AesManaged Provider = new AesManaged();

            // Mostrar las propiedades por defecto del proveedor
            Console.WriteLine("Provider default properties:");
            Console.WriteLine("BlockSize: " + Provider.BlockSize);      // Tamaño de bloque (siempre 128 bits para AES)
            Console.WriteLine("KeySize: " + Provider.KeySize);          // Tamaño de clave por defecto
            Console.WriteLine("Padding: " + Provider.Padding);          // Modo de relleno
            Console.WriteLine("Mode: " + Provider.Mode);                // Modo de cifrado
            Console.WriteLine();

            // Configurar el proveedor con nuestros parámetros
            Provider.Key = Clave;
            Provider.Padding = PaddingMode.PKCS7;  // PKCS7 rellena el último bloque si no es múltiplo de 16
            Provider.Mode = CipherMode.CBC;         // CBC: más seguro, usa IV
            Provider.IV = VI;                       // Establecer el IV correctamente

            // Mostrar las propiedades modificadas
            Console.WriteLine("Provider modified properties:");
            Console.WriteLine("BlockSize: " + Provider.BlockSize);
            Console.WriteLine("KeySize: " + Provider.KeySize);
            Console.WriteLine("Padding: " + Provider.Padding);
            Console.WriteLine("Mode: " + Provider.Mode);
            Console.WriteLine();

            // Generar una nueva clave aleatoria (sobrescribe la que habíamos establecido)
            // Provider.GenerateKey(); // Eliminado para usar la clave proporcionada
            
            Ayuda ayuda = new Ayuda();
            Console.WriteLine("Clave de cifrado:");
            ayuda.WriteHex(Provider.Key, TamClave);

            // Generar un nuevo IV aleatorio
            Provider.GenerateIV();
            Console.WriteLine("Vector de inicialización:");
            ayuda.WriteHex(Provider.IV, 16);

            // ==================== CIFRADO ====================
            // Nombre del archivo donde se guardará el texto cifrado
            const String NombreFichero = "zz_TextoCifrado.bin";
            
            // Crear un FileStream para escribir en el archivo
            FileStream fs = new FileStream(NombreFichero, 
                FileMode.Create, FileAccess.Write, FileShare.None);

            // Crear un transformador de cifrado
            ICryptoTransform encryptor = Provider.CreateEncryptor();
            
            // Crear un CryptoStream que cifra automáticamente lo que se escribe en él
            // El CryptoStream envuelve el FileStream y aplica el cifrado
            CryptoStream cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write);

            // Crear un StreamWriter para escribir texto (en lugar de bytes)
            StreamWriter sw = new StreamWriter(cs);

            // Escribir el texto plano (se cifrará automáticamente)
            sw.WriteLine("Texto plano.");

            // Asegurar que todo se escribe y cerrar los streams
            sw.Flush(); 
            sw.Close();
            sw.Dispose();
            cs.Flush();
            cs.Close();
            fs.Close();
            cs.Dispose();

            // ==================== MOSTRAR TEXTO CIFRADO ====================
            // Cargar el archivo cifrado para mostrarlo en hexadecimal
            long TamFich = ayuda.BytesFichero(NombreFichero);
            byte[] TextoCifrado = new byte[TamFich];
            
            // Inicializar con 0xFF para verificar que se sobrescribe
            for (int i = 0; i < TamFich; i++)
            {
                TextoCifrado[i] = 0xFF;
            }
            ayuda.CargaBufer(NombreFichero, TextoCifrado);

            Console.WriteLine("\nTexto cifrado:");
            ayuda.WriteHex(TextoCifrado, TextoCifrado.Length);

            // ==================== DESCIFRADO ====================
            // Abrir el archivo cifrado para lectura
            fs = new FileStream(NombreFichero,
                FileMode.Open, FileAccess.Read, FileShare.None);

            // Crear un transformador de descifrado
            ICryptoTransform decryptor = Provider.CreateDecryptor();

            // Crear un CryptoStream que descifra automáticamente lo que se lee de él
            cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read);

            // Crear un StreamReader para leer texto (en lugar de bytes)
            StreamReader sr = new StreamReader(cs);

            // Leer todo el texto descifrado
            String TextoDescifradoPlano = sr.ReadToEnd();

            // Cerrar todos los streams
            cs.Flush();
            cs.Close();
            fs.Close();
            sr.Close();
            sr.Dispose();

            // Mostrar el texto descifrado (debería ser igual al original)
            Console.WriteLine("\nTexto descifrado: ");
            Console.WriteLine(TextoDescifradoPlano);
        }
    }
}
