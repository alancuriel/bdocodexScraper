using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scraper.Scrapers;

namespace Scraper
{
    internal class Program
    {
        private static void Main()
        {
            string chromeDriverLocation = "C:/Users/ALANC/Desktop/";
            CodexRecipeScraper scraper = new CodexRecipeScraper(chromeDriverLocation);

            var recipes = scraper.GetCookingRecipes();

            
            string file = "CookingRecipes.json";
            string json = JsonConvert.SerializeObject(recipes);
            File.WriteAllText(file, json);

            //foreach (var recipeLink in recipeLinks)
            //{
            //    var cookie = new CookieContainer();
            //    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(recipeLink);
            //    httpWebRequest.CookieContainer = cookie;
            //    httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            //    httpWebRequest.Referer = "https://bdocodex.com/us/recipes/culinary/";
            //    httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
            //    httpWebRequest.Method = "GET";
            //    httpWebRequest.AllowAutoRedirect = true;

            //    var response = await httpWebRequest.GetResponseAsync();

            //    var outputString = string.Empty;
            //    using (Stream stream = response.GetResponseStream())
            //    {
            //        using (StreamReader reader = new StreamReader(stream))
            //        {
            //            outputString = reader.ReadToEnd();
            //        }
            //    }

            //    htmlDocument.LoadHtml(outputString);

            //    var recipe = new BdoRecipeModel();

            //    HtmlNode recipeBox = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div/div[1]/div");
            //    HtmlNode recipeCraftingMats = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div/div[1]/div/table/tbody/tr[5]/td");

            //    //Get Recipe id from page
            //    int recipeId = 0;
            //    var idTD = recipeBox.Descendants("td")
            //    .Where(e => e.InnerText.StartsWith("ID: "));
            //    Int32.TryParse(Regex.Match(idTD.First().InnerText, @"\d+").Value , out recipeId);
            //    recipe.Id = recipeId;

            //    Console.WriteLine(recipeId);
            //    //Get Recipe Name
            //    var nameElem = recipeBox.Descendants("span").Where(n => n.GetAttributeValue("id", "") == "item_name").FirstOrDefault();
            //    var name = nameElem?.Descendants("b").FirstOrDefault().InnerText;
            //    if (name != null)
            //    {
            //        recipe.Name = name; 
            //    }

            //    //Get item Grade
            //    ItemGrade cacheItemGrade = ItemGrade.White;
            //    var gradeString = nameElem.GetClasses().ToList()[1].Replace("item_grade_","");
            //    Enum.TryParse(gradeString, out cacheItemGrade);
            //    recipe.Grade = cacheItemGrade;

            //    //Get Recipe Img
            //    var recipeImg = recipeBox.Descendants("img").Where(r => r.GetAttributeValue("class", "") == "item_icon")
            //        .FirstOrDefault().GetAttributeValue("href",string.Empty);
            //    recipe.Img = recipeImg;

            //    //Get skill level
            //    var recipeSkillLevel = recipeBox.Descendants("span").Where(n => n.InnerText.StartsWith("Skill level: ")).FirstOrDefault();
            //    if (recipeSkillLevel != null)
            //    {
            //        recipe.SkillLevel = recipeSkillLevel.InnerText.Replace("Skill level: ", ""); 
            //    }

            //    //Get recipe EXP
            //    int recipeExpNum = 0;
            //    var recipeExp = recipeBox.Descendants("td").Where(t => t.InnerText.Contains("EXP")).FirstOrDefault();
            //    var recipeElemInnerText = recipeExp?.InnerText;
            //    if (recipeElemInnerText != null)
            //    {
            //        var recipeExpString = Regex.Match(recipeElemInnerText.Substring(recipeElemInnerText.LastIndexOf("EXP:") + 1)
            //            .Replace("'",""), @"\d+").Value;
            //        Int32.TryParse(recipeExpString, out recipeExpNum); 
            //    }
            //    recipe.Exp = recipeExpNum;

            //    //Get Recipe Crafting mats


            //    recipe.Type = RecipeType.Cooking;

            //    recipes.Add(recipe);
            //}




        }
        
    }
}
