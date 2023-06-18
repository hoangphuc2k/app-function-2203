using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace appFunction2000
{
    public static class appFunction2000
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> RunProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            SqlConnection sqlConnection = getConnection();

            List<Products> products = new List<Products>();

            String query = "SELECT productId,productName,quantity FROM Products";

            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Products product = new Products()
                    {
                        productId = reader.GetInt32(0),
                        productName = reader.GetString(1),
                        quantity = reader.GetInt32(2),
                    };

                    products.Add(product);
                }
            }

            sqlConnection.Close();

            return new OkObjectResult(products);
        }

        [FunctionName("GetProduct")]
        public static async Task<IActionResult> RunProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            SqlConnection sqlConnection = getConnection();

            int productId = int.Parse(req.Query["id"]);

            List<Products> products = new List<Products>();

            String query = String.Format("SELECT productId,productName,quantity FROM Products WHERE productId={0}", productId);

            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    reader.Read();
                    Products product = new Products()
                    {
                        productId = reader.GetInt32(0),
                        productName = reader.GetString(1),
                        quantity = reader.GetInt32(2),
                    };

                    return new OkObjectResult(product);

                }
            }
            catch (Exception exception) {
                sqlConnection.Close();
                return new OkObjectResult("Record not found");
            }
        }

        private static SqlConnection getConnection()
        {
            return new SqlConnection("Server=tcp:appdb2000.database.windows.net,1433;Initial Catalog=appdb;Persist Security Info=False;User ID=sqladmin;Password=hoangphuc@123A;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        }
    }
}

