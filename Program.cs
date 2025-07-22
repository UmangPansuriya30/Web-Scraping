using HtmlAgilityPack;
using System.Net.Http;

class Program
{
    static String URL = "https://ineichen.com/auctions/past/";
    public static void Main(string[] args)
    {
        var htmlClient = new HttpClient();
        var html = htmlClient.GetStringAsync(URL).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        string filePath = "C:/WorkingDirectory/Tasks/WebScraping/index.html";
        //File.WriteAllText(filePath, htmlDocument.ParsedText);

        var auctionItemList = htmlDocument.DocumentNode.SelectNodes("//div[@class='auctions-list']//div[@class='auction-item']");

        File.WriteAllText(filePath,auctionItemList.ElementAt(0).InnerHtml);

    }
}