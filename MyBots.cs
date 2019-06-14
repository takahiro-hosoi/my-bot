using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

public class MyBot : IBot
{
    private MyStateAccessors accessors;
    private DialogSet dialogs;

    /* 
        コンストラクタ
            引数として指定したMyStateAccessorsはDIとして自動解決される（意味不
            　DIとは
　　            IoCコンテナという場所に登録した情報を元に、必要なオブジェクトを動的に取り出す仕組みのこと
                ASP.NET Coreでは標準でこの機能が提供されている
    //*/
    public MyBot(MyStateAccessors accessors)
    {
        this.accessors = accessors;
        this.dialogs = new DialogSet(accessors.ConversationDialogState);

        /*
        /*
            ウォーターフォールのステップを定義
            処理準にメソッドを追加
            （メソッドは下のほうに定義している）
        ///
        var waterfallSteps = new WaterfallStep[]
        {
            NameStepAsync,
            NameConfirmStepAsync,
            AgeStepAsync,
            ConfirmStepAsync,
            SummaryStepAsync
        };

        //ウォーターフォールダイアログを追加
        dialogs.Add(new WaterfallDialog("profile", waterfallSteps));
        //テキスト型のプロンプトとしてid=nameで作成（意味不
        dialogs.Add(new TextPrompt("name"));
        //数値型のプロンプトとしてid=ageで作成
        dialogs.Add(new NumberPrompt<int>("age"));
        //
        dialogs.Add(new ConfirmPrompt("confirm"));

        //*/

        //コンポーネントダイアログを追加
        dialogs.Add(new ProfileDialog(accessors));

    }

    public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
    {
        //await turnContext.SendActivityAsync("Hello from MyBot");

        /*
        if(turnContext.Activity.Type == ActivityTypes.Message && !string.IsNullOrEmpty(turnContext.Activity.Text))
        {
            await turnContext.SendActivityAsync(turnContext.Activity.Text);
        }
        //*/

        /*
        if (turnContext.Activity.Type == ActivityTypes.Message)
        {
            //DialogSetからコンテキストを作成
            var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);

            //まずContinueDialogAsyncを実行し、既存のダイアログがあれば継続実行。
            var results = await dialogContext.ContinueDialogAsync(cancellationToken);

            //DialogTurnStatusが空の場合は既存のダイアログがないため、新規に実行
            if (results.Status == DialogTurnStatus.Empty)
            {
                /*
                //id=nameのプロンプトを送信
                await dialogContext.PromptAsync(
                    "name",
                    new PromptOptions { Prompt = MessageFactory.Text("名前を入力してね！") },
                    cancellationToken);
                ///

                /*
                //ウォーターフォールダイアログを送信
                await dialogContext.BeginDialogAsync("profile", null, cancellationToken);
                ///

                //コンポーネントダイアログを送信
                await dialogContext.BeginDialogAsync(nameof(ProfileDialog), null, cancellationToken);
            }
            //DialogTurnStatusがCompleteの場合、ダイアログは完了したため結果を処理
            else if ( results.Status == DialogTurnStatus.Complete)
            {
                /*
                if (results.Result != null)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"ようこそ、'{results.Result}'さん！"));
                }
                ///
                var userProfile = await accessors.UserProfile.GetAsync(turnContext);
                await turnContext.SendActivityAsync(MessageFactory.Text($"ようこそ '{userProfile.Name}'さん！"));
            }

            //*/

            //DialogSetからコンテキストを作成
            var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);

            //ユーザからメッセージが来たとき
            if(turnContext.Activity.Type == ActivityTypes.Message)
            {
                //まずContinueDialogAsyncを実行して、既存のダイアログがあれば継続実行
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                //DialogTrunStatusがCompleteの場合、ダイアログは完了したため結果を処理
                if(results.Status == DialogTurnStatus.Complete)
                {
                    var userProfile = await accessors.UserProfile.GetAsync(turnContext, () => new UserProfile(), cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text($"ようこそ'{userProfile.Name}'さん！"));
                }
            }
            //ユーザとボットが会話に参加したとき
            else if(turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                //turnContextよりActivityを取得
                var activity = turnContext.Activity.AsConversationUpdateActivity();
                //ユーザの参加に対してだけ、プロファイルダイアログを開始
                if(activity.MembersAdded.Any(member => member.Id != activity.Recipient.Id))
                {
                    await turnContext.SendActivityAsync("ようこそMyBotへ！");
                    await dialogContext.BeginDialogAsync(nameof(ProfileDialog), null, cancellationToken);
                }
            }

            //最後に現在のダイアログステートを保存 
            await accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

        //}

    }


    /*
        ウォーターフォールダイアログを使ったユーザプロファイル取得（意味不
    //*/
    private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        //"name"ダイアログ（プロンプト）を返信
        //ユーザに対する表示はPromptOptionsで指定（意味不
        return await stepContext.PromptAsync("name", new PromptOptions {Prompt = MessageFactory.Text("名前を入力してね！")},cancellationToken);
    }

    private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        //UserProfileをステートより取得
        var userProfile = await accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
        //Resultより名前の更新
        userProfile.Name = (string) stepContext.Result;
        //年齢を聞いてもよいのか確認のため、"confirm"ダイアログを送信
        return await stepContext.PromptAsync("confirm", new PromptOptions {Prompt = MessageFactory.Text("年齢を聞いてもよいですか？")}, cancellationToken);
    }

    private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        //Resulより結果の確認
        if((bool)stepContext.Result)
        {
            //年齢を聞いてもよい場合は"age"ダイアログを送信
            return await stepContext.PromptAsync("age", new PromptOptions { Prompt = MessageFactory.Text("年齢を入力してね！")}, cancellationToken);
        }
        else
        {
            //"いいえ"を選択した場合、次のステップに進む。"age"ダイアログの結果は"-1"を指定。
            return await stepContext.NextAsync(-1, cancellationToken);
        }
    }

    private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        //年齢の回答を取得
        var age = (int)stepContext.Result;
        //UserProfileをステートから取得
        var userProfile = await accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
        //年齢をスキップしなかった場合はユーザプロファイルに設定
        if(age != -1)
        {
            userProfile.Age = age;
        }
        //すべて正しいか確認。"confirm"ダイアログを再利用
        var prompt = $"次の情報で登録します。いいですか？{Environment.NewLine}名前：{userProfile.Name} 年齢：{userProfile.Age}";

        return await stepContext.PromptAsync("confirm", new PromptOptions {Prompt = MessageFactory.Text(prompt)}, cancellationToken);
    }

    private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        if((bool)stepContext.Result)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("プロファイルを保存します！"));
        }
        else
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("プロファイルを破棄します。"));
        }

        //ダイアログの終了
        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }
}