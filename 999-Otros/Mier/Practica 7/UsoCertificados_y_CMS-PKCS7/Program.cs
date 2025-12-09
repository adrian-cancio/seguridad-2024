using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace UsoCertificados_y_CMS_PKCS7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // booleano que almacena si la firma esta desasociada o no

            /*   Si Desasociada == true -> El contenido del mensaje NO se integra en el fichero de la firma.
                 Si Desasociada == false -> El contenido del mensaje SI se integra en el fichero de la firma.

                Desasociar el mensaje de la firma es conveniente si se quiere evitar el duplicado de la información,
                sobre todo con documentos grandes. Otra utilidad en S/MIME (correo seguro) es que si la
                información va separada de la firma, siempre se puede ver la información aunque no se soporte la
                verificación de firmas.
            */ 
            bool desasociada = true;

            // Creacion del objeto ayuda
            Ayuda a = new Ayuda();

            // Creacion del Msg
            byte[] Msg = new byte[64];
            for (int i = 0; i < Msg.Length; i++)
            {
                Msg[i] = Convert.ToByte(i);
            }

            // 2. Obtener le certificado
            // Extraer el certificado
            string nombre = "zpusu";
            Console.WriteLine("\nObtencion del certificado...\n");
            Console.WriteLine("Se procede a extraer el certificado zpusu.as...");
            X509Certificate2 Certificado = ExtraerCertificado(nombre);

            // 3. Firmar con la clave publica
            // Invocar metodo Firma
            byte[] MsgCmsFirmadoCod = FirmaCMS(Msg, Certificado, desasociada);

            // Guardar Msg en fichero "Fichero.dat". 
            a.GuardaBufer("Fichero.dat", Msg);
            // Guadar MsgCmsFirmadoCod en el fichero "Fichero.p7b"
            a.GuardaBufer("Fichero.p7b", MsgCmsFirmadoCod);

            // 4. Verificar con la clave publica
            // Esto permite parar la ejecución del programa para alterar los ficheros con HxD, por ejemplo.
            Console.WriteLine("\n\nAltera el fichero de datos o firma si quieres ..."); 
            Console.WriteLine("Pulsa Enter para continuar cuando hayas utilizado HxD...");
            Console.ReadKey();

            // Cargar la firma
            byte[] MsgCmsFirmadoCod2 = new byte[MsgCmsFirmadoCod.Length];
            a.CargaBufer("Fichero.p7b", MsgCmsFirmadoCod2);

            // Cargar el mensaje en la variable textoplano2 solo si la firma es desasociada
            byte[] textoplano2 = null;
            if (desasociada)
            {
                textoplano2 = new byte[Msg.Length];
                a.CargaBufer("Fichero.dat", textoplano2);
            }

            // Creacion de verificacion y asignacion del metodo VerificaCMS
            bool verificacion = VerificaCMS(textoplano2, MsgCmsFirmadoCod2, desasociada);

            // Mostrar por pantalla
            Console.WriteLine("Firma verificada: " + verificacion);

            // 5. Cifrar (Usando clave publica)
            // Invocar al metodo CifraCMS()
            byte[] MsgCmsCifradoCod = CifraCMS(Msg, Certificado);

            // Guardarlo en el fichero
            a.GuardaBufer("Fcifrado.dat", MsgCmsCifradoCod);

            // Mostrar por consola el mensaje cifrado
            Console.WriteLine("\n[Mensaje cifrado]: ");
            a.WriteHex(MsgCmsCifradoCod, MsgCmsCifradoCod.Length);

            // 6. Descifrar (Usando clave privada)
            // Cargar en MsgCmsCifradoCod2 el mensaje cifrado desde el fichero que lo contiene
            byte[] MsgCmsCifradoCod2 = new byte[a.BytesFichero("Fcifrado.dat")];
            a.CargaBufer("Fcifrado.dat", MsgCmsCifradoCod2);

            // Mostar por consola el mensaje descifrado
            byte[] MsgDescifrado = DescifraCMS(MsgCmsCifradoCod2);
            Console.WriteLine("\n[Mensaje descifrado]: ");
            a.WriteHex(MsgDescifrado, MsgDescifrado.Length);

            // 7. Anidacion de operaciones
            // La firma no debe ser desasociada
            desasociada = false;

            // -- EMISOR
            // Creacion del array de bytes, asignando FirmaCMS
            byte[] MsgCmsFirmadoCod3 = FirmaCMS(Msg, Certificado, desasociada);

            // Creacion del array de bytes, asignando CifraCMS
            byte[] MsgCmsCifradoCod3 = CifraCMS(MsgCmsFirmadoCod3, Certificado);

            // -- RECEPTOR
            // Creacion del array de bytes, asignando DescifraCMS
            byte[] MsgCmsDescifrado3 = DescifraCMS(MsgCmsCifradoCod3);

            // Creacion del array de bytes, asignando VerificaCMS
            bool verificacion2 = VerificaCMS(Msg, MsgCmsDescifrado3, desasociada);

            // Resultado de la validacion con operaciones anidadas
            Console.WriteLine("[Firma verificada con operaciones anidadas]: " + verificacion2);
            Console.WriteLine();
        }


        //////////////////////////////////////////////////////////////////// METODOS ////////////////////////////////////////////////////////////////////



        static X509Certificate2 ExtraerCertificado(String sujeto)
        {
            // Obtener el almacen de certificados
            X509Store almacenUser = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Abrimos almacen en modo lectura y solo almacenes existentes
            almacenUser.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            // Extraemos todos los certificados del almacen del usuario
            X509Certificate2Collection CertsUser = almacenUser.Certificates;

            // Asignamos a cert el certificado solicitado (true solo para certificados válidos)
            X509Certificate2 cert = CertsUser.Find(X509FindType.FindBySubjectName, sujeto, false)[0];

            return cert;
        }

        static public byte[] FirmaCMS(Byte[] Msg, X509Certificate2 CertFirma, bool Desasociada)
        {
            // Creamos un nuevo objeto ayuda
            Ayuda a = new Ayuda();

            // Crearcion de un identificador del mensaje
            // Valores posibles:
            // 1.2.840.113549.1.7.1 data
            // 1.2.840.113549.1.7.2 signedData
            // 1.2.840.113549.1.7.3 envelopedData
            // 1.2.840.113549.1.7.4 signedAndEnvelopedData 
            // 1.2.840.113549.1.7.5 hashedData
            // 1.2.840.113549.1.7.5 digestedData
            // 1.2.840.113549.1.7.6 encryptedData
            Oid idMsg = new Oid("1.2.840.113549.1.7.1");

            // Creacion del CI
            ContentInfo CI = new ContentInfo(idMsg, Msg);

            // Mostrar informacion por pantalla
            Console.WriteLine("\nPropiedades del objeto CI contentType: " + CI.ContentType);
            Console.WriteLine("\nContenido del objeto CI: ");
            a.WriteHex(CI.Content, CI.Content.Length);

            // Creacion del objeto CmsFirmado
            // Si Desasociada == true->  El contenido del mensaje NO se integra en el fichero de la firma. 
            // Si Desasociada == false-> El contenido del mensaje SI se integra en el fichero de la firma. 
            SignedCms CmsFirmado = new SignedCms(SubjectIdentifierType.SubjectKeyIdentifier, CI, Desasociada);

            // Creacion del objeto FirmanteCMS
            CmsSigner FirmanteCMS = new CmsSigner(CertFirma);

            // Mostrar informacion por pantalla
            Console.Write("\nFirmanteCMS CMS: ");
            Console.Write(FirmanteCMS.Certificate.Subject);

            // Generar la firma
            CmsFirmado.ComputeSignature(FirmanteCMS);

            // Codificar objeto en una cadena de bytes
            byte[] response = CmsFirmado.Encode();

            // Retornar el objeto
            return response;
        }

        static public bool VerificaCMS(byte[] Msg, byte[] CmsFirmadoCodificado, bool Desasociada)
        {
            // Declaracion del objeto CmsFirmado
            SignedCms CmsFirmado = null;

            // Si es desasociada
            if (Desasociada)
            {
                // Creacion del IdMsg
                Oid IdMsg = new Oid("1.2.840.113549.1.7.1");

                // Creacion del objeto CI
                ContentInfo ci = new ContentInfo(IdMsg, Msg);

                // Creacion del objeto CmsFirmado
                CmsFirmado = new SignedCms(SubjectIdentifierType.SubjectKeyIdentifier, ci, Desasociada);
            }

            // Si no es desasociada
            else
            {
                // Creacion del objeto CmsFirmado usando el constructor sin parametros
                CmsFirmado = new SignedCms();
            }

            // Introduccion de informacion de firma
            CmsFirmado.Decode(CmsFirmadoCodificado);

            // Declaracion de la variable de resultado de verificacion
            bool Resultado;

            // Intento de verificacion: Funciona si es valida, o genera una excepcion
            try
            {
                // Mostrar por pantalla
                Console.WriteLine("\nComprobando firma...");

                // Comprobar si la firma es valida
                CmsFirmado.CheckSignature(true);

                // Si no se genera la excepcion, se muestra por pantalla
                Console.WriteLine("La firma es valida\n");

                // Asigna el valor a la variable resultado
                Resultado = true;
            }
            // Si salta la excepcion -> La firma no es valida
            catch (Exception e)
            {
                // Se muestra por pantalla
                Console.WriteLine("Exception al comprobar la firma: " + e);

                // Asigna el valor a la variable resultado
                Resultado = false;
            }
            // Retornamos el valor de la verificacion
            return Resultado;
        }

        static public byte[] CifraCMS(byte[] Msg, X509Certificate2 CertReceptor)
        {
            // Creacion del IdMsg
            Oid IdMsg = new Oid("1.2.840.113549.1.7.1");

            // Creacion del objeto CI
            ContentInfo CI = new ContentInfo(IdMsg, Msg);

            // Creacion de CmsCifrado con el CI
            EnvelopedCms CmsCifrado = new EnvelopedCms(CI);

            // Creacion del objeto de la clase CmsRecipient
            CmsRecipient ReceptorCms = new CmsRecipient(SubjectIdentifierType.SubjectKeyIdentifier, CertReceptor);

            // Cifrar el contenido de ReceptorCms
            CmsCifrado.Encrypt(ReceptorCms);

            // Mostrar por pantalla
            Console.WriteLine("\n[Algoritmo de cifrado]: " + CmsCifrado.ContentEncryptionAlgorithm.Oid.Value);

            // Codificacion del objeto
            byte[] retorno = CmsCifrado.Encode();

            // Retornarlo
            return retorno;
        }

        static public byte[] DescifraCMS(byte[] CmsCifradoCodificado)
        {
            // Crear el objeto CmsCifrado
            EnvelopedCms CmsCifrado = new EnvelopedCms();

            // Inicializar el objeto CmsCifrado
            CmsCifrado.Decode(CmsCifradoCodificado);

            // Creacion de una coleccion de receptores
            RecipientInfoCollection ReceptorCole = CmsCifrado.RecipientInfos;

            // Creacion del Recipiente
            RecipientInfo Receptor = null;

            // Comprobación de que el numero de receptores del mensaje es solo 1
            if (ReceptorCole.Count == 1)
            {
                // Extraemos el unico elemento receptor del mensaje
                Receptor = ReceptorCole[0];

                // Mostrar informacion
                Console.WriteLine("\n[Valor del receptor]: " + Receptor.RecipientIdentifier.Value);
                Console.WriteLine("\n[Tipo del receptor]: " + Receptor.RecipientIdentifier.Type);

                // Desciframos el mensaje del objeto CmsCifrado
                CmsCifrado.Decrypt(Receptor);

                // Retornar el valor
                return CmsCifrado.ContentInfo.Content;
            }
            // Esto significa que habia mas de uno
            Console.WriteLine("Había más de un RecipientInfo");

            // Retorna null
            return null;
        }
    }
}