using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace getv2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3Controller : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
            string output = string.Empty;
            


                IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USWest2);

                GetObjectRequest request = new GetObjectRequest();
                request.BucketName = "ch2-sms-startchat-authentication";
                request.Key = "Token.txt";
                var response = await client.GetObjectAsync(request);

                using (var reader = new StreamReader(response.ResponseStream))
                {
                    output = await reader.ReadToEndAsync();
                }
            return output;
        }
    }
}