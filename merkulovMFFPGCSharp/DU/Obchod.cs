using System;
using System.Collections.Generic;
using System.Text;

namespace simulace
{
    public enum TypUdalosti
    {
        Start,
        Trpelivost,
        Obslouzen
    }

    public class Udalost
    {
        public int kdy;
        public Proces kdo;
        public TypUdalosti co;
        public Udalost(int kdy, Proces kdo, TypUdalosti co)
        {
            this.kdy = kdy;
            this.kdo = kdo;
            this.co = co;

        }
    }
    public class Kalendar
    {
        private List<Udalost> seznam;
        public Kalendar()
        {
            seznam = new List<Udalost>();
        }
        public void Pridej(int kdy, Proces kdo, TypUdalosti co)
        {
            //Console.WriteLine("PLAN: {0} {1} {2}", kdy, kdo.ID, co);
            // pro hledani chyby:
            foreach (Udalost ud in seznam)
                if (ud.kdo == kdo)
                    Console.WriteLine("");


            seznam.Add(new Udalost(kdy, kdo, co));
        }
        public void Odeber(Proces kdo, TypUdalosti co)
        {
            foreach (Udalost ud in seznam)
            {
                if ((ud.kdo == kdo) && (ud.co == co))
                {
                    seznam.Remove(ud);
                    return; // odebiram jen jeden vyskyt!
                }
            }
        }
        public Udalost Prvni()
        {
            Udalost prvni = null;
            foreach (Udalost ud in seznam)
                if ((prvni == null) || (ud.kdy < prvni.kdy))
                    prvni = ud;
            seznam.Remove(prvni);
            return prvni;
        }
        public Udalost Vyber()
        {
            return Prvni();
        }

    }

    public abstract class Proces
    {
        public static char[] mezery = { ' ' };
        public int patro;
        public string ID;
        public abstract void Zpracuj(Udalost ud);
        public void log(string zprava)
        {
            //if (ID == "Dana")
            //if (ID == "elefant")
            //if (this is Zakaznik)
                //Console.WriteLine($"{model.Cas}/{patro} {ID}: {zprava}");
        }
        protected Model model;
    }

    public class Oddeleni : Proces
    {
        private int rychlost;
        private List<Zakaznik> fronta;
        private bool obsluhuje;

        public Oddeleni(Model model, string popis)
        {
            this.model = model;
            string[] popisy = popis.Split(Proces.mezery, StringSplitOptions.RemoveEmptyEntries);
            this.ID = popisy[0];
            this.patro = int.Parse(popisy[1]);
            if (this.patro > model.MaxPatro)
                model.MaxPatro = this.patro;
            this.rychlost = int.Parse(popisy[2]);
            obsluhuje = false;
            fronta = new List<Zakaznik>();
            model.VsechnaOddeleni.Add(this);
        }
        public void ZaradDoFronty(Zakaznik zak)
        {
            fronta.Add(zak);
            log("do fronty " + zak.ID);

            if (obsluhuje) ; // nic
            else
            {
                obsluhuje = true;
                model.Naplanuj(model.Cas, this, TypUdalosti.Start);
            }
        }
        public void VyradZFronty(Zakaznik koho)
        {
            fronta.Remove(koho);
        }
        public override void Zpracuj(Udalost ud)
        {
            switch (ud.co)
            {
                case TypUdalosti.Start:
                    if (fronta.Count == 0)
                        obsluhuje = false; // a dal neni naplanovana a probudi se tim, ze se nekdo zaradi do fronty
                    else
                    {
                        Zakaznik zak = fronta[0];
                        fronta.RemoveAt(0);
                        model.Odplanuj(zak, TypUdalosti.Trpelivost);
                        model.Naplanuj(model.Cas + rychlost, zak, TypUdalosti.Obslouzen);
                        model.Naplanuj(model.Cas + rychlost, this, TypUdalosti.Start);
                    }
                    break;
            }
        }
        
