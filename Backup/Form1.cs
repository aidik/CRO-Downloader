using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace CRO
{
    public partial class Form1 : Form
    {
        public static int i;
        public static TimeSpan doba;
        public static TimeSpan celkem_doba;
        public static TimeSpan nyni;
        public static TimeSpan ubehlo;
        public static int cyklu;
        public static bool stahuje;
        public static bool bezi = false;
        public static bool vybranaCesta = false;
        public static string soubor;

        public Form1()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Metoda pro přidání zvoleného data a času do kolekce listBoxu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime cas = dateTimePicker1.Value;
            DateTime datum = dateTimePicker2.Value;
            DateTime combo = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, dateTimePicker1.Value.Hour, dateTimePicker1.Value.Minute, dateTimePicker1.Value.Second);
            listBox1.Items.Add(Convert.ToString(combo));
            
        }

        /// <summary>
        /// Metoda odstaňuje vybranou položku z kolekce listBoxu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            listBox1.Items.Remove(listBox1.SelectedItem);
            
        }

        /// <summary>
        /// Metoda čistící celou kolekci listBoxu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }


       /// <summary>
       /// Metoda zahajující stahování. Uloží a načte aktuální informace z uživatelských polí a zahájí vlákno pro stahování a vlákno pro kontorlu údajů.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (bezi == false)
            {
                bezi = true;
                bool start = true;

                try
                {
                    saveRSD(null, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "active.xml");
                    listBox1.Items.Clear();
                    openRSD(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "active.xml");
                    Radio.path = textBox5.Text.Substring(0, 2) + Radio.Odfiltruj(textBox5.Text.Substring(2));

                    try
                    {

                        Thread thr = new Thread(Radio.Start);
                        Thread cnt = new Thread(new ThreadStart(this.Control));
                        thr.Start();
                        cnt.Start();
                    }
                    catch
                    {
                        MessageBox.Show("Při startování úlohy nastala chyba. Ověřte problematická vlákna.", "Start úlohy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bezi = false;
                        start = false;
                    }
                }
                catch
                {
                    if (start == true)
                    {
                        MessageBox.Show("Při startování úlohy nastala chyba. Ověřte, že jsou všechna pole vyplněna platnými hodnotami.", "Start úlohy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bezi = false;
                    }
 
                }
                



                

            }
            else
            {
                //nedelej nic;
 
            }
        }


        public delegate void ShowControlsDelegate(string jmeno, string do_konce, int progres, int progres_celkem);
 
        /// <summary>
        /// Metoda zobrazující údaje o průběhu stahování.
        /// </summary>
        /// <param name="jmeno"></param>
        /// <param name="do_konce"></param>
        /// <param name="progres"></param>
        /// <param name="progres_celkem"></param>
        public void ShowControls(string jmeno, string do_konce, int progres, int progres_celkem)
        {
            if (this.label6.InvokeRequired || this.label7.InvokeRequired || this.progressBar1.InvokeRequired || this.progressBar2.InvokeRequired)
            {
                try
                {
                    this.Invoke(new ShowControlsDelegate(ShowControls), jmeno, do_konce, progres, progres_celkem);
                }
                catch
                { }
            }
            else
            {
                this.label6.Text = jmeno;
                this.label7.Text = do_konce;
                this.progressBar1.Value = progres;
                this.progressBar2.Value = progres_celkem;
                
            }

        }




        /// <summary>
        /// Metoda na přípravu údajů o úloze před zobrazením
        /// </summary>
        private void Control()
        {
            ShowControls("Získávám hodnoty...", "Získávám hodnoty...", 0, 0);
            Thread.Sleep(1250);
            while (1 == 1)
            {
                Thread.Sleep(500);
                string stav;
                TimeSpan odpocet;
                double procenta;
                if (stahuje == true)
                {
                    procenta = nyni.TotalMilliseconds / doba.TotalMilliseconds;
                    odpocet = doba - nyni;
                    procenta = procenta * 320;
                    stav = " konce ";
                
                }

                                     
                else
                {
                    procenta = 0;
                    odpocet = nyni;
                    stav = " začátku ";
                }


                double uplna_procenta = ubehlo.TotalMilliseconds / celkem_doba.TotalMilliseconds;
                TimeSpan uplny_konec = celkem_doba - ubehlo;
                uplna_procenta = uplna_procenta * 320;


                ShowControls("Do" + stav  + i.ToString() + ". úlohy ze " + cyklu.ToString() + " zbývá " + OsetriDny(odpocet), "Všechny úlohy budou dokončeny za " + OsetriDny(uplny_konec), (int)procenta, (int)uplna_procenta);
                try
                {
                    notifyIcon1.Text = "ČRO Downloader\n" + "Do" + stav + i.ToString() + ". z " + cyklu.ToString() + " " + OsetriDny(odpocet) + "\nVše za " + OsetriDny(uplny_konec);
                }
                catch
                {
                    try
                    {
                        notifyIcon1.Text = "ČRO Downloader\n" + "Do" + stav + i.ToString() + ". z " + cyklu.ToString() + " " + OsetriDny(odpocet);
                    }
                    catch
                    {
                        notifyIcon1.Text = "ČRO Downloader\n" + "Vše bude dokončeno za velmi dlouho. :-)";
 
                    }
 
                }
            }

        }
        /// <summary>
        /// Metoda, která na základě časového úseku ts rozhodne o vhodném použití slova den / dnů.
        /// </summary>
        /// <param name="ts">časový rozsah úlohy</param>
        /// <returns></returns>
        private string OsetriDny(TimeSpan ts)
        {
            string vs;
            if (ts.TotalDays >= 1)
            {
                string den = " dnů ";
                if ((ts.Days - 1) % 10 == 0)
                {
                    den = " den ";
                }
                
                vs = ts.Days.ToString() + den + ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                
            }
            else
            {
                vs = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString(); 
            }
            return vs;
        }

        /// <summary>
        /// Metoda ukáže dialog "O aplikaci"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oAplikaciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab1 = new AboutBox1();
            ab1.ShowDialog();
        }

        /// <summary>
        /// Metoda na vyvoální nabídky pro uložení úlohy - souboru RSD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ulozitJakoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "RSD File|*.rsd";
            sfd.FileName = textBox1.Text;
            sfd.Title = "Uložit RSD soubor s úlohou";
            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                try
                {
                    saveRSD(sfd.OpenFile(), null);
                }
                catch
                {
                    MessageBox.Show("Při ukládání RSD souboru " + sfd.FileName +" došlo k chybě. Ověřte, že všechna pole jsou vyplněna  platnými hodnotami.", "Ukládání RSD souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
 
                }

                
            }

        }
        /// <summary>
        /// Metoda načítající data ze zvoleného RSD souboru.
        /// </summary>
        /// <param name="jm">Cesta k soboru</param>
        private void openRSD(string jm)
        {
            DataSet ods = new DataSet();

            ods.ReadXml(jm);

            Radio.jmeno = Radio.Odfiltruj((string)ods.Tables["Informace"].Rows[0]["Jmeno"]);
            Radio.doba = Convert.ToInt32(ods.Tables["Informace"].Rows[0]["Trvani"]);
            Radio.koncovka = (string)ods.Tables["Informace"].Rows[0]["Koncovka"];
            Radio.stream1 = new Uri((string)ods.Tables["Informace"].Rows[0]["URL1"]);
            Radio.stream2 = new Uri((string)ods.Tables["Informace"].Rows[0]["URL2"]);

            Radio.cyklu = ods.Tables["Casy"].Rows.Count;
            string[] whh = new string[Radio.cyklu];

            for (int r = 0; r < ods.Tables["Casy"].Rows.Count; r++)
            {

                whh[r] = (string)ods.Tables["Casy"].Rows[r]["Cas"];

            }

            Radio.when = whh;
            Radio.stahovac = new Thread[Radio.cyklu];
            Radio.nuly = Radio.kolik_nul();


            textBox1.Text = Radio.jmeno;
            textBox2.Text = Radio.doba.ToString();
            textBox3.Text = Radio.stream1.ToString();
            textBox4.Text = Radio.stream2.ToString();
            comboBox1.Text = Radio.koncovka;
            
            
            listBox1.Items.Clear();
            foreach (string lbi in whh)
            {

                listBox1.Items.Add(lbi);

            }
            
            

        }
        /// <summary>
        /// Metoda ukládající data do zvoleného RSD souboru
        /// </summary>
        /// <param name="sn">Stream z save dialogu</param>
        /// <param name="st">String s názvem souboru</param>
        private void saveRSD(Stream sn, string st)
        {
            DataSet sds = new DataSet();
            DataTable info = new DataTable("Informace");

            DataColumn trvani = new DataColumn("Trvani", Type.GetType("System.Int32"));
            DataColumn jmenoF = new DataColumn("Jmeno");
            DataColumn koncovkaF = new DataColumn("Koncovka");
            DataColumn url1 = new DataColumn("URL1");
            DataColumn url2 = new DataColumn("URL2");


            info.Columns.Add(trvani);
            info.Columns.Add(jmenoF);
            info.Columns.Add(koncovkaF);
            info.Columns.Add(url1);
            info.Columns.Add(url2);

            sds.Tables.Add(info);

            DataTable casy = new DataTable("Casy");
            DataColumn cas = new DataColumn("Cas");

            casy.Columns.Add(cas);
            sds.Tables.Add(casy);

            DataRow ps = sds.Tables["Informace"].NewRow();

            ps["Trvani"] = Convert.ToInt32(textBox2.Text);
            ps["Jmeno"] = textBox1.Text;
            ps["Koncovka"] = comboBox1.Text;
            ps["URL1"] = textBox3.Text;
            if (textBox4.Text == null || textBox4.Text == "")
            {
                ps["URL2"] = textBox3.Text;
            }
            else
            {
                ps["URL2"] = textBox4.Text;
            }


            sds.Tables["Informace"].Rows.Add(ps);


            foreach (object wh in listBox1.Items)
            {

                DataRow pd = sds.Tables["Casy"].NewRow();

                pd["Cas"] = wh;

                sds.Tables["Casy"].Rows.Add(pd);

            }

            if (sn == null)
            {
                
                sds.WriteXml(st, XmlWriteMode.IgnoreSchema);

            }
            else
            {
                sds.WriteXml(sn, XmlWriteMode.IgnoreSchema);
            }

        
        }
        /// <summary>
        /// Metoda pro zobrazení otevíracího dialogu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void otevritToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "RSD File|*.rsd";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    openRSD(ofd.FileName);
                }
                catch
                {
                     MessageBox.Show("Při načítání RSD souboru " + ofd.FileName +" došlo k chybě. Soubor je pravděpodobně poškozen.", "Načítání RSD souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        /// <summary>
        /// Metoda pro ukončení aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void konecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);

        }
        /// <summary>
        /// Metoda pro ukončení aplikace po kliknutí na křížek.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        /// <summary>
        /// Metoda pro zobrazení uživatelské příručky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ovladaniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(null, "http://otehot.net/radio.html");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyDocuments;
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                textBox5.Text = fbd.SelectedPath;
                vybranaCesta = true;
            }
            else
            {
                zjistiCestu();
            }
            

        }

        private void zjistiCestu()
        {
            if (vybranaCesta == false)
            {
                textBox5.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + textBox1.Text;
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            zjistiCestu();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            Hide();

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                Hide();
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            zjistiCestu();

            if (Program.soubor != null)
            {
                try
                {
                    openRSD(Program.soubor);
                }
                catch
                {
                    MessageBox.Show("Při načítání RSD souboru " + Program.soubor + " došlo k chybě. Soubor je pravděpodobně poškozen.", "Načítání RSD souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime cas = dateTimePicker1.Value;
                DateTime datum = dateTimePicker2.Value;
                DateTime combo = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, dateTimePicker1.Value.Hour, dateTimePicker1.Value.Minute, dateTimePicker1.Value.Second);
                for (int i = 0; i < Convert.ToInt32(textBox6.Text); i++)
                {
                    listBox1.Items.Add(Convert.ToString(combo));
                    combo = combo.AddDays(1);
                }
            }
            catch
            {
                MessageBox.Show("Při vkládání opakování nastala chyba. Ověřte, že pole opakování je vyplněno celými čísly.", "Vkládání opakování", MessageBoxButtons.OK, MessageBoxIcon.Error);
 
            }

        }



        private void Form1_DragDrop(object sender, DragEventArgs e)
        {

            string[] soubory = (string[])e.Data.GetData(DataFormats.FileDrop);

            try
            {
                openRSD(soubory[0]);

            }
            catch
            {
                MessageBox.Show("Při načítání RSD souboru došlo k chybě. Soubor je pravděpodobně poškozen.", "Načítání RSD souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
 
            }

        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }

        
    }
}
