using System;
using System.IO;
using System.Net;

namespace Downloader
{
    public class DownloadConfiguration
    {
        public DownloadConfiguration()
        {
            MaxTryAgainOnFailover = int.MaxValue;  // the maximum number of times to fail.
            ParallelDownload = true; // download parts of file as parallel or not
            ChunkCount = 1; // file parts to download
            Timeout = 1000; // timeout (millisecond) per stream block reader
            OnTheFlyDownload = false; // caching in-memory mode
            BufferBlockSize = 1024 * 1024 * 2; // usually, hosts support max to 8000 bytes
            MaximumBytesPerSecond = ThrottledStream.Infinite; // No-limitation in download speed
            RequestConfiguration = new RequestConfiguration(); // Default requests configuration
            TempDirectory = "C:\\temp"; // Default chunks path
            SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads");
            RequestConfiguration = new RequestConfiguration
            {
                Accept = "*/*",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36",
                ProtocolVersion = HttpVersion.Version11,
                KeepAlive = false,
                UseDefaultCredentials = false
            };
        }


        /// <summary>
        /// Can fetch file size by HEAD request or must be used GET method to support host.
        /// Default value is True.
        /// </summary>
        public bool AllowedHeadRequest { get; set; } = true;

        /// <summary>
        /// 缓冲区块的大小
        /// </summary>
        public int MinimumBufferBlockSize { get; } = 1024;

        /// <summary>
        /// 是否异步分块下载
        /// </summary>
        public bool ParallelDownload { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// download file without caching chunks in disk. In the other words,
        /// all chunks stored in memory.
        /// </summary>
        public bool OnTheFlyDownload { get; set; }

        /// <summary>
        /// 分块数量
        /// </summary>
        public int ChunkCount { get; set; }

        /// <summary>
        /// 保存目录
        /// </summary>
        public string SaveDirectory { get; set; }
        /// <summary>
        /// 分块临时目录
        /// </summary>
        public string TempDirectory { get; set; }

        /// <summary>
        /// 临时文件后缀
        /// </summary>
        public string TempFilesExtension { get; set; } = ".dsc";

        /// <summary>
        /// 下载成功后清除临时文件
        /// </summary>
        public bool ClearPackageAfterDownloadCompleted { get; set; } = true;

        /// <summary>
        /// 用于块大小的流缓冲区大小
        /// </summary>
        public int BufferBlockSize { get; set; }

        /// <summary>
        /// 下载失败重新尝试次数
        /// </summary>
        public int MaxTryAgainOnFailover { get; set; }

        /// <summary>
        /// 每秒最大流量
        /// </summary>
        public long MaximumBytesPerSecond { get; set; }

        /// <summary>
        /// Custom body of your requests
        /// </summary>
        public RequestConfiguration RequestConfiguration { get; set; }



        public void Validate()
        {
            var maxSpeedPerChunk = MaximumBytesPerSecond / ChunkCount;
            ChunkCount = Math.Max(1, ChunkCount);
            MaximumBytesPerSecond = Math.Max(0, MaximumBytesPerSecond);
            BufferBlockSize = (int)Math.Min(Math.Max(maxSpeedPerChunk, BufferBlockSize), BufferBlockSize);
        }
    }
}
