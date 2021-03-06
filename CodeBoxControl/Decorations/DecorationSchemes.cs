﻿using System.Collections.Generic;
using System.Windows.Media;

namespace CodeBoxControl.Decorations
{
	public static class DecorationSchemes
	{
		#region C#

		public static DecorationScheme CSharp3
		{
			get
			{
				DecorationScheme ds = new DecorationScheme();

				ds.Name = "C#";
				MultiRegexWordDecoration BlueWords = new MultiRegexWordDecoration();
				BlueWords.Brush = new SolidColorBrush(Colors.Blue);
				BlueWords.Words = CSharpReservedWords();
				ds.BaseDecorations.Add(BlueWords);

				MultiRegexWordDecoration BlueClasses = new MultiRegexWordDecoration();
				BlueClasses.Brush = new SolidColorBrush(Colors.Blue);
				BlueClasses.Words = CSharpVariableReservations();
				ds.BaseDecorations.Add(BlueClasses);

				MultiStringDecoration regions = new MultiStringDecoration();
				regions.Brush = new SolidColorBrush(Colors.Blue);
				regions.Strings.AddRange(CSharpRegions());
				ds.BaseDecorations.Add(regions);

				RegexDecoration quotedText = new RegexDecoration();
				quotedText.Brush = new SolidColorBrush(Colors.Brown);
				quotedText.RegexString = "(?s:\".*?\")";
				ds.BaseDecorations.Add(quotedText);

				//Color single line comments green
				RegexDecoration singleLineComment = new RegexDecoration();
				singleLineComment.DecorationType = EDecorationType.TextColor;
				singleLineComment.Brush = new SolidColorBrush(Colors.Green);
				singleLineComment.RegexString = "//.*";
				ds.BaseDecorations.Add(singleLineComment);

				//Color multiline comments green
				RegexDecoration multiLineComment = new RegexDecoration();
				multiLineComment.DecorationType = EDecorationType.TextColor;
				multiLineComment.Brush = new SolidColorBrush(Colors.Green);
				multiLineComment.RegexString = @"(?s:/\*.*?\*/)";
				ds.BaseDecorations.Add(multiLineComment);

				return ds;
			}
		}

		private static List<string> CSharpReservedWords()
		{
			return new List<string>() { "using" ,"namespace" , "static", "class" ,"public" ,"get" , "private" , "return" ,"partial" , "new"
		  ,"set" , "value"  };
		}

		private static List<string> CSharpVariableReservations()
		{
			return new List<string>() { "string", "int", "double", "long" };
		}

		private static List<string> CSharpRegions()
		{
			return new List<string>() { "#region", "#endregion" };
		}

		#endregion

		#region SQL Server

