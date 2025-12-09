using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio2_AUDITA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<EventRecord> listaEvt = LeeFicheroEventos("UserSessionErrors.evtx");
            Console.WriteLine("El número de eventos de la lista es: " + listaEvt.Count);
            List<EventRecord> ListaEvtFiltrada = FiltroIDs(listaEvt, new long[] { 4625 });
            Console.WriteLine("El número de eventos del registro filtrado es: " + ListaEvtFiltrada.Count);
            FiltroEventos(ListaEvtFiltrada);
        }

        static List<EventRecord> LeeFicheroEventos(String nombreFichero)
        {
            EventLogReader LectorEventos = new EventLogReader(nombreFichero, PathType.FilePath);
            List<EventRecord> ListaEventos = new List<EventRecord>();
            EventRecord evt;
            while ((evt = LectorEventos.ReadEvent()) != null)
            {
                ListaEventos.Add(evt);
            }
            return ListaEventos;
        }

        static List<EventRecord> FiltroIDs(List<EventRecord> ListaEnt, long[] ListaID)
        {
            List<EventRecord> ListaSal = new List<EventRecord>();
            foreach (EventRecord e in ListaEnt)
            {
                for (int i = 0; i < ListaID.Length; i++)
                {
                    if (e.Id == ListaID[i])
                    {
                        ListaSal.Add(e);
                        break;
                    }
                }
            }
            return ListaSal;
        }

        static void FiltroEventos(List<EventRecord> listaEnt)
        {

            List<List<EventRecord>> ListaGrupos = new List<List<EventRecord>>(); //declaramos la lista de grupos como una lista de lista de EventRecord
            List<EventRecord> Grupo = null; //declaramos grupo como una lista de EventRecord

            //declaramos las variables para los tiempos
            DateTime Tprevio = DateTime.MinValue;
            DateTime Tactual = DateTime.MinValue;

            // Recorremos todos los eventos de la lista 
            for (int i = 0;i < listaEnt.Count;i++)
            {
                if (listaEnt[i].Id != 4625) continue; // Solo nos interesan los eventos con id 4625, si no continuamos

                // Almacenar el intervalo entre el evento actual y el previo en un timespan
                Tactual = (DateTime)listaEnt[i].TimeCreated;
                TimeSpan intervalo = Tactual - Tprevio; // Creamos el intervalo

                double Imax = 10;

                if(intervalo.TotalSeconds > Imax)
                {
                    // Creamos un nuevo objeto grupo y lo añadimos a ListaGrupos
                    Grupo = new List<EventRecord>();
                    ListaGrupos.Add(Grupo);
                }

                // Añadimos el evento actual a grupo
                Grupo.Add(listaEnt[i]);

                // Asignamos tactual a tprevio
                Tprevio = Tactual;

                Console.WriteLine("Evento añadido");
            }

            // Mostrar por la consola una linea para cada grupo encontrado: GRUPO 1, GRUPO 2... y debajo de cada grupo
            // una linea para cada evento incluido en el grupo con estos campos: fecha-hora_creacion ID = Num_evento TaskDisplayName
            for (int i = 0; i < ListaGrupos.Count;i++)
            {
                Console.WriteLine("Grupo {0}", i + 1);

                // Recorremos los eventos de cada grupo
                for (int j = 0; j < ListaGrupos[i].Count;j++)
                {
                    EventRecord evt = ListaGrupos[i][j];
                    Console.WriteLine(evt.TimeCreated + " ID=" + evt.Id + " " + evt.TaskDisplayName);
                }
            }

        }


    }
}
