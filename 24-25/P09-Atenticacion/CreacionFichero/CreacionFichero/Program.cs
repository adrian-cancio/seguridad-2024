using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Programa principal que demuestra la creación de una lista de usuarios
/// con autenticación basada en hash de contraseñas.
/// 
/// Este programa:
/// 1. Crea una lista de usuarios
/// 2. Inicializa 5 usuarios con nombres y contraseñas
/// 3. Muestra la lista por consola
/// 4. Guarda la lista en formato binario (ListaU.bin)
/// 5. Guarda la lista en formato texto (ListaU.txt)
/// 
/// CONCEPTO IMPORTANTE:
/// Las contraseñas nunca se almacenan en texto plano.
/// Solo se almacenan:
/// - La sal (salt): valor aleatorio único por usuario
/// - El hash: resultado de aplicar PBKDF2 a (contraseña + sal)
/// 
/// Esto es fundamental para la seguridad:
/// - Si alguien roba el archivo, no puede obtener las contraseñas
/// - Cada usuario tiene una sal diferente, incluso con contraseñas iguales
/// - El hash no es reversible (no se puede obtener la contraseña del hash)
/// </summary>
namespace CreacionFichero
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear una nueva lista de usuarios
            ListaU LU = new ListaU();
            
            // Inicializar usuarios con nombre y contraseña
            // Nota: Las contraseñas son solo para demostración
            // En producción, las contraseñas deberían solicitarse al usuario de forma segura
            LU.IniUsu(0, new User("Antonio", "conA"));
            LU.IniUsu(1, new User("Benito", "conB"));
            LU.IniUsu(2, new User("Carlos", "conC"));
            LU.IniUsu(3, new User("David", "conD"));
            LU.IniUsu(4, new User("Eduardo", "conE"));
            
            // Mostrar la lista de usuarios por consola
            // Esto mostrará: nombre, sal (en hex) y hash (en hex) de cada usuario
            LU.VerLista();
            
            // Guardar la lista en formato binario
            // Formato compacto, no legible por humanos
            LU.GuardaListaBin("ListaU.bin");
            
            // Guardar la lista en formato texto
            // Usa Base64 para codificar los bytes de sal y hash
            // Formato legible (con editor de texto) pero menos eficiente
            LU.GuardaListaTxt("ListaU.txt");
        }
    }
}
