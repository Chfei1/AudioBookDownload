using System;
using System.Collections.Generic;
using System.Text;

namespace DownLoadMP3
{
    public class BookSeries
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public List<BookSeriesItem> SeriesList { get; set; }
        public BookSeries()
        {
            SeriesList = new List<BookSeriesItem>();
        }
    }
}
