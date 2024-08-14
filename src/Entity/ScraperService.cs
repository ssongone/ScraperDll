using HtmlAgilityPack;
using System.Numerics;
using System.Text;

namespace ScraperDll.Entity
{
    public static class PolicyString
    {
        public static string URL_LIST_BOOK = "https://www.e-hon.ne.jp/bec/SE/Genre?dcode=06&";
        public static string URL_LIST_MAGAZINE = "https://www.e-hon.ne.jp/bec/ZS/ZSRank";
    }

    public interface ScrapePolicy { 
        public string GenerateListUrl(int option);
        public Publication ConvertSummaryToPublication(PublicationSummary summary, HtmlDocument document);        
        public string GenerateDescriptionTable(Publication publication);
        public string GenerateDescriptionDetail(HtmlDocument document);
    }

    public class BookPolicy : ScrapePolicy
    {
        public string GenerateListUrl(int option)
        {
            string stringOption = option.ToString("D2");
            string result = $"ccode={stringOption}&Genre_id=06{stringOption}00";
            return PolicyString.URL_LIST_BOOK + result;
        }

        public Publication ConvertSummaryToPublication(PublicationSummary summary, HtmlDocument document)
        {
            Publication book = new Publication();

            string title = book.ScrapeText(document, "//p[@class='itemTitle']");
            string author = book.ScrapeText(document, "//ul[@class='AuthorsName']//a"); 
            string date = book.ScrapeDate(document, "出版年月");
            string page = book.ScrapeText(document, "//div[@class='mainItemTable']//tr[th[contains(text(), '頁数')]]//td");
            string ISBN = book.ScrapeText(document, "//div[@class='mainItemTable']//tr[th[contains(text(), 'ISBNコード')]]//span[@class='codeSelect']");
            string price = book.ScrapePrice(document); 
            string publisher = book.ScrapeText(document, "//div[@class='mainItemTable']//tr[th[contains(text(), '出版社名')]]//td");
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
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<h2 class=\"heading02\">도서정보</h2>");
            builder.AppendLine("<div class=\"mainItemTable\">");
            builder.AppendLine("<div><strong>제목:</strong> " + publication.Title + "</div>");
            builder.AppendLine("<div><strong>저자:</strong> " + publication.Author + "</div>");
            builder.AppendLine("<div><strong>출판사:</strong> " + publication.Publisher + "</div>");
            builder.AppendLine("<div><strong>발매일:</strong> " + publication.Date + "</div>");
            builder.AppendLine("<div><strong>ISBN:</strong>" + publication.ISBN + "</div>");
            builder.AppendLine("<div><strong>페이지 수:</strong> " + publication.Page + "</div>");
            builder.AppendLine("</div>");
            return builder.ToString();
        }

