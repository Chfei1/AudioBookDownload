using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;

namespace Downloader
{
    public class DownloadManager
    {
        /// <summary>
        /// 最大线程数
        /// </summary>
        public static int MaxThread { get; set; }
        /// <summary>
        /// 正在运行的线程
        /// </summary>
        private static int RunThrad = 0;

        /// <summary>
        /// 下载完成事件
        /// </summary>
        public static event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted;
        /// <summary>
        /// 下载进度事件
        /// </summary>
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        /// <summary>
        /// 下载全部完成
        /// </summary>
        public static event EventHandler DownloadAllFileCompleted;

        private static List<DownloadPackage> _downloadPackages = new List<DownloadPackage>();
        public static string AddTask(string url, string name, string savePath = null, int threadCount = 4, int blockSize = 3)
        {
            MaxThread = threadCount;
            DownloadConfiguration configuration = new DownloadConfiguration();
            configuration.SaveDirectory = savePath ?? configuration.SaveDirectory;
            configuration.ChunkCount = blockSize;
            configuration.TempDirectory = "C:\\temp";
            DownloadPackage package = new DownloadPackage
            {
                Options = configuration,
                Address = new Uri(url),
                FileName = name,
                SaveDirectory = configuration.SaveDirectory
            };
            _downloadPackages.Add(package);
            return package.DId;
        }

        public static DownloadPackage GetPackage(string packageID)
        {
            return _downloadPackages.FirstOrDefault(a => a.DId == packageID);
        }
        public static void UpdatePackage(string packageID, Action<DownloadPackage> actionpackage)
        {
            DownloadPackage _package = _downloadPackages.FirstOrDefault(a => a.DId == packageID);
            actionpackage(_package);
        }
        public static void Run(string[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                IEnumerable<DownloadPackage> packages = _downloadPackages.Where(a => ids.Contains(a.DId) && a.Status == DownloadStatus.Waiting);

                foreach (var item in packages)
                {
                    Task.Delay(500);
                    Task.Run(() =>
                    {
                        _run(item);
                    });
                    RunThrad++;
                    while (RunThrad >= MaxThread)
                    {
                        Task.Delay(1000);
                    }
                }
            }
        }
        private static async void _run(DownloadPackage package)
        {
            DownloadService ds = new DownloadService(package.Options);
            ds.DownloadProgressChanged += OnDownloadProgressChanged;
            ds.DownloadFileCompleted += OnDownloadFileCompleted;
            await ds.DownloadFileAsync(package.Address.AbsoluteUri, package.FileName);

        }
        /// <summary>
        /// 下载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            RunThrad--;
            ((DownloadService)sender).Package.Status = DownloadStatus.Finished;
            DownloadFileCompleted?.Invoke(sender, e);
            if (RunThrad == 0)
            {
                DownloadAllFileCompleted?.Invoke(null, EventArgs.Empty);
            }
        }
        /// <summary>
        /// 下载进度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(sender, e);
        }


    }
}
