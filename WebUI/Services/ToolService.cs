using DownLoadMP3;
using WebUI.Models;
using Masuit.Tools.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downloader;
using System.ComponentModel;

namespace WebUI.Serivces
{
    public class ToolService
    {
        public static Dictionary<string, double> DonLoadProgress { get; set; }
        private readonly IBookService _bookService;

        public ToolService(IBookService bookService)
        {
            DonLoadProgress = new Dictionary<string, double>();
            _bookService = bookService;

        }
        public double GetProgress(string ID)
        {
            if (DonLoadProgress.ContainsKey(ID))
            {
                return DonLoadProgress[ID];
            }
            return 0;
        }
        public static void SetProgress(string ID, float progress)
        {
            if (!DonLoadProgress.ContainsKey(ID))
            {
                DonLoadProgress.Add(ID, 0);
            }
            DonLoadProgress[ID] = progress;
        }
        public BookSeriesModel GetBookSeriesList(string bookid)
        {
            BookSeries book = _bookService.GetBookinfo(bookid);

            return BookSeriesModel.Convert(book);
        }
        public void Download(BookSeriesItemModel bookitem,string Savepath, Action<DownloadService,DownloadProgressChangedEventArgs> TotalProgressChanged = null, Action<DownloadService, AsyncCompletedEventArgs> FileDownloadFinished = null)
        {
            _bookService.DownloadBook(bookitem.ToBookSeriesItem(), Savepath, TotalProgressChanged, FileDownloadFinished);
        }

        private void TotalProgressChanged(BookSeriesItemModel item, DownloadService downloader, DownloadProgressChangedEventArgs e)
        {
            item.Progress = e.ProgressPercentage;
            if (!DonLoadProgress.ContainsKey(item.ID))
            {
                DonLoadProgress.Add(item.ID, 0);
            }
            DonLoadProgress[item.ID] = e.ProgressPercentage;
        }
        private void FileDownloadFinished(BookSeriesItemModel item, DownloadService downloader, AsyncCompletedEventArgs e)
        {
            item.Progress = 100;

            DonLoadProgress[item.ID] = 100;
        }
    }
}
