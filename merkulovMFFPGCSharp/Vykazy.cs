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
        string mesicAktivity;

        Dictionary<string, int> casPoMesicich = new Dictionary<string, int>();

        while (true)
        {
            // Pokud je prazdny radek
            if (radek.Length == 0)
            {
                radek = Ctecka.PrectiRadek();
                continue;
            }
            // Pokud radek obsahuje pouze tecku
            if (radek.Length == 1 && radek[0] == ".")
            {
                break;
            }
            if (JeValidiniRadek(radek))
            {
                casAktivity = CasNaSekundy(radek[^1]) - CasNaSekundy(radek[1]);
                mesicAktivity = ZjistiMesicARok(radek[0]);
                if (casPoMesicich.ContainsKey(mesicAktivity))
                {
                    casPoMesicich[mesicAktivity] += casAktivity;
                }
                else
                {
                    casPoMesicich.Add(mesicAktivity, casAktivity);
                }
                celkovyCas += casAktivity;
            }

            radek = Ctecka.PrectiRadek();
        }

        foreach (KeyValuePair<string, int> keyValuePair in casPoMesicich)
        {
            string mesic = keyValuePair.Key;
            string casMesicniAktivity = SekundyNaCas(keyValuePair.Value);
            Console.WriteLine($"{mesic}: {casMesicniAktivity}");
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

    private string ZjistiMesicARok(string datum)
    /*
     * Funkce dostane datum a z neho zjisti mesic
     */
    {
        string[] rozdeleneDatum = datum.Split('.');
        string mesic = rozdeleneDatum[1];
        string rok = rozdeleneDatum[2];

        return $"{mesic}/{rok}";
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