using Dapper;
using MySqlConnector;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
namespace Smart_Document_Management_System.Helpers
{
    public class CommonMethods
    {
        internal static string Table = CommonMethods.GetFromConfig("Database", "Table");
        internal static string DB = GetFromConfig("Database", "ConnectionString");
        internal static void Initialize()
        {
            ReadConfig();
            LogPathInitialize();
        }
        private static void ReadConfig() => CollectionHelper.ds_Config.ReadXml(Path.Combine(CollectionHelper.Application_StartupPath, "config.xml"));
        internal static string GetFromConfig(string TableName, string ColumnName, int Row = 0) => CollectionHelper.ds_Config.Tables[TableName].Rows[Row][ColumnName].ToString();

        public static void LogPathInitialize()
        {
            // Implement your logging logic here
            if (!Directory.Exists(GetFromConfig("OTHER", "LOG-PATH")))
                Directory.CreateDirectory(GetFromConfig("OTHER", "LOG-PATH"));
            if (!Directory.Exists(Path.Combine(GetFromConfig("OTHER", "LOG-PATH"), $@"{DateTime.Now:ddMMMyyyy}")))
                Directory.CreateDirectory(Path.Combine(GetFromConfig("OTHER", "LOG-PATH"), $@"{DateTime.Now:ddMMMyyyy}"));
            if (!File.Exists(Path.Combine(GetFromConfig("OTHER", "LOG-PATH"), $@"{DateTime.Now:ddMMMyyyy}", "DebugLog.txt")))
                File.Create(Path.Combine(GetFromConfig("OTHER", "LOG-PATH"), $@"{DateTime.Now:ddMMMyyyy}", "DebugLog.txt")).Close();
            File.WriteAllText(Path.Combine(GetFromConfig("OTHER", "LOG-PATH"), $@"{DateTime.Now:ddMMMyyyy}", "DebugLog.txt"), $"[Server Initialized] at {DateTime.Now:dd-MM-yyyy HH:mm:ss:fff tt}");
        }

        public static void DebugLog(string message)
        {
            string logFilePath = Path.Combine(GetFromConfig("OTHER", "LOG-PATH"), $@"{DateTime.Now:ddMMMyyyy}", "DebugLog.txt");
            string logMessage = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss:fff tt} - {message}";
            using (StreamWriter sw = new StreamWriter(logFilePath, true)) { sw.WriteLine(logMessage); }
        }

        public static void ExceptionLog(Exception ex, string methodName)
        {
            string logFilePath = Path.Combine(GetFromConfig("OTHER", "LOG-PATH"), $@"{DateTime.Now:ddMMMyyyy}", "ExceptionLog.txt");
            string logMessage = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss:fff tt} - {methodName} \nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
            using (StreamWriter sw = new StreamWriter(logFilePath, true)) { sw.WriteLine(logMessage); }
        }
        public static bool ExtractTextFrom(string FilePath, out string textString)
        {
            StringBuilder text = new StringBuilder();
            textString = string.Empty;
            bool isSuccess = false;
            try
            {
                if (FilePath.ToLower().Contains(".pdf"))
                {

                    using (PdfDocument pdf = PdfDocument.Open(FilePath))
                    {
                        foreach (Page page in pdf.GetPages())
                        {
                            text.Append(page.Text);
                        }
                    }
                    isSuccess = true;
                }
                else if (FilePath.ToLower().Contains(".txt"))
                {
                    text.Append(File.ReadAllText(FilePath));
                    isSuccess = true;
                }
                else
                {
                    text.Append("Unsupported file format. Please provide a PDF or TXT file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text from PDF: {ex.Message}");
                return isSuccess;
            }
            textString = text.ToString();
            return isSuccess;
        }
        public static bool DBUpdateQuery(string guid,string FileName,string Summary, string Category, string Reasoning, string text, string embeddingJson)
        {
            bool isSuccess = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB))
                {
                    string query = $"INSERT INTO {Table} (ID,FileName,Summary, Category, Reasoning, Content, Embedding) VALUES (\"{guid}\",\"{FileName}\",\"{Summary.Replace("\"","")}\",\"{Category}\", \"{Reasoning.Replace("\"", "")}\", \"{text.Replace("\"", "")}\", \"{embeddingJson}\")";
                    conn.Open();
                    var result = conn.Query(query);
                    
                        isSuccess = true;
                    
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex, nameof(DBUpdateQuery));
            }
            return isSuccess;
        }
        public static bool ExistInDB(string fileName)
        {
            bool isExists = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB))
                {
                    string query = $"SELECT COUNT(*) FROM {Table} WHERE FileName = \"{fileName}\"";
                    conn.Open();
                    var result = conn.QuerySingle<int>(query);
                    isExists = result > 0;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex, nameof(ExistInDB));
            }
            return isExists;
        }
        internal static float CosineSimilarity(List<float> v1, List<float> v2)
        {
            float dot = 0, mag1 = 0, mag2 = 0;
            for (int i = 0; i < v1.Count; i++)
            {
                dot += v1[i] * v2[i];
                mag1 += v1[i] * v1[i];
                mag2 += v2[i] * v2[i];
            }
            return dot / ((float)Math.Sqrt(mag1) * (float)Math.Sqrt(mag2));
        }
    }
}