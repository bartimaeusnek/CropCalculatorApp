using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CropApp.Pages
{
    public class ImpressumModel : PageModel
    {
        private readonly ILogger<ImpressumModel> _logger;

        public ImpressumModel(ILogger<ImpressumModel> logger) => this._logger = logger;

        public void OnGet() { }
    }
}