using BdoCodexSraping.Core;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Scrapers
{
    public class CodexItemGroupScraper
    {
        public static string ItemGroupStartUrl { get; set; } = "https://bdocodex.com/us/materialgroup/";
        public static int ItemGroupAmount { get; set; } = 20;

        public async Task<List<BdoItemGroupModel>> GetBdoItemGroupsAsync()
        {
            var itemGroups = new List<BdoItemGroupModel>();
            var htmlDoc = new HtmlDocument();

            for(int i = 1; i < 54; i++)
            {
                if (i == 12 || (i >= ItemGroupAmount &&  i < 48))
                    continue;

                var itemGroup = new BdoItemGroupModel();
                itemGroup.Id = i;

                var cookie = new CookieContainer();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($"{ItemGroupStartUrl}{i}");
                httpWebRequest.CookieContainer = cookie;
                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                httpWebRequest.Referer = "https://bdocodex.com/us/items/materials/";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
                httpWebRequest.Method = "GET";
                httpWebRequest.AllowAutoRedirect = true;


                Console.WriteLine("Scraping {0}{1}",ItemGroupStartUrl,i);

                var response = await httpWebRequest.GetResponseAsync();

                var outputString = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        outputString = reader.ReadToEnd();
                    }
                }
                htmlDoc.LoadHtml(outputString);

                itemGroup.Name = htmlDoc.DocumentNode.Descendants()
                    .FirstOrDefault(n => n.GetAttributeValue("property", "") == "og:title")
                    .GetAttributeValue("content", "unknown");

                HtmlNode itemBox = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='outer item_info']");
                var itemTrs = itemBox.Descendants("div").Where(n => n.GetAttributeValue("class", "") == "iconset_wrapper_medium inlinediv");

                var itemLinks = new List<string>();

                foreach(var itemTr in itemTrs)
                {
                    //var item = new BdoItemModel();

                    var itemLinkElem = itemTr.Descendants("a").FirstOrDefault();

                    itemLinks.Add($"https://bdocodex.com{itemLinkElem.GetAttributeValue("href","")}");

                    //item.Id = Int32.Parse(itemLinkElem.GetAttributeValue("data-id", "").Replace("item--", ""));
                    //itemGroup.Items.Add(item);
                }


                itemGroup.Items = await CodexItemScraper.ScrapeItemLinksAsync(itemLinks);


                itemGroups.Add(itemGroup);
            }
            return itemGroups;
        }

    }
}
