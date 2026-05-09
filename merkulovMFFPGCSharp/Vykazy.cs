namespace merkulovMFFPGCSharp;

public class Vykazy
{
    public void Hlavni()
    /*
     * Funkce zpracuje vstup a vypise vysledek
     */
    {
        string[] radek = Ctecka.PrectiRadek();
        int celkovyCas = 0;
        int casAktivity;

        while (true)
        {
            if (radek.Length == 0)
            {
                radek = Ctecka.PrectiRadek();
                continue;
            }

            if (radek[0] == ".")
            {
                break;
            }
            if (JeValidiniRadek(radek))
            {
                casAktivity = CasNaSekundy(radek[^1]) - CasNaSekundy(radek[1]);
                celkovyCas += casAktivity;
            }

            radek = Ctecka.PrectiRadek();
        }

        Console.WriteLine($"celkem: {SekundyNaCas(celkovyCas)}");
    }

    private int CasNaSekundy(string cas)
    /*
     * Funkce prevede cas na sekundy
     */
    {
        string[] rozdelenyCas = cas.Split(':');

        int hodiny = int.Parse(rozdelenyCas[0]);
        int minuty = int.Parse(rozdelenyCas[1]);
        int sekundy = int.Parse(rozdelenyCas[2]);

        sekundy = sekundy + (minuty * 60) + (hodiny * 3600);
        
        return sekundy;
    }

    private string SekundyNaCas(int celyCasSekundy)
    /*
     * Funkce prevede sekundy na cas
     */
    {
        int sekundy = celyCasSekundy % 60;
        celyCasSekundy /= 60;
        string formatovaneSekundy = NaformatujCas(sekundy);
        
        int minuty = celyCasSekundy % 60;
        celyCasSekundy /= 60;
        string formatovaneMinuty = NaformatujCas(minuty);
        
        int hodiny = celyCasSekundy;
        return $"{hodiny}:{formatovaneMinuty}:{formatovaneSekundy}";
    }

    private string NaformatujCas(int jednotkaCasu)
    {
        string naformatovanaJednotka = "";
        if (jednotkaCasu < 10)
        {
            naformatovanaJednotka = $"0{jednotkaCasu}";
        }
        else
        {
            naformatovanaJednotka = $"{jednotkaCasu}";
        }

        return naformatovanaJednotka;
    }
    
    private bool JeValidiniRadek(string[] radek)
    /*
     * Funkce overi zda-li radek je validni nebo ne
     */
    {
        if (radek.Length < 5) return false;
        if (radek[0].Equals(radek[^2])) return true;
        return false;
    }
}