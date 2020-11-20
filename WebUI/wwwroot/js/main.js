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
            axios.post('/api/download', Qs.stringify(item), {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            })
                .then(function (res) {
                console.log(res.data);
            });
        }
    }
});
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/msghub")
    .configureLogging(signalR.LogLevel.Information)
    .build();
connection.on("msg", function (item) {
    var list = app.book.seriesList.filter(function (a) { return a.id == item.id; });
    if (list.length > 0) {
        list[0].progress = item.progress;
    }
});
connection.start();
//# sourceMappingURL=main.js.map