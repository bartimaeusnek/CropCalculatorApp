using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CropApp.Frontend;
using CropApp.Util;
using Newtonsoft.Json;

namespace CropApp.Backend
{
    public static class CropCalculation
    {
        private const int             Precision = 100000;
        public static List<CropModel> AllCrops  = new List<CropModel>();

        public static Dictionary<(string, string), List<(string, double)>> BreedingDict =
            new Dictionary<(string, string), List<(string, double)>>();

        private static readonly ConcurrentDictionary<long, ConcurrentBag<int>> Breeding =
            new ConcurrentDictionary<long, ConcurrentBag<int>>();

        private static readonly Dictionary<long, int> RatioZmapping = new Dictionary<long, int>();

        private static XorShiftRandom _xstr = new XorShiftRandom();
        
        private static int[] ratios = new int[AllCrops.Count * AllCrops.Count];
        
        public static async Task ProcessBreeding()
        {
            if (await TryLoadCropJsonAsync())
                return;

            PrecalculateRatios();
            Console.WriteLine("precalulated ratios");
            
            ParallelProcessCrops();
            Console.WriteLine("done with Parallel processing");

            Console.WriteLine("Sort Directory");
            SortDictionary();
            Console.WriteLine("Done sorting Directory");
            await WriteCropJsonToDisk();
        }

        private static async Task WriteCropJsonToDisk()
        {
            await using var streamWriter = new StreamWriter("wwwroot/BreedingDict.json");
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(BreedingDict));
            await streamWriter.FlushAsync();
            streamWriter.Close();
        }
        
        private static void PrecalculateRatios()
        {
            var counter = 0;

            foreach (var cropA in AllCrops)
            {
                foreach (var cropB in AllCrops)
                {
                    ratios[counter] = CalculateRatioFor(cropA, cropB);
                    var id = cropA.GetCropIDsFromModels(cropB);
                    RatioZmapping[id] = counter;
                    ++counter;
                }
            }
        }
        
        private static async Task<bool> TryLoadCropJsonAsync()
        {
            var fi = new FileInfo("wwwroot/BreedingDict.json");
            if (!fi.Exists)
                return false;
            
            using var r    = fi.OpenText();
            var       json = await r.ReadToEndAsync();
            BreedingDict = JsonConvert.DeserializeObject<Dictionary<(string, string), List<(string, double)>>>(json);
            return true;
        }
        
        private static void ParallelProcessCrops()
        {
              Parallel.ForEach(AllCrops,
                             new ParallelOptions {MaxDegreeOfParallelism = 2},
                             cropA =>
                             {
                                 if (cropA.mod)
                                     return;
                                 Parallel.ForEach(AllCrops,
                                                  new ParallelOptions {MaxDegreeOfParallelism = 2},
                                                  cropB =>
                                                  {
                                                      if (cropB.mod)
                                                          return;
                                                      var id = cropA.GetCropIDsFromModels(cropB);
                                                      Breeding.TryAdd(id, new ConcurrentBag<int>());
                                                      Span<int> span = stackalloc int[Precision];
                                                      //Console.WriteLine(@$"get crossing chance over an average of 10k for {cropA.name} &{cropB.name}");
                                                      for (var i = 0; i < Precision; i++)
                                                          span[i] =
                                                              AttemptCrossing(ratios,
                                                                              new[] {cropA.interalID, cropB.interalID})
                                                                 .interalID;
                                                      //Breeding[id].Add(AttemptCrossing(cropA, cropB).interalID);

                                                      //Console.WriteLine("Writings chanches to Array");
                                                      foreach (var i in span) Breeding[id].Add(i);

                                                      //Console.WriteLine("done with this task");
                                                  });
                             });
        }
        
        
        private static void SortDictionary()
        {
            foreach (KeyValuePair<long, ConcurrentBag<int>> kvp in Breeding.AsEnumerable()!
               .OrderBy(k => k.Key))
            {
                Dictionary<string, double> q = kvp.Value.GroupBy(x => x)
                                                  .Select(g => new
                                                               {
                                                                   Value = g.Key,
                                                                   Count = g.Count() / (double) kvp.Value.Count
                                                               })
                                                  .OrderByDescending(x => x.Count)
                                                  .ToDictionary(arg => AllCrops[arg.Value].name, arg => arg.Count);

                BreedingDict.Add(kvp.Key.GetCropNamesFromLong(), q.Select(x => (x.Key, x.Value)).ToList());
            }
        }
        
        private static CropModel AttemptCrossing(Span<int> ratioz, IReadOnlyCollection<int> cropIds)
        {
            if (cropIds.Count > 4 || cropIds.Count < 2)
                return null;

            Span<int> ratios = stackalloc int[AllCrops.Count];
            var       total  = 0;
            for (var index = 0; index < AllCrops.Count; index++)
            {
                foreach (var te in cropIds)
                    total += ratioz[RatioZmapping[((long) index << 24) | (long) te]];

                ratios[index] = total;
            }

            if (total == 0)
                return null;

            var search = Math.Abs(_xstr.NextInt32()) % total;
            var min    = 0;
            var max    = ratios.Length - 1;
            
            while (min < max)
            {
                var cur = (min + max) / 2;
                if (search < ratios[cur])
                    max = cur;
                else
                    min = cur + 1;
            }

            return AllCrops[min];
        }

        private static int CalculateRatioFor(CropModel a, CropModel b)
        {
            if (a == b)
                return 500;

            var value = 0;

            for (var i = 0; i < 5; ++i)
                value += -Math.Abs(a.statArr[i] - b.statArr[i]) + 2;

            foreach (var x in a.attributes)
            {
                foreach (var y in b.attributes)
                    if (string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase))
                        value += 5;
            }

            var diff = a.tier - b.tier;

            if (diff > 1)
                value -= 2 * diff;

            if (diff < -3)
                value += diff;

            return Math.Max(value, 0);
        }
    }
}