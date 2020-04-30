using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Draw : MonoBehaviour {
    private const float INTERVAL_TIME = 0.005f;
    private const int RAY_DISTANCE = 3;

    [System.NonSerialized]
    private float elapsedTime;
    // ユーザが描いた描画領域のポイントを保存する。テクスチャのサイズで初期化される。
    private short[][] drewPos2D;
    public short[][] DrewPos2D { get => drewPos2D; private set => drewPos2D = value; }

    [SerializeField]
    private GameObject magicCircleCanvas; // MagicCircleCavasインスタンスをセットする
    private Renderer magicCanvasRenderer;
    private Texture2D drawTexture;
    private Color[] drawPixels;
    private int brushSize = 0;
    private int magicCanvasID = 0;


    void Start() {
        this.SetTargetMagicCircle(this.magicCircleCanvas);
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        // 最短でもINTERVAL_TIME間隔でしか座標を記録しない。
        if (elapsedTime >= INTERVAL_TIME) {
            if (this.NeedCreateRay()) {
                var ray = this.CreateRay();
                RaycastHit hit;
                // MagicCircleCanvas == 8
                int layerMask = LayerMask.GetMask(new string[] { LayerMask.LayerToName(8) });
                if (Physics.Raycast(ray, out hit, RAY_DISTANCE, layerMask) && hit.collider.gameObject.GetInstanceID() == this.magicCanvasID) {
                    // テクスチャの座標は左下がy=0で配列は0行目が左上なので上下を反転させて格納する。
                    //int posY = (this.drawTexture.height - 1) - Mathf.FloorToInt(hit.textureCoord.y * this.drawTexture.height);
                    //int posX = (Mathf.FloorToInt(hit.textureCoord.x * this.drawTexture.width));
                    int posY = Mathf.FloorToInt(hit.textureCoord.y * this.drawTexture.height);
                    int posX = Mathf.FloorToInt(hit.textureCoord.x * this.drawTexture.width);
                    this.DrewPos2D[posY][posX] = 1;

                    var hitPoint = new Vector2(hit.textureCoord.x * this.drawTexture.width, hit.textureCoord.y * this.drawTexture.height);
                    this.drawPoint(hitPoint);
                    this.updateTexture();

                    //DebugLogger.Log(Mathf.CeilToInt(hit.textureCoord.y * this.drawTexture.height).ToString() + " : " + Mathf.CeilToInt(hit.textureCoord.x * this.drawTexture.width).ToString());
                }

                elapsedTime = 0;
            }
        }

        // デバッグ用
        if (Input.GetKeyDown(KeyCode.A)) {
            var pm = new PatternMatching(this.magicCircleCanvas.GetComponent<MagicCircle>(), this.drewPos2D);
            if (pm.IsMatch()) {
                DebugLogger.Log("成功！");
            }
            else {
                DebugLogger.Log("残念");
            }
        }

        // デバッグ用
        if (Input.GetKeyDown(KeyCode.D)) {
            DebugLogger.Dump2D(this.DrewPos2D);
        }
    }

    // brushSize分のピクセルを塗りつぶす
    void drawPoint(Vector2 hitPoint) {
        for (int x = (int)(hitPoint.x - this.brushSize / 2); x < hitPoint.x + this.brushSize; x++) {
            for (int y = (int)(hitPoint.y - this.brushSize / 2); y < hitPoint.y + this.brushSize; y++) {
                if (x >= 0 && y >= 0) {
                    this.drawPixels.SetValue(Color.green, (int)x + this.drawTexture.width * (int)y);
                }
            }
        }
    }

    // 描いた結果のテクスチャを反映
    void updateTexture() {
        this.drawTexture.SetPixels(this.drawPixels);
        this.drawTexture.Apply();
        this.magicCanvasRenderer.material.mainTexture = this.drawTexture;
    }

    // TODO: 最終的にはVR対応する
    bool NeedCreateRay() {
        if (Input.GetMouseButton(0)) {
            return true;
        }

        return false;
    }

    // TODO: 最終的にはVR対応する
    Ray CreateRay() {
        // Screenのマウスの位置から空間に向けてレイ(光線)を放つ
        var result = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(result.origin, result.direction * RAY_DISTANCE, Color.red, 20, false);
        return result;
    }

    void SetTargetMagicCircle(GameObject magicCircleCanvas) {
        this.magicCanvasID = this.magicCircleCanvas.GetInstanceID();

        // 塗りつぶした場所を表示する用テクスチャをセットする
        this.magicCanvasRenderer = this.magicCircleCanvas.GetComponent<Renderer>();
        var canvasTexture = (Texture2D)this.magicCanvasRenderer.material.mainTexture; // 複製されているので自分で破棄しないとリークする
        DebugLogger.Log(canvasTexture.width.ToString() + " : " + canvasTexture.height.ToString());
        var magicCanvasPixels = canvasTexture.GetPixels();
        this.drawPixels = new Color[magicCanvasPixels.Length];
        magicCanvasPixels.CopyTo(this.drawPixels, 0);
        this.drawTexture = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGBA32, false);
        this.drawTexture.filterMode = FilterMode.Point;
        this.drawTexture.SetPixels(this.drawPixels);
        this.drawTexture.Apply();
        this.magicCanvasRenderer.material.mainTexture = this.drawTexture;

        this.DrewPos2D = new short[canvasTexture.height][];
        for (int i = 0; i < this.DrewPos2D.Length; i++) {
            this.DrewPos2D[i] = new short[canvasTexture.width];
        }
    }
}