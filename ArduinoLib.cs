// Copyright(C) 2019 utopiat.net https://github.com/utopiat-ire/
using System;

namespace Produire.Translator
{
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
