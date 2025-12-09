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
            FileInfo fichero = new FileInfo(NombreFich);
            return fichero.Length;
        } // BytesFichero()


        public void GuardaBufer(string NombreFich, byte[] Bufer)
        {
            File.WriteAllBytes(NombreFich, Bufer);

        } // GuardaBufer()
        

        public void CargaBufer(string NombreFich, byte[] Bufer)
        {
            byte[] buferIn = File.ReadAllBytes(NombreFich);
            buferIn.CopyTo(Bufer, 0);

        } // CargaBufer()

        public void GuardaTxt(string NombreFich, byte[] Bufer)
        {
            FileStream Fs = new FileStream(NombreFich, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter Sw = new StreamWriter(Fs);
            string resultado = BitConverter.ToString(Bufer).Replace("-", "");
            Sw.Write(resultado);
            Sw.Close();
            Fs.Close();
        } // GuardaTxt()

    } // class Ayuda

} // namespace Apoyo
