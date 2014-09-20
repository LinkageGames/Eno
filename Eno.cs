//-------------------------------
//            Eno
// Released under GNU GPL v3.0
// Developed by Linkage Games
// Developed for u/miraoister
//-------------------------------


using UnityEngine;
using System.Collections;
using UnityEditor;

public class Eno : EditorWindow {
    private enum measurementStates { first, second, showing, none };
    private measurementStates measurementState = measurementStates.first;
    private Vector3 first = Vector3.zero;
    
    private Vector3 second = Vector3.zero;
    

    [MenuItem("Window/Eno")]
    static void Init() {
        Eno window = (Eno)EditorWindow.GetWindow(typeof(Eno));
        window.minSize = new Vector2(215f, 22f);
        window.maxSize = new Vector2(1000f, 222f);
        window.position = new Rect(window.position.x,window.position.y, 215f,22f);
    }
    bool test;
    void OnGUI() {
        
        GUILayout.BeginHorizontal();
        
        if (measurementState.Equals(measurementStates.none)) {
            if (GUILayout.Button("Measure")) {
                measurementState = measurementStates.first;
            }
        }
        else if (measurementState.Equals(measurementStates.first)) {
            GUILayout.Label("Pick first point");
        }
        else if (measurementState.Equals(measurementStates.second)) {
            GUILayout.Label("Pick last point");
        }
        else if (measurementState.Equals(measurementStates.showing)) {
            GUILayout.Label(Vector3.Distance(first,second).ToString());
        }
        if (GUILayout.Button("Restart")) {
            Restart();
        }
        GUILayout.EndHorizontal();
        
    }

    void OnEnable() {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    public void OnSceneGUI(SceneView sceneView) {
       
        
        if (measurementState.Equals(measurementStates.first) && DropPoint(out first)) {
            
            measurementState = measurementStates.second;

        }
        else if (measurementState.Equals(measurementStates.first)) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else if (measurementState.Equals(measurementStates.second) && DropPoint(out second)) {
            measurementState = measurementStates.showing;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }else if (measurementState.Equals(measurementStates.second)){
            CheckCancel();
            Handles.SphereCap(42, first, Quaternion.identity, 0.05f);
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current .mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, 100f)) {
                
                Handles.DrawLine(first, hit.point);
                Handles.SphereCap(42, hit.point, Quaternion.identity, 0.05f);
            }
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else if (measurementState.Equals(measurementStates.showing)) {
            CheckCancel();
            HandleUtility.Repaint();
            Handles.DrawLine(first, second);
            Handles.SphereCap(42, first, Quaternion.identity, 0.05f);
            Handles.SphereCap(42, second, Quaternion.identity, 0.05f);
            if (Event.current != null && (Event.current.button == 0) && Event.current.type.Equals(EventType.mouseDown)) {
                Restart();
            }
            this.Repaint();
        }
        else {
            HandleUtility.Repaint();
        }
        SceneView.RepaintAll();
    }


    private bool DropPoint(out Vector3 point) {
        point = Vector3.zero;
        Event e = Event.current;
        

        if (e.button == 0 && e.type.Equals(EventType.mouseDown)) {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, 100f)) {
                
                point = hit.point;
                return true;
            }

        }
        return false;
    }

    private void Restart() {
        
        measurementState = measurementStates.none;
        first = Vector3.zero;
        second = Vector3.zero;
    }

    private void CheckCancel() {
        Event e = Event.current;
        if (e.keyCode == KeyCode.Escape && e.type.Equals(EventType.keyDown)) {
            Restart();
            this.Repaint();
        }
    }
}
