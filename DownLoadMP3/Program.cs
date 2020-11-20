using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsharpHttpHelper;
using Masuit.Tools.Net;

namespace DownLoadMP3
{
    class Program
    {
        static string BookId = "";
        static int StartIndex = 0;
        static int EndIndex = 0;
        static void Main(string[] args)
        {
            //XMTSService xMTS = new XMTSService();
            //BookSeries book = xMTS.GetBookinfo("315");
            //xMTS.DownloadBook(book.BooklId, book.SeriesList[0], @"E:\download",
            //    (MultiThreadDownloader downloader) =>
            //    {
            //        Console.WriteLine("下载进度：" + downloader.TotalProgress.ToString("f2") + "%");
            //        Console.WriteLine("下载速度：" + downloader.TotalSpeedInBytes / 1024 / 1024 + "MBps");
            //    }, (MultiThreadDownloader a) =>
            //    {
            //        Console.WriteLine($"下载完成");
            //    });
            //Console.ReadKey();
            //xMTS.DownloadBook()
            //Console.Write("输入书籍ID:");
            //BookId = Console.ReadLine();
            //Console.Write("输入开始集数:");
            //StartIndex = int.Parse(Console.ReadLine());
            //Console.Write("输入结束集数:");
            //EndIndex = int.Parse(Console.ReadLine());
            ////Console.WriteLine($" {BookId} {StartIndex} {EndIndex}");

            ////namedic.loadingDirectory();
            //Start();
            //Console.WriteLine("下载完成");
            //Console.ReadKey();
        }
        static int index = 0;
        static string IpTemp = "145.213.{0}.{1}";
        static string Ip = "";
        static void Start()
        {
            for (int i = StartIndex; i <= EndIndex; i++)
            {

                int item = i;
                //}
                //foreach (int item in list)
                //{
                Random ra = new Random();

                Ip = string.Format(IpTemp, ra.Next(1, 255), ra.Next(1, 255));
                index = item;

                log($"正在下载 {index} {namedic.getname(index)}");
                string html = DownLoadHtml(index);
                if (!string.IsNullOrWhiteSpace(html))
                {
                    log($"    获取Mp3地址");
                    Match match = Regex.Match(html, "<source src=\"(?<url>.+?)\"");
                    if (match.Success && match.Groups["url"].Success)
                    {
                        string url = match.Groups["url"].Value;
                        byte[] bytes = DownLoadMp3(url);
                        if (bytes != null)
                        {
                            log($"    下载完成");
                            SaveMp3(bytes, index);
                            log($"    保存完成");
                            Console.WriteLine();
                        }
                        else
                        {
                            error("下载地址获取失败");
                        }
                    }
                }
                else
                {
                    error("地址获取失败");
                }
            }
            Console.ReadLine();
        }

        static string DownLoadHtml(int index)
        {

            string unix = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string md5 = MD5_32($"1234{unix}115599");
            HttpItem item = new HttpItem();

            item.Header.Add("X-Forwarded-For", Ip);
            item.Host = "m.ixinmo.com";
            item.Cookie = $"__cfduid=d8407223173229c4c785f13d7eec632fe1602592716; ooo={unix}|{md5}";
            item.Referer = $"http://m.ixinmo.com/shu/{BookId}.html";
            item.URL = $"http://m.ixinmo.com/shu/{BookId}/{index}.html";
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
        static byte[] DownLoadMp3(string url)
        {
            try
            {
                HttpItem item = new HttpItem();
                item.Header.Add("X-Forwarded-For", Ip);
                item.URL = url;
                item.Host = "e1.ixinmo.com";
                item.ResultType = CsharpHttpHelper.Enum.ResultType.Byte;
                item.Cookie = "__cfduid=d8407223173229c4c785f13d7eec632fe1602592716";
                item.Referer = "http://m.ixinmo.com/";
                item.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                item.Accept = "*/*";
                item.ContentType = "";
                item.ContentType = null;
                item.KeepAlive = false;
                item.Header.Add("Accept-Language", "zh-CN,zh;q=0.9");
                item.Timeout = 30 * 1000;
                HttpHelper http = new HttpHelper();
                HttpResult result = http.GetHtml(item);
                if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.PartialContent)
                {
                    if (result.ResultByte != null)
                    {
                        if (result.ResultByte.Length < 11520)
                        {
                            error("错误的地址");
                        }
                        else if (result.ResultByte.Length == 11520)
                        {
                            error("访问过快");
                        }
                        else if (result.ResultByte.Length > 11520)
                        {
                            return result.ResultByte;

                        }


                    }
                }
            }
            catch (Exception ex)
            {
                error($"下载失败:index:{index} name:{namedic.getname(index)} ex:{ex.Message}");
            }

            error($"下载失败:index:{index} name:{namedic.getname(index)}");
            return null;
        }

        static void SaveMp3(byte[] bytes, int index)
        {
            string name = namedic.getname(index);
            string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download", $"{index}_{name}.m4a");
            FileStream fs = new FileStream(savePath, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Dispose();

        }


        static string MD5_32(string str)
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

        static void log(string msg)
        {
            Console.Write(msg);
        }

        static void error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
            StreamWriter wr = new StreamWriter("./error.txt", true, Encoding.UTF8);
            wr.WriteLine(msg);
            wr.Close();
        }
    }
}
