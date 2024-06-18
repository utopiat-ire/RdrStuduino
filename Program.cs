// Copyright(C) 2019 utopiat.net https://github.com/utopiat-ire/
using Newtonsoft.Json;
using Produire;
using Produire.Parser;
using Produire.TypeModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Produire.Translator
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				OutputUsage();
				return;
			}
			string mode = args[0];
			string rdrfile;
			string inofile = null;
			switch (mode)
			{
				case "/trans":
				case "/write":
					rdrfile = args[1];
					if (args.Length > 2) inofile = args[2];
					break;
				default:
					rdrfile = args[0];
					inofile = null;
					break;
			}

			PluginManager pluginManager = new PluginManager();
			var plugin = PluginManager.Load(typeof(Program).Assembly) as PNamespacePlugin;
			PluginManager.AddStandard(plugin);

			string rdrName = Path.GetFileNameWithoutExtension(rdrfile);
			string rdrDir = Path.GetDirectoryName(rdrfile);
			if (inofile == null)
			{
				string inoDir = Path.Combine(rdrDir, rdrName);
				Directory.CreateDirectory(inoDir);
				inofile = Path.Combine(inoDir, rdrName + ".ino");
			}
			else
			{
			}
			string inoFN = Path.GetFileName(inofile);

			//共通設定の読み込み
			string settingConfig = "settings.json";
			string json;
			if (File.Exists(settingConfig))
				json = File.ReadAllText(settingConfig);
			else
				json = "";
			Settings setting = JsonConvert.DeserializeObject<Settings>(json);

			//個別設定の読み込み
			string settingConfig2 = Path.Combine(rdrDir, rdrName + ".json");
			if (File.Exists(settingConfig2))
			{
				string json2 = File.ReadAllText(settingConfig2);
				Settings setting2 = JsonConvert.DeserializeObject<Settings>(json2);
				if (setting2.SensorPort != null) setting.SensorPort = setting2.SensorPort;
				if (setting2.ServomotorPort != null) setting.ServomotorPort = setting2.ServomotorPort;
				if (setting2.DCMotorPort != null) setting.DCMotorPort = setting2.DCMotorPort;
			}

			ScriptParser parser = new ScriptParser(false);
			ProduireFile rdr = parser.ParseFrom(rdrfile, pluginManager);
			var compiler = new StuduinoCodeGenerator();
			compiler.Settings=  setting;
			if (!compiler.Compile(rdr, inofile))
			{
				Console.WriteLine("変換に失敗しました。");
				return;
			}

			if (mode != "/trans")
			{
				//転送
				string temp = Path.GetTempPath();
				string build_dir = Path.Combine(temp, @"rdr_arduino_build");
				string cache_dir = Path.Combine(temp, @"rdr_arduino_cache");
				Directory.CreateDirectory(build_dir);
				Directory.CreateDirectory(cache_dir);
				string code = File.ReadAllText("board.bat");
				code = code.Replace("{Arduino_dir}", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Arduino\"));
				code = code.Replace("{lib_dir}", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Arduino\libraries"));
				code = code.Replace("{ino_file}", inofile);
				code = code.Replace("{comport}", setting.ComPort);
				code = code.Replace("{build_dir}", build_dir);
				code = code.Replace("{cache_dir}", cache_dir);
				code = code.Replace("{inoFileName}", inoFN);
				File.WriteAllText("compile.bat", code, Encoding.Default);

				Process p = new Process();
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardInput = false;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.CreateNoWindow = true;
				p.OutputDataReceived += p_OutputDataReceived;
				p.ErrorDataReceived += P_ErrorDataReceived;
				p.StartInfo.FileName = "compile.bat";

				p.Start();
				p.BeginOutputReadLine();
				p.BeginErrorReadLine();
				p.WaitForExit();

				Directory.Delete(build_dir, true);
				Directory.Delete(cache_dir, true);

				Console.WriteLine("完了しました。");
			}
			else
			{
				Console.WriteLine("成功しました!");
			}
		}

		private static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			Console.WriteLine(e.Data);
		}

		private static void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			Console.WriteLine(e.Data);
		}
		private static void OutputUsage()
		{
			Console.WriteLine("RdrStuduino(プロデルStuduinoツール)");
			Console.WriteLine("Usage: [{/compile|/write}] プロデルファイル {出力先}");
		}
	}
}
