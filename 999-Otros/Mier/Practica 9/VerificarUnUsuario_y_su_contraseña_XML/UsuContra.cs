using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VerificarUnUsuario_y_su_contraseña_XML
{
    internal class UsuContra
    {
        // Declaraciones iniciales
        private const int maxUsu = 5;
        private const int maxNombre = 16;
        private const int maxSalt = 16;
        private const int maxResuContra = 32;
        private char[] Nombre = new char[maxNombre];
        private byte[] Salt = new byte[maxSalt];
        private byte[] ResuContra = new byte[maxResuContra];

        public byte[] NuevaContraBytes;
        public byte[] SaltYContra;

        // Metodo que verifica una contraseña
        public int Verifica(String NombreIn, String ContraIn)
        {
            bool usuarioEncontrado = false;
            int retorno = 0;

            // 1) Copiar el nombre
            char[] NombreInChars = new char[maxNombre];
            for (int i = 0; i < NombreInChars.Length; i++) { NombreInChars[i] = ' '; }
            for (int i = 0; i < NombreIn.Length; i++) { NombreInChars[i] = NombreIn[i]; }

            // 2) Declarar el SaltChar
            int NumCarSalt = (int)(4 * Math.Ceiling(((double)maxSalt / 3.0)));
            char[] SaltChar = new char[NumCarSalt];

            // 3) Declarar el array de caracteres ResuContraChar
            int NumCarResuContra = (int)(4 * Math.Ceiling(((double)maxResuContra / 3.0)));
            char[] ResuContraChar = new char[NumCarResuContra];

            // 4) Buscar el nombre en el fichero de contraseñas
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = true,
                DtdProcessing = DtdProcessing.Prohibit
            };
            FileStream fs = new FileStream("zz_Usuarios.xml", FileMode.Open);
            using (XmlReader reader = XmlReader.Create(fs, settings))
            {
                ////////////////////////// OPCION DETALLADA //////////////////////////////////////////////
                do
                {
                    // Valida que el lector esté en un nodo de inicio
                    if (reader.IsStartElement())
                    {
                        // Valida si el nodo actual es "NOMBRE"
                        if (reader.Name == "NOMBRE")
                        {
                            // Lee el nombre y elimina espacios adicionales
                            string nombreLeido = reader.ReadString().Trim();
                            Nombre = nombreLeido.PadRight(maxNombre).ToArray();

                            // Compara el nombre leído con el de entrada
                            if (usuarioEncontrado = CompareCharArray(Nombre, NombreInChars))
                            {
                                // Valida y lee el siguiente nodo "SALT"
                                if (reader.ReadToFollowing("SALT"))
                                {
                                    Salt = Convert.FromBase64String(reader.ReadString().Trim());
                                }

                                // Valida y lee el siguiente nodo "RESUCONTRA"
                                if (reader.ReadToFollowing("RESUCONTRA"))
                                {
                                    ResuContra = Convert.FromBase64String(reader.ReadString().Trim());
                                }

                                // Sale del bucle tras encontrar el usuario
                                break;
                            }
                        }
                    }
                } while (reader.Read());
                /////////////////////////////////////////////////////////////////////////////////////////

                ////////////////////////// OPCION COMPACTA //////////////////////////////////////////////
                if (reader.ReadToFollowing("LISTA"))
                {
                    do
                    {
                        // Valida que el nodo actual sea "USUARIO"
                        if (reader.IsStartElement("USUARIO"))
                        {
                            reader.ReadStartElement("USUARIO");

                            // Lee el nombre
                            if (reader.IsStartElement("NOMBRE"))
                            {
                                string nombreLeido = reader.ReadString().Trim();
                                Nombre = nombreLeido.PadRight(maxNombre).ToArray();
                                reader.ReadEndElement(); // de NOMBRE
                            }

                            // Lee el salt
                            if (reader.IsStartElement("SALT"))
                            {
                                Salt = Convert.FromBase64String(reader.ReadString().Trim());
                                reader.ReadEndElement(); // de SALT
                            }

                            // Lee el resumen de la contraseña
                            if (reader.IsStartElement("RESUCONTRA"))
                            {
                                ResuContra = Convert.FromBase64String(reader.ReadString().Trim());
                                reader.ReadEndElement(); // de RESUCONTRA
                            }

                            reader.ReadEndElement(); // de USUARIO

                            // Compara el usuario leído con el de entrada
                            if (usuarioEncontrado = CompareCharArray(Nombre, NombreInChars))
                            {
                                break;
                            }
                        }
                    } while (reader.Read());
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////

            // 5) Si se ha encontrado el usuario se muestra
            if (usuarioEncontrado)
            {
                Console.WriteLine("\nUsuario encontrado");
            }
            // Si no se ha encontrado, se muestra y se sale
            else { Console.WriteLine("\nUsuario no encontrado"); retorno = 1; return retorno;}

            // 6) Calcular el resumen de la contraseña
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(ContraIn, Salt);
            deriveBytes.IterationCount = 1000;
            byte[] resumenContra = deriveBytes.GetBytes(32);

            // APARTADOS DE MEJORA CON SHA Y TIEMPOS DE EJECUCION DEL RESUMEN
            /*NuevaContraBytes = Encoding.Unicode.GetBytes(ContraIn);
            SaltYContra = new byte[NuevaContraBytes.Length + Salt.Length];*/
            /*Salt.CopyTo(SaltYContra, 0);
            NuevaContraBytes.CopyTo(SaltYContra, Salt.Length);
            SHA256Managed sHA256 = new SHA256Managed();
            byte[] resumenContra = sHA256.ComputeHash(SaltYContra);*/

            // 7) Comprobar que los resumenes son iguales
            bool sonIguales = CompareByteArrays(resumenContra, ResuContra);
            if (sonIguales)
            {
                Console.WriteLine("\nLos resúmenes son iguales.");
                Console.WriteLine("Contraseña correcta");
            }
            else
            {
                Console.WriteLine("\nLos resúmenes son diferentes.");
                retorno = 2;
                return retorno;
            }
            return retorno;
        }

        // Metodo que compara dos arrays de caracteres
        static bool CompareCharArray(char[] array1, char[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        // Metodo que compara dos arrays de bytes
        static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }
    }
}