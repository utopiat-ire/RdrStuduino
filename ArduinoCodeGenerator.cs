// Copyright(C) 2019 utopiat.net https://github.com/utopiat-ire/
using System;
using System.Collections.Generic;
using System.Text;
using Produire.Model.Phrase;
using Produire.Model;
using Produire.Model.Statement;
using Produire.Parser;
using Produire.TypeModel;
using System.IO;
using Newtonsoft.Json;

namespace Produire.Translator
{
	public class StuduinoCodeGenerator
	{
		#region デリゲート

		protected delegate void StatementDelegate(IStatement statement, StringWriter writer);
		protected delegate void CodeElementDelegate(ICodeElement element, StringWriter writer);

		#endregion

		#region フィールド

		DelegateList<StatementDelegate> actions = new DelegateList<StatementDelegate>();
		DelegateList<CodeElementDelegate> evals = new DelegateList<CodeElementDelegate>();

		#endregion

		#region コンストラクタ

		public StuduinoCodeGenerator()
		{
			InitStatementDelegate();
			InitCodeElementDelegate();
			InitPhraseDelegate();
		}

		private void InitStatementDelegate()
		{
			actions.Add(PhraseTypes.StaticCallExpression, new StatementDelegate(GSStaticCall));
			actions.Add(PhraseTypes.DynamicCallExpression, new StatementDelegate(GSDynamicCall));
			actions.Add(PhraseTypes.繰り返し文, new StatementDelegate(GS繰り返し文));
			actions.Add(PhraseTypes.もし文, new StatementDelegate(GSもし文));
			actions.Add(PhraseTypes.分岐文, new StatementDelegate(GS分岐文));

			actions.Add(PhraseTypes.WithStatement, new StatementDelegate(GSWith));
			actions.Add(PhraseTypes.TryCatchStatement, new StatementDelegate(GSTryCatch));
			actions.Add(PhraseTypes.AssignEventStatement, new StatementDelegate(GSAssignEvent));
			actions.Add(PhraseTypes.NoArgsProcedureCallStatement, new StatementDelegate(GS引数無し手順呼出し));
			actions.Add(PhraseTypes.CreateObjectExpression, new StatementDelegate(GSCreateObject));
			actions.Add(PhraseTypes.SetPropertyValueStatement, new StatementDelegate(GSSetPropertyValue));

			actions.Add(PhraseTypes.代入文, new StatementDelegate(GS代入文));
			actions.Add(PhraseTypes.ProcessAndAssignStatement, new StatementDelegate(GS呼出し代入文));

			actions.Add(PhraseTypes.ParseErrorStatements, new StatementDelegate(GSParseError));
			actions.Add(PhraseTypes.変数宣言句, new StatementDelegate(GS変数宣言句));
			actions.Add(PhraseTypes.DeclareFieldStatement, new StatementDelegate(GS変数宣言句));
			actions.Add(PhraseTypes.返す文, new StatementDelegate(GS返す文));
			actions.Add(PhraseTypes.抜け出す文, new StatementDelegate(GS抜け出す文));
		}

		private void InitCodeElementDelegate()
		{
			evals.Add(PhraseTypes.手順定義, new CodeElementDelegate(GE手順定義));
			evals.Add(PhraseTypes.固定手順定義, new CodeElementDelegate(GE手順定義));
			evals.Add(PhraseTypes.種類定義, new CodeElementDelegate(GE種類定義));
			evals.Add(PhraseTypes.NamespacePart, new CodeElementDelegate(GENamespacePart));
		}

