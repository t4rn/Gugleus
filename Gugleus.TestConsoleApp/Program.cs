using RestSharp;
using System;
using System.IO;

namespace Gugleus.TestConsoleApp
{
    class Program
    {
        private static readonly string _apiHash = "abc";

        static void Main(string[] args)
        {
            string url = "http://localhost:65508";
            string filePath = @"C:\img6mb.jpg";
            byte[] fileIntByteArray = File.ReadAllBytes(filePath);
            string fileInBase64String = Convert.ToBase64String(fileIntByteArray);

            FileInfo fileInfo = new FileInfo(filePath);
            Console.WriteLine($"File size: {fileInfo.Length / 1024} kB");

            int fileByteCount = System.Text.ASCIIEncoding.ASCII.GetByteCount(fileInBase64String);
            string base64FileSize = $"{fileByteCount / 1024} kB";

            Console.WriteLine($"Base64String file size: {base64FileSize} - for string.Length = {fileInBase64String.Length}");

            HitApi(url, fileInBase64String);

            Console.WriteLine("\n\nEnd...");
            Console.Read();
        }

        private static void HitApi(string url, string fileInBase64String)
        {
            var client = new RestClient(url);

            Ping(client);
            AddPost(client, fileInBase64String);
        }

        private static void Ping(RestClient client)
        {
            var ping = new RestRequest("posts", Method.GET);
            ping.AddHeader("Hash", _apiHash);

            var pingResponse = client.Execute(ping);
            ShowResponse(pingResponse, nameof(Ping));
        }

        private static void AddPost(RestClient client, string fileInBase64String)
        {
            var request = new RestRequest("posts", Method.POST);

            request.AddHeader("Hash", _apiHash);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new
            {
                content = "tresc posta",
                user = new
                {
                    username = "john",
                    password = "pass",
                    additionalEmail = "jp@yahoo.com",
                    phone = "600100200"
                },
                image = new
                {
                    content = fileInBase64String,
                    format = "jpg"
                }
            });

            var response = client.Execute(request);
            ShowResponse(response, nameof(AddPost));
        }

        private static void ShowResponse(IRestResponse response, string method)
        {
            var responseObj = new
            {
                status = response.StatusCode,
                content = response.Content
            };

            Console.WriteLine($"\n{method.ToUpper()} response:\n {responseObj}");
        }
    }
}
