using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Clase que gestiona una lista de usuarios.
/// Proporciona funcionalidad para:
/// - Almacenar múltiples usuarios
/// - Mostrar la lista de usuarios por consola
/// - Guardar la lista en formato binario
/// - Guardar la lista en formato texto (Base64)
/// 
/// Esta clase demuestra diferentes formas de persistencia de datos:
/// - Formato binario: más eficiente en espacio, no legible por humanos
/// - Formato texto: menos eficiente pero legible y portable
/// </summary>
namespace CreacionFichero
{
    internal class ListaU
    {
        // ==================== CONSTANTES ====================
        
        /// <summary>Número máximo de usuarios que puede contener la lista</summary>
        private const int MAX_USU = 5;

        // ==================== ATRIBUTOS ====================
        
        /// <summary>Array que almacena los usuarios</summary>
        public User[] lista { get; }

        // ==================== CONSTRUCTOR ====================
        
        /// <summary>
        /// Constructor que inicializa la lista de usuarios
        /// </summary>
        public ListaU()
        {
            lista = new User[MAX_USU];
        }

        // ==================== MÉTODOS ====================
        
        /// <summary>
        /// Inicializa un usuario en una posición específica de la lista
        /// </summary>
        /// <param name="indice">Índice donde colocar el usuario (0 a MAX_USU-1)</param>
        /// <param name="usuario">Objeto User a almacenar</param>
        public void IniUsu(int indice, User usuario)
        {
            lista[indice] = usuario;
        }

        /// <summary>
        /// Muestra por consola todos los usuarios de la lista
        /// Itera por todos los usuarios y muestra su información
        /// </summary>
        public void VerLista()
        {
            int i = 0;
            
            // Iterar por cada usuario en la lista
            foreach (User usuario in lista)
            {
                // Solo mostrar si el usuario no es null
                if (usuario != null)
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("Usuario " + i + ":");
                    // Usa el método ToString() del User para mostrar su información
                    Console.WriteLine(usuario);
                }
                i++;
            }
            Console.WriteLine("----------------------------------------------------------------------");
        }

        /// <summary>
        /// Guarda la lista de usuarios en un archivo binario.
        /// El formato binario es más compacto pero no es legible por humanos.
        /// 
        /// Estructura del archivo:
        /// - Para cada usuario: name (chars) + salt (bytes) + hash (bytes)
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo donde guardar</param>
        public void GuardaListaBin(string NombreFich)
        {
            // Usar 'using' garantiza que el writer se cierre automáticamente
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(
                System.IO.File.Open(NombreFich, System.IO.FileMode.Create)))
            {
                // Iterar por cada usuario
                foreach (User usuario in lista)
                {
                    // Solo guardar usuarios que no sean null
                    if (usuario != null)
                    {
                        // Escribir los datos del usuario en formato binario
                        writer.Write(usuario.name);  // Array de caracteres
                        writer.Write(usuario.salt);  // Array de bytes (sal)
                        writer.Write(usuario.hash);  // Array de bytes (hash)
                    }
                }
            }
        }

        /// <summary>
        /// Guarda la lista de usuarios en un archivo de texto.
        /// Los datos binarios (salt y hash) se codifican en Base64 para hacerlos legibles.
        /// 
        /// Estructura del archivo:
        /// - Para cada usuario:
        ///   Línea 1: nombre (caracteres)
        ///   Línea 2: salt codificado en Base64
        ///   Línea 3: hash codificado en Base64
        /// 
        /// Base64 convierte bytes a texto usando solo caracteres imprimibles (A-Z, a-z, 0-9, +, /)
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo donde guardar</param>
        public void GuardaListaTxt(string NombreFich)
        {
            // Crear un StreamWriter para escribir texto
            // Parámetros:
            //   - NombreFich: archivo destino
            //   - false: no añadir al final (sobrescribir)
            //   - Encoding.ASCII: usar codificación ASCII
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(
                NombreFich, false, Encoding.ASCII))
            {
                // Iterar por cada usuario
                foreach (User usuario in lista)
                {
                    // Solo guardar usuarios que no sean null
                    if (usuario != null)
                    {
                        // Escribir el nombre directamente (ya es texto)
                        writer.WriteLine(usuario.name);
                        
                        // Convertir la sal (bytes) a Base64 (texto)
                        writer.WriteLine(Convert.ToBase64String(usuario.salt));
                        
                        // Convertir el hash (bytes) a Base64 (texto)
                        writer.WriteLine(Convert.ToBase64String(usuario.hash));
                    }
                }
            }
        }
    }
}
