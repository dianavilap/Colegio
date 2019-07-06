using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ColegioArceProject.Classes
{
    public class CookieTempDataProvider : ITempDataProvider
    {
        public void SaveTempData(ControllerContext controllerContext, IDictionary<String, Object> values)
        {
            //THIS WAS THE LAST CHANGE I DID TO THE CODE AFTER SAW IT WORKING (COOKIE WAS NOT BEIGN REMOVED HOWEVER)
            if (values.Count == 0)
            {
                // if we have no data then issue an expired cookie to clear the cookie
                IssueCookie(controllerContext, null);
            }

            // convert the temp data dictionary into json
            var value = Serialize(values);

            // compress the json (it really helps)
            //var Bytes = Compress(value);

            // issue the cookie
            IssueCookie(controllerContext, EncryptionService.Encrypt(value));
        }
        public IDictionary<String, Object> LoadTempData(ControllerContext controllerContext)
        {
            // get the cookie
            var rawValue = GetCookieValue(controllerContext);

            if (rawValue == null)
            {
                return null;
            }

            // verify and decrypt the value via the asp.net machine key
            var value = EncryptionService.Decrypt(rawValue);

            // decompress to json
            //value = Decompress(Bytes);

            // convert the json back to a dictionary
            return JsonConvert.DeserializeObject<Dictionary<String, Object>>(value);
        }

        String GetCookieValue(ControllerContext controllerContext)
        {
            var cookie = controllerContext.HttpContext.Request.Cookies[Configuration.TempDataCookie];

            if (cookie != null)
            {
                return cookie.Value;
            }

            return null;
        }

        void IssueCookie(ControllerContext controllerContext, String value)
        {
            var cookie = new HttpCookie(Configuration.TempDataCookie, value)
            {
                HttpOnly = true,
                Secure = controllerContext.HttpContext.Request.IsSecureConnection,
                Expires = DateTime.Now.AddMinutes(Configuration.CookieLifeTime),
            };

            // if we have no data then issue an expired cookie to clear the cookie
            if (value == null)
            {
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }

            //if we have new data OR the request has a cookie, send (or clear) the cookie.
            if (value != null || controllerContext.HttpContext.Request.Cookies[Configuration.TempDataCookie] != null)
            {
                controllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }

        private Byte[] Compress(String value)
        {
            if (value == null) return null;

            var data = Encoding.UTF8.GetBytes(value);

            using (var input = new MemoryStream(data))
            {
                using (var output = new MemoryStream())
                {
                    using (var stream = new DeflateStream(output, CompressionMode.Compress))
                    {
                        input.CopyTo(stream);
                    }

                    return output.ToArray();
                }
            }
        }

        private String Decompress(Byte[] data)
        {
            if (data == null || data.Length == 0) return null;

            using (var input = new MemoryStream(data))
            {
                using (var output = new MemoryStream())
                {
                    using (var stream = new DeflateStream(input, CompressionMode.Decompress))
                    {
                        stream.CopyTo(output);
                    }

                    var result = output.ToArray();
                    return Encoding.UTF8.GetString(result);
                }
            }
        }

        private String Serialize(IDictionary<String, Object> data)
        {
            if (data == null || data.Keys.Count == 0) return null;

            //JavaScriptSerializer ser = new JavaScriptSerializer();
            //return ser.Serialize(data);

            return JsonConvert.SerializeObject(data);
        }

        private IDictionary<String, Object> Deserialize(String data)
        {
            if (String.IsNullOrWhiteSpace(data)) return null;

            //JavaScriptSerializer ser = new JavaScriptSerializer();
            //return ser.Deserialize<IDictionary<String, Object>>(data);

            return JsonConvert.DeserializeObject<IDictionary<String, Object>>(data);
        }
    }
}