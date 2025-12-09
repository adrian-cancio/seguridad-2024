using System;
using Apoyo;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.ConstrainedExecution;
using System.Collections;
using System.Runtime.Serialization.Formatters;

namespace CifradoYDescifrado
{
    internal class Program
    {
        static void Main(string[] args)
        {



            /*  Programa que primero cifra un texto plano 
                (array de bytes) y almacena el texto cifrado(array de bytes) en un fichero.
                            
                Después lee el array de bytes del fichero y lo descifra para obtener nuevamente el texto plano */



            /* --------------------------------------- 3. Cifrado y descifrado de arrays de bytes (uno o más bloques AES) --------------------------------------- */
            Console.WriteLine("\n--- 3. Cifrado y descifrado de arrays de bytes (uno o más bloques AES) ---\n\n");



            // Declaramos objeto de la clase AesCryptoServiceProvider para cifrar y descifrar información.
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

            // Declaramos objeto ayuda para utilizar metodos de buffers y ficheros
            Ayuda ayuda = new Ayuda();

            // Tamaño a trabajar con la clave
            int TamClave = 16;

            // array de bytes Clave de tamaño TamClave
            Byte[] Clave = new Byte[TamClave];

            // Damos valor a la clave
            for (int i = 0; i < Clave.Length; i++){
                Clave[i] = (byte)(i % 256);
               }

            // vector de inicialización VI que será un array de 16 bytes
            Byte[] VI = new Byte[TamClave];

            // con los bytes 0xA0 a 0xAF.
            for (int i = 0; i < VI.Length; i++) {
                VI[i] = (byte)((i + 160) % 256);
            }

            // array de 48 bytes
            // El TextoPlano tiene exactamente el tamaño de tres bloques de información que cifra AES. En este 
            // caso los tres bloques son idénticos. Usando solo las dos primeras líneas de números, TextoPlano
            // tiene el tamaño de 1 bloque AES. Usando solo las tres primeras líneas de números, TextoPlano
            // tiene el tamaño de 1,5 bloques AES, etc.

            byte[] TextoPlano =
            {0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
             0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
             0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
             0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
             0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
             0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F};

            // Mostrar en la consola las propiedades básicas del objeto proveedor
            Console.WriteLine("BlockSize antes = " + aes.BlockSize.ToString());
            Console.WriteLine("KeySize antes = " + aes.KeySize.ToString());
            Console.WriteLine("Padding antes = " + aes.Padding.ToString());
            Console.WriteLine("Mode antes = " + aes.Mode.ToString() + "\n");

            // aes.BlockSize = 128;    Siempre 128
            aes.KeySize = 128; // Minimo 128 o maximo 256
            aes.Padding = PaddingMode.ANSIX923; // Valor boolean de 0 o 1 para el sistema de rellenado
            aes.Mode = CipherMode.ECB; // Valor boolean de 0 o 1 para el modo de cifrado de bloques

            // Mostrando los cambios
            Console.WriteLine("BlockSize despues del cambio = " + aes.BlockSize.ToString());
            Console.WriteLine("KeySize despues del cambio = " + aes.KeySize.ToString());
            Console.WriteLine("Padding despues del cambio = " + aes.Padding.ToString());
            Console.WriteLine("Mode despues del cambio = " + aes.Mode.ToString() + "\n");

            // Generar una clave simétrica aleatoria para el proveedor utilizando el método Generatekey()
            aes.GenerateKey();

            // La printeamos
            Console.WriteLine("Clave simetrica generada: ");
            ayuda.WriteHex(aes.Key, aes.Key.Length);
            Console.WriteLine();

            // Generar un vector de inicialización aleatorio para el proveedor utilizando el método GenerateIV()
            aes.GenerateIV();

            // Lo printeamos
            Console.WriteLine("Vector de inicializacion generado: ");
            ayuda.WriteHex(aes.IV, aes.IV.Length);
            Console.WriteLine();



            //   ---   Proceso de cifrado de un array de bytes   ---   //



            // Definir un flujo de datos (Stream) para almacenar el texto cifrado
            // Usar el modo Create para el fichero, pues así siempre se crea un fichero 
            // nuevo. El modo de acceso será Write y el de compartición None      
            FileStream fich = new FileStream("zz_TextoCifrado.bin", FileMode.Create, FileAccess.Write, FileShare.None);

            // Para que el objeto proveedor AES pueda cifrar información es necesario crear un objeto cifrador 
            // definido por la interfaz ICryptoTransform. Para crear el cifrador usar el método CreateEncryptor()
            // de la clase AesCryptoServiceProvider
            ICryptoTransform cryptoTransform = aes.CreateEncryptor(Clave, VI);

            // El objeto que realiza la transformación criptográfica, el cifrado en este caso, no funciona solo, 
            // sino que realiza su trabajo para un objeto de una clase Stream, concretamente, para un objeto de
            // la clase CryptoStream

            // Crear un objeto de la clase CryptoStream. El constructor utiliza tres parámetros. El primero es el 
            // Stream con el que opera el CryptoStream, en este caso el FileStream de escritura creado
            // previamente.El segundo es el objeto que realiza la transformación criptográfica de cifrado y que
            // se ha creado previamente. El tercero es el modo de trabajo del CryptoStream con el FileStream
            // definido en el primer parámetro y que puede ser Read o Write. En este caso, el modo de trabajo es
            // Write para que el CryptoStream escriba la información cifrada en el FileStream.
            CryptoStream crypto = new CryptoStream(fich, cryptoTransform, CryptoStreamMode.Write);

            // Cifrar la información llamando al método Write() del objeto CryptoStream creado previamente
            crypto.Write(TextoPlano, 0, TextoPlano.Length);

            /*  Para que los bytes se vuelquen al Stream de salida hay que llamar al método Flush(). Finalmente 
                hay que cerrar el CryptoStream llamando al método Close().Al llamar a Close() se hace
                previamente el volcado del contenido del CryptoStream.

                Después hay que liberar los recursos asignados al objeto cifrador invocando a su método 
                Dispose().No obstante, si se va a usar el mismo objeto Proveedor AES para crear un descifrador en
                la siguiente sección de la práctica, no debe invocarse el método Dispose().

                Finalmente hay que cerrar el FileStream llamando a su método Close(). */

            // Para que los bytes se vuelquen al Stream de salida --> método Flush
            // y cerrar CryptoStream llamando al método Close()

            crypto.Flush();
            crypto.Close();
            //cryptoTransform.Dispose();
            fich.Close();

            /* Comprobación de que el texto cifrado ha sido creado
             
               Verificar que aparece el fichero "zz_TextoCifrado.bin" en el directorio ..\bin\Debug\ del proyecto 
               correspondiente. Visualizar el contenido del fichero con un programa visualizador de ficheros 
               binarios como HxD
            */



            //   ---   Proceso de descifrado de un array de bytes   ---   //



            // Creamos una variable con longitud suficiente para almacenar texto a descifrar
            byte[] TextoDescifrado = new byte[TextoPlano.Length];

            // Hacemos un flujo de datos FileStream para leer el fichero con los parametros correspondientes
            FileStream fileRead = new FileStream("zz_TextoCifrado.bin", FileMode.Open, FileAccess.Read, FileShare.None);

            // Creamos el objeto descifrador
            ICryptoTransform descifrador = aes.CreateDecryptor(Clave, VI);

            // Objeto de la clase CryptoStream para cifrar
            /* El objeto que realiza la transformación criptográfica, el descifrado en este caso, no funciona
            solo, sino que realiza su trabajo para un objeto de una clase Stream, concretamente, para un
            objeto de la clase CryptoStream. Para el descifrado, la salida de un objeto FileStream (que lee la
            información cifrada en un fichero) alimenta la entrada de un objeto CryptoStream (que descifra la
            información). */
            CryptoStream csDescifrar = new CryptoStream(fileRead, descifrador, CryptoStreamMode.Read);

            // Cifrar la informacion y volcar a la salida estandar
            csDescifrar.Read(TextoDescifrado, 0, TextoDescifrado.Length);
            csDescifrar.Flush();

            // Resultado de encriptar el texto plano y desencriptarlo
            Console.WriteLine("Texto plano desencriptado:");
            ayuda.WriteHex(TextoDescifrado, TextoDescifrado.Length);

            // Cerrar y liberar memoria
            csDescifrar.Close();
            descifrador.Dispose();
            fileRead.Close();



            /* --------------------------------------- 4. Pruebas con el programa --------------------------------------- */
            Console.WriteLine("\n\n--- 4. Pruebas con el programa ---\n");



            //   ---   Encadenamiento de cifradores   ---   //



            // 1. Prueba con Modo ECB (Electronic Codebook)

            aes.Mode = CipherMode.ECB; // Configuramos el modo de cifrado en ECB (Electronic Codebook), que cifra cada bloque por separado.
            aes.Padding = PaddingMode.None;  // Desactivamos el relleno (padding) porque el tamaño del texto plano es múltiplo exacto de 16 bytes.
            Console.WriteLine("\n * Cifrado en modo ECB");


            // Cifrado en ECB

            ICryptoTransform encryptorECB = aes.CreateEncryptor(Clave, VI); // Creamos un cifrador usando la clave y el vector de inicialización (VI).
            MemoryStream msEncryptECB = new MemoryStream(); // Creamos un stream en memoria para almacenar el texto cifrado.
            CryptoStream csEncryptECB = new CryptoStream(msEncryptECB, encryptorECB, CryptoStreamMode.Write); // Creamos un CryptoStream que cifra los datos escritos en el MemoryStream.
            csEncryptECB.Write(TextoPlano, 0, TextoPlano.Length); // Escribimos el texto plano en el CryptoStream, lo que lo cifra automáticamente.
            csEncryptECB.FlushFinalBlock(); // Volcamos todos los datos pendientes al stream y finalizamos el cifrado.

            byte[] TextoCifradoECB = msEncryptECB.ToArray(); // Convertimos el contenido cifrado del MemoryStream en un array de bytes.
            Console.WriteLine("\nTexto cifrado (ECB):");
            ayuda.WriteHex(TextoCifradoECB, TextoCifradoECB.Length); // Mostramos el texto cifrado en formato hexadecimal.


            // Descifrado en ECB

            ICryptoTransform decryptorECB = aes.CreateDecryptor(Clave, VI); // Creamos un descifrador usando la clave y el vector de inicialización (aunque VI no se usa en ECB).
            MemoryStream msDecryptECB = new MemoryStream(TextoCifradoECB); // Creamos un MemoryStream con el texto cifrado.
            CryptoStream csDecryptECB = new CryptoStream(msDecryptECB, decryptorECB, CryptoStreamMode.Read); // Creamos un CryptoStream para leer y descifrar los datos del MemoryStream.

            byte[] TextoDescifradoECB = new byte[TextoPlano.Length]; // Creamos un array para almacenar el texto descifrado.
            csDecryptECB.Read(TextoDescifradoECB, 0, TextoDescifradoECB.Length); // Leemos el texto cifrado y lo desciframos en el array.

            Console.WriteLine("\nTexto descifrado (ECB):");
            ayuda.WriteHex(TextoDescifradoECB, TextoDescifradoECB.Length); // Mostramos el texto descifrado en formato hexadecimal.


            // 2. Prueba con Modo CBC (Cipher Block Chaining)

            aes.Mode = CipherMode.CBC; // Configuramos el modo de cifrado en CBC (Cipher Block Chaining), donde los bloques están encadenados.
            aes.Padding = PaddingMode.None;  // Desactivamos el relleno (padding) porque el tamaño del texto plano es múltiplo exacto de 16 bytes.
            Console.WriteLine("\n* Cifrado en modo CBC");


            // Cifrado en CBC

            ICryptoTransform encryptorCBC = aes.CreateEncryptor(Clave, VI); // Creamos un cifrador CBC usando la clave y el vector de inicialización (VI).
            MemoryStream msEncryptCBC = new MemoryStream(); // Creamos un stream en memoria para almacenar el texto cifrado.
            CryptoStream csEncryptCBC = new CryptoStream(msEncryptCBC, encryptorCBC, CryptoStreamMode.Write); // Creamos un CryptoStream que cifra los datos escritos en el MemoryStream.
            csEncryptCBC.Write(TextoPlano, 0, TextoPlano.Length); // Escribimos el texto plano en el CryptoStream, lo que lo cifra automáticamente.
            csEncryptCBC.FlushFinalBlock(); // Volcamos todos los datos pendientes al stream y finalizamos el cifrado.

            byte[] TextoCifradoCBC = msEncryptCBC.ToArray(); // Convertimos el contenido cifrado del MemoryStream en un array de bytes.
            Console.WriteLine("\nTexto cifrado (CBC):");
            ayuda.WriteHex(TextoCifradoCBC, TextoCifradoCBC.Length); // Mostramos el texto cifrado en formato hexadecimal.


            // Descifrado en CBC

            ICryptoTransform decryptorCBC = aes.CreateDecryptor(Clave, VI); // Creamos un descifrador CBC usando la clave y el vector de inicialización.
            MemoryStream msDecryptCBC = new MemoryStream(TextoCifradoCBC); // Creamos un MemoryStream con el texto cifrado.
            CryptoStream csDecryptCBC = new CryptoStream(msDecryptCBC, decryptorCBC, CryptoStreamMode.Read); // Creamos un CryptoStream para leer y descifrar los datos del MemoryStream.

            byte[] TextoDescifradoCBC = new byte[TextoPlano.Length]; // Creamos un array para almacenar el texto descifrado.
            csDecryptCBC.Read(TextoDescifradoCBC, 0, TextoDescifradoCBC.Length); // Leemos el texto cifrado y lo desciframos en el array.

            Console.WriteLine("\nTexto descifrado (CBC):");
            ayuda.WriteHex(TextoDescifradoCBC, TextoDescifradoCBC.Length); // Mostramos el texto descifrado en formato hexadecimal.



            //   ---   Modos de relleno   ---   //



            // Texto plano de 40 bytes (2,5 bloques de 16 bytes)
            byte[] TextoPlano40 =
            {
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
                0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
                0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37
            };


            // --- Prueba con PaddingMode.None ---

            aes.Padding = PaddingMode.None; // No se utiliza ningún relleno, lo que implica que el texto debe ser un múltiplo de 16 bytes.
            Console.WriteLine("\n* Prueba con PaddingMode.None");

            try
            {
                ICryptoTransform encryptorNone = aes.CreateEncryptor(Clave, VI); // Creamos el cifrador con PaddingMode.None.
                MemoryStream msEncryptNone = new MemoryStream();
                CryptoStream csEncryptNone = new CryptoStream(msEncryptNone, encryptorNone, CryptoStreamMode.Write);

                // Intentamos cifrar, pero fallará porque los 40 bytes no son múltiplos de 16.
                csEncryptNone.Write(TextoPlano40, 0, TextoPlano40.Length);
                csEncryptNone.FlushFinalBlock();

                byte[] TextoCifradoNone = msEncryptNone.ToArray();
                Console.WriteLine("Texto cifrado (None):");
                ayuda.WriteHex(TextoCifradoNone, TextoCifradoNone.Length);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("\nError: " + e.Message); // Esperamos una excepción porque el texto no es múltiplo de 16.
            }


            // --- Prueba con PaddingMode.Zeros ---

            aes.Padding = PaddingMode.Zeros; // Usamos PaddingMode.Zeros, que rellena con ceros hasta completar los 48 bytes (3 bloques de 16 bytes).
            Console.WriteLine("\n* Prueba con PaddingMode.Zeros");

            ICryptoTransform encryptorZeros = aes.CreateEncryptor(Clave, VI); // Creamos el cifrador con PaddingMode.Zeros.
            MemoryStream msEncryptZeros = new MemoryStream();
            CryptoStream csEncryptZeros = new CryptoStream(msEncryptZeros, encryptorZeros, CryptoStreamMode.Write);

            csEncryptZeros.Write(TextoPlano40, 0, TextoPlano40.Length); // Escribimos el texto plano en el CryptoStream, se añadirá el padding de ceros.
            csEncryptZeros.FlushFinalBlock();

            byte[] TextoCifradoZeros = msEncryptZeros.ToArray(); // Convertimos el texto cifrado en un array de bytes.
            Console.WriteLine("\nTexto cifrado (Zeros):");
            ayuda.WriteHex(TextoCifradoZeros, TextoCifradoZeros.Length);


            // Descifrado con PaddingMode.Zeros

            ICryptoTransform decryptorZeros = aes.CreateDecryptor(Clave, VI); // Creamos el descifrador con el mismo PaddingMode.
            MemoryStream msDecryptZeros = new MemoryStream(TextoCifradoZeros);
            CryptoStream csDecryptZeros = new CryptoStream(msDecryptZeros, decryptorZeros, CryptoStreamMode.Read);

            byte[] TextoDescifradoZeros = new byte[TextoPlano40.Length]; // Inicializamos el array del tamaño exacto del texto plano original.
            csDecryptZeros.Read(TextoDescifradoZeros, 0, TextoDescifradoZeros.Length); // Desciframos el texto.

            Console.WriteLine("\nTexto descifrado (Zeros):");
            ayuda.WriteHex(TextoDescifradoZeros, TextoDescifradoZeros.Length);


            // --- Prueba con PaddingMode.ANSIX923 ---

            aes.Padding = PaddingMode.ANSIX923; // Usamos PaddingMode.ANSIX923, que añade ceros y el número de bytes añadidos al final.
            Console.WriteLine("\n* Prueba con PaddingMode.ANSIX923");

            ICryptoTransform encryptorANSIX923 = aes.CreateEncryptor(Clave, VI);
            MemoryStream msEncryptANSIX923 = new MemoryStream();
            CryptoStream csEncryptANSIX923 = new CryptoStream(msEncryptANSIX923, encryptorANSIX923, CryptoStreamMode.Write);

            csEncryptANSIX923.Write(TextoPlano40, 0, TextoPlano40.Length); // Ciframos el texto con relleno ANSIX923.
            csEncryptANSIX923.FlushFinalBlock();

            byte[] TextoCifradoANSIX923 = msEncryptANSIX923.ToArray();
            Console.WriteLine("\nTexto cifrado (ANSIX923):");
            ayuda.WriteHex(TextoCifradoANSIX923, TextoCifradoANSIX923.Length);


            // Descifrado con PaddingMode.ANSIX923

            ICryptoTransform decryptorANSIX923 = aes.CreateDecryptor(Clave, VI);
            MemoryStream msDecryptANSIX923 = new MemoryStream(TextoCifradoANSIX923);
            CryptoStream csDecryptANSIX923 = new CryptoStream(msDecryptANSIX923, decryptorANSIX923, CryptoStreamMode.Read);

            byte[] TextoDescifradoANSIX923 = new byte[TextoPlano40.Length]; // Inicializamos el array del tamaño exacto del texto plano.
            csDecryptANSIX923.Read(TextoDescifradoANSIX923, 0, TextoDescifradoANSIX923.Length);

            Console.WriteLine("\nTexto descifrado (ANSIX923):");
            ayuda.WriteHex(TextoDescifradoANSIX923, TextoDescifradoANSIX923.Length);


            // --- Prueba con PaddingMode.ISO10126 ---

            aes.Padding = PaddingMode.ISO10126; // Usamos PaddingMode.ISO10126, que añade bytes aleatorios y el número de bytes añadidos al final.
            Console.WriteLine("\n* Prueba con PaddingMode.ISO10126");

            ICryptoTransform encryptorISO10126 = aes.CreateEncryptor(Clave, VI);
            MemoryStream msEncryptISO10126 = new MemoryStream();
            CryptoStream csEncryptISO10126 = new CryptoStream(msEncryptISO10126, encryptorISO10126, CryptoStreamMode.Write);

            csEncryptISO10126.Write(TextoPlano40, 0, TextoPlano40.Length); // Ciframos el texto con relleno ISO10126.
            csEncryptISO10126.FlushFinalBlock();

            byte[] TextoCifradoISO10126 = msEncryptISO10126.ToArray();
            Console.WriteLine("\nTexto cifrado (ISO10126):");
            ayuda.WriteHex(TextoCifradoISO10126, TextoCifradoISO10126.Length);


            // Descifrado con PaddingMode.ISO10126

            ICryptoTransform decryptorISO10126 = aes.CreateDecryptor(Clave, VI);
            MemoryStream msDecryptISO10126 = new MemoryStream(TextoCifradoISO10126);
            CryptoStream csDecryptISO10126 = new CryptoStream(msDecryptISO10126, decryptorISO10126, CryptoStreamMode.Read);

            byte[] TextoDescifradoISO10126 = new byte[TextoPlano40.Length];
            csDecryptISO10126.Read(TextoDescifradoISO10126, 0, TextoDescifradoISO10126.Length);

            Console.WriteLine("\nTexto descifrado (ISO10126):");
            ayuda.WriteHex(TextoDescifradoISO10126, TextoDescifradoISO10126.Length);


            // --- Prueba con PaddingMode.PKCS7 ---

            aes.Padding = PaddingMode.PKCS7; // Usamos PaddingMode.PKCS7, que añade el número de bytes añadidos como relleno.
            Console.WriteLine("\n* Prueba con PaddingMode.PKCS7");

            ICryptoTransform encryptorPKCS7 = aes.CreateEncryptor(Clave, VI);
            MemoryStream msEncryptPKCS7 = new MemoryStream();
            CryptoStream csEncryptPKCS7 = new CryptoStream(msEncryptPKCS7, encryptorPKCS7, CryptoStreamMode.Write);

            csEncryptPKCS7.Write(TextoPlano40, 0, TextoPlano40.Length); // Ciframos el texto con relleno PKCS7.
            csEncryptPKCS7.FlushFinalBlock();

            byte[] TextoCifradoPKCS7 = msEncryptPKCS7.ToArray();
            Console.WriteLine("\nTexto cifrado (PKCS7):");
            ayuda.WriteHex(TextoCifradoPKCS7, TextoCifradoPKCS7.Length);


            // Descifrado con PaddingMode.PKCS7

            ICryptoTransform decryptorPKCS7 = aes.CreateDecryptor(Clave, VI);
            MemoryStream msDecryptPKCS7 = new MemoryStream(TextoCifradoPKCS7);
            CryptoStream csDecryptPKCS7 = new CryptoStream(msDecryptPKCS7, decryptorPKCS7, CryptoStreamMode.Read);

            byte[] TextoDescifradoPKCS7 = new byte[TextoPlano40.Length];
            csDecryptPKCS7.Read(TextoDescifradoPKCS7, 0, TextoDescifradoPKCS7.Length);

            Console.WriteLine("\nTexto descifrado (PKCS7):");
            ayuda.WriteHex(TextoDescifradoPKCS7, TextoDescifradoPKCS7.Length);



            //   ---   Modos de relleno   ---   //



            // Tamaño de la clave (puedes probar con 128, 192 o 256 bits)
            int tamanoClave = 256; // Cambia a 128 o 192 para probar otros tamaños

            // Crear una instancia de AES manualmente
            Aes aesAlg = Aes.Create();
            aesAlg.KeySize = tamanoClave;  // Establecemos el tamaño de la clave en bits

            // Generar una nueva clave y vector de inicialización (IV)
            aesAlg.GenerateKey();
            aesAlg.GenerateIV();

            Console.WriteLine("\nClave AES generada (Tamaño {0} bits):", tamanoClave);
            Console.WriteLine(BitConverter.ToString(aesAlg.Key)); // Imprimir la clave generada

            // Texto plano a cifrar (40 bytes para pruebas)
            string textoPlano = "Este es un texto de prueba de 40 bytes!!\n"; // 40 bytes exactos
            byte[] textoPlanoBytes = System.Text.Encoding.UTF8.GetBytes(textoPlano);

            // --- Cifrado del texto plano ---
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(textoPlanoBytes, 0, textoPlanoBytes.Length);
            csEncrypt.FlushFinalBlock();

            byte[] textoCifrado = msEncrypt.ToArray();

            Console.WriteLine("\nTexto cifrado: ");
            Console.WriteLine(BitConverter.ToString(textoCifrado));

            // --- Descifrado del texto cifrado ---
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            MemoryStream msDecrypt = new MemoryStream(textoCifrado);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            byte[] textoDescifrado = new byte[textoPlanoBytes.Length];
            csDecrypt.Read(textoDescifrado, 0, textoDescifrado.Length);

            Console.WriteLine("\nTexto descifrado: ");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(textoDescifrado));

