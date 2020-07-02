using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CropApp.Backend;
using CropApp.Frontend;

namespace CropApp.Util
{
    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToFirstOrDefault")]
    public static class Extensions
    {
        public static (string, string) GetCropNamesFromLong(this long lLong)
            => (CropCalculation.AllCrops[(int) (lLong >> 24)].name,
                CropCalculation.AllCrops[(int) (lLong & 0xFFFFFF)].name);

        public static long GetCropIDsFromModels(this CropModel cropModel, CropModel other)
            => ((long) cropModel.interalID << 24) | (long) other.interalID;

        public static CropModel GetCropFromName(this string name)
            => CropCalculation.AllCrops.Where(x => x.name == name).FirstOrDefault();
        
        private class SortTripleToupleByThirdObject : IComparer<(string, string, double)>
        {
            public int Compare((string, string, double) c1, (string, string, double) c2) 
                => c1.Item3 < c2.Item3 ? 1 : c1.Item3 > c2.Item3 ? -1 : 0;
        }
        
        public static List<(string, string, double)> GetPossibleParents(this string cropName)
        {
            var ret = new List<(string, string, double)>();
            foreach (var breedingDictValue in CropCalculation.BreedingDict)
            {
                foreach (var valueTuple in breedingDictValue.Value)
                {
                    if (
                        valueTuple.Item1 == cropName 
                     && breedingDictValue.Key.Item1 != cropName 
                     && breedingDictValue.Key.Item2 != cropName
                        )
                    {
                        ret.Add((breedingDictValue.Key.Item1, breedingDictValue.Key.Item2, valueTuple.Item2));
                    }
                }
            }

            ret.Sort(new SortTripleToupleByThirdObject());
            
            return ret;
        }
    }
    
}