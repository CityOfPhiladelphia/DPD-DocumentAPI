import cx_Oracle
import PyPDF2
import requests
import os
from botocore.config import Config
import boto3
from datetime import datetime

ACCESS_KEY = os.environ.get('AWS_AccessKey')
SECRET_KEY = os.environ.get('AWS_SecretKey')

http_proxy  = os.environ.get('http_proxy')
https_proxy  = os.environ.get('https_proxy')

proxyDict = { 
              "http"  : http_proxy, 
              "https" : https_proxy, 
            }

s3 = boto3.client(
    's3',
    config=Config(
        proxies={
            'https': https_proxy,
            'http': http_proxy
        },
        connect_timeout=30,
        read_timeout=6000,
        retries={'max_attempts': 10}
    ),
    aws_access_key_id=ACCESS_KEY,
    aws_secret_access_key=SECRET_KEY
)
axBucket = os.environ.get('DocAPI_Bucket')

ApiEndpoint = os.environ.get('DocAPI_PublicEndpoint')
DocumentApiEntitiesResponse = requests.get(ApiEndpoint, proxies = proxyDict)
DocumentApiEntities = DocumentApiEntitiesResponse.json()[0]
CategoryIds = [category['id'] for category in DocumentApiEntities['categories']]

for categoryId in CategoryIds:
    restService = os.environ.get('DocAPI_RestEndpoint')
    credentials = os.environ.get('DocAPI_RestCredentials')
    pdf_dir = 'E:\\document-api-s3-staging\\'
    log_dir = 'E:\\document-api-s3-staging\\LOGS\\'
    v_app_id = str(categoryId)

    app_query = os.environ.get('DocAPI_AppQuery') + v_app_id
    doc_query = os.environ.get('DocAPI_DocQuery') + v_app_id

    imageDB = cx_Oracle.connect(
        os.environ.get('DocAPI_CredentialUser'),
        os.environ.get('DocAPI_CredentialPassword'),
        os.environ.get('DocAPI_CredentialSchema')
    )

    cur = imageDB.cursor()
    cur.execute(app_query)
    app = cur.fetchone()[0]
    restService = restService + app
    cur.execute(doc_query)
    docs = cur.fetchall()
    cur.close()
    imageDB.close()

    pdf_dir = pdf_dir + app
    try:
        os.mkdir(pdf_dir)
        print('Created New Directory:  ' + pdf_dir)
    except:
        print('Directory Already Exists:  ' + pdf_dir)

    for doc in docs:
        pdf_name = v_app_id + '-' + str(doc[0]) + '.pdf'
        pdf_file = pdf_dir + '\\' + pdf_name
        try:
            f = open(pdf_file, 'rb')
            read_pdf = PyPDF2.PdfFileReader(f)
            f.close()
            print(pdf_file + ' Already Exists')
        except:
            URL = restService + '/' + str(doc[0]) + credentials
            webcontent = requests.get(URL, proxies = proxyDict)
            webPDF = webcontent.content
            with open(pdf_file, 'wb') as f:
                f.write(webPDF)
                f.close()
            try:
                f = open(pdf_file, 'rb')
                read_pdf = PyPDF2.PdfFileReader(f)
                f.close()
                print(pdf_file + ' ********** DOWNLOAD SUCCESSFUL **********')
                l = open(log_dir + app + '.log', 'a+')
                l.write(str(doc[0]) + ' ' + pdf_file + '\n')
                l.close()
                
                try:
                    s3.upload_file(pdf_file, axBucket, pdf_name)
                    date_time  = datetime.now().strftime("%m/%d/%Y, %H:%M:%S")
                    print(pdf_file + ' ********** AWS UPLOAD SUCCESSFUL: @' + date_time)
                    l = open(log_dir + app + '.log', 'a+')
                    l.write(str(doc[0]) + ' ' + pdf_file + ": to AWS @" + date_time + '\n')
                    l.close()

                    os.remove(pdf_file)

                except Exception as e:
                    print(pdf_file + ' ************ AWS UPLOAD FAILED ************')
                    message = f"\n Could Not Upload Document To AWS S3 \n {str(e)} \n"
                    f.close()
                    b = open(log_dir + app + '_ERROR.log', 'a+')
                    b.write(str(doc[0]) + message + '\n')
                    b.close()


            except:
                print(pdf_file + ' ************ FAILED ************')
                f.close()
                os.remove(pdf_file)
                b = open(log_dir + app + '_ERROR.log', 'a+')
                b.write("Error Downloading - " + str(doc[0]) + '\n')
                b.close()
