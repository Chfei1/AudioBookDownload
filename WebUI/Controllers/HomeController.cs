using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            return Json(book);
        }
        [Route("api/download")]
        [HttpPost]
        public JsonResult Download(BookSeriesItemModel model)
        {
            string SavePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download", model.BookId);
            _toolService.Download(model, SavePath, (e) =>
            {
                TotalProgressChanged(model, e);
            }, (e) =>
            {
                FileDownloadFinished(model, e);
            });
            return Json(true);
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

        private void TotalProgressChanged(BookSeriesItemModel item, MultiThreadDownloader downloader)
        {
            item.Progress = downloader.TotalProgress;
            _hubContext.Clients.All.SendAsync("msg", item);
        }
        private void FileDownloadFinished(BookSeriesItemModel item, MultiThreadDownloader downloader)
        {
            item.Progress = 100;
        }
    }
}
