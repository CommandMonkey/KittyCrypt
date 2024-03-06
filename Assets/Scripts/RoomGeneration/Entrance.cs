using System;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public Direction direction;
    [NonSerialized] public Entrance connection; 
}
