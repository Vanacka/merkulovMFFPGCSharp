using System.Collections;

namespace merkulovMFFPGCSharp;

public class Stromecky
{
    public char[,] les;

    public Stromecky(char[,] les)
    {
        this.les = les;
    }
    public void Ker(int radek, int sloupec, int k)
    {
        /*
         * zapise ker od spravnych souradnic do celkoveho pole les
         */
        for (int i = 1; i < k + 1; i++)
        {
            int pocetTecek = k - i;
            int pocetHvezdicek = i * 2 - 1;
            
            PridejZnak(radek + i - 1, sloupec + pocetTecek, pocetHvezdicek, '*');
        }
    }

    public void Strom(int radek, int sloupec, int k, int l)
    {
        /*
         * zapise strom od spravnych souradnic do celkoveho pole les
         */
        Ker(radek, sloupec, k);
        int pocetTecek = k - 1;

        for (int i = k; i < k + l; i++)
        {
            PridejZnak(radek + i, sloupec + pocetTecek,  1, '*');
        }
    }

    private void PridejZnak(int radek, int sloupec, int pocet, char znak)
    {
        /*
         * zapise urcity znak do celkoveho pole les
         */
        for (int i = 0; i < pocet; i++)
        {
            les[radek, sloupec + i] = znak;
        }
    }
}