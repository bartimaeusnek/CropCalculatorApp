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
        public static IList<CropModel> AllCrops  = new List<CropModel>();

        public static Dictionary<(string, string), List<(string, double)>> BreedingDict =
            new Dictionary<(string, string), List<(string, double)>>();

        internal static readonly ConcurrentDictionary<long, ConcurrentBag<int>> Breeding =
            new ConcurrentDictionary<long, ConcurrentBag<int>>();

        internal static Dictionary<long, int> RatioZmapping = new Dictionary<long, int>();
        internal static Dictionary<long, (int, int[])> RatioXmapping = new Dictionary<long, (int, int[])>();

        public static async Task ProcessBreeding()
        {
            if (await TryLoadCropJsonAsync())
                return;

            RatioZmapping = PrecalculateRatios();
            Console.WriteLine("precalulated ratios");

            RatioXmapping = PrecalculateRatioListForCrops();
            
            ParallelProcessCrops();
            Console.WriteLine("done with Parallel processing");

            Console.WriteLine("Sort Directory");
            SortDictionary();
            Console.WriteLine("Done sorting Directory");
            await WriteCropJsonToDisk();
        }

        internal static async Task WriteCropJsonToDisk()
        {
            await using var streamWriter = new StreamWriter("wwwroot/BreedingDict.json");
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(BreedingDict));
            await streamWriter.FlushAsync();
            streamWriter.Close();
        }
        
        internal static Dictionary<long, int> PrecalculateRatios()
        {
            var dict = new Dictionary<long, int>();
            foreach (var cropA in AllCrops)
            {
                foreach (var cropB in AllCrops)
                {
                    var id = cropA.GetCropIDsFromModels(cropB);
                    dict[id] = CalculateRatioFor(cropA, cropB);
                }
            }

            return dict;
        }

        internal static Dictionary<long, (int, int[])> PrecalculateRatioListForCrops()
        {
            var dict = new Dictionary<long, (int, int[])>();
            foreach (var cropA in AllCrops)
            {
                foreach (var cropB in AllCrops)
                {
                    var id = cropA.GetCropIDsFromModels(cropB);
                    dict[id] = GetRatioListForCrops(new[] {cropA.interalID, cropB.interalID});
                }
            }

            return dict;
        }
        
        internal static async Task<bool> TryLoadCropJsonAsync()
        {
            var fi = new FileInfo("wwwroot/BreedingDict.json");
            if (!fi.Exists)
                return false;
            
            using var r    = fi.OpenText();
            var       json = await r.ReadToEndAsync();
            BreedingDict = JsonConvert.DeserializeObject<Dictionary<(string, string), List<(string, double)>>>(json);
            return true;
        }
        
        internal static void ParallelProcessCrops()
        {
            foreach (var cropA in AllCrops)
            {
                if (cropA.mod)
                    continue;
                foreach (var cropB in AllCrops)
                {
                    if (cropB.mod)
                        continue;
                    Console.WriteLine($"Crossing {cropA.name} with {cropB.name}");
                    var id = cropA.GetCropIDsFromModels(cropB);

                    Breeding.TryAdd(id, new ConcurrentBag<int>(AttemptCrossing(RatioXmapping[id])));
                }
            }
        }
        
        internal static void SortDictionary()
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

        internal static (int, int[]) GetRatioListForCrops(IReadOnlyCollection<int> cropIds)
        {
            if (cropIds.Count > 4 || cropIds.Count < 2)
                return (-1,null);

            Span<int> ratios = stackalloc int[AllCrops.Count];
            var       total  = 0;
            for (var index = 0; index < AllCrops.Count; index++)
            {
                foreach (var te in cropIds)
                    total += RatioZmapping[((long) index << 24) | (long) te];

                ratios[index] = total;
            }

            if (total == 0)
                return (-1,null);
            
            return (total, ratios.ToArray());
        }

        internal static int[] AttemptCrossing((int total, int[] ratios) ratioList)
        {
            Span<int> ret = stackalloc int[ratioList.total-1];
            for (int i = 0; i < ratioList.total-1; i++)
                ret[i] = BinarySearchArray(i, ratioList.ratios);
            
            return ret.ToArray();
        }

        internal static int BinarySearchArray(int search, int[] array)
        {
            var min    =  0;
            var max= array.Length - 1;
            
            while (min < max)
            {
                var cur = (min + max) / 2;
                if (search < array[cur])
                    max = cur;
                else
                    min = cur + 1;
            }

            return min;
        }
        
        internal static int CalculateRatioFor(CropModel a, CropModel b)
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