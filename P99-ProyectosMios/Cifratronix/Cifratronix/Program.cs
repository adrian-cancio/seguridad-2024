using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Cifratronix
{
    static void Main(string[] args)
    {
        // Variables para almacenar los parámetros
        string ficheroEntrada = "";
        string ficheroSalida = "";
        string clave = "";
        bool decrypt = false;
        int keySize = 256; // Tamaño de clave más seguro por defecto
        int blockSize = 128; // Tamaño de bloque más seguro por defecto
        int bufferSize = 4096; // Tamaño del buffer para el cifrado en bloques (4 KB por defecto)
        CipherMode cipherMode = CipherMode.CBC; // Modo de cifrado más seguro por defecto
        PaddingMode paddingMode = PaddingMode.PKCS7; // Relleno más seguro por defecto

        // Comprobación de argumentos mínimos
        if (args.Length == 0)
        {
            MostrarAyuda();
            return;
        }

        // Comportamiento simplificado: Cifratronix <ficheroEntrada> <clave>
        if (args.Length == 2)
        {
            ficheroEntrada = args[0];
            clave = args[1];

            // Comprobar si el archivo termina con ".cfx" y no se ha especificado el modo de descifrado
            if (ficheroEntrada.EndsWith(".cfx", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("El archivo parece estar cifrado. ¿Desea descifrarlo? (S/N): ");
                string respuesta = Console.ReadLine()?.Trim().ToLower();

                if (respuesta == "s")
                {
                    decrypt = true;
                }
                else
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }
            }

            // Definir el nombre del archivo de salida según la operación
            ficheroSalida = decrypt ? ficheroEntrada.Substring(0, ficheroEntrada.Length - 4) : ficheroEntrada + ".cfx";
        }
        else
        {
            // Procesar los argumentos
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-input":
                        if (i + 1 < args.Length)
                        {
                            ficheroEntrada = args[i + 1];
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Falta el valor para el parámetro -Input.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    case "-key":
                        if (i + 1 < args.Length)
                        {
                            clave = args[i + 1];
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Falta el valor para el parámetro -Key.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    case "-output":
                        if (i + 1 < args.Length)
                        {
                            ficheroSalida = args[i + 1];
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Falta el valor para el parámetro -Output.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    case "-decrypt":
                        decrypt = true;
                        break;

                    case "-keysize":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out keySize))
                        {
                            if (keySize != 128 && keySize != 192 && keySize != 256)
                            {
                                Console.WriteLine("Error: Tamaño de clave no soportado. Debe ser 128, 192 o 256.");
                                return;
                            }
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Valor inválido para el parámetro -KeySize.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    case "-blocksize":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out blockSize))
                        {
                            if (blockSize != 128 && blockSize != 192 && blockSize != 256)
                            {
                                Console.WriteLine("Error: Tamaño de bloque no soportado. Debe ser 128, 192 o 256.");
                                return;
                            }
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Valor inválido para el parámetro -BlockSize.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    case "-buffersize":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out bufferSize))
                        {
                            if (bufferSize <= 0)
                            {
                                Console.WriteLine("Error: Tamaño de búfer debe ser mayor que 0.");
                                return;
                            }
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Valor inválido para el parámetro -BufferSize.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    case "-mode":
                        if (i + 1 < args.Length)
                        {
                            string mode = args[i + 1].ToLower();
                            if (mode == "cbc") cipherMode = CipherMode.CBC;
                            else if (mode == "ecb") cipherMode = CipherMode.ECB;
                            else if (mode == "cfb") cipherMode = CipherMode.CFB;
                            else if (mode == "ofb") cipherMode = CipherMode.OFB;
                            else
                            {
                                Console.WriteLine("Error: Modo de cifrado no soportado.");
                                MostrarAyuda();
                                return;
                            }
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Falta el valor para el parámetro -Mode.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    case "-padding":
                        if (i + 1 < args.Length)
                        {
                            string padding = args[i + 1].ToLower();
                            if (padding == "pkcs7") paddingMode = PaddingMode.PKCS7;
                            else if (padding == "none") paddingMode = PaddingMode.None;
                            else if (padding == "zeros") paddingMode = PaddingMode.Zeros;
                            else if (padding == "ansix923") paddingMode = PaddingMode.ANSIX923;
                            else if (padding == "iso10126") paddingMode = PaddingMode.ISO10126;
                            else
                            {
                                Console.WriteLine("Error: Tipo de relleno no soportado.");
                                MostrarAyuda();
                                return;
                            }
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Error: Falta el valor para el parámetro -Padding.");
                            MostrarAyuda();
                            return;
                        }
                        break;

                    default:
                        Console.WriteLine($"Error: Parámetro desconocido '{args[i]}'.");
                        MostrarAyuda();
                        return;
                }
            }

            // Si no se proporcionó fichero de salida, asignar un valor por defecto
            if (string.IsNullOrEmpty(ficheroSalida))
            {
                if (decrypt)
                {
                    if (ficheroEntrada.EndsWith(".cfx", StringComparison.OrdinalIgnoreCase))
                    {
                        ficheroSalida = ficheroEntrada.Substring(0, ficheroEntrada.Length - 4);
                    }
                    else
                    {
                        Console.WriteLine("Error: Falta el valor para el parámetro -Output.");
                        MostrarAyuda();
                        return;
                    }
                }
                else
                {
                    ficheroSalida = ficheroEntrada + ".cfx";
                }
            }
        }

        // Validar que los parámetros requeridos estén presentes
        if (string.IsNullOrEmpty(ficheroEntrada) || string.IsNullOrEmpty(clave))
        {
            Console.WriteLine("Error: Los parámetros -Input y -Key son obligatorios.");
            MostrarAyuda();
            return;
        }

        // Realizar la operación de cifrado o descifrado
        try
        {
            if (decrypt)
            {
                DescifrarArchivo(ficheroEntrada, ficheroSalida, clave, keySize, cipherMode, paddingMode, bufferSize);
            }
            else
            {
                CifrarArchivo(ficheroEntrada, ficheroSalida, clave, keySize, cipherMode, paddingMode, bufferSize);
            }

            Console.WriteLine($"Operación completada. Archivo de salida: {ficheroSalida}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error durante la operación: {ex.Message}");
        }
    }

    static void MostrarAyuda()
    {
        const string ayudaProg = "Uso: Cifratronix -Input <ficheroEntrada> -Key <clave> [-Output <ficheroSalida>] [-Decrypt] [-KeySize <128|192|256>] [-BlockSize <128|192|256>] [-BufferSize <tamaño en bytes>] [-Mode <CBC|ECB|CFB|OFB>] [-Padding <PKCS7|None|Zeros|ANSIX923|ISO10126>]\n" +
                                  "También puede usar: Cifratronix <ficheroEntrada> <clave>";
        Console.WriteLine(ayudaProg);
    }

    static void CifrarArchivo(string inputFile, string outputFile, string password, int keySize, CipherMode mode, PaddingMode padding, int bufferSize)
    {
        // Generar una salt aleatoria
        byte[] salt = new byte[16]; // 128 bits
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        // Generar clave y IV usando la salt
        byte[] claveAES, ivAES;
        GenerarClaveIV(password, keySize, salt, out claveAES, out ivAES);

        using (Aes aes = Aes.Create())
        {
            aes.Key = claveAES;
            aes.IV = ivAES;
            aes.Mode = mode;
            aes.Padding = padding;

            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                // Escribir la salt en los primeros bytes del archivo cifrado
                fsOutput.Write(salt, 0, salt.Length);

                using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[bufferSize];
                        int read;
                        while ((read = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }
    }

    static void DescifrarArchivo(string inputFile, string outputFile, string password, int keySize, CipherMode mode, PaddingMode padding, int bufferSize)
    {
        using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
        {
            // Leer la salt de los primeros bytes del archivo cifrado
            byte[] salt = new byte[16];
            fsInput.Read(salt, 0, salt.Length);

            // Generar clave y IV usando la salt leída
            byte[] claveAES, ivAES;
            GenerarClaveIV(password, keySize, salt, out claveAES, out ivAES);

            using (Aes aes = Aes.Create())
            {
                aes.Key = claveAES;
                aes.IV = ivAES;
                aes.Mode = mode;
                aes.Padding = padding;

                using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[bufferSize];
                        int read;
                        while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOutput.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }
    }

    static void GenerarClaveIV(string password, int keySize, byte[] salt, out byte[] claveAES, out byte[] ivAES)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 1000))
        {
            claveAES = deriveBytes.GetBytes(keySize / 8);
            ivAES = deriveBytes.GetBytes(16);
        }
    }
}
