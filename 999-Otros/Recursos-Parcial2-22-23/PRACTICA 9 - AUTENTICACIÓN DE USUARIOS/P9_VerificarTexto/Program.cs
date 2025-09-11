using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace P9_VerificarBinario
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Introduce un nombre de usuario:");
            string usuario = Console.ReadLine();
            Console.WriteLine("Introduce contraseña:");
            string contra = Console.ReadLine();
            UsuContra UC = new UsuContra();
            Console.WriteLine("Verificamos usuario y contraseña");
            switch (UC.Verifica(usuario, contra))
            {
                case 0:
                    Console.WriteLine("Verificacion correcta");
                    break;
                case 1:
                    Console.WriteLine("Usuario incorrecto");
                    break;
                case 2:
                    Console.WriteLine("Contraseña incorrecta");
                    break;
            }
            Console.ReadKey();
        }
    }
    public class UsuContra
    {
        private static int MaxUsu = 5;
        private static int lNombre = 16;
        private static int lSalt = 16;
        private static int lResu = 32;
        public char[] Nombre = new char[lNombre];
        public byte[] Salt = new byte[lSalt];
        public byte[] ResuContra = new byte[lResu];

        public int Verifica(string NombreIn, string ContraIn)
        {
            //Guardamos NombreIn
            char[] aux = new char[lNombre];
            for (int i = 0; i < aux.Length; i++)
            {
                aux[i] = '\0';
            }
            for (int j = 0; j < NombreIn.Length; j++)
            {
                aux[j] = NombreIn[j];
            }     

            //SALTCHAR Y RESUCONTRACHAR
            int NumCarSalt = (int)(4 * Math.Ceiling(((double)lSalt / 3.0)));
            char[] SaltChar = new char[NumCarSalt];
            int NumCarResuContra = (int)(4 * Math.Ceiling(((double)lResu / 3.0)));
            char[] ResuContraChar = new char[NumCarResuContra];


            //Leemos el fichero
            FileStream fs = new FileStream("zz_Usuarios.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Unicode);
            int n = 0;
            bool b = false;
            //VERIFICAR NOMBRE
            do
            {
                sr.Read(Nombre,0,lNombre);
                sr.Read(SaltChar, 0, NumCarSalt);
                Salt = Convert.FromBase64CharArray(SaltChar, 0, NumCarSalt);
                sr.Read(ResuContraChar, 0, NumCarResuContra);
                ResuContra = Convert.FromBase64CharArray(ResuContraChar, 0, NumCarResuContra);
                //Leer la linea
                sr.ReadLine();
                Console.WriteLine(Nombre);
                b = ComparaArray(Nombre, aux);
                n++;
            } while (!b && n < MaxUsu);
            sr.Close();
            fs.Close();
            if (n == MaxUsu)
            {
                return 1;
            }
            //VERIFICAR CONTRASEÑA
            //Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(ContraIn, Salt);
            //rfc.IterationCount = 1000;
            byte[] NuevaContraBytes = Encoding.Unicode.GetBytes(ContraIn);
            byte[] SaltYContra = new byte[Salt.Length+NuevaContraBytes.Length];
            Salt.CopyTo(SaltYContra, 0);
            NuevaContraBytes.CopyTo(SaltYContra, Salt.Length);

            SHA256Managed ProvSHA = new SHA256Managed();

            //Medicion del tiempo del calculo del resumen
            Stopwatch Crono = new Stopwatch();
            long Frecuencia = Stopwatch.Frequency;
            Crono.Start();
            byte[] ResuIn=ProvSHA.ComputeHash(SaltYContra);
           // byte[] ResuIn = rfc.GetBytes(ContraIn.Length);
            Crono.Stop();
            Console.WriteLine("Tiempo (ms) en calcular el resumen:");
            Console.WriteLine(Crono.ElapsedMilliseconds);
            Console.WriteLine("Tiempo(ticks):");
            double Tresu = ((double)(1000000L * Crono.ElapsedTicks)) / Frecuencia;
            Console.WriteLine(Tresu);

            if (ComparaResumen(ResuIn, ResuContra))
            {
                return 0;
            }
            else
            {
                return 2;
            }


        }

        public bool ComparaArray(char[] leido, char[] preparado)
        {
            if (leido.Length != preparado.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < leido.Length; i++)
                {
                    if (leido[i] != preparado[i])
                    {
                        return false;
                    }
                }
                return true;
            }

        }

        public bool ComparaResumen(byte[] calculado, byte[] leido)
        {
            if (leido.Length != leido.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < calculado.Length; i++)
                {
                    if (leido[i] != calculado[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
