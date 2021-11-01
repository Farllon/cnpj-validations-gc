using System;
using System.Diagnostics;

namespace cnpj_validator
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

        public static bool IsValid(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            int[] multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            if (cnpj.Equals("00000000000000") ||
                cnpj.Equals("11111111111111") ||
                cnpj.Equals("22222222222222") ||
                cnpj.Equals("33333333333333") ||
                cnpj.Equals("44444444444444") ||
                cnpj.Equals("55555555555555") ||
                cnpj.Equals("66666666666666") ||
                cnpj.Equals("77777777777777") ||
                cnpj.Equals("88888888888888") ||
                cnpj.Equals("99999999999999"))
                return false;

            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }
}
