using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public string Jan { get; set; }

        public string ScrapeText(HtmlDocument document, string tag)
        {
            var node = document.DocumentNode.SelectSingleNode(tag);
            return node?.InnerText.Trim() ?? string.Empty;
        }

        public string ScrapeDate(HtmlDocument document, string tag)
        {
            string xpath = $"//div[@class='mainItemTable']//tr[th[contains(text(), '{tag}')]]//td";
            var node = document.DocumentNode.SelectSingleNode(xpath);
            if (node == null) return String.Empty;

            string result = node.InnerText.Trim()
                .Replace("年", "-")
                .Replace("月", "-");

            if (result.Contains("日"))
            {
                result = result.Replace("日", "");
            } else
            {
                result += "1";
            }

            return result;
        }

        public string ScrapePrice(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'mainItemTable')]//tr[th[contains(text(), '税込価格')]]//td");

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

        public override string ToString()
        {
            return $"Title: {Title}\n" +
                   $"Price: {Price}\n" +
                   $"Publisher: {Publisher}\n" +
                   $"Main Image URL: {MainImageUrl}\n" +
                   $"Description: {Description}\n" +
                   $"ISBN: {ISBN}\n" +
                   $"Date: {Date}\n" +
                   $"Author: {Author}\n" +
                   $"Page: {Page}\n" +
                   $"Jan: {Jan}";
        }
    }
}