            // Cerrar y liberar recursos
            csEncrypt.Close();
            msEncrypt.Close();
            csDecrypt.Close();
            msDecrypt.Close();
            aesAlg.Clear();  // Limpiar y liberar el objeto AES



            //   ---   Cambiar el tipo de proveedor de servicios criptográficos   ---   //



            // Crear una instancia de AES utilizando AesManaged
            aesAlg = new AesManaged();  // Usamos AesManaged en lugar de AesCryptoServiceProvider

            // Generar clave y vector de inicialización (IV)
            aesAlg.GenerateKey();
            aesAlg.GenerateIV();

            Console.WriteLine("Clave AES generada (con AesManaged):");
            Console.WriteLine(BitConverter.ToString(aesAlg.Key));

            // Texto plano a cifrar (40 bytes para pruebas)
            textoPlano = "Este es un texto de prueba de 40 bytes!!";  // 40 bytes exactos
            textoPlanoBytes = System.Text.Encoding.UTF8.GetBytes(textoPlano);

            // --- Cifrado del texto plano ---
            encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            msEncrypt = new MemoryStream();
            csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(textoPlanoBytes, 0, textoPlanoBytes.Length);
            csEncrypt.FlushFinalBlock();

            textoCifrado = msEncrypt.ToArray();

