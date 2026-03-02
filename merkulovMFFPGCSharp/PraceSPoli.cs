namespace merkulovMFFPGCSharp;

public class PraceSPoli
{
    static void Main(string[] args)
    {
        VypisSudaCislaAUlozLicha();
    }

    static void VypisPoleOpacne(int[] a, int posledniCisloVPoli)
    {
        for (int i = posledniCisloVPoli; i >= 0; i--)
        {
            Console.Write(a[i] + " ");
        }
    }

    static void VypisSudaCislaAUlozLicha()
    {
        int[] poleLichych = new int[1000];
        int aktualni = Ctecka.PrectiCislo();


        int i = 0;
        
        while (aktualni != -1)
        {
            if (aktualni % 2 == 0) Console.Write(aktualni + " ");
            else
            {
                poleLichych[i] = aktualni;
                i++;
            }
            aktualni = Ctecka.PrectiCislo();
        }
        VypisLichaCisla(poleLichych, i);
    }

    static void VypisLichaCisla(int[] a, int posledniCisloVPoli)
    {
        for (int i = 0; i < posledniCisloVPoli; i++)
        {
            Console.Write(a[i] + " ");
        }
    }
}