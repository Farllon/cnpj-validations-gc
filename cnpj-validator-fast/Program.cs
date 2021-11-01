using System;
using System.Diagnostics;

namespace cnpj_validator_fast
{
    class Program
    {
        static void Main(string[] args)
        {
            var before0 = GC.CollectionCount(0);
            var before1 = GC.CollectionCount(1);
            var before2 = GC.CollectionCount(2);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1_000_000; i++)
            {
                if (!IsValid("22.006.951/0001-00"))
                    throw new Exception("Error!");

                if (IsValid("22.006.951/0001-01"))
                    throw new Exception("Error!");
            }

            sw.Stop();

            System.Console.WriteLine($"\nTime .: {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"# Gen0: {GC.CollectionCount(0) - before0}");
            System.Console.WriteLine($"# Gen1: {GC.CollectionCount(1) - before1}");
            System.Console.WriteLine($"# Gen2: {GC.CollectionCount(2) - before2}");
            System.Console.WriteLine($"Memory: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024 } mb");
        }

        private static int[] MULTIPLICADOR1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        private static int[] MULTIPLICADOR2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};

        public static bool IsValid(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            Span<int> tempCnpj = stackalloc int[14];
            int pos = 0;
            bool todosIguais = true;

            for (int i = 0; i < cnpj.Length; i++)
            {
                if (char.IsDigit(cnpj[i]))
                {
                    if (pos > 13)
                        return false;

                    tempCnpj[pos] = (cnpj[i] - '0');

                    if (todosIguais && (pos > 0))
                        todosIguais = tempCnpj[pos] == tempCnpj[pos - 1];
                    
                    pos++;
                }
            }

            if (pos != 14 || todosIguais)
                return false;

            int soma1 = 0;
            int soma2 = 0;

            for (int i = 0; i < 12; i++)
            {
                soma1 += tempCnpj[i] * MULTIPLICADOR1[i];
                soma2 += tempCnpj[i] * MULTIPLICADOR2[i];
            }

            int resto = (soma1 % 11);

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            int digito1 = resto;

            if (tempCnpj[12] != digito1)
                return false;

            soma2 += digito1 * MULTIPLICADOR2[12];

            resto = (soma2 % 11);

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            return tempCnpj[13] == resto;
        }
    }
}
