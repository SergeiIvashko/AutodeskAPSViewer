﻿using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;

namespace MyAutodeskAPS.Models
{
    public partial class Aps
    {
        private async Task EnsureBucketExists(string bucketKey)
        {
            var token = await GetInternalToken();
            var api = new BucketsApi();
            api.Configuration.AccessToken = token.AccessToken;
            try
            {
                await api.GetBucketDetailsAsync(bucketKey);
            }
            catch (ApiException e)
            {
                if (e.ErrorCode == 404)
                {
                    await api.CreateBucketAsync(new PostBucketsPayload(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Persistent));
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<ObjectDetails> UploadModel(string objectName, Stream content)
        {
            await EnsureBucketExists(bucket);
            var token = await GetInternalToken();
            var api = new ObjectsApi();
            api.Configuration.AccessToken = token.AccessToken;
            var results = await api.uploadResources(bucket, new List<UploadItemDesc> {
            new UploadItemDesc(objectName, content)
        });
            if (results[0].Error)
            {
                throw new ArgumentException(results[0].completed.ToString());
            }
            else
            {
                var json = results[0].completed.ToJson();
                return json.ToObject<ObjectDetails>();
            }
        }

        public async Task<IEnumerable<ObjectDetails>> GetObjects()
        {
            const int PageSize = 64;
            await EnsureBucketExists(bucket);
            var token = await GetInternalToken();
            var api = new ObjectsApi();
            api.Configuration.AccessToken = token.AccessToken;
            var results = new List<ObjectDetails>();
            var response = (await api.GetObjectsAsync(bucket, PageSize)).ToObject<BucketObjects>();
            results.AddRange(response.Items);
            while (!string.IsNullOrEmpty(response.Next))
            {
                var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(new Uri(response.Next).Query);
                response = (await api.GetObjectsAsync(bucket, PageSize, null, queryParams["startAt"])).ToObject<BucketObjects>();
                results.AddRange(response.Items);
            }
            return results;
        }
    }

}