using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(EnergyGaugeUI))]
public class EnergyGaugeUIEditor : Editor
{
    EnergyGaugeUI myTarget;
  
    public void OnEnable()
    {
        if (!Application.isPlaying)
        {
            LegacySetup();
        }
    }
    public override void OnInspectorGUI()
    {
        myTarget = (EnergyGaugeUI)target;
        SerializedObject serializedObject = new SerializedObject(target);

        SerializedProperty serializedProperty_current = serializedObject.FindProperty("current");
        SerializedProperty serializedProperty_max = serializedObject.FindProperty("max");

         EditorGUI.BeginChangeCheck();
        int preCurrent = serializedProperty_current.intValue;
        GUILayout.BeginHorizontal();
        preCurrent = EditorGUILayout.IntSlider(preCurrent, 0, serializedProperty_max.intValue);
        EditorGUILayout.LabelField(" / " + serializedProperty_max.intValue, GUILayout.Width(80));
        GUILayout.EndHorizontal();
       if (EditorGUI.EndChangeCheck())
        {
           serializedProperty_current.intValue = preCurrent;
        }
        serializedObject.ApplyModifiedProperties();

        if (!Application.isPlaying)
        {
            CheckUpdates();
            myTarget.MainGauge();
            myTarget.RequiredGauge();
        }

        SerializedProperty barSets_p = serializedObject.FindProperty("barSets");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+ Notch"))
        {
            if (Application.isPlaying)
            {
                myTarget.AddNotch();
            }
            else
            {
                //clear new notch
                serializedObject.Update();
                barSets_p.InsertArrayElementAtIndex(barSets_p.arraySize);

                int id = barSets_p.arraySize - 1;
                SerializedProperty mask_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("mask");
                mask_p.objectReferenceValue = null;
                SerializedProperty img_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("img");
                img_p.objectReferenceValue = null;
                SerializedProperty requiredImg_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("requiredImg");
                requiredImg_p.objectReferenceValue = null;
                SerializedProperty requiredMask_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("requiredMask");
                requiredMask_p.objectReferenceValue = null;
                SerializedProperty bleedImg_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("bleedImg");
                bleedImg_p.objectReferenceValue = null;

                serializedObject.ApplyModifiedProperties();
            }
        }
        if (barSets_p.arraySize > 1) GUI.enabled = true;
        else GUI.enabled = false;
        if (GUILayout.Button("- Notch"))
        {
            if (Application.isPlaying)
            {
                myTarget.RemoveNotch();
            }
            else
            {
                //delete check
                serializedObject.Update();

                int id = barSets_p.arraySize - 1;
                SerializedProperty mask_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("mask");
                if (mask_p.objectReferenceValue != null)
                {
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(((Image)mask_p.objectReferenceValue).gameObject);
                   if (prefab != null) Undo.DestroyObjectImmediate(prefab);
                    else
                    {
                        GameObject go = ((Image)mask_p.objectReferenceValue).gameObject;
                        Undo.DestroyObjectImmediate(go);
                    }
                }

                SerializedProperty img_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("img");
                if (img_p.objectReferenceValue != null)
                {
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(((Image)img_p.objectReferenceValue).gameObject);
                    if (prefab != null) Undo.DestroyObjectImmediate(prefab);
                    else
                    {
                        GameObject go = ((Image)img_p.objectReferenceValue).gameObject;
                        Undo.DestroyObjectImmediate(go);
                    }

                }

                SerializedProperty requiredImg_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("requiredImg");
                if (requiredImg_p.objectReferenceValue != null)
                {
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(((Image)requiredImg_p.objectReferenceValue).gameObject);
                    if (prefab != null) Undo.DestroyObjectImmediate(prefab);
                    else
                    {
                         GameObject go = ((Image)requiredImg_p.objectReferenceValue).gameObject;
                         Undo.DestroyObjectImmediate(go);
                    }
                }
                SerializedProperty requiredMask_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("requiredMask");
                if (requiredMask_p.objectReferenceValue != null)
                {
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(((Image)requiredMask_p.objectReferenceValue).gameObject);
                    if (prefab != null) Undo.DestroyObjectImmediate(prefab);
                    else
                    {
                        GameObject go = ((Image)requiredMask_p.objectReferenceValue).gameObject;
                         Undo.DestroyObjectImmediate(go);
                    }
                }

                SerializedProperty bleedImg_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("bleedImg");
                if (bleedImg_p.objectReferenceValue != null)
                {
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(((Image)bleedImg_p.objectReferenceValue).gameObject);
                    if (prefab != null) Undo.DestroyObjectImmediate(prefab);
                    else
                    {
                        GameObject go = ((Image)bleedImg_p.objectReferenceValue).gameObject;
                         Undo.DestroyObjectImmediate(go);
                    }
                }
;
                barSets_p.DeleteArrayElementAtIndex(barSets_p.arraySize - 1);
                serializedObject.ApplyModifiedProperties();
            }
        }
        GUI.enabled = true;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        showDebug = GUILayout.Toggle(showDebug,"show debug");

          base.OnInspectorGUI();
    }
    void LegacySetup()
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty barSets_p = serializedObject.FindProperty("barSets");
        serializedObject.Update();

