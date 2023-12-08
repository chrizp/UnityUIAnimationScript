using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AnimateObject : MonoBehaviour
{
    [Header("Animation Settings:")]
    //loop the animation forever or not.
    public bool loop = false;
    //if the animation should play each time the game object is enabled.
    public bool animateOnEnable = false;
    //period in seconds, putting at 0.5 means it will animate in half a second and so on.
    public float animationPeriod = 1;

    //scale variables
    [HideInInspector] public bool animateScale = false;
    [HideInInspector] public bool linkedScale = false;
    [HideInInspector] public AnimationCurve xScale = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
    [HideInInspector] public AnimationCurve yScale = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

    //position variables
    [HideInInspector] public bool animatePos = false;
    [HideInInspector] public AnimationCurve xPos = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
    [HideInInspector] public AnimationCurve yPos = new AnimationCurve(new Keyframe(0f, 0), new Keyframe(1f, 0f));

    //rotation variables
    [HideInInspector] public bool animateRotation = false;
    [HideInInspector] public AnimationCurve zRot = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));

    //color variables
    [HideInInspector] public bool animateColor = false;
    [HideInInspector] public Gradient colorGradient = new Gradient();
    [HideInInspector] public Graphic colorObj;

    private float time = 0;
    private Vector2 defaultPos;

    private void Awake()
    {
        if (colorObj == null) colorObj = gameObject.GetComponent<Graphic>();
        defaultPos = transform.localPosition;
    }

    private void OnEnable()
    {
        if (animateOnEnable) Animate();
    }

    //call this method to animate object.
    public virtual void Animate()
    {
        if (!gameObject.activeInHierarchy) return;

        time = 0;
        StopAllCoroutines();
        StartCoroutine(AnimationLoop());
    }

    private IEnumerator AnimationLoop()
    {
        while (true)
        {
            //increase time which is used in animating.
            time += Time.deltaTime / animationPeriod;

            //scale anim
            if (animateScale)
            {
                if (linkedScale) transform.localScale = Vector2.one * xScale.Evaluate(time);
                else transform.localScale = (Vector2.right * xScale.Evaluate(time)) + (Vector2.up * yScale.Evaluate(time));
            }

            //pos anim
            if (animatePos) transform.localPosition = defaultPos + ((Vector2.right * xPos.Evaluate(time)) + (Vector2.up * yPos.Evaluate(time)));

            //rotation anim
            if (animateRotation) transform.rotation = Quaternion.Euler(0, 0, zRot.Evaluate(time));

            //color anim
            if (animateColor) colorObj.color = colorGradient.Evaluate(time);

            if (time >= 1)
            {
                if (loop) time = 0;
                else yield break;
            }

            yield return null;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimateObject))]
public class AnimateObjectEditor : Editor
{
    SerializedProperty animateScale;
    SerializedProperty linkedScale;
    SerializedProperty xScale;
    SerializedProperty yScale;

    SerializedProperty animatePos;
    SerializedProperty xPos;
    SerializedProperty yPos;

    SerializedProperty animateRotation;
    SerializedProperty zRot;

    SerializedProperty animateColor;
    SerializedProperty colorGradient;

    private void OnEnable()
    {
        // Initialize SerializedProperties
        animateScale = serializedObject.FindProperty("animateScale");
        linkedScale = serializedObject.FindProperty("linkedScale");
        xScale = serializedObject.FindProperty("xScale");
        yScale = serializedObject.FindProperty("yScale");

        animatePos = serializedObject.FindProperty("animatePos");
        xPos = serializedObject.FindProperty("xPos");
        yPos = serializedObject.FindProperty("yPos");

        animateRotation = serializedObject.FindProperty("animateRotation");
        zRot = serializedObject.FindProperty("zRot");

        animateColor = serializedObject.FindProperty("animateColor");
        colorGradient = serializedObject.FindProperty("colorGradient");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.Space(20);

        //animate scaling?
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.PropertyField(animateScale, new GUIContent("Animate Scale:"));
        if (animateScale.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(linkedScale, new GUIContent("Link X/Y scale:"));
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(xScale, new GUIContent("X Scale"));
            if (!linkedScale.boolValue) EditorGUILayout.PropertyField(yScale, new GUIContent("Y Scale"));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(20);

        //animate pos?
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.PropertyField(animatePos, new GUIContent("Animate Position:"));
        if (animatePos.boolValue)
        {
            EditorGUILayout.PropertyField(xPos, new GUIContent("X Pos"));
            EditorGUILayout.PropertyField(yPos, new GUIContent("Y Pos"));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(20);

        //animate rotation?
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.PropertyField(animateRotation, new GUIContent("Animate Rotation:"));
        if (animateRotation.boolValue)
        {
            EditorGUILayout.PropertyField(zRot, new GUIContent("Z Rot"));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(20);

        //animate rotation?
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.PropertyField(animateColor, new GUIContent("Animate Color:"));
        if (animateColor.boolValue)
        {
            EditorGUILayout.PropertyField(colorGradient, new GUIContent("Color Gradient:"));
        }
        EditorGUILayout.EndVertical();

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
