using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;


namespace Ejercicio1_VERIFICA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ayuda a = new Ayuda();

            String nombre = "zzUSU1712.cer";

            X509Certificate2 cert = ExtraeCertificado(nombre);

            // El programa tiene que leer 2 o 3 argumentos de la linea de comandos:
            // Argumento 0: el nombre del fichero con la firma
            Console.WriteLine("Introduzca el nombre del fichero con la firma: ");
            string firmAsociada  = Console.ReadLine();

            //2º argumento (0 ó 1) indicando si la firma es desasociada o no
            Console.WriteLine("Firma desasociada: 0==false y 1==true: ");
            string desasociada = Console.ReadLine();

            if(!(desasociada.Equals("0") || desasociada.Equals("1")))
            {
                Console.WriteLine("Segundo parametro mal");
                System.Environment.Exit(1);
            }

            int Desasociada = int.Parse(desasociada);

            //3º argumento -> unicamente si desasociada =  1
            byte[] Mensaje = null;
            bool Desasociadas = false;
            String mensaje = "";
            if (Desasociada == 1)
            {
                Desasociadas = true;
                Console.WriteLine("Introduzca el nombre del fichero con el mensaje: ");
                mensaje = Console.ReadLine();
            }

            //Sugerimos al usuario si lo desea que modifique alguno de los ficheros
            Console.WriteLine();
            Console.WriteLine("Altera el fichero de datos o firma si quieres ...");
            Console.ReadKey();

            //CARGAMOS LOS ARGUMENTOS
            byte[] MensajeFirmado = new byte[a.BytesFichero(firmAsociada)];
            a.CargaBufer(firmAsociada, MensajeFirmado);
            if (Desasociada == 1)
            {
                Mensaje = new byte[a.BytesFichero(mensaje)];
                a.CargaBufer(mensaje, Mensaje);
            }

            //VERIFICACION DE FIRMA
            bool verificacion = false;
            verificacion = VerificaCMS(Mensaje, MensajeFirmado, Desasociadas);
            Console.WriteLine("Verificada: " + verificacion);
            Console.WriteLine();

        }

        public static X509Certificate2 ExtraeCertificado(String sujeto)
        {
            // Crear objeto almacen
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Abrir el almacen en modo lectura y que unicamente abra almacenes ya existentes
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            // Coloca a todos los certificados del almacén en un objeto de clase X509Certificate2Collection
            X509Certificate2Collection CertUser = new X509Certificate2Collection(store.Certificates);

            CertUser = CertUser.Find(X509FindType.FindBySubjectName, sujeto, false);

            return CertUser[0];
        }

        public static bool VerificaCMS(byte[] Msg, byte[] CmsFirmadoCodificado, bool Desasociada)
        {
           
            //Creo objeto CmsFirmado
            SignedCms CmsFirmado;
            //Si es desasociada
            if (Desasociada)
            {
                //Asigno este Oid (exclusivamente)
                Oid IdMsg = new Oid("1.2.840.113549.1.7.1");
                //Creo el CI
                ContentInfo CI = new ContentInfo(IdMsg, Msg);
                //Firmo el CMS
                CmsFirmado = new SignedCms(new SubjectIdentifierType(), CI, true); //Siempre debe ser TRUE
            }
            else //SINO
            {
                CmsFirmado = new SignedCms();
            }
            //VerPropSignedCms(CmsFirmado);
            //Decodifico en CmsFirmado
            CmsFirmado.Decode(CmsFirmadoCodificado);
            bool resultado = false;
            //Trato excepciones ya que el metodo CheckSignature retorna excepcion en caso de fallo
            try
            {
                Console.WriteLine("Comprobando la firma .........");
                CmsFirmado.CheckSignature(true);
                Console.WriteLine("La firma es válida");
                resultado = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                resultado = false;
            }
            return resultado;
        }
    }
}
