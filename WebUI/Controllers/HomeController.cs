using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Downloader;
using Masuit.Tools.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebUI.Models;
using WebUI.Serivces;
using WebUI.Services;
using IO = System.IO;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<MsgHUB> _hubContext;
        private readonly ToolService _toolService;
        private static List<BookSeriesModel> _bookSeries = new List<BookSeriesModel>();
        private static List<BookSeriesItemModel> _downBookSeries = new List<BookSeriesItemModel>();
        private static int MaxThread = 1;
        private static int RunThread = 0;
        private static bool IsRun = false;
        public HomeController(
            IHubContext<MsgHUB> hubContext,
            ToolService toolService)
        {
            _hubContext = hubContext;
            _toolService = toolService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("api/getinfo")]
        [HttpGet]
        public JsonResult GetBookinfo(string id)
        {
            BookSeriesModel book = _toolService.GetBookSeriesList(id);
            _bookSeries.Add(book);
            return Json(book);
        }
        [Route("api/download")]
        [HttpPost]
        public JsonResult Download(string bookId, string itemId)
        {
            BookSeriesModel book = _bookSeries.FirstOrDefault(a => a.BookId == bookId);
            if (book != null)
            {
                BookSeriesItemModel item = book.SeriesList.FirstOrDefault(a => a.ID == itemId);
                if (item != null)
                {
                    item.Status = DownloadStatus.Waiting;
                    _downBookSeries.Add(item);
                    if (!IsRun)
                    {
                        IsRun = true;
                        Task.Run(async () =>
                        {
                            await _Download();
                        });
                    }
                }

            }
            return Json(true);
        }

        private async Task _Download()
        {
            while (_downBookSeries.Count > 0)
            {
                var item = _downBookSeries.First();

                if (item.Status != DownloadStatus.Waiting)
                {
                    continue;
                }
                item.Status = DownloadStatus.Downing;
                string SavePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download", item.BookId);
                await _toolService.Download(item, SavePath, (down, e) =>
                {
                    item.Progress = e.ProgressPercentage;
                    _hubContext.Clients.All.SendAsync("downing", item);
                }, (down, e) =>
                {
                    item.Progress = 100;
                    item.Status = DownloadStatus.Finished;
                    _hubContext.Clients.All.SendAsync("finished", item);
                    _downBookSeries.Remove(item);
                });
                await Task.Delay(1000);
            }
            IsRun = false;
        }
        [Route("api/getprogress")]
        [HttpGet]
        public JsonResult GetDownloadProgress(DownloadStatus status = DownloadStatus.None)
        {
            List<BookSeriesItemModel> items = new List<BookSeriesItemModel>();
            items = _downBookSeries;
            if (status != DownloadStatus.None)
            {
                items = _downBookSeries.Where(a => a.Status == status).ToList();
            }
            return Json(items);
        }

        [Route("api/getdirectory")]
        [HttpGet]
        public JsonResult GetDirectory(string dirpath = "")
        {
            if (string.IsNullOrEmpty(dirpath))
            {
                dirpath = AppDomain.CurrentDomain.BaseDirectory;
            }
            string[] list = IO.Directory.GetDirectories(dirpath);
            return Json(list);
        }

        private void TotalProgressChanged(BookSeriesItemModel item, DownloadProgressChangedEventArgs downloader)
        {
            item.Progress = downloader.ProgressPercentage;
            //_hubContext.Clients.All.SendAsync("msg", item);

        }
        private void FileDownloadFinished(BookSeriesItemModel item, AsyncCompletedEventArgs e)
        {
            item.Progress = 100;
            RunThread--;
        }

    }
}
