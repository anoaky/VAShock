using System.Net.Http;
using System;
using System.Text;
using Newtonsoft.Json;

namespace VAShock
{
    public class PSStatusResponse
    {
        public bool paused { get; set; }
    }
    public class VoiceAttackPlugin
    {
        public static string VA_DisplayName()
        {
            return "VAShock - v0.1a";
        }

        public static string VA_DisplayInfo()
        {
            return "For accessing the PiShock API";
        }

        public static Guid VA_Id()
        {
            return new Guid("{d395b1cf-b26d-4915-b8cc-cb68b632c5b2}");
        }

        public static void VA_StopCommand()
        {
            return;
        }

        public static void VA_Init1(dynamic vaProxy)
        {
            vaProxy.SetText("name", "VAShock for Voice Attack v0.1a");

        }

        public static void VA_Exit1(dynamic vaProxy)
        {
            return;
        }

        public static void VA_Invoke1(dynamic vaProxy)
        {
            string username = vaProxy.GetText("username");
            string apikey = vaProxy.GetText("apikey");
            string code = vaProxy.GetText("code");
            string action = vaProxy.GetText("action");

            if (action.Equals("shock"))
            {
                var data = new
                {
                    Username = username,
                    Apikey = apikey,
                    Code = code,
                    Name = vaProxy.GetText("name"),
                    Op = 0,
                    Duration = vaProxy.GetInt("duration"),
                    Intensity = vaProxy.GetInt("intensity")
                };

                using (var client = new HttpClient())
                {
                    var opUrl = "https://do.pishock.com";
                    var apiUrl = "/api/apioperate/";
                    var jsonData = JsonConvert.SerializeObject(data);

                    client.BaseAddress = new Uri(opUrl);
                    var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var task = client.PostAsync(apiUrl, contentData);
                    task.Wait();
                }
            }
            else if (action.Equals("check"))
            {
                var data = new
                {
                    Username = username,
                    Apikey = apikey,
                    Code = code,
                };

                using (var client = new HttpClient())
                {
                    var opUrl = "https://do.pishock.com";
                    var apiUrl = "/api/GetShockerInfo";
                    var jsonData = JsonConvert.SerializeObject(data);

                    client.BaseAddress = new Uri(opUrl);
                    var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var task = client.PostAsync(apiUrl, contentData);
                    task.Wait();
                    var response = task.Result;

                    var readTask = response.Content.ReadAsStringAsync();
                    readTask.Wait();
                    var responseString = readTask.Result;
                    var responseData = JsonConvert.DeserializeObject<PSStatusResponse>(responseString);
                    vaProxy.SetBoolean("paused", responseData?.paused);
                }
            }
        }
    }
}
