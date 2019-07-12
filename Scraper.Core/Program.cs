using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scraper.Scrapers;

namespace Scraper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string chromeDriverLocation = "C:/Users/ALANC/Desktop/";
            CodexItemScraper scraper = new CodexItemScraper(chromeDriverLocation);

            var recipes = scraper.GetMaterialItems();

            
            string file = "MaterialItems.json";
            string json = JsonConvert.SerializeObject(recipes);
            
            File.WriteAllText(file, json);

        } 
    }
}
