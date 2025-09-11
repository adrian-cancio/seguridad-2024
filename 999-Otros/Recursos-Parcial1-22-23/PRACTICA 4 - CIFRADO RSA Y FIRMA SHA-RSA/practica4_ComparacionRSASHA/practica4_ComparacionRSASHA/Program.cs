using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apoyo;
using System.Security.Cryptography;
namespace practica4_ComparacionRSASHA
{
    class Program
    {
        static void Main(string[] args)
        {
            Ayuda help = new Ayuda();
            byte[] Mensaje = new byte[64];
            for (int i = 0; i < Mensaje.Length; i++)
            {
                Mensaje[i] = (byte)(i % 256);
            }

            //FASE 1
            //Creamos un objeto proveedor de servicios criptográficos para SHA256
            SHA256CryptoServiceProvider sha_provider = new SHA256CryptoServiceProvider();
            Console.WriteLine("Propiedad - Tamaño del Hash generado: " + sha_provider.HashSize);
            //Declaramos el array resumen y lo cargamos con el valor del hash obtenido del objeto
            //proveedor
            byte[] Resumen = sha_provider.ComputeHash(Mensaje);
            Console.WriteLine("Hash calculado: ");
            help.WriteHex(Resumen, Resumen.Length);
            //Liberar los recursos
            sha_provider.Dispose();


            //FASE EXTENDIDA
            //PROVEEDOR RSA PARA CIFRAR EL VALOR DEL HASH
            RSACryptoServiceProvider rsa_provider = new RSACryptoServiceProvider();
            //PRUEBAS
            //RSACng rsa_provider = new RSACng();




            //Comprobamos el tamaño de clave
            //rsa_provider.KeySize = 1024;
            Console.WriteLine("Tamaño de clave: " + rsa_provider.KeySize);
            //Declaramos AlgResumen que tendrá la cadena que define el alg usado para calcular hash
            string AlgResumen = "SHA256";
            //Objeto HashAlgorithmName usando la cadena
            HashAlgorithmName NomAlgRes = new HashAlgorithmName(AlgResumen);
            
            
            //Creamos el objeto RellenoFirma de RSASignaturePadding
            RSASignaturePadding RellenoFirma = RSASignaturePadding.Pkcs1;
            //PRUEBAS
            //RSASignaturePadding RellenoFirma = RSASignaturePadding.Pss;
            
            //Creamos el array Firma con el metodo SignHash al que le pasamos lo anterior
            byte[] Firma = rsa_provider.SignHash(Resumen, NomAlgRes, RellenoFirma);
            Console.WriteLine("Resultado Firma 1: "); help.WriteHex(Firma, Firma.Length);
            //Realizamos la sobrecarga
            byte[] Firma2 = rsa_provider.SignHash(Resumen, NomAlgRes, RellenoFirma);
            Console.WriteLine("Resultado Firma 2: "); help.WriteHex(Firma2, Firma2.Length);
            byte[] Firma3 = rsa_provider.SignHash(Resumen, NomAlgRes, RellenoFirma);
            Console.WriteLine("Resultado Firma 3: "); help.WriteHex(Firma3, Firma3.Length);

            //Usamos la sobrecarga de VerifyHash() que usa cuatro parametros
            bool VFR = rsa_provider.VerifyHash(Resumen, Firma, NomAlgRes, RellenoFirma);
            bool VFM = rsa_provider.VerifyData(Mensaje, Firma, NomAlgRes, RellenoFirma);
            Console.WriteLine("Comprobacion firma: " + VFR);
            Console.WriteLine("Comprobacion datos: " + VFM);

            rsa_provider.Dispose();
            Console.ReadKey();

            //Si se emplea un resumen SHA512 con longitud de clave de 1024
            //Genera una excepción de HASH incorrecto


        }
    }
}
