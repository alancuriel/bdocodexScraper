using BdoCodexSraping.Core;
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

namespace Scraper.Scrapers
{
    public class CodexItemScraper
    {
        public static string MaterialItemsUrl { get; set; } = "https://bdocodex.com/us/items/materials/";
        public static string MaterialTableHtml { get; set; } = "MainItemTable";
        public ChromeDriver Driver { get; set; }

        public CodexItemScraper(string chromeDriver)
        {
            Driver = new ChromeDriver(chromeDriver);
        }

        public List<BdoItemModel> GetMaterialItems()
        {
            var materialLinks = GetItemLinks(MaterialItemsUrl,MaterialTableHtml);
            var materials = new List<BdoItemModel>();
            var htmlDocument = new HtmlDocument();

            foreach (var materialLink in materialLinks)
            {
                var cookie = new CookieContainer();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(materialLink);
                httpWebRequest.CookieContainer = cookie;
                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                httpWebRequest.Referer = MaterialItemsUrl;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
                httpWebRequest.Method = "GET";
                httpWebRequest.AllowAutoRedirect = true;

                var response = httpWebRequest.GetResponse();

                var outputString = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        outputString = reader.ReadToEnd();
                    }
                }

                Console.WriteLine(outputString);
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

                

                materials.Add(item);
            }


            return materials;
        }

        private List<string> GetItemLinks(string url, string htmlTableName)
        {
            var links = new List<string>();

            using(var driver = Driver)
            {
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(20000);
                driver.Navigate().GoToUrl(url);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementExists(By.Id($"{htmlTableName}_next")));

                //while (driver.FindElementById($"{htmlTableName}_next").GetAttribute("class")
                //    != "paginate_button next disabled")
                //{
                    var titleElements = driver.FindElementsByClassName("dt-title-search").Where(e => e.TagName == "td");

                    foreach (var titleElement in titleElements)
                    {
                        var linkElement = titleElement.FindElement(By.TagName("a"));
                        string itemUrl = linkElement.GetAttribute("href");
                        
                        links.Add(itemUrl);
                    }

                //    var nextbtnSurroundElem = driver.FindElementById($"{htmlTableName}_next");
                //    var nextLink = nextbtnSurroundElem.FindElement(By.TagName("a"));
                //    nextLink.Click();
                //}

                    driver.Close();
            }

            return links;
        }
    }
}
