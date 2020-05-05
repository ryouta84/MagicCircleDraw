using UnityEngine;

public class Draw : MonoBehaviour {
    private const int RAY_DISTANCE = 3;

    // ユーザが描いた描画領域のポイントを保存する。テクスチャのサイズで初期化される。
    private short[][] drewPos2D;
    public short[][] DrewPos2D { get => drewPos2D; private set => drewPos2D = value; }

    [SerializeField]
    private GameObject currentMagicCircleCanvas; // MagicCircleCavasインスタンスをセットする
    [SerializeField]
    private GameObject OtherMagicCircleCanvas; // 切り替えるMagicCircleCavasプレハブをセットする
    private Renderer magicCanvasRenderer;
    private Texture2D drawTexture;
    private Color[] drawPixels;
    private int brushSize = 0;
    private int magicCanvasID = 0;


    void Start() {
        this.SetTargetMagicCircleCanvas(this.currentMagicCircleCanvas);
    }

    void Update() {
        this.DrawMagicCircle();

        // デバッグ用
        if (Input.GetKeyDown(KeyCode.A)) {
            var pm = new PatternMatching(this.currentMagicCircleCanvas.GetComponent<MagicCircle>(), this.drewPos2D);
            if (pm.IsMatch()) {
                DebugLogger.Log("成功！");
            }
            else {
                DebugLogger.Log("残念");
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            // 対象の魔法陣を別のものに切り替える。
            this.SwitchMagicCircleCanvas(this.OtherMagicCircleCanvas);
            return;
        }

        // デバッグ用
        if (Input.GetKeyDown(KeyCode.D)) {
            DebugLogger.Dump2D(this.DrewPos2D);
            return;
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
    virtual protected bool NeedCreateRay() {
        if (Input.GetMouseButton(0)) {
            return true;
        }

        return false;
    }

    // TODO: 最終的にはVR対応する
    virtual protected Ray CreateRay() {
        // Screenのマウスの位置から空間に向けてレイ(光線)を放つ
        var result = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(result.origin, result.direction * RAY_DISTANCE, Color.red, 20, false);
        return result;
    }

    void DrawMagicCircle() {
        if (this.NeedCreateRay()) {
            var ray = this.CreateRay();
            RaycastHit hit;
            // MagicCircleCanvas == 8
            int layerMask = LayerMask.GetMask(new string[] { LayerMask.LayerToName(8) });
            // FIXME: 別の魔法陣の後ろに生成すると手前の魔法陣が邪魔になって生成した方に線がかけないのでRaycastAllを使うように修正する。
            if (Physics.Raycast(ray, out hit, RAY_DISTANCE, layerMask) && hit.collider.gameObject.GetInstanceID() == this.magicCanvasID) {
                int posY = Mathf.FloorToInt(hit.textureCoord.y * this.drawTexture.height);
                int posX = Mathf.FloorToInt(hit.textureCoord.x * this.drawTexture.width);
                this.DrewPos2D[posY][posX] = 1;

                var hitPoint = new Vector2(hit.textureCoord.x * this.drawTexture.width, hit.textureCoord.y * this.drawTexture.height);
                this.drawPoint(hitPoint);
                this.updateTexture();
            }
        }
    }

    // 現在の魔法陣を削除して別の魔法陣を生成する。
    public void SwitchMagicCircleCanvas(GameObject target) {
        Destroy(this.currentMagicCircleCanvas);
        var instance = Instantiate(target, new Vector3(0, 0, 0), Quaternion.identity);
        this.SetTargetMagicCircleCanvas(instance);
    }

    // 描く魔法陣を切り替える
    void SetTargetMagicCircleCanvas(GameObject targetMagicCircleCanvas) {
        this.magicCanvasID = targetMagicCircleCanvas.GetInstanceID();

        // 塗りつぶした場所を表示する用テクスチャをセットする
        this.magicCanvasRenderer = targetMagicCircleCanvas.GetComponent<Renderer>();
        // FIXME: 複製されているので自分で破棄しないとメモリリークする
        var canvasTexture = (Texture2D)this.magicCanvasRenderer.material.mainTexture;
        DebugLogger.Log(canvasTexture.width.ToString() + " : " + canvasTexture.height.ToString());
        var magicCanvasPixels = canvasTexture.GetPixels();
        this.drawPixels = new Color[magicCanvasPixels.Length];
        magicCanvasPixels.CopyTo(this.drawPixels, 0);
        this.drawTexture = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGBA32, false);
        this.drawTexture.filterMode = FilterMode.Point;
        this.drawTexture.SetPixels(this.drawPixels);
        this.drawTexture.Apply();
        this.magicCanvasRenderer.material.mainTexture = this.drawTexture;

        // ユーザが魔法陣をなぞった跡を保存する配列を作る
        this.DrewPos2D = new short[canvasTexture.height][];
        for (int i = 0; i < this.DrewPos2D.Length; i++) {
            this.DrewPos2D[i] = new short[canvasTexture.width];
        }

        this.currentMagicCircleCanvas = targetMagicCircleCanvas;
    }
}