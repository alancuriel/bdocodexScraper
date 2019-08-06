using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BdoCodexSraping.Core;
using Newtonsoft.Json;
using Scraper.Scrapers;

namespace Scraper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string chromeDriverLocation = Directory.GetCurrentDirectory();
            CodexItemScraper itemScraper = null;
            CodexRecipeScraper recipeScraper = null;
            string file = string.Empty;
            string json = string.Empty;
            List<BdoRecipeModel> recipes = null;
            List<BdoItemModel> items = null;

            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                if(args[0].ToLower() == "recipe")
                {
                    if(args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                    {
                        if(args[1].ToLower() == "-cooking")
                        {
                            recipeScraper = new CodexRecipeScraper(chromeDriverLocation);
                            recipes = recipeScraper.GetCookingRecipes();
                            file = "CookingRecipes.json";  
                        }
                        else if(args[1].ToLower() == "-alchemy")
                        {
                            recipeScraper = new CodexRecipeScraper(chromeDriverLocation);
                            recipes = recipeScraper.GetAlchemyRecipes();
                            file = "AlchemyRecipes.json";
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid parameter eg: -cooking -alchemy");
                            return;
                        }

                        Console.WriteLine("Writing To File");
                        json = JsonConvert.SerializeObject(recipes);
                        File.WriteAllText(file, json);
                    }
                    else
                    {
                        Console.WriteLine("Please give a parameter eg: -cooking -alchemy");
                        return;
                    }
                }
                else if(args[0].ToLower() == "item")
                {
                    if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                    {
                        if(args[1] == "-material")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = itemScraper.GetMaterialItems();
                            file = "MaterialItems.json";
                        }
                        else if(args[1] == "-crystal")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = itemScraper.GetMaterialItems();
                            file = "SocketItems.json";
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid parameter eg: -material");
                            return;
                        }
                        Console.WriteLine("Writing To File");
                        json = JsonConvert.SerializeObject(items);
                        File.WriteAllText(file, json);
                    }
                    else
                    {
                        Console.WriteLine("Please give a parameter eg: -material");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Give a valid argument. eg: recipe, item");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Please enter an argument eg: recipe, item");
                return;
            }

        } 
    }
}
