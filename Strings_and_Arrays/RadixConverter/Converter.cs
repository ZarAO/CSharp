/*
	Конвертер из любой системы счисления в любую в пределах 36-ричной системы на C#
	Автор: Амельченя Андрей
*/
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RadixConverter
{
    public static class Converter
    {
        /// <summary>
        /// Переводит символ в числовое представление в зависимости от системы счисления
        /// </summary>
        /// <param name="c">Символ для перевода</param>
        /// <param name="radixOriginal">Система счисления оригинального числа</param>
        /// <returns></returns>
        public static int CharToInt(char c, int radixOriginal = 10)
        {
            if (c >= '0' && c <= '9' && (c - '0') < radixOriginal)
            {
                return c - '0';
            }
            if (c >= 'A' && c <= 'Z' && (c - 'A') < radixOriginal)
            {
                return c - 'A' + 10;
            }
            return -1;
        }
        /// <summary>
        /// Переводит числовое представление в символьное в зависимости от системы счисления
        /// </summary>
        /// <param name="c">Числовое представление</param>
        /// <returns></returns>
        public static char IntToChar(int c)
        {

            if (c >= 0 && c <= 9)
            {
                return (char)(c + '0');
            }
            return (char)(c + 'A' - 10);
        }
        /// <summary>
        /// Получает следующее число в новой системе счисления
        /// </summary>
        /// <param name="a">Массив символов оригинального числа</param>
        /// <param name="radixTo">Целевая система счисления</param>
        /// <param name="radixOriginal">Оригинальная система счисления</param>
        /// <returns></returns>
        private static char NextNumber(ref char[] a, int radixTo, int radixOriginal = 10)
        {
            var temp = 0;
            for (int i = 0; i < a.Count(); i++)
            {
                temp = temp * radixOriginal + CharToInt(a[i]);
                a[i] = IntToChar(temp / radixTo);
                temp %= radixTo;
            }
            return IntToChar(temp);
        }
        /// <summary>
        /// Проверка массива символов на "пустоту"
        /// </summary>
        /// <param name="a">Массив символов</param>
        /// <returns></returns>
        private static bool ArrayIsNull(ref char[] a)
        {
            for (int i = 0; i < a.Count(); i++)
            {
                if (a[i] > '0')
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Переводит число из одной системы счисления в другую
        /// </summary>
        /// <param name="original">Оригинальное число в строковом представлении</param>
        /// <param name="radixTo">Целевая система счисления</param>
        /// <param name="radixOriginal">Оригинальная система счисления</param>
        /// <returns></returns>
        public static String ConvertTo(string original, int radixTo, int radixOriginal = 10)
        {
            var a = original.ToUpper().ToArray();
            var str = new StringBuilder();

            do
            {
                var next = NextNumber(ref a, radixTo, radixOriginal);
                str.Append(next);
            } while (!ArrayIsNull(ref a));
            //Число в новой системе счисления записано в обратном порядке
            var sb = new StringBuilder();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }

        public static String ConvertTo(long original, int radixTo, int radixOriginal = 10)
        {
            return ConvertTo(original.ToString(CultureInfo.InvariantCulture), radixTo, radixOriginal);
        }
    }
}
