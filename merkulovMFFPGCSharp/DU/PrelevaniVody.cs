namespace merkulovMFFPGCSharp.DU;

public struct Nadoba
/*
 * Struktura, ktera uchovava aktualni stav jedne nadoby
 */
{
    public int Objem;
    public int AktualniObjem;

    public Nadoba(int objem, int aktualniObjem)
    {
        Objem = objem;
        AktualniObjem = aktualniObjem;
    }
}

public class StavPreliti
/*
 * Uchovava aktualni stav nadob
 */
{
    public Nadoba a;
    public Nadoba b;
    public Nadoba c;
    public int pocetPreliti;

    public StavPreliti(int objemA, int aktualniObjemA, int objemB, int aktualniObjemB, int objemC, int aktualniObjemC, int preliti)
    {
        a = new Nadoba(objemA, aktualniObjemA);
        b = new Nadoba(objemB, aktualniObjemB);
        c = new Nadoba(objemC, aktualniObjemC);
        pocetPreliti = preliti;
    }

    public List<StavPreliti> ZiskejDalsiStavy()
    /*
     * Metoda zjisti vsechny dalsi mozne stavy, ktere mohu ziskat
     */
    {
        // Vytvorim list stavu nadob a pomocne pole na objemy
        List<StavPreliti> dalsiStavy = new List<StavPreliti>();
        Nadoba[] noveNadoby = {a, b, c};
        int[] noveObjemy = new int[3];
        
        // Iteruji 2 cykly abych mohl ziskat vsech 6 kombinaci stavu
        for (int i = 0; i < noveNadoby.Length; i++)
        {
            for (int j = 0; j < noveNadoby.Length; j++)
            {
                // Abych nepreleval ze stejne do stejne
                if (j != i)
                {
                    // Nastavim prozatimni potrebne objemy
                    int aktualniObjemDostavajici = noveNadoby[j].AktualniObjem;
                    int objemDostavajici = noveNadoby[j].Objem;
                    int aktualniObjemPredavajici = noveNadoby[i].AktualniObjem;
                    
                    // Pokud muzu prelevat a v prelevajici nadobe neni 0
                    if (aktualniObjemDostavajici != objemDostavajici && aktualniObjemPredavajici > 0)
                    {
                        noveObjemy[0] = a.AktualniObjem;
                        noveObjemy[1] = b.AktualniObjem;
                        noveObjemy[2] = c.AktualniObjem;

                        int kolikSeVejde = objemDostavajici - aktualniObjemDostavajici;
                        int kolikPrelejeme = Math.Min(kolikSeVejde, aktualniObjemPredavajici);

                        noveObjemy[i] -= kolikPrelejeme;
                        noveObjemy[j] += kolikPrelejeme;

                        StavPreliti novyStavPreliti = new StavPreliti(a.Objem, noveObjemy[0], b.Objem, noveObjemy[1], c.Objem,
                            noveObjemy[2], pocetPreliti + 1);
                        dalsiStavy.Add(novyStavPreliti);
                    }
                }
            }
        }
        return dalsiStavy;
    }

    public override int GetHashCode()
    /*
     * Vlastni upravena metoda na pocitani hashe objektu, protoze nechci aby se mi vytvarel ten hash z parametru pocetPreliti
     */
    {
        return HashCode.Combine(a.AktualniObjem, b.AktualniObjem, c.AktualniObjem);
    }

    public override bool Equals(object? obj)
    /*
     * Vlastni metoda Equals na porovnavani Stavu aby se neporovnavali v parametru pocetPreliti
     */
    {
        if (obj is StavPreliti)
        {
            StavPreliti druhyStavPreliti = (StavPreliti)obj;
            return a.AktualniObjem == druhyStavPreliti.a.AktualniObjem &&
                   b.AktualniObjem == druhyStavPreliti.b.AktualniObjem &&
                   c.AktualniObjem == druhyStavPreliti.c.AktualniObjem;
        }
        return false;
    }
}
 
public class ZkoumaneStavy
/*
 * Udrzuje si jiz prozkoumane stavy abychom je zbytecne nezkoumali znovu
 */
{
    public HashSet<StavPreliti> prozkoumaneStavy;

    public ZkoumaneStavy()
    {
        prozkoumaneStavy = new HashSet<StavPreliti>();
    }

    public void pridejStav(StavPreliti stavPreliti)
    {
        prozkoumaneStavy.Add(stavPreliti);
    }

    public bool obsahujeStav(StavPreliti stavPreliti)
    {
        return prozkoumaneStavy.Contains(stavPreliti);
    }
}