		private void InitPhraseDelegate()
		{
			actions.Add(PhraseTypes.ArithmeticFormula, new StatementDelegate(D数式));
			actions.Add(PhraseTypes.TermFormula, new StatementDelegate(D数式));
			actions.Add(PhraseTypes.FactorFormula, new StatementDelegate(D数式));

			actions.Add(PhraseTypes.NumberTokenInt, new StatementDelegate(D整数字句));
			actions.Add(PhraseTypes.NumberTokenLong, new StatementDelegate(D長整数字句));
			actions.Add(PhraseTypes.NumberTokenFloat, new StatementDelegate(D浮動小数字句));
			actions.Add(PhraseTypes.真値字句, new StatementDelegate(D真偽値定数));
			actions.Add(PhraseTypes.偽値字句, new StatementDelegate(D真偽値定数));
			actions.Add(PhraseTypes.バツ字句, new StatementDelegate(Dバツ字句));
			actions.Add(PhraseTypes.加算演算子字句, new StatementDelegate(D演算子字句));
			actions.Add(PhraseTypes.減算演算子字句, new StatementDelegate(D演算子字句));
			actions.Add(PhraseTypes.乗算演算子字句, new StatementDelegate(D演算子字句));
			actions.Add(PhraseTypes.除算演算子字句, new StatementDelegate(D演算子字句));
			actions.Add(PhraseTypes.剰演算子字句, new StatementDelegate(D演算子字句));
			actions.Add(PhraseTypes.RemainderOperatorToken, new StatementDelegate(D演算子字句));
			actions.Add(PhraseTypes.列挙体名字句, new StatementDelegate(D列挙体名字句));
			actions.Add(PhraseTypes.定数値名字句, new StatementDelegate(D定数値名字句));

			actions.Add(PhraseTypes.空白字句, new StatementDelegate(D空白字句));
			actions.Add(PhraseTypes.コメント字句, new StatementDelegate(D空白字句));

			actions.Add(PhraseTypes.文字列定数字句, new StatementDelegate(D文字列定数字句));
			actions.Add(PhraseTypes.文字列定数句, new StatementDelegate(D文字列定数句));
			actions.Add(PhraseTypes.連結文字列句, new StatementDelegate(D連結文字列句));
			actions.Add(PhraseTypes.配列句, new StatementDelegate(D配列句));
			actions.Add(PhraseTypes.辞書定数句, new StatementDelegate(D辞書定数句));
			actions.Add(PhraseTypes.NoArgsProcedureCallPhrase, new StatementDelegate(GS引数無し手順呼出し));

			actions.Add(PhraseTypes.変数字句, new StatementDelegate(D変数字句));
			actions.Add(PhraseTypes.レシーバ呼出句, new StatementDelegate(Dレシーバ呼出句));
			actions.Add(PhraseTypes.ブロック句, new StatementDelegate(Dブロック句));
			actions.Add(PhraseTypes.無字句, new StatementDelegate(D無));

			actions.Add(PhraseTypes.カンマ列挙句, new StatementDelegate(Dカンマ列挙句));
			actions.Add(PhraseTypes.静的変数式, new StatementDelegate(D静的変数式));

			actions.Add(PhraseTypes.IndexerAccessPhrase, new StatementDelegate(DIndexerAccessPhrase));
			actions.Add(PhraseTypes.設定項目アクセス句, new StatementDelegate(D設定項目アクセス句));
			actions.Add(PhraseTypes.フィールドアクセス句, new StatementDelegate(Dフィールドアクセス句));
			actions.Add(PhraseTypes.ObjectAccessPhrase, new StatementDelegate(DObjectAccessPhrase));

			actions.Add(PhraseTypes.ExpressionCondition, new StatementDelegate(DConditionPhrase));
			actions.Add(PhraseTypes.FactorCondition, new StatementDelegate(DConditionPhrase));
			actions.Add(PhraseTypes.SingleCondition, new StatementDelegate(DConditionPhrase));
			actions.Add(PhraseTypes.NotCondition, new StatementDelegate(DNotCondition));
			actions.Add(PhraseTypes.TypeOfCondition, new StatementDelegate(DConditionTypeOfPhrase));

			actions.Add(PhraseTypes.日本語条件句, new StatementDelegate(D日本語条件句));
			actions.Add(PhraseTypes.末尾付き日本語条件句, new StatementDelegate(D末尾付き日本語条件句));
			actions.Add(PhraseTypes.論理積字句, new StatementDelegate(D論理字句));
			actions.Add(PhraseTypes.論理和字句, new StatementDelegate(D論理字句));
			actions.Add(PhraseTypes.比較演算子字句, new StatementDelegate(D比較演算子字句));
			actions.Add(PhraseTypes.等価比較演算子字句, new StatementDelegate(D等価比較演算子字句));

			actions.Add(PhraseTypes.四角囲い句, new StatementDelegate(D囲い句));
			actions.Add(PhraseTypes.DoubleQuotedPhrase, new StatementDelegate(D囲い句));
			actions.Add(PhraseTypes.丸囲い句, new StatementDelegate(D囲い句));
			actions.Add(PhraseTypes.未知語字句, new StatementDelegate(D未知語字句));
			actions.Add(PhraseTypes.ひらがな字句, new StatementDelegate(D未知語字句));
			actions.Add(PhraseTypes.ParseErrorPhrase, new StatementDelegate(DParseErrorPhrase));
			actions.Add(PhraseTypes.IncompleteStatement, new StatementDelegate(DIncompleteStatement));
			actions.Add(PhraseTypes.IncompleteStatement2, new StatementDelegate(DIncompleteStatement));

			actions.Add(PhraseTypes.KeywordToken, new StatementDelegate(DKeywordToken));
		}

