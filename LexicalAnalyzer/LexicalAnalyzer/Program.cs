using System;
using System.Collections.Generic;
using System.IO;

namespace LexicalAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\Учеба\2 курс\1 семестр\Игнатьев\Mine\Sample.txt";
            string result = ReadFile(path);
            SetUnits(result);
            //SetTable(result);


            //foreach (Token l in lexems)
            //{
            //    Console.WriteLine("Value = " + l.Value + ", Type = " + l.Type);
            //}
            //Console.WriteLine(result);

            foreach (var t in units)
            {
                Console.WriteLine(t);
                Console.WriteLine("---");
            }

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine(result);

            Console.ReadKey();
        }

        class Unit
        {
            public Unit()
            {

            }

            List<Unit> Units { get; set; }

            string Value { get; set; }
        }

        static List<string> units = new List<string>();

        static private string GetFirstLex(ref string line)
        {
            string tmp = "";

            foreach (var c in line)
            {
                if (separators.Contains(c))
                {
                    if(string.IsNullOrEmpty(tmp))
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
            "if",
            "readln",
            "writeln"
        };

        static private void SetUnits(string line)
        {
            int bracCount = 0;

            for (; line.Length > 0;)
            {
                string tmp = "";
                string firstLex = GetFirstLex(ref line);
                
                bool start = false;

                if (BracesOperators.Contains(firstLex))
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        char c = line[i];

                        if (c == ' ' || c == '\r' || c == '\n')
                        {
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

                        if(bracCount < 0)
                        {
                            throw new Exception("Лишние фигурные скобки");
                        }

                        if (bracCount == 0 && start)
                        {
                            units.Add(tmp);
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

                    if(c == '{' || c == '}')
                    {
                        throw new Exception("Скобки вообще тут неуместны");
                    }


                    if (c == ';')
                    {
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            units.Add(tmp);
                            tmp = "";
                        }

                        line = line.Substring(i + 1);
                        break;
                    }

                    tmp += c;
                }
            }

            if(bracCount != 0)
            {
                throw new Exception("Лишние скобки");
            }
        }

        static string ReadFile(string path)
        {
            using StreamReader sr = new StreamReader(path);
            return sr.ReadToEnd();
        }

        static void SetTable(string line)
        {
            string tmp = "";

            foreach (var c in line)
            {
                if (separators.Contains(c))
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        lexems.Add(new Token(tmp));
                        tmp = "";
                    }
                    continue;
                }

                if (operations.Contains(c))
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        lexems.Add(new Token(tmp));
                        tmp = "";
                    }
                }

                tmp += c;
            }
        }


        static List<Token> lexems = new List<Token>();


        static List<char> separators = new List<char>()
        {
            ' ',
            '(',
            
            //';',
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
