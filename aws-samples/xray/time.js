var AWSXRay = require("aws-xray-sdk");
AWSXRay.config([AWSXRay.plugins.EC2Plugin]);
var express = require("express");

const app = express();
app.use(AWSXRay.express.openSegment("timejs"));
app.get("/", (req, res) => {
  return res.send(Date.now().toString());
});

app.post("/", (req, res) => {
  return res.send("Received a POST HTTP method");
});

app.put("/", (req, res) => {
  return res.send("Received a PUT HTTP method");
});

app.delete("/", (req, res) => {
  return res.send("Received a DELETE HTTP method");
});

app.use(AWSXRay.express.closeSegment());

app.listen("8086", () => console.log(`Example app listening on port 8086!`));
