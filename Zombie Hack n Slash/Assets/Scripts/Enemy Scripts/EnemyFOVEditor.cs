﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyFOVEditor : Editor
{
    void OnSceneGUI() {
        EnemyController fov = (EnemyController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle /2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle /2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA *  fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB *  fov.viewRadius);

        Handles.color = Color.red;
         if(fov.targetInRange)
         {
             Handles.DrawLine(fov.transform.position, fov.GetTargetPosition());
         }
        // else
        // {
        //     Handles.DrawLine(fov.transform.position, Vector3.zero);
        // }
    }
}
