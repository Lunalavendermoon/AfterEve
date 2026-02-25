using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Yarn;
using Yarn.Markup;
using Yarn.Unity;

public class YarnDialogueListener : DialoguePresenterBase
{
    public DialogueHistoryLogUI historyLog;

    public void AddLineToHistory(LocalizedLine line)
    {
        string speaker = line.CharacterName;
        string text = line.TextWithoutCharacterName.Text;

        historyLog.AddLineToLog(speaker, text, false);
    }

    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        AddLineToHistory(line);
        return YarnTask.CompletedTask;
    }

/*    public override async YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, LineCancellationToken cancellationToken)
    {
        DialogueOption selectedOption =
        await base.RunOptionsAsync(dialogueOptions, cancellationToken);

        return selectedOption;
    }*/

    public override YarnTask OnDialogueCompleteAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueStartedAsync()
    {
        return YarnTask.CompletedTask;
    }
}
