using System.Collections.Generic;
using CropApp.Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CropApp.Pages
{
    public class CropCalculationModel : PageModel
    {
        [BindProperty(SupportsGet = true)] public List<(string, double)> BreedingDict { get; private set; }

        public void OnGet(string cropA = "reed", string cropB = "reed")
        {
            this.BreedingDict = CropCalculation.BreedingDict[(cropA, cropB)];
        }
    }
}