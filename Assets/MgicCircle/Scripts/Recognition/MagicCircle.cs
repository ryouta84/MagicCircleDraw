using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour {
    private Renderer magicCanvasRenderer;
    // 正解の図形を2次元配列表現したデータ
    private short[][] template;
    public short[][] Template { get => template; private set => template = value; }

    // 正解の閾値
    [SerializeField]
    private int successThreshold = 0;
    public int SuccessThreshold { get => successThreshold; private set => successThreshold = value; }

    void Start() {
        this.magicCanvasRenderer = this.gameObject.GetComponent<Renderer>();
        var canvasTexture = (Texture2D)this.magicCanvasRenderer.material.mainTexture; // 複製されているので自分で破棄しないとメモリリークする
        this.GenerateShape2D(canvasTexture.GetPixels(), canvasTexture.height, canvasTexture.width);

        DebugLogger.Dump2D(this.Template);
    }

    void Update() {

    }

    // マッチング対象の形を2次元配列で表現したデータを作る。人間が出力した値を見ると上下反対にみえる。
    void GenerateShape2D(Color[] colors, int height, int width) {
        var result = new short[height][];
        for (int y = 0; y < result.Length; y++) {
            result[y] = new short[width];
            for (int x = 0; x < width; x++) {
                int index = y * height + x;
                if (colors[index].r == Color.black.r && colors[index].g == Color.black.g && colors[index].b == Color.black.b && colors[index].a == Color.black.a) {
                    result[y][x] = 1;
                }
                else {
                    result[y][x] = 0;
                }
            }
        }

        this.Template = result;
    }
}
