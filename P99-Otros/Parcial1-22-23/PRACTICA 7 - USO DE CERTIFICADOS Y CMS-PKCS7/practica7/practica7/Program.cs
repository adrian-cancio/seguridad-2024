using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Apoyo;

namespace practica7ObtenerCertificados
{
    class Program
    {
        static void Main(string[] args)
        {
            Ayuda help = new Ayuda();
            //Declaramos el array mensaje de bytes con longitud 64 bytes
            byte[] mensaje = new byte[64];
            for(int i = 0; i < mensaje.Length; i++)
            {
                mensaje[i] = (byte) ((i+160) % 256);
            }

            //Hacemos la llamada al metodo estaico ExtraeCertificado() para obtener un certificado
            //Del almacen de certs del usuario
            string NombreSujetoCer = " DIAZ RUENEZ JOSE CARLOS";
            //Llamamos a ExtraeCertificado
            X509Certificate2 Certificado = ExtraeCertificado(NombreSujetoCer);
            Console.WriteLine();
            bool Desasociada = true;
            //Llamamos a FirmaCMS y lo guardamos en un array
            byte[] MsgCmsFirmadoCod = FirmaCMS(mensaje, Certificado, Desasociada);
            //Guardamos el mensaje y el mensaje firmado com cms por separado
            help.GuardaBufer("Fichero.dat", mensaje);
            help.GuardaBufer("Fichero.p7b", MsgCmsFirmadoCod);


            //PARTE 4
            //Console.WriteLine("Altera el fichero de datos o firma si quieres ...");
            //Console.ReadKey();
            //Obtenemos el tamaño del fichero y lo usamos para guardar en un nuevo array tal fichero
            long tamDecod = help.BytesFichero("Fichero.p7b");
            byte[] MsgCmsFirmadoCod2 = new byte[tamDecod];
            help.CargaBufer("Fichero.p7b", MsgCmsFirmadoCod2);
            byte[] Msg2 = null;
            //En caso de que la firma sea desasociada
            if(Desasociada == true)
            {
                Msg2 = new byte[help.BytesFichero("Fichero.dat")];
                help.CargaBufer("Fichero.dat", Msg2);
            }
            //Realizamos la verificaciónCMS
            bool Verificacion = VerificaCMS(Msg2, MsgCmsFirmadoCod2, Desasociada);
            Console.WriteLine("Valor Verificacion: " + Verificacion);

            //Ciframos usando clave publica
            if(Certificado != null)
            {
                byte[] MsgCmdCifradoCod = CifraCMS(mensaje, Certificado);
                help.GuardaBufer("Fcifrado.dat", MsgCmdCifradoCod);
                help.WriteHex(MsgCmdCifradoCod, MsgCmdCifradoCod.Length);

                //Desciframos el mensaje - PARTE 6
                byte[] MsgCmdCifradoCod2 = new byte[help.BytesFichero("Fcifrado.dat")];
                help.CargaBufer("Fcifrado.dat", MsgCmdCifradoCod2);
                byte[] MsgDescifrado = DescifraCMS(MsgCmdCifradoCod2);
                Console.WriteLine("Mensaje descifrado: ");
                help.WriteHex(MsgDescifrado, MsgDescifrado.Length);
            }
            else
            {
                Console.WriteLine("El certificado era nulo");
            }

        }

        //creamos el metodo estatico extracertificado()
        static X509Certificate2 ExtraeCertificado(String Sujeto)
        {
            //Obtenemos el almacen de certificados
            X509Store AlmacenUsuario = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            AlmacenUsuario.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
            X509Certificate2Collection ColeCert = AlmacenUsuario.Certificates;
            Console.WriteLine("NumCertificados con subject" + Sujeto + ":" + 
                ColeCert.Find(X509FindType.FindBySubjectName, Sujeto, true).Count);

            X509Certificate2 certificadoRetornado =
                ColeCert.Find(X509FindType.FindBySubjectName, Sujeto, true)[0];
            Console.WriteLine("Depuracion: ");
            Console.WriteLine(certificadoRetornado.SerialNumber);
            return certificadoRetornado;
        }

        //FIRMAR CON CLAVE PRIVADA
        static public byte[] FirmaCMS(byte[] msg, X509Certificate2 CertFirma, bool Desasociada)
        {
            //Declaramos el identificador del mensaje
            Ayuda help = new Ayuda();
            Oid IdMsg = new Oid("1.2.840.113549.1.7.1");
            // 1.2.840.113549.1.7.1 data
            // 1.2.840.113549.1.7.5 digestedData
            // 1.2.840.113549.1.7.6 encryptedData
            // 1.2.840.113549.1.7.3 envelopedData
            // 1.2.840.113549.1.7.5 hashedData
            // 1.2.840.113549.1.7.4 signedAndEnvelopedData
            // 1.2.840.113549.1.7.2 signedData
            //Objeto de ContentInfo que representa los datos a firmar, datos a firmar & OID
            ContentInfo CI = new ContentInfo(IdMsg, msg);
            //Comprobacion, escribimos dos propiedades
            Console.WriteLine("Propiedades de CI: ");
            Console.WriteLine(CI.ContentType); help.WriteHex(CI.Content, CI.Content.Length);

            //El tercer parametro indica si es True, que el contenido del msg NO se integra en el fichero de firma
            //Si es false -> SI se integra
            SignedCms CmsFirmado = new SignedCms(SubjectIdentifierType.SubjectKeyIdentifier, CI, Desasociada);
            //Le pasamos en el constructor el certificado que queremos que firme
            CmsSigner FirmanteCMS = new CmsSigner(CertFirma);
            Console.WriteLine(FirmanteCMS.Certificate.Subject);
            //Generamos la firma que queda integrada en el objeto cmsfirmado
            CmsFirmado.ComputeSignature(FirmanteCMS);
            //Codificamos el objeto en una cadena de bytes y retornamos
            byte[] retorno = CmsFirmado.Encode();
            return retorno;
        }

