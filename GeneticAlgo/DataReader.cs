using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace GeneticAlgo
{
  public class DataReader
  {
    public List<User> ReadData()
    {
      var path = @"<yourpath>\GeneticAlgo\data\RetailMart.xlsx";

      var excel = new ExcelPackage(new FileInfo(path));
      // Pak de 'Linear Model' tab van het excel bestand.
      var worksheet = excel.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Linear Model");

      // Gekozen Rijen van excel bestand.
      var rows = worksheet.SelectedRange[8, 2, 1007, 22];

      // Customers List
      var customers = new List<User>();

      // Rijen values
      var data = ((object[,])rows.Value);

      for (int i = 0; i < rows.Rows; i++)
      {
        var user = new User();
        var dna = new List<int>();
        user.Name = "User: " + i;
        for (int y = 0; y < rows.Columns; y++)
        {
          if (y == rows.Columns - 1)
          {
            // Assign de pregnant waardes per user
            user.Pregnant = int.Parse(data[i, y].ToString());
          }
          else
          {
            dna.Add(int.Parse(data[i, y].ToString()));
          }
        }
        user.DNA = dna;
        customers.Add(user);
      }

      // return 1000 klanten 
      // (DNA: producten values(20: 0 of 1), Name: User 0, Pregnant: pregnant value (0 of 1), Prediction value)
      return customers;
    }

    public class User
    {
      public string Name { get; set; }
      public List<int> DNA { get; set; }
      public int Pregnant { get; set; }
      public double Prediction { get; set; } = 0.00;
      public double SE { get; set; } = 0.00;
      public double LowerBoundry { get; set; }
      public double UpperBoundry { get; set; }

      // Prediction Method
      public void CalculatePrediction(List<double> Coefficienten)
      {
        for (int i = 0; i < Coefficienten.Count; i++)
        {
          Prediction += DNA[i] * Coefficienten[i];
        }

        Prediction = Math.Round(Prediction, 2);
      }

      // SE Method
      public void CalculateSE()
      {
        SE = Math.Pow(Pregnant - Prediction, 2);
      }
    }
  }
}
