using System;
using System.Collections.Generic;
using System.Linq;

namespace TextGen
{
    public static class TextGen
    {
        private const string Start = "#";
        private static readonly string[] PhraseEnd = { ".", "?", "!" };
        private static readonly string[] Delimiters = { ".", ",", ";", ":" };

        public static IEnumerable<(string first, string second, string third)> GetTrigrams(this IEnumerable<string> tokens)
        {
            var (first, second) = (Start, Start);
            using var enumerator = tokens.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var third = enumerator.Current;
                yield return (first, second, third);
                if (PhraseEnd.Contains(third))
                {
                    yield return (second, third, Start);
                    yield return (third, Start, Start);
                    (first, second) = (Start, Start);
                }
                else
                {
                    (first, second) = (second, third);
                }
            }
        }

        public static string Generate(this Dictionary<(string first, string second), (string word, double frequency)[]> model, Random random)
        {
            IEnumerable<string> GenerateWords()
            {
                var (first, second) = (Start, Start);
                while (true)
                {
                    var randomWord = model[(first, second)].GetRandomWord(random);
                    (first, second) = (second, randomWord);
                    if (second == Start)
                        yield break;
                    if (PhraseEnd.Contains(second) || Delimiters.Contains(second) || first == Start)
                    {
                        yield return second;
                    }
                    else
                    {
                        yield return " ";
                        yield return second;
                    }
                }
            }

            return string.Concat(GenerateWords());
        }

        private static string GetRandomWord(this IList<(string word, double frequency)> words, Random random)
        {
            var sum = words.Sum(t => t.frequency);
            var index = random.NextDouble() * sum;
            var fsum = 0d;
            foreach (var (word, frequency) in words)
            {
                fsum += frequency;
                if (index < fsum)
                    return word;
            }

            return words[0].word;
        }

        public static Dictionary<(string first, string second), (string word, double frequency)[]> CountProbabilities(
            this IEnumerable<(string first, string second, string third)> trigrams)
        {
            trigrams = trigrams.ToArray();
            var bigramCounts = trigrams.CountItems(trigram => (trigram.first, trigram.second));
            var trigramCounts = trigrams.CountItems(Id);

            return trigramCounts
                .Select(kv => (trigram: kv.Key, bigram: (kv.Key.first, kv.Key.second), frequency: kv.Value))
                .Select(t => (t.trigram, t.bigram, probability: t.frequency * 1d / bigramCounts[t.bigram]))
                .ToLookup(t => t.bigram, t => (t.trigram.third, t.probability))
                .ToDictionary(t => t.Key, g => g.ToArray());
        }

        private static T Id<T>(T t)
        {
            return t;
        }
    }
}