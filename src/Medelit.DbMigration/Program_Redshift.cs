using System;
using System.Data;
using System.Data.Odbc;

namespace DbMigrations
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");
      var dataset = ExecuteQuery();
      foreach (DataRow item in dataset.Tables[0].Rows)
      {
        Console.WriteLine(item["id"]);
      }


      Console.ReadLine();
    }


    public static DataSet ExecuteQuery(string query = "select * from dbo.city order by 1 desc limit 10")
    {
      var dataSet = new DataSet();
      string redshiftConnectionString = "Driver={Amazon Redshift (x64)}; Server=redshift-cluster-1.cmf7racjrbxc.us-east-1.redshift.amazonaws.com; Database=dev; UID=awsuser; PWD=Secure_2019; Port=5439;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=true";

      using (var connection = new OdbcConnection(redshiftConnectionString))
      {
        var adapter = new OdbcDataAdapter(query, connection);
        adapter.Fill(dataSet);
      }

      return dataSet;
    }
  }
}