        public string GenerateDescriptionDetail(HtmlDocument document)
        {
            StringBuilder builder = new StringBuilder();

            var authorContents = document.DocumentNode.SelectNodes("//div[@class='authorContents']//dl[@class='authorDescList']");
            var itemDetailTable = document.DocumentNode.SelectSingleNode("//div[@class='itemDetailTable']//tr[contains(., '要旨')]//td");
            var itemDetailTable2 = document.DocumentNode.SelectSingleNode("//div[@class='itemDetailTable']//tr[contains(., '目次')]//td");
            var itemDetailTable3 = document.DocumentNode.SelectSingleNode("//div[@class='itemDetailTable']//tr[contains(., '文学賞情報')]//td");
            var commentContents = document.DocumentNode.SelectSingleNode("//div[@class='commentContents']//div[@class='commentDetail']");
            var netGalleyReviews = document.DocumentNode.SelectNodes("//div[@id='netgalley']//div[@class='tx_area01']//div[@class='review']");

            if (authorContents != null && authorContents.Count > 0)
            {
                builder.AppendLine("<h2 class=\"heading02\">저자 소개</h2>");
                foreach (var element in authorContents)
                {
                    var authorName = element.SelectSingleNode(".//dt")?.InnerText.Trim();
                    var authorDesc = element.SelectSingleNode(".//dd")?.InnerText.Trim();
                    builder.AppendLine("<div><strong>저자:</strong> " + authorName + "<br>" + authorDesc + "</div>");
                    builder.AppendLine("<br>");
                }
                builder.AppendLine("<br>");
            }

            if (itemDetailTable != null)
            {
                builder.AppendLine("<h2 class=\"heading02\">상품 내용</h2>");
                builder.AppendLine("<div><strong>요약:</strong> " + itemDetailTable.InnerText.Trim() + "</div>");
                builder.AppendLine("<br>");
            }

            if (itemDetailTable2 != null)
            {
                builder.AppendLine("<div><strong>목차:</strong> " + itemDetailTable2.InnerText.Trim() + "</div>");
                builder.AppendLine("<br>");
            }

            if (itemDetailTable3 != null)
            {
                builder.AppendLine("<div><strong>문학상 정보:</strong> " + itemDetailTable3.InnerText.Trim() + "</div>");
                builder.AppendLine("<br><br>");
            }

            if (commentContents != null)
            {
                builder.AppendLine("<h2 class=\"heading02\">코멘트</h2>");
                builder.AppendLine("<div><strong>출판사 메이커 코멘트:</strong> " + commentContents.InnerText.Trim() + "</div>");
                builder.AppendLine("<br>");
            }

            if (netGalleyReviews != null && netGalleyReviews.Count > 0)
            {
                builder.AppendLine("<h2 class=\"heading02\">NetGalley 회원 리뷰</h2>");
                foreach (var element in netGalleyReviews)
                {
                    var reviewer = element.SelectSingleNode(".//div[@class='reviewerclass']//span")?.InnerText.Trim();
                    var reviewText = element.SelectSingleNode(".//p[@class='f_tx']")?.InnerText.Trim();
                    builder.AppendLine("<div><strong>" + reviewer + "</strong> <br>");
                    builder.AppendLine(reviewText);
                    builder.AppendLine("<br>");
                }
                builder.AppendLine("<br><br>");
            }

            return builder.ToString();
        }

    }

    public class MagazinePolicy : ScrapePolicy
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
            Publication magazine = new Publication();

            string title = magazine.ScrapeText(document, "//p[@class='itemTitle']");
            string date = magazine.ScrapeDate(document, "発売日");
            string jan = magazine.ScrapeText(document, "//div[@class='mainItemTable']//tr[th[contains(text(), 'JAN')]]//td");
            string ISBN = magazine.ScrapeText(document, "//div[@class='mainItemTable']//tr[th[contains(text(), '雑誌コード')]]//td");
            string price = magazine.ScrapePrice(document);
            string publisher = magazine.ScrapeText(document, "//div[@class='mainItemTable']//tr[th[contains(text(), '出版社名')]]//td");
            string mainImageUrl = magazine.ScrapeMainImageUrl(document);

            magazine.Title = title;
            magazine.Date = date;
            magazine.Jan = jan;
            magazine.Publisher = publisher;
            magazine.MainImageUrl = mainImageUrl;
            magazine.ISBN = ISBN;
            magazine.Price = ((int.Parse(price) * ScraperConfig.MAGAZINE_MARGIN) / 10 * 10).ToString();
            return magazine;
        }

        public string GenerateDescriptionTable(Publication publication)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<h2 class=\"heading02\">잡지정보</h2>");
            builder.AppendLine("<div class=\"mainItemTable\">");
            builder.AppendLine("<div><strong>제목:</strong> " + publication.Title + "</div>");
            builder.AppendLine("<div><strong>출판사:</strong> " + publication.Publisher + "</div>");
            builder.AppendLine("<div><strong>발매일:</strong> " + publication.Date + "</div>");
            builder.AppendLine("<div><strong>잡지 코드:</strong>" + publication.ISBN + "</div>");
            builder.AppendLine("<div><strong>잡지 JAN:</strong> " + publication.Jan + "</div>");
            builder.AppendLine("</div>");
            return builder.ToString();
        }


        public string GenerateDescriptionDetail(HtmlDocument document)
        {
            var firstSection = document.DocumentNode.SelectSingleNode("//div[@class='itemDetailTable']//tr[td[contains(text(), '雑誌銘柄情報')]]/td");
            var secondSection = document.DocumentNode.SelectSingleNode("//div[@class='itemDetailTable']//tr[td[contains(text(), '特集情報')]]/td");
            var thirdSection = document.DocumentNode.SelectSingleNode("//div[@class='itemDetailTable']//tr[td[contains(text(), '出版社情報')]]/td");
            if (firstSection == null && secondSection == null && thirdSection == null)
            {
                return "";
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("<h2 class=\"heading02\">상품내용</h2>\n");
            if (firstSection != null)
            {
                builder.Append("<div><strong>잡지유명상표정보:</strong> ").Append(firstSection.InnerText.Trim()).Append("</div>\n");
            }

            if (secondSection != null)
            {
                builder.Append("<div><strong>특집 정보:</strong> ").Append(secondSection.InnerText.Trim()).Append("</div>\n");
            }

            if (thirdSection != null)
            {
                builder.Append("<div><strong>출판사:</strong> ").Append(thirdSection.InnerText.Trim()).Append("</div>\n");
            }
            return builder.ToString();
        }
    }
}
