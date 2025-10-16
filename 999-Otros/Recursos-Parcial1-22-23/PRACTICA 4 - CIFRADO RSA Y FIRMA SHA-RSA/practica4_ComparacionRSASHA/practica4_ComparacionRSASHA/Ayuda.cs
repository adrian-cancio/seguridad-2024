using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Para trabajar con ficheros
using System.IO;

/// <summary>
/// Clase de utilidades que proporciona métodos auxiliares para:
/// - Mostrar bytes en formato hexadecimal
/// - Trabajar con archivos (guardar, cargar, obtener tamaño)
/// Esta clase es utilizada en múltiples ejercicios para evitar duplicar código
/// </summary>
namespace Apoyo
{
    class Ayuda
    {
        /// <summary>
        /// Muestra un array de bytes en formato hexadecimal con formato de tabla
        /// Los bytes se muestran en filas de 16 columnas (estilo dump hexadecimal)
        /// </summary>
        /// <param name="Bufer">Array de bytes a mostrar</param>
        /// <param name="NumBytes">Número de bytes a mostrar del array</param>
        public void WriteHex(byte[] Bufer, int NumBytes)
        {
            // Número de columnas por fila (16 bytes = una línea típica en hex dumps)
            int NumCols = 16;

            // Calcular cuántas filas son necesarias para mostrar todos los bytes
            // Se usa Math.Ceiling para redondear hacia arriba (si no es divisible exactamente)
            int NumFils = (int)Math.Ceiling((double)NumBytes / (double)NumCols);

            // Índice para recorrer el array de bytes
            int i = 0;

            // Iterar por cada fila
            for (int Fil = 0; Fil < NumFils; Fil++)
            {
                // Iterar por cada columna de la fila actual
                for (int Col = 0; Col < NumCols; Col++)
                {
                    // Mostrar el byte en formato hexadecimal con 2 dígitos y un espacio
                    // {0,2:X2} significa: posición 0, ancho mínimo 2, formato hexadecimal mayúscula con 2 dígitos
                    Console.Write("{0,2:X2} ", Bufer[i]);
                    i++;
                    // Si hemos mostrado todos los bytes, salir del bucle
                    if (i >= NumBytes) break;
                }
                // Nueva línea al final de cada fila
                Console.WriteLine();
            }
        } // WriteHex()

        /// <summary>
        /// Obtiene el tamaño en bytes de un archivo
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo</param>
        /// <returns>Tamaño del archivo en bytes</returns>
        public long BytesFichero(string NombreFich)
        {
            // Abrir el archivo en modo lectura
            FileStream fs = new FileStream(NombreFich, FileMode.Open);
            
            // Obtener la propiedad Length que contiene el tamaño del archivo
            // Esta es la forma más eficiente (alternativa comentada lee byte a byte)
            long bytes = fs.Length;
            
            // Cerrar el archivo para liberar recursos
            fs.Close();
            return bytes;

        } // BytesFichero()


        /// <summary>
        /// Guarda un array de bytes en un archivo binario
        /// Si el archivo existe, lo sobrescribe
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo donde guardar</param>
        /// <param name="Bufer">Array de bytes a guardar</param>
        public void GuardaBufer(string NombreFich, byte[] Bufer)
        {
            // Crear un FileStream para escribir en el archivo
            // FileMode.Create: crea un archivo nuevo o sobrescribe si existe
            // FileAccess.Write: solo escritura
            // FileShare.None: no permite que otros procesos accedan mientras está abierto
            FileStream fs = new FileStream(NombreFich, FileMode.Create, FileAccess.Write, FileShare.None);
            
            // Escribir todo el contenido del buffer al archivo de una vez
            // Parámetros: buffer, offset (inicio), cantidad de bytes
            fs.Write(Bufer, 0, Bufer.Length);
            
            // Cerrar el archivo para asegurar que los datos se escriben y liberar recursos
            fs.Close();
        } // GuardaBufer()


        /// <summary>
        /// Carga el contenido de un archivo binario en un array de bytes
        /// Verifica que el buffer sea suficientemente grande antes de cargar
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo a leer</param>
        /// <param name="Bufer">Array de bytes donde cargar los datos (debe ser suficientemente grande)</param>
        public void CargaBufer(string NombreFich, byte[] Bufer)
        {
            // Abrir el archivo en modo lectura
            // FileMode.Open: el archivo debe existir
            // FileAccess.Read: solo lectura
            // FileShare.None: acceso exclusivo
            FileStream fs = new FileStream(NombreFich, FileMode.Open, FileAccess.Read, FileShare.None);
            
            // Verificar que el buffer tenga capacidad suficiente para los datos del archivo
            if (Bufer.Length < fs.Length)
            {
                Console.WriteLine("Tamaño de bufer insuficiente");
                Environment.Exit(0);
            }
            
            // Leer todo el contenido del archivo al buffer
            fs.Read(Bufer, 0, Bufer.Length);
            
            // Cerrar el archivo
            fs.Close();
        } // CargaBufer()
        
    } // class Ayuda
} // namespace Apoyo