        if (barSets_p.arraySize == 0) barSets_p.InsertArrayElementAtIndex(0);
        //legacy----------------------------
        SerializedProperty mask_p = serializedObject.FindProperty("mask");
        SerializedProperty img_p = serializedObject.FindProperty("img");
        SerializedProperty requiredImg_p = serializedObject.FindProperty("requiredImg");

        SerializedProperty new_mask_p = barSets_p.GetArrayElementAtIndex(0).FindPropertyRelative("mask");
        SerializedProperty new_img_p = barSets_p.GetArrayElementAtIndex(0).FindPropertyRelative("img");
        SerializedProperty new_requiredImg_p = barSets_p.GetArrayElementAtIndex(0).FindPropertyRelative("requiredImg");

        if (mask_p.objectReferenceValue != null)
        {
            new_mask_p.objectReferenceValue = mask_p.objectReferenceValue;
            mask_p.objectReferenceValue = null;
        }
        if (img_p.objectReferenceValue != null)
        {
            new_img_p.objectReferenceValue = img_p.objectReferenceValue;
            img_p.objectReferenceValue = null;
        }
        if (requiredImg_p.objectReferenceValue != null)
        {
            new_requiredImg_p.objectReferenceValue = requiredImg_p.objectReferenceValue;
            requiredImg_p.objectReferenceValue = null;
        }

        //------------------------------------

