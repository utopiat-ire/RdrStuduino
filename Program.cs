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
			pluginManager.AddStandard(plugin);

			if (inofile == null)
			{
				string inoName = Path.GetFileNameWithoutExtension(rdrfile);
				string inoDir = Path.Combine(Path.GetDirectoryName(rdrfile), inoName);
				Directory.CreateDirectory(inoDir);
				inofile = Path.Combine(inoDir, inoName + ".ino");
			}
			else
			{
			}
			string inoFN = Path.GetFileName(inofile);

			ScriptParser parser = new ScriptParser(false);
			ProduireFile rdr = parser.ParseFrom(rdrfile, pluginManager);
			var compiler = new StuduinoCodeGenerator();
			if (!compiler.Compile(rdr, inofile))
			{
				Console.WriteLine("失敗!");
				return;
			}

			//設定
			string settingConfig = "settings.json";
			string json;
			if (File.Exists(settingConfig))
				json = File.ReadAllText(settingConfig);
			else
				json = "";

			Settings setting = JsonConvert.DeserializeObject<Settings>(json);

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
				var p = Process.Start("compile.bat");
				p.WaitForExit();
				Directory.Delete(build_dir, true);
				Directory.Delete(cache_dir, true);
			}
			Console.WriteLine("成功しました!");
		}

		private static void OutputUsage()
		{
			Console.WriteLine("RdrStuduino(プロデルStuduinoツール)");
			Console.WriteLine("Usage: [{/compile|/write}] プロデルファイル {出力先}");
		}
	}
}
