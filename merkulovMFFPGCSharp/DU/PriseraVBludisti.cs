namespace merkulovMFFPGCSharp.DU;

public enum Smer
{
    Nahoru,
    Dolu,
    Doleva,
    Doprava,
    Nic
}

public static class SmeryLogika
/*
 * Trida zpracovava enum smery
 */
{
    public static Smer ZnakNaSmer(char sipka)
    /*
     * Funkce prevede smer zapsany jako char na spravny smer a vrati ho
     */
    {
        switch (sipka)
        {
            case '^':
                return Smer.Nahoru;
            case 'v':
                return Smer.Dolu;
            case '<':
                return Smer.Doleva;
            case '>':
                return Smer.Doprava;
            default:
                return Smer.Nic;
        }
    }

    public static char SmerNaZnak(Smer smer)
    /*
     * Funkce prevede smer na podrebny znak (sipecku)
     */
    {
        switch (smer)
        {
            case Smer.Nahoru:
                return '^';
            case Smer.Dolu:
                return 'v';
            case Smer.Doleva:
                return '<';
            case Smer.Doprava:
                return '>';
            default:
                return ' ';
        }
    }

    public static (int x, int y) SmerNaPohyb(Smer smer)
    /*
     * Funkce prevede smer na potrebny smer pohybu
     */
    {
        switch (smer)
        {
            case Smer.Nahoru:
                return (0, -1);
            case Smer.Dolu:
                return (0, 1);
            case Smer.Doleva:
                return (-1, 0);
            case Smer.Doprava:
                return (1, 0);
            default:
                return (0, 0);
        }
    }

    public static Smer KamSeOtocimDoleva(Smer smer)
    /*
     * Funkce vrati vysledny smer po otoceni doleva
     */
    {
        switch (smer)
        {
            case Smer.Nahoru:
                return Smer.Doleva;
            case Smer.Dolu:
                return Smer.Doprava;
            case Smer.Doleva:
                return Smer.Dolu;
            case Smer.Doprava:
                return Smer.Nahoru;
            default:
                return Smer.Nic;
        }
    } 
    
    public static Smer KamSeOtocimDoprava(Smer smer)
        /*
         * Funkce vrati vysledny smer po otoceni doprava
         */
    {
        switch (smer)
        {
            case Smer.Nahoru:
                return Smer.Doprava;
            case Smer.Dolu:
                return Smer.Doleva;
            case Smer.Doleva:
                return Smer.Nahoru;
            case Smer.Doprava:
                return Smer.Dolu;
            default:
                return Smer.Nic;
        }
    }
}

public class Prisera
/*
 * Trida implementujici priseru
 */
{
    public int X;
    public int Y;
    public Smer AktualniSmer;
    public bool JduDopredu;
    private Bludiste _mapa;

    public Prisera(int x, int y, Smer smer, Bludiste mapa)
    {
        X = x;
        Y = y;
        AktualniSmer = smer;
        JduDopredu = false;
        _mapa = mapa;
    }
    
    private void OtocSeDoleva()
    /*
     * Funkce otoci priseru doleva
     */
    {
        Smer vyslednySmer = SmeryLogika.KamSeOtocimDoleva(AktualniSmer);
        AktualniSmer = vyslednySmer;
    }

    private void OtocSeDoprava()
    /*
     * Funkce ototci priseru doprava
     */
    {
        Smer vyslednySmer = SmeryLogika.KamSeOtocimDoprava(AktualniSmer);
        AktualniSmer = vyslednySmer;
    }

    private void PosunSeDopredu()
    /*
     * Funkce posune priseru dopredu
     */
    {
        (int xPosun, int yPosun) = SmeryLogika.SmerNaPohyb(AktualniSmer);
        X += xPosun;
        Y += yPosun;
    }

    private bool JeZlevaZed()
    /*
     * Funkce zjisti zda-li ma prisera zleva zed
     */
    {
        Smer levySmer = SmeryLogika.KamSeOtocimDoleva(AktualniSmer);
        (int xSmer, int ySmer) = SmeryLogika.SmerNaPohyb(levySmer);
        bool jeTamZed = _mapa.JeTamZed(X + xSmer, Y + ySmer);

        return jeTamZed;
    }

    private bool JeZpravaZed()
    /*
     * Funkce zjisti zda-li ma prisera zprava zed
     */
    {
        Smer pravySmer = SmeryLogika.KamSeOtocimDoprava(AktualniSmer);
        (int xSmer, int ySmer) = SmeryLogika.SmerNaPohyb(pravySmer);
        bool jeTamZed = _mapa.JeTamZed(X + xSmer, Y + ySmer);

        return jeTamZed;
    }

    private bool JePredemnouZed()
    /*
     * Funkce zjisti zda-li ma prisera pred sebou zed
     */
    {
        (int xSmer, int ySmer) = SmeryLogika.SmerNaPohyb(AktualniSmer);
        bool jeTamZed = _mapa.JeTamZed(X + xSmer, Y + ySmer);

        return jeTamZed;
    }
    
    public void ProvedTah()
    /*
     * Funkce provede tah prisery
     */
    {
        if (JduDopredu)
        {
            PosunSeDopredu();
            JduDopredu = false;
        }
        else if (!JeZpravaZed())
        {
            OtocSeDoprava();
            JduDopredu = true;
        }
        else if (!JePredemnouZed())
        {
            PosunSeDopredu();
            JduDopredu = false;
        }
        else
        {
            OtocSeDoleva();
            JduDopredu = false;
        }
    }
}

