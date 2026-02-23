namespace merkulovMFFPGCSharp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(DruheNejvetsiCislo());
    }

    static int DruheNejvetsiCislo()
    {
        int max1 = Ctecka.PrectiCislo();
        int akt = Ctecka.PrectiCislo();
        int max2 = 0;

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
}