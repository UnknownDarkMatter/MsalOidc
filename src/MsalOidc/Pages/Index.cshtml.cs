using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace OicdDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public List<FileInfo> MainJsWebPath = new List<FileInfo>();
        public List<FileInfo> MainCssWebPath = new List<FileInfo>();

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public void OnGet()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string jsPath = webRootPath;
            string cssPath = webRootPath;

            var jsRegex = new Regex(".+\\.js");
            var cssRegex = new Regex(".+\\.css");

            MainJsWebPath = (new DirectoryInfo(jsPath).GetFiles())
                .Where(f => jsRegex.IsMatch(f.Name))
                .OrderByDescending(f => f.Name) //Polyfills before Main
                .ToList();

            MainCssWebPath = (new DirectoryInfo(cssPath).GetFiles())
                .Where(f => cssRegex.IsMatch(f.Name))
                .ToList();
            
        }
    }
}