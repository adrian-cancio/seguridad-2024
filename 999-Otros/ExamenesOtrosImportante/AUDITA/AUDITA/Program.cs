using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

/// <summary>
/// PROGRAMA DE EXAMEN: AUDITA
/// Este programa cayó en el segundo parcial.
/// 
/// FUNCIÓN:
/// Audita (analiza) los eventos del firewall de Windows almacenados en un archivo .evtx
/// para generar estadísticas sobre las reglas del cortafuegos.
/// 
/// CONCEPTOS CLAVE:
/// - Archivo .evtx: formato binario de registro de eventos de Windows
/// - EventLogReader: clase de .NET para leer archivos .evtx
/// - EventRecord: representa un evento individual del log
/// - Event ID: identificador único del tipo de evento
///   * 2099: Regla agregada al firewall
///   * 2006: Regla eliminada del firewall
/// 
/// FUNCIONALIDAD:
/// 1. Lee un archivo de eventos del firewall (.evtx)
/// 2. Filtra eventos por IDs específicos (2099 y 2006)
/// 3. Genera estadísticas de reglas agregadas, modificadas y eliminadas
/// 4. Desglosa las reglas agregadas por dirección y acción:
///    - Dirección: Entrante o Saliente
///    - Acción: Bloquear o Permitir
/// 
/// PROPIEDADES DE LOS EVENTOS (EventRecord.Properties):
/// - Properties[5]: Dirección de la regla (Entrante/Saliente)
/// - Properties[9]: Acción de la regla (Bloquear/Permitir)
/// 
/// SALIDA DEL PROGRAMA:
/// - Total de eventos en el archivo
/// - Total de eventos filtrados (solo reglas agregadas/eliminadas)
/// - Estadísticas de reglas por tipo
/// - Desglose de reglas agregadas por dirección y acción
/// </summary>
namespace AUDITA
{
    class Program
    {
        static void Main(string[] args)
        {
            // Nombre del archivo de eventos a analizar
            string pathToEventLog = "RegWFA_2023-11-27.evtx";
            
            // Leer todos los eventos del archivo
            List<EventRecord> listaEventos = LeeFicheroEventos(pathToEventLog);

            Console.WriteLine("El número total de eventos son: " + listaEventos.Count);

            // IDs de eventos a filtrar:
            // 2099: Regla agregada
            // 2006: Regla eliminada
            long[] listaID = { 2099, 2006 };
            
            // Filtrar solo los eventos que coinciden con los IDs
            List<EventRecord> listaFiltrada = FiltroIDs(listaEventos, listaID);

            Console.WriteLine("El número total de eventos filtrados es: " + listaFiltrada.Count);

            // Generar estadísticas
            VerDatosReglas(listaFiltrada);
            VerReglaAgregar(listaFiltrada);
        }

        /// <summary>
        /// Lee todos los eventos de un archivo .evtx y los devuelve en una lista
        /// </summary>
        /// <param name="nombreFich">Ruta del archivo .evtx</param>
        /// <returns>Lista de todos los eventos del archivo</returns>
        static List<EventRecord> LeeFicheroEventos(string nombreFich)
        {
            // Crear un lector de eventos para el archivo
            // PathType.FilePath indica que nombreFich es una ruta de archivo
            EventLogReader lectorEventos = new EventLogReader(nombreFich, PathType.FilePath);
            
            // Lista para almacenar todos los eventos
            List<EventRecord> listaEventos = new List<EventRecord>();
            
            // Leer el primer evento
            EventRecord evento = lectorEventos.ReadEvent();

            // Leer todos los eventos del archivo
            while (evento != null)
            {
                listaEventos.Add(evento);
                evento = lectorEventos.ReadEvent(); // Leer siguiente evento
            }
            
            return listaEventos;
        }

        /// <summary>
        /// Filtra una lista de eventos para quedarse solo con los que tienen ciertos IDs
        /// </summary>
        /// <param name="listaEventos">Lista completa de eventos</param>
        /// <param name="listaID">Array de IDs a buscar</param>
        /// <returns>Lista de eventos que coinciden con alguno de los IDs</returns>
        static List<EventRecord> FiltroIDs(List<EventRecord> listaEventos, long[] listaID)
        {
            List<EventRecord> listaFiltrada = new List<EventRecord>();
            
            // Recorrer cada evento
            foreach (var evento in listaEventos)
            {
                // Verificar si el ID del evento coincide con algún ID buscado
                foreach (var id in listaID)
                {
                    if (evento.Id == id)
                    {
                        listaFiltrada.Add(evento);
                        break; // Ya encontramos coincidencia, no seguir buscando
                    }
                }
            }
            
            return listaFiltrada;
        }

        /// <summary>
        /// Muestra estadísticas generales de reglas agregadas, modificadas y eliminadas
        /// </summary>
        /// <param name="listaEventos">Lista de eventos filtrados</param>
        static void VerDatosReglas(List<EventRecord> listaEventos)
        {
            int agregadas = 0;
            int modificadas = 0;
            int eliminadas = 0;

            // Contar eventos por tipo
            foreach (var evento in listaEventos)
            {
                if (evento.Id == 2099) 
                    agregadas++;  // ID 2099 = regla agregada
                else if (evento.Id == 2006) 
                    eliminadas++; // ID 2006 = regla eliminada
                // Nota: En este ejemplo no se manejan reglas modificadas
            }

            // Mostrar resultados
            Console.WriteLine($"Reglas agregadas: {agregadas}");
            Console.WriteLine($"Reglas modificadas: {modificadas}");
            Console.WriteLine($"Reglas eliminadas: {eliminadas}");
        }

        /// <summary>
        /// Muestra estadísticas detalladas de reglas agregadas,
        /// desglosadas por dirección (Entrante/Saliente) y acción (Bloquear/Permitir)
        /// </summary>
        /// <param name="listaEventos">Lista de eventos filtrados</param>
        static void VerReglaAgregar(List<EventRecord> listaEventos)
        {
            // Contadores para cada combinación de dirección y acción
            int agregadasEntranteBloquear = 0;
            int agregadasEntrantePermitir = 0;
            int agregadasSalienteBloquear = 0;
            int agregadasSalientePermitir = 0;

            // Analizar cada evento
            foreach (var evento in listaEventos)
            {
                // Solo procesar eventos de reglas agregadas (ID 2099)
                if (evento.Id == 2099)
                {
                    // Extraer la dirección de la regla (Properties[5])
                    var direccion = evento.Properties[5].Value.ToString();
                    
                    // Extraer la acción de la regla (Properties[9])
                    var accion = evento.Properties[9].Value.ToString();

                    // Clasificar según dirección y acción
                    if (direccion == "Entrante" && accion == "Bloquear") 
                        agregadasEntranteBloquear++;
                    if (direccion == "Entrante" && accion == "Permitir") 
                        agregadasEntrantePermitir++;
                    if (direccion == "Saliente" && accion == "Bloquear") 
                        agregadasSalienteBloquear++;
                    if (direccion == "Saliente" && accion == "Permitir") 
                        agregadasSalientePermitir++;
                }
            }

            // Mostrar resultados detallados
            Console.WriteLine($"Reglas agregadas para dirección Entrante y acción Bloquear: {agregadasEntranteBloquear}");
            Console.WriteLine($"Reglas agregadas para dirección Entrante y acción Permitir: {agregadasEntrantePermitir}");
            Console.WriteLine($"Reglas agregadas para dirección Saliente y acción Bloquear: {agregadasSalienteBloquear}");
            Console.WriteLine($"Reglas agregadas para dirección Saliente y acción Permitir: {agregadasSalientePermitir}");
        }
    }
}
