﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// Helper class for Amazon Simple Queue Service.
    /// </summary>
    public class SQSHelper : IDisposable
    {
        private bool _disposed;

        #region Constructors

        public SQSHelper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = new AmazonSQSClient(awsAccessKeyId, awsSecretAccessKey);
        }

        public SQSHelper(AwsClientDetails clientDetails)
        {
            Client = new AmazonSQSClient(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        ~SQSHelper()
        {
            Dispose(false);
        }

        #endregion

        protected AmazonSQSClient Client
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a SQS queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public string CreateQueue(string queueName)
        {
            var request = new CreateQueueRequest { QueueName = queueName };

            CreateQueueResponse response = Client.CreateQueue(request);

            return response.CreateQueueResult.QueueUrl;
        }

        /// <summary>
        /// Sets the permissions on the queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <param name="label"></param>
        /// <param name="actionNames"></param>
        /// <param name="awsAccountIds"></param>
        public void SetQueuePermissions(string queueUrl, string label, IEnumerable<string> actionNames, IEnumerable<string> awsAccountIds)
        {
            var request = new AddPermissionRequest
                              {
                                  ActionName = new List<string>(actionNames),
                                  QueueUrl = queueUrl,
                                  AWSAccountId = new List<string>(awsAccountIds),
                                  Label = label
                              };

            Client.AddPermission(request);
        }

        /// <summary>
        /// Deletes the SQS Queue
        /// </summary>
        /// <param name="queueUrl"></param>
        public void DeleteQueue(string queueUrl)
        {
            var request = new DeleteQueueRequest { QueueUrl = queueUrl };

            Client.DeleteQueue(request);
        }

        /// <summary>
        /// Lists the Queues
        /// </summary>
        /// <returns></returns>
        public string[] ListQueues()
        {
            var request = new ListQueuesRequest();

            ListQueuesResponse response = Client.ListQueues(request);

            return response.ListQueuesResult.QueueUrl.ToArray();
        }

        /// <summary>
        /// Sends a message to the SQS Queue
        /// </summary>
        /// <param name="messageBody"></param>
        /// <param name="queueUrl"></param>
        /// <returns></returns>
        public string SendMessage(string messageBody, string queueUrl)
        {
            var request = new SendMessageRequest { MessageBody = messageBody, QueueUrl = queueUrl };

            SendMessageResponse response = Client.SendMessage(request);

            return response.SendMessageResult.MessageId;
        }

        /// <summary>
        /// Receives a message from the SQS Queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <returns></returns>
        public Message ReceiveMessage(string queueUrl)
        {
            var request = new ReceiveMessageRequest { MaxNumberOfMessages = 1, QueueUrl = queueUrl };

            ReceiveMessageResponse response = Client.ReceiveMessage(request);

            if (response.IsSetReceiveMessageResult())
            {
                return response.ReceiveMessageResult.Message.FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Deletes a message from the queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <param name="receiptHandle"></param>
        public void DeleteMessage(string queueUrl, string receiptHandle)
        {
            var request = new DeleteMessageRequest { QueueUrl = queueUrl, ReceiptHandle = receiptHandle };

            Client.DeleteMessage(request);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        virtual protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (!disposing)
                {
                    try
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }

        #endregion
    }
}