﻿using BdoCodexSraping.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net.Cache;
using System.Threading.Tasks;

namespace Scraper.Scrapers
{
    public class CodexItemScraper
    {
        public static string MaterialItemsUrl { get; set; } = "https://bdocodex.com/us/items/materials/";
        public static string GemItemsUrl { get; set; } = "https://bdocodex.com/us/items/gems/";
        public static string AlchemyStonesUrl { get; set; } = "https://bdocodex.com/us/items/stones/";
        public static string EnchancementItemsUrl { get; set; } = "https://bdocodex.com/us/items/powerup/";
        public static string ConsumableItemsUrl { get; set; } = "https://bdocodex.com/us/items/consumables/";
        public static string MountItemUrl { get; set; } = "https://bdocodex.com/us/items/mounts/";
        public static string MaterialTableHtml { get; set; } = "MainItemTable";
        public static string GemTableHtml { get; set; } = "GemsTable";
        public static string ConsumableTableHtml { get; set; } = "ConsumablesTable";
        public static string MaterialLinkElementId { get; set; } = "dt-title-search";
        public static string GemLinkElementId { get; set; } = "sorting_1";

        public static string CookingRecipesLink { get; set; } = "https://bdocodex.com/us/recipes/culinary/";
        public static string CookingDivFilterId { get; set; } = "CulinaryRecipesTable_material_filters_content";
        public static string AlchemyRecipesLink { get; set; } = "https://bdocodex.com/us/recipes/alchemy/";
        public static string AlchemyDivFilterId { get; set; } = "AlchemyRecipesTable_material_filters_content";

        public ChromeDriver Driver { get; set; }

        public CodexItemScraper(string chromeDriver)
        {
            Driver = new ChromeDriver(chromeDriver);
        }

        public async Task<List<BdoItemModel>> GetMaterialItemsAsync()
        {
            var materialLinks = GetItemLinks(MaterialItemsUrl, MaterialTableHtml, MaterialLinkElementId);
            return await ScrapeItemLinksAsync(materialLinks);
        }

        public async Task<List<BdoItemModel>> GetCrystalItemsAsync()
        {
            var materialLinks = GetItemLinks(GemItemsUrl, GemTableHtml, GemLinkElementId);
            return await ScrapeItemLinksAsync(materialLinks);
        }

        public async Task<List<BdoItemModel>> GetAlchemyStoneItemsAsync()
        {
            var alchStones = GetItemLinks(AlchemyStonesUrl, MaterialTableHtml, MaterialLinkElementId);
            return await ScrapeItemLinksAsync(alchStones);
        }

        public async Task<List<BdoItemModel>> GetEnhancementItemsAsync()
        {
            var enhanceItems = GetItemLinks(EnchancementItemsUrl, MaterialTableHtml, MaterialLinkElementId);
            return await ScrapeItemLinksAsync(enhanceItems);
        }

        public async Task<List<BdoItemModel>> GetConsumableItemsAsync()
        {
            var consumables = GetItemLinks(ConsumableItemsUrl, ConsumableTableHtml, GemLinkElementId);
            return await ScrapeItemLinksAsync(consumables);
        }

        public async Task<List<BdoItemModel>> GetCookingMaterialsAsync()
        {
            var cookingMaterialLinks = GetFilterItemLinks(CookingRecipesLink, CookingDivFilterId);
            return await ScrapeItemLinksAsync(cookingMaterialLinks);
        }

        public async Task<List<BdoItemModel>> GetAlchemyMaterialsAsync()
        {
            var alchemyMaterialLinks = GetFilterItemLinks(AlchemyRecipesLink, AlchemyDivFilterId);
            return await ScrapeItemLinksAsync(alchemyMaterialLinks);
        }

        public async Task<List<BdoItemModel>> GetMountItemsAsync()
        {
            var mountItemLinks = GetItemLinks(MountItemUrl, MaterialTableHtml, MaterialLinkElementId);
            return await ScrapeItemLinksAsync(mountItemLinks);
        }

        private List<string> GetFilterItemLinks(string url, string DivFilterId)
        {
            var links = new List<string>();

            using (var driver = Driver)
            {
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(20000);
                driver.Navigate().GoToUrl(url);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementExists(By.Id(DivFilterId)));

                var filterSurroundElem = driver.FindElementById(DivFilterId);

                var itemFilters = filterSurroundElem.FindElements(By.ClassName("material_wrapper"));

                foreach (var itemFilter in itemFilters)
                {
                    var imgElem = itemFilter.FindElement(By.TagName("img"));
                    if (imgElem.GetAttribute("data-id").StartsWith("item"))
                    {
                        links.Add($"https://bdocodex.com/us/item/{imgElem.GetAttribute("data-item_id")}");
                    }
                }
                driver.Close();
            }
            return links;
        }

        private List<string> GetItemLinks(string url, string htmlTableName, string linkElementId)
        {
            var links = new List<string>();

            using (var driver = Driver)
            {
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(20000);
                driver.Navigate().GoToUrl(url);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementExists(By.Id($"{htmlTableName}_next")));

                while (driver.FindElementById($"{htmlTableName}_next").GetAttribute("class")
                    != "paginate_button next disabled")
                {
                    var titleElements = driver.FindElementsByClassName(linkElementId).Where(e => e.TagName == "td");

                    foreach (var titleElement in titleElements)
                    {
                        var linkElement = titleElement.FindElement(By.TagName("a"));
                        string itemUrl = linkElement.GetAttribute("href");
                        Console.WriteLine($"Got {itemUrl}");
                        links.Add(itemUrl);
                    }

                    var nextbtnSurroundElem = driver.FindElementById($"{htmlTableName}_next");
                    var nextLink = nextbtnSurroundElem.FindElement(By.TagName("a"));
                    nextLink.Click();
                }


                var titleElements2 = driver.FindElementsByClassName(linkElementId).Where(e => e.TagName == "td");

                foreach (var titleElement in titleElements2)
                {
                    var linkElement = titleElement.FindElement(By.TagName("a"));
                    string itemUrl = linkElement.GetAttribute("href");
                    Console.WriteLine($"Got {itemUrl}");
                    links.Add(itemUrl);
                }


                    driver.Close();
            }

