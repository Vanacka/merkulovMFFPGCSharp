using System.Collections;

namespace merkulovMFFPGCSharp.DU;

public class Souradnice
/*
 * Trida, ktera uchovava souradnic na sachovnici a zaroven vzdalenost ke startu
 */
{
    public int Radek;
    public int Sloupec;
    public int VzdalenostOdStartu;
    public Souradnice Predchudce;

    public Souradnice(int r, int s, int v, Souradnice p)
    {
        Radek = r;
        Sloupec = s;
        VzdalenostOdStartu = v;
        Predchudce = p;
    }
}
public class CestaKralemPoSachovnici
{
    private int[,] sachovnice = new int[8, 8];
    private (int radek, int sloupec)[] smery = new (int, int)[] { (1, 0), (1, 1), ( 0, 1 ), ( -1, 1 ), ( -1, 0 ), ( -1, -1), ( 0, -1 ), (1, -1) };

    public void Hlavni()
    /*
     * Funkce osetri spravne nacteni vstupu a nasledne zavola BFS algoritmus,
     * ktery zjisti nejkratsi cestu k cili
     */
    {
        // Prectu kolik bude prekazek na vstupu
        int pocetPrekazek = Ctecka.PrectiCislo();
        
        int[] prekazka;
        int radekPrekazky = 0;
        int sloupecPrekazky = 0;
        
        // Nastavim prekazky jako -2
        for (int i = 0; i < pocetPrekazek; i++)
        {
            prekazka = Ctecka.PrectiCisla(2);
            radekPrekazky = prekazka[0] - 1;
            sloupecPrekazky = prekazka[1] - 1;
            sachovnice[radekPrekazky, sloupecPrekazky] = -2;
        }
        // Vytvorim souradnici zacatku se vzdalenosti 0
        prekazka = Ctecka.PrectiCisla(2);
        radekPrekazky = prekazka[0] - 1;
        sloupecPrekazky = prekazka[1] - 1;
        Souradnice zacatek = new Souradnice(radekPrekazky, sloupecPrekazky, 0, null);
        
        // Vytvorim souradnici konce se vzdalenosti -1
        prekazka = Ctecka.PrectiCisla(2);
        radekPrekazky = prekazka[0] - 1;
        sloupecPrekazky = prekazka[1] - 1;
        Souradnice cil = new Souradnice(radekPrekazky, sloupecPrekazky, -1, null);
        sachovnice[radekPrekazky, sloupecPrekazky] = -1;
        
        // Kdyz bude zacatek stejny jako cil nema smysl spoustet BFS
        if ((zacatek.Radek == cil.Radek) && (zacatek.Sloupec == cil.Sloupec))
        {
            Console.WriteLine(0);
        }
        else
        {
            int vzdalenost = BFS(zacatek, cil);
            if (vzdalenost == -1)
            {
                Console.WriteLine(vzdalenost);
            }
        }
    }

    private int BFS(Souradnice zacatek, Souradnice cil)
    /*
     * Pruchod do sirky po sachovnici kazdemu policku, do ktereho se dostane, priradi vzdalenost od zacatku 
     */
    {
        // Priradim do fronty pocatecni prvek
        Queue<Souradnice> fronta = new Queue<Souradnice>();
        fronta.Enqueue(zacatek);
        
        while (fronta.Count > 0)
        {
            // Vyberu prvni prvek z fronty
            Souradnice zkoumany =  fronta.Dequeue();
            int aktualniVzdalenost = zkoumany.VzdalenostOdStartu;
            int radek = zkoumany.Radek;
            int sloupec = zkoumany.Sloupec;
            
            // Pokud jsem v cili vypisu vzdalenost k nemu
            if (sachovnice[radek, sloupec] == -1)
            {
                sachovnice[radek, sloupec] = aktualniVzdalenost;
                VypisCestu(zkoumany);
                return aktualniVzdalenost;
            }
            
            // Zjisitim vsechny policka, do kterych se mohu dostat z aktualniho
            List<Souradnice> mozniSousedi = MozniSousedi(zkoumany);
            
            // Projdu vsechny sousedy, pridam je do fronty a zmenim vzdalenost na sachovnici
            foreach (Souradnice soused in mozniSousedi)
            {
                fronta.Enqueue(soused);
                int r = soused.Radek;
                int s = soused.Sloupec;
                // Pokud je soused cil nemenim jeho hodnotu na sachovnici
                if (sachovnice[r, s] != -1)
                {
                    sachovnice[r, s] = soused.VzdalenostOdStartu;
                }
            }
        }
        return cil.VzdalenostOdStartu;
    }

    List<Souradnice> MozniSousedi(Souradnice zkoumany)
    /*
     * Funkce najde vsechny mozne sousedy zkoumaneho policka,
     * neboli na jaka policka se kral muze pohnout jednim tahem z aktualne zkoumaneho
     */
    {
        List<Souradnice> sousede = new List<Souradnice>(); 
        
        // Iteruji pres vsechny mozne smery
        for (int i = 0; i < 8; i++)
        {
            int souradniceRadku = smery[i].radek + zkoumany.Radek;
            int souradniceSloupce = smery[i].sloupec + zkoumany.Sloupec;
            int vzdalenostOdStartu = zkoumany.VzdalenostOdStartu;
            // Pokud se prictenim smeru nedostanu mimo rozmery sachovnice
            if ((0 <= souradniceRadku && souradniceRadku < 8) && (0 <= souradniceSloupce && souradniceSloupce < 8))
            {
                // Pokud se prictenim smeru dostanu na policko, kde neni prekazka
                if ((sachovnice[souradniceRadku, souradniceSloupce] == -1) || (sachovnice[souradniceRadku, souradniceSloupce] == 0))
                {
                    Souradnice soused = new Souradnice(souradniceRadku, souradniceSloupce, vzdalenostOdStartu + 1, zkoumany);
                    sousede.Add(soused);
                }
            }
        }
        return sousede;
    }

    void VypisCestu(Souradnice cil)
    /*
     * Funkce vypise nejkratsi cestu krale od startu k cili
     */
    {
        List<Souradnice> souradniceCesty = new List<Souradnice>();
        Souradnice zkoumana = cil;
        int radek = 0;
        int sloupec = 0;
        
        while (zkoumana != null)
        {
            souradniceCesty.Add(zkoumana);
            zkoumana = zkoumana.Predchudce;
        }

        for (int i = souradniceCesty.Count - 1; i >= 0; i--)
        {
            radek = souradniceCesty[i].Radek;
            sloupec = souradniceCesty[i].Sloupec;
            Console.WriteLine($"{radek + 1} {sloupec + 1}");
        }
    }
}