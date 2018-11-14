using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Common
{
    public static class McFHTMLHelper
    {
        public static Dictionary<string, string> ParseHTMLFile(string url, DateTime dt)
        {
            var web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument resultat = web.Load(url);
            var data = resultat.DocumentNode.SelectNodes("//div[@class='fileElement']");
            Dictionary<string, string> str = new Dictionary<string, string>();
            foreach (HtmlNode node in data)
            {
                string innerText = node.InnerHtml;
                if (innerText.Contains(".zip"))
                {
                    int startlen = innerText.IndexOf("href=\"") + 6;
                    int endlen = innerText.IndexOf("\"", startlen);
                    //int startend = innerText.
                    string dataurl = innerText.Substring(startlen, endlen - startlen);
                    string fileName = Path.GetFileNameWithoutExtension(dataurl);
                    string date = fileName.Replace("CropProg-", String.Empty).Replace("_revision", string.Empty).
                        Replace("_correction", string.Empty).Replace("HogsPigs-", String.Empty).
                        Replace("ChicEggs-", String.Empty).Replace("BroiHatc-", String.Empty).Replace("CattOnFe-", String.Empty).
                        Replace("FatsOils-", String.Empty).Replace("_Non Ambulatory Cattle and Calves", string.Empty);
                    date = date.Substring(0, 10);
                    DateTime fileDate = Convert.ToDateTime(date);
                    if (fileDate.Date == dt.Date)
                    {
                        str[date] = dataurl;
                    }
                }
            }
            return str;
        }

        public static Dictionary<string,string> ParseWASDEFile(string url, DateTime DataDate)
        {
            var web = new HtmlWeb();
            Dictionary<string, string> str = new Dictionary<string, string>();

            HtmlAgilityPack.HtmlDocument resultat = web.Load(url);
            foreach (HtmlNode node in resultat.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            {
                string href = node.Attributes["href"].Value;
                if (href.Contains(".xlsx") || href.Contains(".xls"))
                {
                    string lastString = href.Substring(href.LastIndexOf('/') + 1);
                    string lastSubString = lastString.Substring(lastString.IndexOf('-') + 1);
                    if (lastSubString.Length <= 15)
                    {
                        DateTime dt = DateTime.Parse(lastSubString.Substring(0, 10));
                        string date = dt.ToShortDateString();
                        if (dt.Date == DataDate.Date)
                        {
                            str[date] = href;
                        }
                    }
                }
            }
            return str;
        }
    }
}
