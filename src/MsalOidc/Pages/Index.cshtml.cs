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
        public List<FileInfo> ChunkJsWebPath = new List<FileInfo>();
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

            var mainJsRegex = new Regex("main.*\\.js");
            var polyfillsJsRegex = new Regex("polyfills.*\\.js");
            var cssRegex = new Regex(".*\\.css");

            MainJsWebPath.Add((new DirectoryInfo(jsPath).GetFiles()).First(f => polyfillsJsRegex.IsMatch(f.Name)));
            MainJsWebPath.Add((new DirectoryInfo(jsPath).GetFiles()).First(f => mainJsRegex.IsMatch(f.Name)));

            MainCssWebPath = (new DirectoryInfo(cssPath).GetFiles())
                .Where(f => cssRegex.IsMatch(f.Name))
                .ToList();

            var indexHtml = System.IO.File.ReadAllText(Path.Combine(webRootPath, "index.html"));
            var chunkRegex = new Regex("<link(\\s+)rel(\\s*)=(\\s*)\"modulepreload\"(\\s+)href(\\s*)=(\\s*)\"(?<ChunkFileName>chunk[^.]+\\.js)\"(\\s*)>");
            var chunkRegexAsString = "<link(\\s+)rel(\\s*)=(\\s*)\"modulepreload\"(\\s+)href(\\s*)=(\\s*)\"(?<ChunkFileName>chunk[^.]+\\.js)\"(\\s*)>";
            MatchCollection matches = Regex.Matches(indexHtml, chunkRegexAsString);
            foreach(Match match in matches)
            {
                string fileName = match.Groups["ChunkFileName"].Value;
                ChunkJsWebPath.Add(new FileInfo(Path.Combine(webRootPath, fileName)));
            }

        }
    }
}