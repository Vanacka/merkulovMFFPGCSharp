namespace merkulovMFFPGCSharp.DU;

public class Pismeno
/*
 * Trida, ktera uchovava znak pismene, jeho pozici v poli a aktualni pocet kroku potrebny k dosazeni daneho pismene
 */
{
    public readonly int Radek;
    public readonly int Sloupec;
    //public int PocetKroku;
    public readonly char Znak;

    public Pismeno(int radek, int sloupec, char znak)
    {
        Radek = radek;
        Sloupec = sloupec;
        //PocetKroku = pocetKroku;
        Znak = znak;
    }
}

public class Abeceda
{
    public void Hlavni()
    /*
     * Funkce nacte, zpracuje vstup a vypocita vysledek
     */
    {
        
        int sirka = Ctecka.PrectiCislo();
        int vyska = Ctecka.PrectiCislo();
        char[] pismenaTabulky = Ctecka.PrectiARozdelRadekNaZnaky();
        char[] text = Ctecka.PrectiARozdelRadekNaZnaky();
        
        // Vytvorim slovnik pismen, ve kterem budou ulozeny pozice vsech pismen
        Dictionary<char, List<Pismeno>> pismena = new Dictionary<char, List<Pismeno>>();

        Pismeno pocatecniPismeno = new Pismeno(0, 0, ' ');
        
        // Zapise pismena se spravnou pozici do slovniku
        for (int i = 0; i < pismenaTabulky.Length; i++)
        {
            char aktualniPismeno = pismenaTabulky[i];
            int aktualniRadek = i / sirka;
            int aktualniSloupec = i % sirka;
            Pismeno pismeno = new Pismeno(aktualniRadek, aktualniSloupec, aktualniPismeno);
            if (!pismena.ContainsKey(aktualniPismeno))
            {
                // Vytvorim pismeno a pridam ho do seznamu pismen ve slovniku ke spravnemu klici
                List<Pismeno> seznamPismen = new List<Pismeno>();
                seznamPismen.Add(pismeno);
                pismena.Add(aktualniPismeno, seznamPismen);
            }
            else
            {
                pismena[aktualniPismeno].Add(pismeno);
            }
        }

        if (text.Length == 0) Console.WriteLine(0);
        else Console.WriteLine(NajdiNejkratsiPosloupnost(pocatecniPismeno, pismena, text));
    }

    public int NajdiNejkratsiPosloupnost(Pismeno zacatek, Dictionary<char, List<Pismeno>> pismena, char[] text)
        /*
         * Metoda najde nejkratsi mozny pocet tahu pro napsani daneho textu a vrati vysledek
         */
    {
        Dictionary<Pismeno, int> aktualniStavy = new Dictionary<Pismeno, int>();
        char aktualniZnak;
        int aktualniRadek;
        int aktualniSloupec;
        int aktualniVzdalenost;
        aktualniStavy.Add(zacatek, 0);

        // Postupne prochazim poslopnost textu
        for (int i = 0; i < text.Length; i++)
        {
            aktualniZnak = text[i];
            if (!pismena.ContainsKey(aktualniZnak)) continue;
            List<Pismeno> zkoumanaPismena = pismena[aktualniZnak];
            Dictionary<Pismeno, int> dalsiStavy = new Dictionary<Pismeno, int>();
            
            // Pro kazde startovni pismeno najdu vzdalenosti do nasledujicich
            foreach (KeyValuePair<Pismeno, int> stav in aktualniStavy)
            {
                aktualniRadek = stav.Key.Radek;
                aktualniSloupec = stav.Key.Sloupec;
                aktualniVzdalenost = stav.Value;
                
                // Pro kazde nyni zkoumane pismeno zjistim nejlepsi mozny pocet kroku, na ktery se tam dokazu dostat
                foreach (Pismeno pismeno in zkoumanaPismena)
                {
                    int radekPismena = pismeno.Radek;
                    int sloupecPismena = pismeno.Sloupec;
                    int vzdalenost = Math.Abs(aktualniRadek - radekPismena) + Math.Abs(aktualniSloupec - sloupecPismena) + 1;
                    if (!dalsiStavy.ContainsKey(pismeno))
                    {
                        dalsiStavy.Add(pismeno, vzdalenost + aktualniVzdalenost);
                    }
                    if (dalsiStavy[pismeno] > aktualniVzdalenost + vzdalenost)
                    {
                        dalsiStavy[pismeno] = aktualniVzdalenost + vzdalenost;
                    }
                }
            }
            aktualniStavy = dalsiStavy;
        }
        return aktualniStavy.Values.Min();
    }
}