		#endregion

		#region メソッド

		List<Procedure> functionList = new List<Procedure>();
		Dictionary<Procedure, string> funcNameList = new Dictionary<Procedure, string>();
		Dictionary<PVariable, string> varNameList = new Dictionary<PVariable, string>();
		public bool Compile(ProduireFile rdr, string fullPath)
		{
			foreach (Procedure procedure in rdr.Global.Procedures)
			{
				if (procedure is FixedProcedure) continue;
				string name = procedure.UniqueName;
				if (name == "スタート")
					name = "artecRobotMain";
				else
				{
					string text = ConvertToAscii(name);
					if (text.Length == 0)
					{
						text = "Func";
					}
					name = "ARSR_" + text;
					int number = 1;
					while (funcNameList.ContainsValue(name))
					{
						name = "ARSR_" + text + number;
						number++;
					}
				}
				funcNameList[procedure] = name;
				functionList.Add(procedure);
			}

			StringWriter writer = new StringWriter();
			writer.Write(File.ReadAllText("head.txt"));
			WriteSettings(writer);
			Generate(rdr, writer);
			writer.Write(File.ReadAllText("foot.txt"));

			writer.Close();
			File.WriteAllText(fullPath, writer.ToString());
			return true;
		}

		const string Letters = "0123456789abcdefghijklmnopqrstuwvxyzABCDEFGHIJKLMNOPQRSTUWVXYZ";

		Settings settings;
		public Settings Settings
		{
			get { return settings; }
			set { settings = value; }
		}

