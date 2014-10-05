using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using ZipperUpper.Models;

namespace ZipperUpper.Controllers
{
    //EnableCors enables Cross Origin REquests. origins: "*" allows all origins. 
    //Change to Microsoft.com or some variation to make this tool private for MSCOM
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ZipController : ApiController
    {
        /// <summary> 
        /// Generate and Return the zip file that corresponds to a Ticket Number which is created by passing a String[] to the POST request
        /// </summary> 
        /// <remarks> 
        /// POST happens first to create a consumable GET request ID.  Address is /api/get
        /// </remarks>
        public async Task<HttpResponseMessage> Get(string id)
        {
            //Attempt to download and zip up files
            try
            {
                //Use the Ticket Number passed in through the id as a Key in a custom Dictionary
                //to pull out the corresponding List<string>
                var urlList = CacheDictionary<String, List<String>>.Instance[id];
                //Helper exists to perform the brute operations like downloading files and zipping them up
                //Would be well served as a Singleton
                var helper = new Helper();
                //The actualy zipfile returned from the helper
                var zipFile = await helper.CreateZipFormUrls(urlList);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new MemoryStream();
                zipFile.Save(stream);
                response.Content = new ByteArrayContent(stream.ToArray());

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                response.Content.Headers.ContentLength = stream.Length;
                response.Content.Headers.ContentDisposition.FileName = "download.zip";

                return response;
            }
            //Oops. Open as a webpage with an error string
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message + "<br/><br/>" + ex.StackTrace)
                };
                return response;
            }
        }

        /// <summary> 
        /// Generate and Return the zip file that corresponds to a Ticket Number which is created by passing a String[] to the POST request
        /// </summary> 
        /// <remarks> 
        /// POST happens first to create a consumable GET request ID. Address is /api/post
        /// </remarks>
        public HttpResponseMessage Post([FromBody]string value)
        {
            //Try to deserialize the string into a List<string>
            try
            {
                var urlList = new JavaScriptSerializer().Deserialize<List<string>>(value);
                //Limit downloads to 30 files at a time
                if (urlList.Count >= 30)
                {
                    throw new Exception("Too many items");
                }
                var helper = new Helper();
                //Random number generated as a ticket to be consumed by the GET request
                var random = helper.GenerateNumber();
                //Add the List<string> to the persistent Dictionary with the ticket as the KEY and the List as the VALUE
                CacheDictionary<String, List<String>>.Instance.Add(random, urlList);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    //Return the Ticket number as a plain string for the AJAX request
                    Content = new StringContent(random)
                };
                response.Content.Headers.ContentLength = random.Length;

                return response; 
            }
            catch (Exception ex)
            {
                //Ooops
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    //Tell them why it failed. If it ws too many items, that will be the ex.Message
                    //If it's another error, we should handle that
                    Content = new StringContent(ex.Message)
                };
                response.Content.Headers.ContentLength = ex.Message.Length;

                return response;
            }
        }
    }
}
