using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio2_AES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            // Cifrar un bloque de bytes de cualquier longitud a partir de una contraseña
            Console.WriteLine("Introduce una contraseña: ");
            string contraseña = Console.ReadLine();
            byte[] cod = Encoding.Unicode.GetBytes(contraseña);
            
            // Obtener un resumen de los bytes extraidos utilizando un objeto de la clase SHA384
            SHA384CryptoServiceProvider sha = new SHA384CryptoServiceProvider();
            byte[] resumen = sha.ComputeHash(cod);
            Console.WriteLine("Resumen generado: ");
            a.WriteHex(resumen, resumen.Length);


            // Generar una clave aes de 256 bits (32 bytes) usando los primeros bytes del resumen
            int TamClave = 32;
            byte[] clave = new byte[TamClave];
            for(int i = 0; i < TamClave; i++)
            {
                clave[i] = resumen[i];
            }

            // Escribir la clave por consola
            Console.WriteLine("Clave:");
            a.WriteHex(clave, clave.Length);


            // Generar el vector de inicialización con los ultimos bytes del resumen
            int tamVI = 16;
            byte[] VI = new byte[tamVI];
            for(int i = 0;i < tamVI; i++)
            {
                VI[i] = resumen[tamVI + i];
            }

            // Escribir VI por consola
            Console.WriteLine("VI:");
            a.WriteHex(VI, VI.Length);


            // Prepara la información a cifrar en un byte[] denominado TextoPlano
            // que contendra una longitud de 108 bytes y mostrar por consola
            byte[] TextoPlano = new byte[108];
            for(int i = 0; i < TextoPlano.Length; i++)
            {
                TextoPlano[i] = Convert.ToByte(i);
            }

            Console.WriteLine("Mensaje a cifrar: ");
            a.WriteHex(TextoPlano, TextoPlano.Length);



            // Crear el proveedor AES
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

            // Asignarle propiedades
            aes.KeySize = TamClave * 8;
            aes.Key = clave;
            aes.IV = VI;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ANSIX923;


            // Preparar la cadena de streams para el cifrado
            cifradoArrays(aes, "zz_TextoCifrado.bin", TextoPlano);

            // Comprobamos si el cifrado ha sido correcto realizando el descifrado
            byte[] textoDescifrado = new byte[a.BytesFichero("zz_TextoCifrado.bin")];
            int tamaño = descifradoArrays(aes, "zz_TextoCifrado.bin", textoDescifrado);

            Console.WriteLine("Texto descifrado: ");
            a.WriteHex(textoDescifrado, tamaño);
            


        }

        static void cifradoArrays(AesCryptoServiceProvider aes, String nombreFichero, Byte[] textoPlano)
        {
            // AesCryptoServiceProvider aes
            // Variables
            FileStream fileStreamCreate;
            ICryptoTransform encriptador;
            CryptoStream streamCifrado;

            // Guardo texto en el fichero
            fileStreamCreate = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write, FileShare.None);
            // Objeto para cifrar la información
            encriptador = aes.CreateEncryptor();
            // Stream para descifrar el texto
            streamCifrado = new CryptoStream(fileStreamCreate, encriptador, CryptoStreamMode.Write);
            streamCifrado.Write(textoPlano, 0, textoPlano.Length);

            // Mostrar por salida estándar
            streamCifrado.Flush();

            // Cerrar
            streamCifrado.Dispose();
            fileStreamCreate.Close();
        } // Cifrado arrays

        static int descifradoArrays(AesCryptoServiceProvider aes, String nombreFichero, Byte[] textoDescifrado)
        {
            // AesCryptoServiceProvider aes
            // Variables
            FileStream fileStreamOpen;
            ICryptoTransform desencriptador;
            CryptoStream streamDescifrado;

            // Proceso de descifrado de un array de bytes
            fileStreamOpen = new FileStream(nombreFichero, FileMode.Open, FileAccess.Read, FileShare.None);
            // Objeto para descifrar la información
            desencriptador = aes.CreateDecryptor();
            // Objeto para descifrar la información
            streamDescifrado = new CryptoStream(fileStreamOpen, desencriptador, CryptoStreamMode.Read);
            int leidos = streamDescifrado.Read(textoDescifrado, 0, textoDescifrado.Length);

            // Mostrar por salida estándar
            streamDescifrado.Flush();

            // Cerrar
            streamDescifrado.Dispose();
            fileStreamOpen.Close();

            return leidos;
        } //descifrado arrays
    }
}
