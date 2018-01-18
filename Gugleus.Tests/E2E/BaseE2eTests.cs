using Gugleus.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gugleus.Tests.E2E
{
    public class BaseE2eTests
    {
        protected readonly HttpClient Client;
        protected readonly TestServer Server;

        protected BaseE2eTests()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        protected static StringContent PrepareContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        protected async Task<T> GetOneAsync<T>(string uri)
        {
            using (HttpResponseMessage response = await Client.GetAsync(uri))
            {
                string responseString = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<T>(responseString);
                return item;
            }
        }

        protected async Task<IEnumerable<T>> GetAllAsync<T>(string uri)
        {
            using (HttpResponseMessage response = await Client.GetAsync(uri))
            {
                string responseString = await response.Content.ReadAsStringAsync();

                IEnumerable<T> items = JsonConvert.DeserializeObject<IEnumerable<T>>(responseString);
                return items;
            }
        }
    }
}
