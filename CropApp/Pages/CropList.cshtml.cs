using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CropApp.Backend;
using CropApp.Frontend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CropApp.Pages
{
    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class CropListModel : PageModel
    {
        [BindProperty(SupportsGet = true)] public List<CropModel> Crops { get; private set; }

        public void OnGet()
        {
            this.Crops = CropCalculation.AllCrops;
        }
    }
}