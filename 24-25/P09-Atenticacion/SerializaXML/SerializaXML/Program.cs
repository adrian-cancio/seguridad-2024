using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Credenciales;

/// <summary>
/// Programa que demuestra la SERIALIZACIÓN XML de objetos en C#.
/// 
/// SERIALIZACIÓN XML:
/// Es el proceso de convertir un objeto (o árbol de objetos) a formato XML.
/// XML (eXtensible Markup Language) es un formato de texto estructurado que:
/// - Es legible por humanos
/// - Es portable entre diferentes sistemas
/// - Puede editarse con cualquier editor de texto
/// - Es ampliamente soportado
/// 
/// VENTAJAS DE LA SERIALIZACIÓN XML:
/// - Persistencia: guardar objetos en archivos
/// - Comunicación: enviar objetos entre aplicaciones
/// - Configuración: almacenar configuraciones complejas
/// - Debugging: inspeccionar el estado de objetos
/// 
/// PROCESO:
/// 1. Marcar las clases con [Serializable]
/// 2. Crear un XmlSerializer con el tipo de la clase
/// 3. Llamar a Serialize() con un stream y el objeto
/// 
/// DESERIALIZACIÓN (proceso inverso):
/// 1. Crear un XmlSerializer con el tipo de la clase
/// 2. Llamar a Deserialize() con un stream
/// 3. Hacer cast al tipo apropiado
/// 
/// Este programa:
/// 1. Crea varios usuarios con credenciales simuladas
/// 2. Los añade a un almacén
/// 3. Muestra el almacén por consola
/// 4. Serializa el almacén completo a un archivo XML
/// </summary>
namespace SerializaXML
{
    internal class Program
    {
        /// <summary>
        /// Serializa un almacén de usuarios a un archivo XML
        /// </summary>
        /// <param name="Alma">Almacén de usuarios a serializar</param>
        /// <param name="fichero">Ruta del archivo XML destino</param>
        static void SerializaAlmacenXML(Almacen Alma, string fichero)
        {
            // Usar 'using' para garantizar que el FileStream se cierre automáticamente
            using (System.IO.FileStream fs = new System.IO.FileStream(fichero, System.IO.FileMode.Create))
            {
                // Crear un serializador XML para la clase Almacen
                // typeof(Almacen) obtiene el tipo en tiempo de ejecución
                System.Xml.Serialization.XmlSerializer SerXML = new System.Xml.Serialization.XmlSerializer(typeof(Almacen));
                
                // Serializar el objeto Almacen al archivo XML
                // Esto convierte el objeto y todos sus objetos hijos a XML
                SerXML.Serialize(fs, Alma);
            }
            // El archivo se cierra automáticamente al salir del bloque using
        }
        
        static void Main(string[] args)
        {
            // ==================== CREAR USUARIOS ====================
            
            // Crear usuarios con datos simulados
            // En un sistema real, estos datos vendrían de un proceso de registro
            // donde la contraseña se hashea con PBKDF2 o similar
            Usuario NuevoUsu1 = new Usuario("Pepe", new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 });
            Usuario NuevoUsu2 = new Usuario("Juan", new byte[] { 7, 8, 9 }, new byte[] { 10, 11, 12 });
            Usuario NuevoUsu3 = new Usuario("Luis", new byte[] { 13, 14, 15 }, new byte[] { 16, 17, 18 });
            Usuario NuevoUsu4 = new Usuario("Ana", new byte[] { 19, 20, 21 }, new byte[] { 22, 23, 24 });

            // ==================== CREAR Y LLENAR EL ALMACÉN ====================
            
            // Crear un almacén vacío
            Almacen Almacen = new Almacen();
            
            // Añadir todos los usuarios al almacén
            Almacen.Add(NuevoUsu1);
            Almacen.Add(NuevoUsu2);
            Almacen.Add(NuevoUsu3);
            Almacen.Add(NuevoUsu4);

            // Mostrar el contenido del almacén por consola
            Console.WriteLine(Almacen);

            // ==================== SERIALIZAR A XML ====================
            
            // Serializar el almacén completo a un archivo XML
            // El archivo resultante contendrá todos los usuarios en formato XML estructurado
            SerializaAlmacenXML(Almacen, "AlmacenUsuarios.xml");
            
            // NOTA: El archivo XML generado tendrá una estructura similar a:
            // <?xml version="1.0"?>
            // <Almacen>
            //   <Lista>
            //     <Usuario>
            //       <Nombre>Pepe</Nombre>
            //       <Salt>AQID</Salt>
            //       <ResuContra>BAUG</ResuContra>
            //     </Usuario>
            //     ...
            //   </Lista>
            // </Almacen>

        }
    }
}
