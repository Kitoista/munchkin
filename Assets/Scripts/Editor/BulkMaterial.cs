using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class BulkMaterial : ScriptableWizard {

     public Shader Shader;

    [MenuItem("Assets/Bulk Material Creator")]
    static void main() {
        ScriptableWizard.DisplayWizard("Bulk Material Creator",typeof(BulkMaterial));
    }
    
    void OnWizardUpdate() {
    }

    void OnWizardCreate() {
        foreach (var obj in Selection.objects) {
            if (obj.GetType() == typeof(Texture2D)) {
                var texture = (Texture2D)obj;
                var material = GenerateMaterial(texture);
                var path = GetDirectory(obj) + "/" + material.name + ".mat";
                AssetDatabase.CreateAsset(material, path);
            }
        }
    }

    private Material GenerateMaterial(Texture2D texture) {
        var material = new Material(Shader);
        material.name = texture.name;
        material.mainTexture = texture;
        return material;
    }

    private string GetDirectory(Object obj) {
        var path = AssetDatabase.GetAssetPath(obj);
        if (path.Contains("/")) {
        path = path.Substring(0, path.LastIndexOf('/'));
        }
        return path;
    }

}
