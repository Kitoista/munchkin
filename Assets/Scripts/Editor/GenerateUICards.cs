using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class GenerateUICards {

    [MenuItem("Assets/Generate UI Cards")]
    static void main() {
        GameObject prefab = GameObject.Find("UICardPrefab");
        GameObject parent = GameObject.Find("UICards");

        GameObject obj = null;
        using(var reader = new StreamReader(@".\Assets\Data\Cards.csv")) {

            int i=0;
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');
                
                obj = GameObject.Instantiate(prefab);
                obj.transform.SetParent(parent.transform);
                obj.transform.position = new Vector3();
                obj.gameObject.name = "UI_" + values[3];
                obj.SetActive(false);
                if (values[0] == "D") {
                    obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Cards/Door/UI/" + values[3]);
                } else {
                    obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Cards/Treasure/UI/" + values[3]);
                }
                ++i;
            }
        }
    }
}
