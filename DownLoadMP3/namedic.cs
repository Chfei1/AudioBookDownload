using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsharpHttpHelper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DownLoadMP3
{
    static class namedic
    {
        static Dictionary<int, string> namelist = new Dictionary<int, string>();


        public static void loadingDirectory()
        {
            int pages = 1;
            int page = 1;
            int pageSize = 100;
            int totalCount = 1;
            
            do
            {
                string url = $"https://m.ximalaya.com/m-revision/common/album/queryAlbumTrackRecordsByPage?albumId=11346422&page={page}&pageSize={pageSize}&asc=true&countKeys=play%2Ccomment&v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

                string json = HttpClientGetHtmls(url);

                JToken token = JToken.Parse(json);

                JToken data = token["data"];

                page = data["page"].ToObject<int>();
                pageSize = data["pageSize"].ToObject<int>();
                totalCount = data["totalCount"].ToObject<int>();
                pages = (totalCount - 1) / pageSize + 1;

                JArray list = (JArray)data["trackDetailInfos"];

                foreach (JToken info in list)
                {
                    string name = info["trackInfo"]["title"].ToString();
                    string index = Regex.Match(name, @"\d+").Value;
                    namedic.namelist.Add(int.Parse(index), name);
                }

            }
            while (page <= pages);


            string ss = JsonConvert.SerializeObject(namelist);
           
        }



        public static string getname(int index)
        {
            //return namelist[index];
            return index.ToString();
        }


        public static string HttpClientGetHtmls(string url)
        {
            try
            {
                var client = new HttpClient();
                var response = client.GetAsync(new Uri(url)).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception exception)
            {
            }
            return "";
        }
    }
}
