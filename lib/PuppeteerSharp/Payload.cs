﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Payload information.
    /// </summary>
    public class Payload
    {
        private static readonly ISet<string> IgnoredHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "accept",
            "referer",
            "x-devtools-emulate-network-conditions-client-id",
            "cookie",
            "origin",
            "content-type",
            "intervention"
        };

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        /// <value>HTTP method.</value>
        [JsonConverter(typeof(HttpMethodConverter))]
        public HttpMethod Method { get; set; }
        /// <summary>
        /// Gets or sets the post data.
        /// </summary>
        /// <value>The post data.</value>
        public string PostData { get; set; }
        /// <summary>
        /// Gets or sets the HTTP headers.
        /// </summary>
        /// <value>HTTP headers.</value>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        internal string Hash
        {
            get
            {
                var normalizedUrl = Url;

                try
                {
                    // Decoding is necessary to normalize URLs.
                    // The method will throw if the URL is malformed. In this case,
                    // consider URL to be normalized as-is.
                    normalizedUrl = HttpUtility.UrlDecode(Url);
                }
                catch
                {
                }

                var hash = new Payload
                {
                    Url = Url,
                    Method = Method,
                    PostData = PostData
                };

                if (!normalizedUrl.StartsWith("data:", StringComparison.Ordinal))
                {
                    foreach (var item in Headers.OrderBy(kv => kv.Key))
                    {
                        if (IgnoredHeaders.Contains(item.Key))
                        {
                            continue;
                        }
                        hash.Headers[item.Key] = item.Value;
                    }
                }
                return JsonConvert.SerializeObject(hash, JsonHelper.DefaultJsonSerializerSettings);
            }
        }
    }
}