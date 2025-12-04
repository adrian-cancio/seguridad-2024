using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace LecturaEventos
{
    internal class Program
    {
        /// <summary>
        /// Lee eventos de un fichero .evtx
        /// </summary>
        /// <param name="nombreFichero">Ruta del fichero con eventos</param>
        /// <returns>Lista de eventos leídos</returns>
        static List<EventRecord> LeeFicheroEventos(string nombreFichero)
        {
            EventLogReader LectorEventos = new EventLogReader(nombreFichero, PathType.FilePath);
            List<EventRecord> ListaEventos = new List<EventRecord>();

            EventRecord evento;
            while ((evento = LectorEventos.ReadEvent()) != null)
            {
                ListaEventos.Add(evento);
            }

            return ListaEventos;
        }

        /// <summary>
        /// Lee eventos de un registro de Windows (ej: Security, Application, System)
        /// </summary>
        /// <param name="nombreRegistro">Nombre del registro de Windows</param>
        /// <returns>Lista de eventos leídos</returns>
        static List<EventRecord> LeeRegistroEventos(string nombreRegistro)
        {
            EventLogReader LectorEventos = new EventLogReader(nombreRegistro, PathType.LogName);
            List<EventRecord> ListaEventos = new List<EventRecord>();

            EventRecord evento;
            while ((evento = LectorEventos.ReadEvent()) != null)
            {
                ListaEventos.Add(evento);
            }

            return ListaEventos;
        }

        /// <summary>
        /// Filtra eventos por IDs específicos
        /// </summary>
        /// <param name="ListaEnt">Lista de eventos de entrada</param>
        /// <param name="ListaID">Array con los IDs de interés</param>
        /// <returns>Lista de eventos filtrados</returns>
        static List<EventRecord> FiltroIDs(List<EventRecord> ListaEnt, long[] ListaID)
        {
            List<EventRecord> ListaSal = new List<EventRecord>();

            for (int i = 0; i < ListaEnt.Count; i++)
            {
                for (int j = 0; j < ListaID.Length; j++)
                {
                    if (ListaEnt[i].Id == ListaID[j])
                    {
                        ListaSal.Add(ListaEnt[i]);
                        break;
                    }
                }
            }

            return ListaSal;
        }

        static void Main(string[] args)
        {
            // ===== LECTURA DE EVENTOS DE UN FICHERO =====
            Console.WriteLine("===== LECTURA DE EVENTOS DE FICHERO =====");
            try
            {
                List<EventRecord> ListaEventosInicial = LeeFicheroEventos("Fichero.evtx");
                Console.WriteLine("Número de eventos leídos del fichero: " + ListaEventosInicial.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer fichero: " + ex.Message);
                Console.WriteLine("Asegúrese de que el fichero 'Fichero.evtx' existe en el directorio de ejecución.");
            }

            Console.WriteLine();

            // ===== LECTURA DE EVENTOS DE UN REGISTRO DE WINDOWS =====
            Console.WriteLine("===== LECTURA DE EVENTOS DE REGISTRO SECURITY =====");
            Console.WriteLine("NOTA: Requiere ejecutar como Administrador");
            try
            {
                List<EventRecord> ListaEventosInicial = LeeRegistroEventos("Security");
                Console.WriteLine("Número de eventos leídos del registro Security: " + ListaEventosInicial.Count);

                // Ejemplo de filtrado de eventos
                // IDs comunes de seguridad:
                // 4624 - Inicio de sesión exitoso
                // 4625 - Inicio de sesión fallido
                // 4634 - Cierre de sesión
                // 4648 - Inicio de sesión con credenciales explícitas
                long[] IDsInteres = { 4624, 4625, 4634, 4648 };
                List<EventRecord> ListaEventosFiltrados = FiltroIDs(ListaEventosInicial, IDsInteres);
                Console.WriteLine("Número de eventos filtrados (IDs 4624, 4625, 4634, 4648): " + ListaEventosFiltrados.Count);

                // Mostrar algunos eventos filtrados
                Console.WriteLine("\nPrimeros 5 eventos filtrados:");
                int contador = 0;
                foreach (EventRecord evento in ListaEventosFiltrados)
                {
                    if (contador >= 5) break;
                    Console.WriteLine($"  ID: {evento.Id}, Fecha: {evento.TimeCreated}, Nivel: {evento.LevelDisplayName}");
                    contador++;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Acceso denegado. Ejecute el programa como Administrador.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer registro: " + ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Pulse cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
