using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Homework_NP_8
{
    
    class Program
    {
        static async Task Main()
        {
            //t.me/QuizPaintingsBot
            //7914751696:AAEcBqwthJrP7WCM3paxv_E2HfGZeUB4ayE
            //https://core.telegram.org/bots/api


            var botClient = new TelegramBotClient("7914751696:AAEcBqwthJrP7WCM3paxv_E2HfGZeUB4ayE");
            using var cts = new CancellationTokenSource();


            //Начало приема не блокирует поток вызова. Прием осуществляется в пуле потоков.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // получаем все типы обновлений
            };
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            var me = await botClient.GetMeAsync();


            Console.WriteLine($"Бот под именем @{me.Username}, запущен.");
            Console.ReadLine();
            // Отправляем токен отмены для завершения работы бота
            cts.Cancel();
        }
        

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //это нужно для кнопочной клавиатуры
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery1(botClient, update.CallbackQuery);
            }


            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;


            var chatId = message.Chat.Id;
            Console.WriteLine($"Получено сообщение: '{messageText}', в чате: {chatId}.");


            //////создаем клавиатуру 4 кнопки
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
            // first row
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Пабло Пикассо", callbackData: "Это не правильный ответ!"),
                InlineKeyboardButton.WithCallbackData(text: "Винсент Ван Гог", callbackData: "Это не правильный ответ!"),
            },
            // second row
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Клод Моне", callbackData: "Это правильный ответ!"),
                InlineKeyboardButton.WithCallbackData(text: "Энди Уорхол", callbackData: "Это не правильный ответ!"),
            }
            });
            //на сообщение пользователя посылаем картинку с текстом
            Message sendMessage = await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: InputFile.FromUri("https://ideyka.com.ua/files/resized/products/2858.1800x1800w.jpg"),
            caption: "<b>Кем была написана эта картина? (Фамилия автора)</b>.",
            parseMode: ParseMode.Html,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
        }


        static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };


            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        static async Task HandleCallbackQuery1(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {

            //Выводим пользователю идентификатор нажатой клавиши (можно здесь обрабатывать и делать действия)

            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Data);
            return;
        }
    }
    
}
