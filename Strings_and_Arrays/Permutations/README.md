Поиск всех перестановок из набора символов
======

##Получить список перестановок из 1234

    const string str = "1234";
    var per = new Permutations(str);
    var list = per.GetPermutationsList();
    foreach (var l in list)
    {
        Console.WriteLine(l);
    }

##Получить отсортированный список

    var list = per.GetPermutationsSortList();

##Получить перестановки без повторений
    var list = per.GetPermutationsSortList(false);