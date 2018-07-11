### signalBot
Мой институтский проект по созданию **Telegram**-бота на **C#** для помощи трейдеру в области криптовалют в принятии решений по осуществлению сделок. Основным функционалом является автоматизированный поиск [«свечных паттернов»](https://en.wikipedia.org/wiki/Candlestick_chart#Candlestick_patterns).
## Что реализовано 
* Базовый класс для выполнения запросов к API  
* Класс для взаимодействия с API биржи *BitMEX*  
* Классы для взаимодействия с API сервисов *coinmarketcap.com, coincap.io, blockchain.com (aka blockchain.info)*
* Создана база данных SQLite
* Черновая реализация алгоритмов поиска «свечных паттернов»
* Черновая реализация алгоритма мониторинга

![](https://github.com/StelX/signalBot/blob/master/classDia.PNG "")

## Что использовано
* C# 
* .NET Core Console app
* SQLite
* Telegram Bot API
* Newtonsoft JSON.Net
## TODO
* Осуществление мониторинга бирж с помощью API
* Предоставление пользователю аналитической информации
* Предоставление торговых сигналов на основе анализа объёма и «японских свечей»
* Поиск «свечных паттернов» 
* Индивидуальная настройка через интерфейс бота: биржи для мониторинга и фильтров
* Отправка в мессенджер сообщения при преодолении цены выбранных пользователем пар порогового значения
## История изменений
* 0.1 – Реализованы классы для взаимодействия с REST API внешних сервисов. Создана база данных. Черновая реализация алгоритмов поиска и мониторинга.
