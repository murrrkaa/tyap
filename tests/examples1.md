# Примеры программ

# Эпик №1 - Базовый уровень

## Возможности языка на этом этапе

На данном этапе язык поддерживает:

- Типы данных: `int`, `float`, `string`
- Объявление переменных (`var`) и констант (`const`)
- Арифметические операции (`+`, `-`, `*`, `/`)
- Конкатенацию строк
- Встроенные функции:
  - `readInt()`
  - `readFloat()`
  - `readString()`
  - `len()`
  - `substring()`
- Вывод через `print(...)`
- Обязательную функцию `main(): int`

---

## CircleSquare

### Что демонстрирует

- Тип `float`
- Арифметику с плавающей точкой
- Константы
- Приоритет операций
- Строгую типизацию
- Ввод и вывод

### Код программы

```plaintext
function main(): int 
{
    var radius: float = readFloat();
    const pi: float = 3.1415926;
    var area: float = pi * radius * radius;
    print('Area = ', area);
    return 0;
}
```

###  Пояснение

Программа считывает радиус круга, вычисляет его площадь по формуле:

```
area = π × r × r
```

---

## Reverse

### Что демонстрирует

- Тип `string`
- Встроенную функцию `substring`
- Конкатенацию строк
- Использование арифметики в выражениях

### Код программы

```plaintext
function main(): int 
{
    var s: string = readString();
    var first: string = substring(s, 0, 1);
    var second: string = substring(s, 1, 1);
    var reversed: string = second + first;
    print(reversed);
    return 0;
}
```

### Пояснение

Программа считывает строку и меняет порядок первых двух символов.

---

# Эпик №2 - Контроль потока управления 

## Новые возможности языка

На этом этапе добавлены:

- Тип `bool`
- Ветвления `if ... else`
- Цикл `while`
- Пользовательские функции
- Рекурсия
- Инструкция `return`
- Сравнения (`==`, `!=`, `<`, `>`, `<=`, `>=`)
- Логические операторы

---

## GCD (алгоритм Евклида)

### Что демонстрирует

- Пользовательскую функцию
- Цикл `while`
- Ветвление `if/else`
- Сравнение значений
- Возврат значения из функции
- Изменение параметров функции

### Код программы

```plaintext
function gcd(a: int, b: int): int 
{
    while (a != b) 
    {
        if (a > b) 
        {
            a = a - b;
        }
        else 
        {
            b = b - a;
        }
    }
    return a;
}

function main(): int 
{
    var x: int = readInt();
    var y: int = readInt();
    var result: int = gcd(x, y);
    print('GCD = ', result);
    return 0;
}
```

### Пояснение

Реализован алгоритм Евклида через вычитание:

Пока числа не равны:
- из большего вычитается меньшее

---

## Factorial (рекурсивная версия)

### Что демонстрирует

- Рекурсию
- Условное ветвление
- Возврат значения
- Работу стека вызовов

### Код программы

```plaintext
function factorial(n: int): int 
{
    if (n == 0) 
    {
        return 1;
    } 
    else 
    {
        return n * factorial(n - 1);
    }
}

function main(): int 
{
    var value: int = readInt();
    var result: int = factorial(value);
    print('Factorial = ', result);
    return 0;
}
```

### Пояснение

Факториал вычисляется рекурсивно:

- Базовый случай: `0! = 1`
- Рекурсивный случай: `n! = n × (n - 1)!`

---

## ReverseString

### Что демонстрирует

- Тип `string`
- Встроенную функцию `substring`и `len`
- Конкатенацию строк
- Использование арифметики в выражениях
- цикл while

### Код программы

```plaintext
function reverse(s: string): string 
{
var result: string = '';
var i: int = len(s) - 1;
    while (i >= 0) 
    {
        result = result + substring(s, i, 1);
        i = i - 1;
    }
    return result;
}
function main(): int 
{
var input: string = readString();
var reversed: string = reverse(input);
print(reversed);
return 0;
}
```

### Пояснение

Переворачивает строку.
