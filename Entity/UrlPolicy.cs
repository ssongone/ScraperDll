using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScraperDll.Entity
{
    public static class PolicyString
    {
        public static string URL_LIST_BOOK = "https://www.e-hon.ne.jp/bec/SE/Genre?dcode=06&";
        public static string URL_LIST_MAGAZINE = "https://www.e-hon.ne.jp/bec/ZS/ZSRank";
    }

    public interface ListUrlPolicy
    {
        string MakeListUrl(int option);
    }

    public class BookUrlPolicy : ListUrlPolicy
    {
        public string MakeListUrl(int option)
        {
            string stringOption = option.ToString("D2");
            string result = $"ccode={stringOption}&Genre_id=06{stringOption}00";
            return PolicyString.URL_LIST_BOOK + result;
        }
    }

    public class MagazineUrlPolicy : ListUrlPolicy
    {
        public string MakeListUrl(int option) // 20, 40, 60, 80
        {
            if (option == 0)
                return PolicyString.URL_LIST_MAGAZINE;

            option = option * 20;
            string result = $"?listcnt={option}";
            return PolicyString.URL_LIST_MAGAZINE + result;
        }
    }

    

}
