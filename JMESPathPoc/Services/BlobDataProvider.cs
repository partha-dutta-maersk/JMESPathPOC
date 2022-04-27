using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JsonQueryPOC.Contracts;
using JsonQueryPOC.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace JsonQueryPOC.Services
{
    public class BlobDataProvider : IBlobDataProvider
    {
        private const string _blobStorageConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=stgazewtmlns001finops;AccountKey=XwcERtkj/BkeUOorBBzEeIebHYaaR80eapo76iJlwlH5oE/dIccw9grLgszACTe9M5QGVLK1BMHjcu0bZcUEwg==;EndpointSuffix=core.windows.net";

        public static readonly Lazy<CloudBlobClient> CloudBlobClient = new(() =>
        {
            var storageAccount = CloudStorageAccount.Parse(_blobStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient;
        });

        public BlobDataProvider()
        {
        }

        public async Task<T> Get<T>(string blobUrl, BlobSerializationType? serializationType = BlobSerializationType.Json)
        {
            var blockBlob = await GetCloudBlockBlob(blobUrl);
            await using var stream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(stream);
            stream.Position = 0;
            var blobContent = Encoding.UTF8.GetString(stream.ToArray());
            return Deserialize<T>(blobContent, serializationType.Value);
        }

        public async Task<string> GetJsonStringIndentedOrDefault(string blobUrl)
        {
            var blob = await Get<string>(blobUrl, BlobSerializationType.None);
            return blob.TryParseJson(out var value) ? value.ToStringIndented() : blob;
        }

        public async Task Upload(string content, string blobUrl)
        {
            var blockBlob = await GetCloudBlockBlob(blobUrl);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            await blockBlob.UploadFromByteArrayAsync(contentBytes, 0, contentBytes.Length);
        }

        public async Task Delete(string blobUrl)
        {
            var blockBlob = await GetCloudBlockBlob(blobUrl);
            await blockBlob.DeleteIfExistsAsync();
        }

        private async Task<CloudBlockBlob> GetCloudBlockBlob(string blobUrl)
        {
            var (container, fileName) = SplitPathAndFileName();

            var cloudBlobContainer = CloudBlobClient.Value.GetContainerReference(container);
            await cloudBlobContainer.CreateIfNotExistsAsync();
            return cloudBlobContainer.GetBlockBlobReference(fileName);

            (string container, string fileName) SplitPathAndFileName()
            {
                var splittedPathParts = blobUrl.Split('/');
                var containerName = splittedPathParts.First();
                var file = splittedPathParts.Length > 1 ? string.Join("/", splittedPathParts.Skip(1)) : containerName;
                return (containerName, file);
            }
        }

        private static T Deserialize<T>(string content, BlobSerializationType serializationType)
        {
            switch (serializationType)
            {
                case BlobSerializationType.None:
                    return (T)Convert.ChangeType(content, typeof(T));

                case BlobSerializationType.Xml:
                    using (var reader = new StringReader(content))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(T));
                        return (T)xmlSerializer.Deserialize(reader);
                    }

                case BlobSerializationType.Json:
                    return JsonConvert.DeserializeObject<T>(content);

                default:
                    throw new StorageException($"Unknown serialization type: {serializationType}");
            }
        }

        public async Task Upload(byte[] contentBytes, string blobUrl, string contentType)
        {
            var blockBlob = await GetCloudBlockBlob(blobUrl);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromByteArrayAsync(contentBytes, 0, contentBytes.Length);
        }
    }
}
