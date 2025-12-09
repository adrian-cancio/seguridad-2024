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
        // 4 metodos. leeFicheroEventos, Filtrar por IDs, propiedades del evento y filtrar eventos
        static void Main(string[] args)
        {
            List<EventRecord> listaEventos = leeFicheroEventos("FW.evtx"); // No tenemos el archivo disponible :(
            Console.WriteLine("El número de eventos de la lista es: " + listaEventos.Count);
            List<EventRecord> ListaEvtFiltrada = filtroID(listaEventos, new long[] { 4950 });
            Console.WriteLine("El número de eventos del registro filtrado es: " + ListaEvtFiltrada.Count);
            VerPropEven(ListaEvtFiltrada);
            filtrarEventos(ListaEvtFiltrada, "Público");
        }

        static List<EventRecord> leeFicheroEventos(string nombreFichero)
        {
            EventLogReader LectorEventos = new EventLogReader(nombreFichero, PathType.FilePath);
            List<EventRecord> listaEventos = new List<EventRecord>();

            EventRecord eventRecord = LectorEventos.ReadEvent();

            while(eventRecord != null)
            {
                listaEventos.Add(eventRecord);
            }

            return listaEventos;
        }

        static List<EventRecord> filtroID(List<EventRecord> listaEventos, long[] listaID)
        {
            List<EventRecord> salida = new List<EventRecord>();
            foreach(EventRecord record in listaEventos)
            {
                for(int i = 0; i < listaID.Length; i++)
                {
                    if(record.Id == listaID[i])
                    {
                        salida.Add(record);
                        break;
                    }
                }
            }

            return salida;
        }

        static void VerPropEven(List<EventRecord> listaEventos)
        {
            foreach(EventRecord record in listaEventos)
            {
                Console.WriteLine("Propiedades del evento con ID " + record.Id);
                IList<EventProperty> properties = record.Properties;
                foreach(EventProperty property in properties)
                {
                    Console.WriteLine(property.Value.ToString());
                }
            }
        }

        static void filtrarEventos(List<EventRecord> listaEventos, string PerfilFW)
        {
            String perfil, tipo, valor;

            // Declaramos el periodo de desactivacion
            List<PeriodoDesactivacion> ListPerDes = new List<PeriodoDesactivacion>();
            PeriodoDesactivacion PerDes = null;

            // Recorremos todos los eventos de la lista
            for(int i = 0;i < listaEventos.Count;i++)
            {
                // Saltamos el evento si su ID != 4950
                if (listaEventos[i].Id != 4950) continue;
                
                // Extraemos las propiedades del evento
                perfil = listaEventos[i].Properties[0].Value.ToString();
                tipo = listaEventos[i].Properties[1].Value.ToString();
                valor = listaEventos[i].Properties[2].Value.ToString();

                // Saltamos el evento si perfil != perfil seleccionado
                if (perfil != PerfilFW) continue;

                // Saltamos el evento si su tipo de configuración != "Registrar paquetes perdidos"
                if (tipo != "Registrar paquetes perdidos") continue;

                // Procesar el evento en funcion a los valores
                if(valor == "Sí")
                {
                    // Se activa el registro del firewall
                    PerDes = new PeriodoDesactivacion();

                    // Se crea un nuevo objeto de la clase periodo
                    PerDes.Tini = (DateTime)listaEventos[i].TimeCreated;
                }

                if(valor == "No")
                {
                    PerDes.Tfin = (DateTime)listaEventos[i].TimeCreated;
                    PerDes.Duracion = PerDes.Tfin - PerDes.Tini;

                    // Añadimos la informacion del evento al objeto actual y añadimos el objeto a la lista de periodos
                    ListPerDes.Add(PerDes);

                    // Se desactiva el registro del FW
                    PerDes = null;
                }

            }

            // Escribimos por consola
            Console.WriteLine("Se han encontrado los siguientes eventos con el perfil indicado: ");
            for(int i = 0; i < listaEventos.Count;i++)
            {
                Console.WriteLine("Periodo " + i + "desde " + ListPerDes.ElementAt(i).Tini + "hasta " + ListPerDes.ElementAt(i).Tfin + "y duración " + ListPerDes.ElementAt(i).Duracion);
            }
        }

        class PeriodoDesactivacion
        {
            public DateTime Tini = new DateTime();
            public DateTime Tfin = new DateTime();
            public TimeSpan Duracion = new TimeSpan();
        }




    }
}
