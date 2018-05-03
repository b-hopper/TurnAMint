using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RotateAndDuplicate_TEMP : EditorWindow
{
    GameObject target;
    int instances;
    float distance;
    Vector3 center, axis;

    [MenuItem("Window/TEMP/RotateAndDuplicate")]
    private static void Init()
    {
        RotateAndDuplicate_TEMP window = (RotateAndDuplicate_TEMP)GetWindow(typeof(RotateAndDuplicate_TEMP));
        window.minSize = new Vector2(300f, 350f);
        window.maxSize = new Vector2(300f, 400f);

        window.autoRepaintOnSceneChange = true;
        window.titleContent.text = "Rotate and Duplicate";
        window.Show();
    }


    private void OnGUI()
    {
        target = Selection.activeGameObject;
        instances = EditorGUILayout.IntField("Instances of object", instances);
        distance = EditorGUILayout.FloatField("Distance from center", distance);
        center = EditorGUILayout.Vector3Field("Center", center);
        axis = EditorGUILayout.Vector3Field("Axis", axis);

        if (GUILayout.Button("Rotate and Duplicate"))
        {
            if (target != null && instances != 0)
            {
                float degreesToChange = 360 / (float)instances;

                for (int i = 0; i < instances; i++)
                {
                    GameObject tmp = Instantiate(target);
                    tmp.transform.parent = target.transform.parent;
                    tmp.transform.position = center + new Vector3(0, target.transform.position.y, distance);
                    tmp.transform.parent.Rotate(axis, degreesToChange);
                }
            }
        }
    }
}
