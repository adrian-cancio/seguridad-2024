using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using Apoyo;

/// <summary>
/// Programa que demuestra la generación de números aleatorios criptográficamente seguros
/// utilizando RNGCryptoServiceProvider de .NET Framework.
/// Este programa:
/// 1. Genera 64 bytes aleatorios criptográficamente seguros
/// 2. Los muestra en formato hexadecimal
/// 3. Los guarda en un archivo binario
/// 4. Los carga nuevamente del archivo
/// 5. Los muestra para verificar que se guardaron correctamente
/// </summary>
namespace Aleatorio
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear una instancia de la clase de ayuda que contiene métodos auxiliares
            Ayuda ayuda = new Ayuda();
            
            // Definir el tamaño del buffer en bytes (64 bytes = 512 bits)
            const int BYTES = 64;
            
            // Crear un array de bytes para almacenar los números aleatorios
            byte[] bufer = new byte[BYTES];
            
            // Crear un generador de números aleatorios criptográficamente seguro
            // RNGCryptoServiceProvider es la clase que genera números aleatorios seguros
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            
            // Generar números aleatorios y llenar el buffer
            provider.GetBytes(bufer);
            
            // Liberar los recursos del proveedor de números aleatorios
            provider.Dispose();
            
            // Mostrar los bytes aleatorios generados en formato hexadecimal
            ayuda.WriteHex(bufer, BYTES);

            // Nombre del archivo donde se guardarán los bytes aleatorios
            const String NOMBRE_FICHERO = "NumeroAleatorio.bin";

            // Guardar el buffer de bytes en un archivo binario
            ayuda.GuardaBufer(NOMBRE_FICHERO, bufer);
            
            // Mostrar confirmación del tamaño del archivo guardado
            Console.WriteLine("Fichero de "+ ayuda.BytesFichero(NOMBRE_FICHERO) + " bytes guardado");

            // Crear un nuevo buffer para cargar los bytes desde el archivo
            byte[] newBufer = new byte[BYTES];
            
            // Cargar los bytes del archivo al nuevo buffer
            ayuda.CargaBufer(NOMBRE_FICHERO, newBufer);

            // Mostrar los bytes cargados en formato hexadecimal para verificar la operación
            ayuda.WriteHex(newBufer, BYTES);
        }
    }
}
