using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDoor : DoorInteract
{
    [SerializeField]
    private SCENE targetStage;
    [SerializeField]
    private int stageID;

    protected override void DoorOpenComplete()
    {
        GameManager.Inst.SceneMove(targetStage);
    }

    protected override void DoorOpening()
    {

    }
}