		private string ConvertToAscii(string name)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				if (Letters.IndexOf(c) == -1) c = Letters[i % 28 + 10];
				builder.Append(c);
			}
			return builder.ToString();
		}

		private void WriteSettings(StringWriter writer)
		{
			writer.Write(@"
// ---------------------------------------
// Servomotor calibration data
// ---------------------------------------
");
			writer.Write("char SvCalibrationData[] = { " + string.Join(", ", settings.SvCalibrationData) + " };");
			writer.Write(@"
// ---------------------------------------
// DC motor calibration data
// ---------------------------------------
");
			writer.Write("byte DCCalibrationData[] = {" + string.Join(", ", settings.DCCalibrationData) + " };");
			writer.Write(@"
// ---------------------------------------
// prototype declaration
// ---------------------------------------
");
			foreach (Procedure procedure in functionList)
			{
				writer.WriteLine(@"void " + funcNameList[procedure] + "();");
			}
			writer.Write(@"
	// ---------------------------------------
	// Global variable
	// ---------------------------------------
	byte port[8];
byte degree[8];
// ---------------------------------------
// Artec robot setup routine
// ---------------------------------------
void artecRobotSetup() {
	board.SetServomotorCalibration(SvCalibrationData);
	board.SetDCMotorCalibration(DCCalibrationData);
");
			foreach (var item in settings.DCMotorPort)
			{
				writer.WriteLine("\tboard.InitDCMotorPort(PORT_" + item + ");");
			}
			foreach (var item in settings.ServomotorPort)
			{
				writer.WriteLine("\tboard.InitServomotorPort(PORT_" + item + ");");
			}
			foreach (var pair in settings.SensorPort)
			{
				writer.WriteLine("\tboard.InitSensorPort(PORT_" + pair.Key + ", " + pair.Value + ");");
			}
			writer.Write(@"
}
");
		}

		public bool Compile(List<ProduireFile> list, string fullPath)
		{
			StringWriter writer = new StringWriter();

			foreach (var rdr in list)
			{
				Generate(rdr, writer);
			}

			writer.Close();
			File.WriteAllText(fullPath, writer.ToString());
			return true;
		}
		public void Generate(ProduireFile rdr, StringWriter writer)
		{
			if (rdr == null) throw new ArgumentNullException("rdr");

			//開始の手順の生成
			FixedProcedure entry = rdr.Global.EntryCode;
			var statements = entry.Statements;

			if (statements.Count != 0)
			{
				Generate(statements, writer);
			}

			//手順と種類の生成
			CodeElementCollection globalCodeList = rdr.Global.CodeList;
			if (globalCodeList.Count != 0)
			{
				string indent = globalCodeList.Indent;
				for (int i = 0; i < globalCodeList.Count; i++)
				{
					ICodeElement element = globalCodeList[i];
					if (element is FixedProcedure &&
						(element as Procedure).Kind == Procedure.ProcedureKinds.はじめ)
						continue;

					if (element.PhraseType == PhraseTypes.改行字句) continue;
					Generate(element, writer, indent);
				}
			}
		}

		/// <summary>コードリストを生成します</summary>
		private void Generate(CodeElementCollection codeList, StringWriter writer)
		{
			string indent = codeList.Indent;
			foreach (ICodeElement element in codeList)
			{
				if (element.PhraseType == PhraseTypes.改行字句) continue;
				Generate(element, writer, indent);
			}
		}

		/// <summary>フレーズからプログラムを生成します</summary>
		private void Generate(IPhrase phrase, StringWriter writer)
		{
			if (phrase == null) return;
			StatementDelegate act;
			if (actions.TryGetValue(phrase.PhraseType, out act))
				act(phrase, writer);
			else if (phrase is IPhraseContainer)
			{
				Generate((phrase as IPhraseContainer).Phrases, writer);
			}
		}

		private void Generate(IPhrase[] phrases, StringWriter writer)
		{
			for (int i = 0; i < phrases.Length; i++)
			{
				Generate(phrases[i], writer);
			}
		}

		private void Generate(IPhrase phrase, CodePartsType type, StringWriter writer)
		{
			writer.Write(phrase.Text);
		}

		private void Generate(IPhrase[] phrases, CodePartsType type, StringWriter writer)
		{
			for (int i = 0; i < phrases.Length; i++)
			{
				Generate(phrases[i], type, writer);
			}
		}

		#endregion

		#region デリゲートハンドリング

		/// <summary>種類を生成します</summary>
		internal void GE種類定義(ICodeElement element, StringWriter writer)
		{
			Construct construct = element as Construct;
		}

		/// <summary>名前空間を生成します</summary>
		internal void GENamespacePart(ICodeElement element, StringWriter writer)
		{
		}

		/// <summary>コードを生成します</summary>
		/// <param name="element">生成するコード要素</param>
		/// <param name="writer">出力先のStringWriter</param>
		private void Generate(ICodeElement element, StringWriter writer, string indent)
		{
			CodeElementDelegate act;
			if (evals.TryGetValue(element.PhraseType, out act))
			{
				act(element, writer);
			}
			else if (element is IPhrase)
			{
				Generate(element as IPhrase, writer);
			}
			else if (element is IStatement)
			{
				IStatement statement = element as IStatement;
				Generate(statement, writer);
			}
		}

		private void Generate(IStatement statement, StringWriter writer)
		{
			//文を出力
			StatementDelegate act;
			if (actions.TryGetValue(statement.PhraseType, out act))
			{
				act(statement, writer);
			}
		}

		/// <summary>手順を生成します</summary>
		internal void GE手順定義(ICodeElement element, StringWriter writer)
		{
			Procedure procedure = element as Procedure;
			string name = funcNameList[procedure];
			writer.Write("void ");
			writer.Write(name);
			GenerateProcedureHeader(procedure, writer);
			writer.Write("{");

			var statements = procedure.Statements;
			Generate(statements, writer);
			writer.WriteLine("}");
		}


		public static string GenerateProcedureHeader(Procedure procedure)
		{
			StringWriter writer = new StringWriter();
			StuduinoCodeGenerator generator = new StuduinoCodeGenerator();
			generator.GenerateProcedureHeader(procedure, writer);
			return writer.ToString();
		}

		/// <summary>手順名を出力します</summary>
		private void GenerateProcedureHeader(Procedure procedure, StringWriter writer)
		{
			writer.Write("(");
			writer.Write(")");
		}

		public string GenerateString(StatementCollection statements, int startLineIndex)
		{
			StringBuilder builder = new StringBuilder();
			StringWriter writer = new StringWriter();
			Generate(statements, writer);
			writer.Close();
			return builder.ToString();
		}
		public string GenerateString(IPhrase phrase)
		{
			StringWriter writer = new StringWriter();
			Generate(phrase, writer);
			writer.Close();
			return writer.ToString();
		}

		/// <summary>StatementCollectionにあるコードを生成</summary>
		internal void Generate(StatementCollection statements, StringWriter writer)
		{
#if !DEBUG
			try
#endif
			{
				string indent = statements.Indent;
				for (int i = 0; i < statements.Count; i++)
				{
					IStatement statement = statements[i];
					if (statement.PhraseType == PhraseTypes.改行字句)
					{
						writer.WriteLine();
						continue;
					}
					if (statement.PhraseType == PhraseTypes.ブロック終了文)
						continue;
					if (statement is IPhrase)
						Generate(statement as IPhrase, writer);
					else
						Generate(statement, writer);
					if (statement.PhraseType == PhraseTypes.もし文
						|| statement.PhraseType == PhraseTypes.繰り返し文
						|| statement.PhraseType == PhraseTypes.コメント字句
						|| statement.PhraseType == PhraseTypes.空白字句)
						continue;
					writer.Write(";");
				}
			}
#if !DEBUG
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
#endif
		}

		private void GenerateStruct(IPhrase[] phrases, StringWriter writer)
		{
			for (int i = 0; i < phrases.Length; i++)
			{
				var phrase = phrases[i];
				if (phrase is 実補語句)
					GenerateStruct((phrase as 実補語句).Phrases, writer);
				else if (phrase is 助詞字句)
				{
				}
				else if (phrase is 動詞字句 && (phrase as 動詞字句).Verb is 予約済み動詞定義)
				{
				}
				else
					Generate(phrases[i], writer);
			}
		}

		private void GSSentence(IStatement statement, StringWriter writer)
		{
			var st = statement as Sentence;
			if (st.Phrases != null)
			{
				Generate(st.Phrases, writer);
			}
		}
		private void GSStaticCall(IStatement statement, StringWriter writer)
		{
			var st = statement as StaticCallExpression;
			TranscriptSentence(writer, st.MethodInfo, st.Expression);
		}

		private void GSDynamicCall(IStatement statement, StringWriter writer)
		{
			var st = statement as DynamicCallExpression;
			手順定義 methodInfo = st.Overloads[0];
			foreach (var method in st.Overloads)
			{
				if (method is Procedure)
				{
					methodInfo = method;
					break;
				}
			}
			TranscriptSentence(writer, methodInfo, st.Expression);
		}

		private void TranscriptSentence(StringWriter writer, 手順定義 methodInfo, IPrototypeExpression expr)
		{
			string code = "";
			if (methodInfo is 外部手順定義)
			{
				var attrs = (methodInfo as 外部手順定義).MethodInfo.GetCustomAttributes(typeof(翻訳Attribute), false);
				if (attrs.Length > 0) code = (attrs[0] as 翻訳Attribute).Code;
			}
			else if (methodInfo is Procedure)
			{
				var procedure = methodInfo as Procedure;
				code = procedure.GetAnnotationParam("翻訳");
				string funcName;
				if (code == null && funcNameList.TryGetValue(procedure, out funcName))
				{
					code = funcName + "()";
				}
			}
			foreach (var item in methodInfo.Complements)
			{
				var pc = item as 実補語定義;

				if (pc != null)
				{
					IPhrase phrase;
					expr.TryGetPhrase(pc.Particle, out phrase);
					code = code.Replace("【" + pc.ParameterName + "】", GenerateString(phrase));
				}
			}
			writer.Write(code);
		}

		private void GSCreateObject(IStatement statement, StringWriter writer)
		{
			var st = statement as CreateObjectExpression;
			GSSentence(st.Sentence, writer);
		}
		private void GSSetPropertyValue(IStatement statement, StringWriter writer)
		{
			var st = statement as SetPropertyValueStatement;
			GSSentence(st.Sentence, writer);
		}
		private void GS引数無し手順呼出し(IStatement statement, StringWriter writer)
		{
			writer.Write(statement);
		}
		private void GS返す文(IStatement statement, StringWriter writer)
		{
			var st = statement as 返す文;
			writer.Write("return ");
			Generate(st.ReturnPhrase, writer);
		}
		private void GS抜け出す文(IStatement statement, StringWriter writer)
		{
			var st = statement as 抜け出す文;
			writer.Write("break");
		}
		private void GS変数宣言句(IStatement statement, StringWriter writer)
		{
			var st = statement as 変数宣言句;
			string name = st.Variable.Name;
			string text = ConvertToAscii(name);
			if (text.Length == 0)
			{
				text = "VAR";
			}
			name = "ARVAL_" + text;
			int number = 1;
			while (varNameList.ContainsValue(name))
			{
				name = "ARVAL_" + text + number;
				number++;
			}
			varNameList[st.Variable] = name;
			writer.Write("float " + name);
		}
		private void GS呼出し代入文(IStatement statement, StringWriter writer)
		{
			var st = statement as ProcessAndAssignStatement;

			Generate(st.VariablePhrase, writer);    //変数
			writer.Write(" = ");
			Generate(st.ExprPhrase, writer);    //式
		}

		private void GSParseError(IStatement statement, StringWriter writer)
		{
			var st = statement as ParseErrorStatements;
			Generate(st.Statements, writer);
		}

		private void GS繰り返し文(IStatement statement, StringWriter writer)
		{
			var st = statement as 繰り返し文;
			if (st.ForLoopType == 繰り返し文.ForTypes.Count)
			{
				string expr = GenerateString(st.LoopCount);
				writer.Write(@"for (int i = 0;i <" + expr + "; i++) {");
			}
			else if (st.ForLoopType == 繰り返し文.ForTypes.While)
			{
				string expr = GenerateString(st.Condition);
				writer.Write(@"while(" + expr + ") {");
			}
			else if (st.ForLoopType == 繰り返し文.ForTypes.Infinity)
			{
				writer.Write(@"for (;;) {");
			}
			Generate(st.Statements, writer);
			writer.WriteLine(@"}");
		}

		private void GSもし文(IStatement statement, StringWriter writer)
		{
			var st = statement as もし文;

			//ifおよび各elseif
			var caseList = st.CaseList;
			for (int i = 0; i < caseList.Count; i++)
			{
				CaseIf ifcase = caseList[i];
				writer.Write("if(");
				Generate(ifcase.Condition, writer);
				writer.Write("){");

				Generate(ifcase.Statements, writer);
				writer.Write("}");
			}

			//elseがある場合
			if (st.ElseCase != null)
			{
				writer.Write(" else {");
				Generate(st.ElseCase.Statements, writer);
				writer.Write("}");
			}

			writer.WriteLine();
		}

		private void GS分岐文(IStatement statement, StringWriter writer)
		{
			var st = statement as 分岐文;

			GenerateStruct(st.Sentence.Phrases, writer);

			//各case:
			var codeList = st.CodeList;
			for (int i = 0; i < codeList.Count; i++)
			{
				ICodeElement element = codeList[i];
				if (element is SwitchCaseBase)
				{
					SwitchCaseBase switchCase = element as SwitchCaseBase;
					if (switchCase is SwitchCase) Generate((switchCase as SwitchCase).CasePhrase, writer);
				}
				else
					Generate(element, writer, null);
			}
		}

		/// <summary>代入</summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		private void GS代入文(IStatement statement, StringWriter writer)
		{
			var st = statement as 代入文;
			Generate(st.Left, writer);
			writer.Write(" = ");
			Generate(st.Right, writer);
		}

		/// <summary>～について</summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		private void GSTryCatch(IStatement statement, StringWriter writer)
		{
			var st = statement as TryCatchStatement;

			//例外監視

			//コード
			Generate(st.TryStatements, writer);

			//発生した場合
			Generate(st.CatchStatements, writer);
		}

		/// <summary>～について</summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		private void GSWith(IStatement statement, StringWriter writer)
		{
			var st = statement as WithStatement;

			Generate(st.Phrases, writer);
			Generate(st.Statements, writer);
		}

		/// <summary>手順設定</summary>
		private void GSAssignEvent(IStatement statement, StringWriter writer)
		{
			var st = statement as AssignEventStatement;
			IPhrase[] phrases = st.Phrases;
			for (int i = 0; i < phrases.Length; i++)
			{
				if (phrases[i] is KeywordToken)
					Generate(phrases[i], CodePartsType.Struct, writer);
				else
					Generate(phrases[i], writer);
			}
		}

		#endregion

		#region Phraseデリゲートハンドリング

		private void D文字列定数字句(IStatement statement, StringWriter writer)
		{
		}

		private void D文字列定数句(IStatement statement, StringWriter writer)
		{
			var st = statement as 文字列定数句;
		}

		private void D連結文字列句(IStatement statement, StringWriter writer)
		{
			var ph = statement as 連結文字列句;
			IPhrase[] phrases = ph.Phrases;
			for (int i = 0; i < phrases.Length; i++)
			{
				if (phrases[i] is 四角囲い句 || phrases[i] is アンパサンド字句) continue;
				Generate(phrases[i], writer);
			}
		}

		private void D配列句(IStatement statement, StringWriter writer)
		{
			var ph = statement as 配列句;
			IPhrase[] phrases = ph.Phrases;
			for (int i = 1; i < phrases.Length - 1; i++)
			{
				if (phrases[i].PhraseType == PhraseTypes.カンマ字句) continue;
				Generate(phrases[i], writer);
			}
		}

		private void D辞書定数句(IStatement statement, StringWriter writer)
		{
			var ph = statement as 辞書定数句;
			IPhrase[] phrases = ph.Phrases;
			for (int i = 0; i < phrases.Length; i++)
			{
				Generate(phrases[i], writer);
			}
		}

		private void D数式(IStatement statement, StringWriter writer)
		{
			var st = statement as 演算式句;
			Generate(st.Formula, writer);
		}

		private void DConditionPhrase(IStatement statement, StringWriter writer)
		{
			var st = statement as 条件式句;
			Generate(st.Formula, writer);
		}

		private void D変数字句(IStatement statement, StringWriter writer)
		{
			var st = statement as 変数字句;
			string name;
			if (varNameList.TryGetValue(st.Variable, out name))
				writer.Write(name);
			else
				writer.Write(statement.Text);
		}

		private void Dレシーバ呼出句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as レシーバ呼出句;
			//レシーバ
			Generate(ph.Receiver, writer);

			//文
			Generate(ph.Prototype, writer);
		}

		private void Dブロック句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as ブロック句;
			Generate(ph.Statements, writer);
		}

		/// <summary>
		/// 整数定数
		/// </summary>
		private void D整数字句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 数値字句<int>;
			writer.Write(ph.NumberValue.ToString());
		}
		private void D長整数字句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 数値字句<long>;
			writer.Write(ph.NumberValue.ToString());
		}
		private void D浮動小数字句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 数値字句<float>;
			writer.Write(ph.NumberValue.ToString());
		}
		private void D真偽値定数(IStatement phrase, StringWriter writer)
		{
			if (phrase is 真値字句)
				writer.Write("True");
			else
				writer.Write("False");
		}
		private void Dバツ字句(IStatement phrase, StringWriter writer)
		{
			writer.Write("False");
		}

		private void D無(IStatement phrase, StringWriter writer)
		{
			writer.Write("null");
		}
		private void Dカンマ列挙句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as カンマ列挙句;
			foreach (IPhrase item in ph.Phrases)
			{
				Generate(item, writer);
			}
		}
		private void D演算子字句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 演算子字句;
			switch (ph.PhraseType)
			{
				case PhraseTypes.除算演算子字句:
					writer.Write("/");
					break;
				case PhraseTypes.乗算演算子字句:
					writer.Write("*");
					break;
				case PhraseTypes.加算演算子字句:
					writer.Write("+");
					break;
				case PhraseTypes.減算演算子字句:
					writer.Write("-");
					break;
				case PhraseTypes.PowerRemainderOperatorToken:
					writer.Write(phrase.Text);
					break;
			}
		}
		private void D列挙体名字句(IStatement phrase, StringWriter writer)
		{
			var token = phrase as 列挙体名字句;
			Type type = token.EnumType;
			var field = type.GetField(phrase.Text);
			var attrs = field.GetCustomAttributes(typeof(翻訳Attribute), false);
			if (attrs.Length > 0)
			{
				var attr = attrs[0] as 翻訳Attribute;
				writer.Write(attr.Code);
			}
			else
			{
				writer.Write(phrase.Text);
			}
		}
		private void D定数値名字句(IStatement phrase, StringWriter writer)
		{
			writer.Write(phrase.Text);
		}

		private void DIndexerAccessPhrase(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as IndexerAccessPhrase;
			Generate(ph.FunctionName, writer);

			Generate(ph.ParameterPhrase, writer);
		}

		private void D設定項目アクセス句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 設定項目アクセス句;
			var phrases = ph.Phrases;
			Generate(ph.ReceiverPhrase, writer);
		}

		private void Dフィールドアクセス句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as フィールドアクセス句;
			var phrases = ph.Phrases;

			Generate(ph.ReceiverPhrase, writer);
		}

		private void DObjectAccessPhrase(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as ObjectAccessPhrase;
			var phrases = ph.Phrases;
			for (int i = 0; i < phrases.Length; i++)
			{
				Generate(phrases[i], writer);
			}
		}

		private void D静的変数式(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 静的変数式;
		}

		private void D空白字句(IStatement phrase, StringWriter writer)
		{
			string remark = phrase.Text.Replace("　", " ");
			writer.Write(remark);
		}

		private void DNotCondition(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 否定条件句;
			Generate(ph.式句, writer);
		}
		private void DConditionTypeOfPhrase(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as TypeOfCondition;
			Generate(ph.LeftPhrase, writer);

			Generate(ph.RightPhrase, writer);
		}
		private void D日本語条件句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 日本語条件句;
			Generate(ph.左辺, writer);
			WriteConditionOper(writer, CompareTypes.Equal);
			Generate(ph.右辺, writer);
		}
		private void D末尾付き日本語条件句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 末尾付き日本語条件句;
			Generate(ph.左辺, writer);
			WriteConditionOper(writer, ph.末尾.CompareType);
			Generate(ph.右辺, writer);
		}
		private void D論理字句(IStatement phrase, StringWriter writer)
		{
			switch (phrase.PhraseType)
			{
				case PhraseTypes.論理積字句:
					writer.Write("&&");
					break;
				case PhraseTypes.論理和字句:
					writer.Write("||");
					break;
			}
		}
		private void D等価比較演算子字句(IStatement phrase, StringWriter writer)
		{
			writer.Write("==");
		}
		private void D比較演算子字句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 比較演算子字句;
			WriteConditionOper(writer, ph.CompareType);
		}

		private static void WriteConditionOper(StringWriter writer, CompareTypes compareType)
		{
			switch (compareType)
			{
				case CompareTypes.Equal:
					writer.Write("==");
					break;
				case CompareTypes.Not:
					writer.Write("!");
					break;
				case CompareTypes.NotEqual:
					writer.Write("!=");
					break;
				case CompareTypes.LargerThan:
					writer.Write(">");
					break;
				case CompareTypes.LargerEqual:
					writer.Write(">=");
					break;
				case CompareTypes.SmallerThan:
					writer.Write("<");
					break;
				case CompareTypes.SmallEqual:
					writer.Write("<=");
					break;
				case CompareTypes.And:
					writer.Write("&&");
					break;
				case CompareTypes.Or:
					writer.Write("||");
					break;
			}
		}

		private void D囲い句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 囲い句;
			string a = "", b = "";
			if (ph is 丸囲い句) { a = "("; b = ")"; }
			writer.Write(a);
			Generate(ph.中身, writer);
			writer.Write(b);
		}
		private void D未知語字句(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as 未知語字句;
			Generate(ph.ActualPhrase, writer);
		}

		private void DKeywordToken(IStatement phrase, StringWriter writer)
		{
			var ph = phrase as KeywordToken;
		}

		private void DParseErrorPhrase(IStatement phrase, StringWriter writer)
		{
			var phrases = (phrase as ParseErrorPhrase).Phrases;
			for (int i = 0; i < phrases.Length; i++)
			{
				IPhrase innerPhrase = phrases[i];
				if (innerPhrase is IPhraseContainer)
				{
					Generate(innerPhrase, writer);
				}
			}
		}
		private void DIncompleteStatement(IStatement phrase, StringWriter writer)
		{
			var phrases = (phrase as IncompleteStatement).Phrases;
			for (int i = 0; i < phrases.Length; i++)
			{
				Generate(phrases[i], writer);
			}
		}

		#endregion

	}
}
