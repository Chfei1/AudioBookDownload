﻿using CsharpHttpHelper;
using HtmlAgilityPack;
using Masuit.Tools.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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


        public void DownloadBook(BookSeriesItem bookSeries, string Savepath, Action<MultiThreadDownloader> TotalProgressChanged = null, Action<MultiThreadDownloader> FileDownloadFinished = null)
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
                    _downLoadMp3(url, bookSeries.Name+ extension,  Savepath, Ip, TotalProgressChanged, FileDownloadFinished);
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


        private void _downLoadMp3(string url, string name, string Savepath, string Ip, Action<MultiThreadDownloader> TotalProgressChanged, Action<MultiThreadDownloader> FileDownloadFinished)
        {
            try
            {
                Directory.CreateDirectory(Savepath);


                var mtd = new MultiThreadDownloader(url, Path.Combine(Savepath, name), 3);
                mtd.Configure(item =>
                {
                    item.Headers.Add("X-Forwarded-For", Ip);
                    item.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                    item.Host = "e1.ixinmo.com";
                    //CookieContainer cookie= new CookieContainer();
                    item.Headers.Add("Cookie", "__cfduid=d8407223173229c4c785f13d7eec632fe1602592716");
                    //item.CookieContainer.Add(new System.Net.Cookie("__cfduid", "d8407223173229c4c785f13d7eec632fe1602592716"));
                    item.Referer = "http://m.ixinmo.com/";
                    item.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                    item.Accept = "*/*";
                    item.ContentType = null;
                    item.KeepAlive = false;
                    item.Timeout = 30 * 1000;

                });
                //mtd.TotalProgressChanged = TotalProgressChanged;
                mtd.TotalProgressChanged += (sender) =>
                {
                    if (TotalProgressChanged != null)
                    {
                        TotalProgressChanged(sender);
                    }
                    //var downloader = sender;
                    //Console.WriteLine("下载进度：" + downloader.TotalProgress + "%");
                    //Console.WriteLine("下载速度：" + downloader.TotalSpeedInBytes / 1024 / 1024 + "MBps");
                };
                mtd.FileDownloadFinished += (sender, e) =>
                {
                    if (FileDownloadFinished != null)
                    {
                        FileDownloadFinished((MultiThreadDownloader)sender);
                    }
                    //Console.WriteLine($"下载完成 {e}");
                };
                mtd.Start();
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