		public static DecorationScheme SQLServer2008
		{
			get
			{
				DecorationScheme ds = new DecorationScheme();
				ds.Name = "SQL Server";

				// Color Built in functions Magenta
				MultiRegexWordDecoration builtInFunctions = new MultiRegexWordDecoration();
				builtInFunctions.Brush = new SolidColorBrush(Colors.Magenta);
				builtInFunctions.Words.AddRange(GetBuiltInFunctions());
				ds.BaseDecorations.Add(builtInFunctions);

				//Color global variables Magenta
				MultiStringDecoration globals = new MultiStringDecoration();
				globals.Brush = new SolidColorBrush(Colors.Magenta);
				globals.Strings.AddRange(GetGlobalVariables());
				ds.BaseDecorations.Add(globals);

				//Color most reserved words blue
				MultiRegexWordDecoration bluekeyWords = new MultiRegexWordDecoration();
				bluekeyWords.Brush = new SolidColorBrush(Colors.Blue);
				bluekeyWords.Words.AddRange(GetBlueKeyWords());
				ds.BaseDecorations.Add(bluekeyWords);

				MultiRegexWordDecoration grayKeyWords = new MultiRegexWordDecoration();
				grayKeyWords.Brush = new SolidColorBrush(Colors.Gray);
				grayKeyWords.Words.AddRange(GetGrayKeyWords());
				ds.BaseDecorations.Add(grayKeyWords);

				MultiRegexWordDecoration dataTypes = new MultiRegexWordDecoration();
				dataTypes.Brush = new SolidColorBrush(Colors.Blue);
				dataTypes.Words.AddRange(GetDataTypes());
				ds.BaseDecorations.Add(dataTypes);


				MultiRegexWordDecoration systemViews = new MultiRegexWordDecoration();
				systemViews.Brush = new SolidColorBrush(Colors.Green);
				systemViews.Words.AddRange(GetSystemViews());
				ds.BaseDecorations.Add(systemViews);

				MultiStringDecoration operators = new MultiStringDecoration();
				operators.Brush = new SolidColorBrush(Colors.Gray);
				operators.Strings.AddRange(GetOperators());
				ds.BaseDecorations.Add(operators);


				RegexDecoration quotedText = new RegexDecoration();
				quotedText.Brush = new SolidColorBrush(Colors.Red);
				quotedText.RegexString = "'.*?'";
				ds.BaseDecorations.Add(quotedText);

				RegexDecoration nQuote = new RegexDecoration();
				//nQuote.DecorationType = EDecorationType.TextColor;
				nQuote.Brush = new SolidColorBrush(Colors.Red);
				nQuote.RegexString = "N''";
				ds.BaseDecorations.Add(nQuote);

				//Color single line comments green
				RegexDecoration singleLineComment = new RegexDecoration();
				singleLineComment.DecorationType = EDecorationType.TextColor;
				singleLineComment.Brush = new SolidColorBrush(Colors.Green);
				singleLineComment.RegexString = "--.*";
				ds.BaseDecorations.Add(singleLineComment);

				//Color multiline comments green
				RegexDecoration multiLineComment = new RegexDecoration();
				multiLineComment.DecorationType = EDecorationType.Strikethrough;
				multiLineComment.Brush = new SolidColorBrush(Colors.Green);
				multiLineComment.RegexString = @"(?s:/\*.*?\*/)";
				ds.BaseDecorations.Add(multiLineComment);
				return ds;
			}
		}

		static string[] GetBuiltInFunctions()
		{
			string[] funct = { "parsename", "db_name", "object_id", "count", "ColumnProperty", "LEN",
							 "CHARINDEX" ,"isnull" , "SUBSTRING" };
			return funct;

		}

		static string[] GetGlobalVariables()
		{

			string[] globals = { "@@fetch_status" };
			return globals;

		}

		static string[] GetDataTypes()
		{
			string[] dt = { "int", "sysname", "nvarchar", "char" };
			return dt;

		}

