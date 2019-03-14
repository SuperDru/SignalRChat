using System.Text;
using System;

namespace Qoden.Util
{
	public static class QodenFormat
	{
	    struct FormatParser
		{
			int _idx;
		    readonly StringBuilder _result;
		    readonly StringBuilder _tmp;
			Record _context;
			readonly string _format;
			FormatException _error;

			public FormatParser (string format, Record context)
			{
				_context = context;
				_format = format;
				_idx = 0;
				_result = new StringBuilder();
				_tmp = new StringBuilder();
				_error = null;
			}

			public bool Parse (bool throwError)
			{
				while (!IsEof ()) {
					
					if (LookingAt ('{')) {						
						if (LookingAt ('{', 1)) {
							//{{
							MatchOpenEscapeSeq ();
						} else {
							//{<ID>}
							MatchId ();
						}
					} else if (LookingAt ('}')) {						
						if (LookingAt ('}', 1)) {
							//}}
							MatchCloseEscapeSeq ();
						} else {
							//}
							_error = new FormatException("Unexpected } at " + _idx);
						}
					} else {
						_result.Append (ConsumeChar ());
					}

					if (_error != null) {
						if (throwError) {
							throw _error;
						} else {
							return false;
						}
					} 
				}
				return true;
			}

			public string Result => _result.ToString ();

		    bool IsEof (int lookahead = 0)
			{
				return _idx + lookahead >= _format.Length;
			}

			bool LookingAt (char c, int lookahead = 0)
			{
				return !IsEof(lookahead) && _format [_idx + lookahead] == c;
			}

			void MatchOpenEscapeSeq()
			{
				_result.Append ('{');
				_idx += 2;
			}

			void MatchCloseEscapeSeq()
			{
				_result.Append ('}');
				_idx += 2;
			}

			void MatchId()
			{
				_tmp.Clear ();
				if (!Consume ('{'))
					return;
				while (!LookingAt ('}')) {
					if (IsEof ())
						break;
					_tmp.Append (ConsumeChar ());
				}
				if (!Consume ('}'))
					return;

				object value;
				if (_context.TryGetValue (_tmp.ToString (), out value)) {
					_result.Append (value);
				}
			}

			bool Consume(char c) 
			{
				if (IsEof ()) {
					_error = new FormatException ("Unexected end of format string.");
				} else {
					if (c == _format [_idx]) {
						_idx++;
					} else {
						_error = new FormatException (string.Format ("Unexpected '{0}' at {1}. Expected '{2}'.", _format [_idx], _idx, c));
					}
				}
				return _error == null;
			}

			char ConsumeChar()
			{
				var c = _format [_idx];
				_idx++;
				return c;
			}
		}

		public static string FormatWithObject (this string format, object obj)
		{
			var parser = new FormatParser (format, new Record (obj));
			parser.Parse (true);
			return parser.Result;				
		}

		public static bool TryFormatWithObject (this string format, object obj, out string result)
		{
			var parser = new FormatParser (format, new Record (obj));
			if (parser.Parse (false)) {
				result = parser.Result;	
				return true;
			} else {
				result = null;
				return false;
			}
		}
	}
}