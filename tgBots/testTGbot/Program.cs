using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace testTGbot
{
    class Program
    {
        static TelegramBotClient botClient = new TelegramBotClient("6003182405:AAFnonMTCgNap70UmroUL119aAMdIygkvW0");
        static SqlConnection sql = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\Desktop\\РАБОТА\\tgBots\\testTGbot\\Database1.mdf;Integrated Security=True");

        private const string FIRST_BUT = "Sharpness 10%";
        private const string SECOND_BUT = "Sharpness 30%";
        private const string THIRD_BUT = "Instagram";
        private const string FOURTH_BUT = "Black-white";
        static string EXE = "";

        static async Task Main(string[] args)
        {
            var me = await botClient.GetMeAsync();
            botClient.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "/start")
                {
                    if (sql.State == ConnectionState.Closed)
                    {
                        sql.Open();
                        SqlCommand cmd = new SqlCommand($"insert into %Table% (%Id%, %step%) values ('{message.Chat.Id}', '0')", sql);
                    }
                }

                Console.WriteLine($"{message.Chat.FirstName} | {message.Text}");
                if (message.Text.ToLower().Contains("hello brat"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose what to do with image:", replyMarkup: GetButtons());
                    return;
                }
                switch (message.Text)
                {
                    case FIRST_BUT:
                        EXE = "rezkost.exe";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Insert photo doc format");
                        break;
                    case SECOND_BUT:
                        EXE = "rezkost30.exe";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Insert photo doc format");
                        break;
                    case THIRD_BUT:
                        EXE = "inst.exe";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Insert photo doc format");
                        break;
                    case FOURTH_BUT:
                        EXE = "chb.exe";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Insert photo doc format");
                        break;
                    default:
                        break;
                }
            }

            if (message.Photo != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "If u send doc file it will be better");
            }

            if (message.Document != null && EXE != "")
            {
                CreateEditedPhoto(message, update, botClient, EXE);

            }
        }

        async static void CreateEditedPhoto(Message message, Update update, ITelegramBotClient botClient, string exe)
        {
            var fileId = update.Message.Document.FileId;
            var fileInfo = await botClient.GetFileAsync(fileId);
            var filePath = fileInfo.FilePath;

            string downloadFilePath = $@"C:\Users\User\Desktop\РАБОТА\tgBots\folderForEdited\{message.Document.FileName}";
            await using FileStream fileStream = System.IO.File.Create(downloadFilePath);
            await botClient.DownloadFileAsync(filePath, fileStream);
            fileStream.Close();

            Process.Start($@"C:\Users\User\Desktop\РАБОТА\tgBots\folderForEdited\{exe}", $@"""{downloadFilePath}""");
            await Task.Delay(5000);

            await using Stream stream = System.IO.File.OpenRead($@"C:\Users\User\Desktop\РАБОТА\tgBots\folderForEdited\{message.Document.FileName}");
            Message messageUpload = await botClient.SendDocumentAsync(
                message.Chat.Id,
                document: InputFile.FromStream(stream: stream, message.Document.FileName.Replace(".jpg", "(edited).jpg")),
                caption: "I've redone your picture \n =)"); 

            EXE = "";
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            (
                new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {
                        new KeyboardButton(FIRST_BUT), new KeyboardButton(SECOND_BUT)},
                    new List<KeyboardButton> {
                        new KeyboardButton(THIRD_BUT), new KeyboardButton(FOURTH_BUT)}
                }
            );
        }


        private static Task Error(ITelegramBotClient botClient, Exception update, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
