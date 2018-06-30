using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using System.Collections.Generic;
using signalBot;
using static signalBot.ExchangeBase;

namespace signalBot
{
    partial class Program
    {
        enum Exchange { bitmex } // ТУДУ формировать список из базы
        enum Patterns { Absorption, ThreeCandle, Bifractal, Volume }
        
        BitMEXApi bitmex = new BitMEXApi(AppSettings.Bitmex_apiKey, AppSettings.Bitmex_apiSecret);

        // Experimental
        void Analyse()
        {


            //ArrayList arrayList = new ArrayList();

            var candle_list = new List<Candle>();
            //var candle_kit = new Dictionary<string, List<Candle>>();
            var array_exchanges = Enum.GetNames(typeof(Exchange));
            //var array_exchanges = Enum.GetValues(typeof(Exchange)).Cast<int>().Select(x => x.ToString()).ToArray();

            // проверка на наличие в словаре ключа
            //foreach (var item in array_exchanges) { if (!candle_kit.ContainsKey(item)) { candle_kit.Add(item, null); } };

            var timeframes_list = new List<string>() { "1h", "4h", "12h", "1d" };
            var assets_list = new List<string>() { "XBTUSD" }; // bitmex.GetAssets();

            //foreach (var item in array_exchanges)
            //{
            //    switch (item)
            //    {
            //        case "bitmex":
            //            for (var i = 5; i > 0; i--)
            //            {
            //                var candle = bitmex.GetCandle("XBTUSD", "4h", i - 1);
            //                //arrayList.Add(candle);
            //                list.Add(candle);
            //            }
            //            candleKit["bitmex"] = list;
            //            //dic.Add("bitmex", list);
            //            //list.Clear();
            //            break;
            //    }
            //}

            //var list = new List<ValueTuple<string, string, List<Candle>>>();

            foreach (var asset in assets_list)
            {
                foreach (var timeframe in timeframes_list)
                {
                    for (var i = 5; i > 0; i--)
                    {
                        var candle = bitmex.GetCandle(asset, timeframe, i - 1);
                        candle_list.Add(candle);
                    }

                    // List<string> pattern_list = SearchPattern(candle_list)
                    // SendSignals(asset, timeframe, pattern_list);

                }

            }

            // добавить анализ объёмов и реализовать сам метод

            //List<string> SearchPattern(List<Candle> list_c)
            //{
            //    var pattern_list = new List<string>() { "23" };
            //    return pattern_list;
            //}

            // Делаем запрос данных о капитализации и объёме. Отсеиваем пользователей по фильтрам.
            // Отправляем оставшимся пользователям данной биржи сообщение о том, что на указанном активе обнаружен определённый паттерн.

            void SendSignal(int asset, int timeframe, List<int> pattern_list) // связан с реализацией бота
            {
                //var volume; var cap;
                //var user_list= new Dictionary<string, ValueTuple>();
                //foreach(var user in user_list)
                //{
                //    if(user_list[user][volume]>volume && user_list[user][cap] > cap)
                //    {
                //        sendMessage();
                //    }
                //}

            }
            
            List<string> SearchPattern(List<signalBot.ExchangeBase.Candle> list)
            {
                var signalsList = new List<string>();
                if (bitmex.IsThreeCandleModel(candle_list[2], candle_list[3], candle_list[4])) { signalsList.Add(Patterns.ThreeCandle.ToString()); }
                if (bitmex.IsAbsorptionModel(candle_list[3], candle_list[4])) { signalsList.Add(Patterns.Absorption.ToString()); }
                if (bitmex.IsBifractalModel(candle_list[0], candle_list[1], candle_list[2], candle_list[3], candle_list[4])) { signalsList.Add(Patterns.Bifractal.ToString()); }
                if ((candle_list[0].Volume - candle_list[1].Volume / candle_list[1].Volume * 100) > 20) { signalsList.Add(Patterns.Volume.ToString()); }

                return signalsList;
            }

        }
    }
    partial class Program
    {
        enum State { ZERO, ENTER_VOLUME, ENTER_CAP }


        private static readonly TelegramBotClient Bot = new TelegramBotClient(AppSettings.Telegram_token);

