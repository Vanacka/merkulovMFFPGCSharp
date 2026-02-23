namespace merkulovMFFPGCSharp;

public class Ctecka
{
    public static int PrectiCislo()
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
}