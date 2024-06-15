using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class Eng : ScriptableObject
{
	public List<Interaction> interaction; // Replace 'EntityType' to an actual type that is serializable.
	public List<CauseOfDeath> causeOfDeath; // Replace 'EntityType' to an actual type that is serializable.
}
