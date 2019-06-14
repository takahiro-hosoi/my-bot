
/*
    名前を聞くボット？
 //*/
using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

public class MyStateAccessors {

    /*
        コンストラクタ
    //*/
    public MyStateAccessors(
        UserState userState,
        ConversationState conversationState)
    {
        UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
    }

    //ダイアログを記録するプロパティ（謎
    public IStatePropertyAccessor<UserProfile> UserProfile { get; set; }
    public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
    public UserState UserState { get; }
    public ConversationState ConversationState { get; }
}