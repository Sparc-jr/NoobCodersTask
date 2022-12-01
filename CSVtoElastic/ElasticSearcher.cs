﻿using Elasticsearch.Net;
using Nest;
using System;

namespace CSVtoElastic
{
    public static class ElasticsearchHelper
    {
        static string cloudID = "NoobCodersTask:dXMtY2VudHJhbDEuZ2NwLmNsb3VkLmVzLmlvOjQ0MyQ4NjQyZWI4MDg4YTg0ZDUxYjhiMDdiMGE4NDFkMTdiYyRjY2I1NTFlZDY3MWI0MTRhYTc4ZDUyZjIxYThlMGI5Nw==";
        public static ElasticClient GetESClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            CloudConnectionPool connectionPool;

            //Provide your ES cluster addresses
            //var nodes = new Uri[] { new Uri("http://478a6a642d54.ngrok.io/") };
            connectionPool = new CloudConnectionPool(cloudID, new ApiKeyAuthenticationCredentials("6ySZlK9YwKAscofZcP7JYqdU"));
            connectionSettings = new ConnectionSettings(connectionPool);
            elasticClient = new ElasticClient(connectionSettings);
            return elasticClient;
        }

        public static void CreateDocument(ElasticClient elasticClient, string indexName, List<Posts> posts)
        {

            var response = elasticClient.IndexMany(posts);
        }

        public static void SearchDocument(ElasticClient elasticClient, string indexName, List<Posts> posts)
        {

            var searchResponse = elasticClient.Search<object>(s => s
                .AllIndices()
                .MatchAll(m => m));
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
      

