using Downloader;
using Masuit.Tools.Net;
using System;
using System.ComponentModel;

namespace DownLoadMP3
{
    public interface IBookService
    {
        void DownloadBook(BookSeriesItem bookSeries, string Savepath, Action<DownloadService, DownloadProgressChangedEventArgs> TotalProgressChanged = null, Action<DownloadService, AsyncCompletedEventArgs> FileDownloadFinished = null);
       
        BookSeries GetBookinfo(string BookId);
    }
}