            Console.WriteLine("\nTexto cifrado: ");
            Console.WriteLine(BitConverter.ToString(textoCifrado));

            // --- Descifrado del texto cifrado ---
            decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            msDecrypt = new MemoryStream(textoCifrado);
            csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            textoDescifrado = new byte[textoPlanoBytes.Length];
            csDecrypt.Read(textoDescifrado, 0, textoDescifrado.Length);

            Console.WriteLine("\nTexto descifrado: ");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(textoDescifrado));

            // Cerrar y liberar recursos
            csEncrypt.Close();
            msEncrypt.Close();
            csDecrypt.Close();
            msDecrypt.Close();
            aesAlg.Clear();  // Limpiar y liberar el objeto AesManaged



            /* --------------------------------------- 5. Cifrado y descifrado de cadenas de caracteres --------------------------------------- */
            Console.WriteLine("\n\n--- 5. Cifrado y descifrado de cadenas de caracteres ---\n\n");



            // Flujo de datos FileStream para escribir el fichero
            fich = new FileStream("zz_CharCifrado.bin", FileMode.Create, FileAccess.Write);

            // Objeto de la clase CryptoStream para cifrar
            cryptoTransform = aes.CreateEncryptor(Clave, VI);
            crypto = new CryptoStream(fich, cryptoTransform, CryptoStreamMode.Write);

