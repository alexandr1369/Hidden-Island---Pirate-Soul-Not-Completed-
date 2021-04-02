using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// info about current location stage
[CreateAssetMenu(fileName = "New Stage Info", menuName = "StageInfo")]
[System.Serializable]
[ExecuteInEditMode]
public class StageInfo : ScriptableObject
{
    public int stageIndex = 1; // index of current stage(1-5) [4 - unique, 5 = BOSS]

    public StageState stageState; // is completed trigger

    public float minVelocity; // min velocity of core's run speed
    public float maxVelocity; // max velocity of core's run speed

    public int prefabAmount; // needed amount of destroyed prefabs(animals or other items for 4th stage)

    // обязательно нужно определеять поле именно в данном классе, в custom editor не считается
    public List<EffectType> effectsList = new List<EffectType>(0); // list of effects(1-4)] [buffs + debuffs] || ONLY FOR 1-3 STAGES ||
}

#if UNITY_EDITOR
[CustomEditor(typeof(StageInfo))]
[CanEditMultipleObjects]
public class StageInfoEditor : Editor
{
    SerializedProperty stageIndex;
    SerializedProperty stageState;
    
    SerializedProperty minVelocity;
    SerializedProperty maxVelocity;

    SerializedProperty prefabAmount;

    SerializedProperty effectsList;

    public void OnEnable()
    {
        stageIndex = serializedObject.FindProperty("stageIndex");

        stageState = serializedObject.FindProperty("stageState");

        minVelocity = serializedObject.FindProperty("minVelocity");
        maxVelocity = serializedObject.FindProperty("maxVelocity");

        prefabAmount = serializedObject.FindProperty("prefabAmount");

        effectsList = serializedObject.FindProperty("effectsList");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // get target as Stage Info
        StageInfo stageInfo = target as StageInfo;

        // info box (the same as Header)
        EditorGUILayout.LabelField("Stage Selection", EditorStyles.boldLabel);

        // stage index
        int[] values = { 1, 2, 3, 4, 5 };
        string[] names = { "Stage I", "Stage II", "Stage III", "Stage IV", "Stage V" };
        stageIndex.intValue = EditorGUILayout.IntPopup("Index", stageInfo.stageIndex, names, values);

        // get stage state
        EditorGUILayout.PropertyField(stageState, new GUIContent("State"));

        // space and info box
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stage Settings", EditorStyles.boldLabel);

        if (stageInfo.stageIndex != 5)
        {
            // get velocities and prefab amount
            minVelocity.floatValue = EditorGUILayout.Slider("Min Velocity", stageInfo.minVelocity, 1, 10);
            maxVelocity.floatValue = EditorGUILayout.Slider("Max Velocity", stageInfo.maxVelocity, 1, 10);

            prefabAmount.intValue = EditorGUILayout.IntField("Prefab Amount", stageInfo.prefabAmount);
        }
        else
        {
            // get velocity
            minVelocity.floatValue = maxVelocity.floatValue = EditorGUILayout.Slider("Velocity", stageInfo.minVelocity, 1, 10);
        }

        // effects list
        int listSize;
        if(stageInfo.stageIndex == 1)
            listSize = 2;
        else if(stageInfo.stageIndex == 2)
            listSize = 3;
        else if (stageInfo.stageIndex == 3)
            listSize = 4;
        else
            listSize = 0;

        if(listSize != 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects Selection", EditorStyles.boldLabel);

            // clear list
            //effectsList.ClearArray();

            // fill it
            for (int i = 0; i < listSize; i++)
            {
                // add list item
                if(i >= effectsList.arraySize)
                    effectsList.InsertArrayElementAtIndex(i);

                // show list item
                EditorGUILayout.PropertyField(effectsList.GetArrayElementAtIndex(i), new GUIContent("Effect " + (i + 1)));
            }
        } // if

        serializedObject.ApplyModifiedProperties();
    } // OnInspectorGUI()
}
#endif
public enum StageState
{
    Current,
    Undiscovered,
    Completed
}
