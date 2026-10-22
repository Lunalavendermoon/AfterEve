using UnityEditor;
using UnityEngine;

public static class InventoryDiagnosis
{
    [MenuItem("Tools/Inventory Diagnosis/Prepare One Present Card")]
    static void Prepare()
    {
        if (!Application.isPlaying) return;
        var runner = Object.FindFirstObjectByType<Yarn.Unity.DialogueRunner>();
        if (runner != null && runner.IsDialogueRunning) runner.Stop();
        var manager = TarotManager.instance;
        if (manager.presentTarot.Count == 0 && manager.pastTarot.Count == 0 && manager.futureTarot.Count == 0)
            manager.AddCard(TarotCard.GetCardFromData(TarotCard.Arcana.Fool, TarotCard.TarotType.Present));
        PlayerController.instance.EnablePlayerInput();
        var ui = Object.FindFirstObjectByType<InventoryUIScript>();
        ui.SetInventoryDisplay(true);
        ui.SwitchDisplay(1);
        Debug.Log("[Inventory Diagnosis] Prepared live one-card reproduction.");
    }
}
