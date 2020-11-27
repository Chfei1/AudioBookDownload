using CsharpHttpHelper;
using HtmlAgilityPack;
using Masuit.Tools.Net;
using System;
using System.Collections.Generic;
using System.IO;

using System.Text;
using System.Text.RegularExpressions;
using Downloader;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DownLoadMP3
{
    public class XMTSService : IBookService
    {
        public BookSeries GetBookinfo(string BookId)
        {
            BookSeries bookSeries = new BookSeries();
            bookSeries.BookId = BookId;
            List<BookSeriesItem> list = new List<BookSeriesItem>();
            HttpItem item = new HttpItem();
            item.Method = "GET";
            item.URL = $"http://m.ixinmo.com/shu/{BookId}.html";
            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(result.Html);
                HtmlNode body = document.DocumentNode;

                HtmlNode nameNode = body.SelectSingleNode("//span[@class=\"bt\"]");
                bookSeries.BookName = nameNode.InnerText;

                HtmlNodeCollection listNode = body.SelectNodes("//*[@id=\"playlist\"]/ul/li");
                foreach (HtmlNode seriesNode in listNode)
                {
                    HtmlNode node = seriesNode.SelectSingleNode(".//a");
                    BookSeriesItem series = new BookSeriesItem();
                    series.BookId = BookId;
                    series.Name = node.InnerText;
                    series.Url = node.GetAttributeValue("href", "");
                    list.Add(series);
                }
                bookSeries.SeriesList = list;
            }

            return bookSeries;
        }


        public async Task DownloadBook(BookSeriesItem bookSeries, string Savepath, Action<DownloadService, DownloadProgressChangedEventArgs> TotalProgressChanged = null, Action<DownloadService, AsyncCompletedEventArgs> FileDownloadFinished = null)
        {
            string IpTemp = "145.213.{0}.{1}";
            Random ra = new Random();
            string Ip = string.Format(IpTemp, ra.Next(1, 255), ra.Next(1, 255));
            string html = _downLoadHtml(bookSeries.BookId, bookSeries.Url, Ip);
            if (!string.IsNullOrWhiteSpace(html))
            {
                Match match = Regex.Match(html, "<source src=\"(?<url>.+?)\"");

                if (match.Success && match.Groups["url"].Success)
                {
                    string url = match.Groups["url"].Value;
                    string extension = Path.GetExtension(url);
                    await _downLoadMp3(url, bookSeries.Name + extension, Savepath, Ip, TotalProgressChanged, FileDownloadFinished);
                    //byte[] bytes = _downLoadMp3(url, Ip);
                    //if (bytes != null)
                    //{
                    //    SaveMp3(bytes, index);
                    //}
                    //else
                    //{
                    //    throw new Exception("下载地址获取失败");
                    //}
                }
            }
        }






        private string _downLoadHtml(string BookId, string BookSeriesUrl, string Ip)
        {

            string unix = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string md5 = MD5_32($"1234{unix}115599");
            HttpItem item = new HttpItem();

            item.Header.Add("X-Forwarded-For", Ip);
            item.Host = "m.ixinmo.com";
            item.Cookie = $"__cfduid=d8407223173229c4c785f13d7eec632fe1602592716; ooo={unix}|{md5}";
            item.Referer = $"http://m.ixinmo.com/shu/{BookId}.html";
            item.URL = BookSeriesUrl;
            item.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
            item.ResultCookieType = CsharpHttpHelper.Enum.ResultCookieType.String;
            item.KeepAlive = false;
            item.Accept = "*/*";
            item.ContentType = "";
            item.Timeout = 10 * 1000;
            item.Header.Add("Accept-Language", "zh-CN,zh;q=0.9");
            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.Html;
            }
            return "";
        }


        private async Task _downLoadMp3(string url, string name, string Savepath, string Ip, Action<DownloadService, DownloadProgressChangedEventArgs> TotalProgressChanged, Action<DownloadService, AsyncCompletedEventArgs> FileDownloadFinished)
        {
            try
            {
                Directory.CreateDirectory(Savepath);

                DownloadConfiguration configuration = new DownloadConfiguration();
                configuration.SaveDirectory = Savepath;
                configuration.RequestConfiguration.Headers.Add("X-Forwarded-For", Ip);
                configuration.RequestConfiguration.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                configuration.RequestConfiguration.Host = "e1.ixinmo.com";
                configuration.RequestConfiguration.Headers.Add("Cookie", "__cfduid=d8407223173229c4c785f13d7eec632fe1602592716");
                configuration.RequestConfiguration.Referer = "http://m.ixinmo.com/";
                configuration.RequestConfiguration.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                configuration.RequestConfiguration.Accept = "*/*";
                configuration.RequestConfiguration.ContentType = null;
                configuration.RequestConfiguration.KeepAlive = false;
                configuration.RequestConfiguration.Timeout = 30 * 1000;


                DownloadService dService = new DownloadService(configuration);
                dService.DownloadProgressChanged += (sender, e)=>{

                    TotalProgressChanged((DownloadService)sender, e);
                };
                dService.DownloadFileCompleted += (sender, e) =>
                {
                    FileDownloadFinished((DownloadService)sender, e);
                };
                await dService.DownloadFileAsync(url, name);




                //string packageID = DownloadManager.AddTask(url, name, Savepath, 4, 1);
                //DownloadManager.UpdatePackage(packageID, (package) =>
                //{
                //    package.Options.RequestConfiguration.Headers.Add("X-Forwarded-For", Ip);
                //    package.Options.RequestConfiguration.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                //    package.Options.RequestConfiguration.Host = "e1.ixinmo.com";
                //    package.Options.RequestConfiguration.Headers.Add("Cookie", "__cfduid=d8407223173229c4c785f13d7eec632fe1602592716");
                //    package.Options.RequestConfiguration.Referer = "http://m.ixinmo.com/";
                //    package.Options.RequestConfiguration.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                //    package.Options.RequestConfiguration.Accept = "*/*";
                //    package.Options.RequestConfiguration.ContentType = null;
                //    package.Options.RequestConfiguration.KeepAlive = false;
                //    package.Options.RequestConfiguration.Timeout = 30 * 1000;

                //});
                //var mtd = new MultiThreadDownloader(url, Path.Combine(Savepath, name), 3);
                //mtd.Configure(item =>
                //{
                //    item.Headers.Add("X-Forwarded-For", Ip);
                //    item.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                //    item.Host = "e1.ixinmo.com";
                //    //CookieContainer cookie= new CookieContainer();
                //    item.Headers.Add("Cookie", "__cfduid=d8407223173229c4c785f13d7eec632fe1602592716");
                //    //item.CookieContainer.Add(new System.Net.Cookie("__cfduid", "d8407223173229c4c785f13d7eec632fe1602592716"));
                //    item.Referer = "http://m.ixinmo.com/";
                //    item.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                //    item.Accept = "*/*";
                //    item.ContentType = null;
                //    item.KeepAlive = false;
                //    item.Timeout = 30 * 1000;

                //});
                ////mtd.TotalProgressChanged = TotalProgressChanged;
                //mtd.TotalProgressChanged += (sender) =>
                //{
                //    if (TotalProgressChanged != null)
                //    {
                //        TotalProgressChanged(sender);
                //    }
                //    //var downloader = sender;
                //    //Console.WriteLine("下载进度：" + downloader.TotalProgress + "%");
                //    //Console.WriteLine("下载速度：" + downloader.TotalSpeedInBytes / 1024 / 1024 + "MBps");
                //};
                //mtd.FileDownloadFinished += (sender, e) =>
                //{
                //    if (FileDownloadFinished != null)
                //    {
                //        FileDownloadFinished((MultiThreadDownloader)sender);
                //    }
                //    //Console.WriteLine($"下载完成 {e}");
                //};
                //mtd.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveMp3(byte[] bytes, int index)
        {
            string name = namedic.getname(index);
            string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download", $"{index}_{name}.m4a");
            using (FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Dispose();
            }

        }

        string MD5_32(string str)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
    }
}