public class Bludiste
{
    public char[,] HraciPole;

    public Bludiste(int sirka, int vyska)
    {
        HraciPole = new char[vyska, sirka];
    }

    public bool JeTamZed(int x, int y)
    /*
     * Funkce zjisti jestli na danych souradnicich je zed nebo ne
     */
    {
        char znak = HraciPole[y, x];
        if (znak == 'X')
        {
            return true;
        }
        return false;
    }
}

public class PriseraVBludisti
{
    public void Hlavni()
    /*
     * Funkce nacte a zpracuje vstup a zavola hlavni algoritmus pro pohyb prisery
     */
    {
        const int pocetTahu = 20;
        
        // Nactu vysku a sirku herniho pole
        int sirka = Ctecka.PrectiCislo();
        int vyska = Ctecka.PrectiCislo();
        
        Bludiste bludiste = new Bludiste(sirka, vyska);
        int priseraX = 0;
        int priseraY = 0;
        Smer smerPrisery = Smer.Nic;

        // Nactu herni pole ze vstupu
        for (int i = 0; i < vyska; i++)
        {
            char[] radek = Ctecka.PrectiARozdelRadekNaZnaky();
            for (int j = 0; j < sirka; j++)
            {
                char znak = radek[j];
                if (znak == '>' || znak == '<' || znak == 'v' || znak == '^')
                {
                    priseraX = j;
                    priseraY = i;
                    smerPrisery = SmeryLogika.ZnakNaSmer(znak);
                    bludiste.HraciPole[i, j] = '.';
                }
                else
                {
                    bludiste.HraciPole[i, j] = radek[j];
                }
            }
        }
        
        // Vytvorim priseru
        Prisera prisera = new Prisera(priseraX, priseraY, smerPrisery, bludiste);

        for (int i = 0; i < pocetTahu; i++)
        {
            prisera.ProvedTah();
            VypisHraciPole(prisera.X, prisera.Y, prisera.AktualniSmer, bludiste, vyska, sirka);
        }
    }

    public void VypisHraciPole(int priseraX, int priseraY, Smer priseraOrientace, Bludiste bludiste, int vyska, int sirka)
    /*
     * Funkce vypise hraci pole s aktualni orientaci a pozici prisery
     */
    {
        for (int i = 0; i < vyska; i++)
        {
            for (int j = 0; j < sirka; j++)
            {
                char znak = bludiste.HraciPole[i, j];
                
                if (j == priseraX && i == priseraY)
                {
                    znak = SmeryLogika.SmerNaZnak(priseraOrientace);
                }
                Console.Write(znak);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}