        //VERIFICAR CLAVE PUBLICA
        static bool VerificaCMS(byte[] Msg, byte[] CmsFirmadoCodificado, bool Desasociada)
        {

            SignedCms CmsFirmado = null;
            if (Desasociada == true)
            {
                Oid IdMsg = new Oid("1.2.840.113549.1.7.1");
                ContentInfo CI = new ContentInfo(IdMsg, Msg);
                CmsFirmado = new SignedCms(SubjectIdentifierType.SubjectKeyIdentifier, CI, Desasociada);
            }
            else
            {
                CmsFirmado = new SignedCms();
            }
            CmsFirmado.Decode(CmsFirmadoCodificado);
            bool Resultado;
            //Una vez que el objeto CmsFirmado ha sido inicializado correctamente, podemos verificar
            //La firma que contiene
            try
            {
                Console.WriteLine("Comprobando firma...");
                //Comprobamos la firma que contiene, pero únicamente, si la firma no es correcta
                //Se genera una excepcion que es capturada abajo, ya que como tal el método no devuelve un valor
                //Sino una excepcion
                CmsFirmado.CheckSignature(true);
                Console.WriteLine("La firma es valida");
                Resultado = true;
            }
            catch (Exception e)
            {
                //Esto significa que la firma no es correcta
                Console.WriteLine("Exception al comprobar la firma: " + e);
                Resultado = false;
            }
            //Retornamos el valor de la validacion
            return Resultado;
        }

        //PARTE 5 - CIFRAR
        static public byte[] CifraCMS(byte[] Msg, X509Certificate2 CertReceptor)
        {
            Oid IdMsg = new Oid("1.2.840.113549.1.7.1");
            ContentInfo CI = new ContentInfo(IdMsg, Msg);
            //Como unico parametro le pasamos el CI creado en la linea anterior
            EnvelopedCms CmsCifrado = new EnvelopedCms(CI);
            //Mostramos algunas propiedades
            Console.WriteLine("Algoritmo usado para cifrar: " + CmsCifrado.ContentEncryptionAlgorithm.Oid);
            Console.WriteLine("Longitud clave algoritmo usado para cifrar: " + CmsCifrado.ContentEncryptionAlgorithm.KeyLength);

            //Declaramos el objeto de tipo CmsRecipient, como primer parametro el identificador y como segundo
            //El certificado pasado como parametro
            CmsRecipient ReceptorCms = new CmsRecipient(SubjectIdentifierType.SubjectKeyIdentifier, CertReceptor);
            //Lo ciframos y lo pasamos a un nuevo array codificado, lo devolvemos
            CmsCifrado.Encrypt(ReceptorCms);
            byte[] retorno = CmsCifrado.Encode();
            return retorno;
        }

        //PARTE 6 - DESCIFRAR
        static public byte[] DescifraCMS(byte[] CmsCifradoCodificado)
        {
            //Creamos el objeto de tipo EnvelopedCMS y lo usamos para decodificar el array de byte
            //que le pasamos como parametro, que esta cifrado
            EnvelopedCms CmsCifrado = new EnvelopedCms();
            CmsCifrado.Decode(CmsCifradoCodificado);
            //Comprobación de que el numero de receptores del mensaje es solo 1.
            //Declaramos el objeto RecipientInfoCollection y lo usamos para saberlo.
            //Si es uno, extrae el unico elemento, si no, no
            RecipientInfoCollection ReceptorCole = CmsCifrado.RecipientInfos;
            if (ReceptorCole.Count == 1)
            {
                RecipientInfo Receptor = null;
                Receptor = ReceptorCole[0];
                Console.WriteLine("Valor y tipo de receptor: " +
                Receptor.RecipientIdentifier.Type + " - " + Receptor.RecipientIdentifier.Value);
                //Desciframos el mensaje del objeto CmsCifrado. ADmite varias sobrecargas, en este caso
                //le pasamos el Receptor obtenido de CmsCifrado
                CmsCifrado.Decrypt(Receptor);
                return CmsCifrado.ContentInfo.Content;
            }
            //Esto significa que habia mas de uno
            Console.WriteLine("Había más de un RecipientInfo"); return null;
        }
    }



}
