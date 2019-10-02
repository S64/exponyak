using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using S64.Bot.Builder.Adapters.Slack;
using S64.Excite.Translator;

namespace ExPonyak
{

    class ExPonyakBot : ActivityHandler
    {

        private readonly ExciteTranslatorClient translator
            = new ExciteTranslatorClient(Program.ExciteApiKey);

        // 区切り文字一覧
        private readonly string[] DELIMITERS = {
            " ", // 半角スペース
            "　", // 全角スペース
        };

        protected override async Task OnMessageActivityAsync(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken
        )
        {
            if (!SlackAdapter.CHANNEL_ID.Equals(turnContext.Activity.ChannelId))
            {
                // S64.Bot.Builder.Adapters.Slack以外を経由した場合は強制終了
                throw new NotImplementedException();
            }

            var data = turnContext.Activity.ChannelData as SlackChannelData;

            if (!turnContext.Activity.Type.Equals(ActivityTypes.Message) || data.IsMention != true || data.IsBot == true)
            {
                // 人以外やメンション等以外を無視
                Console.WriteLine("Ignored reason not supported message type.");
                return;
            }

            // 続く判定用に、区切り文字を使って分割する。第2引数の設定により、空白は削除される。
            var parts = turnContext.Activity.Text.Split(DELIMITERS, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2 || !parts[0].StartsWith("<@") || !parts[0].EndsWith(">"))
            {
                // 先頭がメンションではない場合は無視
                Console.WriteLine("Ignored reason not supported message format.");
                return;
            }

            // 翻訳対象として、最初のメンションを取り除く
            var origin = turnContext.Activity.Text.Substring(parts[0].Length + 1);

            var result = await translator.TranslateWithRetranslate(
                query: origin,
                source: Language.Ja,
                target: Language.En
            );

            await turnContext.SendActivityAsync(
                MessageFactory.Text(
                    $"{result.TranslatedText} ({result.RetranslatedText})"
                ),
                cancellationToken
            );
        }

    }

}
