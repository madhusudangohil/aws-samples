using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Amazon.SimpleNotificationService;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using Amazon.SimpleNotificationService.Util;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;

namespace snsEnpoint.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post()
        {
            try
            {
                var sharedFile = new SharedCredentialsFile();
                CredentialProfile devProfile;
                if (sharedFile.TryGetProfile("dev", out devProfile))
                {
                    var assumeRoleResult = new AssumeRoleAWSCredentials(new SessionAWSCredentials(devProfile.Options.AccessKey,
                                devProfile.Options.SecretKey,
                                devProfile.Options.Token),
                                devProfile.Options.RoleArn,
                                "channelsrole");


                    String messagetype = Request.Headers["x-amz-sns-message-type"];
                   
                    //If message doesn't have the message type header, don't process it.
                    if (messagetype == null)
                    {
                        return StatusCode(400);
                    }
                    var message = string.Empty;
                    using (var reader = new StreamReader(Request.Body))
                    {
                        message = reader.ReadToEnd();
                        var sm = Amazon.SimpleNotificationService.Util.Message.ParseMessage(message);

                         // Check the signature and throw an exception if the signature verification fails.
                            if (isMessageSignatureValid(sm.SigningCertURL, sm.Signature, message).Result)
                            {
                                Debug.WriteLine("Signature verification succeeded");
                                if (sm.IsSubscriptionType)
                                {
                                    var model = new Amazon.SimpleNotificationService.Model.ConfirmSubscriptionRequest(sm.TopicArn, sm.Token);
                                    AmazonSimpleNotificationServiceClient c = new AmazonSimpleNotificationServiceClient(assumeRoleResult, RegionEndpoint.USWest2);
                                    c.ConfirmSubscriptionAsync(model).Wait();
                                    return Ok(); // CONFIRM THE SUBSCRIPTION
                                }
                                if (sm.IsNotificationType) // PROCESS NOTIFICATIONS
                                {
                                    dynamic json = JObject.Parse(sm.MessageText);
                                    //extract value: var s3OrigUrlSnippet = json.input.key.Value as string;
                                }
                            }
                            else
                            {
                                Console.WriteLine(">>Signature verification failed");
                                return StatusCode(400);
                            }                        

                    }

                    //do stuff
                    return Ok(new { });
                }
                return Ok(new { });
            }
            catch (Exception ex)
            {
                //LogIt.E(ex);
                return StatusCode(500, ex);
            }
        }


        private static async Task<Boolean> isMessageSignatureValid(string sigUrl, string signature, string body)
        {
            
            Uri certUrl = new Uri(sigUrl);                
            if (!((certUrl.Port == 443 || certUrl.IsDefaultPort) && certUrl.Scheme == "https"))
            {
                return false;
            }

            using (var web = new System.Net.Http.HttpClient())
            {
                var certificate = await web.GetByteArrayAsync(certUrl);
                var cert = new X509Certificate2(certificate);
                Debug.WriteLine($"Cert is. {cert.FriendlyName}");
                Debug.WriteLine("Checking certs expire date");
                Debug.WriteLine($"Cert is valid from {cert.NotBefore} to {cert.NotAfter}");


                var sha1 = System.Security.Cryptography.SHA1.Create();

                UnicodeEncoding encoding = new UnicodeEncoding();

                var data = sha1.ComputeHash(encoding.GetBytes(body));

                using (RSA rsa = cert.GetRSAPublicKey())

                {

                    Debug.WriteLine($"RSA key size is {rsa.KeySize}");

                    Debug.WriteLine($"Body data byte array size is {data.Count()}");


                    if (rsa == null || rsa.VerifyHash(data, Encoding.ASCII.GetBytes(signature), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1))

                        return false;

                }
            }
            return true;    
            
            
        }


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
