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
            //return File.ReadAllBytes(NombreFich).Length;
            FileStream fs = new FileStream(NombreFich, FileMode.Open);
            /* Metodo byte a byte:
            int bytes = 0;
            while (fs.ReadByte() != -1)
            {
                bytes++;
            }
            fs.Close();
            return bytes;
            */
            long bytes = fs.Length;
            fs.Close();
            return bytes;

        } // BytesFichero()


        public void GuardaBufer(string NombreFich, byte[] Bufer)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Create, FileAccess.Write, FileShare.None);
            /* Metodo byte a byte
            for (int i = 0; i < Bufer.Length; i++) 
            {
                fs.WriteByte(Bufer[i]);
            }
            */
            fs.Write(Bufer, 0, Bufer.Length);
            fs.Close();
        } // GuardaBufer()


        public void CargaBufer(string NombreFich, byte[] Bufer)
        {
            FileStream fs = new FileStream(NombreFich, FileMode.Open, FileAccess.Read, FileShare.None);
            if (Bufer.Length < fs.Length)
            {
                Console.WriteLine("Tamaño de bufer insuficiente");
                Environment.Exit(0);
            }
            fs.Read(Bufer, 0, Bufer.Length);
            fs.Close();
        } // CargaBufer()
        
    } // class Ayuda
} // namespace Apoyo
