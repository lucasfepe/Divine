using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.S3;
using System.Threading.Tasks;
using Amazon.S3.Model;
using System.Threading;
using System;
using System.IO;
using Amazon.Runtime;
using System.Net.Sockets;

public class S3 : MonoBehaviour
{
    public AmazonS3Client s3Client;
    public static S3 Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        s3Client = new AmazonS3Client(
            new BasicAWSCredentials(DynamoDB.ACCESS_KEY, DynamoDB.SECRET), RegionEndpoint.USEast1);
    }

    public async Task<bool> DownloadObjectFromBucketAsync(IAmazonS3 client,
            string bucketName,
            string objectName,
            string filePath)
    {
        if (File.Exists($"{filePath}\\{objectName}"))
        {

            return true;
        };
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objectName,
        };

        // Issue request and remember to dispose of the response
        using GetObjectResponse response = await client.GetObjectAsync(request);

        try
        {
            // Save object to local file
            await response.WriteResponseStreamToFileAsync($"{filePath}\\{objectName}", false, CancellationToken.None);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error saving {objectName}: {ex.Message}");
            return false;
        }
    }


}
