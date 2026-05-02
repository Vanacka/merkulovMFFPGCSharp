namespace merkulovMFFPGCSharp.DU;
using System.IO;

public class ABCDTabulkaZnaku
{
    public void Hlavni()
    {
        string prectenyText = "";
        string cestaKSouboru = "/home/ivan/DU/mffPG/merkulovMFFPGCSharp/merkulovMFFPGCSharp/DU/text-abecedy.txt";
        if (File.Exists(cestaKSouboru))
        {
            prectenyText = File.ReadAllText(cestaKSouboru);
        }

        Dictionary<char, float> znakyTextu = new Dictionary<char, float>();

        foreach (char pismeno in prectenyText)
        {
            if (znakyTextu.ContainsKey(pismeno))
            {
                znakyTextu[pismeno] += 1;
            }
            else
            {
                znakyTextu.Add(pismeno, 1);
            }
        }

        znakyTextu = znakyTextu.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        float celkovyPocetZnaku = znakyTextu.Values.Sum();
        float suma = 0;
        float hodnota = 0;
        int kolikNadNula = 0;
        foreach (KeyValuePair<char, float> pair in znakyTextu)
        {
            hodnota = pair.Value / (celkovyPocetZnaku / 100) * 64 / 100;
            Console.WriteLine($"Pismeno: {pair.Key} cetnost: {hodnota}");
            suma += hodnota;
            if (hodnota > 1) kolikNadNula++;
        }

        Console.WriteLine(celkovyPocetZnaku);
        Console.WriteLine(suma);
        Console.WriteLine(kolikNadNula);
        
        //"WHRJYOFUTwEQLjDMIASfPVCNhvqgb.pdcomrlnastuie    eeiiuuttsanlrmoc"
    }
}