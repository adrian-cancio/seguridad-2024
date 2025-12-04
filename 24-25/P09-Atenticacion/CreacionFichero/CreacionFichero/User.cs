using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Clase que representa un usuario con autenticación basada en contraseña hash.
/// Utiliza PBKDF2 (Rfc2898DeriveBytes) para derivar un hash seguro de la contraseña.
/// 
/// CONCEPTOS CLAVE DE SEGURIDAD:
/// - No almacena la contraseña en texto plano
/// - Usa una "sal" (salt) única por usuario para evitar rainbow tables
/// - Aplica múltiples iteraciones (1000) para hacer más costoso el ataque por fuerza bruta
/// - El hash resultante se almacena en lugar de la contraseña
/// 
/// Para verificar una contraseña:
/// - Se toma la contraseña ingresada
/// - Se combina con la misma sal almacenada
/// - Se aplica el mismo algoritmo
/// - Se compara el hash resultante con el hash almacenado
/// </summary>
namespace CreacionFichero
{
    internal class User
    {
        // ==================== CONSTANTES ====================
        
        /// <summary>Longitud máxima del nombre de usuario en caracteres</summary>
        private const int NAME_MAX_LENGTH = 16;
        
        /// <summary>Tamaño de la sal en bytes (128 bits)</summary>
        private const int SALT_BYTES = 16;
        
        /// <summary>Tamaño del hash de la contraseña en bytes (256 bits)</summary>
        private const int HASH_BYTES = 32;

        // ==================== ATRIBUTOS ====================
        
        /// <summary>Nombre del usuario (array de caracteres de longitud fija)</summary>
        public char[] name { get; }
        
        /// <summary>
        /// Sal criptográfica única para este usuario.
        /// Se genera aleatoriamente al establecer la contraseña.
        /// </summary>
        public byte[] salt { get; private set; }
        
        /// <summary>
        /// Hash de la contraseña derivado usando PBKDF2.
        /// Resultado de aplicar el algoritmo a (contraseña + sal + iteraciones).
        /// </summary>
        public byte[] hash { get; private set; }

        // ==================== CONSTRUCTORES ====================
        
        /// <summary>
        /// Constructor por defecto que inicializa los arrays con sus tamaños correspondientes
        /// </summary>
        public User()
        {
            name = new char[NAME_MAX_LENGTH];
            salt = new byte[SALT_BYTES];
            hash = new byte[HASH_BYTES];
        }

        /// <summary>
        /// Constructor que crea un usuario con nombre y contraseña
        /// </summary>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="password">Contraseña del usuario (se derivará a hash)</param>
        public User(String name, String password) : this()
        {
            this.SetName(name);
            this.SetPassword(password);
        }

        // ==================== MÉTODOS ====================
        
        /// <summary>
        /// Establece el nombre del usuario
        /// </summary>
        /// <param name="name">Nombre del usuario</param>
        /// <exception cref="ArgumentException">Si el nombre es demasiado largo</exception>
        public void SetName(String name)
        {
            // Validar que el nombre no exceda la longitud máxima
            if (name.Length > NAME_MAX_LENGTH)
            {
                throw new ArgumentException("Name too long");
            }
            
            // Copiar cada carácter del nombre al array
            for (int i = 0; i < name.Length; i++)
            {
                this.name[i] = name[i];
            }
        }

        /// <summary>
        /// Establece la contraseña del usuario.
        /// Genera una sal aleatoria y deriva el hash usando PBKDF2.
        /// </summary>
        /// <param name="password">Contraseña en texto plano (no se almacena)</param>
        public void SetPassword(String password)
        {
            // Generar una sal aleatoria criptográficamente segura
            // Cada usuario tiene una sal única, incluso si tienen la misma contraseña
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(this.salt);
            
            // Derivar el hash de la contraseña usando PBKDF2
            // Parámetros:
            //   - password: la contraseña en texto plano
            //   - this.salt: la sal aleatoria única
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, this.salt);
            
            // Establecer el número de iteraciones
            // Más iteraciones = más seguro pero más lento
            // 1000 es un mínimo razonable (estándares actuales recomiendan 10,000+)
            rfc.IterationCount = 1000;
            
            // Generar el hash de HASH_BYTES (32 bytes = 256 bits)
            this.hash = rfc.GetBytes(HASH_BYTES);
        }

        /// <summary>
        /// Convierte el usuario a una representación en texto
        /// Muestra el nombre, sal y hash en formato hexadecimal
        /// </summary>
        /// <returns>String con la información del usuario</returns>
        override public String ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            // Añadir el nombre
            stringBuilder.Append("Name: ");
            stringBuilder.Append(this.name);
            
            // Añadir la sal en formato hexadecimal
            stringBuilder.Append("\nSalt: ");
            // BitConverter.ToString convierte bytes a hex con guiones (ej: "A1-B2-C3")
            // Replace("-", "") elimina los guiones
            stringBuilder.Append(BitConverter.ToString(this.salt).Replace("-", ""));
            
            // Añadir el hash en formato hexadecimal
            stringBuilder.Append("\nHash: ");
            stringBuilder.Append(BitConverter.ToString(this.hash).Replace("-", ""));
            
            return stringBuilder.ToString();
        }
    }
}
