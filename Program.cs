using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Firewall
{
    class Program
    {
        private static byte[] DeriveKey(string passphrase, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(passphrase, salt, 16384, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32);
            }
        }

        public static string EncryptPassword(string plainPassword, string passphrase)
        {
            byte[] salt = new byte[16];
            byte[] nonce = new byte[12];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
                rng.GetBytes(nonce);
            }

            byte[] key = DeriveKey(passphrase, salt);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainPassword);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[16];

            using (AesGcm aes = new AesGcm(key))
            {
                aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            }

            byte[] encryptedData = new byte[nonce.Length + salt.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, encryptedData, 0, nonce.Length);
            Buffer.BlockCopy(salt, 0, encryptedData, nonce.Length, salt.Length);
            Buffer.BlockCopy(tag, 0, encryptedData, nonce.Length + salt.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, encryptedData, nonce.Length + salt.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(encryptedData);
        }


        public static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password.ToString();
        }

        public static string DecryptPassword(string encryptedPassword, string passphrase)
        {
            try
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedPassword);

                byte[] nonce = new byte[12];
                byte[] salt = new byte[16];
                byte[] tag = new byte[16];
                byte[] ciphertext = new byte[encryptedData.Length - 44];

                Buffer.BlockCopy(encryptedData, 0, nonce, 0, 12);
                Buffer.BlockCopy(encryptedData, 12, salt, 0, 16);
                Buffer.BlockCopy(encryptedData, 28, tag, 0, 16);
                Buffer.BlockCopy(encryptedData, 44, ciphertext, 0, ciphertext.Length);

                byte[] key = DeriveKey(passphrase, salt);

                using (AesGcm aes = new AesGcm(key))
                {
                    byte[] decryptedPassword = new byte[ciphertext.Length];
                    aes.Decrypt(nonce, ciphertext, tag, decryptedPassword);

                    return Encoding.UTF8.GetString(decryptedPassword);
                }
            }
            catch (CryptographicException)
            {
                return "Error: Invalid passphrase or corrupted encrypted data.";
            }
            catch (FormatException)
            {
                return "Error: Encrypted password format is invalid.";
            }
        }
        private static Dictionary<string, Regex> RegexPattern = new Dictionary<string, Regex>
        {
            { "find_help", new Regex(@"--help", RegexOptions.Compiled) },
            { "exit", new Regex("exit", RegexOptions.Compiled)},
            { "encrypt", new Regex(@"enc -f (?<frase>[^\s]+) -p (?<password>[^\s]+)", RegexOptions.Compiled) },
            { "decrypt", new Regex(@"dec -f (?<frase>[^\s]+) -p (?<password>[^\s]+)", RegexOptions.Compiled) }
        };

        // Method to read and match command input using regex
        private static string ReadCommandInputFromUser(string input)
        {
            foreach (var pattern in RegexPattern)
            {
                Match match = pattern.Value.Match(input);
                if (match.Success)
                {
                    if (pattern.Key == "find_help")
                    {
                        string listCommand= "enc -f <passphrase> -p <your_password> => for Decrypt\ndec -f <passphrase> -p <your_password> => for Encrypt\n exit => exit the console\n";
                        return listCommand;
                    }

                    if (pattern.Key == "encrypt")
                    {
                        string frase = match.Groups["frase"].Value;
                        string password = match.Groups["password"].Value;
                        
                        return EncryptPassword(password,frase)+"\n";
                    }

                    if (pattern.Key == "decrypt")
                    {
                        string frase = match.Groups["frase"].Value;
                        string password = match.Groups["password"].Value;
                        return DecryptPassword(password,frase)+"\n";
                    }

                    else if (pattern.Key == "exit")
                    {
                        return "exit"; // Special case for exit command
                    }
                }
            }
            return "Invalid command format.\n run --help for list commands\n";
        }

        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Write("f_enc_dec> ");
                string commandInput = Console.ReadLine();

                // Process the command using the regex patterns
                string result = ReadCommandInputFromUser(commandInput);

                if (result == "exit")
                {
                    Console.WriteLine("Exiting the program...");
                    break; // Exit the loop and terminate the program
                }

                Console.WriteLine(result);
            }
        }
    }
}
