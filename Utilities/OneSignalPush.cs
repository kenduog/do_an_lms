using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class OneSignalPush
    {
        private static IConfiguration configuration;
        static OneSignalPush()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
        public static void OneSignalWebPushNotifications(string headings, string content, List<string> OneSignal_PlayerId, string urlOfOneSignal)
        {
            try
            {
                if (OneSignal_PlayerId.Any())
                {
                    string onesignalAppId = configuration.GetSection("MySettings:OnesignalAppId").Value.ToString();//cái này sửa lại
                    string onesignalRestId = configuration.GetSection("MySettings:OnesignalRestId").Value.ToString();//cái này sửa lại

                    var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
                    request.KeepAlive = true;
                    request.Method = "POST";
                    request.ContentType = "application/json; charset=utf-8";
                    request.Headers.Add("authorization", "Basic " + onesignalRestId);

                    var obj = new
                    {
                        app_id = onesignalAppId,
                        headings = new { en = headings },
                        contents = new { en = content },
                        channel_for_external_user_ids = "push",
                        include_player_ids = OneSignal_PlayerId, //Gửi cho user đc chỉ định
                        url = urlOfOneSignal,
                        //included_segments = new string[] { "Subscribed Users" } //Gửi cho tất cả user nào đăng ký
                    };
                    var param = JsonConvert.SerializeObject(obj);
                    byte[] byteArray = Encoding.UTF8.GetBytes(param);
                    string responseContent = null;
                    try
                    {
                        using (var writer = request.GetRequestStream())
                        {
                            writer.Write(byteArray, 0, byteArray.Length);
                        }
                        using (var response = request.GetResponse() as HttpWebResponse)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                    }
                    System.Diagnostics.Debug.WriteLine(responseContent);
                }
            }
            catch { }
        }
    }
}
