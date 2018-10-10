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
    public class ValuesController : ControllerBase
    {        
        public ValuesController()
        {            
        }
        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
            string output = string.Empty;
            var sharedFile = new SharedCredentialsFile();
            CredentialProfile devProfile;
            if (sharedFile.TryGetProfile("build", out devProfile))
            {
                BasicAWSCredentials credentials =
                    new BasicAWSCredentials(devProfile.Options.AccessKey, devProfile.Options.SecretKey);
                var assumerole = new AssumeRoleAWSCredentials(credentials, devProfile.Options.RoleArn, "channelsrole");



                IAmazonS3 client = new AmazonS3Client(assumerole, RegionEndpoint.USWest2);

            GetObjectRequest request = new GetObjectRequest();
                request.BucketName = "ch2-sms-startchat-authentication";
                request.Key = "Token.txt";
                var response = await client.GetObjectAsync(request);
                
                using (var reader = new StreamReader(response.ResponseStream))
                {
                    output = await reader.ReadToEndAsync();
                }
            }
            return output;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