        private static async void Bot_OnMessageRecieved(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            IReplyMarkup keyboard = new ReplyKeyboardRemove();
            var chat_id = message.Chat.Id;
            var text = "???";
            State state = State.ZERO;


            if (message == null || message.Type != MessageType.Text) { return; }

            if ((message.Text.StartsWith("/start")) || (message.Text.StartsWith("/help")))
            {
                text = "Привет. Я бот-помощник. Я помогаю криптотрейдерам в их нелёгком деле.\n\nДержи команды:\n/info – получение информации о текущем состоянии рынка\n/settings – изменение параметров мониторинга\n/remind – работа с напоминаниями\n/menu – вызов меню\n\nПиши /help, если забудешь команды ";
            }


            if (message.Text.StartsWith("Сводка"))
            {
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Сводка")},
                    new[]{new KeyboardButton("Настройки")},
                    new[]{new KeyboardButton("Напоминания")}
                });
                text = "сводная информация";
            }
            if (message.Text.StartsWith("/menu") || message.Text.StartsWith("Назад") || message.Text.StartsWith("Отмена"))
            {

                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Сводка")},
                    new[]{new KeyboardButton("Настройки")},
                    new[]{new KeyboardButton("Напоминания")}
                });
                text = "Меню";
            }
            if (message.Text.StartsWith("Настройки"))
            {
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Выбор биржи")},
                    new[]{new KeyboardButton("Фильтрация")},
                    new[]{new KeyboardButton("Назад")}
                });
                text = "Настройки";
            }
            if (message.Text.StartsWith("Выбор биржи"))
            {
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Bitmex")},
                    new[]{new KeyboardButton("Binance")},
                    new[]{new KeyboardButton("Назад")}
                });
                text = "Выбор биржи";
            }
            if (message.Text.StartsWith("Фильтрация"))
            {
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Объём")},
                    new[]{new KeyboardButton("Капитализация")},
                    new[]{new KeyboardButton("Назад")}
                });
                text = "Фильтрация";
            }
            if (message.Text.StartsWith("Объём"))
            {
                state = State.ENTER_VOLUME;
                // TODO
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Отмена")}
                });
                text = "Введите значение. По активам, у которых данный параметр меньше заданного, не будут приходить сигналы";
            }
            if (state == State.ENTER_VOLUME)
            {
                if (Int32.TryParse(message.Text, out int value))
                {
                    // TODO

                }
                text = "Фильтр настроен";
            }
            if (message.Text.StartsWith("Капитализация"))
            {
                // TODO
                state = State.ENTER_CAP;
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Отмена")}
                });
                text = "Введите значение. По активам, у которых данный параметр меньше заданного, не будут приходить сигналы";
            }
            if (state == State.ENTER_CAP)
            {
                if (Int32.TryParse(message.Text, out int value))
                {
                    // TODO
                }
                text = "Фильтр настроен";
            }
            if (message.Text.StartsWith("Напоминания"))
            {
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{ new KeyboardButton("Добавить напоминание"),new KeyboardButton("Удалить напоминание")},
                    new[]{new KeyboardButton("Назад")}
                });
                text = "Напоминания";
            }
            if (message.Text.StartsWith("Добавить"))
            {
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Назад")}
                });
                text = "Введите название торговой пары";
            }
            if (message.Text.StartsWith("Удалить"))
            {
                keyboard = new ReplyKeyboardMarkup(new[]{
                    new[]{new KeyboardButton("Назад")}
                });
                text = "Напоминания";
            }
            //switch(message.Text)
            //{

            //}
            await Bot.SendTextMessageAsync(chatId: chat_id, text: text, replyMarkup: keyboard);
        }

        static void Main(string[] args)
        {
            Bot.OnMessage += Bot_OnMessageRecieved;
            //Bot.OnCallbackQuery += Bot_OnCallbackQueryReceived;
            //var me = Bot.GetMeAsync().Result;
            //Console.WriteLine($"{ me.Username} starts");
            Bot.StartReceiving();
            //while(true)

            var s = new Coinmarketcap().GetBtcDominanceByCap();
            Console.WriteLine(s);
            Console.ReadKey();
        }

        private static void Bot_OnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
