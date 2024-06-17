using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDoor : DoorInteract
{
    protected override void DoorOpenComplete()
    {
        PlayerController.Inst.DoorInteract(transform);
        GameManager.Inst.ChangeGameStatus(GameStatus.GameClear);
    }

    protected override void DoorOpening()
    {
        
    }
}
