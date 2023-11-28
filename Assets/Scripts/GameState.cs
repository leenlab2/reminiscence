using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ending
{
    EndingA,
    EndingB,
    Neurtal
}

public class GameState
{
    public static int level;
    private static int routeTracker;

    public static Ending ending
    {
        get
        {
            if (routeTracker > 0)
            {
                return Ending.EndingA;
            }
            else if (routeTracker < 0)
            {
                return Ending.EndingB;
            }
            else
            {
                return Ending.Neurtal;
            }
        }
    }

    public static void RecordRoute(bool isBranchA)
    {
        if (isBranchA)
        {
            routeTracker++;
        }
        else
        {
            routeTracker--;
        }
    }

    public static void ResetRoute()
    {
        routeTracker = 0;
    }
}
