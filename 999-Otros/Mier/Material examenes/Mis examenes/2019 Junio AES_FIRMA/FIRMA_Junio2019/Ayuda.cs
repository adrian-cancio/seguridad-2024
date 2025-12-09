using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Para trabajar con ficheros
using System.IO;


namespace Apoyo
{
    class Ayuda
    {
        public void WriteHex(byte[] Bufer, int NumBytes)
        {

            int NumCols = 16;

            // Console.WriteLine("NumBytes: " + NumBytes); // DEBUG
            int NumFils = (int)Math.Ceiling((double)NumBytes / (double)NumCols);
            // Console.WriteLine("NumFils: " + NumFils); // DEBUG

            int i = 0;

            for (int Fil = 0; Fil < NumFils; Fil++)
            {
                for (int Col = 0; Col < NumCols; Col++)
                {
                    Console.Write("{0,2:X2} ", Bufer[i]);
                    i++;
                    if (i >= NumBytes) break;
                }
                Console.WriteLine();
            }
        } // WriteHex()

        public long BytesFichero(string NombreFich)
        {

            long resultado = -1;

            try
            {
                FileInfo fi;
                fi = new FileInfo(NombreFich);
                resultado = fi.Length;

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Ha ocurrido un error obtener información del archivo");
            }
            return resultado;

        } // BytesFichero()


        public void GuardaBufer(string NombreFich, byte[] Bufer)
        {

            try
            {
                // Abro fichero
                FileStream sw;
                sw = new FileStream(NombreFich, FileMode.Create, FileAccess.Write, FileShare.None);
                if (sw == null)
                {
                    throw new FileNotFoundException();
                }
                // Escribo en fichero
                sw.Write(Bufer, 0, Bufer.Length);

                // Cierro fichero
                sw.Close();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Ha ocurrido un error al escribir en el archivo");
            }
        } // GuardaBufer()


        public void CargaBufer(string NombreFich, byte[] Bufer)
        {

            try
            {
                // Abro fichero
                FileStream sr;
                sr = new FileStream(NombreFich, FileMode.Open, FileAccess.Read, FileShare.None);
                if (sr == null)
                {
                    throw new FileNotFoundException();
                }
                // Leo fichero
                sr.Read(Bufer, 0, Bufer.Length);

                // Cierro fichero
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Ha ocurrido un error al abrir el archivo");
            }

        } // CargaBufer()

    } // class Ayuda
} // namespace Apoyo
