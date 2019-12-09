using System;
using System.Collections.Generic;
using System.IO;

namespace LexicalAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\Учеба\2 курс\1 семестр\Игнатьев\Mine\Git\LexicalAnalyzer\Sample.txt";
            string result = ReadFile(path);
            SetUnits(result, _units);
            //SetTable(result);


            //foreach (Token l in lexems)
            //{
            //    Console.WriteLine("Value = " + l.Value + ", Type = " + l.Type);
            //}
            //Console.WriteLine(result);

            foreach (var t in _units)
            {
                Console.WriteLine(t.Value);
                Console.WriteLine("---");
            }

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine(result);

            Console.ReadKey();
        }

        class Unit
        {
            List<Unit> _units2 = new List<Unit>();
            List<string> tokens = new List<string>();

            public Unit(string line)
            {
                Value = line;
                char c;
                for (int i = 0; i < line.Length; i++)
                {
                    c = line[i];
                    if (c == '{')
                    {
                        int l = line.Length;

                        if (line[l - 1] == '}')
                        {
                            line = line.Substring(i + 1, l - 1 - i - 1);
                            SetUnits(line, _units2);
                        }
                        else
                        {
                            throw new Exception("Со скобками беда");
                        }
                    }
                }
                Units = _units2;

                if (_units2.Count == 0)
                {
                    Simple = true;

                    if (Value[Value.Length - 1] != ';')
                    {
                        throw new Exception("Где ; в конце строки???");
                    }

                    SetTable(Value, tokens);
                }
                else
                {
                    Simple = false;
                    GetBrTokens(Value);
                }
            }

            private void GetBrTokens(string line)
            {
                char c;
                string tmp = "";
                line = line.Replace(" ", "");
                line = line.Replace("\r\n", "");
                int i = 0;

                //Get keyword
                for (; i < line.Length; i++)
                {
                    c = line[i];

                    if (c == '(')
                    {
                        if (!string.IsNullOrEmpty(tmp) && BracesOperators.Contains(tmp))
                        {
                            tokens.Add(tmp);
                            tmp = "";
                            break;
                        }
                        else
                        {
                            throw new Exception("Неправильно!");
                        }
                    }

                    tmp += c;
                }

                line = line.Substring(i + 1);

                List<string> lexs = new List<string>();

                //Get comdition
                for (int j = 0; j < line.Length; j++)
                {
                    c = line[j];
                    tmp += c;

                    if (c == ')')
                    {
                        SetTable(tmp, lexs);
                        break;
                    }
                }

                foreach(var s in lexs)
                {
                    tokens.Add(s);
                }

            }

            public List<Unit> Units { get; set; }

            public string Value { get; set; }

            public bool Simple { get; set; }
        }

        static List<Unit> _units = new List<Unit>();

        static private string GetFirstLex(ref string line)
        {
            string tmp = "";

            foreach (var c in line)
            {
                if (separators.Contains(c))
                {
                    if (string.IsNullOrEmpty(tmp))
                    {
                        line = line.Substring(1);
                        continue;
                    }
                    return tmp;
                }

                tmp += c;
            }

            return tmp;
        }

        static List<string> BracesOperators = new List<string>()
        {
            "for",
            "if"
        };

        static private void SetUnits(string line, List<Unit> units)
        {
            int bracCount = 0;
            string tmp = "";

            for (int y = 0; line.Length > 0 || y < line.Length; y++)
            {
                string firstLex = GetFirstLex(ref line);

                bool start = false;

                if (BracesOperators.Contains(firstLex))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        char c = line[i];

                        if (c == ' ' || c == '\r' || c == '\n')
                        {
                            if (start && c == ' ')
                            {
                                tmp += c;
                            }

                            continue;
                        }

                        if (c == '{')
                        {
                            bracCount++;
                            start = true;
                        }

                        if (c == '}')
                        {
                            bracCount--;
                        }

                        tmp += c;

                        if (bracCount < 0)
                        {
                            throw new Exception("Лишние фигурные скобки");
                        }

                        if (bracCount == 0 && start)
                        {
                            units.Add(new Unit(tmp));
                            line = line.Substring(i + 1);
                            tmp = "";
                            break;
                        }


                    }

                    continue;
                }

                if (Int32.TryParse(firstLex, out _))
                {
                    throw new Exception("Число не может быть первым");
                }

                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];

                    if (c == '{' || c == '}')
                    {
                        throw new Exception("Скобки вообще тут неуместны");
                    }


                    if (c == ';')
                    {
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            tmp += c;
                            units.Add(new Unit(tmp));
                            tmp = "";
                        }

                        line = line.Substring(i + 1);
                        break;
                    }

                    tmp += c;
                }
            }

            if (bracCount != 0)
            {
                throw new Exception("Лишние скобки");
            }

            if (!string.IsNullOrEmpty(tmp))
            {
                throw new Exception("tmp не пуст, что-то не так");
            }
        }

        static string ReadFile(string path)
        {
            using StreamReader sr = new StreamReader(path);
            return sr.ReadToEnd();
        }

        static void SetTable(string line, List<string> lexems)
        {
            string tmp = "";
            string logicTmp = "";

            foreach (var c in line)
            {
                if (separators.Contains(c))
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        lexems.Add(tmp);
                        tmp = "";
                    }

                    if (!string.IsNullOrEmpty(logicTmp))
                    {
                        lexems.Add(logicTmp);
                        logicTmp = "";
                    }

                    continue;
                }

                if (operations.Contains(c))
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        lexems.Add(tmp);
                        tmp = "";
                    }

                    logicTmp = "";
                }

                if (Logic.Contains(c))
                {
                    logicTmp += c;
                    continue;
                }

                if (LogicalOperations.Contains(logicTmp) || logicTmp == "=")
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        lexems.Add(tmp);
                        lexems.Add(logicTmp);
                        logicTmp = "";
                        tmp = "";
                    }
                    else
                    {
                        throw new Exception("Косяк в логике");
                    }
                }

                tmp += c;
            }

            if (!string.IsNullOrEmpty(tmp))
            {
                throw new Exception("Что-то не так в условии, либо не хватает ;");
            }
        }


        static List<Token> _lexems = new List<Token>();


        static List<char> separators = new List<char>()
        {
            ' ',
            '(',
            ')',
            ';',
            '\r',
            '\n'
        };

        static List<char> operations = new List<char>()
        {
            '+',
            '-',
            '/',
            '*'
        };



        static List<string> LogicalOperations = new List<string>()
        {
            "<",
            ">",
            "<=",
            ">=",
            "!=",
            "==",
        };

        static List<char> Logic = new List<char>()
        {
            '<',
            '>',
            '<',
            '>',
            '!',
            '=',
        };


        static List<string> keyWords = new List<string>()
        {
            "int",
            "for",
            "if"
        };
    }

    enum LexType
    {
        KeyWord,
        Identifier,
        Operation,
        AssignmentSymbol,
        Constant
    }

    class Token
    {
        static List<string> keyWords = new List<string>()
        {
            "int"
        };

        static List<string> operations = new List<string>()
        {
            "+",
            "-",
            "/",
            "*"
        };

        public Token(string value)
        {
            Value = value;
            Type = GetType(value);
        }

        public string Value { get; set; }
        public LexType Type { get; set; }

        private LexType GetType(string lex)
        {
            if (keyWords.Contains(lex))
            {
                return LexType.KeyWord;
            }

            if (lex == "=")
            {
                return LexType.AssignmentSymbol;
            }

            if (operations.Contains(lex))
            {
                return LexType.AssignmentSymbol;
            }

            if (Int32.TryParse(lex, out _))
            {
                return LexType.Constant;
            }

            return LexType.Identifier;
        }
    }
}