            // Objeto de la clase StreanWriter para cifrar caracteres
            StreamWriter writerCifrador = new StreamWriter(crypto);

            // Cadena a cifrar
            writerCifrador.WriteLine("Hola, es una prueba de mi practica 3");

            // Cerrar y liberar recursos
            writerCifrador.Close();
            crypto.Close();
            fich.Close();

            // Flujo de datos FileStream para leer el fichero
            fileRead = new FileStream("zz_CharCifrado.bin", FileMode.Open, FileAccess.Read);

            // Objeto  de  la  clase  CryptoStream para descifrar
            descifrador = aes.CreateDecryptor(Clave, VI);
            csDescifrar = new CryptoStream(fileRead, descifrador, CryptoStreamMode.Read);

            // Objeto de la clase StreanReader para descifrar caracteres
            StreamReader reader = new StreamReader(csDescifrar);

            // Cadena a descifrar
            Console.WriteLine("Cadena de caracteres descifrada: " + reader.ReadToEnd());

            // Cerrar y liberar recursos
            reader.Close();
            csDescifrar.Close();
            fileRead.Close();



            /* --------------------------------------- 6. Cifrado y descifrado de ficheros --------------------------------------- */
            Console.WriteLine("\n--- 6. Cifrado y descifrado de ficheros ---\n\n");



