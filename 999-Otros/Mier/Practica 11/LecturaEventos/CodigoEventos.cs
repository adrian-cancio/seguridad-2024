using System;                                      // Espacio de nombres principal de .NET que incluye tipos básicos
using System.Collections.Generic;                  // Permite usar colecciones genéricas como List<T>
using System.Diagnostics.Eventing.Reader;          // Contiene clases para leer eventos del Visor de Eventos de Windows

namespace LecturaEventos
{
    internal class CodigoEventos
    {
        static void Main(string[] args)
        {
            // Llama a la función que lee los eventos del fichero "eventos.evtx"
            List<EventRecord> Records = LeeFicheroEventos("eventos.evtx");
            // Mostrar por pantalla el numero de eventos en la lista
            Console.WriteLine("Eventos en el fichero eventos.evtx: " + Records.Count);

            // Llamada a LeeRegistroEventos => EJECUTAR EL PROGRAMA EN MODO ADMINISTRADOR
            List<EventRecord> Records2 = LeeRegistroEventos("Security");
            // Mostrar por pantalla el numero de eventos en la lista
            Console.WriteLine("Eventos en el Registro de Windows: " + Records2.Count);

            Console.WriteLine();

            // Aplica un filtro a la lista de eventos, quedándose solo con aquellos
            // cuyos IDs coinciden con los indicados en el array (en este caso, 4950)
            List<EventRecord> RecordsFiltrado = FiltroIDs(Records, new long[1] { 5379 });
            // Mostrar por pantalla el numero de eventos que pasan el filtro
            Console.WriteLine("Eventos filtrados del fichero eventos.evtx (siendo: new long[1] { 5379 }): " + RecordsFiltrado.Count);

            // Otra forma de hacer un filtro. Lo aplicamos al Registros de Windows por ejemplo
            long[] filtros = new long[2];
            filtros[0] = 1234;
            filtros[1] = 5379;
            List<EventRecord> RecordsFiltrado2 = FiltroIDs(Records2, filtros);
            // Mostrar por pantalla el numero de eventos que pasan el filtro
            Console.WriteLine("Eventos filtrados del Registro de Windows (siendo: new long[] filtros = new long[2] filtros[0] = 1234 y filtros[1] = 5379): " + RecordsFiltrado2.Count);

            Console.WriteLine();

            // Llama a la función que lee los eventos del fichero "firewall.evtx" para mostrar períodos en los que se ha desactivado el Firewall en distintos perfiles: Público, Privado y Dominio
            List<EventRecord> RecordsF = LeeFicheroEventos("firewall.evtx");
            // Mostrar por pantalla el numero de eventos en la lista
            Console.WriteLine("Eventos en el fichero firewall.evtx: " + RecordsF.Count);

            Console.WriteLine();

            Console.WriteLine("Períodos en los que se ha desactivado el Firewall en perfil Público");
            VerPerFwDesactivado(RecordsF, "Público");
            Console.WriteLine();
            Console.WriteLine("Períodos en los que se ha desactivado el Firewall en perfil Privado");
            VerPerFwDesactivado(RecordsF, "Privado");
            Console.WriteLine();
            Console.WriteLine("Períodos en los que se ha desactivado el Firewall en perfil Dominio");
            VerPerFwDesactivado(RecordsF, "Dominio");
        }

        // ------------------------------   LECTURA DE EVENTOS DE UN FICHERO   ------------------------------ //

        // Método que lee un fichero de eventos (EVTX) usando EventLogReader 
        // y los va agregando a una lista de EventRecord
        static List<EventRecord> LeeFicheroEventos(string NombreFichero)
        {
            // Crea un lector de eventos para un archivo EVTX específico
            // Declarar un lector de eventos
            EventLogReader LectorEventos = new EventLogReader(NombreFichero, PathType.FilePath);
            // Declarar la lista de eventos a devolver
            List<EventRecord> ListaEventos = new List<EventRecord>();
            // Elemento que sera leido
            EventRecord evento = LectorEventos.ReadEvent(); ;

            // Lee cada evento hasta que no haya más (retorne null)
            while (evento != null)
            {
                // Añade el evento a la lista
                ListaEventos.Add(evento);
                // Pasa al siguiente evento
                evento = LectorEventos.ReadEvent();
            }

            // Retorna la lista
            return ListaEventos;
        }

        // ------------------------------   LECTURA DE EVENTOS DE UN REGISTRO DE WINDOWS   ------------------------------ //
        
        // Igual que el anterior pero cambia el segundo parametro del metodo por PathType.LogName
        static List<EventRecord> LeeRegistroEventos(String registro)
        {
            // Declaracion de un lector de eventos -> Cambia el segundo parametro del constructor
            EventLogReader LectorEventos = new EventLogReader(registro, PathType.LogName);
            // Declara la lista de eventos a devolver
            List<EventRecord> ListaEventos = new List<EventRecord>();
            // Elemento que sera leido
            EventRecord evento = LectorEventos.ReadEvent();

            // Mientras el evento exista, los recorre todos
            while (evento != null)
            {
                // Añade el evento a la lista
                ListaEventos.Add(evento);
                // Pasa al siguiente evento
                evento = LectorEventos.ReadEvent();
            }
            // Retorna la lista
            return ListaEventos;
        }

