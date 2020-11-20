using DownLoadMP3;
using WebUI.Models;
using Masuit.Tools.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Serivces
{
    public class ToolService
    {
        public static Dictionary<string, float> DonLoadProgress { get; set; }
        private readonly IBookService _bookService;

        public ToolService(IBookService bookService)
        {
            DonLoadProgress = new Dictionary<string, float>();
            _bookService = bookService;

        }
        public float GetProgress(string ID)
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
        public void Download(BookSeriesItemModel bookitem,string Savepath, Action<MultiThreadDownloader> TotalProgressChanged = null, Action<MultiThreadDownloader> FileDownloadFinished = null)
        {
            _bookService.DownloadBook(bookitem.ToBookSeriesItem(), Savepath, TotalProgressChanged, FileDownloadFinished);
        }

        private void TotalProgressChanged(BookSeriesItemModel item, MultiThreadDownloader downloader)
        {
            item.Progress = downloader.TotalProgress;
            if (!DonLoadProgress.ContainsKey(item.ID))
            {
                DonLoadProgress.Add(item.ID, 0);
            }
            DonLoadProgress[item.ID] = downloader.TotalProgress;
        }
        private void FileDownloadFinished(BookSeriesItemModel item, MultiThreadDownloader downloader)
        {
            item.Progress = 100;

            DonLoadProgress[item.ID] = 100;
        }
    }
}
