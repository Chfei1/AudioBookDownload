
declare let axios;

declare let Vue;
declare let Qs;
enum DownloadStatus {
    None,
    /**空闲中 */
    Free,
    /**队列中 */
    Waiting,
    /**下载中 */
    Downing,
    /**下载完成 */
    Finished,
    /**暂停中 */
    Pause,
    /**错误 */
    Error
}
interface BookSeriesModel {
    bookId?: string;
    bookName?: string;
    seriesList?: Array<BookSeriesItemModel>;
}
interface BookSeriesItemModel {
    id?: string;
    bookId?: string;
    name?: string;
    url?: string;
    progress?: number;
    status: DownloadStatus;
}
var app = new Vue({
    el: '#app',
    data: {
        bookId: '',
        book: {}
    },
    methods: {
        getinfo() {
            if (this.bookId.length > 0) {
                axios.get('/api/getinfo?id=' + this.bookId)
                    .then(result => {
                        this.book = result.data;
                    })
                    .catch();
            }
        },
        download(item: BookSeriesItemModel) {
            axios.post('/api/download', Qs.stringify({ "bookId": item.bookId, "itemId": item.id })
            )
                .then(res => {
                    console.log(res.data);
                })
        },
        getproggress(status: DownloadStatus) {
            const time = setInterval(function () {
                axios.get('/api/getprogress?status=' + status)
                    .then(result => {
                        const data = result.data as Array<BookSeriesItemModel>;
                        if (data.length > 0) {
                            for (var i = 0; i < data.length; i++) {
                                const _item = data[i];
                                var item = app.book.seriesList.filter(a => a.id == _item.id)[0];
                              
                                item.progress = _item.progress;

                            }
   
                        }
                        //item.progress = result.data;
                        //if (item.progress >= 100) {
                        //    clearInterval(time);
                        //}

                    })
                    .catch(a => {

                    });
            }, 1000);
        }
    }
})
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