        public int DelkaFronty()
        {
            return fronta.Count;
        }
    }
    public enum SmeryJizdy
    {
        Nahoru,
        Dolu,
        Stoji
    }
    public class Vytah : Proces
    {
        private int kapacita;
        private int dobaNastupu;
        private int dobaVystupu;
        private int dobaPatro2Patro;
        static int[] ismery = { +1, -1, 0 }; // prevod (int) SmeryJizdy na smer

        private class Pasazer
        {
            public Proces kdo;
            public int kamJede;
            public Pasazer(Proces kdo, int kamJede)
            {
                this.kdo = kdo;
                this.kamJede = kamJede;
            }
        }

        private List<Pasazer>[,] cekatele; // [patro,smer]
        private List<Pasazer> naklad;   // pasazeri ve vytahu
        private SmeryJizdy smer;
        private int kdyJsemMenilSmer;

        public void PridejDoFronty(int odkud, int kam, Proces kdo)
        {
            Pasazer pas = new Pasazer(kdo, kam);
            if (kam > odkud)
                cekatele[odkud, (int)SmeryJizdy.Nahoru].Add(pas);
            else
                cekatele[odkud, (int)SmeryJizdy.Dolu].Add(pas);

            // pripadne rozjet stojici vytah:
            if (smer == SmeryJizdy.Stoji)
            {
                model.Odplanuj(model.vytah, TypUdalosti.Start); // kdyby nahodou uz byl naplanovany
                model.Naplanuj(model.Cas, this, TypUdalosti.Start);
            }
        }
        public bool CekaNekdoVPatrechVeSmeruJizdy()
        {
            int ismer = ismery[(int)smer];
            for (int pat = patro + ismer; (pat > 0) && (pat <= model.MaxPatro); pat += ismer)
                if ((cekatele[pat, (int)SmeryJizdy.Nahoru].Count > 0) || (cekatele[pat, (int)SmeryJizdy.Dolu].Count > 0))
                {
                    if (cekatele[pat, (int)SmeryJizdy.Nahoru].Count > 0)
                        log("Nahoru čeká " + cekatele[pat, (int)SmeryJizdy.Nahoru][0].kdo.ID
                            + " v patře " + pat + "/" + cekatele[pat, (int)SmeryJizdy.Nahoru][0].kdo.patro);
                    if (cekatele[pat, (int)SmeryJizdy.Dolu].Count > 0)
                        log("Dolů čeká " + cekatele[pat, (int)SmeryJizdy.Dolu][0].kdo.ID
                            + " v patře " + pat + "/" + cekatele[pat, (int)SmeryJizdy.Dolu][0].kdo.patro);

                    //log(" x "+cekatele[pat, (int)SmeryJizdy.Nahoru].Count+" x "+cekatele[pat, (int)SmeryJizdy.Dolu].Count);
                    return true;
                }
            return false;
        }
        public int DelkaCekateluVPatre(int pat)
        {
            return cekatele[pat, (int)SmeryJizdy.Nahoru].Count + cekatele[pat, (int)SmeryJizdy.Dolu].Count;
        }

