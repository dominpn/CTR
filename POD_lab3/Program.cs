using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
/* Tryb CTR
 * 1. Implementacja wybranego trybu
 * 2. Porównanie szybkości szyfr/deszyfr w porównaniu do ECB - pliki o różnych rozmiarach
 * 3. Propagacja błedów.*/
namespace POD_lab3
{
    class Program
    {
        static string szyfrowanie_ECB(AesCryptoServiceProvider szyfr, string tekst)
        {
            byte [] oryginalny_tekst = System.Text.ASCIIEncoding.ASCII.GetBytes(tekst);
            szyfr.Mode = CipherMode.ECB;
            ICryptoTransform dekryptor = szyfr.CreateEncryptor(szyfr.Key, szyfr.IV);
            byte[] zaszyfrowany_tekst = dekryptor.TransformFinalBlock(oryginalny_tekst, 0, oryginalny_tekst.Length);
            dekryptor.Dispose();
            return Convert.ToBase64String(zaszyfrowany_tekst);
                  
           
         }
        static string xor_string(string oryg_tekst, string szyfr)
        {
            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < oryg_tekst.Length; i++)
            {
                temp.Append((char)(szyfr[i] ^ oryg_tekst[i]));
            }
            string wynik = temp.ToString();
            return wynik;
        }
        static string CTR(AesCryptoServiceProvider szyfr, string tekst)
        {
            string wynik = "";
            string temp;
            int nonce;
            string wynik_szyfrowania;
            for (int i = 0; i <= tekst.Length/24; i++)
            {
                nonce = BitConverter.ToInt32(szyfr.IV, 0) + i;
                wynik_szyfrowania = szyfrowanie_ECB(szyfr, nonce.ToString());
                if ((tekst.Length - (0 + i * 24)) >= 24)
                {
                    temp = tekst.Substring(0 + (i * 24), 24);
                    wynik = wynik + xor_string(temp, wynik_szyfrowania);
                }
                else
                {
                    temp = tekst.Substring(0 + i * 24, tekst.Length - i * 24);
                    wynik = wynik + xor_string(temp , wynik_szyfrowania);
                }
            }      
            return wynik;
        }
       
        static string deszyfrowanie_ECB(AesCryptoServiceProvider szyfr, string tekst)
        {
            byte[] zaszyfrowany_tekst = Convert.FromBase64String(tekst);
            szyfr.Mode = CipherMode.ECB;
            ICryptoTransform dekryptor = szyfr.CreateDecryptor(szyfr.Key, szyfr.IV);
            byte[] odszyfrowany_tekst = dekryptor.TransformFinalBlock(zaszyfrowany_tekst, 0, zaszyfrowany_tekst.Length);
            dekryptor.Dispose();
            return System.Text.ASCIIEncoding.ASCII.GetString(odszyfrowany_tekst);
            

        }
       
        static void Main(string[] args)
        {
            AesCryptoServiceProvider szyfr = new AesCryptoServiceProvider();
            szyfr.GenerateKey();
            szyfr.GenerateIV();
            string tekst = "Troche tekstu do zaszyfrowania";
            string zaszyfrowany_tekst = szyfrowanie_ECB(szyfr, tekst);
            string odszyfrowany_tekst = deszyfrowanie_ECB(szyfr, zaszyfrowany_tekst);
            Console.WriteLine("Zaszyfrowany tekst " + zaszyfrowany_tekst);
            Console.WriteLine("Odszyfrowany_tekst " + odszyfrowany_tekst);
            string zaszyfrowany_CTR = CTR(szyfr, tekst);
            Console.Write("Zaszyfrowany tekst za pomoca CTR ");
            Console.WriteLine(zaszyfrowany_CTR);
            Console.WriteLine("Odszyfrowany tekst " + CTR(szyfr, zaszyfrowany_CTR));
            Console.ReadKey();
            
        }
    }
}
