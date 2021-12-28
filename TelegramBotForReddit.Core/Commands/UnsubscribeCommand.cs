using System.Threading.Tasks;
using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Commands
{
    public class UnsubscribeCommand : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IMapper _mapper;
        
        public UnsubscribeCommand
        (
            string commandName,
            IUserService userService, 
            IUserSubscribeService userSubscribeService,
            IMapper mapper
        ) 
            : base(commandName)
        {
            _userService = userService;
            _userSubscribeService = userSubscribeService;
            _mapper = mapper;
        }

        public override string Name { get; init; }
        
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
        {
            if (message.Text.Split(' ').Length != 2)
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Необходимо указать команду в виде /unsubscribe RedditName");
           
            var subredditName = message.Text.Split(' ')[1];

            var user = await _userService.GetById(message.From.Id);
            if (user == null)
                return await client.SendTextMessageAsync
                    (message.Chat.Id, "Необходимо перезапустить бот с помощью команды /start");

            var userSubscribe = await _userSubscribeService.GetActual(user.Id, subredditName);
            if (userSubscribe == null)
                return await client.SendTextMessageAsync(message.Chat.Id, $"Вы не подписаны на {subredditName}");

            _mapper.Map<UserSubscribeDto>(await _userSubscribeService.Unsubscribe(userSubscribe.Id));
            return await client.SendTextMessageAsync (message.Chat.Id, $"Подписка на {subredditName} отменена"); 
        }
    }
}