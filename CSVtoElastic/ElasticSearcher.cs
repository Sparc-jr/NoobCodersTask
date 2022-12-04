using Elasticsearch.Net;
using Microsoft.Extensions.Hosting;
using Nest;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace CSVtoElastic
{
    public static class ElasticsearchHelper
    {
        static string cloudID = "NoobCodersTask:dXMtY2VudHJhbDEuZ2NwLmNsb3VkLmVzLmlvOjQ0MyQ4NjQyZWI4MDg4YTg0ZDUxYjhiMDdiMGE4NDFkMTdiYyRjY2I1NTFlZDY3MWI0MTRhYTc4ZDUyZjIxYThlMGI5Nw==";
        public static ElasticClient GetESClient()
        {

            var credentials = new BasicAuthenticationCredentials("elastic", "ZZwhmkol45v3JtMEZusfiLpe");
            var connectionPool = new CloudConnectionPool(cloudID, credentials);
            var connectionSettings = new ConnectionSettings(connectionPool)
                .EnableApiVersioningHeader()
                .DefaultIndex("posts")
                .ThrowExceptions()
                .EnableDebugMode();
            var elasticClient = new ElasticClient(connectionSettings);
            return elasticClient;
        }

        public static void CreateDocument(ElasticClient elasticClient, string indexName, List<Post> posts)
        {
            elasticClient.DeleteIndex(indexName);
            List<string> postsToIndex = new List<string>();
            int i = 0;
            foreach (var post in posts)
            {
                postsToIndex.Add(post.Fields[1].ToString());
                i++;
            }
            var response = elasticClient.IndexMany(posts);
            MessageBox.Show(response.IsValid ? "Индекс создан" : response.ToString());
        }

        public static List<Post> SearchDocument(ElasticClient elasticClient, string indexName, string stringToSearch)
        {
            var searchResponse = elasticClient.Search<Post>(s => s
                .Size(20)
                .Index(indexName)
                .Query(q => q.
                    .Term(t => t
                    .d(f => f.Fields[1])
                    .Query(stringToSearch)
                    )
                )
            );


            /*var request = new SearchRequest("posts")
            {
                From = 0,
                Size = 20,
                Query = new TermQuery("user") { Value = stringToSearch }
            };

            var searchResponse = elasticClient.Search<Post>(request);*/



            return searchResponse.Documents.ToList();
        }

    }

}


