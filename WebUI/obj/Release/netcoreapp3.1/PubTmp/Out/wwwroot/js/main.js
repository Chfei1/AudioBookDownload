var DownloadStatus;
(function (DownloadStatus) {
    DownloadStatus[DownloadStatus["None"] = 0] = "None";
    /**空闲中 */
    DownloadStatus[DownloadStatus["Free"] = 1] = "Free";
    /**队列中 */
    DownloadStatus[DownloadStatus["Waiting"] = 2] = "Waiting";
    /**下载中 */
    DownloadStatus[DownloadStatus["Downing"] = 3] = "Downing";
    /**下载完成 */
    DownloadStatus[DownloadStatus["Finished"] = 4] = "Finished";
    /**暂停中 */
    DownloadStatus[DownloadStatus["Pause"] = 5] = "Pause";
    /**错误 */
    DownloadStatus[DownloadStatus["Error"] = 6] = "Error";
})(DownloadStatus || (DownloadStatus = {}));
var app = new Vue({
    el: '#app',
    data: {
        bookId: '',
        book: {}
    },
    methods: {
        getinfo: function () {
            var _this = this;
            if (this.bookId.length > 0) {
                axios.get('/api/getinfo?id=' + this.bookId)
                    .then(function (result) {
                    _this.book = result.data;
                })["catch"]();
            }
        },
        download: function (item) {
            axios.post('/api/download', Qs.stringify({ "bookId": item.bookId, "itemId": item.id }))
                .then(function (res) {
                console.log(res.data);
            });
        },
        getproggress: function (status) {
            var time = setInterval(function () {
                axios.get('/api/getprogress?status=' + status)
                    .then(function (result) {
                    var data = result.data;
                    if (data.length > 0) {
                        var _loop_1 = function () {
                            var _item = data[i];
                            item = app.book.seriesList.filter(function (a) { return a.id == _item.id; })[0];
                            item.progress = _item.progress;
                        };
                        var item;
                        for (var i = 0; i < data.length; i++) {
                            _loop_1();
                        }
                    }
                    //item.progress = result.data;
                    //if (item.progress >= 100) {
                    //    clearInterval(time);
                    //}
                })["catch"](function (a) {
                });
            }, 1000);
        }
    }
});
app.getproggress(DownloadStatus.None);
//declare let signalR;
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("/msghub")
//    .configureLogging(signalR.LogLevel.Information)
//    .build();
//connection.on("msg", (item: BookSeriesItemModel) => {
//    var list = app.book.seriesList.filter(a => a.id == item.id);
//    if (list.length > 0) {
//        list[0].progress = item.progress;
//    }
//});
//connection.start();
//# sourceMappingURL=main.js.map