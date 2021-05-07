from modules import boto3

s3 = boto3.client('s3')

print('Original object from the S3 bucket:')
original = s3.get_object(
  Bucket='tutorial-bluegreen-bucket-mg-nv',
  Key='sample.txt')
print(original['Body'].read().decode('utf-8'))

print('Object processed by S3 Object Lambda:')
transformed = s3.get_object(
  Bucket='arn:aws:s3-object-lambda:us-east-1:699589633627:accesspoint/myfirstobjectlambdaap',
  Key='sample.txt')
print(transformed['Body'].read().decode('utf-8'))