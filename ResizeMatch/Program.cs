using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResizeMatch.classes;
using System.IO;
namespace ResizeMatch
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathOfFolder = "results";
            DirectoryInfo mainDirectory = new DirectoryInfo(pathOfFolder);
            DirectoryInfo[] subDirectories = mainDirectory.GetDirectories();

            // define the format of csv file;
            string filePath = pathOfFolder + @"/test.csv";
            string delimiter = ",";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(delimiter, "\"clusterName\"", "\"respresentativePic\""));
            File.AppendAllText(filePath, sb.ToString());
            sb.Clear();
            
            // start loop for each subFolder
            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                ClusterMatch clusterMatch = new ClusterMatch();
                sb.AppendLine(string.Join(delimiter, subDirectory.ToString(), clusterMatch.Match(pathOfFolder + "\\" + subDirectory + "\\")));
                File.AppendAllText(filePath, sb.ToString());
                sb.Clear();
            }

            Console.ReadKey();
            
        }
    }
}
