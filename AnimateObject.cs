using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimateObject : MonoBehaviour, IPointerClickHandler
{
    [Header("Animation Settings:")]
    //should the animation loop forever or not.
    public bool loop = false;
    //should the animation play each time the game object is enabled.
    public bool animateOnEnable = false;
    //should the gameObject be disabled after completing an animation cycle.
    public bool disableOnCompletion = false;
    //should the animation play each time the player clicks the ui.
    public bool animateOnClick = false;
    //lifetime in seconds for a complete animation cycle.
    public float animationLifetime = 1;

    //the transform that will be animated with pos and scale.
    //automatically referenced in awake if not assigned.
    [SerializeField] public Transform animatedTransform;

    //the color graphics object that will be animated with color.
    //automatically referenced in awake if not assigned.
    [SerializeField] public Graphic animatedGraphic;

    //scale variables
    [SerializeField][HideInInspector] private bool animateScale = false;
    [SerializeField][HideInInspector] private bool linkedScale = false;
    [SerializeField][HideInInspector] private AnimationCurve xScale = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
    [SerializeField][HideInInspector] private AnimationCurve yScale = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

    //position variables
    [SerializeField][HideInInspector] private bool animatePos = false;
    [SerializeField][HideInInspector] public AnimationCurve xPos = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
    [SerializeField][HideInInspector] public AnimationCurve yPos = new AnimationCurve(new Keyframe(0f, 0), new Keyframe(1f, 0f));

    //rotation variables
    [SerializeField][HideInInspector] private bool animateRotation = false;
    [SerializeField][HideInInspector] private AnimationCurve zRot = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));

    //color variables
    [SerializeField][HideInInspector] private bool animateColor = false;
    [SerializeField][HideInInspector] private Gradient colorGradient = new Gradient();

    private float time = 0;
    private Vector2 defaultPos;

    private void Awake()
    {
        if (animatedTransform == null) animatedTransform = transform;
        if (animatedGraphic == null) animatedGraphic = gameObject.GetComponent<Graphic>();   
        SetDefaultPos(transform.localPosition);
    }

    public void SetDefaultPos(Vector2 pos)
    {
        defaultPos = pos;
    }

    private void OnEnable()
    {
        if (animateOnEnable) Animate();
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (animateOnClick) Animate();
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
            time += Time.deltaTime / animationLifetime;

            //scale anim
            if (animateScale)
            {
                if (linkedScale) animatedTransform.localScale = Vector2.one * xScale.Evaluate(time);
                else animatedTransform.localScale = (Vector2.right * xScale.Evaluate(time)) + (Vector2.up * yScale.Evaluate(time));
            }

            //pos anim
            if (animatePos) animatedTransform.localPosition = defaultPos + ((Vector2.right * xPos.Evaluate(time)) + (Vector2.up * yPos.Evaluate(time)));

            //rotation anim
            if (animateRotation) animatedTransform.rotation = Quaternion.Euler(0, 0, zRot.Evaluate(time));

            //color anim
            if (animateColor) animatedGraphic.color = colorGradient.Evaluate(time);

            if (time >= 1)
            {
                if (loop) time = 0;
                else if (disableOnCompletion) gameObject.SetActive(false);
                else yield break;
            }

            yield return null;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimateObject))]
[CanEditMultipleObjects]
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
