using System;
using System.Threading;

namespace Downloader
{
    public class DownloadPackage
    {
        public string DId { get; set; }
        // ReSharper disable once InconsistentNaming
        protected long _bytesReceived;
        public long BytesReceived
        {
            get => _bytesReceived;
            set => Interlocked.Exchange(ref _bytesReceived, value);
        }
        /// <summary>
        /// 下载总大小
        /// </summary>
        public long TotalFileSize { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 保存目录
        /// </summary>
        public string SaveDirectory { get; set; }
        /// <summary>
        /// 文件保存全名称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 下载状态
        /// </summary>
        public DownloadStatus Status { get; set; }
        /// <summary>
        /// 分块信息
        /// </summary>
        public DownloadService.Chunk[] Chunks { get; set; }
        /// <summary>
        /// 下载地址
        /// </summary>
        public Uri Address { get; set; }
        /// <summary>
        /// 下载配置信息
        /// </summary>
        public DownloadConfiguration Options { get; set; }

        public DownloadPackage()
        {
            DId = Guid.NewGuid().ToString();
            Status = DownloadStatus.Free;
            Options = new DownloadConfiguration();
        }
    }
}
