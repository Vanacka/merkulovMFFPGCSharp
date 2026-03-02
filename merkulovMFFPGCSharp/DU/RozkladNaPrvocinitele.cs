using System.Collections;

namespace merkulovMFFPGCSharp.DU;

public class RozkladNaPrvocinitele
{
    /*
    static void Main(string[] args)
    {
        int n = Ctecka.PrectiCislo();

        Console.Write(n + "=");
        RozkladNaPrvocisla(n);
    }
    */

    static void RozkladNaPrvocisla(int n)
    {
        int puvodni = n;
        int i = 2;
        bool vypsalo = false;
        
        while (i*i <= n)
        {
            if (n % i == 0)
            {
                vypsalo = true;
                n /= i;
                Console.Write(i + "*");
            }
            else
            {
                i++;   
            }
        }
        if (!vypsalo) Console.Write(puvodni);
        else Console.Write(n);
    }
}