		static string[] GetBlueKeyWords() // List from 
		{
			string[] res = {"ADD","EXISTS","PRECISION","ALL","EXIT","PRIMARY","ALTER","EXTERNAL",
							"PRINT","FETCH","PROC","ANY","FILE","PROCEDURE","AS","FILLFACTOR",
							"PUBLIC","ASC","FOR","RAISERROR","AUTHORIZATION","FOREIGN","READ","BACKUP",
							"FREETEXT","READTEXT","BEGIN","FREETEXTTABLE","RECONFIGURE","BETWEEN","FROM",
							"REFERENCES","BREAK","FULL","REPLICATION","BROWSE","FUNCTION","RESTORE",
							"BULK","GOTO","RESTRICT","BY","GRANT","RETURN","CASCADE","GROUP","REVERT",
							"CASE","HAVING","REVOKE","CHECK","HOLDLOCK","RIGHT","CHECKPOINT","IDENTITY",
							"ROLLBACK","CLOSE","IDENTITY_INSERT","ROWCOUNT","CLUSTERED","IDENTITYCOL",
							"ROWGUIDCOL","COALESCE","IF","RULE","COLLATE","IN","SAVE","COLUMN","INDEX",
							"SCHEMA","COMMIT","INNER","SECURITYAUDIT","COMPUTE","INSERT","SELECT",
							"CONSTRAINT","INTERSECT","SESSION_USER","CONTAINS","INTO","SET","CONTAINSTABLE",
							"SETUSER","CONTINUE","JOIN","SHUTDOWN","CONVERT","KEY","SOME","CREATE",
							"KILL","STATISTICS","CROSS","LEFT","SYSTEM_USER","CURRENT","LIKE","TABLE",
							"CURRENT_DATE","LINENO","TABLESAMPLE","CURRENT_TIME","LOAD","TEXTSIZE",
							"CURRENT_TIMESTAMP","MERGE","THEN","CURRENT_USER","NATIONAL","TO","CURSOR",
							"NOCHECK","TOP","DATABASE","NONCLUSTERED","TRAN","DBCC","NOT","TRANSACTION",
							"DEALLOCATE","NULL","TRIGGER","DECLARE","NULLIF","TRUNCATE","DEFAULT","OF",
							"TSEQUAL","DELETE","OFF","UNION","DENY","OFFSETS","UNIQUE","DESC", "ON",
							"UNPIVOT","DISK","OPEN","UPDATE","DISTINCT","OPENDATASOURCE","UPDATETEXT",
							"DISTRIBUTED","OPENQUERY","USE","DOUBLE","OPENROWSET","USER","DROP","OPENXML",
							"VALUES","DUMP","OPTION","VARYING","ELSE","OR","VIEW","END","ORDER","WAITFOR",
							"ERRLVL","OUTER","WHEN","ESCAPE","OVER","WHERE","EXCEPT","PERCENT","WHILE",
							"EXEC","PIVOT","WITH","EXECUTE","PLAN","WRITETEXT", "GO", "ANSI_NULLS",
							"NOCOUNT", "QUOTED_IDENTIFIER", "master"};
			return res;
		}

		static string[] GetGrayKeyWords()
		{
			string[] res = { "AND", "Null", "IS" };

			return res;

		}

		static string[] GetOperators()
		{
			string[] ops = { "=", "+", ".", ",", "-", "(", ")", "*", "<", ">" };

			return ops;

		}

		static string[] GetSystemViews()
		{
			string[] views = { "syscomments", "sysobjects", "sys.syscomments" };
			return views;
		}

		#endregion
	
		#region Java

		public static DecorationScheme Java
		{
			get
			{
				DecorationScheme ds = new DecorationScheme();

				ds.Name = "Java";
				MultiRegexWordDecoration BlueWords = new MultiRegexWordDecoration
				{
					Brush = new SolidColorBrush(Colors.Blue),
					Words = JavaReservedWords(),
					IsCaseSensitive = true,
				};
				ds.BaseDecorations.Add(BlueWords);

				RegexDecoration quotedText = new RegexDecoration
				{
					Brush = new SolidColorBrush(Colors.Brown),
					RegexString = "\"(.*?)(?<![^\\\\]\\\\)\""
				};
				ds.BaseDecorations.Add(quotedText);
				
				RegexDecoration comment = new RegexDecoration
				{
					DecorationType = EDecorationType.TextColor,
					Brush = new SolidColorBrush(Colors.Green),
					RegexString = @"(?<!:)\/\/.*|\/\*(\s|.)*?\*\/"
				};
				ds.BaseDecorations.Add(comment);

				return ds;
			}
		}

		public static List<string> JavaReservedWords()
		{
			return new List<string>() { "abstract",	"assert",	"boolean",	"break",	"byte" ,
										"case",		"catch",	"char",		"class",	"const",
										"continue",	"default",  "do",		"double",	"else",
										"enum",		"extends",	"final",	"finally",	"float",
										"for",		"goto",		"if",		"implements","import",
										"instanceof","int",		"interface","long",		"native",
										"new",		"package",	"private",  "protected","public",
										"return",   "strictfp",	"short",    "static",	"super",
										"switch",   "synchronized","this",	"throw",	"throws",
										"transient","try",		"void",		"volatile",	"while",};
		}
		#endregion
	}
}
