using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Clases para representar credenciales de usuarios y almacenarlas.
/// Estas clases están diseñadas para ser serializables a XML.
/// 
/// SERIALIZACIÓN XML:
/// - [Serializable]: atributo que marca una clase como serializable
/// - Permite convertir objetos a XML y viceversa
/// - Útil para persistir datos en formato legible y portable
/// - XML es un formato de texto, fácil de leer y editar manualmente
/// 
/// ESTRUCTURA:
/// - Almacen: contiene una lista de usuarios
/// - Usuario: representa un usuario con nombre, sal y hash de contraseña
/// </summary>
namespace Credenciales
{
    /// <summary>
    /// Clase que representa un almacén (colección) de usuarios.
    /// Permite agregar usuarios y obtener una representación en texto.
    /// </summary>
    [Serializable] // Marca la clase como serializable para XML
    public class Almacen
    {
        /// <summary>Lista de usuarios almacenados</summary>
        public List<Usuario> Lista;
        
        /// <summary>
        /// Constructor que inicializa la lista de usuarios vacía
        /// </summary>
        public Almacen()
        {
            Lista = new List<Usuario>();
        }

        /// <summary>
        /// Añade un usuario al almacén
        /// </summary>
        /// <param name="usu">Usuario a añadir</param>
        public void Add(Usuario usu)
        {
            Lista.Add(usu);
        }

        /// <summary>
        /// Convierte el almacén a una representación en texto
        /// Muestra todos los usuarios del almacén
        /// </summary>
        /// <returns>String con la información de todos los usuarios</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            // Añadir la representación de cada usuario
            foreach (Usuario usu in Lista)
            {
                sb.AppendLine(usu.ToString());
            }
            
            return sb.ToString();
        }
    }

    /// <summary>
    /// Clase que representa un usuario con sus credenciales hasheadas.
    /// Contiene el nombre del usuario, la sal y el hash de su contraseña.
    /// </summary>
    [Serializable] // Marca la clase como serializable para XML
    public class Usuario
    {
        /// <summary>Nombre del usuario</summary>
        public string Nombre;
        
        /// <summary>Sal criptográfica única del usuario</summary>
        public byte[] Salt;
        
        /// <summary>Resumen (hash) de la contraseña del usuario</summary>
        public byte[] ResuContra;

        /// <summary>
        /// Constructor por defecto (necesario para la deserialización XML)
        /// </summary>
        public Usuario()
        {
        }

        /// <summary>
        /// Constructor que crea un usuario con todos sus datos
        /// </summary>
        /// <param name="nombre">Nombre del usuario</param>
        /// <param name="salt">Sal criptográfica</param>
        /// <param name="resuContra">Hash de la contraseña</param>
        public Usuario(string nombre, byte[] salt, byte[] resuContra)
        {
            Nombre = nombre;
            Salt = salt;
            ResuContra = resuContra;
        }

        /// <summary>
        /// Convierte el usuario a una representación en texto
        /// Muestra el nombre, sal y hash de forma legible
        /// </summary>
        /// <returns>String con la información del usuario</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            // Añadir el nombre
            sb.AppendLine(String.Format("Nombre: {0}",Nombre));
            
            // Añadir la sal (bytes separados por espacios)
            sb.AppendLine("Salt: ");
            for (int i = 0; i < Salt.Length; i++)
            {
                sb.Append(Salt[i]+" ");
            }
            sb.AppendLine();

            // Añadir el hash de la contraseña (bytes separados por espacios)
            sb.AppendLine("Resumen de la contraseña: ");
            for (int i = 0; i < ResuContra.Length; i++)
            {
                sb.Append(ResuContra[i]+" ");
            }
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