            // Fichero de entrada plano en nuestro 
            FileStream fs_entrada = new FileStream("TextoPlanoACifrar.txt", FileMode.Open, FileAccess.Read, FileShare.None);

            // Flujo de datos FileStream para escribir el fichero
            fich = new FileStream("TextoCifrado.txt", FileMode.Create, FileAccess.Write, FileShare.None);

            // Objeto  de  la  clase  CryptoStream para cifrar
            cryptoTransform = aes.CreateEncryptor(Clave, VI);
            crypto = new CryptoStream(fich, cryptoTransform, CryptoStreamMode.Write);

            byte[] arrayBufferMem = new byte[fs_entrada.Length];
            int bytesSinLeer = (int)fs_entrada.Length;
            int bytesLeidos = 0;

            // Volcar el mensaje cifrado al fichero de salida
            while (bytesLeidos < bytesSinLeer)
            {
                int n = fs_entrada.Read(arrayBufferMem, bytesLeidos, bytesSinLeer);
                crypto.Write(arrayBufferMem, bytesLeidos, bytesSinLeer);
                bytesLeidos += n;
                bytesSinLeer -= n;
            }

            // Cerrar y liberar recursos
            fs_entrada.Close();
            crypto.Close();
            fich.Close();
            cryptoTransform.Dispose();