        serializedObject.ApplyModifiedProperties();
    }
    void CheckUpdates()
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty barSets_p = serializedObject.FindProperty("barSets");
        serializedObject.Update();

        myTarget = (EnergyGaugeUI)target;

        myTarget.ConsistentPosSize();

        for (int i = 0; i < myTarget.barSets.Count; i++)
        {
            SerializedProperty new_mask_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("mask");
            SerializedProperty new_img_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("img");
            SerializedProperty new_requiredMask_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("requiredMask");
            SerializedProperty new_required_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("requiredImg");

            if (myTarget.barSets[i].mask == null)
            {
                GameObject _mask = new GameObject("Mask_" + i);
                _mask.transform.SetParent(myTarget.transform);
                Image _maskImg = _mask.AddComponent<Image>();
                Mask _mc = _mask.AddComponent<Mask>();
                _mc.showMaskGraphic = false;

                //myTarget.barSets[i].mask = _maskImg;
                new_mask_p.objectReferenceValue = _maskImg;

                Undo.RegisterCreatedObjectUndo(_mask, "create GO");
                // Undo.RecordObject(target, "Setup");
            }
            else
            {
                if (myTarget.showMask)
                {
                    ((Image)new_mask_p.objectReferenceValue).enabled = true;
                    if (myTarget.maskImg != null) ((Image)new_mask_p.objectReferenceValue).sprite = myTarget.maskImg;
                    ((Image)new_mask_p.objectReferenceValue).GetComponent<Mask>().showMaskGraphic = myTarget.showMaskGraphic;
                    //myTarget.barSets[i].mask.enabled = true;
                    // if (myTarget.maskImg != null) myTarget.barSets[i].mask.sprite = myTarget.maskImg;
                   // myTarget.barSets[i].mask.GetComponent<Mask>().showMaskGraphic = myTarget.showMaskGraphic;

                }
                else
                {
                    if (myTarget.barSets[i].mask.enabled)
                    {
                        ((Image)new_mask_p.objectReferenceValue).enabled = false;
                        if (myTarget.maskImg != null) ((Image)new_mask_p.objectReferenceValue).sprite = null;
                        ((Image)new_mask_p.objectReferenceValue).GetComponent<Mask>().showMaskGraphic = false;
                       // myTarget.barSets[i].mask.enabled = false;
                       // myTarget.barSets[i].mask.sprite = null;
                        //myTarget.barSets[i].mask.GetComponent<Mask>().showMaskGraphic = false;
                    }
                }
                HideGO(myTarget.barSets[i].mask.gameObject);

                if (myTarget.barSets[i].img == null)
                {
                    Debug.Log("create main guage");
                    GameObject _gauge = new GameObject("mainGauge_" + i);
                    _gauge.transform.SetParent(myTarget.barSets[i].mask.transform);
                    Image _img = _gauge.AddComponent<Image>();

                    //myTarget.barSets[i].img = _img;
                    new_img_p.objectReferenceValue = _img;
                    Undo.RegisterCreatedObjectUndo(_gauge, "create GO");
                }
                else //has
                {
                    if (myTarget.barImg != null) ((Image)new_img_p.objectReferenceValue).sprite = myTarget.barImg;

                    HideGO(myTarget.barSets[i].img.gameObject);
                }


                if (myTarget.canRequired)
                {
                    //mask
                    if (myTarget.barSets[i].requiredMask == null)
                    {
                        GameObject _mask = new GameObject("RequiredMask_" + i);
                        _mask.transform.SetParent(myTarget.barSets[i].mask.transform);
                        Image _maskImg = _mask.AddComponent<Image>();
                        Mask _mc = _mask.AddComponent<Mask>();
                        _mc.showMaskGraphic = false;

                        //myTarget.barSets[i].requiredMask = _maskImg;
                        new_requiredMask_p.objectReferenceValue = _maskImg;

                        Undo.RegisterCreatedObjectUndo(_mask, "create GO");
                   //     Undo.RecordObject(target, "Setup");
                    }
                    else
                    {
                        //has
                        ((Image)new_requiredMask_p.objectReferenceValue).sprite = myTarget.maskImg;
                        //myTarget.barSets[i].requiredMask.sprite = myTarget.barSets[i].img.sprite;

                        HideGO(myTarget.barSets[i].requiredMask.gameObject);
                    }
                    //guage
                    if (myTarget.barSets[i].requiredImg == null)
                    {
                        Image requiredImg = Instantiate((Image)new_img_p.objectReferenceValue, ((Image)new_requiredMask_p.objectReferenceValue).transform);
                        requiredImg.name = "Required_" + i;
                        requiredImg.transform.SetParent(((Image)new_requiredMask_p.objectReferenceValue).transform);

                        new_required_p.objectReferenceValue = requiredImg;
                        Undo.RegisterCreatedObjectUndo(requiredImg, "create GO");
                    }
                    else
                    {
                        //has
                        ((Image)new_required_p.objectReferenceValue).sprite = myTarget.barImg;
                        ((Image)new_required_p.objectReferenceValue).transform.SetParent(((Image)new_requiredMask_p.objectReferenceValue).transform);
                        //myTarget.barSets[i].requiredImg.sprite = myTarget.barSets[i].img.sprite;
                        // myTarget.barSets[i].requiredImg.transform.SetParent(    ((Image)new_requiredMask_p.objectReferenceValue).transform  );

                        HideGO(myTarget.barSets[i].requiredImg.gameObject);
                    }
                }
                else
                {
                    if (myTarget.barSets[i].requiredImg != null)
                    {
                        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(myTarget.barSets[i].requiredImg.gameObject);
                        if (prefab != null) Undo.DestroyObjectImmediate(prefab);
                        else Undo.DestroyObjectImmediate(myTarget.barSets[i].requiredImg.gameObject);
                    }
                    if (myTarget.barSets[i].requiredMask != null)
                    {
                        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(myTarget.barSets[i].requiredMask.gameObject);
                        if (prefab != null) Undo.DestroyObjectImmediate(prefab);
                        else Undo.DestroyObjectImmediate(myTarget.barSets[i].requiredMask.gameObject);
                    }
                }

                if (myTarget.displayText != null || myTarget.displayTextTMP != null)
                {
                    myTarget.DisplayLogic();
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    bool showDebug;
    void HideGO(GameObject _go)
    {
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(_go);
        if (!showDebug)
        {
            if (prefab != null) prefab.hideFlags = HideFlags.HideAndDontSave;
            else _go.hideFlags = HideFlags.HideAndDontSave;
        }
        else
        {
            if (prefab != null) prefab.hideFlags = HideFlags.DontSave;
            else _go.hideFlags = HideFlags.DontSave;
        }
    }
}
