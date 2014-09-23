using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CRO
{
    class Radio
    {

        public static int i = 0;
        public static int doba;
        public static int cyklu;
        public static string[] when;
        public static Thread[] stahovac;
        public static string jmeno;
        public static string koncovka;
        public static string nuly;
        public static Uri stream1;
        public static Uri stream2;
        public static string proident;
        public static string path;


        /// <summary>
        ///Metoda startující a kontrolující průběh stahování. 
        /// </summary>
       public static void Start()
        {
          
                   
            DateTime[] kdy = new DateTime[when.Length];
            DateTime[] kdy_konec = new DateTime[when.Length];
            int w = 0;

            foreach (string s in when)
            {
                kdy[w] = Convert.ToDateTime(when[w]);
                w++;

            }
            Array.Sort(kdy);
            w = 0;


            foreach (DateTime dt in kdy)
            {


                kdy_konec[w] = kdy[w].AddMinutes(Convert.ToDouble(doba));

                if (kdy_konec[w] < DateTime.Now)
                {
                    i++;
                }
                if (i >= cyklu)
                {
                    System.Environment.Exit(1);
                }

                w++;

            }

            w = i;

            bool stahuje = false;
            for (int d = 0; d < cyklu; d++)
            {
                Thread pomoc = new Thread(Stahuj);
                stahovac[d] = pomoc;
            }

            while (1 == 1)
            {
                DateTime now = DateTime.Now;
                if (stahuje == true)
                {
                    Form1.nyni = now - kdy[w];
                    Form1.doba = kdy_konec[w] - kdy[w];
                    
                    Form1.stahuje = true;                    
                }

                else
                {
                    Form1.nyni = kdy[w] - now;
                    Form1.stahuje = false;
                }

                Form1.cyklu = cyklu;
                Form1.i = w + 1;
                Form1.ubehlo = now - kdy[0];
                Form1.celkem_doba = kdy_konec[cyklu - 1] - kdy[0];



                if (kdy[w] <= now && kdy_konec[w] >= now && stahuje == false)
                {
                   
                    stahuje = true;
                    proident = now.ToString("ddMMyyHHmmss");
                    stahovac[i].Start();
                    Thread.Sleep(500);
                    i++;


                }

                if (kdy_konec[w] <= now && stahuje == true && i < cyklu)
                {
                    stahuje = false;
                    Uloz(i);
                    w++;

                }


                if (kdy_konec[w] <= now && stahuje == true && i >= cyklu)
                {

                    Uloz(i);
                    Thread.Sleep(1000);
                    System.Environment.Exit(1);


                }

                else
                {


                    Thread.Sleep(500);
                }

            }

        }
        /// <summary>
        /// Metoda likvidující nepovelné znaky ze jména souboru.
        /// </summary>
        /// <param name="s">Jméno souboru</param>
        /// <returns></returns>
        public static string Odfiltruj(string s)
        {

            s = s.Replace(":", " - ");
            s = Regex.Replace(s, "[\\<>|*:\"?]", "");

            return s;

        }
        /// <summary>
        /// Metoda určující počet nul nutných před číslování souborů (správné řazení).
        /// </summary>
        /// <returns>Formátovací string s počtem nul.</returns>
        public static string kolik_nul()
        {
            int a = cyklu.ToString().Length;

            string nul = "0";

            for (int b = 1; b < a; b++)
            {
                nul += "0";

            }
            return nul;
        }
        /// <summary>
        /// Metoda ukládající stažený stream.
        /// </summary>
        /// <param name="i">pořadí stahovaného souboru.</param>
        static void Uloz(int i)
        {
            try
            {
                stahovac[i - 1].Suspend();
            }
            catch
            {}

            string a = String.Format("{0:" + nuly + "}", i);
            try
            {
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "stream" + proident + ".dwl", path +"\\"+ a + ". " + jmeno + "." + koncovka, true);
            }
            catch
            {
                
                MessageBox.Show("Nepodařilo se uložit zaznamenaný stream. Ověřte své připojení k internetu a oprávnění pro zápis do zvolené složky.", "Ukládání zaznamenaného streamu", MessageBoxButtons.OK, MessageBoxIcon.Error);
          // Přidat časový odpočet na púotvrzení této hlášky
            }

            
            try
              {
                stahovac[i - 1].Resume();
                stahovac[i - 1].Abort();
              }
            catch
              {
              }
            
            
        }
        /// <summary>
        /// Metoda zahajující stahování streamu.
        /// </summary>
        static void Stahuj()
        {

            while (internet() == false)
            {
                Thread.Sleep(2500);
            }
            
            bool funguje = true;
            WebClient wb = new WebClient();

            try
            {
               wb.DownloadFile(stream1,Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "stream" + proident + ".dwl");
            }
            catch
            {
            funguje = false;
            }

            if (funguje == false)
            {
            try
            {
                wb.DownloadFile(stream2,Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "stream" + proident + ".dwl");
            }
            catch
            {}
                
            }
                

            
        }

        static bool internet()
        {
            try
            {
                System.Net.IPHostEntry iphe = System.Net.Dns.GetHostByName("www.google.com");
                return true;
            }
            catch
            {
                return false; 
            }

        
        }
    }
}
