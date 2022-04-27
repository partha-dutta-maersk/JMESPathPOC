using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonQueryPOC.Contracts
{
    public interface IBlobDataProvider
    {
        /// <summary>
        /// Downloads content from blob
        /// </summary>
        /// <typeparam name="T">Type to which content will be converted</typeparam>
        /// <param name="blobUrl">Blob content url</param>
        /// <param name="serializationType">Type of blob file content, used during deserialization</param>
        /// <returns></returns>
        Task<T> Get<T>(string blobUrl, BlobSerializationType? serializationType = BlobSerializationType.Json);

        /// <summary>
        /// Downloads content from blob and returns as json string with indented format
        /// </summary>
        /// <param name="blobUrl">Blob content url</param>
        /// <returns></returns>
        Task<string> GetJsonStringIndentedOrDefault(string blobUrl);

        /// <summary>
        /// Add or overwrite if exists content of blob
        /// </summary>
        /// <param name="content">Content which will be uploaded to blob</param>
        /// <param name="blobUrl">Url under which content will be uploaded in formart "Container/Folder/FileName" </param>
        /// <returns></returns>
        Task Upload(string content, string blobUrl);

        /// <summary>
        /// Deletes file from blob
        /// </summary>
        /// <param name="blobUrl"></param>
        /// <returns></returns>
        Task Delete(string blobUrl);

        /// <summary>
        /// Add or overwrite if exists content of blob with specific file type
        /// </summary>
        /// <param name="contentBytes"></param>
        /// <param name="blobUrl"></param>
        /// <param name="contentType">content mime type</param>
        /// <returns></returns>
        Task Upload(byte[] contentBytes, string blobUrl, string contentType);
    }
}
