using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Para trabajar con ficheros
using System.IO;

/// <summary>
/// Clase de utilidades (plantilla) que proporciona métodos auxiliares para:
/// - Mostrar bytes en formato hexadecimal
/// - Trabajar con archivos (guardar, cargar, obtener tamaño)
/// 
/// NOTA: Esta es una versión plantilla de la clase Ayuda.
/// Los métodos BytesFichero, GuardaBufer y CargaBufer están vacíos 
/// y deben ser implementados como ejercicio.
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
        /// EJERCICIO: Implementar este método para devolver el tamaño del archivo
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo</param>
        /// <returns>Tamaño del archivo en bytes</returns>
        public long BytesFichero(string NombreFich)
        {
            // TODO: Implementar
            // Pista: Usar FileStream y la propiedad Length
        } // BytesFichero()


        /// <summary>
        /// Guarda un array de bytes en un archivo binario
        /// EJERCICIO: Implementar este método para guardar el buffer en un archivo
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo donde guardar</param>
        /// <param name="Bufer">Array de bytes a guardar</param>
        public void GuardaBufer(string NombreFich, byte[] Bufer)
        {
            // TODO: Implementar
            // Pista: Usar FileStream con FileMode.Create y Write()
        } // GuardaBufer()
        

        /// <summary>
        /// Carga el contenido de un archivo binario en un array de bytes
        /// EJERCICIO: Implementar este método para cargar datos de un archivo al buffer
        /// </summary>
        /// <param name="NombreFich">Ruta del archivo a leer</param>
        /// <param name="Bufer">Array de bytes donde cargar los datos</param>
        public void CargaBufer(string NombreFich, byte[] Bufer)
        {
            // TODO: Implementar
            // Pista: Usar FileStream con FileMode.Open y Read()
            // No olvidar verificar que el buffer es suficientemente grande
        } // CargaBufer()

    } // class Ayuda
} // namespace Apoyo
