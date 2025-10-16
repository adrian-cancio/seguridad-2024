using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

/// <summary>
/// Programa que demuestra el uso de PBKDF2 (Password-Based Key Derivation Function 2)
/// para derivar claves criptográficas seguras a partir de una contraseña.
/// 
/// PBKDF2 (implementado en .NET como Rfc2898DeriveBytes) es un estándar para:
/// - Convertir contraseñas en claves criptográficas
/// - Añadir "sal" (salt) para evitar rainbow tables
/// - Aplicar múltiples iteraciones para dificultar ataques de fuerza bruta
/// 
/// Este programa:
/// 1. Solicita una contraseña al usuario
/// 2. Deriva una clave AES y un vector de inicialización (IV)
/// 3. Demuestra el efecto del método Reset()
/// 4. Muestra la diferencia entre resetear y continuar derivando bytes
/// </summary>
namespace GeneraClavesAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Solicitar contraseña al usuario
            Console.Write("Introduce la clave: ");
            String Contra = Console.ReadLine();

            // Definir una "sal" (salt) para añadir al proceso de derivación
            // La sal debe ser única por contraseña en aplicaciones reales
            // Aquí se usa una sal fija solo con fines demostrativos
            byte[] Sal = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            // Crear el objeto de derivación de claves PBKDF2
            // Parámetros:
            //   - Contra: la contraseña
            //   - Sal: bytes aleatorios para hacer única la derivación
            //   - 1000: número de iteraciones (más iteraciones = más seguro pero más lento)
            Rfc2898DeriveBytes DeriveBytes = new Rfc2898DeriveBytes(Contra, Sal, 1000);

            // Tamaño de la clave AES en bytes
            // 16 bytes = 128 bits, 24 bytes = 192 bits, 32 bytes = 256 bits
            byte AESByteNum = 32; // Usamos 256 bits para máxima seguridad
            
            // Derivar los bytes necesarios para la clave AES
            byte[] ClaveAES = DeriveBytes.GetBytes(AESByteNum);
            
            // Derivar los bytes para el vector de inicialización (siempre 16 bytes para AES)
            byte[] VIAES = DeriveBytes.GetBytes(16);
            
            // Crear instancia de la clase de ayuda
            Ayuda ayuda = new Ayuda();

            // Mostrar la clave AES generada
            Console.WriteLine("Clave AES: ");
            ayuda.WriteHex(ClaveAES, AESByteNum);
            
            // Mostrar el vector de inicialización generado
            Console.WriteLine("Vector de Inicialización AES: ");
            ayuda.WriteHex(VIAES, 16);
            Console.WriteLine();

            // DEMOSTRACIÓN: Efecto del método Reset()
            Console.WriteLine("Reseteamos el objeto DeriveBytes");
            DeriveBytes.Reset();

            // Después de Reset(), vuelve a generar las mismas claves
            byte[] NuevaClaveAES = DeriveBytes.GetBytes(AESByteNum);
            byte[] NuevaVIAES = DeriveBytes.GetBytes(16);

            Console.WriteLine("Nueva Clave AES: ");
            ayuda.WriteHex(NuevaClaveAES, AESByteNum);
            Console.WriteLine("Nuevo Vector de Inicialización AES: ");
            ayuda.WriteHex(NuevaVIAES, 16);
            Console.WriteLine();

            // DEMOSTRACIÓN: Sin resetear, los bytes derivados son diferentes
            Console.WriteLine("Pero si lo hacemos si reseterar:");
            
            // Estos bytes serán diferentes porque continúan desde donde quedó
            byte[] ClaveAES2 = DeriveBytes.GetBytes(AESByteNum);
            byte[] VIAES2 = DeriveBytes.GetBytes(16);
            
            Console.WriteLine("Clave AES: ");
            ayuda.WriteHex(ClaveAES2, AESByteNum);
            Console.WriteLine("Vector de Inicialización AES: ");
            ayuda.WriteHex(VIAES2, 16);
        }
    }
}
