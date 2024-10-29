using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
          /*  //Declaramos un FileStream para NombreFich
            FileStream fs = new FileStream(NombreFich, FileMode.Open);
            //Comprobamos que existe el fichero pasado como parametro
            if (!File.Exists(NombreFich))
            {
                Console.WriteLine("El fichero no existe");
                Environment.Exit(0);
            }
            //Retornamos el numero de bytes del fichero en formato long
            return fs.Length;
            */
            long valor = 0;
            byte[] aux = File.ReadAllBytes(NombreFich);
            valor = aux.Length;

            return valor;
        } // BytesFichero()


        public void GuardaBufer(string NombreFich, byte[] Bufer)
        {
            //Creamos un objeto FileStream de soporte
            //Creamos un objeto BinaryWriter con el que escribir a Fichero
            FileStream fs = new FileStream(NombreFich, FileMode.OpenOrCreate);
            BinaryWriter bin_writer = new BinaryWriter(fs);
            //Escribimos en el buffer
            bin_writer.Write(Bufer);
            //Cerramos el fichero
            bin_writer.Close();
            fs.Close();
        } // GuardaBufer()
        

        public void CargaBufer(string NombreFich, byte[] Bufer)
        {
            //Creamos un objeto FileStream de soportepara poder cargar el fichero
            FileStream fs = File.OpenRead(NombreFich);

            //Cargamos en el bufer
            fs.Read(Bufer, 0,  Bufer.Length);
            fs.Close();
        } // CargaBufer()

    } // class Ayuda
} // namespace Apoyo
