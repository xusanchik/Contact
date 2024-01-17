using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Newtonsoft.Json;
using File = System.IO.File;

namespace Contacts
{
    internal class Handlers
    {
        private static string usage;

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");

            string paht = "D:\\C#projects\\NajotTalim\\N03\\Tasks\\Task01\\T02\\T_G_task2\\Users\\Users.txt";
            if (message.Type == MessageType.Contact && message.Contact != null)
            {

                Console.WriteLine($"Phone number: {message.Contact.PhoneNumber}");
                var data = File.ReadAllText(paht);

                var listuser = JsonConvert.DeserializeObject<List<Contact>>(data);
                var find = listuser.FirstOrDefault(x => x.UserId == message.Contact.UserId);

                if (find is null)
                {
                    listuser.Add(message.Contact);
                    File.WriteAllText(paht, JsonConvert.SerializeObject(listuser));
                }


                message.Text = "bu foydalanuvchi mavjud";

                if (message.Type != MessageType.Text)
                    return;





                var action = message.Text!.Split(' ')[0] switch
                {
                    "/getAllUser" => GEtallUser(botClient, message),
                    _ => Usage(botClient, message)
                };
                Message sentMessage = await action;
                Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");


                static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message)
                {
                    ReplyKeyboardMarkup requestReplyKeyboard = new(
                        new[]
                        {
                KeyboardButton.WithRequestContact("Send my phone Number"),
                        });

                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: "Could you please send your phone number?",
                                                                replyMarkup: requestReplyKeyboard);
                }

                static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
                {
                    const string usage = "/GetAllUser  - Are you get Users ";

                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: usage,
                                                                replyMarkup: new ReplyKeyboardRemove());
                }
                 static async Task<Message> GEtallUser(ITelegramBotClient botClient, Message message)
                {
                    ReplyKeyboardMarkup requestReplyKeyboard = new(
                      new[]
                      {
                KeyboardButton.WithRequestContact("nimaga sdfkmsdlfm"),
                      });
                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                              text: "Are you get Users",
                                                              replyMarkup: requestReplyKeyboard);
                }
            }
            else
            {

                var action = message.Text!.Split(' ')[0] switch

                {
                    "/myNumber" => RequestContactAndLocation(botClient, message),
                    "/GetAllUser" => GEtallUser(botClient, message),
                    _ => Usage(botClient, message),

                } ;


                static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message)
                {
                    ReplyKeyboardMarkup requestReplyKeyboard = new(
                        new[]
                        {
                KeyboardButton.WithRequestContact("Send my phone Number"),
                        });

                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: "Could you please send your phone number?",
                                                                replyMarkup: requestReplyKeyboard);
                }

                static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
                {
                    const string usage = "/myNumber  - to send your phone number";

                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: usage,
                                                                replyMarkup: new ReplyKeyboardRemove());
                }

            }

            static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
            {
                Console.WriteLine($"Unknown update type: {update.Type}");
                return Task.CompletedTask;
            }
        }

        private static async Task GEtallUser(ITelegramBotClient botClient, Message message)
        {
            string paht = "D:\\C#projects\\NajotTalim\\N03\\Tasks\\Task01\\T02\\T_G_task2\\Users\\Users.txt";
            var data = File.ReadAllText(paht);

            var listuser = JsonConvert.DeserializeObject<List<Contact>>(data);
            foreach(var item in listuser)
            {
                await botClient.SendContactAsync(chatId: message.Chat.Id,
                    phoneNumber: item.PhoneNumber, firstName: item.FirstName);
            }
        } 
    }
}
