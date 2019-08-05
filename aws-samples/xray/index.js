var AWSXRay = require("aws-xray-sdk");
AWSXRay.config([AWSXRay.plugins.EC2Plugin]);
var express = require("express");
const req = require("request");
AWSXRay.capturePromise();

const http = require("http");
var bodyParser = require("body-parser");
var jsonParser = bodyParser.json();

var ddb = AWSXRay.captureAWSClient(new AWS.DynamoDB({region:'us-west-1'}));
var http = AWSXRay.captureHTTPs(require('http'));


const app = express();
app.use(AWSXRay.express.openSegment("myxray"));
app.use(jsonParser);


app.get("/tcodes", (req,res) =>{
return  getTCodes().then(c => res.send(c));
});


app.get("/", (req, res) => {
  return getTime().then(t => res.send("Received a GET HTTP method" + t));
});

app.post("/", (req, res) => {
  console.log(req.headers);
  console.log(req.body);
  return res.send("Received a POST HTTP method");
});

app.put("/", (req, res) => {
  return res.send("Received a PUT HTTP method");
});

app.delete("/", (req, res) => {
  return res.send("Received a DELETE HTTP method");
});

function getTime() {
  return new Promise((res, rej) => {
    req(
      {
        url: "http://localhost:8086/",
        method: "GET",
        headers: {
          Accept: "application/json",
          "content-Type": "application/json; charset=UTF-8"
        }
      },
      function(err, response, body) {
        console.log(body);
        if (err) rej(err);
        else res(body);
      }
    );
  });
}


function getTCodes(){
let params = {
    "TableName": "TransportCodes",
    "IndexName": "BrandId-index",
    "KeyConditionExpression": "BrandId = :brand",
    "ExpressionAttributeValues": {
        ":brand": {"S": "1000"}
    },
    "ProjectionExpression": "TransportCode"
};

return ddb.query(params).promise();

}


function getTimeWithHttp() {
  return new Promise((resovle, reject) => {
    https
      .get("http://localhost:8088/", resp => {
        let data = "";

        // A chunk of data has been recieved.
        resp.on("data", chunk => {
          data += chunk;
        });

        // The whole response has been received. Print out the result.
        resp.on("end", () => {
          console.log(JSON.parse(data).explanation);
          resolve(data);
        });
      })
      .on("error", err => {
        console.log("Error: " + err.message);
        reject(err);
      });
  });
}

app.listen("8087", () => console.log(`Example app listening on port 8086!`));
app.use(AWSXRay.express.closeSegment());
