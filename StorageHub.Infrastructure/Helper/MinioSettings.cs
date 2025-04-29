namespace StorageHub.Infrastructure
{
    public class MinioSettings
    {
            public string Endpoint { get; set; } // MinIO server endpoint
            public string AccessKey { get; set; } // Access key for MinIO
            public string SecretKey { get; set; } // Secret key for MinIO
            public string BucketName { get; set; } // Name of the bucket to use
            public string UseSSL { get; set; } // Region of the bucket (optional)
        

    }
}
