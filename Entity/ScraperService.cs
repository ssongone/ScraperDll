using HtmlAgilityPack;
using System.Numerics;

namespace ScraperDll.Entity
{
    public static class PolicyString
    {
        public static string URL_LIST_BOOK = "https://www.e-hon.ne.jp/bec/SE/Genre?dcode=06&";
        public static string URL_LIST_MAGAZINE = "https://www.e-hon.ne.jp/bec/ZS/ZSRank";
    }

    public interface ScrapeService { 
        public string GenerateListUrl(int option);
        public Publication ConvertSummaryToPublication(PublicationSummary summary, HtmlDocument document);
        //public string GenerateDescription(Publication publication, HtmlDocument document);
        
        public string GenerateDescriptionTable(Publication publication);
        public string GenerateDescriptionDetail(HtmlDocument document);
    }

    public class BookService : ScrapeService
    {
        public string GenerateListUrl(int option)
        {
            string stringOption = option.ToString("D2");
            string result = $"ccode={stringOption}&Genre_id=06{stringOption}00";
            return PolicyString.URL_LIST_BOOK + result;
        }

/*        public async Task<List<Book>> GetPublicationDetailListAsync(List<PublicationSummary> summaries)
        {
            List<Book> books = new List<Book>(summaries.Count);
            return books;
        }*/

        public Publication ConvertSummaryToPublication(PublicationSummary summary, HtmlDocument document)
        {
            Publication book = new Publication();

            string title = book.ScrapeText(document, "p.itemTitle");
            string author = book.ScrapeText(document, "ui.AuthorsName");
            string date = book.ScrapeDate(document);
            string page = book.ScrapeText(document, "div.mainItemTable tr:contains(頁数) td");
            string ISBN = book.ScrapeText(document, "//div[contains(@class, 'mainItemTable')]//tr[td[contains(text(), 'ISBN')]]//span[contains(@class, 'codeSelect')]");
            string price = book.ScrapePrice(document);
            string publisher = book.ScrapeText(document, "div.mainItemTable tr:contains(出版社名) td");
            string mainImageUrl = book.ScrapeMainImageUrl(document);

            book.Title = title;
            book.Author = author;
            book.Date = date;
            book.Page = page;
            book.Publisher = publisher;
            book.MainImageUrl = mainImageUrl;
            book.ISBN = ISBN;
            book.Price = ((int.Parse(price) * ScraperConfig.BOOK_MARGIN) / 10 * 10).ToString();

            return book;
        }

        public string GenerateDescriptionTable(Publication publication)
        {
            return "";
        }
        
        public string GenerateDescriptionDetail(HtmlDocument document)
        {
            return "";
        }

    }

    public class MagazineService : ScrapeService
    {
        public string GenerateListUrl(int option) // 20, 40, 60, 80
        {
            option -= 1;
            if (option == 0)
                return PolicyString.URL_LIST_MAGAZINE;

            option = option * 20;
            string result = $"?listcnt={option}";
            return PolicyString.URL_LIST_MAGAZINE + result;
        }
        public Publication ConvertSummaryToPublication(PublicationSummary summary, HtmlDocument document)
        {
            Publication publication = new Publication();
            return publication;
        }

        public string GenerateDescriptionTable(Publication publication)
        {
            return "";
        }

        public string GenerateDescriptionDetail(HtmlDocument document)
        {
            return "";
        }
    }

    

}
