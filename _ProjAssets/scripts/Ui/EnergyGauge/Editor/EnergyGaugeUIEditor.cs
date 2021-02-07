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
        myTarget = (EnergyGaugeUI)target;
        if (!Application.isPlaying)
        {
            /*
            Transform[] _tr = myTarget.transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < _tr.Length; i++)
            {
                if (_tr[i] != null && _tr[i] != myTarget.transform ) DestroyImmediate(_tr[i].gameObject);
            }
            */
            CheckUpdates();
            HideGos(showDebug);
        }

    }

    public override void OnInspectorGUI()
    {
        myTarget = (EnergyGaugeUI)target;
        SerializedObject serializedObject = new SerializedObject(target);

        SerializedProperty serializedProperty_current = serializedObject.FindProperty("current");
        SerializedProperty serializedProperty_max = serializedObject.FindProperty("max");

        EditorGUI.BeginChangeCheck();
        showDebug = GUILayout.Toggle(showDebug, "show debug");
        if (EditorGUI.EndChangeCheck())
        {
            HideGos(showDebug);
        }
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        int preCurrent = serializedProperty_current.intValue;
        preCurrent = EditorGUILayout.IntSlider(preCurrent, 0, serializedProperty_max.intValue);

        if (EditorGUI.EndChangeCheck())
        {
            serializedProperty_current.intValue = preCurrent;
        }

        //call this if inspector changed

        CheckUpdates();
        myTarget.MainGauge();
        myTarget.RequiredGauge();



        //serializedProperty_current.intValue = preCurrent;
        EditorGUILayout.LabelField(" / " + serializedProperty_max.intValue, GUILayout.Width(80));
        GUILayout.EndHorizontal();


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

                CheckUpdates();

                serializedObject.Update();

                //adjust
               // RectTransform thisRt = myTarget.transform.GetComponent<RectTransform>();
                RectTransform _rt = ((Image)mask_p.objectReferenceValue).rectTransform;
               // _rt.anchorMin = thisRt.anchorMin;
               // _rt.anchorMax = thisRt.anchorMax;
                //_rt.pivot = thisRt.pivot;
                _rt.anchoredPosition = new Vector2((myTarget.barSets.Count-1) * myTarget.size.x, 0);
                _rt.localScale = Vector3.one;

                myTarget.MainGauge();
                myTarget.RequiredGauge();
                HideGos();


            }
        }
        if (barSets_p.arraySize > 0) GUI.enabled = true;
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
                    DestroyGo(((Image)mask_p.objectReferenceValue).gameObject);
                    serializedObject.ApplyModifiedProperties();
                }

                SerializedProperty img_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("img");
                if (img_p.objectReferenceValue != null)
                {
                    DestroyGo(((Image)img_p.objectReferenceValue).gameObject);
                    serializedObject.ApplyModifiedProperties();
                }

                SerializedProperty requiredImg_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("requiredImg");
                if (requiredImg_p.objectReferenceValue != null)
                {
                    DestroyGo(((Image)requiredImg_p.objectReferenceValue).gameObject);
                    serializedObject.ApplyModifiedProperties();
                }
                SerializedProperty requiredMask_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("requiredMask");
                if (requiredMask_p.objectReferenceValue != null)
                {
                    DestroyGo(((Image)requiredMask_p.objectReferenceValue).gameObject);
                    serializedObject.ApplyModifiedProperties();
                }

                SerializedProperty bleedImg_p = barSets_p.GetArrayElementAtIndex(id).FindPropertyRelative("bleedImg");
                if (bleedImg_p.objectReferenceValue != null)
                {
                    DestroyGo(((Image)bleedImg_p.objectReferenceValue).gameObject);
                    serializedObject.ApplyModifiedProperties();
                }
