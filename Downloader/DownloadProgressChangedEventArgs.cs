using System;

namespace Downloader
{
    /// <summary>
    /// Provides data for the DownloadService.DownloadProgressChanged event of a
    /// DownloadService.
    /// </summary>
    public class DownloadProgressChangedEventArgs : EventArgs
    {
        public DownloadProgressChangedEventArgs(string id, long totalBytesToReceive, long bytesReceived, long bytesPerSecond)
        {
            ProgressId = id;
            TotalBytesToReceive = totalBytesToReceive;
            BytesReceived = bytesReceived;
            ProgressPercentage = (double)bytesReceived * 100 / totalBytesToReceive;
            BytesPerSecondSpeed = bytesPerSecond;
        }

        /// <summary>
        /// Progress unique identity
        /// </summary>
        public string ProgressId { get; }

        /// <summary>
        /// 进度百分比
        /// </summary>
        /// <returns></returns>
        public double ProgressPercentage { get; }

        /// <summary>
        /// 接收的字节数
        /// </summary>
        /// <returns></returns>
        public long BytesReceived { get; }

        /// <summary>
        /// 下载总大小
        /// </summary>
        /// <returns>An System.Int64 value that indicates the number of bytes that will be received.</returns>
        public long TotalBytesToReceive { get; }

        /// <summary>
        /// 每秒速度
        /// </summary>
        public long BytesPerSecondSpeed { get; }
    }
}
