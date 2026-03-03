namespace merkulovMFFPGCSharp;

class StromeckyMain
{
   public static void Main(string[] args)
    {
        int maxSirka = 0;
        int maxVyska = 0;
        List<int[]> hodnoty = new List<int[]>();
        
        // nactu vsechny radky a zjistim maximalni sirku a vysku lesu
        string? vstup;
        while (!String.IsNullOrWhiteSpace(vstup=Console.ReadLine()))
        {
            string[] cisla = vstup.Split(' ');
            int x = int.Parse(cisla[0]);
            int y = int.Parse(cisla[1]);
            int k = int.Parse(cisla[2]);
            int l = int.Parse(cisla[3]);
            int[] souradnice = new int[4];
            souradnice[0] = x;
            souradnice[1] = y;
            souradnice[2] = k;
            souradnice[3] = l;
            hodnoty.Add(souradnice);
            maxSirka = Math.Max(maxSirka, x + (2*k - 1));
            maxVyska = Math.Max(maxVyska, y + k + l);
        }
        // vytvorim pole charu a vsechny znaky nastavim na '.'
        char[,] les = new char[maxVyska + 1, maxSirka + 1];
        for (int i = 0; i < maxVyska; i++)
        {
            for (int j = 0; j < maxSirka; j++)
            {
                les[i, j] = '.';
            }
        }
        
        // vytvorim instanci tridy Stromecky a poslu ji pole spravnych parametru do konstruktoru
        Stromecky stromecky = new Stromecky(les);
        
        // vyhodnotim kazdy strom a pridam potrebne znaky do lesa
        foreach (int[] cisla in hodnoty)
        {
            int sloupec = cisla[0];
            int radek = cisla[1];
            int k = cisla[2];
            int l = cisla[3];
            stromecky.Strom(radek, sloupec, k, l);
        }
        
        // vypisu na vystup hotovy les
        for (int i = 0; i < maxVyska; i++)
        {
            for (int j = 0; j < maxSirka; j++)
            {
                Console.Write(stromecky.les[i,j]);
            }

            Console.WriteLine();
        }
    }
}