using System.ComponentModel.DataAnnotations;

namespace Misea.Options
{
    public class TelegramServiceOptions
    {
        public const string TelegramService = "TelegramService";

        [Required]
        public string BotApiKey { get; set; }

        [Required]
        public int AdminId { get; set; }

        public string ActionsHelpCommand { get; set; }
    }
}
