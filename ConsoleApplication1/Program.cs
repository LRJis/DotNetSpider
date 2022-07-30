using System;
using System.IO;
using System.Net;
using System.Threading;
using OpenQA.Selenium.Edge;
using LoadJS;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            EdgeOptions options = new EdgeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--nogpu");
            
            using (EdgeDriver driver = new EdgeDriver(options))
            {
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl("https://docs.microsoft.com/zh-cn/cpp/cpp/welcome-back-to-cpp-modern-cpp?view=msvc-170");
                string script = LocalFile.Load("main.js");
                
                driver.ExecuteScript(script);
                Thread.Sleep(500);
                
                driver.ExecuteScript(LocalFile.Load("click.js"));
                Thread.Sleep(1000);
                // LocalFile.WriteIn(@"file\main.html", driver.PageSource);
                Spider spider = new Spider(driver.PageSource);
                spider.Distribute();
                Console.ReadKey();
            }
        }
    }

    public class Spider
    {
        public IDocument Document;

        public Spider(string content)
        {
            HtmlParser htmlParser = new HtmlParser();
            Document = htmlParser.ParseDocument(content);
        }

        public void Distribute()
        {
            foreach (IElement element in Document.QuerySelectorAll("li[role=none]"))
            {
                IElement element2 = element.QuerySelector("a");
                if (element2==null) continue;
                string url = element2.GetAttribute("href");
                string file = element2.TextContent+".html";
                if (url == null)
                {
                    Console.WriteLine("Invalid!");
                    continue;
                }
                new Downloader(url, file).Work();
            }
        }
    }

    public class Downloader
    {
        public string Url { get; set; }
        public string Path;

        public Downloader(string url, string file)
        {
            Url = url;
            Path = file;
        }

        public async void Work()
        {
            int n = 0;
            while (n <= 3)
            {
                try
                {
                    Console.WriteLine($"Work at:{Url}!");
                    HttpWebRequest webRequest = WebRequest.CreateHttp(Url);
                    WebResponse webResponse = webRequest.GetResponse();
                    Stream stream = webResponse.GetResponseStream();
                    if (stream == null) return;
                    StreamReader streamReader = new StreamReader(stream);
                    string content = await streamReader.ReadToEndAsync();
                    Console.WriteLine($"End at {Url}!");
                    LocalFile.WriteIn($@"file\{Path}", content);
                    Console.WriteLine($"Write into {Path} finished....");
                    break;
                }
                catch (WebException)
                {
                    Console.WriteLine($"failed.try again at {Url}");
                    n++;
                }
            }
        }
    }
}