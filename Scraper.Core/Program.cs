using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BdoCodexSraping.Core;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using static System.IO.Directory;
using Cookie = System.Net.Cookie;

namespace Scraper
{
    internal class Program
    {
        private static async Task Main()
        {
            string chromeDriverLocation = "C:/Users/ALANC/Desktop/";
            string codexRecipePage = "https://bdocodex.com/us/recipes/alchemy/";
            List<string> recipeLinks = new List<string>();
            var recipes = new List<BdoRecipeModel>();

            using (var driver = new ChromeDriver(chromeDriverLocation))
            {
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(20000);
                driver.Navigate().GoToUrl(codexRecipePage);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementExists(By.Id("CulinaryRecipesTable_next")));

                while (driver.FindElementById("CulinaryRecipesTable_next").GetAttribute("class") != "paginate_button next disabled")
                {
                    var recipeRows = driver.FindElementByXPath("//*[@id='CulinaryRecipesTable']/tbody").FindElements(By.TagName("tr"));

                    foreach(var row in recipeRows)
                    {
                        
                        var newRecipe = new BdoRecipeModel();

                        //Recipe ID
                        string recipeIdString = row.FindElement(By.XPath(".//td[@class=' dt-id']")).Text;
                        if(recipeIdString == null)
                        {
                            continue;
                        }
                        Int32.TryParse(recipeIdString, out int recipeID);
                        newRecipe.Id = recipeID;

                        //Recipe Img
                        var recipeImgData = row.FindElement(By.XPath(".//td[@class=' dt-icon']"));
                        string recipeImgString = recipeImgData.FindElement(By.TagName("img")).GetAttribute("src");
                        newRecipe.Img = recipeImgString;

                        //Recipe Name and Grade
                        var recipeTitleData = row.FindElement(By.XPath(".//td[@class='dt-title sorting_1']"));
                        var recipeNameLink = recipeTitleData.FindElement(By.TagName("a"));
                        string recipeName = recipeNameLink.FindElement(By.TagName("b")).Text;
                        newRecipe.Name = recipeName;

                        string recipeGradeClass = recipeNameLink.GetAttribute("class");
                        Int32.TryParse(Regex.Match(recipeGradeClass, @"\d+").Value, out int recipeGrade);
                        newRecipe.Grade = (ItemGrade)recipeGrade;

                        //Recipe Type, Skill Level, and EXP
                        var recipeInfoData = row.FindElements(By.XPath(".//td[@class=' dt-level']"));

                        string recipeTypeString = recipeInfoData[0].Text;
                        Enum.TryParse(recipeTypeString, out RecipeType recipeType);
                        newRecipe.Type = recipeType;

                        string recipeSkillLevel = recipeInfoData[1].Text;
                        newRecipe.SkillLevel = recipeSkillLevel;

                        if (!string.IsNullOrEmpty(recipeInfoData[2].Text))
                        {
                            Int32.TryParse(recipeInfoData[2].Text.Replace("'",""), out int recipeExp);
                            newRecipe.Exp = recipeExp;
                        }

                        /**Recipe Materials And Crafting Results **/
                        var materialAndResultData = row.FindElements(By.XPath(".//td[@class=' dt-reward']"));

                        //Materials and Amounts
                        var materialsData = materialAndResultData[0]
                                            .FindElements(By.XPath(".//div[@class='iconset_wrapper_medium inlinediv']"));
                        foreach(var materialData in materialsData)
                        {
                            var materialCategoryId = materialData.FindElement(By.TagName("a")).GetAttribute("data-id");
                            int materialAmount = 1;
                            var materialAmoundData = materialData.FindElements(By.XPath(".//div[@class='quantity_small nowrap']"));
                            var materialAmountString = string.Empty;
                            if(materialAmoundData.Count > 0){
                                materialAmountString = materialAmoundData[0].Text;
                            }
                            Int32.TryParse(materialAmountString, out materialAmount);

                            if(materialCategoryId != null)
                            {
                                if (newRecipe.ItemMaterials == null)
                                    newRecipe.ItemMaterials = new Dictionary<string, int>();

                                newRecipe.ItemMaterials.Add(materialCategoryId, materialAmount);
                            }
                        }

                        //Crafting Results
                        var resultsData = materialAndResultData[1]
                                          .FindElements(By.XPath(".//div[@class='iconset_wrapper_medium inlinediv']"));
                        foreach(var resultData in resultsData)
                        {
                            if (newRecipe.CraftingResults == null)
                                newRecipe.CraftingResults = new List<string>();

                            string dataId = resultData.FindElement(By.TagName("a")).GetAttribute("data-id");
                            if(dataId != null)
                            {
                                newRecipe.CraftingResults.Add(dataId);
                            }
                        }
                            

                        recipes.Add(newRecipe);
                    }

                    //var recipeCards = driver.FindElementsByXPath("//td[@class='dt-title sorting_1']");
                    //foreach (var elem in recipeCards)
                    //{
                    //    var linkElem = elem.FindElement(By.TagName("a"));
                    //    var link = linkElem.GetAttribute("href");
                    //    Console.WriteLine(link);
                    //    recipeLinks.Add(link);
                    //}

                    var nextbtnSurroundElem = driver.FindElementById("CulinaryRecipesTable_next");
                    var nextLink = nextbtnSurroundElem.FindElement(By.TagName("a"));
                    nextLink.Click();
                }
                driver.Close();
            }


            var htmlDocument = new HtmlDocument();
            
            string file = "AlchemyRecipes.json";
            

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


            string json = JsonConvert.SerializeObject(recipes);
            File.WriteAllText(file, json);

        }
        
    }
}