;
                barSets_p.DeleteArrayElementAtIndex(barSets_p.arraySize - 1);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                CheckUpdates();
                myTarget.MainGauge();
                myTarget.RequiredGauge();
                HideGos(showDebug);
            }
        }
        GUI.enabled = true;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);


          base.OnInspectorGUI();






        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
   /* void LegacySetup()
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
    }*/

    void CheckUpdates()
    {
      //  SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty barSets_p = serializedObject.FindProperty("barSets");


        myTarget = (EnergyGaugeUI)target;

        for (int i = 0; i < myTarget.barSets.Count; i++)
        {
            serializedObject.Update();

            SerializedProperty new_mask_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("mask");
            SerializedProperty new_img_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("img");
            SerializedProperty new_requiredMask_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("requiredMask");
            SerializedProperty new_required_p = barSets_p.GetArrayElementAtIndex(i).FindPropertyRelative("requiredImg");


            if (new_mask_p.objectReferenceValue == null) //mask
            {

                GameObject _mask = new GameObject("Mask_" + i);
                //_mask.transform.SetParent(myTarget.transform);
                HideGO(_mask);
                ParentGo(_mask.gameObject, myTarget.gameObject);

                Image _maskImg = _mask.AddComponent<Image>();
                Mask _mc = _mask.AddComponent<Mask>();
                _mc.showMaskGraphic = false;

                //myTarget.barSets[i].mask = _maskImg;
                new_mask_p.objectReferenceValue = _maskImg;

                _mc.enabled = myTarget.showMask;
                _mc.showMaskGraphic = myTarget.showMaskGraphic;

                _maskImg.sprite = myTarget.maskImg;

                serializedObject.ApplyModifiedProperties();

                Debug.Log("making mask");
            }
            else
            {


                Mask mask = ((Image)new_mask_p.objectReferenceValue).GetComponent<Mask>();

                mask.enabled = myTarget.showMask;
                mask.showMaskGraphic = myTarget.showMaskGraphic;

                ((Image)new_mask_p.objectReferenceValue).sprite = myTarget.maskImg;
                serializedObject.ApplyModifiedProperties();

            }
            

            if (new_img_p.objectReferenceValue == null) //img bar
                {
                
               // Debug.Log("create main guage");
                    GameObject _gauge = new GameObject("mainGauge_" + i);
                HideGO(_gauge);
                ParentGo(_gauge.gameObject, myTarget.barSets[i].mask.gameObject);
                   // _gauge.transform.SetParent(myTarget.barSets[i].mask.transform);
                    Image _img = _gauge.AddComponent<Image>();

                    //myTarget.barSets[i].img = _img;
                    new_img_p.objectReferenceValue = _img;

                    //Undo.RegisterCreatedObjectUndo(_gauge, "create GO");
                serializedObject.ApplyModifiedProperties();
             
            }

            if (myTarget.barImg != null)
            {
                //if (((Image)new_img_p.objectReferenceValue).sprite != myTarget.barImg)
                //{
                    ((Image)new_img_p.objectReferenceValue).sprite = myTarget.barImg;

                    serializedObject.ApplyModifiedProperties();
               
               // }
            }
                


                //mask
                if (new_requiredMask_p.objectReferenceValue == null)
                {
              
                GameObject _mask = new GameObject("RequiredMask_" + i);
                    HideGO(_mask);
                    ParentGo(_mask.gameObject, myTarget.barSets[i].mask.gameObject);
                // _mask.transform.SetParent(myTarget.barSets[i].mask.transform);

                Image _maskImg = _mask.AddComponent<Image>();
                Mask _mc = _mask.AddComponent<Mask>();
                _mc.showMaskGraphic = false;

                //myTarget.barSets[i].requiredMask = _maskImg;
                new_requiredMask_p.objectReferenceValue = _maskImg;

               // Undo.RegisterCreatedObjectUndo(_mask, "create GO");
 
                serializedObject.ApplyModifiedProperties();
              
                }
                //has
                if (myTarget.barImg != null)
                {
                   // if (((Image)new_requiredMask_p.objectReferenceValue).sprite != myTarget.maskImg)
                   // {
                        ((Image)new_requiredMask_p.objectReferenceValue).sprite = myTarget.barImg;                    //myTarget.barSets[i].requiredMask.sprite = myTarget.barSets[i].img.sprite;

                        serializedObject.ApplyModifiedProperties();
                        
                   // }
                }

                //guage
                if (new_required_p.objectReferenceValue == null)
                {
                   
                    Image requiredImg = Instantiate((Image)new_img_p.objectReferenceValue, ((Image)new_requiredMask_p.objectReferenceValue).transform);
                    requiredImg.name = "Required_" + i;
                    HideGO(requiredImg.gameObject);
                    ParentGo(requiredImg.gameObject, ((Image)new_requiredMask_p.objectReferenceValue).gameObject);
                    //requiredImg.transform.SetParent(((Image)new_requiredMask_p.objectReferenceValue).transform);

                    new_required_p.objectReferenceValue = requiredImg;

                    //Undo.RegisterCreatedObjectUndo(requiredImg, "create GO");
                        serializedObject.ApplyModifiedProperties();
                     
                }
                //has
                if (myTarget.barImg != null)
                {
                    //if (((Image)new_required_p.objectReferenceValue).sprite != myTarget.barImg)
                    //{
                        ((Image)new_required_p.objectReferenceValue).sprite = myTarget.barImg;

                        serializedObject.ApplyModifiedProperties();
                       
                    //}
                }

            if (myTarget.canRequired)
            {
                ((Image)new_requiredMask_p.objectReferenceValue).enabled = true;
                ((Image)new_required_p.objectReferenceValue).enabled = true;
            }
            else
            {
                ((Image)new_requiredMask_p.objectReferenceValue).enabled = false;
                ((Image)new_required_p.objectReferenceValue).enabled = false;
            }
            if (!Application.isPlaying)
            {
                if (myTarget.displayText != null || myTarget.displayTextTMP != null)
                {
                    myTarget.DisplayLogic();
                }
            }
        }
        myTarget.ConsistentPosSize();

        serializedObject.ApplyModifiedProperties();
       
    }
    bool showDebug;
    void HideGos(bool show = false)
    {
        if (show)
        {
            Transform[] _tr = myTarget.transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < _tr.Length; i++)
            {
                if (_tr[i] != null && _tr[i] != myTarget.transform) _tr[i].gameObject.hideFlags = HideFlags.None;
            }
        }
        else
        {
            for (int i = 0; i < myTarget.barSets.Count; i++)
            {
                if (myTarget.barSets[i].img) HideGO(myTarget.barSets[i].img.gameObject);
               if (myTarget.barSets[i].mask) HideGO(myTarget.barSets[i].mask.gameObject);
              if (myTarget.barSets[i].bleedImg) HideGO(myTarget.barSets[i].bleedImg.gameObject);
                if (myTarget.barSets[i].requiredImg) HideGO(myTarget.barSets[i].requiredImg.gameObject);
                if (myTarget.barSets[i].requiredMask) HideGO(myTarget.barSets[i].requiredMask.gameObject);
           }
        }
    }
    void HideGO(GameObject _go)
    {
       
        if (_go == null) return;

            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(_go);
            if (!showDebug)
            {
               // if (prefab != null) prefab.hideFlags = HideFlags.HideAndDontSave;
               // else _go.hideFlags = HideFlags.HideAndDontSave;
           _go.hideFlags = HideFlags.HideInHierarchy;
        }
            else
            {
            // if (prefab != null) prefab.hideFlags = HideFlags.DontSave;
            // else _go.hideFlags = HideFlags.DontSave;
          _go.hideFlags = HideFlags.None;
        }
            EditorApplication.DirtyHierarchyWindowSorting();
        
    }
    void ParentGo(GameObject _go, GameObject _parentGo )
    {
        //GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(_parentGo);
        //if (prefab != null) _go.transform.SetParent(prefab.transform);
        //else 
            _go.transform.SetParent(_parentGo.transform);
    }
    void DestroyGo(GameObject _go)
    {
        
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(_go);
        if (prefab != null)
        {
            Debug.Log("destroy " + prefab.name);
            Undo.DestroyObjectImmediate(prefab);
        }
        else Undo.DestroyObjectImmediate(_go);
        

        /*
        if (PrefabUtility.IsPartOfPrefabInstance(myTarget.transform))
            PrefabUtility.UnpackPrefabInstance(myTarget.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        Undo.DestroyObjectImmediate(_go);
        */
    }
}