            return links;
        }

        public static async System.Threading.Tasks.Task<List<BdoItemModel>> ScrapeItemLinksAsync(List<string> links)
        {
            var materials = new List<BdoItemModel>();
            var htmlDocument = new HtmlDocument();

            foreach (var materialLink in links)
            {
                Console.WriteLine($"Scraping from {materialLink}");
                var cookie = new CookieContainer();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(materialLink);
                httpWebRequest.CookieContainer = cookie;
                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                httpWebRequest.Referer = MaterialItemsUrl;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
                httpWebRequest.Method = "GET";
                httpWebRequest.AllowAutoRedirect = true;

                var response = await httpWebRequest.GetResponseAsync();

                var outputString = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        outputString = reader.ReadToEnd();
                    }
                }


                htmlDocument.LoadHtml(outputString);

                var item = new BdoItemModel();

                HtmlNode itemBox = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='outer item_info']");

                //Item id
                int itemId = 0;
                var idTD = itemBox.Descendants("td")
                .Where(e => e.InnerText.StartsWith("ID: "));
                Int32.TryParse(Regex.Match(idTD.First().InnerText, @"\d+").Value, out itemId);
                item.Id = itemId;


                //Item Name
                var nameElem = itemBox.Descendants("span").Where(n => n.GetAttributeValue("id", "") == "item_name").FirstOrDefault();
                var name = nameElem?.Descendants("b").FirstOrDefault().InnerText;
                if (name != null)
                {
                    item.Name = name;
                }

                //Item Grade
                ItemGrade cacheItemGrade = ItemGrade.White;
                var gradeString = nameElem.GetClasses().ToList()[1].Replace("item_grade_", "");
                Enum.TryParse(gradeString, out cacheItemGrade);
                item.Grade = cacheItemGrade;

                //Item Weight
                string weight = "0 LT";
                var weightElem = itemBox.Descendants("td").Where(t => t.InnerText.Contains("Weight: ")).FirstOrDefault();
                string weightElemString = weightElem.InnerText;
                if (weightElem != null)
                {
                    weight = weightElemString.Substring
                        (weightElemString.LastIndexOf("Weight: ") + 8, weightElemString.Length - weightElemString.LastIndexOf("LT") + 3);
                }
                item.Weight = weight;

                //Item Img
                var itemImg = itemBox.Descendants("img").Where(r => r.GetAttributeValue("class", "") == "item_icon")
                    .FirstOrDefault().GetAttributeValue("src", string.Empty);
                item.Img = $"https://{response.ResponseUri.Host}{itemImg}";

                //Item Category
                var itemCategory = itemBox.Descendants("span")
                                   .Where(n => n.GetAttributeValue("class", string.Empty) == "category_text")
                                   .FirstOrDefault();
                if (itemCategory != null)
                {
                    item.Category = itemCategory.InnerText;
                }

                //Item Description
                var itemDescriptionElem = itemBox.Descendants("td").Where(n => n.InnerText.Contains("Description")).FirstOrDefault();
                if (itemDescriptionElem != null)
                {
                    item.Description = itemDescriptionElem.InnerText;//Replace with innerHtml for colored text markups
                    if (item.Description.Contains("Knowledge:"))
                    {
                        var knowledgeNode = itemDescriptionElem.Descendants("a")
                            .FirstOrDefault(e => e.GetAttributeValue("data-id", "").StartsWith("theme--"));

                        if (knowledgeNode != null)
                        {
                            item.Knowledge = knowledgeNode.GetAttributeValue("data-id", "");
                        }
                    }
                    var sellOrMarket = "Sell price: ";
                    if (!item.Description.Contains(sellOrMarket))
                        sellOrMarket = "Market Price: ";

                    if (item.Description.Contains("Buy price:"))
                    {

                        string sellBuyPrice = string.Empty;

                        if (item.Description.Contains("Repair price"))
                        {
                            int buyI = item.Description.IndexOf("Buy price:");
                            sellBuyPrice = item.Description.Substring(buyI, item.Description.IndexOf("Repair price:") - buyI);
                        }
                        else
                        {
                            sellBuyPrice = item.Description.Substring(item.Description.IndexOf("Buy price:"));
                        }


                        var buyPrice = sellBuyPrice.Substring(0, sellBuyPrice.IndexOf(sellOrMarket));
                        item.BuyPrice = buyPrice.Replace("Buy price: ", "");

                        var sellprice = sellBuyPrice.Substring(sellBuyPrice.IndexOf(sellOrMarket));
                        item.SellPrice = sellprice.Replace(sellOrMarket, "");
                    } else
                    {
                        item.SellPrice = "0";
                        item.BuyPrice = "0";
                    }

                }

                materials.Add(item);
                Console.WriteLine($"Finished Scraping {materialLink}");
            }


            return materials;
        }
    }
}
