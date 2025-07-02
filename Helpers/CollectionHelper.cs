using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System.Data;

namespace Smart_Document_Management_System.Helpers
{
    public class CollectionHelper
    {
        internal static CollectionHelper Instance = null;
        internal static string Application_StartupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        internal static DataSet ds_Config = new DataSet();
    }
}
