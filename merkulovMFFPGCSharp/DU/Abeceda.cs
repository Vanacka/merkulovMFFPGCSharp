namespace merkulovMFFPGCSharp.DU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Pismeno
{
    public readonly int Radek;
    public readonly int Sloupec;
    public readonly char Znak;

    public Pismeno(int radek, int sloupec, char znak)
    {
        Radek = radek;
        Sloupec = sloupec;
        Znak = znak;
    }
}

public class Abeceda
{
    public void Hlavni()
    {
        string prectenyText = "";
        string cestaKSouboru = "/home/ivan/DU/mffPG/merkulovMFFPGCSharp/merkulovMFFPGCSharp/DU/text-abecedy.txt";
        
        if (File.Exists(cestaKSouboru))
        {
            prectenyText = File.ReadAllText(cestaKSouboru);
        }

        char[] text = prectenyText.ToCharArray();
        if (text.Length == 0)
        {
            Console.WriteLine(0);
            return;
        }

        // Vychozi tabulka
        char[] nejlepsiTabulka = "HWFjDLERJMsu. ATUNm coSOfvatdneIeolriplwt ues CmcnqgtaurYiVbihPQ".ToCharArray();
        
        // Zjistim skore vychozi tabulky
        int nejlepsiCena = SpoctiCenuTabulky(nejlepsiTabulka, text);
        Console.WriteLine($"Vychozi tabulka ma cenu: {nejlepsiCena}");

        Random rnd = new Random();
        int pocetIteraci = 100000;

        Console.WriteLine("Spoustim Hill Climbing...");

        // Cyklus na optimalizaci momentalni tabulky
        for (int i = 0; i < pocetIteraci; i++)
        {
            // Vytvorim kopii momentalne nejlepsi tabulky
            char[] zkoumanaTabulka = (char[])nejlepsiTabulka.Clone();
            
            // Vyberu dva nahodne indexy a prohodim jejich znaky
            int index1 = rnd.Next(64);
            int index2 = rnd.Next(64);
            
            char temp = zkoumanaTabulka[index1];
            zkoumanaTabulka[index1] = zkoumanaTabulka[index2];
            zkoumanaTabulka[index2] = temp;

            // Spocitam cenu teto nove tabulky
            int novaCena = SpoctiCenuTabulky(zkoumanaTabulka, text);

            // Pokud je lepsi ulozim ji misto puvodni
            if (novaCena < nejlepsiCena)
            {
                nejlepsiCena = novaCena;
                nejlepsiTabulka = zkoumanaTabulka;
                
                Console.WriteLine($"Nove zlepseni. Iterace {i}: {nejlepsiCena} stisku");
                Console.WriteLine($"Tabulka: \"{new string(nejlepsiTabulka)}\"");
            }
        }
        
        Console.WriteLine("\nKonec hledani. Nejlepsi nalezena tabulka:");
        Console.WriteLine($"\"{new string(nejlepsiTabulka)}\" s poctem stisku {nejlepsiCena}");
    }
    
    private int SpoctiCenuTabulky(char[] pismenaTabulky, char[] text)
    /*
     * Metoda vytvori slovnik a zapise pismena na spravnou pozici do slovniku
     */
    {
        int sirka = 8;
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

        return NajdiNejkratsiPosloupnost(pocatecniPismeno, pismena, text);
    }

    public int NajdiNejkratsiPosloupnost(Pismeno zacatek, Dictionary<char, List<Pismeno>> pismena, char[] text)
    /*
     * Metoda najde nejkratsi mozny pocet tahu pro napsani daneho textu a vrati vysledek
     */
    {
        Dictionary<Pismeno, int> aktualniStavy = new Dictionary<Pismeno, int>();
        aktualniStavy.Add(zacatek, 0);
        
        // Postupne prochazim poslopnost textu
        for (int i = 0; i < text.Length; i++)
        {
            char aktualniZnak = text[i];
            
            if (!pismena.ContainsKey(aktualniZnak)) continue; 
            
            List<Pismeno> zkoumanaPismena = pismena[aktualniZnak];
            Dictionary<Pismeno, int> dalsiStavy = new Dictionary<Pismeno, int>();
            
            // Pro kazde startovni pismeno najdu vzdalenosti do nasledujicich
            foreach (KeyValuePair<Pismeno, int> stav in aktualniStavy)
            {
                int aktualniRadek = stav.Key.Radek;
                int aktualniSloupec = stav.Key.Sloupec;
                int aktualniVzdalenost = stav.Value;
                
                // Pro kazde nyni zkoumane pismeno zjistim nejlepsi mozny pocet kroku, na ktery se tam dokazu dostat
                foreach (Pismeno pismeno in zkoumanaPismena)
                {
                    int radekPismena = pismeno.Radek;
                    int sloupecPismena = pismeno.Sloupec;
                    
                    // Výpočet Manhattanské vzdálenosti + 1 za potvrzení (Enter)
                    int vzdalenost = Math.Abs(aktualniRadek - radekPismena) + Math.Abs(aktualniSloupec - sloupecPismena) + 1;
                    int novaVzdalenost = vzdalenost + aktualniVzdalenost;

                    if (!dalsiStavy.ContainsKey(pismeno))
                    {
                        dalsiStavy.Add(pismeno, novaVzdalenost);
                    }
                    else if (dalsiStavy[pismeno] > novaVzdalenost)
                    {
                        dalsiStavy[pismeno] = novaVzdalenost;
                    }
                }
            }
            aktualniStavy = dalsiStavy;
        }
        return aktualniStavy.Values.Min();
    }
}