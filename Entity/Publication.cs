using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScraperDll.Entity
{
    public class Publication
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Price { get; set; }
        public string Publisher { get; set; }
        public string MainImageUrl { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Author { get; set; }
        public string Page { get; set; }
        private string Jan { get; set; }

        public string ScrapeText(HtmlDocument document, string tag)
        {
            var node = document.DocumentNode.SelectSingleNode(tag);
            return node?.InnerText.Trim() ?? string.Empty;
        }

        public string ScrapeDate(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'mainItemTable')]//tr[td[contains(text(), '出版年月')]]/td");
            if (node == null) return string.Empty;

            string result = node.InnerText.Trim()
                .Replace("年", "-")
                .Replace("月", "-");

            if (result.Contains("日"))
            {
                result.Replace("日", "");
            } else
            {
                result += "1";
            }

            return result;
        }

        public string ScrapePrice(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'mainItemTable')]//tr[td[contains(text(), '税込価格')]]/td");
            return node?.InnerText.Trim()
                .Replace(",", "")
                .Replace("円", "") ?? string.Empty;
        }

        public string ScrapeMainImageUrl(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'mainItemImg')]//img");
            if (node == null) return string.Empty;

            var src = node.GetAttributeValue("src", string.Empty);
            var questionMarkIndex = src.IndexOf("?");
            return questionMarkIndex != -1 ? src.Substring(0, questionMarkIndex) : src;
        }

        public string CreateImgTag()
        {
            StringBuilder imgTagBuilder = new StringBuilder();

            imgTagBuilder.Append("<img src=\"").Append(MainImageUrl).Append("\">");

            return imgTagBuilder.ToString();
        }

    }

}
