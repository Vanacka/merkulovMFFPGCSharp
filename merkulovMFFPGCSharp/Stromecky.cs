namespace merkulovMFFPGCSharp;

public class Stromecky
{
    public static void Ker(int k)
    {
        for (int i = 1; i < k + 1; i++)
        {
            int pocetTecek = k - i;
            int pocetHvezdicek = i * 2 - 1;
            
            for (int j = 0; j < pocetTecek; j++)
            {
                Console.Write(".");
            }
            for (int j = 0; j < pocetHvezdicek; j++)
            {
                Console.Write("*");
            }
            for (int j = 0; j < pocetTecek; j++)
            {
                Console.Write(".");
            }
            Console.WriteLine();
        }
    }

    public static void Strom(int k, int l)
    {
        Ker(k);

        int pocetTecek = k - 1;

        for (int i = 0; i < l; i++)
        {
            for (int j = 0; j < pocetTecek; j++)
            {
                Console.Write(".");
            }
            Console.Write("*");
            for (int j = 0; j < pocetTecek; j++)
            {
                Console.Write(".");
            }
            Console.WriteLine();
        }
    }

    public static void Les()
    {
        
    }
}