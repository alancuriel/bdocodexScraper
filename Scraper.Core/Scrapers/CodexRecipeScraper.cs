using BdoCodexSraping.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Scraper.Scrapers
{
    public class CodexRecipeScraper
    {
        public static string CookingRecipesUrl { get; } = "https://bdocodex.com/us/recipes/culinary/";
        public static string AlchemyRecipesUrl { get; } = "https://bdocodex.com/us/recipes/alchemy/";

        public ChromeDriver Driver { get; set; }

        public CodexRecipeScraper(string chromeDriverDir)
        {
            Driver = new ChromeDriver(chromeDriverDir);
        }

        public List<BdoRecipeModel> GetCookingRecipes()
        {
            return GetBdoCodexRecipes(CookingRecipesUrl, "CulinaryRecipesTable");
        }

        public List<BdoRecipeModel> GetAlchemyRecipes()
        {
            return GetBdoCodexRecipes(AlchemyRecipesUrl, "AlchemyRecipesTable");
        }

        private List<BdoRecipeModel> GetBdoCodexRecipes(string url ,string htmlTableName)
        {
            var recipes = new List<BdoRecipeModel>();

            using (var driver = Driver)
            {
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(20000);
                driver.Navigate().GoToUrl(url);


                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementExists(By.Id($"{htmlTableName}_next")));

                while (driver.FindElementById($"{htmlTableName}_next").GetAttribute("class") != "paginate_button next disabled")
                {
                    var recipeRows = driver.FindElementByXPath($"//*[@id='{htmlTableName}']/tbody").FindElements(By.TagName("tr"));

                    foreach (var row in recipeRows)
                    {

                        var newRecipe = new BdoRecipeModel();

                        //Recipe ID
                        string recipeIdString = row.FindElement(By.XPath(".//td[@class=' dt-id']")).Text;
                        if (recipeIdString == null)
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
                            Int32.TryParse(recipeInfoData[2].Text.Replace("'", ""), out int recipeExp);
                            newRecipe.Exp = recipeExp;
                        }

                        /**Recipe Materials And Crafting Results **/
                        var materialAndResultData = row.FindElements(By.XPath(".//td[@class=' dt-reward']"));

                        //Materials and Amounts
                        var materialsData = materialAndResultData[0]
                                            .FindElements(By.XPath(".//div[@class='iconset_wrapper_medium inlinediv']"));
                        foreach (var materialData in materialsData)
                        {
                            var materialCategoryId = materialData.FindElement(By.TagName("a")).GetAttribute("data-id");
                            int materialAmount = 1;
                            var materialAmoundData = materialData.FindElements(By.XPath(".//div[@class='quantity_small nowrap']"));
                            var materialAmountString = string.Empty;
                            if (materialAmoundData.Count > 0)
                            {
                                materialAmountString = materialAmoundData[0].Text;
                            }
                            Int32.TryParse(materialAmountString, out materialAmount);
                            if (materialAmount == 0)
                                materialAmount++;

                            if (materialCategoryId != null)
                            {
                                if (newRecipe.ItemMaterials == null)
                                    newRecipe.ItemMaterials = new List<KeyValuePair<string, int>>();

                                newRecipe.ItemMaterials.Add(new KeyValuePair<string,int>(materialCategoryId, materialAmount));
                            }
                        }

                        //Crafting Results
                        var resultsData = materialAndResultData[1]
                                          .FindElements(By.XPath(".//div[@class='iconset_wrapper_medium inlinediv']"));
                        foreach (var resultData in resultsData)
                        {
                            if (newRecipe.CraftingResults == null)
                                newRecipe.CraftingResults = new List<string>();

                            string dataId = resultData.FindElement(By.TagName("a")).GetAttribute("data-id");
                            if (dataId != null)
                            {
                                newRecipe.CraftingResults.Add(dataId);
                            }
                        }


                        recipes.Add(newRecipe);
                    }

                    var nextbtnSurroundElem = driver.FindElementById($"{htmlTableName}_next");
                    var nextLink = nextbtnSurroundElem.FindElement(By.TagName("a"));
                    nextLink.Click();
                }

                var recipeRows0 = driver.FindElementByXPath($"//*[@id='{htmlTableName}']/tbody").FindElements(By.TagName("tr"));

                foreach (var row in recipeRows0)
                {

                    var newRecipe = new BdoRecipeModel();

                    //Recipe ID
                    string recipeIdString = row.FindElement(By.XPath(".//td[@class=' dt-id']")).Text;
                    if (recipeIdString == null)
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
                        Int32.TryParse(recipeInfoData[2].Text.Replace("'", ""), out int recipeExp);
                        newRecipe.Exp = recipeExp;
                    }

                    /**Recipe Materials And Crafting Results **/
                    var materialAndResultData = row.FindElements(By.XPath(".//td[@class=' dt-reward']"));

                    //Materials and Amounts
                    var materialsData = materialAndResultData[0]
                                        .FindElements(By.XPath(".//div[@class='iconset_wrapper_medium inlinediv']"));
                    foreach (var materialData in materialsData)
                    {
                        var materialCategoryId = materialData.FindElement(By.TagName("a")).GetAttribute("data-id");
                        int materialAmount = 1;
                        var materialAmoundData = materialData.FindElements(By.XPath(".//div[@class='quantity_small nowrap']"));
                        var materialAmountString = string.Empty;
                        if (materialAmoundData.Count > 0)
                        {
                            materialAmountString = materialAmoundData[0].Text;
                        }
                        Int32.TryParse(materialAmountString, out materialAmount);
                        if (materialAmount == 0)
                            materialAmount++;

                        if (materialCategoryId != null)
                        {
                            if (newRecipe.ItemMaterials == null)
                                newRecipe.ItemMaterials = new List<KeyValuePair<string, int>>();

                            newRecipe.ItemMaterials.Add(new KeyValuePair<string, int>(materialCategoryId, materialAmount));
                        }
                    }

                    //Crafting Results
                    var resultsData = materialAndResultData[1]
                                      .FindElements(By.XPath(".//div[@class='iconset_wrapper_medium inlinediv']"));
                    foreach (var resultData in resultsData)
                    {
                        if (newRecipe.CraftingResults == null)
                            newRecipe.CraftingResults = new List<string>();

                        string dataId = resultData.FindElement(By.TagName("a")).GetAttribute("data-id");
                        if (dataId != null)
                        {
                            newRecipe.CraftingResults.Add(dataId);
                        }
                    }


                    recipes.Add(newRecipe);
                }

                driver.Close();
            }

            return recipes;
        }
    }
}
