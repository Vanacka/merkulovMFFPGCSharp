namespace merkulovMFFPGCSharp;

public class Ctecka
{
    public static int PrectiCislo()
    /*
     * Precte jedno cislo
     */
    {
        int znak = Console.Read();
        bool zaporne = false;
        
        // preskocit nezajimave znaky:
        while (znak < '0' || znak > '9')
        {
            if (znak == '-') zaporne = true;
            else zaporne = false;
            znak = Console.Read();
        }
        
        // cist cislo
        int x = 0;
        while (znak >= '0' && znak <= '9')
        {
            x = 10 * x + znak - '0';
            znak = Console.Read();
        }

        if (zaporne) x = -x;
        return x;
    }

    public static int[] PrectiCisla(int pocetCisel)
    /*
     * Precte zadany pocet cisel
     */
    {
        int[] prectenaCisla = new int[pocetCisel];
        for (int i = 0; i < pocetCisel; i++)
        {
            prectenaCisla[i] = PrectiCislo();
        }
        return prectenaCisla;
    }
}