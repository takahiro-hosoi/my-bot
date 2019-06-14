using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

/*
    受信したものを記録するミドルウェア

//*/
public class MyLoggingMiddleware : IMiddleware
{
    public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
    {
        /*
        throw new System.NotImplementedException();
        //*/

        Debug.WriteLine($"{turnContext.Activity.From}:{turnContext.Activity.Type}");
        Debug.WriteLineIf(
            !string.IsNullOrEmpty(turnContext.Activity.Text),
            turnContext.Activity.Text
        );
        await next.Invoke(cancellationToken);
    }
}
