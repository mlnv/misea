using Misea.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Misea.Services
{
    public class TelegramService : IService
    {
        public event Action<string> MessageReceived;

        private TelegramBotClient telegramBot;

        // TODO: Find a solution to make it generic
        private long chatId;

        private readonly ILogger<TelegramService> logger;

        private List<string> availableActions { get; set; }

        private readonly TelegramServiceOptions options;

        public TelegramService(ILogger<TelegramService> logger, IOptions<TelegramServiceOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task Start()
        {
            telegramBot = new TelegramBotClient(options.BotApiKey);

            var me = await telegramBot.GetMeAsync();

            telegramBot.OnMessage += BotOnMessageReceived;
            telegramBot.OnMessageEdited += BotOnMessageReceived;
            //telegramBot.OnCallbackQuery += BotOnCallbackQueryReceived;
            //telegramBot.OnInlineQuery += BotOnInlineQueryReceived;
            //telegramBot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            telegramBot.OnReceiveError += BotOnReceiveError;

            telegramBot.StartReceiving(Array.Empty<UpdateType>());
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (options.AdminId != message.From.Id)
            {
                return;
            }

            if (message == null || message.Type != MessageType.Text)
            {
                return;
            }

            chatId = message.Chat.Id;

            if (message.Text == options.ActionsHelpCommand)
            {
                Task.Run(() => SendAvailableActions());
                return;
            }

            MessageReceived?.Invoke(message.Text);
        }

        public async Task Stop()
        {
            telegramBot.StopReceiving();
        }

        public async Task SendMessage(string text)
        {
            await telegramBot.SendTextMessageAsync(
                    chatId: chatId,
                    text: text);
        }

        public void SetAvailableActions(List<string> actions)
        {
            this.availableActions = actions;
        }

        private async Task SendAvailableActions()
        {
            if (availableActions == null || availableActions.Count == 0)
            {
                await SendMessage("There are no available actions");
                return;
            };

            var buttons = availableActions.Select(action => new List<KeyboardButton>() { new KeyboardButton(action) }).ToList();

            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                buttons,
                resizeKeyboard: true
            );

            //var replyKeyboardMarkup = new ReplyKeyboardMarkup(
            //    new KeyboardButton[][]
            //    {
            //            new KeyboardButton[] { StartFileServerServicesText, StopFileServerServicesText },
            //            new KeyboardButton[] { ShowTemperatureText },
            //            new KeyboardButton[] { MountUSBDriveText, UnmountUSBDriveText },
            //    },
            //    resizeKeyboard: true
            //);

            await telegramBot.SendTextMessageAsync(
                chatId: chatId,
                text: "Choose action",
                replyMarkup: replyKeyboardMarkup

            );
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            logger.LogError($"Received bot error code: {receiveErrorEventArgs.ApiRequestException.ErrorCode} " +
                $"message: {receiveErrorEventArgs.ApiRequestException.Message}");
        }
    }
}
