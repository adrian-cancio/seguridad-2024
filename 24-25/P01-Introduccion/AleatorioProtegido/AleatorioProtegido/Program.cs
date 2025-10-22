using System;
using System.Linq;
using System.Security.Cryptography;
using Apoyo;

/// <summary>
/// Programa que demuestra el uso de la API de protección de datos (DPAPI) de Windows
/// para cifrar datos utilizando las credenciales del usuario actual.
/// 
/// DPAPI (Data Protection API) permite cifrar datos de forma que solo el usuario
/// que los cifró pueda descifrarlos. Es útil para:
/// - Proteger datos sensibles en disco
/// - Almacenar credenciales de forma segura
/// - Proteger configuraciones sin necesidad de gestionar claves manualmente
/// 
/// Este programa:
/// 1. Genera 64 bytes aleatorios criptográficamente seguros
/// 2. Protege (cifra) esos bytes usando DPAPI
/// 3. Guarda los bytes protegidos en un archivo
/// 4. Carga los bytes protegidos desde el archivo
/// 5. Desprotege (descifra) los bytes
/// 6. Verifica que los datos originales y descifrados son idénticos
/// </summary>
namespace AleatorioProtegido
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Definir constantes para el tamaño del buffer y nombre del archivo
            const int BYTES = 64;
            const String NOMBRE_FICHERO = "NumeroAleatorio.bin";
            
            // Crear instancia de la clase de ayuda
            Ayuda ayuda = new Ayuda();
            
            // Crear buffer para almacenar los bytes aleatorios
            byte[] bufer = new byte[BYTES];

            // Generar números aleatorios criptográficamente seguros
            // Using garantiza que el objeto se dispose correctamente
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(bufer);
            }

            // PASO 1: Proteger (cifrar) el buffer usando DPAPI
            // ProtectedData.Protect cifra los datos usando las credenciales del usuario actual
            // Parámetros:
            //   - bufer: datos a proteger
            //   - null: no se usa entropía adicional (sal)
            //   - DataProtectionScope.CurrentUser: solo el usuario actual puede descifrar
            byte[] buferProtegido = ProtectedData.Protect(bufer, null, DataProtectionScope.CurrentUser);

            // Mostrar el buffer cifrado en formato hexadecimal
            Console.WriteLine("Búfer cifrado");
            ayuda.WriteHex(buferProtegido, buferProtegido.Length);

            // PASO 2: Guardar el buffer protegido en un archivo
            // Los datos están cifrados, así que el archivo no contiene los datos originales
            ayuda.GuardaBufer(NOMBRE_FICHERO, buferProtegido);
            Console.WriteLine("Fichero de " + ayuda.BytesFichero(NOMBRE_FICHERO) + " bytes guardado");

            // PASO 3: Cargar el buffer protegido desde el archivo
            // Simulamos el escenario de cargar datos previamente cifrados
            byte[] newBuferProtegido = new byte[buferProtegido.Length];
            ayuda.CargaBufer(NOMBRE_FICHERO, newBuferProtegido);

            // PASO 4: Desproteger (descifrar) el buffer cargado
            // ProtectedData.Unprotect descifra los datos usando las credenciales del usuario actual
            // Solo funcionará si el usuario que descifra es el mismo que cifró
            byte[] newBufer = ProtectedData.Unprotect(newBuferProtegido, null, DataProtectionScope.CurrentUser);

            // Mostrar el buffer descifrado en formato hexadecimal
            Console.WriteLine("Búfer descifrado");
            ayuda.WriteHex(newBufer, newBufer.Length);

            // PASO 5: Verificar que el buffer original y el descifrado son iguales
            // SequenceEqual compara elemento por elemento los dos arrays
            bool iguales = bufer.SequenceEqual(newBufer);
            Console.WriteLine("Los búferes {0}son iguales ", (iguales?"":"no "));
        }
    }
}
