using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateCards {

    public static Dictionary<string, Material> backs = new Dictionary<string, Material>();


    static void init() {
        backs["D"] = Resources.Load<Material>("Images/Cards/Back/DoorBack");
        backs["T"] = Resources.Load<Material>("Images/Cards/Back/TreasureBack");
        Debug.Log(backs);
    }

    [MenuItem("Assets/Generate Cards")]
    static void main() {
        init();

        GameObject prefab = GameObject.Find("CardPrefab");
        GameObject parent = GameObject.Find("Cards");

        GameObject obj = null;
        using(var reader = new StreamReader(@".\Assets\Data\Cards.csv")) {

            int i=0;
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');
                
                obj = GameObject.Instantiate(prefab);
                obj.transform.parent = parent.transform;
                obj.transform.position = new Vector3();
                obj.gameObject.name = values[3];
                obj.SetActive(false);
                if (values[0] == "D") {
                    obj.transform.Find("Front").GetComponent<Renderer>().material = Resources.Load<Material>("Images/Cards/Door/" + values[3]);
                    obj.transform.Find("Back").GetComponent<Renderer>().material = backs["D"];
                } else {
                    obj.transform.Find("Front").GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Images/Cards/Treasure/" + values[3]);
                    obj.transform.Find("Back").GetComponent<Renderer>().sharedMaterial = backs["T"];
                }
                ++i;
            }
        }
    }
}
