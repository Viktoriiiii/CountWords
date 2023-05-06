# CountWords

### Задание:
Подсчитать количество слов во всех файлах и вывести в json файл в формате 

json
{ "слово":  "кол-во повторений"}

### Примечание:
Текст в файлах только на русском и английском языках. 
Брать файлы из папки в корне программы с названием “Words”.
Результат записывать в файл "result.json"
Желательно вести git  в процессе работы над задачей

### Усложнение:
* Добавить флаг, не считающий предлоги словами
* Добавить флаг для разделения вывода по файлам 
(без флага, все слова суммируются, с флагом выводятся 
последовательно слова в каждом файле), 
результат записывать в файл "result-per-file.json"

json
{ 
  "Название файла": {
    "Слово": "Кол-во повторений"
  }
}
