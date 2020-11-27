using Downloader;
using Masuit.Tools.Net;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DownLoadMP3
{
    public interface IBookService
    {
        Task DownloadBook(BookSeriesItem bookSeries, string Savepath, Action<DownloadService, DownloadProgressChangedEventArgs> TotalProgressChanged = null, Action<DownloadService, AsyncCompletedEventArgs> FileDownloadFinished = null);
       
        BookSeries GetBookinfo(string BookId);
    }
}