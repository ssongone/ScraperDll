using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScraperDll.Entity
{
    static class ScraperConfig
    {
        public static string DEFAULT_IMAGE_URL = "기본 이미지 주소";
        public static int BOOK_MARGIN = 12;
        public static int MAGAZINE_MARGIN = 12;
    }

    public static class ExcelConfig
    {
        public const string WINDOWS_FILE_PATH = @"C:\ddadduBot\";

        public const int PRODUCT_CODE_COLUMN = 0;
        public const int CATEGORY_CODE_COLUMN = 1;
        public const int PRODUCT_NAME_COLUMN = 2;
        public const int PRODUCT_PRICE_COLUMN = 4;
        public const int PRODUCT_STOCK_COLUMN = 6;
        public const int PRODUCT_MAIN_IMAGE_COLUMN = 17;
        public const int PRODUCT_DESCRIPTION_COLUMN = 19;
        public const int ORIGIN_CODE_COLUMN = 24;
        public const int IMPORTER_COLUMN = 25;
        public const int MINOR_COLUMN = 28;
        public const int DELIVERY_CODE_COLUMN = 29;
        public const int AS_NUMBER_COLUMN = 51;
        public const int AS_INFO_COLUMN = 52;
        public const int ISBN_COLUMN = 75;
        public const int PUBLICATION_DATE_COLUMN = 78;
        public const int PUBLISHER_COLUMN = 79;
        public const int WRITER_COLUMN = 80;
        public const int ILLUSTRATOR_COLUMN = 81;
        public const int INCOME_DEDUCTION_COLUMN = 83;

        public const string CATEGORY_CODE_VALUE = "50005752";
        public const int PRODUCT_STOCK_VALUE = 100;
        public const string ORIGIN_CODE_VALUE = "0200036";
        public const string DELIVERY_CODE_VALUE = "2618409";
        public const string AS_NUMBER_VALUE = "010-7268-5664";
        public const string ILLUSTRATOR_VALUE = "상세페이지 표기";

        public const string Y_VALUE = "Y";
        public const string N_VALUE = "N";
    }

}
