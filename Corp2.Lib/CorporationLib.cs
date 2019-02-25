using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Corp2.Lib
{
    public class CorporationLib
    {
        public async Task<bool> AddCorporation(Corporation corp)
        {
            var client = new HttpClient();
            var url = ConfigurationManager.AppSettings["AzureFunction"];
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(corp);
            HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonString,Encoding.UTF8,"application/json"));
            if (response.IsSuccessStatusCode)
            {
                var results = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return true;
            }
            else return false;
            
        }
    }
}
