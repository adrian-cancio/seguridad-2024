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
        

        //Devuelve el número de bytes almacenados en el fichero
        public long BytesFichero(string NombreFich)
        {
            FileStream fileStream = new FileStream(NombreFich, FileMode.Open);

            //Almacena en una variable el numero de bytes del fichero para retornar
            long bytes = fileStream.Length;

            fileStream.Close();
            fileStream.Dispose();
            return bytes;
        } // BytesFichero()
        
        //Guarda en un fichero NombreFich el buffer de memoria
        //indicado en el segundo parámetro (para fich bin)
        public void GuardaBufer(string NombreFich, byte[] Buffer)
        {
            FileStream fileStream = new FileStream(NombreFich, FileMode.Create, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);

            //Guarda en el fichero binario el buffer de memoria
            binaryWriter.Write(Buffer);

            binaryWriter.Close();
            fileStream.Close();
            binaryWriter.Dispose();
            fileStream.Dispose();
        } // GuardaBufer()
        
        //Carga en el buffer de memoria que se indica en el segundo
        //parámetro los bytes almacenados en el fichero cuyo nombre se
        //encuentra en el primer parámetro
        public void CargaBufer(string NombreFich, byte[] Buffer)
        {
            FileStream fileStream = File.OpenRead(NombreFich);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            //Carga en el buffer los bytes almaenados en el fichero binario
            fileStream.Read(Buffer, 0, (int)Buffer.Length);

            binaryReader.Close();
            fileStream.Close();
            binaryReader.Dispose();
            fileStream.Dispose();
        } // CargaBufer()

    } // class Ayuda
} // namespace Apoyo
