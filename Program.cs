using System;
using System.Linq;

namespace FindTail
{
  class Program
  {
    private static bool _silent; //Если true, отладочные данные в консоль не выводятся

    //максимальное число в массиве, влияет на скорость работы алгоритма и читаемость отладочных данных 
    //(внимание, при больших _maxNumber логгирование падает с Out Of Memory) - чинить пока не стал
    private static int _maxNumber = 10; 

    static void Main(string[] args)
    {
      _silent = true; //отключаем вывод отладочной информации
      _maxNumber = 10000; //увеличиваем размер массива

      for (var i = 0; i < 10000; i++)
        FindTail();
    }

    /// <summary>
    /// Полное описание алгоритма - в README
    /// Кратко:
    /// 1) Идем по массиву начиная с нулевого индекса методом перехода к индексу, лежащему в очередной ячейке
    /// 2) Ищем индекс, гарантированно участвующий в "петле" (повторяющейся последовательности переходов по индексам)
    /// 3) На основании индекса, входящего в петлю считаем длину "петли": переходим к этому индексу и считаем кол-во итераций, необходимых чтобы снова вернуться в этот индекс.
    /// 4) Начиная с нулевого индекса, параллельно итерируемся по массиву методом перехода к индексу сразу двумя итераторами, отстающими друг от друга на длину "петли". 
    /// 5) Когда оба итератора укажут на один и тот же индекс - мы нашли повторяющееся число в массиве
    /// </summary>
    private static void FindTail()
    {
      int[] testData = GetData(); //генерируем тестовые данные
      ShowLog("array: " + string.Join(',', testData)); //вывод данных в консоль (для отладки)

      //Находим индекс, который гарантированно участвует в "петле"
      //(Это пункты 1-2 алгоритма или пункт (2) полного описания из Readme)
      //Сложность - O(n) - один проход по данным
      int tailIndex = GetTailIndex(testData);
      ShowLog("tailed index: " + tailIndex);

      //Считаем кол-во элементов в "петле"
      //(Это пункт 3 алгоритма или пункт (5) полного описания из Readme)
      //Сложность - O(n) - один полный проход по данным в худшем случае, когда в "петле" все данные массива
      int tailLength = GetTailLength(testData, tailIndex);
      ShowLog("tail length: " + tailLength);

      //Находим начало "петли"/дублирующийся элемент массива
      //(Это пункты 4-5 алгоритма или пункт (6) полного описания из Readme)
      //Сложность - O(2n) - два прохода по данным в худшем случае, когда "петля" начинается в конце массива
      int tailStart = GetTailStart(testData, tailLength); //в худшем случае еще два прохода по данным
      ShowLog("tail started from (answer): " + tailStart);

      //общая сложность O(4n) => O(n), константным значением можно пренебречь т.к. его влияние уменьшается с ростом n

      //проверим результат другим алгоритмом:
      bool isChecked = testData.Where(x => x == tailStart).Count() > 1;
      if (!isChecked)
        throw new Exception("Результат не прошел проверку альтернативным алгоритмом!");
    }

    private static int GetTailStart(int[] data, int tailLength)
    {
      int nextIndex = 0;
      for (int i = 0; i < tailLength; i++)
        nextIndex = data[nextIndex];

      int checkIndex = 0;
      for (int i = 0; i < data.Length; i++)
      {
        if (data[nextIndex] == data[checkIndex])
          return data[checkIndex];

        nextIndex = data[nextIndex];
        checkIndex = data[checkIndex];
      }

      throw new Exception("Начало 'петли' не найдено");
    }

    private static int GetTailLength(int[] data, int tailIndex)
    {
      int result = 1;
      int nextIndex = data[tailIndex];

      while (tailIndex != nextIndex)
      {
        result++;
        nextIndex = data[nextIndex];
      }

      return result;
    }

    private static int GetTailIndex(int[] data)
    {
      int res = 0; //начнем с индекса, на который не указывает ни один элемент массива, т.к. в данных массива нет нулей

      ShowLog("algo steps: ", true);
      for (int i = 0; i < data.Length; i++) //сделаем столько итераций перехода к следующему индексу, чтобы гарантировать закольцовывание
      {
        res = data[res];

        ShowLog(res.ToString() + '-', true);
      }

      ShowLog("");

      return res; //вернем индекс, который гарантированно участвует в закольцованной части данных
    }

    private static int[] GetData()
    {
      Random randomizer = new Random();
      int maxNum = randomizer.Next(1, _maxNumber); //минимум - 1 (по условию задачи), максимум задан чтобы не затягивать работу улгоритма, но вообще может быть любым
      int resultLength = maxNum + 1; //длина массива на 1 больше, чем теоретическое кол-во уникальных данных в массиве (по условию задачи)

      int[] result = new int[resultLength];
      for (int i = 0; i < result.Length; i++)
        result[i] = randomizer.Next(1, maxNum);

      return result;
    }

    private static void ShowLog(string message, bool inline = false)
    {
      if (_silent)
        return;

      if (inline)
        Console.Write(message);
      else
        Console.WriteLine(message);
    }
  }
}
