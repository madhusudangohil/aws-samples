fileName=$1
s3Bucket=$2
s3AccessKey=$3
s3SecretKey=$4

date=`date +%Y%m%d`
dateFormatted=`date -R`
relativePath="/${s3Bucket}/${fileName}"
contentType="application/octet-stream"
stringToSign="PUT\n\n${contentType}\n${dateFormatted}\n${relativePath}"
signature=`echo -en ${stringToSign} | openssl sha1 -hmac ${s3SecretKey} -binary | base64`

curl -v -X PUT -T "${fileName}" \
-H "Host: ${s3Bucket}.s3.amazonaws.com" \
-H "Date: ${dateFormatted}" \
-H "Content-Type: ${contentType}" \
-H "Authorization: AWS ${s3AccessKey}:${signature}" \
https://${s3Bucket}.s3.amazonaws.com/${fileName}


#usage
# ./uploadfile.sh samplefile.txt mysample-bucket XXXXXXXXXXXXXLD UEPUEOEUpjdpieeerkjljuuidfs