        public Vytah(Model model, string popis)
        {
            this.model = model;
            string[] popisy = popis.Split(Proces.mezery, StringSplitOptions.RemoveEmptyEntries);
            this.ID = popisy[0];
            this.kapacita = int.Parse(popisy[1]);
            this.dobaNastupu = int.Parse(popisy[2]);
            this.dobaVystupu = int.Parse(popisy[3]);
            this.dobaPatro2Patro = int.Parse(popisy[4]);
            this.patro = 0;
            this.smer = SmeryJizdy.Stoji;
            this.kdyJsemMenilSmer = -1;

            cekatele = new List<Pasazer>[model.MaxPatro + 1, 2];
            for (int i = 0; i < model.MaxPatro + 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    cekatele[i, j] = new List<Pasazer>();
                }

            }
            naklad = new List<Pasazer>();
        }
        public override void Zpracuj(Udalost ud)
        {
            switch (ud.co)
            {
                case TypUdalosti.Start:

                    // HACK pro cerstve probuzeny vytah:
                    if (smer == SmeryJizdy.Stoji)
                        // stoji, tedy nikoho neveze a nekdo ho prave probudil => nastavim jakykoliv smer a najde ho:
                        smer = SmeryJizdy.Nahoru;

                    // chce nekdo vystoupit?
                    foreach (Pasazer pas in naklad)
                        if (pas.kamJede == patro)
                        // bude vystupovat:
                        {
                            naklad.Remove(pas);

                            pas.kdo.patro = patro;
                            model.Naplanuj(model.Cas + dobaVystupu, pas.kdo, TypUdalosti.Start);
                            log("vystupuje " + pas.kdo.ID);

                            model.Naplanuj(model.Cas + dobaVystupu, this, TypUdalosti.Start);

                            return; // to je pro tuhle chvili vsechno
                        }

                    // muze a chce nekdo nastoupit?
                    if (naklad.Count == kapacita)
                    // i kdyby chtel nekdo nastupovat, nemuze; veze lidi => pokracuje:
                    {
                        // popojet:
                        int ismer = ismery[(int)smer];
                        patro = patro + ismer;

                        string spas = "";
                        foreach (Pasazer pas in naklad)
                            spas += " " + pas.kdo.ID;
                        log("odjíždím");
                        model.Naplanuj(model.Cas + dobaPatro2Patro, this, TypUdalosti.Start);
                        return; // to je pro tuhle chvili vsechno
                    }
                    else
                    // neni uplne plny
                    {
                        // chce nastoupit nekdo VE SMERU jizdy?
                        if (cekatele[patro, (int)smer].Count > 0)
                        {
                            log("nastupuje " + cekatele[patro, (int)smer][0].kdo.ID);
                            naklad.Add(cekatele[patro, (int)smer][0]);
                            cekatele[patro, (int)smer].RemoveAt(0);
                            model.Naplanuj(model.Cas + dobaNastupu, this, TypUdalosti.Start);

                            return; // to je pro tuhle chvili vsechno
                        }

                        // ve smeru jizdy nikdo nenastupuje:
                        if (naklad.Count > 0)
                        // nikdo nenastupuje, vezu pasazery => pokracuju v jizde:
                        {
                            // popojet:
                            int ismer = ismery[(int)smer];
                            patro = patro + ismer;

                            string spas = "";
                            foreach (Pasazer pas in naklad)
                                spas += " " + pas.kdo.ID;
                            //log("nekoho vezu");
                            log("odjíždím: " + spas);

                            model.Naplanuj(model.Cas + dobaPatro2Patro, this, TypUdalosti.Start);
                            return; // to je pro tuhle chvili vsechno
                        }

                        // vytah je prazdny, pokud v dalsich patrech ve smeru jizdy uz nikdo neceka, muze zmenit smer nebo se zastavit:
                        if (CekaNekdoVPatrechVeSmeruJizdy() == true)
                        // pokracuje v jizde:
                        {
                            // popojet:
                            int ismer = ismery[(int)smer];
                            patro = patro + ismer;

                            //log("nekdo ceka");
                            log("odjíždím");
                            model.Naplanuj(model.Cas + dobaPatro2Patro, this, TypUdalosti.Start);
                            return; // to je pro tuhle chvili vsechno
                        }

                        // ve smeru jizdy uz nikdo neceka => zmenit smer nebo zastavit:
                        if (smer == SmeryJizdy.Nahoru)
                            smer = SmeryJizdy.Dolu;
                        else
                            smer = SmeryJizdy.Nahoru;

                        log("změna směru");

                        //chce nekdo nastoupit prave tady?
                        if (kdyJsemMenilSmer != model.Cas)
                        {
                            kdyJsemMenilSmer = model.Cas;
                            // podivat se, jestli nekdo nechce nastoupit opacnym smerem:
                            model.Naplanuj(model.Cas, this, TypUdalosti.Start);
                            return;
                        }

                        // uz jsem jednou smer menil a zase nikdo nenastoupil a nechce => zastavit
                        log("zastavuje");
                        smer = SmeryJizdy.Stoji;
                        return; // to je pro tuhle chvili vsechno
                    }
            }
        }
    }
    public class Zakaznik : Proces
    {
        private int trpelivost;
        private int prichod;
        private List<string> Nakupy;
        public int P;
        public Zakaznik(Model model, string popis)
        {
            this.model = model;
            string[] popisy = popis.Split(Proces.mezery, StringSplitOptions.RemoveEmptyEntries);
            this.ID = popisy[0];
            this.prichod = int.Parse(popisy[1]);
            this.trpelivost = int.Parse(popisy[2]);
            Nakupy = new List<string>();
            for (int i = 3; i < popisy.Length; i++)
            {
                Nakupy.Add(popisy[i]);
            }
            this.patro = 0;
            //
            //Console.WriteLine("Init Zakaznik: {0}", ID);
            model.Naplanuj(prichod, this, TypUdalosti.Start);
        }
        public override void Zpracuj(Udalost ud)
        {
            switch (ud.co)
            {
                case TypUdalosti.Start:
                    if (Nakupy.Count > 1) 
                    {
                        int druh = P % 3;

                        // DRUHÝ DRUH (S1): Přednostně hledá nákup v aktuálním patře
                        if (druh == 2)
                        {
                            for (int i = 0; i < Nakupy.Count; i++)
                            {
                                Oddeleni odd = OddeleniPodleJmena(Nakupy[i]);
                                if (odd != null && odd.patro == this.patro)
                                {
                                    // Našel oddělení ve svém patře! Přesune ho na začátek seznamu.
                                    string vybranyNakup = Nakupy[i];
                                    Nakupy.RemoveAt(i);
                                    Nakupy.Insert(0, vybranyNakup);
                                    break; 
                                }
                            }
                        }
                        // TŘETÍ DRUH (S2): Hledá nákup s nejkratší frontou (včetně výtahu)
                        else if (druh == 0)
                        {
                            int nejmensiSkore = int.MaxValue;
                            int indexNejlepsiho = 0;

                            for (int i = 0; i < Nakupy.Count; i++)
                            {
                                Oddeleni odd = OddeleniPodleJmena(Nakupy[i]);
                                if (odd != null)
                                {
                                    int skore = odd.DelkaFronty();
                    
                                    // Pokud je oddělení v jiném patře, připočítáme lidi čekající na výtah
                                    if (odd.patro != this.patro)
                                    {
                                        skore += model.vytah.DelkaCekateluVPatre(this.patro);
                                    }

                                    if (skore < nejmensiSkore)
                                    {
                                        nejmensiSkore = skore;
                                        indexNejlepsiho = i;
                                    }
                                }
                            }
                            // Přesune vybraný nákup s nejmenší frontou na začátek
                            string vybranyNakup = Nakupy[indexNejlepsiho];
                            Nakupy.RemoveAt(indexNejlepsiho);
                            Nakupy.Insert(0, vybranyNakup);
                        }
                    }
                    if (Nakupy.Count == 0)
                        // ma nakoupeno
                    {
                        if (patro == 0) 
                        {
                            //log("-------------- odchází"); // nic, konci
                            int druh = P % 3;
                            model.CelkovyCasPodleDruhu[druh] += (model.Cas - this.prichod);
                            model.PocetOdejdutychPodleDruhu[druh]++;
                        }
                        else
                            model.vytah.PridejDoFronty(patro, 0, this);
                    }
                    else
                    {
                        Oddeleni odd = OddeleniPodleJmena(Nakupy[0]);
                        int pat = odd.patro;
                        if (pat == patro) // to oddeleni je v patre, kde prave jsem
                        {
                            if (Nakupy.Count > 1)
                                model.Naplanuj(model.Cas + trpelivost, this, TypUdalosti.Trpelivost);
                            odd.ZaradDoFronty(this);
                        }
                        else
                            model.vytah.PridejDoFronty(patro, pat, this);
                    }
                    break;
                case TypUdalosti.Obslouzen:
                    log("Nakoupeno: " + Nakupy[0]);
                    Nakupy.RemoveAt(0);
                    // ...a budu hledat dalsi nakup -->> Start
                    model.Naplanuj(model.Cas, this, TypUdalosti.Start);
                    break;
                case TypUdalosti.Trpelivost:
                    log("!!! Trpělivost: " + Nakupy[0]);
                    // vyradit z fronty:
                    {
                        Oddeleni odd = OddeleniPodleJmena(Nakupy[0]);
                        odd.VyradZFronty(this);
                    }

                    // prehodit tenhle nakup na konec:
                    string nesplneny = Nakupy[0];
                    Nakupy.RemoveAt(0);
                    Nakupy.Add(nesplneny);

                    // ...a budu hledat dalsi nakup -->> Start
                    model.Naplanuj(model.Cas, this, TypUdalosti.Start);
                    break;
            }
        }

        private Oddeleni OddeleniPodleJmena(string kamChci)
        {
            foreach (Oddeleni odd in model.VsechnaOddeleni)
                if (odd.ID == kamChci)
                    return odd;
            return null;
        }
    }


    public class Model
    {
        public int Cas;
        public Vytah vytah;
        public List<Oddeleni> VsechnaOddeleni = new List<Oddeleni>();
        public int MaxPatro;
        private Kalendar kalendar;
        public double[] CelkovyCasPodleDruhu = new double[3];
        public int[] PocetOdejdutychPodleDruhu = new int[3];
        public void Naplanuj(int kdy, Proces kdo, TypUdalosti co)
        {
            kalendar.Pridej(kdy, kdo, co);
        }
        public void Odplanuj(Proces kdo, TypUdalosti co)
        {
            kalendar.Odeber(kdo, co);
        }
        public void VytvorProcesy()
        {
            System.IO.StreamReader soubor
                = new
          System.IO.StreamReader("/home/ivan/DU/mffPG/merkulovMFFPGCSharp/merkulovMFFPGCSharp/DU/obchod_data.txt");
            while (!soubor.EndOfStream)
            {
                string s = soubor.ReadLine();
                if (s != "")
                {
                    switch (s[0])
                    {
                        case 'O':
                            new Oddeleni(this, s.Substring(1));
                            break;
                        case 'Z':
                            new Zakaznik(this, s.Substring(1));
                            break;
                        case 'V':
                            vytah = new Vytah(this, s.Substring(1));
                            break;
                    }
                }
            }
            soubor.Close();
        }
        public int Vypocet(int pocetZakazniku)
        {
            Array.Clear(CelkovyCasPodleDruhu, 0, 3);
            Array.Clear(PocetOdejdutychPodleDruhu, 0, 3);
            Cas = 0;
            kalendar = new Kalendar();
            VytvorProcesy();
            GenerujZakazniky(pocetZakazniku);

            Udalost ud;

            while ((ud = kalendar.Vyber()) != null)
            {
                //Console.WriteLine("{0} {1} {2}", ud.kdy, ud.kdo.ID, ud.co);
                Cas = ud.kdy;
                ud.kdo.Zpracuj(ud);
            }
            return Cas;
        }
        
        public void GenerujZakazniky(int pocet)
        {
            // Názvy oddělení musí přesně odpovídat indexům (0 = papírnictví, atd.)
            string[] nazvyOddeleni = { "papírnictví", "potraviny", "drogerie", "textil", "nábytek", "elektronika", "CD-DVD" };

            for (int p = 1; p <= pocet; p++) // Proměnná 'p' poslouží jako pořadové číslo P
            {
                // Generování hodnot podle zadání (pozor: horní mez u rnd.Next je exkluzivní, proto +1)
                int prichod = Program.rnd.Next(0, 601);
                int trpelivost = Program.rnd.Next(1, 181);
                int pocetNakupu = Program.rnd.Next(1, 21);

                List<string> vybraneNakupy = new List<string>();
                for (int i = 0; i < pocetNakupu; i++)
                {
                    int indexOddeleni = Program.rnd.Next(0, nazvyOddeleni.Length);
                    vybraneNakupy.Add(nazvyOddeleni[indexOddeleni]);
                }

                // Původní konstruktor Zákazníka bere string ve formátu "Jmeno Prichod Trpelivost Nakup1 Nakup2..."
                // Sestavíme ten string ručně, ať nemusíme přepisovat vnitřek třídy Zakaznik.
                string jmeno = "Zakaznik_" + p;
                string popis = $"{jmeno} {prichod} {trpelivost} {string.Join(" ", vybraneNakupy)}";

                Zakaznik novyZakaznik = new Zakaznik(this, popis);
        
                // DŮLEŽITÉ: Budeš si u zákazníka potřebovat uložit jeho 'P', aby s ním později šlo
                // rozlišovat ty 3 druhy chování (P%3). 
                // -> Přidej 'public int P;' jako vlastnost do třídy Zakaznik.
                novyZakaznik.P = p; 
            }
        }
    }

    class Program
    {
        public static Random rnd = new Random(12345);
        
        static void Mainova(string[] args)
        {
            // Hlavička pro výpis (formát CSV pro snadný import do Excelu)
        Console.WriteLine("PocetZakazniku; Obycejni_Druh1; S1_Druh2; S2_Druh0");

        // Cyklus pro počty zákazníků: 1, 11, 21, ..., 501
        for (int N = 1; N <= 501; N += 10)
        {
            // Tři samostatné seznamy pro výsledky 10 měření
            List<double> vysledkyDruh0 = new List<double>(); // S2 (Nejkratší fronta)
            List<double> vysledkyDruh1 = new List<double>(); // Obyčejní
            List<double> vysledkyDruh2 = new List<double>(); // S1 (Nákup ve stejném patře)

            // Pro každý počet spustíme simulaci 10x
            for (int i = 0; i < 10; i++)
            {
                Model model = new Model();
                model.Vypocet(N);

                // Zjistíme a uložíme průměr pro Druh 0 (S2)
                if (model.PocetOdejdutychPodleDruhu[0] > 0)
                    vysledkyDruh0.Add(model.CelkovyCasPodleDruhu[0] / model.PocetOdejdutychPodleDruhu[0]);
                else
                    vysledkyDruh0.Add(0); // Pojistka, kdyby náhodou nikdo z tohoto druhu nebyl

                // Zjistíme a uložíme průměr pro Druh 1 (Obyčejní)
                if (model.PocetOdejdutychPodleDruhu[1] > 0)
                    vysledkyDruh1.Add(model.CelkovyCasPodleDruhu[1] / model.PocetOdejdutychPodleDruhu[1]);
                else
                    vysledkyDruh1.Add(0);

                // Zjistíme a uložíme průměr pro Druh 2 (S1)
                if (model.PocetOdejdutychPodleDruhu[2] > 0)
                    vysledkyDruh2.Add(model.CelkovyCasPodleDruhu[2] / model.PocetOdejdutychPodleDruhu[2]);
                else
                    vysledkyDruh2.Add(0);
            }

            // Zpracujeme nasbíraná data (zahodíme 2 extrémy a zprůměrujeme 8 hodnot)
            double prumer0 = ZpracujVysledky(vysledkyDruh0);
            double prumer1 = ZpracujVysledky(vysledkyDruh1);
            double prumer2 = ZpracujVysledky(vysledkyDruh2);

            // Vypíšeme na jeden řádek vedle sebe
            Console.WriteLine($"{N}; {Math.Round(prumer1, 2)}; {Math.Round(prumer2, 2)}; {Math.Round(prumer0, 2)}");
        }

        Console.WriteLine("Hotovo. Stiskni Enter.");
        Console.ReadLine();
    }

    // Pomocná metoda pro vyhození extrémů a výpočet průměru z 8 hodnot
    static double ZpracujVysledky(List<double> vysledky)
    {
        // Pojistka, kdyby list nebyl plný
        if (vysledky.Count != 10) return 0; 

        vysledky.Sort();
        vysledky.RemoveAt(9); // Odstraní největší (max)
        vysledky.RemoveAt(0); // Odstraní nejmenší (min)

        double suma = 0;
        foreach (double v in vysledky)
        {
            suma += v;
        }
        return suma / 8.0;
        }
    }
}