public class MozneObjemy
/*
 * Trida, ktera uchovava jak nejlepe dosahnout nejakeho objemu a jestli ho vubec muzeme dostat pokud ne ma hodnotu -1
 */
{
    public int[] nejlepsiObjemy;

    public MozneObjemy(int nejvetsiMoznyObjem)
    {
        nejlepsiObjemy = new int[nejvetsiMoznyObjem];
        for (int i = 0; i < nejvetsiMoznyObjem; i++)
        {
            nejlepsiObjemy[i] = int.MaxValue;
        }
    }

    public void zmenObjem(int objem, int pocetPreliti)
    /*
     * Funkce zmeni pocet preliti pro urcity objem na aktualni pokud je mensi nez dosavadni
     */
    {
        if (nejlepsiObjemy[objem] > pocetPreliti)
        {
            nejlepsiObjemy[objem] = pocetPreliti;
        } 
    }
}

public class PrelevaniVody
{
    public void Hlavni()
        /*
         * Nacte a zpracuje vstup
         */
    {
        // Nactu vstupni hodnoty a priradim je do spravnych promennych
        int[] objemyNadob = Ctecka.PrectiCisla(3);
        int[] objemyVNadobach = Ctecka.PrectiCisla(3);
        int a = objemyNadob[0];
        int b = objemyNadob[1];
        int c = objemyNadob[2];
        int x = objemyVNadobach[0];
        int y = objemyVNadobach[1];
        int z = objemyVNadobach[2];
        
        // Vytvorim pocatecni stav
        StavPreliti pocatecniStavPreliti = new StavPreliti(a, x, b, y, c, z, 0);
        
        // Zjistim jakeho nejvetsiho objemu v nadobach muzeme dosahnout proto abych zjistil rozsah zkoumanych hodnot
        int nejvetsiMoznyObjem = 0;
        int celkovyAktualniObjem = x + y + z;
        int maxObjem = NejvetsiZeTriObjemu(a, b, c);
        if (maxObjem > celkovyAktualniObjem)
        {
            nejvetsiMoznyObjem = celkovyAktualniObjem;
        }
        else
        {
            nejvetsiMoznyObjem = maxObjem;
        }
        
        // Inicializuji si seznam moznych objemu
        MozneObjemy mozneObjemy = new MozneObjemy(nejvetsiMoznyObjem + 1);
        mozneObjemy.zmenObjem(x, 0);
        mozneObjemy.zmenObjem(y, 0);
        mozneObjemy.zmenObjem(z, 0);
        
        // Najdu vsechny varianty preliti
        BFS(pocatecniStavPreliti, mozneObjemy);
        
        // Vytisknu konecne objemy
        VytiskniObjemy(mozneObjemy);
    }

    private void VytiskniObjemy(MozneObjemy mozneObjemy)
    /*
     * Vytiskne nejlepsi mozne varianty objemu
     */
    {
        for (int i = 0; i < mozneObjemy.nejlepsiObjemy.Length; i++)
        {
            if (mozneObjemy.nejlepsiObjemy[i] != int.MaxValue)
            {
                Console.Write($"{i}:{mozneObjemy.nejlepsiObjemy[i]} ");
            }
        }
    }

    private int NejvetsiZeTriObjemu(int a, int b, int c)
    /*
     * Dostane tri objemy porovna je a vrati nejvetsi z nich
     */
    {
        int maxObjem = 0;
        if (a > b)
        {
            maxObjem = a;
            if (c > a)
            {
                maxObjem = c;
            }
        }
        else
        {
            maxObjem = b;
            if (c > b)
            {
                maxObjem = c;
            }
        }

        return maxObjem;
    }

    private void BFS(StavPreliti pocatecniStavPreliti, MozneObjemy mozneObjemy)
    /*
     * Metoda, ktera prozkoumava ruzne stavy do sirky
     */
    {
        Queue<StavPreliti> frontaStavu = new Queue<StavPreliti>();
        frontaStavu.Enqueue(pocatecniStavPreliti);
        ZkoumaneStavy prozkoumaneStavy = new ZkoumaneStavy();
        prozkoumaneStavy.pridejStav(pocatecniStavPreliti);
        
        // Dokud fronta neni prazdna
        while (frontaStavu.Count > 0)
        {
            // Vyndam prvni stav z fronty
            StavPreliti praveZkoumany = frontaStavu.Dequeue();
            // Pridam stav do mnoziny stavu abych ho uz nepridaval do fronty
            prozkoumaneStavy.pridejStav(praveZkoumany);
            int x = praveZkoumany.a.AktualniObjem;
            int y = praveZkoumany.b.AktualniObjem;
            int z = praveZkoumany.c.AktualniObjem;
            int pocetPreliti = praveZkoumany.pocetPreliti;
            
            // Zmeni objem pokud jsem ho dostal za lepsi pocet preliti nez predtim
            mozneObjemy.zmenObjem(x, pocetPreliti);
            mozneObjemy.zmenObjem(y, pocetPreliti);
            mozneObjemy.zmenObjem(z, pocetPreliti);
            // Ziskam dalsi stavy po preliti
            List<StavPreliti> sousedniStavy = praveZkoumany.ZiskejDalsiStavy();

            foreach (StavPreliti stav in sousedniStavy)
            {
                if (!prozkoumaneStavy.obsahujeStav(stav))
                {
                    frontaStavu.Enqueue(stav);
                }
            }
        }
    }
}