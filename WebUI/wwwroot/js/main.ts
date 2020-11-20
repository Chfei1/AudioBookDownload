
declare let axios;

declare let Vue;
declare let Qs;
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
            axios.post('/api/download', Qs.stringify(item), {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            })
                .then(res => {
                    console.log(res.data);
                })
        }
    }
})

declare let signalR;
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/msghub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("msg", (item: BookSeriesItemModel) => {
    var list = app.book.seriesList.filter(a => a.id == item.id);
    if (list.length > 0) {
        list[0].progress = item.progress;
    }
});
connection.start();