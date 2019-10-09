using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextGen
{
    internal static class Program
    {
        private const string Filename = "tolstoy.txt";

        private static readonly Regex TokenRegex = new Regex("(?<word>[а-яА-Я0-9-]+|[.,:;?!]+)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static void Main()
        {
            Console.WriteLine("How many phrases do you want?");
            if (int.TryParse(Console.ReadLine(), out var n))
            {
                Console.WriteLine(GenerateText(n));
            }
            else
            {
                Console.WriteLine("That is not a number, you fool!");
                Console.Read();
            }
            //эх, спрашивать бы юзера несколько раз в цикле, да хелп и менюшку написать, вот жизнь была бы!
        }

        private static string GenerateText(int phrasesAmount)
        {
            var model = File.ReadLines(Filename)
                            .SelectMany(GetTokens)
                            .GetTrigrams()
                            .CountProbabilities();
            //вот эту всю модельку как бы взять, да и сохранить как джсонину большую, да не считать ее каждый раз, эх жизнью зажили бы!

            var random = new Random(42);

            return string.Join("\n",
                Enumerable.Range(0, phrasesAmount)
                                .Select(_ => model.Generate(random)));
        }

        private static IEnumerable<string> GetTokens(string line)
            => TokenRegex.Matches(line).Select(m => m.Groups["word"].Value);
    }
}