using HtmlAgilityPack;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using WebScraping;

class Program
{
    static string URL = "https://ineichen.com/auctions/past/";

    public static HtmlNode getElementByXPath(string XPath, HtmlNode htmlNode)
    {
        return htmlNode.SelectSingleNode(XPath);
    }

    public static string descriptionFormatter(string description)
    {
        return Regex.Replace(description.Trim(), @"[ \t]*\n[ \t]*(\n[ \t]*)*", "\n");
    }

    public static string regexMatcher(string input, string pattern)
    {
        Match match = Regex.Match(input, pattern);
        if (match.Success)
            return match.Value.Trim();
        else
            return "0";
    }

    public static void Main(string[] args)
    {
        var htmlClient = new HttpClient();
        var html = htmlClient.GetStringAsync(URL).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        AuctionDB.truncateAuction();

        var auctionItemList = htmlDocument.DocumentNode.SelectNodes("//div[@class='auctions-list']//div[@class='auction-item']");

        Auction auction = new Auction();
        foreach (var item in auctionItemList)
        {
            auction = new Auction();
            string className = "auction-item__";

            #region ImageURL,Title Extraction
            auction.ImageUrl = getElementByXPath($".//a[@class='{className}image']//img[@src]", item).Attributes["src"]?.Value;
            auction.Title = getElementByXPath($".//div[@class='{className}title']//h2[@class='{className}name']//a[@href]", item).InnerText.Trim();
            #endregion

            #region Button Link and LotCount Extraction
            HtmlNode viewBtn = getElementByXPath($".//div[@class='{className}btns']//a[@href]", item);
            auction.Link = viewBtn.Attributes["href"]?.Value;
            auction.LotCount = int.Parse(regexMatcher(viewBtn.InnerText, @"\d+"));
            #endregion

            className = "auction-date-location";

            #region Description,Location Extraction
            auction.Description = descriptionFormatter(getElementByXPath($".//div[@class='{className}']", item).InnerText);
            auction.Location = regexMatcher(auction.Description, @"\n.*$");
            #endregion

            string dataTime = regexMatcher(auction.Description, @"^.*\r?\n");

            #region Date Extraction
            MatchCollection dateRegex = Regex.Matches(dataTime, @"\b\d{1,2}\b");
            if (dateRegex.Count() == 1)
                auction.StartDate = auction.EndDate = int.Parse(dateRegex.ElementAt(0).Value);
            else if (dateRegex.Count() == 2)
            {
                auction.StartDate = int.Parse(dateRegex.ElementAt(0).Value);
                auction.EndDate = int.Parse(dateRegex.ElementAt(1).Value);
            }
            #endregion

            #region year Extracted
            auction.StartYear = auction.EndYear = int.Parse(regexMatcher(dataTime, @"\d{4}"));
            #endregion

            #region Month Extraction
            MatchCollection monthRegex = Regex.Matches(dataTime, @"[a-zA-z]+");
            if (monthRegex.Count() == 1)
                auction.StartMonth = auction.EndMonth = monthRegex.ElementAt(0).Value;
            else if (monthRegex.Count() == 2)
            {
                auction.StartMonth = monthRegex.ElementAt(0).Value;
                auction.EndMonth = monthRegex.ElementAt(1).Value;
            }
            #endregion

            #region Time Extraction
            MatchCollection timeRegex = Regex.Matches(auction.Description, @"\d{1,2}:\d{1,2}");
            if (timeRegex.Count() == 1)
                auction.StartTime = TimeSpan.Parse(timeRegex.ElementAt(0).Value);
            else if (timeRegex.Count() == 2)
            {
                auction.StartTime = TimeSpan.Parse(timeRegex.ElementAt(0).Value);
                auction.EndTime = TimeSpan.Parse(timeRegex.ElementAt(1).Value);
            }
            #endregion

            AuctionDB.insertIntoAuction(auction);
        }
        Console.WriteLine("Crawed Successfully");
    }
}