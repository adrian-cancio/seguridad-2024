using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Apoyo;
namespace practica3AES_ficheros_small
{
    class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();
            int TamClave = 24;
            byte[] Clave = new byte[TamClave];
            byte[] VI = new byte[16];
            AesManaged aes = new AesManaged();
            for (int i = 0; i < Clave.Length; i++)
            {
                Clave[i] = (byte)(i % 256);
            }
            for (int i = 0; i < VI.Length; i++)
            {
                VI[i] = (byte)((i + 160) % 256);
            }
            //Blocksize=128 Keysize=256 Padding=PKCS7 Mode=CBC 
            Console.WriteLine("Blocksize= " + aes.BlockSize);
            Console.WriteLine("Keysize= " + aes.KeySize);
            Console.WriteLine("Padding= " + aes.Padding);
            Console.WriteLine("Mode= " + aes.Mode);
            Console.Write("\n\n\n");

            //Padding: ANSIX923-4 ISO10126-5 None-1 PKCS7-2 Zeros-3
            //Mode: CBC-1 CFB-4 CTS-5 ECB-2 OFB-3
            aes.BlockSize = 128; //Siempre 128
            aes.KeySize = 128; //128, 192, 256
            aes.Padding = PaddingMode.PKCS7; // TODOS 
            aes.Mode = CipherMode.ECB; //ECB, CBC, CFB, OFB

            Console.WriteLine("Una vez modificados");
            Console.WriteLine("Blocksize= " + aes.BlockSize);
            Console.WriteLine("Keysize= " + aes.KeySize);
            Console.WriteLine("Padding= " + aes.Padding);
            Console.WriteLine("Mode= " + aes.Mode);
            Console.Write("\n\n\n");

            //Creamos nueva clave
            aes.GenerateKey();
            Console.WriteLine("Clave simetrica generada: ");
            a.WriteHex(aes.Key, aes.KeySize / 8);

            //Asignamos a la clave la clave creada
            aes.KeySize = TamClave * 8;
            aes.Key = Clave;
            Console.WriteLine("Clave generada al inicio: ");
            a.WriteHex(aes.Key, aes.KeySize / 8);

            //Generar vector de inicializacion
            aes.GenerateIV();
            Console.WriteLine("Vector de inicializacion generado: ");
            a.WriteHex(aes.IV, aes.IV.Length);

            //Asignar vector de inicializacion creado
            aes.IV = VI;
            Console.WriteLine("Vector de inicializacion generado al inicio: ");
            a.WriteHex(aes.IV, aes.IV.Length);

            //Abrir un fichero cifrado para descifrarlo

            Console.WriteLine("Desciframos ahora la cadena: ");
            FileStream fsr = new FileStream("FicheroCifrado.txt", FileMode.Open);
            FileStream f2 = new FileStream("FicheroDescifrado.txt", FileMode.Create);
            ICryptoTransform icr = aes.CreateDecryptor();
            CryptoStream csr = new CryptoStream(fsr, icr, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(csr);
            StreamWriter s = new StreamWriter(f2);
            int len = 0;
            char[] buf = new char[4096];
            while ((len = sr.ReadBlock(buf, 0, 4096)) != 0)
            {
                s.Write(buf);
            }
            Console.Write("\n\n");
            Console.WriteLine("Se ha descifrado el fichero");
            csr.Close();
            csr.Flush();
            icr.Dispose();
            fsr.Close();
            f2.Close();
        }
    }
}
