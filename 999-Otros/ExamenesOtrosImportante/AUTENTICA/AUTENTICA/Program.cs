using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Apoyo;

/// <summary>
/// PROGRAMA DE EXAMEN: AUTENTICA
/// Este programa cayó en el primer parcial y valía 3-4 puntos.
/// 
/// FUNCIÓN:
/// Valida las credenciales de un usuario comparando la contraseña ingresada
/// con un hash almacenado previamente en un archivo de texto.
/// 
/// PROCESO DE AUTENTICACIÓN:
/// 1. Solicita nombre de usuario y contraseña
/// 2. Lee el archivo usuarios.txt que contiene:
///    - Línea N: nombre de usuario
///    - Línea N+1: sal (salt) en Base64
///    - Línea N+2: hash SHA-512 en Base64
///    - Línea N+3: información adicional del usuario
/// 3. Busca el usuario en el archivo
/// 4. Combina la sal almacenada con la contraseña ingresada
/// 5. Calcula el hash SHA-512 de (sal + contraseña)
/// 6. Compara el hash calculado con el hash almacenado
/// 7. Devuelve un código según el resultado:
///    - 0: Usuario y contraseña correctos
///    - 1: Usuario desconocido (no existe)
///    - 2: Contraseña inválida (usuario existe pero contraseña incorrecta)
/// 
/// SEGURIDAD:
/// - La contraseña nunca se almacena en texto plano
/// - Se usa SHA-512 para el hash (512 bits de seguridad)
/// - La sal previene ataques de rainbow table
/// - Cada usuario debe tener una sal única
/// 
/// FORMATO DEL ARCHIVO usuarios.txt:
/// nombreUsuario1
/// salEnBase64
/// hashEnBase64
/// informacionAdicional
/// nombreUsuario2
/// salEnBase64
/// hashEnBase64
/// informacionAdicional
/// ...
/// </summary>
namespace AUTENTICA
{
    class Program
    {
        static void Main(string[] args)
        {
            // ==================== ENTRADA DE CREDENCIALES ====================
            
            // Solicitar nombre de usuario
            Console.WriteLine("Introduce el nombre de usuario: ");
            string nombre = Console.ReadLine();
            
            // Solicitar contraseña
            Console.WriteLine("Introduce la contraseña: ");
            string contra = Console.ReadLine();

            // ==================== LEER DATOS DEL ARCHIVO ====================
            
            // Leer todas las líneas del archivo de usuarios
            string[] lines = File.ReadAllLines("usuarios.txt");
            
            // Variables para almacenar los datos del usuario encontrado
            string nombreUsuario = null;
            byte[] salt = null;
            byte[] resumenAlmacenado = null;
            string infoAdicional = null;

            // Buscar el usuario en el archivo
            // El archivo tiene bloques de 4 líneas por usuario
            for (int i = 0; i < lines.Length; i += 4)
            {
                // Verificar si el nombre coincide
                if (lines[i] == nombre)
                {
                    // Extraer los datos del usuario
                    nombreUsuario = lines[i];                              // Línea 1: nombre
                    salt = Convert.FromBase64String(lines[i + 1]);        // Línea 2: sal (de Base64 a bytes)
                    resumenAlmacenado = Convert.FromBase64String(lines[i + 2]); // Línea 3: hash (de Base64 a bytes)
                    infoAdicional = lines[i + 3];                         // Línea 4: info adicional
                    break; // Usuario encontrado, salir del bucle
                }
            }

            // ==================== VERIFICAR SI EL USUARIO EXISTE ====================
            
            // Si no se encontró el usuario
            if (nombreUsuario == null)
            {
                Console.WriteLine(1); // Código 1: Usuario desconocido
                return;
            }

            // ==================== VERIFICAR LA CONTRASEÑA ====================
            
            // Convertir la contraseña ingresada a bytes usando Unicode
            // IMPORTANTE: Debe usarse la misma codificación que se usó al crear el hash
            byte[] contraBytes = Encoding.Unicode.GetBytes(contra);
            
            // Concatenar sal + contraseña (en ese orden)
            // La sal debe ir primero para que coincida con cómo se creó el hash original
            byte[] saltYContra = salt.Concat(contraBytes).ToArray();

            // Calcular el hash SHA-512 de (sal + contraseña)
            using (SHA512 sha512 = SHA512.Create())
            {
                // Calcular el hash
                byte[] resumenContra = sha512.ComputeHash(saltYContra);
                
                // Comparar el hash calculado con el hash almacenado
                // SequenceEqual compara byte por byte
                if (resumenContra.SequenceEqual(resumenAlmacenado))
                {
                    Console.WriteLine(0); // Código 0: Usuario y contraseña correctos
                }
                else
                {
                    Console.WriteLine(2); // Código 2: Contraseña inválida
                }
            }
        }
    }
}
