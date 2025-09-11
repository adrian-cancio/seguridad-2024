using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace AUDITA
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToEventLog = "RegWFA_2023-11-27.evtx";
            List<EventRecord> listaEventos = LeeFicheroEventos(pathToEventLog);

            Console.WriteLine("El número total de eventos son: " + listaEventos.Count);

            long[] listaID = { 2099, 2006 }; // IDs para reglas agregadas y eliminadas
            List<EventRecord> listaFiltrada = FiltroIDs(listaEventos, listaID);

            Console.WriteLine("El número total de eventos filtrados es: " + listaFiltrada.Count);

            VerDatosReglas(listaFiltrada);
            VerReglaAgregar(listaFiltrada);
        }

        static List<EventRecord> LeeFicheroEventos(string nombreFich)
        {
            EventLogReader lectorEventos = new EventLogReader(nombreFich, PathType.FilePath);
            List<EventRecord> listaEventos = new List<EventRecord>();
            EventRecord evento = lectorEventos.ReadEvent();

            while (evento != null)
            {
                listaEventos.Add(evento);
                evento = lectorEventos.ReadEvent();
            }
            return listaEventos;
        }

        static List<EventRecord> FiltroIDs(List<EventRecord> listaEventos, long[] listaID)
        {
            List<EventRecord> listaFiltrada = new List<EventRecord>();
            foreach (var evento in listaEventos)
            {
                foreach (var id in listaID)
                {
                    if (evento.Id == id)
                    {
                        listaFiltrada.Add(evento);
                        break;
                    }
                }
            }
            return listaFiltrada;
        }

        static void VerDatosReglas(List<EventRecord> listaEventos)
        {
            int agregadas = 0;
            int modificadas = 0;
            int eliminadas = 0;

            foreach (var evento in listaEventos)
            {
                if (evento.Id == 2099) agregadas++;
                else if (evento.Id == 2006) eliminadas++;
                // Asumimos que otro ID representa las modificadas
            }

            Console.WriteLine($"Reglas agregadas: {agregadas}");
            Console.WriteLine($"Reglas modificadas: {modificadas}");
            Console.WriteLine($"Reglas eliminadas: {eliminadas}");
        }

        static void VerReglaAgregar(List<EventRecord> listaEventos)
        {
            int agregadasEntranteBloquear = 0;
            int agregadasEntrantePermitir = 0;
            int agregadasSalienteBloquear = 0;
            int agregadasSalientePermitir = 0;

            foreach (var evento in listaEventos)
            {
                if (evento.Id == 2099)
                {
                    var direccion = evento.Properties[5].Value.ToString();
                    var accion = evento.Properties[9].Value.ToString();

                    if (direccion == "Entrante" && accion == "Bloquear") agregadasEntranteBloquear++;
                    if (direccion == "Entrante" && accion == "Permitir") agregadasEntrantePermitir++;
                    if (direccion == "Saliente" && accion == "Bloquear") agregadasSalienteBloquear++;
                    if (direccion == "Saliente" && accion == "Permitir") agregadasSalientePermitir++;
                }
            }

            Console.WriteLine($"Reglas agregadas para dirección Entrante y acción Bloquear: {agregadasEntranteBloquear}");
            Console.WriteLine($"Reglas agregadas para dirección Entrante y acción Permitir: {agregadasEntrantePermitir}");
            Console.WriteLine($"Reglas agregadas para dirección Saliente y acción Bloquear: {agregadasSalienteBloquear}");
            Console.WriteLine($"Reglas agregadas para dirección Saliente y acción Permitir: {agregadasSalientePermitir}");
        }
    }
}
