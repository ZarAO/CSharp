/*
 * Класс для работы с целыми числами большой длины
 * Амельченя Андрей
 * Не самая оптимальная реализация, но рабочая...
 * !!!В коде много костылей!!!
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BigDigit
{
    public class BigDigit
    {
        private String _value;
        private bool _signOfDefault;
        private bool _negative;

        /// <summary>
        /// Конструктор, принимает строковое представление числа
        /// </summary>
        /// <param name="value">Строковое представление числа</param>
        public BigDigit(String value)
        {
            SetNegative(value);
            Value = value;
        }

        /// <summary>
        /// Конструктор, принимает число типа long
        /// </summary>
        /// <param name="value"></param>
        public BigDigit(long value)
        {
            var strValue = value.ToString(CultureInfo.InvariantCulture);
            SetNegative(strValue);
            Value = strValue;
        }

        /// <summary>
        /// Значение числа в виде строки
        /// </summary>
        public String Value
        {
            get
            {
                if (String.IsNullOrEmpty(_value))
                {
                    return "0";
                }
                return _negative && _value != "0" ? "-" + _value : _value;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _value = "0";
                    _negative = false;
                }
                else
                {
                    if (value[0] == '-')
                    {
                        _negative = true;
                        _value = value.Remove(0, 1);
                    }
                    else
                    {
                        _value = value;
                        _negative = false;
                    }
                }
            }
        }

        /// <summary>
        ///     Сложение двух длинных чисел
        /// </summary>
        /// <param name="a">Первое число</param>
        /// <param name="b">Второе число</param>
        /// <returns></returns>
        public static BigDigit operator +(BigDigit a, BigDigit b)
        {
            if (a._negative && !b._negative)
            {
                a._negative = false;
                return b - a;
            }

            if (!a._negative && b._negative)
            {
                b._negative = false;
                return a - b;
            }

            var summand1 = ReverseString(a._value).ToArray();
            var summand2 = ReverseString(b._value).ToArray();

            var buffer = new StringBuilder();
            var next = 0;
            var maxLength = Math.Max(summand1.Count(), summand2.Count());
            var i = 0;
            do
            {
                if (summand1.Count() > i)
                {
                    next += (summand1[i] - '0');
                }
                if (summand2.Count() > i)
                {
                    next += (summand2[i] - '0');
                }

                buffer.Append((next % 10).ToString(CultureInfo.InvariantCulture));
                next /= 10;
                i++;
            } while (next != 0 || i < maxLength);
            var result = new BigDigit(ReverseString(buffer.ToString()));
            if (a._negative && b._negative)
            {
                result._negative = true;
            }
            CorrectNegative(ref a, ref b);
            return result;
        }

        /// <summary>
        /// Разность двух длинных чисел
        /// </summary>
        /// <param name="a">Уменьшаемое</param>
        /// <param name="b">Вычитаемое</param>
        /// <returns></returns>
        public static BigDigit operator -(BigDigit a, BigDigit b)
        {
            if (a._negative && b._negative)
            {
                b._negative = false;
                a._negative = false;
                return b - a;
            }
            if (b._negative && !a._negative)
            {
                b._negative = false;
                return a + b;
            }

            if (!b._negative && a._negative)
            {
                b._negative = true;
                return a + b;
            }


            var negative = false;

            char[] minuend;
            char[] subtrahend;

            if (b > a)
            {
                subtrahend = ReverseString(a._value).ToArray();
                minuend = ReverseString(b._value).ToArray();
                negative = true;
            }
            else
            {
                minuend = ReverseString(a._value).ToArray();
                subtrahend = ReverseString(b._value).ToArray();
            }
            var i = 0;
            int next = 0;
            var buffer = new StringBuilder();
            int maxLength = minuend.Count();
            var residue = 0;
            do
            {
                var d1 = -1;
                var d2 = -1;
                if (minuend.Count() > i)
                {
                    d1 = (minuend[i] - '0') - residue;
                    residue = 0;
                    if (d1 < 0)
                    {
                        d1 += 10;
                        residue = 1;
                    }
                }
                if (subtrahend.Count() > i)
                {
                    d2 = (subtrahend[i] - '0');
                }
                if (d1 != -1)
                {
                    next = d1;
                    if (d2 != -1)
                    {
                        if (d1 < d2)
                        {
                            next += 10;
                            residue = 1;
                        }
                        next -= d2;
                    }
                }
                buffer.Append((next % 10).ToString(CultureInfo.InvariantCulture));
                i++;
            } while (i < maxLength);
            var result = new BigDigit(CleanForZero(ReverseString(buffer.ToString())));
            if (negative)
            {
                result._negative = true;
            }
            CorrectNegative(ref a, ref b);
            return result;
        }

        /// <summary>
        /// Произведение двух длинных чисел
        /// </summary>
        /// <param name="a">Первый множитель</param>
        /// <param name="b">Второй множитель</param>
        /// <returns></returns>
        public static BigDigit operator *(BigDigit a, BigDigit b)
        {
            var multiplier1 = ReverseString(a._value).Select(x => x - '0').ToArray();
            var multiplier2 = ReverseString(b._value).Select(x => x - '0').ToArray();

            var buffer = new List<int> { 0 };
            var residue = 0;
            for (var i = 0; i < multiplier1.Count(); i++)
            {
                var cur = i;
                for (var j = 0; j < multiplier2.Count(); j++)
                {
                    var mul = multiplier1[i] * multiplier2[j] + residue;
                    if (buffer.Count <= cur)
                    {
                        buffer.Add(0);
                    }
                    var next = mul + buffer[cur];
                    buffer[cur] = next % 10;
                    residue = next / 10;
                    cur++;
                }
                if (residue == 0) continue;
                buffer.Add(residue);
                residue = 0;
            }


            var strResulr = new StringBuilder();
            for (int i = buffer.Count - 1; i >= 0; i--)
            {
                strResulr.Append(buffer[i]);
            }
            var result = new BigDigit(CleanForZero(strResulr.ToString()));
            if (a._negative != b._negative)
            {
                result._negative = true;
            }
            return result;
        }

        /// <summary>
        /// Частное двух больших чисел
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static BigDigit operator /(BigDigit a, BigDigit b)
        {
            var temp = new BigDigit("0");
            return Modulo(a, b, ref temp);
        }

        /// <summary>
        /// Остаток от деления
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static BigDigit operator %(BigDigit a, BigDigit b)
        {
            var mod = new BigDigit("0");
            Modulo(a, b, ref mod);
            return mod;
        }

        /// <summary>
        ///     Сложение длинного числа с обычным типа long
        /// </summary>
        /// <param name="a">Длинное число</param>
        /// <param name="b">Обычное число</param>
        /// <returns></returns>
        public static BigDigit operator +(BigDigit a, long b)
        {
            return a + new BigDigit(b.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Разность длинного числа с обычным типа long
        /// </summary>
        /// <param name="a">Длинное число - уменьшаемое</param>
        /// <param name="b">Обычное число - вычитаемое</param>
        /// <returns></returns>
        public static BigDigit operator -(BigDigit a, long b)
        {
            return a - new BigDigit(b.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Произведение длинного числа с обычным типа long
        /// </summary>
        /// <param name="a">Длинное число</param>
        /// <param name="b">Обычное число</param>
        /// <returns></returns>
        public static BigDigit operator *(BigDigit a, long b)
        {
            return a * new BigDigit(b.ToString(CultureInfo.InvariantCulture));
        }



        /// <summary>
        /// Проверяет условие, что первое число больше второго
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool IsLarger(BigDigit a, BigDigit b)
        {
            if (a._value.Length > b._value.Length)
            {
                return true;
            }
            if (a._value.Length == b._value.Length)
            {
                for (int i = 0; i < a._value.Length; i++)
                {
                    if (a._value[i] > b._value[i])
                    {
                        return true;
                    }

                    if (a._value[i] < b._value[i])
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет условие, что первое число >= второго
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool IsLargerOrGreater(BigDigit a, BigDigit b)
        {
            return a._value == b._value || IsLarger(a, b);
        }

        public static bool operator >(BigDigit a, BigDigit b)
        {
            if (a == b) return false;
            if (!a._negative && !b._negative)
            {
                return IsLarger(a, b);
            }

            if (a._negative && b._negative)
            {
                return !IsLarger(a, b);
            }
            if (a._negative != b._negative)
            {
                if (!a._negative)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator <(BigDigit a, BigDigit b)
        {
            if (a == b) return false;
            return !(a > b);
        }

        public static bool operator ==(BigDigit a, BigDigit b)
        {
            if (a._value == null || b._value == null) return false;
            return (a._value.Equals(b._value) && a._negative == b._negative);
        }

        public static bool operator !=(BigDigit a, BigDigit b)
        {
            return !(a == b);
        }

        public static bool operator >=(BigDigit a, BigDigit b)
        {
            return (a == b || a > b);
        }

        public static bool operator <=(BigDigit a, BigDigit b)
        {
            return (a == b || a < b);
        }

        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Записывает знак числа по умолчанию
        /// </summary>
        /// <param name="value"></param>
        private void SetNegative(String value)
        {
            if (value.Length > 0 && value[0] == '-')
            {
                _signOfDefault = true;
            }
            else
            {
                _signOfDefault = false;
            }
        }

        /// <summary>
        ///     Переворачивает строку
        /// </summary>
        /// <param name="str">Значение строки</param>
        /// <returns></returns>
        private static String ReverseString(String str)
        {
            var sb = new StringBuilder();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Очищает число от ведущих нулей
        /// </summary>
        /// <param name="str">Строковое представление числа</param>
        /// <returns></returns>
        private static string CleanForZero(String str)
        {
            var i = 0;
            while (i < str.Length && str[i] == '0')
            {
                i++;
            }
            return str.Substring(i, str.Length - i);
        }

        /// <summary>
        /// Деление нацело и остаток от деления
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        private static BigDigit Modulo(BigDigit a, BigDigit b, ref BigDigit modulo)
        {
            a._signOfDefault = a._negative;
            b._signOfDefault = b._negative;
            a._negative = false;
            b._negative = false;
           
            if (b._value == "0")
            {
                CorrectNegative(ref a, ref b);
                return new BigDigit("0");
            }

            if (!IsLargerOrGreater(a, b))
            {
              
                modulo = a;
                if (a._signOfDefault)
                {
                    modulo._signOfDefault = true;
                    modulo._negative = true;
                }
                else
                {
                    modulo._signOfDefault = false;
                    modulo._negative = false;
                }
                CorrectNegative(ref a, ref b);
                return new BigDigit("0");
            }
            var strResult = new StringBuilder();
            var i = b._value.Length;
            var buffer = a._value.Substring(0, i);
            var temp = new BigDigit(buffer);
            if (temp < b)
            {
                temp.Value = (buffer + a._value[i]);
                i++;
            }

            while (temp >= b)
            {
                var x = 1;
                var buf = b * x;
                while (temp > buf)
                {
                    x++;
                    buf = b * x;
                }
                if (buf > temp)
                {
                    x--;
                    buf = b * x;
                }

                strResult.Append(x);
                temp -= buf;
                temp._value = CleanForZero(temp._value);
                while (temp < b && i < a._value.Length)
                {

                    var c = a._value[i];
                    if (c != '0' || temp._value.Length > 0)
                    {
                        temp._value += c;
                    }

                    i++;
                    if (temp < b) strResult.Append(0);
                }
            }

            modulo = temp;

            var quotient = new BigDigit(strResult.ToString());
            if (a._signOfDefault != b._signOfDefault)
            {
                quotient._negative = true;
                quotient._signOfDefault = true;
            }
            if (a._signOfDefault)
            {
                modulo._signOfDefault = true;
                modulo._negative = true;
            }
            else
            {
                modulo._signOfDefault = false;
                modulo._negative = false;
            }
            CorrectNegative(ref a, ref b);
            return quotient;
        }

        /// <summary>
        /// Сброс знака числа в начальное значение
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private static void CorrectNegative(ref BigDigit a, ref BigDigit b)
        {
            a._negative = a._signOfDefault;
            b._negative = b._signOfDefault;
        }
    }
}