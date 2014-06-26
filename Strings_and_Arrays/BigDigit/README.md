Работа с числами любой длины
======

>Реализованы операции: +,-,*,/,%,<,>,<=,>=,!=,==
>>!!!Алгоритмы не оптимизированы по скорости.

======

var number1 = new BigDigit(123456789);
var number2 = new BigDigit("987654321");
var number3 = new BigDigit("-777");
var number4 = new BigDigit(-1234);
Console.WriteLine(number1 + number2);
Console.WriteLine(number1 - number2);
Console.WriteLine(number1 * number2);
Console.WriteLine(number1 / number3);
Console.WriteLine(number2 % number4);