using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;


/*
    添付ファイルが送られてきたときに、
    テキストを送るように促すミドルウェア
 //*/
public class MyMiddleware : IMiddleware
{
    public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
    {
        /*
        throw new System.NotImplementedException();
        //*/

        var activity = turnContext.Activity;

        //添付ファイルのチェック
        if(activity.Type == ActivityTypes.Message
            && activity.Attachments != null
            && activity.Attachments.Count != 0)
        {
            //添付ファイルがあるとき
            await turnContext.SendActivityAsync("テキストを送ってね！");
        }
        else
        {
            //添付ファイルがないとき
            await next.Invoke(cancellationToken);
        }
    }
}
