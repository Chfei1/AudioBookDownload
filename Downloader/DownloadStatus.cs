using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Downloader
{
    public enum DownloadStatus
    {
        None,
        /// <summary>
        /// 空闲中
        /// </summary>
        [Description("空闲中")]
        Free,
        /// <summary>
        /// 队列中
        /// </summary>
        [Description("队列中")]
        Waiting,
        /// <summary>
        /// 下载中
        /// </summary>
        [Description("下载中")]
        Downing,
        /// <summary>
        /// 下载完成
        /// </summary>
        [Description("下载完成")]
        Finished,
        /// <summary>
        /// 暂停中
        /// </summary>
        [Description("暂停中")]
        Pause,
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error
    }
}
