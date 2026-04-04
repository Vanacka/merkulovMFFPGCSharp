using System.Reflection;

namespace merkulovMFFPGCSharp.DU;

public static class Skladiste
/*
 * Staticka trida na ulozeni mapy skladiste
 */
{
    public static int DelkaMapy = 10;
    public static char[,] Mapa = new char[DelkaMapy, DelkaMapy];

    public static bool JeToZed(int x, int y)
    /*
     * Metoda zjisti jestli je dane policko zdi nebo ne
     */
    {
        if ((x > DelkaMapy - 1 || x < 0) || (y > DelkaMapy - 1 || y < 0)) return true;
        if (Mapa[y, x] == 'X') return true;
        return false;
    }
}
public class Skladnik
/*
 * Trida reprezentujici skladnika, rika mu jak se muze hybat
 */
{
    public int X;
    public int Y;

    public Skladnik(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    private bool MuzuSeHnout(int x, int y)
    /*
     * Zjisti jestli se na dane policko muze skladnik posunout
     */
    {
        if (Skladiste.JeToZed(x, y)) return false;
        return true;
    }

    public List<Tuple<int, int>> MoznePohyby()
    /*
     * Zjisti vsechny mozne pohyby, kam se muze skladnik posunout, vrati pouze smery pohybu
     */
    {
        List<Tuple<int, int>> pohyby = new List<Tuple<int, int>>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Math.Abs(i) != Math.Abs(j))
                {
                    if (MuzuSeHnout(X + i, Y + j))
                    {
                        Tuple<int, int> souradnice = new Tuple<int, int>(i, j);
                        pohyby.Add(souradnice);
                    }
                }
            }
        }
        return pohyby;
    }
}

