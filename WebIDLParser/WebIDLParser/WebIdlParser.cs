using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MS.Internal
{
    /// <summary>WebIDL Parser</summary>
    public class WebIdlParser
    {
        // These shoudl all come from the config file.
        private const string IntegerRegexString = @"-?([1-9][0-9]*|0[Xx][0-9A-Fa-f]+|0[0-7]*)";
        private const string FloatRegexString = @"-?(([0-9]+\.[0-9]*|[0-9]*\.[0-9]+)([Ee][+-]?[0-9]+)?|[0-9]+[Ee][+-]?[0-9]+)";
        private const string IdentifierRegexString = @"_?[A-Za-z][0-9A-Z_a-z-]*";
        private const string StringRegexString = "\"[^ \"]*\"";
        private const string WhitespaceRegexString = @"[\t\n\r ]+";
        private const string CommentRegexString = @"\/\/.*|\/\*(.|\n)*?\*\/";
        private const string OtherRegexString = @"[^\t\n\r 0-9A-Za-z]";

        private static bool IsWhiteSpace(char? c)
        {
            // U+0009 = <control> HORIZONTAL TAB
            // U+000a = <control> LINE FEED
            // U+000d = <control> CARRIAGE RETURN
            return (c == ' ' || c == '\x0009' || c == '\x000a' || c == '\x000d');
        }

        public void NextNonWhiteSpace(StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                var pc = (char)sr.Peek();
                if (!IsWhiteSpace(pc))
                {
                    break;
                }
                var c = (char)sr.Read();
            }
        }


        public void ExtendedAttributeList(StreamReader sr)
        {
            if (sr.Read() == '[')
            {
                ExtendedAttribute(sr);
            }
            else
            {
                //invalid
            }
        }

        private void ExtendedAttribute(StreamReader sr)
        {

        }

        public void Parse()
        {
            using (var sr = File.OpenText(""))
            {
                NextNonWhiteSpace(sr);
                ExtendedAttributeList(sr);
            }
        }



        public void Tokenize(string inputFile)
        {
            var sb = new StringBuilder();
            var s = new Stack<char>();
            using (TextReader reader = File.OpenText(""))
            {
                var inComment = false;
                var inQuote = false;
                while (true)
                {
                    var c = (char)reader.Read();
                    var pc = (char)reader.Peek();
                    if (inQuote && c != '"')
                    {
                        // append
                        continue;
                    }
                    else if (inComment && pc != '*')
                    {
                        continue;
                    }
                    else if (IsWhiteSpace(c))
                    {
                        //reset
                        continue;
                    }
                    else if (c == '/')
                    {
                        inComment = (pc == '*');
                        if (inComment)
                        {
                            reader.Read();
                            continue;
                        }

                        if (pc == '/')
                        {
                            // single line comments, read remaining line
                            reader.ReadLine();
                        }
                        continue;
                    }
                    else if (inComment && (c == '*') && (pc == '/'))
                    {
                        inComment = false;
                        reader.Read();
                        continue;
                    }
                    else if (c == '(')
                    {
                        s.Push('(');
                    }
                    else if (c == ')')
                    {
                        if (s.Peek() == '(')
                        {
                            s.Pop();
                        }
                        else
                        {
                            //incorect nesting of '( )'
                        }
                    }
                    else if (c == '"')
                    {
                        if (inQuote && s.Peek() == '"')
                        {
                            s.Pop();
                            continue;
                        }
                        inQuote = true;
                    }
                    else if (c == '<')
                    {
                        s.Push('<');
                    }
                    else if (c == '>')
                    {
                        if (s.Peek() == '<')
                        {
                            s.Pop();
                        }
                        else
                        {
                            //incorect nesting of '< >'
                        }
                    }
                    else if (c == '[')
                    {
                        s.Push('[');
                    }
                    else if (c == ']')
                    {
                        if (s.Peek() == '[')
                        {
                            s.Pop();
                        }
                        else
                        {
                            //incorect nesting of '[ ]'
                        }
                    }
                    else if (c == '{')
                    {
                        s.Push('{');
                    }
                    else if (c == '}')
                    {
                        //pop paren off stack
                        if (s.Peek() == '{')
                        {
                            s.Pop();
                        }
                        else
                        {
                            //incorect nesting of '{ }'
                        }
                    }
                    else
                    {
                        //append
                    }
                }
            }
        }
    }
}