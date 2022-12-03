using UnityEditor;

[CustomEditor(typeof(MapSegment))]
public class MapSegmentEditor : Editor
{
    MapSegment segment;
    private void OnEnable() {
        segment = (MapSegment)target;
    }

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();


        for (int d = 0; d < 4; d++)
            segment.socket.sockets[d] = (Socket)EditorGUILayout.ObjectField(((Direction)d).ToString(), segment.socket.sockets[d], typeof(Socket), false);

        EditorGUILayout.Space();

        segment.weight = EditorGUILayout.Slider("Weight", segment.weight, 0.0f, 1.0f);
        segment.flippable = EditorGUILayout.Toggle("Flippable", segment.flippable);


        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(segment);
        }
    }
}
