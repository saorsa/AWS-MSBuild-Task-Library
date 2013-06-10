using Amazon.S3;

namespace Snowcode.S3BuildPublisher.Client
{
    /// <summary>
    /// Holds Client connection details for AWS
    /// </summary>
    public class AwsClientDetails
    {
        
        public string AwsAccessKeyId { get; set; }

        public string AwsSecretAccessKey { get; set; }

        // It is not required
        public AmazonS3Config AmazonS3Config { get; set; } 

    }
}
