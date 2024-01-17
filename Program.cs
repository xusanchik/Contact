using Contacts;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

public static class Program
{
    private static TelegramBotClient? bot;

    public static async Task Main()
    {

        var bot = new TelegramBotClient("6332306275:AAGI4-wfyfvaP-ldH-YWFDzSIbG1KpgZ_Ew");

        User me = await bot.GetMeAsync();
        Console.Title = me.Username ?? "My awesome bot";
        using var cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
        bot.StartReceiving(Handlers.HandleUpdateAsync,
            Handlers.HandleErrorAsync,
            receiverOptions,
            cts.Token);

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        cts.Cancel();
    }
}