        // ------------------------------   FILTRADO DE EVENTOS POR "IDEvento"   ------------------------------ //

        // Aplica un filtro a la lista de eventos: solo se devuelven los eventos
        // cuyo ID esté presente en el array de IDs especificado
        static List<EventRecord> FiltroIDs(List<EventRecord> ListaEnt, long[] ListaID)
        {
            // Declaracion de la lista de retorno
            List<EventRecord> ListaSal = new List<EventRecord>();

            // Bucle que recorre todos los elementos de la lista de eventos inicial
            for (int i = 0; i < ListaEnt.Count; i++)
            {
                // Bucle que recorrre todos los eleentos de la lista de IDs
                for (int j = 0; j < ListaID.Length; j++)
                {
                    // Compara si el ID del elemento esta en la lista de IDs
                    if (ListaEnt[i].Id == ListaID[j])
                    {
                        // Si el ID del evento es de los buscados, se añade a la lista
                        ListaSal.Add(ListaEnt[i]);
                        // Sale del bucle interno
                        break;
                    }
                }
            }
            // Devuelve la lista
            return ListaSal;
        }

        // ------------------------------   PERIODOS DE ACTIVACION Y DESACTIVACION DEL FIREWALL   ------------------------------ //

        // Método que identifica períodos de desactivación del Firewall para un perfil dado.
        // Funciona buscando en la lista de eventos aquellos con ID 4950 (cambios de configuración).
        // Si la propiedad "Habilitar Firewall" pasa de "No" a "Sí", se cierra un período de desactivación.
        static void VerPerFwDesactivado(List<EventRecord> ListEvent, string PerfilFW)
        {
            PeriodoDesactivacion PerDes = null;               // Almacena un período de desactivación en construcción
            List<PeriodoDesactivacion> LisPerDes = new List<PeriodoDesactivacion>(); // Lista de períodos completos

            // Recorre la lista de eventos
            foreach (EventRecord Event in ListEvent)
            {
                // Solo nos interesan los eventos con ID 4950 porque son los relacionados con el FW
                if (Event.Id != 4950) continue;

                // Las propiedades del evento determinan el perfil, el tipo de config y el valor de config
                string Perfil = Event.Properties[0].Value?.ToString();
                string TipoConfig = Event.Properties[1].Value?.ToString();
                string ValorConfig = Event.Properties[2].Value?.ToString();

                // Solo consideramos eventos que coincidan con el perfil seleccionado 
                if (Perfil != PerfilFW) continue;

                // Comprobamos también que se trate del cambio de "Habilitar Firewall de Windows Defender"
                if (TipoConfig != "Habilitar Firewall de Windows Defender") continue;

                // Si ya tenemos un período abierto (PerDes != null) y el nuevo valor es "Sí" => se cierra el período
                if (PerDes != null && ValorConfig == "Sí")
                {
                    // Se define la hora de fin
                    PerDes.Tfin = Event.TimeCreated.GetValueOrDefault();
                    // Calcula la duración (diferencia entre fin e inicio)
                    PerDes.Duracion = PerDes.Tfin - PerDes.Tini;
                    // Se añade a la lista de períodos y se reinicia la variable temporal
                    LisPerDes.Add(PerDes);
                    PerDes = null;
                }
                // Si no tenemos un período abierto y el valor es "No" => se abre un nuevo período
                else if (PerDes == null && ValorConfig == "No")
                {
                    PerDes = new PeriodoDesactivacion();
                    PerDes.Tini = Event.TimeCreated.GetValueOrDefault();
                }
                // Cualquier otro caso se considera inesperado
                else
                {
                    Console.WriteLine($"Aviso: Evento fuera de secuencia para perfil '{PerfilFW}' en {Event.TimeCreated} (valor: {ValorConfig})");
                }
            }

            // Muestra por consola los períodos de desactivación encontrados
            foreach (PeriodoDesactivacion Periodo in LisPerDes)
            {
                Console.WriteLine($"Período de desactivación desde {Periodo.Tini} hasta {Periodo.Tfin} y duración {Periodo.Duracion}");
            }
        }
    }

    // Clase que modela un período de desactivación del Firewall
    internal class PeriodoDesactivacion
    {
        public DateTime Tini = new DateTime();      // Tiempo de inicio de la desactivación
        public DateTime Tfin = new DateTime();      // Tiempo de fin de la desactivación
        public TimeSpan Duracion = new TimeSpan();  // Diferencia entre Tfin y Tini
    }
}
