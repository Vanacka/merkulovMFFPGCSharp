namespace merkulovMFFPGCSharp;

public class PraceSCisly
{
    public static int DruheNejvetsiCislo()
    {
        int max1 = Ctecka.PrectiCislo();
        int akt = Ctecka.PrectiCislo();
        int max2 = akt;

        while (akt != -1)
        {
            if (akt > max1)
            {
                max2 = max1;
                max1 = akt;
            }
            else if (akt > max2)
            {
                max2 = akt;
            }
            akt = Ctecka.PrectiCislo();
        }
        return max2;
    }
    
    public static int DruhaNejvetsiHodnota()
    {
        int max1 = Ctecka.PrectiCislo();
        int max2 = Ctecka.PrectiCislo();

        while (max2 == max1)
        {
            max2 = Ctecka.PrectiCislo();
        }
        if  (max2 > max1)
        {
            (max1, max2) = (max2, max1);
        }
        
        int akt = Ctecka.PrectiCislo();

        while (akt != -1)
        {
            if (akt > max1)
            {
                max2 = max1;
                max1 = akt;
            }
            else if (akt > max2 && akt != max1)
            {
                max2 = akt;
            }
            akt = Ctecka.PrectiCislo();
        }
        return max2;
    }
}