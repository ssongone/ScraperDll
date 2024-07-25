using HtmlAgilityPack;
using ScraperDll.Entity;
using System.Text;

namespace ScraperDll
{
    public class Scraper
    {
        private static readonly HttpClient client = new HttpClient();
        private ListUrlPolicy listUrlPolicy;

        public Scraper(ListUrlPolicy listUrlPolicy) 
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // 인코딩 등록
            this.listUrlPolicy = listUrlPolicy;
        }

        public async Task<HtmlDocument> GetDocumentAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(contentStream, Encoding.GetEncoding("shift_jis")))
            {
                string pageContents = await reader.ReadToEndAsync();
                var document = new HtmlDocument();
                document.LoadHtml(pageContents);
                return document;
            }
            /*HtmlDocument document = new HtmlDocument();
            document.LoadHtml(pageContents);
            return document;*/
        }

        public async Task<List<PublicationSummary>> ScrapeRankingPage(int option)
        {
            string url = listUrlPolicy.MakeListUrl(option);
            HtmlDocument document = await GetDocumentAsync(url);
            
            // nodes null이면 에러 처리
            var nodes = document.DocumentNode.SelectNodes("//div[@id='contents']//div[@class='detail']//p[@class='title']//a");

            var result = new List<PublicationSummary>();

            foreach (var node in nodes) 
            {
                string name = node.InnerText;
                string hrefValue = node.GetAttributeValue("href", string.Empty);
                result.Add(new PublicationSummary(name, hrefValue));
            }

            return result;
        }




    }


}
