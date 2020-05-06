1. 任意のサイズの図形画像を用意する。図形は黒で背景は透明にする。
2. Materials フォルダにimport する。
3. Alpah is transparency と Read/Write　Enabled をオンにする。
4. マテリアルを作成しAlbedoに指定する。ShaderをUnlit/Transparentにする。
5. MagicCircleCanvasプレハブをインスタンス化してMeshRendererのElements0に設定する。
6. Drawコンポーネント の currentMagicCircleに設定すると使える。