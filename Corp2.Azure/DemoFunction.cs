using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
//using System.Net;
//using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using System;

namespace Corp2.Azure
{
    public static class DemoFunction
    {
        [FunctionName("DemoFunction")]
        //    public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        //    {
        //        log.Info("C# HTTP trigger function processed a request.");

        //        // parse query parameter
        //        string name = req.GetQueryNameValuePairs()
        //            .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        //            .Value;

        //        if (name == null)
        //        {
        //            // Get request body
        //            dynamic data = await req.Content.ReadAsAsync<object>();
        //            name = data?.name;
        //        }

        //        return name == null
        //            ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        //            : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        //    }
        //}


        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

            var successful = true;
            string errorDesc = "";
            try
            {
                var cnnString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
                using (var connection = new SqlConnection(cnnString))
                {
                    connection.Open();
                    dynamic data = await req.Content.ReadAsAsync<object>();
                    string CorporationId = data?.CorporationId;
                    string Name = data?.Name;

                    var rLog = await req.Content.ReadAsAsync<LogRequest>();

                    var command = new SqlCommand($"INSERT INTO [dbo].[Corporations] (CorporationId, Name, Address, Phone) VALUES (@Id,@Name,null,null)", connection);
                    command.Parameters.AddWithValue("@Id", CorporationId);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.ExecuteNonQuery();
                    log.Info("Log added to database successfully!");
                }
            }
            catch (Exception ex)
            {
                successful = false;
                errorDesc = ex.Message;
            }

            return !successful
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Unable to process your request" + (errorDesc != "" ? $": {errorDesc}" : string.Empty))
                : req.CreateResponse(HttpStatusCode.OK, "Data saved successfully!");
        }

        public class LogRequest
        {
            public int Id { get; set; }
            public string Log { get; set; }
        }
    }
}
