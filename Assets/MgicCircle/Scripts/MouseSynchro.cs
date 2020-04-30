﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSynchro : MonoBehaviour {

    void Start() {

    }

    void Update() {
        Vector3 mousePos = Input.mousePosition;
        // カメラの奥行きが変わっても同じ大きさで表示するために変換前に定数を設定する。
        mousePos.z = 10f;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
}
