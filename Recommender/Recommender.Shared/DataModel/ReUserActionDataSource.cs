﻿using Recommender.Common;
using Recommender.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Enumeration.Pnp;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Recommender.DataModel
{
    public sealed class ReUserActionDataSource
    {
        private static ReUserActionDataSource _reUserActionDataSource = new ReUserActionDataSource();

        public static async Task SetUserAction(string mediaId)
        {
            await _reUserActionDataSource.PostWebServiceDataAsync(mediaId);
        }

        private async Task PostWebServiceDataAsync(string mediaId)
        {
            try
            {
                Random random = new Random();
                int r = random.Next();
                string requestUrl = "http://replatform.cloudapp.net:8000/userAction?uuid={0}&type={1}&media_id={2}&from={3}&to={4}&r={5}";
                string userid = App.gDeviceName;
                requestUrl = String.Format(requestUrl, userid, App.gDeviceType, mediaId, App.NavigationRoadmap.GetFrom(), App.NavigationRoadmap.GetTo(), r);

                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                filter.AutomaticDecompression = true;
                HttpClient httpClient = new HttpClient(filter);
                CancellationTokenSource cts = new CancellationTokenSource();

                filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;
                filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default;

                Uri resourceUri;
                if (!Uri.TryCreate(requestUrl.Trim(), UriKind.Absolute, out resourceUri))
                {
                    return;
                }

                HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
                string jsonText = await Helpers.GetResultAsync(response, cts.Token);

                GetPostUserActionResult(jsonText);

                if (filter != null)
                {
                    filter.Dispose();
                    filter = null;
                }

                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }

                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }
            }
            catch (Exception) { }
        }

        private void GetPostUserActionResult(string jsonData) { }
    }
}