using Apoyo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CifradoFicheroAES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int TamClave = 32;
            byte[] Clave = new byte[TamClave];
            for (int i = 0; i < Clave.Length; i++) Clave[i] = (byte)(i % 256);

            byte[] VI = new byte[16];
            for (int i = 0; i < VI.Length; i++) VI[i] = (byte)((i + 160) % 256);

            String NombreFichPlano = "zz_TextoPlano.txt";

            Ayuda ayuda = new Ayuda();
            long tamTextoPlano = ayuda.BytesFichero(NombreFichPlano);
            byte[] TextoPlano = null;
            int MaxTamFich = 1024;
            Console.WriteLine("Tamaño del fichero: " + tamTextoPlano);
            if (tamTextoPlano <= MaxTamFich)
            {
                TextoPlano = new byte[tamTextoPlano];
                ayuda.CargaBufer(NombreFichPlano, TextoPlano);
            }            
            AesManaged Provider = new AesManaged();

            Provider.Key = Clave;
            Provider.Padding = PaddingMode.PKCS7;
            Provider.Mode = CipherMode.ECB;

            Console.WriteLine("Provider properties:");
            Console.WriteLine("BlockSize: " + Provider.BlockSize);
            Console.WriteLine("KeySize: " + Provider.KeySize);
            Console.WriteLine("Padding: " + Provider.Padding);
            Console.WriteLine("Mode: " + Provider.Mode);
            Console.WriteLine();

            Provider.GenerateKey();

            Console.WriteLine("Clave de cifrado:");
            ayuda.WriteHex(Provider.Key, TamClave);

            Provider.GenerateIV();
            Console.WriteLine("Vector de inicialización:");
            ayuda.WriteHex(Provider.IV, 16);

            const String NombreFicheroCifrado = "zz_TextoCifrado.bin";
            FileStream FicheroCifrado = new FileStream(NombreFicheroCifrado, FileMode.Create);

            ICryptoTransform Cifrador = Provider.CreateEncryptor();

            CryptoStream cs = new CryptoStream(FicheroCifrado, Cifrador, CryptoStreamMode.Write);

            if (TextoPlano != null)
            {
                cs.Write(TextoPlano, 0, TextoPlano.Length);
            }
            else {
                FileStream fsr = new FileStream(NombreFichPlano, FileMode.Open, FileAccess.Read, FileShare.None);
                int i = 0;
                TextoPlano = new byte[MaxTamFich];
                StreamReader sr = new StreamReader(fsr);
                while (!sr.EndOfStream)
                {
                    int n = sr.BaseStream.Read(TextoPlano, 0, MaxTamFich);
                    cs.Write(TextoPlano, 0, n);
                    i += n;
                }
            }

        }
    }
}
