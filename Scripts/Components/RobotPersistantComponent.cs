using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class RobotPersistantComponent : PersistComponent
{
    public override Godot.Collections.Dictionary<string, Variant> Save()
    {

        //we assume the actor is a robot
        if(actor is not Robot robot)
        {
            GD.PushError("Actor is not robot!");
            return null;
        }
        else
        {

            var baseDict = base.Save();

            var robotInfoDict = new Godot.Collections.Dictionary<string, Variant>()
            {
                { "timeInSecondsForSelfDestruct", robot.timeRemainingFloat},
                { "hasBlownUp", robot.hasBlownUp},
                { "defused", robot.defused}
            };

            baseDict.Merge(robotInfoDict);
            return baseDict;
        }
    }
}
