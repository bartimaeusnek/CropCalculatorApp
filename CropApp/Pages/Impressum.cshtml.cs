using CropApp.Frontend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CropApp.Pages
{
    public class ImpressumModel : PageModel
    {
        
        [BindProperty(SupportsGet = true)]
        public ImpressumData Data { get; } = Program.ImpressumData;

        public ImpressumModel() { }

        public void OnGet() { }
    }
}