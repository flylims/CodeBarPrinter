﻿[![build](https://github.com/mnogomed/CodeBarPrinter/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/mnogomed/CodeBarPrinter/actions/workflows/dotnet-desktop.yml)

# CodeBarPrinter

Программа предназначена для:

- печати штрих-кодов (далее &mdash; ШК) биоматериала по номеру направления;
- регистрации партии биоматериала на текущем рабочем месте.

Является частью комплекса лабораторной информационной системы (далее &mdash; ЛИС) [FlyLIMS](https://github.com/mnogomed/FlyLims).

Для работы приложения необходимо периферийное оборудование:

- сканер двумерных ШК, доступный через последовательный интерфейс
- термо-принтер этикеток, поддерживающий язык EZPL (Godex)

## Установка

Приложение можно скачать со [страницы релиза](https://github.com/mnogomed/CodeBarPrinter/releases/latest), установка заключается в распаковке архива и определение настроек программы.

## Настройка

Настройки приложения находятся в файле `LIMSCodeBarPrinter.exe.config`, представляющий собой XML-документ. Перед первым использованием программы необходимо определить аттрибуты `value` тегов `add` раздела `configuration/appSettings`:

| Key                | Описание                             | Пример значения                            |
| ------------------ | ------------------------------------ | ------------------------------------------ |
| `LimsUrl`          | Адрес ЛИС                            | `http://lims.domain.ltd`                   |
| `LimsToken`        | Токен места регистрации биоматериала | `2e1b8af4de5ecc018d936b00cd7909ec0441c3fd` |
| `SerialPort`       | Порт подключения сканера ШК          | `COM3`                                     |
| `PrinterIPAddress` | IP-адрес термо-принтера этикеток     | `10.98.23.15`                              |
| `PrinterIPPort`    | Порт печати термо-принтера этикеток  | `9100`                                     |