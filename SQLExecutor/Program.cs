using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace SQLExecutor {
    public class Program {
        private static string ConnectionString { get; set; }
        private static string DirectoryPath { get; set; }
        private static bool DeleteAfterwards { get; set; }
        private static int TotalFiles { get; set; }
        private static int TotalExecuted { get; set; }
        private static List<string> NotExecuted { get; set; }

        public static void Main(string[] args) {
            Console.Title = "SQL Executor";

            NotExecuted = new List<string>();

            Console.WriteLine("Connection String: ");
            ConnectionString = Console.ReadLine();

            Console.Write("Directory Path: ");
            DirectoryPath = Console.ReadLine();

            Console.Write("Delete scripts after succesfull execution ('true' or 'false', default: 'false'): ");
            DeleteAfterwards = Console.ReadLine() == "true";

            WriteLine();

            DirSearch(DirectoryPath);

            WriteLine();

            Console.WriteLine("Total scripts: " + TotalFiles + ", total executed: " + TotalExecuted);

            WriteLine();

            Console.WriteLine("Non executed scripts: ");
            Console.WriteLine();

            foreach (var ne in NotExecuted)
                Console.WriteLine(ne);

            WriteLine();

            Console.WriteLine("Type 'exit' to close!");

            while (Console.ReadLine() != "exit") ;
        }

        private static void DirSearch(string sDir) {
            try {
                ListFiles(Directory.GetFiles(sDir).Where(x => x.EndsWith(".sql")).ToArray());
                foreach (var d in Directory.GetDirectories(sDir)) {
                    DirSearch(d);
                }
            } catch (System.Exception excpt) {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void ListFiles(string[] files) {
            foreach (var f in files) {
                ++TotalFiles;
                ExecuteSql(f.Split('\\').Last(), File.ReadAllText(f), f);
            }
        }

        private static void ExecuteSql(string filename, string query, string fullName) {
            try {
                var con = new SqlConnection(ConnectionString);
                var cmd = new SqlCommand(query, con);
                con.Open();
                var rows = cmd.ExecuteNonQuery();
                con.Close();

                ++TotalExecuted;
                Console.WriteLine(filename + " executed, rows affected: " + rows);
            } catch (Exception e) {
                NotExecuted.Add(fullName);
                Console.WriteLine("Error executing script: " + filename);
            }

        }

        private static void WriteLine(int n = 2) {
            Console.WriteLine();
            for (var i = 0; i < n; ++i)
                Console.WriteLine("========================================================================");
            Console.WriteLine();
        }
    }
}
