using Elasticsearch.Net;
using Nest;
using System;
using System.Windows.Forms;

namespace CSVtoElastic
{
    public static class ElasticsearchHelper
    {
        static string cloudID = "NoobCodersTask:dXMtY2VudHJhbDEuZ2NwLmNsb3VkLmVzLmlvOjQ0MyQ4NjQyZWI4MDg4YTg0ZDUxYjhiMDdiMGE4NDFkMTdiYyRjY2I1NTFlZDY3MWI0MTRhYTc4ZDUyZjIxYThlMGI5Nw==";
        public static ElasticClient GetESClient()
        {

            //var connectionPool = new CloudConnectionPool(cloudID, new BasicAuthenticationCredentials("sparcjr@google.com", "_1Q2w3E4r5T_"));//ApiKeyAuthenticationCredentials("6ySZlK9YwKAscofZcP7JYqdU"));
            //var connectionSettings = new ConnectionSettings(connectionPool);


            var credentials = new BasicAuthenticationCredentials("elastic", "ZZwhmkol45v3JtMEZusfiLpe");
            var connectionPool = new CloudConnectionPool(cloudID, credentials);
            var connectionSettings = new ConnectionSettings(connectionPool)
                .EnableApiVersioningHeader()
                .DefaultIndex("posts")
                .ThrowExceptions()
                .EnableDebugMode();

            var elasticClient = new ElasticClient(connectionSettings);


            elasticClient.Ping();



            return elasticClient;
        }

        public static void CreateDocument(ElasticClient elasticClient, string indexName, List<Posts> posts)
        {
            elasticClient.DeleteIndex(indexName);
            List<string> postsToIndex = new List<string>();
            int i = 0;
            foreach (var post in posts)
            {
                postsToIndex.Add(post.Text);
                i++;
            }
            var response = elasticClient.IndexMany(posts);
            MessageBox.Show(response.IsValid?"Индекс создан":response.ToString());
        }

        public static List<Posts> SearchDocument(ElasticClient elasticClient, string indexName, string stringToSearch)
        {
            var searchResponse = elasticClient.Search<Posts>(s => s
                .Size(20)
                .Index(indexName)
                .Query(q => q
                    .Match(m => m
                    .Field(f => f.Text)
                    .Query(stringToSearch)
                    )
                )
            );
            return searchResponse.Documents.ToList();
        }



        /*public static void DeleteDocument(ElasticClient elasticClient, string indexName, string documentId)
        {
            var response = elasticClient.Delete<Product>(documentId, d => d
            .Index(indexName));
        }*/

        //MatchQuery
        /* public static void GetProductByCategory(ElasticClient elasticClient, string indexName, string category)
         {
             var response = elasticClient.Search<Product>(s => s
             .Index(indexName)
             .Size(50)
              .Query(q => q
             .Match(m => m
             .Field(f => f.category)
             .Query(category)
             )
              )
             );
             Console.WriteLine("Product Category matches to {0}", category);
             foreach (var hit in response.Hits)
             {
                 Console.WriteLine("Id:{0} Name:{1} Description:{2} Category:{3} Price:{4}", hit.Source.id, hit.Source.name, hit.Source.description, hit.Source.category, hit.Source.price);
             }

         }*/

        //RangeQuery
        /*public static void GetProductByPriceRange(ElasticClient elasticClient, string indexName, int priceLowerLimit, int priceHigherLimit)
        {
            var response = elasticClient.Search<Product>(s => s
            .Index(indexName)
            .Size(50)
            .Query(q => q
             .Range(m => m
            .Field(f => f.price)
            .GreaterThanOrEquals(priceLowerLimit)
            .LessThan(priceHigherLimit)
            )
            )
            );
            Console.WriteLine("Product Price range between {0}-{1}", priceLowerLimit, priceHigherLimit);
            foreach (var hit in response.Hits)
            {
                Console.WriteLine("Id:{0} Name:{1} Description:{2} Category:{3} Price:{4}", hit.Source.id, hit.Source.name, hit.Source.description, hit.Source.category, hit.Source.price);
            }

        }*/

        //BoolQuery
        /*public static void GetProductByCategoryPriceRange(ElasticClient elasticClient, string indexName, string category, int priceLowerLimit, int priceHigherLimit)
         {
             var response = elasticClient.Search<Product>(s => s
              .Index(indexName)
             .Size(50)
             .Query(q => q
             .Bool(b => b
             .Must(m =>
             m.Match(mt1 => mt1.Field(f1 => f1.category).Query(category)) &&
             m.Range(ran => ran.Field(f1 => f1.price).GreaterThanOrEquals(priceLowerLimit).LessThan(priceHigherLimit))
             )
              )
             )
             );

             Console.WriteLine("Product Category:{0} Price range between {1}-{2}", category, priceLowerLimit, priceHigherLimit);
             foreach (var hit in response.Hits)
             {
                 Console.WriteLine("Id:{0} Name:{1} Description:{2} Category:{3} Price:{4}", hit.Source.id, hit.Source.name, hit.Source.description, hit.Source.category, hit.Source.price);
             }

         }*/

    }

}
      

