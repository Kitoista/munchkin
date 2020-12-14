using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureRepeater : MonoBehaviour
{

    public bool horizontalRepeat = true;
    public bool verticalRepeat = false;

    public bool bottomOffset = true;

    Renderer rend;
    Vector3 lastScale;
    Vector2 lastTextureScale;

    void Start() {
        rend = GetComponent<Renderer>();
        lastScale = transform.localScale;
        lastTextureScale = rend.material.mainTextureScale;
    }

    void Update() {
        if (lastScale.Equals(transform.localScale)) {
            return;
        }
        Vector3 scale = transform.localScale;
        Vector2 textureScale = rend.material.mainTextureScale;
        if (verticalRepeat) {
            textureScale.x = scale.x * lastTextureScale.x / lastScale.x;
        }
        if (horizontalRepeat) {
            textureScale.y = scale.y * lastTextureScale.y / lastScale.y;
        }
        if (bottomOffset) {
            rend.material.mainTextureScale = textureScale;
            Vector2 offset = rend.material.mainTextureOffset;
            offset.x -= textureScale.x - lastTextureScale.x;
            offset.y -= textureScale.y - lastTextureScale.y;

            rend.material.mainTextureOffset = offset;
        }
        
        lastTextureScale = textureScale;
        lastScale = scale;
        
    }
}
