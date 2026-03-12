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

public class Stav
/*
 * Uchovava aktualni stav nadob
 */
{
    public Nadoba a;
    public Nadoba b;
    public Nadoba c;
    public int pocetPreliti;

    public Stav(int objemA, int aktualniObjemA, int objemB, int aktualniObjemB, int objemC, int aktualniObjemC, int preliti)
    {
        a = new Nadoba(objemA, aktualniObjemA);
        b = new Nadoba(objemB, aktualniObjemB);
        c = new Nadoba(objemC, aktualniObjemC);
        pocetPreliti = preliti;
    }
    
    private int[] PrelejAdoB(int aktualniObjemA, int aktualniObjemB, int objemB)
        /*
         * Metoda dostane parametry nadoby A a nabody B preleje z A do B a vrati vysledne objemy obou z nich
         */
    {
        int[] aktualniObjemyAB = new int[2];
        int kolikSeVejdeDoB = objemB - aktualniObjemB;
        if (aktualniObjemA > kolikSeVejdeDoB)
        {
            aktualniObjemB = objemB;
            aktualniObjemA -= kolikSeVejdeDoB;
        }
        else
        {
            aktualniObjemB += aktualniObjemA;
            aktualniObjemA = 0;
        }

        aktualniObjemyAB[0] = aktualniObjemA;
        aktualniObjemyAB[1] = aktualniObjemB;

        return aktualniObjemyAB;
    }

    public List<Stav> ZiskejDalsiStavy()
    /*
     * Metoda zjisti vsechny dalsi mozne stavy, ktere mohu ziskat
     */
    {
        List<Stav> dalsiStavy = new List<Stav>();
        int[] objemy;
        
        // Preliti z A do B
        if (b.AktualniObjem != b.Objem && a.AktualniObjem > 0)
        {
            objemy = PrelejAdoB(a.AktualniObjem, b.AktualniObjem, b.Objem);
            Stav prelitiAdoB = new Stav(a.Objem, objemy[0], b.Objem, objemy[1], c.Objem, c.AktualniObjem,
                pocetPreliti + 1);
            dalsiStavy.Add(prelitiAdoB);   
        }
        
        // Preliti z A do C
        if (c.AktualniObjem != c.Objem && a.AktualniObjem > 0)
        {
            objemy = PrelejAdoB(a.AktualniObjem, c.AktualniObjem, c.Objem);
            Stav prelitiAdoC = new Stav(a.Objem, objemy[0], b.Objem, b.AktualniObjem, c.Objem, objemy[1],
                pocetPreliti + 1);
            dalsiStavy.Add(prelitiAdoC);
        }
        
        // Preliti z B do C
        if (c.AktualniObjem != c.Objem && b.AktualniObjem > 0)
        {
            objemy = PrelejAdoB(b.AktualniObjem, c.AktualniObjem, c.Objem);
            Stav prelitiBdoC = new Stav(a.Objem, a.AktualniObjem, b.Objem, objemy[0], c.Objem, objemy[1],
                pocetPreliti + 1);
            dalsiStavy.Add(prelitiBdoC);
        }

        // Preliti z C do B
        if (b.AktualniObjem != b.Objem && c.AktualniObjem > 0)
        {
            objemy = PrelejAdoB(c.AktualniObjem, b.AktualniObjem, b.Objem);
            Stav prelitiCdoB = new Stav(a.Objem, a.AktualniObjem, b.Objem, objemy[1], c.Objem, objemy[0],
                pocetPreliti + 1);
            dalsiStavy.Add(prelitiCdoB);
        }

        // Preliti z C do A
        if (a.AktualniObjem != a.Objem && c.AktualniObjem > 0)
        {
            objemy = PrelejAdoB(c.AktualniObjem, a.AktualniObjem, a.Objem);
            Stav prelitiCdoA = new Stav(a.Objem, objemy[1], b.Objem, b.AktualniObjem, c.Objem, objemy[0],
                pocetPreliti + 1);
            dalsiStavy.Add(prelitiCdoA);
        }

        // Preliti z B do A
        if (a.AktualniObjem != a.Objem && b.AktualniObjem > 0)
        {
            objemy = PrelejAdoB(b.AktualniObjem, a.AktualniObjem, a.Objem);
            Stav prelitiBdoA = new Stav(a.Objem, objemy[1], b.Objem, objemy[0], c.Objem, c.AktualniObjem,
                pocetPreliti + 1);
            dalsiStavy.Add(prelitiBdoA);
        }
        return dalsiStavy;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(a.AktualniObjem, b.AktualniObjem, c.AktualniObjem);
    }

    public override bool Equals(object obj)
    /*
     * Vlastni metoda Equals na porovnavani Stavu aby se neporovnavali v parametru pocetPreliti
     */
    {
        if (obj is Stav)
        {
            Stav druhyStav = (Stav)obj;
            return a.AktualniObjem == druhyStav.a.AktualniObjem &&
                   b.AktualniObjem == druhyStav.b.AktualniObjem &&
                   c.AktualniObjem == druhyStav.c.AktualniObjem;
        }
        return false;
    }
}
 
public class ZkoumaneStavy
/*
 * Udrzuje si jiz prozkoumane stavy abychom je zbytecne nezkoumali znovu
 */
{
    public HashSet<Stav> prozkoumaneStavy;

    public ZkoumaneStavy()
    {
        prozkoumaneStavy = new HashSet<Stav>();
    }

    public void pridejStav(Stav stav)
    {
        prozkoumaneStavy.Add(stav);
    }

    public bool obsahujeStav(Stav stav)
    {
        return prozkoumaneStavy.Contains(stav);
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
        Stav pocatecniStav = new Stav(a, x, b, y, c, z, 0);
        
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
        BFS(pocatecniStav, mozneObjemy);
        
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

    private void BFS(Stav pocatecniStav, MozneObjemy mozneObjemy)
    /*
     * Metoda, ktera prozkoumava ruzne stavy do sirky
     */
    {
        Queue<Stav> frontaStavu = new Queue<Stav>();
        frontaStavu.Enqueue(pocatecniStav);
        ZkoumaneStavy prozkoumaneStavy = new ZkoumaneStavy();
        prozkoumaneStavy.pridejStav(pocatecniStav);

        while (frontaStavu.Count > 0)
        {
            Stav praveZkoumany = frontaStavu.Dequeue();
            prozkoumaneStavy.pridejStav(praveZkoumany);
            int x = praveZkoumany.a.AktualniObjem;
            int y = praveZkoumany.b.AktualniObjem;
            int z = praveZkoumany.c.AktualniObjem;
            int pocetPreliti = praveZkoumany.pocetPreliti;
            
            mozneObjemy.zmenObjem(x, pocetPreliti);
            mozneObjemy.zmenObjem(y, pocetPreliti);
            mozneObjemy.zmenObjem(z, pocetPreliti);
            List<Stav> sousedniStavy = praveZkoumany.ZiskejDalsiStavy();

            foreach (Stav stav in sousedniStavy)
            {
                if (!prozkoumaneStavy.obsahujeStav(stav))
                {
                    frontaStavu.Enqueue(stav);
                }
            }
        }
    }
}