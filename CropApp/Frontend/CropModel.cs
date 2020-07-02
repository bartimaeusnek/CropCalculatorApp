using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable All
namespace CropApp.Frontend
{
    public class CropModel
    {
        public string       name;
        public int          tier;
        public Stats        stats;
        public List<string> attributes;
        public bool mod;
        
        private int[] _statArrBackingField = null;

        [JsonIgnore] public int interalID;

        [JsonIgnore]
        public int[] statArr => _statArrBackingField ??
                                (_statArrBackingField = new[]
                                                        {
                                                            this.stats.che, this.stats.foo, this.stats.def,
                                                            this.stats.col, this.stats.wee
                                                        });
    }

    public class Stats
    {
        public int che;
        public int foo;
        public int def;
        public int col;
        public int wee;

        public override string ToString() =>
            $"chemical: {this.che},food: {this.foo},defensive: {this.def}, color: {this.col}, weed: {this.wee}";
    }
}