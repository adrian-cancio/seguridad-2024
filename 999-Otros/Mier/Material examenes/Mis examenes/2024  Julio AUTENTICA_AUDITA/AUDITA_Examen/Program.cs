using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LecturaEventos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string rutaFichero = "eventJulio24.evtx";
            List<EventRecord> ListaEvt = LeeFicheroEventos(rutaFichero);
            Console.WriteLine("Numero de eventos leidos: " + ListaEvt.Count);
            List<EventRecord> ListaEvtFiltrada = FiltroIDs(ListaEvt, new long[] { 4625 });
            Console.WriteLine("Número de eventos de interés: " + ListaEvtFiltrada.Count);
            FiltroEventos(ListaEvtFiltrada);
            

        }

        static List<EventRecord> LeeFicheroEventos(string NombreFichero)
        {
            EventLogReader LectorEventos = new EventLogReader(NombreFichero, PathType.FilePath);
            List<EventRecord> ListaEventos = new List<EventRecord>();
            EventRecord e;
            while ((e = LectorEventos.ReadEvent()) != null)
            {
                ListaEventos.Add(e);
            }

            return ListaEventos;
        }

        static List<EventRecord> FiltroIDs(List<EventRecord> ListaEnt, long[] ListaID)
        {
            List<EventRecord> ListaSal = new List<EventRecord>();
            foreach (var Event in ListaEnt)
            {
                foreach (var ID in ListaID)
                {
                    if (ID == Event.Id)
                    {
                        ListaSal.Add(Event);
                        break;
                    }
                }
            }
            return ListaSal;
        }


        static void FiltroEventos(List<EventRecord> eventos)
        {

            int eventosInteres = 0;

            Console.WriteLine("Error de auditoria en logon al sistema");

            foreach (var evento in eventos)
            {
                if (evento.Id == 4625) // Id de error de acceso fallido
                {
                    eventosInteres++;
                    Console.WriteLine($"ID Evento: {evento.Id} || Fecha: {evento.TimeCreated}");
                }
            }

            Console.WriteLine($"\nAUTENTICACIONES INCORRECTAS TOTALES: {eventosInteres}");


        }

    }
}