public class Bedna
/*
 * Trida reprezentujici bednu, urucuje mozne pohyby bedny
 */
{
    public int X;
    public int Y;

    public Bedna(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool MuzuSeHnout(int x, int y)
    /*
     * Funkce zjisti zda-li se benda muze posunout na danou pozici nebo ne
     */
    {
        if (Skladiste.JeToZed(x, y)) return false;
        return true;
    }

}

public class StavSokobanu
/*
 * Trida si uchovava pozici skladnika a bedny aktualne zkoumaneho stavu
 */
{
    public Skladnik Skladnik;
    public Bedna Bedna;
    public int Vzdalenost;

    public StavSokobanu(Skladnik skladnik, Bedna bedna, int vzdalenost)
    {
        Skladnik = skladnik;
        Bedna = bedna;
        Vzdalenost = vzdalenost;
    }

    public List<StavSokobanu> VytvorMozneStavy()
    /*
     * Vytvori nove mozne stavy
     */
    {
        int pohybSkladnikaX;
        int pohybSkladnikaY;
        int pohybBednyX;
        int pohybBednyY;
        List<Tuple<int, int>> moznePohybySkladnika = Skladnik.MoznePohyby();
        List<StavSokobanu> noveStavy = new List<StavSokobanu>();

        Skladnik novySkladnik;
        StavSokobanu novyStavSokobanu;
        
        // Projdu vsechny smeru pohybu skladnika a pro ne urcim,
        // jestli skladnik narazi do bedny nebo ne
        foreach (Tuple<int, int> pohyb in moznePohybySkladnika)
        {
            int pohybX = pohyb.Item1;
            int pohybY = pohyb.Item2;
            pohybSkladnikaX = Skladnik.X + pohybX;
            pohybSkladnikaY = Skladnik.Y + pohybY;
            // Pokud narazi skladnik do bedny, musi mit bedna prostor k pohybu
            if (pohybSkladnikaX == Bedna.X && pohybSkladnikaY == Bedna.Y)
            {
                pohybBednyX = Bedna.X + pohybX;
                pohybBednyY = Bedna.Y + pohybY;
                // Pokud se muze bedna hnout vytvorim novy stav s novou pozici bedny a skladnika
                if (Bedna.MuzuSeHnout(pohybBednyX, pohybBednyY))
                {
                    novySkladnik = new Skladnik(pohybSkladnikaX, pohybSkladnikaY);
                    Bedna novaBedna = new Bedna(pohybBednyX, pohybBednyY);
                    novyStavSokobanu = new StavSokobanu(novySkladnik, novaBedna, Vzdalenost + 1);
                    noveStavy.Add(novyStavSokobanu);
                }
            }
            // Vytvorim novy stav pouze s novou pozici skladnika
            else
            {
                novySkladnik = new Skladnik(pohybSkladnikaX, pohybSkladnikaY);
                novyStavSokobanu = new StavSokobanu(novySkladnik, Bedna, Vzdalenost + 1);
                noveStavy.Add(novyStavSokobanu);
            }
        }
        return noveStavy;
    }

    public override bool Equals(object? obj)
    /*
     * Override metody na porovnani dvou stavu sokobanu
     */
    {
        if (obj is StavSokobanu)
        {
            StavSokobanu druhyStavSokobanu = (StavSokobanu)obj;
            return Skladnik.X == druhyStavSokobanu.Skladnik.X &&
                   Skladnik.Y == druhyStavSokobanu.Skladnik.Y &&
                   Bedna.X == druhyStavSokobanu.Bedna.X &&
                   Bedna.Y == druhyStavSokobanu.Bedna.Y;
        }
        return false;
    }

    public override int GetHashCode()
    /*
     * Override metody na pocitani hashe stavu sokobanu
     */
    {
        return HashCode.Combine(Skladnik.X, Skladnik.Y, Bedna.X, Bedna.Y);
    }
}

public class ProzkoumaneStavy
/*
 * Trida, ktera udrzuje jiz prozkoumane stavy abychom je nezkoumali znovu
 */
{
    public HashSet<StavSokobanu> Stavy;

    public ProzkoumaneStavy()
    {
        Stavy = new HashSet<StavSokobanu>();
    }
    
    public void PridejStav(StavSokobanu stavSokobanu)
    /*
     * Metoda prida stav sokobanu do mnoziny jiz prozkoumanych
     */
    {
        Stavy.Add(stavSokobanu);
    }

    public bool OsahujeStav(StavSokobanu stavSokobanu)
    /*
     * Metoda zjisti jestli tento stav uz byl zkoumany nebo ne
     */
    {
        return Stavy.Contains(stavSokobanu);
    }
}

public class SokobanCiliSkladnik
{
    public void Hlavni()
    /*
     * Metoda nacte a zpracuje vstup
     */
    {
        int bednaX = 0;
        int bednaY = 0;
        int cilX = 0;
        int cilY = 0;
        int skladnikX = 0;
        int skladnikY = 0;
        
        // Nactu mapu ze vstupu ulozim ji do 2D pole a ulozim souradnice bedny a skladnika
        for (int i = 0; i < Skladiste.DelkaMapy; i++)
        {
            char[] radek = Ctecka.PrectiARozdelRadekNaZnaky();
            for (int j = 0; j < Skladiste.DelkaMapy; j++)
            {
                if (radek[j] == 'B')
                {
                    bednaX = j;
                    bednaY = i;
                }
                else if (radek[j] == 'C')
                {
                    cilX = j;
                    cilY = i;
                }
                else if (radek[j] == 'S')
                {
                    skladnikX = j;
                    skladnikY = i;
                }
                Skladiste.Mapa[i, j] = radek[j];
            }
        }
        
        // Inicializuji pocatecni hodnoty
        Bedna pocatecniBedna = new Bedna(bednaX, bednaY);
        Skladnik pocatecniSkladnik = new Skladnik(skladnikX, skladnikY);
        StavSokobanu pocatecniStav = new StavSokobanu(pocatecniSkladnik, pocatecniBedna, 0);
        
        // Zavolam BFS aby nasel nejkratsi cestu pokud existuje
        int vzdalenost = BFS(pocatecniStav, cilX, cilY);
        // Vypisu vysledek
        Console.WriteLine(vzdalenost);
    }

    public int BFS(StavSokobanu pocatecniStav, int CilX, int CilY)
    /*
     * Metoda, ktera prozkouma ruzne stavy do sirky
     */
    {
        Queue<StavSokobanu> fronta = new Queue<StavSokobanu>();
        fronta.Enqueue(pocatecniStav);
        ProzkoumaneStavy prozkoumaneStavy = new ProzkoumaneStavy();
        prozkoumaneStavy.PridejStav(pocatecniStav);
        
        // Dokud fronta neni prazdna
        while (fronta.Count() > 0)
        {
            // Vyndam stav z fronty
            StavSokobanu praveZkoumany = fronta.Dequeue();
            
            int bednaX = praveZkoumany.Bedna.X;
            int bednaY = praveZkoumany.Bedna.Y;
            
            // Zjistim jestli bedna uz neni v cili
            if (bednaX == CilX && bednaY == CilY)
            {
                return praveZkoumany.Vzdalenost;
            }
            // Ziskam dalsi mozne stavy z aktulaniho
            List<StavSokobanu> sousedniStavy = praveZkoumany.VytvorMozneStavy();

            foreach (StavSokobanu stav in sousedniStavy)
            {
                // Pokud jsem tento stav jeste nezkoumal
                if (!prozkoumaneStavy.OsahujeStav(stav))
                {
                    prozkoumaneStavy.PridejStav(stav);
                    fronta.Enqueue(stav);
                }
            }
        }
        // Bednu nelze presunout do cile
        return -1;
    }
}