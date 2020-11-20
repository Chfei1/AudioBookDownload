using DownLoadMP3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class BookSeriesModel
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public List<BookSeriesItemModel> SeriesList { get; set; }
        public BookSeriesModel()
        {
            SeriesList = new List<BookSeriesItemModel>();
        }
        internal static BookSeriesModel Convert(DownLoadMP3.BookSeries item)
        {
            BookSeriesModel model = new BookSeriesModel();
            model.BookId = item.BookId;
            model.BookName = item.BookName;
            item.SeriesList.ForEach(a =>
            {
                var info = new BookSeriesItemModel();
                info.BookId = item.BookId;
                info.Name = a.Name;
                info.Url = a.Url;
                info.Progress = 0;
                info.ID = md5ID($"{info.BookId}_{info.Name}".ToLower());
                model.SeriesList.Add(info);
            });

            return model;
        }

        private static string md5ID(string inputValue)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                var strResult = BitConverter.ToString(result);
                string result3 = strResult.Replace("-", "");
                return result3;
            }

        }
    }
    public class BookSeriesItemModel
    {
        public string ID { get; set; }
        public string BookId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public float Progress { get; set; }

        internal BookSeriesItem ToBookSeriesItem()
        {
            BookSeriesItem model = new BookSeriesItem();
            model.BookId = this.BookId;
            model.Name = this.Name;
            model.Url = this.Url;
            return model;
        }
    }
}
