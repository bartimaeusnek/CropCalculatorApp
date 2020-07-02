using System.Collections.Generic;
using System.Linq;
using CropApp.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CropApp.Pages
{
    public class CropPathModel : PageModel
    {
        [BindProperty(SupportsGet = true)] 
        public List<(string, string, double)> Crops { get; private set; }
        
        public void OnGet(string crop = "reed", params string[] ign) 
            => this.Crops = crop.GetPossibleParents()
                                .Where(x => !ign.Where(y => x.Item1 == y || x.Item2 == y).Any()).Take(25).ToList();
    }
}