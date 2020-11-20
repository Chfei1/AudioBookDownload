using Masuit.Tools.Net;
using System;

namespace DownLoadMP3
{
    public interface IBookService
    {
        void DownloadBook(BookSeriesItem bookSeries, string Savepath, Action<MultiThreadDownloader> TotalProgressChanged = null, Action<MultiThreadDownloader> FileDownloadFinished = null);
        BookSeries GetBookinfo(string BookId);
    }
}