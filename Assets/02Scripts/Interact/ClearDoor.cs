using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDoor : DoorInteract
{
    protected override void DetectionStart()
    {

    }
    protected override void DetectionEnd()
    {

    }


    protected override void DoorOpening()
    {

    }

    protected override void DoorClosing()
    {

    }


    protected override void DoorOpenComplete()
    {
        base.DoorOpenComplete();

        GameManager.Inst.ChangeGameStatus(GameStatus.GameClear);
    }
}
