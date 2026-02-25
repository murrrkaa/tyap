# Синтаксическая структура языка

## 1. Операторы
### Арифметические операторы
| Символ  | Операция    |
|---------|-------------|
| `+`     | Сложение    |
| `-`     | Вычитание   |
| `*`     | Умножение   |
| `/`     | Деление     |

## 2. Приоритет операторов

| Приоритет (по убыванию) | Операторы     |
|-------------------------|---------------|
| 5                       | `!` (унарный) |
| 4                       | `*`, `/`      |
| 3                       | `+`, `-`      |
| 2                       | `and`         |
| 1                       | `or`          |  

## 3.Встроенные функции

| Функция                        | Назначение                                                                                   |
|--------------------------------|----------------------------------------------------------------------------------------------|
| `readInt(): int`               | Считывает число, возвращает значение типа int                                                |
| `readFloat(): float`           | Считывает число, возвращает значение c плавающей точкой типа float                           |
| `readString(): string`         | Считывает строку, возвращает значение типа string                                            |
| `len(s)`                       | Вычисляет длину строки                                                                       |
| `substring(s, start, count)`   | Получает подстроку, где s - строка, start - начальный индекс, count - кол-во символов строки |
| `toString(num)`                | Преобразует число в строку                                                                   |
| `parseInt(str)`                | Преобразует строку в число                                                                   |
| `toBool(num)`                  | Преобразует число в булевый тип                                                              |
| `toFloat(num)`                 | Преобразует число с плавающей точкой в целое число                                           |

```ebnf

(* Программа состоит из определений и обязательной функции main *)
program = { top_level_statement }, main_function ;

(* Функция main — всегда возвращает int *)
main_function = "function", "main", "(", ")", ":", "int", "{", { function_statement }, "}" ;

(* Верхнеуровневая инструкция *)
top_level_statement =
      function_definition
    | variable_declaration, ";" 
    | constant_declaration, ";"
    
(* Объявление функции с типами *)
function_definition =
    "function", identifier, "(", [ typed_parameter_list ], ")", 
    [ ":", return_type ],
    "{", { function_statement }, "}" ;
    
(* Параметры функции с типами *)
typed_parameter_list = typed_parameter, { ",", typed_parameter } ;
typed_parameter = identifier, ":", type ;

(* Инструкции внутри функций *)
function_statement =
      simple_statement, ";"
    | matched_if_statement
    | unmatched_if_statement
    | return_statement
    | while_statement
    | for_statement ;

(* Return только внутри функций *)
return_statement = "return", [ expression ], ";" ;
    
(* Объявление переменной *)
variable_declaration = "var", identifier, ":", type, "=", expression ;

(* Объявление константы *)
constant_declaration = "const", identifier, ":", type, "=", expression ;

(* Типы данных для переменных и функций *)
type = "int" | "float" | "string" | "bool" ;
return_type = type | "void" ;

(* Инструкции *)
statement =
      simple_statement, ";"
    | matched_if_statement
    | unmatched_if_statement
    | while_statement
    | for_statement 
    | function_definition ;

simple_statement =
      assignment_statement
    | function_call
    | variable_declaration
    | constant_declaration
    | print_statement ;
    
(* Присваивание *)
assignment_statement = identifier, "=", expression ;
           
(* Вызовы функций *)
function_call = (identifier | built_in_function), "(", [ expression_list ], ")" ;

(* Имена встроенных функций *)
built_in_function = readInt_function | readFloat_function | readString_function | len_function | substring_function | toString_function | parseInt_function | toFloat_function | toBool_function;

(* readInt() → int *)
readInt_function = "readInt", "(", ")" ;

(* readFloat() → float *)
readFloat_function = "readFloat", "(", ")" ;

(* readString() → string *)
readString_function = "readString", "(", ")" ;

(* len(s) → int *)
len_function = "len", "(", expression, ")" ;

(* substring(s, start, count) → string *)
substring_function = "substring", "(", expression, ",", expression, ",", expression, ")" ;

(* toString(num: int) → string *)
toString_function = "toString", "(", expression, ")" ;  

(* parseInt(str: string) → int *)
parseInt_function = "parseInt", "(", expression, ")" ;  

(* toBool(num: int) → bool *)
toBool_function = "toBool", "(", expression, ")" ;  

(* toFloat(num: int) → float *)
toFloat_function = "toFloat", "(", expression, ")" ;

(* Вывод *)
print_statement = "print", "(", [ expression_list ], ")" ;

(* Блок инструкций *)
block = "{", { statement }, "}" ;

(* Конструкция if с else *)
matched_if_statement =
      "if", "(", expression, ")", block, "else", block ;   
       
(* Конструкция if без else *)
unmatched_if_statement =
      "if", "(", expression, ")", block
    | "if", "(", expression, ")", matched_if_statement, "else", unmatched_if_statement ;
    
(* Инструкция break *)
break_statement = "break" ;

(* Инструкция continue *)
continue_statement = "continue" ;

(* Блок инструкций для циклов (может содержать break/continue) *)
loop_block = "{", { loop_statement }, "}" ;

(* Инструкции внутри циклов *)
loop_statement =
    simple_statement, ";"
    | continue_statement, ";"
    | break_statement, ";"
    | matched_if_statement
    | unmatched_if_statement
    | while_statement
    | for_statement 
    | function_definition ;
    
(* Цикл while *)
while_statement = "while", "(", expression, ")", loop_block ;
      
(* Цикл for *)
for_statement = "for", "(", assignment_statement, ";", expression, ";", assignment_statement, ")", loop_block ;

(* Список выражений *)
expression_list = expression, { ",", expression } ;

(* Основное выражение: операции присваивания и логического ИЛИ *)
expression = logical_or_expression ;

(* Логическое ИЛИ *)
logical_or_expression = logical_and_expression, { "or", logical_and_expression } ;

(* Логическое И *)
logical_and_expression = comparison_expression, { "and", comparison_expression } ;

(* Сравнения: ==, !=, <, <=, >, >= *)
comparison_expression = additive_expression,
                        [ ("==" | "!=" | "<" | "<=" | ">" | ">="), additive_expression ] ;

(* Операции сложения и вычитания *)
additive_expression = term_expression, { ("+" | "-"), term_expression } ;

(* Термы — операции умножения и деления *)
term_expression = factor_expression, { ("*" | "/"), factor_expression } ;

(* Факторы — унарная операция НЕ *)
factor_expression = [ "!" ], simple_expression ;

(* Простые выражения — числа, идентификаторы, функции или подвыражения в скобках *)
simple_expression = number_literal | string_literal | bool_literal | function_call | identifier | "(", expression, ")" ;
```