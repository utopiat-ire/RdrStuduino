// Copyright(C) 2019 utopiat.net https://github.com/utopiat-ire/
using System;
using System.Collections.Generic;

namespace Produire.Translator
{
	public class Settings
	{
		public string ComPort = "COM4";
		public string[] DCCalibrationData;
		public string[] DCMotorPort;
		public string[] SvCalibrationData;
		public IList<string> ServomotorPort;
		public IDictionary<string, string> SensorPort;
	}
}
