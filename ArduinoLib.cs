// Copyright(C) 2019 utopiat.net https://github.com/utopiat-ire/
using System;

namespace Produire.Translator
{
	public class ArduinoLib : IProduireGlobalClass
	{
		[翻訳("delay(【時間】* 1000UL)")]
		public void 待つ([だけ]int 時間)
		{

		}
		[翻訳("【名前】=【名前】+【値】")]
		public void 増やす([を]string 名前, [だけ]int 値)
		{

		}
		/*
		 * ～度だけ回す
		 */
		[手順("設定")]
		[翻訳("board.Servomotor(【モータ】, SVRANGE(【角度】))")]
		public void 設定([を]サーボモーター モータ, [へ]int 角度)
		{

		}
		[手順名("LEDを", "点灯する")]
		[翻訳("board.LED(【ポート】,ON)")]
		public void LEDを点灯する([で]ポート ポート)
		{

		}
		[手順名("LEDを", "消灯する")]
		[翻訳("board.LED(【ポート】,OFF)")]
		public void LEDを消灯する([で]ポート ポート)
		{

		}

		[手順名("DCモーターM1の速さを", "設定")]
		[翻訳("board.DCMotorPower(PORT_M1, DCMPWR(【速さ】))")]
		public void DCモーターM1の速さを設定([助詞("へ")]int 速さ)
		{

		}
		[手順名("DCモーターM2の速さを", "設定")]
		[翻訳("board.DCMotorPower(PORT_M2, DCMPWR(【速さ】))")]
		public void DCモーターM2の速さを設定([助詞("へ")]int 速さ)
		{

		}
		[翻訳("board.DCMotorControl(【モータ】,NORMAL)")]
		public void 正転([を]DCモーター モータ)
		{

		}
		[翻訳("board.DCMotorControl(【モータ】,REVERSE)")]
		public void 逆転([を]DCモーター モータ)
		{

		}
		[翻訳("board.DCMotorControl(【モータ】,NORMAL)")]
		public void 停止([を]DCモーター モータ)
		{

		}
		[翻訳("board.DCMotorControl(【モータ】,COAST)")]
		public void 解放([を]DCモーター モータ)
		{

		}
		[翻訳("PUSHSWITCH(【センサー】)")]
		public void ボタンの値([から]ポート センサー)
		{

		}
		[翻訳("LIGHT_SENSOR(【センサー】)")]
		public void 光センサーの値([から]ポート センサー)
		{

		}
		[翻訳("ACCELEROMETER(【センサー】)")]
		public void 加速度センサーの値([から]軸 軸)
		{

		}
		[翻訳("IRPHOTOREFLECTOR(【センサー】)")]
		public void 赤外線フォトリフレクタの値([から]ポート センサー)
		{

		}
		[翻訳("SOUND_SENSOR(【センサー】)")]
		public void 音センサーの値([から]ポート センサー)
		{

		}
		[翻訳("TEMPERATURE_SENSOR(【センサー】)")]
		public void 温度センサーの値([から]ポート センサー)
		{

		}
		[翻訳("random(【最小値】, (【最大値】+1))")]
		public void 乱数([から]int 最小値, [まで]int 最大値)
		{

		}
		[翻訳("getTimer()")]
		public void タイマー()
		{

		}
		[手順名("タイマーを", "リセット")]
		[翻訳("resetTimer()")]
		public void タイマーをリセット()
		{

		}
		[翻訳("math(SQRT,【値】)")]
		public void 平方根([既定]float 値)
		{

		}
		[翻訳("math(ABS,【値】)")]
		public void 絶対値([既定]float 値)
		{

		}
		[翻訳("math(SIN,【値】)")]
		public void SIN([既定]float 値)
		{

		}
		[翻訳("math(COS,【値】)")]
		public void COS([既定]float 値)
		{

		}
		[翻訳("math(TAN,【値】)")]
		public void TAN([既定]float 値)
		{

		}
		[翻訳("math(LN,【値】)")]
		public void ログ([既定]float 値)
		{

		}
	}

	[列挙体]
	public enum DCモーター
	{
		[翻訳("PORT_M1")]
		DCモーターM1,
		[翻訳("PORT_M2")]
		DCモーターM2
	}
	[列挙体]
	public enum サーボモーター
	{
		[翻訳("PORT_D9")]
		サーボモーターD9,
		[翻訳("PORT_D10")]
		サーボモーターD10,
		[翻訳("PORT_D11")]
		サーボモーターD11,
		[翻訳("PORT_D12")]
		サーボモーターD12
	}
	[列挙体]
	public enum ポート
	{
		[翻訳("PORT_A0")]
		A0,
		[翻訳("PORT_A1")]
		A1,
		[翻訳("PORT_A2")]
		A2,
		[翻訳("PORT_A3")]
		A3,
		[翻訳("PORT_A4")]
		A4,
		[翻訳("PORT_A5")]
		A5,
		[翻訳("PORT_A6")]
		A6,
		[翻訳("PORT_A7")]
		A7,
	}
	[列挙体]
	public enum 軸
	{
		[翻訳("X")]
		X,
		[翻訳("Y")]
		Y,
		[翻訳("X")]
		Z,
	}

}
