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
            List<Record> postsToIndex = new List<Record>();
            int i = 1;
            foreach (var post in posts)
            {
                postsToIndex.Add(new Record(i, post.Fields[0].ToString()));
                i++;
            }
            var response = elasticClient.IndexMany(postsToIndex);
            MessageBox.Show(response.IsValid ? "Индекс создан" : response.ToString());
        }

        public static List<Record> SearchDocument(ElasticClient elasticClient, string indexName, string stringToSearch)
        {
            var searchResponse = elasticClient.Search<Record>(s => s
                .Size(20)
                .Index(indexName)
                .Query(q => q
                    .Term(t => t.Text, stringToSearch)
                    )
                );
            return searchResponse.Documents.ToList();
        }

        public static void DeleteDocument(ElasticClient elasticClient, string indexName, Record post)
        {
            var searchResponse = elasticClient.Delete<Record>(post.Id);
        }


    }

}