            // Flujo de datos FileStream para leer el fichero
            fileRead = new FileStream("TextoCifrado.txt", FileMode.Open, FileAccess.Read);

            // Objeto de la clase CryptoStream para descifrar
            descifrador = aes.CreateDecryptor(Clave, VI);
            csDescifrar = new CryptoStream(fileRead, descifrador, CryptoStreamMode.Read);

            // Objeto de la clase StreanReader para descifrar caracteres
            reader = new StreamReader(csDescifrar);

            // Cadena a descifrar
            Console.WriteLine("Fichero de texto descifrado: " + reader.ReadToEnd());

            // Cerrar y liberar recursos
            reader.Close();
            csDescifrar.Close();
            fileRead.Close();



            /* --------------------------------------- 7. Generar claves a partir de contrasenas --------------------------------------- */
            Console.WriteLine("\n\n--- 7. Generar claves a partir de contrasenas ---\n\n");



            Console.Write("Introduce la contrasena de cifrado de datos: ");
            string Contra = Console.ReadLine();

            // Declara el array Contra y asígnale una cadena de caracteres (la contrasena) leída del teclado con el
            // método ReadLine() de la clase Console
            byte[] Sal = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
            // (Declara el array Contra y asígnale una cadena de caracteres (la contraseña) leída del teclado con el
            //método ReadLine() de la clase Console)

            int N = 16; // longitud de la clave

            // Crea el objeto Generador de la clase Rfc2898DeriveBytes pasándole tres parámetros en el
            // constructor: Contra, Sal y el entero 1000. Este entero especifica el número de iteraciones internas
            // a realizar por el algoritmo y se puede cambiar.
            Rfc2898DeriveBytes generador = new Rfc2898DeriveBytes(Contra, Sal, 1000);

            // Para una misma contraseña, usando el mismo generador siempre se obtendría la misma Clave y vector de inicialización VI.
            // Rfc2898DeriveBytes generador2 = new Rfc2898DeriveBytes(Contra, Sal, 1000);

            // Array N bytes que se utilizarán como clave
            Clave = generador.GetBytes(N);
            Console.WriteLine("\nGeneración de clave simetrica de {0} bytes", N);
            ayuda.WriteHex(Clave, Clave.Length);

            generador.Reset();

            // Derivar los 16 bytes del vector de inicialización de la contraseña
            VI = generador.GetBytes(16);
            Console.WriteLine("\nGeneración del vector de inicialización de {0} bytes", 16);
            ayuda.WriteHex(VI, VI.Length);

            Console.WriteLine();

            generador.Dispose();
            aes.Dispose();
        }
    }
}
