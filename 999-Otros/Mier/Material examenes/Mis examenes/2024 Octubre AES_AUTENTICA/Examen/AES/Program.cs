using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace AES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            // Pedimos el mensaje por consola
            Console.WriteLine("Introduce el mensaje a cifrar: ");
            string mensaje = Console.ReadLine();
            byte[] bytesmensaje = Encoding.UTF8.GetBytes(mensaje);

            byte[] clave = new byte[16];
            byte[] VI = new byte[16];

            a.CargaBufer("zzClaveAES.bin", clave);
            a.CargaBufer("zzVectorIniAES.bin", VI);

            Console.WriteLine("Vector clave: ");
            a.WriteHex(clave, clave.Length);

            Console.WriteLine("VI: ");
            a.WriteHex(VI, VI.Length);
            // Creamos el proveedor criptografico AES
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();


            // Asignamos las propiedades al proveedor cripografico
            aes.Key = clave;
            aes.IV = VI;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            a.WriteHex(clave, clave.Length);
            a.WriteHex(VI, VI.Length);

            // CIFRAR LA INFORMACIÓN

            // Flujo de datos FileStream para escribir el fichero
            FileStream fs = new FileStream("zzCadenaCifrada.bin", FileMode.Create, FileAccess.Write);

            ICryptoTransform cifrador = aes.CreateEncryptor();

            // Objeto  de  la  clase  CryptoStream para cifrar
            CryptoStream csCifrar = new CryptoStream(fs, cifrador, CryptoStreamMode.Write);

            // Objeto de la clase StreamWriter para cifrar caracteres con UTF8
            StreamWriter writer = new StreamWriter(csCifrar, Encoding.UTF8);

            // Ciframos el mensaje qu hemos pedido por consola
            Console.WriteLine("Mensaje cifrado");
            writer.Write(mensaje);


            // Cerrado ficheros
            writer.Close();
            csCifrar.Close();
            fs.Close();


        }
    }
}
