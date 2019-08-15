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
        private static async Task Main(string[] args)
        {
            string chromeDriverLocation = Directory.GetCurrentDirectory();
            CodexItemScraper itemScraper = null;
            CodexRecipeScraper recipeScraper = null;
            CodexItemGroupScraper itemGroupScraper = null;
            string file = string.Empty;
            string json = string.Empty;
            List<BdoRecipeModel> recipes = null;
            List<BdoItemModel> items = null;
            List<BdoItemGroupModel> itemGroups = null;

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
                            items = await itemScraper.GetMaterialItemsAsync();
                            file = "MaterialItems.json";
                        }
                        else if(args[1] == "-crystal")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = await itemScraper.GetCrystalItemsAsync();
                            file = "SocketItems.json";
                        }
                        else if(args[1] == "-alchstones")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = await itemScraper.GetAlchemyStoneItemsAsync();
                            file = "AlchemyStoneItems.json";
                        }
                        else if(args[1] == "-Enhancement")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = await itemScraper.GetEnhancementItemsAsync();
                            file = "EnhancementItems.json";
                        }
                        else if(args[1] == "-Consumable")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = await itemScraper.GetConsumableItemsAsync();
                            file = "ConsumableItems.json";
                        }
                        else if(args[1] == "-Mount")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = await itemScraper.GetMountItemsAsync();
                            file = "MountItems.json";
                        }
                        else if (args[1] == "-CookingMats")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = await itemScraper.GetCookingMaterialsAsync();
                            file = "CookingMaterials.json";
                        }
                        else if(args[1] == "-AlchemyMats")
                        {
                            itemScraper = new CodexItemScraper(chromeDriverLocation);
                            items = await itemScraper.GetAlchemyMaterialsAsync();
                            file = "AlchemyMaterials.json";
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
                else if(args[0].ToLower() == "itemgroups")
                {
                    if(!(args.Length > 1))
                    {
                        itemGroupScraper = new CodexItemGroupScraper();
                        itemGroups = await itemGroupScraper.GetBdoItemGroupsAsync();
                        Console.WriteLine("Writing To File");
                        json = JsonConvert.SerializeObject(itemGroups);
                        File.WriteAllText("ItemGroups.json", json);
                    }
                    else
                    {
                        Console.WriteLine("invalid number of arguments. Just needs itemgroups");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Give a valid argument. eg: recipe, item, itemgroups");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Please enter an argument eg: recipe, item, itemgroups");
                return;
            }

        } 
    }
}
