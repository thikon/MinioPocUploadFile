using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minio.Exceptions;
using Minio.DataModel;

namespace MinioPocUploadFile
{
    class Program
    {
        static void Main(string[] args)
        {
            //var endpoint = "play.min.io";
            //var accessKey = "Q3AM3UQ867SPQQA43P2F";
            //var secretKey = "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG";
            var endpoint = "127.0.0.1:9000";
            var accessKey = "AKIAIOSFODNN7EXAMPLE";
            var secretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
            try
            {
                var minio = new MinioClient(endpoint, accessKey, secretKey); //.WithSSL();
                // FileUpload.Run(minio).Wait();
                Program.Run(minio).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        // File uploader task.
        private async static Task Run(MinioClient minio)
        {
            var bucketName = "mytest";
            var objectName = "sample.txt";
            var filePath = @"C:\Users\Dew\Documents\My Work\codde\poc\doc-scan\source\sample.txt";
            var contentType = "text/plain";

            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await minio.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await minio.MakeBucketAsync(bucketName);
                }
                // Upload a file to bucket.
                await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);
                Console.WriteLine("Successfully uploaded " + objectName);

                var tmp = await minio.ListBucketsAsync();

                tmp.Buckets.ForEach(m => {
                    Console.WriteLine($"Current bucket: {m.Name} / {m.CreationDate.ToString() }");
                });

                // Check whether 'my-bucketname' exists or not.
                bool existsBucket = await minio.BucketExistsAsync(bucketName);
                Console.WriteLine("bucket-name " + ((found == true) ? "exists" : "does not exist"));

                // Check whether 'mybucket' exists or not.
                bool isItem = await minio.BucketExistsAsync(bucketName);
                if (isItem)
                {
                    // List objects from 'my-bucketname'
                    IObservable<Item> observable = minio.ListObjectsAsync(bucketName, null, true);
                    IDisposable subscription = observable.Subscribe(
                            item => Console.WriteLine("OnNext: {0}", item.Key),
                            ex => Console.WriteLine("OnError: {0}", ex.Message),
                            () => Console.WriteLine("OnComplete: {0}"));
                }
                else
                {
                    Console.WriteLine("mybucket does not exist");
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
        }
    }
}
